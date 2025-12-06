using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Vts_Ecommerce.DAL.Repositories;
using Vts_Ecommerce.Models;
using Vts_Ecommerce.Helpers;

namespace Vts_Ecommerce.Controllers
{
    public class SalesInvoiceController : AuthorizedController
    {
        private readonly InvoiceRepository _invoiceRepo = new InvoiceRepository();
        private readonly InvoiceLineRepository _invoiceLineRepo = new InvoiceLineRepository();
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly ProductRepository _productRepo = new ProductRepository();

        public IActionResult Index()
        {
            var invoices = _invoiceRepo.GetAll().OrderByDescending(i => i.InvoiceDate).ToList();
            return View(invoices);
        }

        public IActionResult Details(int id)
        {
            var invoice = _invoiceRepo.GetById(id);
            if (invoice == null) return NotFound();

            var lineItems = _invoiceLineRepo.GetByInvoiceId(id);
            ViewBag.LineItems = lineItems;

            return View(invoice);
        }

        public IActionResult Create()
        {
            ViewBag.Customers = _customerRepo.GetAll().Where(c => c.IsActive).ToList();
            ViewBag.Products = _productRepo.GetAll().Where(p => p.IsActive).ToList();

            var model = new SalesInvoice
            {
                InvoiceDate = DateTime.Now,
                DiscountAmount = 0,
                TaxAmount = 0
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SalesInvoice model, string lineItemsJson)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = _customerRepo.GetAll().Where(c => c.IsActive).ToList();
                ViewBag.Products = _productRepo.GetAll().Where(p => p.IsActive).ToList();
                return View(model);
            }

            try
            {
                // Parse line items from JSON (sent from frontend)
                var lineItems = ParseLineItems(lineItemsJson);

                if (!lineItems.Any())
                {
                    ViewBag.Customers = _customerRepo.GetAll().Where(c => c.IsActive).ToList();
                    ViewBag.Products = _productRepo.GetAll().Where(p => p.IsActive).ToList();
                    ModelState.AddModelError(string.Empty, "Invoice must have at least one line item.");
                    return View(model);
                }

                // Calculate totals
                decimal itemDiscountTotal = lineItems.Sum(li => li.ItemDiscount);
                decimal subtotal = lineItems.Sum(li => li.Quantity * li.UnitPrice);
                decimal invoiceDiscount = model.DiscountAmount;
                decimal tax = model.TaxAmount;

                // Set model properties to match database schema
                model.InvoiceNumber = "INV-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                model.SubTotal = subtotal;
                model.ItemDiscount = itemDiscountTotal;
                model.InvoiceDiscount = invoiceDiscount;
                model.Total = subtotal + tax - invoiceDiscount;
                model.CreatedBy = SessionManager.GetCurrentUserId(HttpContext) ?? 1; // Default to 1 if user not found
                model.CreatedDate = DateTime.Now;

                // Create invoice with line items
                _invoiceRepo.CreateInvoiceWithItems(model, lineItems);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Customers = _customerRepo.GetAll().Where(c => c.IsActive).ToList();
                ViewBag.Products = _productRepo.GetAll().Where(p => p.IsActive).ToList();
                ModelState.AddModelError(string.Empty, "Error creating invoice: " + ex.Message);
                return View(model);
            }
        }



        private List<SalesInvoiceItem> ParseLineItems(string json)
        {
            var items = new List<SalesInvoiceItem>();
            if (string.IsNullOrEmpty(json)) return items;

            try
            {
                var dto = JsonSerializer.Deserialize<List<LineItemDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dto == null) return items;

                foreach (var d in dto)
                {
                    if (d.ProductId > 0 && d.Quantity > 0)
                    {
                        items.Add(new SalesInvoiceItem
                        {
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            ItemDiscount = d.DiscountAmount
                        });
                    }
                }
            }
            catch
            {
                // return empty list on parse errors
            }

            return items;
        }

        private class LineItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal DiscountAmount { get; set; }
        }

        // AJAX endpoint to get product price
        [HttpGet]
        public IActionResult GetProductPrice(int productId)
        {
            var product = _productRepo.GetById(productId);
            if (product == null)
                return Json(new { price = 0 });

            return Json(new { price = product.SellingPrice });
        }
    }
}
