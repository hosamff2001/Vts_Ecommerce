using Microsoft.AspNetCore.Mvc;
using System;
using Vts_Ecommerce.DAL.Repositories;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.Controllers
{
    public class CategoriesController : AuthorizedController
    {
        private readonly CategoryRepository _repo = new CategoryRepository();

        public IActionResult Index()
        {
            var list = _repo.GetAll();
            return View(list);
        }

        public IActionResult Details(int id)
        {
            var model = _repo.GetById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        public IActionResult Create()
        {
            return View(new Category { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category model)
        {
            // Trim whitespace
            if (!string.IsNullOrWhiteSpace(model.Name))
                model.Name = model.Name.Trim();
            if (!string.IsNullOrWhiteSpace(model.Description))
                model.Description = model.Description.Trim();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                int newId = _repo.Create(model);
                if (newId > 0)
                    return RedirectToAction(nameof(Index));
                
                ModelState.AddModelError(string.Empty, "Failed to create category. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating category: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            var model = _repo.GetById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category model)
        {
            if (id != model.Id)
                return BadRequest("ID mismatch");

            // Trim whitespace
            if (!string.IsNullOrWhiteSpace(model.Name))
                model.Name = model.Name.Trim();
            if (!string.IsNullOrWhiteSpace(model.Description))
                model.Description = model.Description.Trim();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                bool success = _repo.Update(model);
                if (success)
                    return RedirectToAction(nameof(Index));
                
                ModelState.AddModelError(string.Empty, "Failed to update category. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating category: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Delete(int id)
        {
            var model = _repo.GetById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                bool success = _repo.Delete(id);
                if (success)
                    return RedirectToAction(nameof(Index));
                
                // If delete failed, show the delete view with error
                var model = _repo.GetById(id);
                ModelState.AddModelError(string.Empty, "Failed to delete category. Please try again.");
                return View("Delete", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting category: {ex.Message}");
                var model = _repo.GetById(id);
                return View("Delete", model);
            }
        }
    }
}
