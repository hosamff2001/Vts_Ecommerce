using System;
using System.Collections.Generic;
using Vts_Ecommerce.DAL.Repositories;
using Vts_Ecommerce.Helpers;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL.DataSeeding
{
    /// <summary>
    /// Database seeding service for populating initial dummy data
    /// </summary>
    public class DatabaseSeeder
    {
        /// <summary>
        /// Seed all dummy data into the database
        /// </summary>
        public static void SeedData()
        {
            try
            {
                Console.WriteLine("Starting database seeding...");

                // Seed users first
                SeedUsers();
                Console.WriteLine("✓ Users seeded");

                // Seed categories
                SeedCategories();
                Console.WriteLine("✓ Categories seeded");

                // Seed products
                SeedProducts();
                Console.WriteLine("✓ Products seeded");

                // Seed customers
                SeedCustomers();
                Console.WriteLine("✓ Customers seeded");

                // Seed invoices and line items
                SeedInvoices();
                Console.WriteLine("✓ Invoices and line items seeded");

                Console.WriteLine("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database seeding: {ex.Message}");
                throw;
            }
        }

        private static void SeedUsers()
        {
            var userRepo = new UserRepository();

            // Check if users already exist
            if (userRepo.GetTotalCount() > 0)
                return;

            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Password = "Admin@123",
                    Email = "admin@vts.com",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new User
                {
                    Username = "salesman1",
                    Password = "Sales@123",
                    Email = "salesman1@vts.com",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                },
                new User
                {
                    Username = "salesman2",
                    Password = "Sales@123",
                    Email = "salesman2@vts.com",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                }
            };

            foreach (var user in users)
            {
                // encrypt password before saving
                if (!string.IsNullOrEmpty(user.Password))
                {
                    user.Password = EncryptionService.Encrypt(user.Password);
                }
                userRepo.Create(user);
            }
        }

        private static void SeedCategories()
        {
            var categoryRepo = new CategoryRepository();

            // Check if categories already exist
            if (categoryRepo.GetTotalCount() > 0)
                return;

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Electronics",
                    Description = "Electronic devices and gadgets",
                    IsActive = true
                },
                new Category
                {
                    Name = "Clothing",
                    Description = "Apparel and fashion items",
                    IsActive = true
                },
                new Category
                {
                    Name = "Books",
                    Description = "Physical and digital books",
                    IsActive = true
                },
                new Category
                {
                    Name = "Home & Garden",
                    Description = "Home improvement and gardening supplies",
                    IsActive = true
                },
                new Category
                {
                    Name = "Sports & Outdoors",
                    Description = "Sports equipment and outdoor gear",
                    IsActive = true
                }
            };

            foreach (var category in categories)
            {
                categoryRepo.Create(category);
            }
        }

        private static void SeedProducts()
        {
            var productRepo = new ProductRepository();
            var categoryRepo = new CategoryRepository();

            // Check if products already exist
            if (productRepo.GetTotalCount() > 0)
                return;

            var categories = categoryRepo.GetAll();
            if (categories.Count == 0)
                return;

            var products = new List<Product>
            {
                // Electronics
                new Product
                {
                    Name = "Laptop Pro 15",
                    Description = "High-performance laptop with 16GB RAM and 512GB SSD",
                    Price = 1299.99m,
                    StockQuantity = 25,
                    CategoryId = categories[0].Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with long battery life",
                    Price = 29.99m,
                    StockQuantity = 150,
                    CategoryId = categories[0].Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "USB-C Hub",
                    Description = "7-in-1 USB-C hub with HDMI and card reader",
                    Price = 49.99m,
                    StockQuantity = 80,
                    CategoryId = categories[0].Id,
                    IsActive = true
                },
                // Clothing
                new Product
                {
                    Name = "Cotton T-Shirt",
                    Description = "100% cotton comfortable t-shirt",
                    Price = 19.99m,
                    StockQuantity = 200,
                    CategoryId = categories[1].Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Jeans",
                    Description = "Classic blue denim jeans",
                    Price = 59.99m,
                    StockQuantity = 120,
                    CategoryId = categories[1].Id,
                    IsActive = true
                },
                // Books
                new Product
                {
                    Name = "C# Programming Guide",
                    Description = "Comprehensive guide to C# programming",
                    Price = 39.99m,
                    StockQuantity = 50,
                    CategoryId = categories[2].Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Clean Code",
                    Description = "A Handbook of Agile Software Craftsmanship",
                    Price = 45.99m,
                    StockQuantity = 45,
                    CategoryId = categories[2].Id,
                    IsActive = true
                },
                // Home & Garden
                new Product
                {
                    Name = "LED Desk Lamp",
                    Description = "Adjustable LED desk lamp with USB charging",
                    Price = 34.99m,
                    StockQuantity = 75,
                    CategoryId = categories[3].Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Plant Pot",
                    Description = "Ceramic plant pot with drainage",
                    Price = 12.99m,
                    StockQuantity = 200,
                    CategoryId = categories[3].Id,
                    IsActive = true
                },
                // Sports & Outdoors
                new Product
                {
                    Name = "Running Shoes",
                    Description = "Professional running shoes with cushioning",
                    Price = 89.99m,
                    StockQuantity = 60,
                    CategoryId = categories[4].Id,
                    IsActive = true
                },
                new Product
                {
                    Name = "Yoga Mat",
                    Description = "Non-slip yoga mat with carrying strap",
                    Price = 24.99m,
                    StockQuantity = 100,
                    CategoryId = categories[4].Id,
                    IsActive = true
                }
            };

            foreach (var product in products)
            {
                productRepo.Create(product);
            }
        }

        private static void SeedCustomers()
        {
            var customerRepo = new CustomerRepository();

            // Check if customers already exist
            if (customerRepo.GetTotalCount() > 0)
                return;

            var customers = new List<Customer>
            {
                new Customer
                {
                    Name = "John Smith",
                    Email = "john.smith@example.com",
                    Phone = "555-0101",
                    Address = "123 Main St, New York, NY 10001",
                    IsActive = true
                },
                new Customer
                {
                    Name = "Jane Doe",
                    Email = "jane.doe@example.com",
                    Phone = "555-0102",
                    Address = "456 Oak Ave, Los Angeles, CA 90001",
                    IsActive = true
                },
                new Customer
                {
                    Name = "Robert Wilson",
                    Email = "robert.wilson@example.com",
                    Phone = "555-0103",
                    Address = "789 Pine Rd, Chicago, IL 60601",
                    IsActive = true
                },
                new Customer
                {
                    Name = "Maria Garcia",
                    Email = "maria.garcia@example.com",
                    Phone = "555-0104",
                    Address = "321 Elm St, Houston, TX 77001",
                    IsActive = true
                },
                new Customer
                {
                    Name = "David Johnson",
                    Email = "david.johnson@example.com",
                    Phone = "555-0105",
                    Address = "654 Maple Dr, Phoenix, AZ 85001",
                    IsActive = true
                }
            };

            foreach (var customer in customers)
            {
                customerRepo.Create(customer);
            }
        }

        private static void SeedInvoices()
        {
            var invoiceRepo = new InvoiceRepository();
            var invoiceLineRepo = new InvoiceLineRepository();
            var customerRepo = new CustomerRepository();
            var productRepo = new ProductRepository();
            var userRepo = new UserRepository();

            // Check if invoices already exist
            if (invoiceRepo.GetTotalCount() > 0)
                return;

            var customers = customerRepo.GetAll();
            var products = productRepo.GetAll();
            var users = userRepo.GetAll();

            if (customers.Count == 0 || products.Count == 0 || users.Count == 0)
                return;

            // Create sample invoices
            var invoices = new List<(SalesInvoice invoice, List<SalesInvoiceItem> items)>
            {
                (
                    new SalesInvoice
                    {
                        InvoiceNumber = "INV-001",
                        CustomerId = customers[0].Id,
                        InvoiceDate = DateTime.Now.AddDays(-30),
                        SubTotal = 1359.97m,
                        ItemDiscount = 50m,
                        InvoiceDiscount = 0m,
                        Total = 1309.97m,
                        CreatedBy = users[0].Id,
                        CreatedDate = DateTime.Now.AddDays(-30)
                    },
                    new List<SalesInvoiceItem>
                    {
                        new SalesInvoiceItem
                        {
                            ProductId = products[0].Id,
                            Quantity = 1,
                            UnitPrice = 1299.99m,
                            ItemDiscount = 50m,
                            LineTotal = 1249.99m
                        },
                        new SalesInvoiceItem
                        {
                            ProductId = products[1].Id,
                            Quantity = 2,
                            UnitPrice = 29.99m,
                            ItemDiscount = 0m,
                            LineTotal = 59.98m
                        }
                    }
                ),
                (
                    new SalesInvoice
                    {
                        InvoiceNumber = "INV-002",
                        CustomerId = customers[1].Id,
                        InvoiceDate = DateTime.Now.AddDays(-20),
                        SubTotal = 149.95m,
                        ItemDiscount = 10m,
                        InvoiceDiscount = 0m,
                        Total = 139.95m,
                        CreatedBy = users[1].Id,
                        CreatedDate = DateTime.Now.AddDays(-20)
                    },
                    new List<SalesInvoiceItem>
                    {
                        new SalesInvoiceItem
                        {
                            ProductId = products[3].Id,
                            Quantity = 5,
                            UnitPrice = 19.99m,
                            ItemDiscount = 10m,
                            LineTotal = 89.95m
                        },
                        new SalesInvoiceItem
                        {
                            ProductId = products[4].Id,
                            Quantity = 1,
                            UnitPrice = 59.99m,
                            ItemDiscount = 0m,
                            LineTotal = 59.99m
                        }
                    }
                ),
                (
                    new SalesInvoice
                    {
                        InvoiceNumber = "INV-003",
                        CustomerId = customers[2].Id,
                        InvoiceDate = DateTime.Now.AddDays(-10),
                        SubTotal = 209.97m,
                        ItemDiscount = 0m,
                        InvoiceDiscount = 20m,
                        Total = 189.97m,
                        CreatedBy = users[0].Id,
                        CreatedDate = DateTime.Now.AddDays(-10)
                    },
                    new List<SalesInvoiceItem>
                    {
                        new SalesInvoiceItem
                        {
                            ProductId = products[5].Id,
                            Quantity = 3,
                            UnitPrice = 39.99m,
                            ItemDiscount = 0m,
                            LineTotal = 119.97m
                        },
                        new SalesInvoiceItem
                        {
                            ProductId = products[8].Id,
                            Quantity = 7,
                            UnitPrice = 12.99m,
                            ItemDiscount = 0m,
                            LineTotal = 90.93m
                        }
                    }
                ),
                (
                    new SalesInvoice
                    {
                        InvoiceNumber = "INV-004",
                        CustomerId = customers[3].Id,
                        InvoiceDate = DateTime.Now.AddDays(-5),
                        SubTotal = 144.97m,
                        ItemDiscount = 5m,
                        InvoiceDiscount = 0m,
                        Total = 139.97m,
                        CreatedBy = users[1].Id,
                        CreatedDate = DateTime.Now.AddDays(-5)
                    },
                    new List<SalesInvoiceItem>
                    {
                        new SalesInvoiceItem
                        {
                            ProductId = products[9].Id,
                            Quantity = 1,
                            UnitPrice = 89.99m,
                            ItemDiscount = 5m,
                            LineTotal = 84.99m
                        },
                        new SalesInvoiceItem
                        {
                            ProductId = products[10].Id,
                            Quantity = 2,
                            UnitPrice = 24.99m,
                            ItemDiscount = 0m,
                            LineTotal = 49.98m
                        }
                    }
                )
            };

            foreach (var (invoice, items) in invoices)
            {
                invoiceRepo.CreateInvoiceWithItems(invoice, items);
            }
        }
    }
}
