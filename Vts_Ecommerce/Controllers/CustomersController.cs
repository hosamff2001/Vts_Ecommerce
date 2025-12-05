using Microsoft.AspNetCore.Mvc;
using System;
using Vts_Ecommerce.DAL.Repositories;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.Controllers
{
    public class CustomersController : AuthorizedController
    {
        private readonly CustomerRepository _customerRepo = new CustomerRepository();

        public IActionResult Index()
        {
            var customers = _customerRepo.GetAll();
            return View(customers);
        }

        public IActionResult Details(int id)
        {
            var model = _customerRepo.GetById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        public IActionResult Create()
        {
            return View(new Customer { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer model)
        {
            // Trim whitespace
            if (!string.IsNullOrWhiteSpace(model.Name))
                model.Name = model.Name.Trim();
            if (!string.IsNullOrWhiteSpace(model.Email))
                model.Email = model.Email.Trim();
            if (!string.IsNullOrWhiteSpace(model.Phone))
                model.Phone = model.Phone.Trim();
            if (!string.IsNullOrWhiteSpace(model.Address))
                model.Address = model.Address.Trim();
            if (!string.IsNullOrWhiteSpace(model.Notes))
                model.Notes = model.Notes.Trim();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                int newId = _customerRepo.Create(model);
                if (newId > 0)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Failed to create customer. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating customer: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Edit(int id)
        {
            var model = _customerRepo.GetById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer model)
        {
            if (id != model.Id)
                return BadRequest("ID mismatch");

            // Trim whitespace
            if (!string.IsNullOrWhiteSpace(model.Name))
                model.Name = model.Name.Trim();
            if (!string.IsNullOrWhiteSpace(model.Email))
                model.Email = model.Email.Trim();
            if (!string.IsNullOrWhiteSpace(model.Phone))
                model.Phone = model.Phone.Trim();
            if (!string.IsNullOrWhiteSpace(model.Address))
                model.Address = model.Address.Trim();
            if (!string.IsNullOrWhiteSpace(model.Notes))
                model.Notes = model.Notes.Trim();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                bool success = _customerRepo.Update(model);
                if (success)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Failed to update customer. Please try again.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating customer: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Delete(int id)
        {
            var model = _customerRepo.GetById(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                bool success = _customerRepo.Delete(id);
                if (success)
                    return RedirectToAction(nameof(Index));

                var model = _customerRepo.GetById(id);
                ModelState.AddModelError(string.Empty, "Failed to delete customer. Please try again.");
                return View("Delete", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting customer: {ex.Message}");
                var model = _customerRepo.GetById(id);
                return View("Delete", model);
            }
        }
    }
}
