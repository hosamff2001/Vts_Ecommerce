using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Vts_Ecommerce.DAL.Repositories;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.Controllers
{
    public class ProductsController : AuthorizedController
    {
        private readonly ProductRepository _productRepo = new ProductRepository();
        private readonly CategoryRepository _categoryRepo = new CategoryRepository();

        public IActionResult Index()
        {
            var products = _productRepo.GetAll();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var model = _productRepo.GetById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _categoryRepo.GetAll();
            return View(new Product { IsActive = true, CostPrice = 0, SellingPrice = 0, StockQuantity = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product model)
        {
            // Trim whitespace
            if (!string.IsNullOrWhiteSpace(model.Name))
                model.Name = model.Name.Trim();
            if (!string.IsNullOrWhiteSpace(model.Description))
                model.Description = model.Description.Trim();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryRepo.GetAll();
                return View(model);
            }

            try
            {
                int newId = _productRepo.Create(model);
                if (newId > 0)
                    return RedirectToAction(nameof(Index));

                ViewBag.Categories = _categoryRepo.GetAll();
                ModelState.AddModelError(string.Empty, "Failed to create product. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Categories = _categoryRepo.GetAll();
                ModelState.AddModelError(string.Empty, $"Error creating product: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            var model = _productRepo.GetById(id);
            if (model == null) return NotFound();
            ViewBag.Categories = _categoryRepo.GetAll();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product model)
        {
            if (id != model.Id)
                return BadRequest("ID mismatch");

            // Trim whitespace
            if (!string.IsNullOrWhiteSpace(model.Name))
                model.Name = model.Name.Trim();
            if (!string.IsNullOrWhiteSpace(model.Description))
                model.Description = model.Description.Trim();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryRepo.GetAll();
                return View(model);
            }

            try
            {
                bool success = _productRepo.Update(model);
                if (success)
                    return RedirectToAction(nameof(Index));

                ViewBag.Categories = _categoryRepo.GetAll();
                ModelState.AddModelError(string.Empty, "Failed to update product. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Categories = _categoryRepo.GetAll();
                ModelState.AddModelError(string.Empty, $"Error updating product: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Delete(int id)
        {
            var model = _productRepo.GetById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                bool success = _productRepo.Delete(id);
                if (success)
                    return RedirectToAction(nameof(Index));

                var model = _productRepo.GetById(id);
                ModelState.AddModelError(string.Empty, "Failed to delete product. Please try again.");
                return View("Delete", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting product: {ex.Message}");
                var model = _productRepo.GetById(id);
                return View("Delete", model);
            }
        }

        // AJAX endpoint for cascading dropdown
        [HttpGet]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            if (categoryId <= 0)
                return Json(new List<Product>());

            var products = _productRepo.GetByCategory(categoryId);
            return Json(products);
        }
    }
}
