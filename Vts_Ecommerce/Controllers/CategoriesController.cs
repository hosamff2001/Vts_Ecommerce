using Microsoft.AspNetCore.Mvc;
using System;
using Vts_Ecommerce.DAL.Repositories;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.Controllers
{
    public class CategoriesController : Controller
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
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _repo.Create(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error creating category: " + ex.Message);
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
        public IActionResult Edit(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _repo.Update(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error updating category: " + ex.Message);
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
                _repo.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error deleting category: " + ex.Message);
                var model = _repo.GetById(id);
                return View("Delete", model);
            }
        }
    }
}
