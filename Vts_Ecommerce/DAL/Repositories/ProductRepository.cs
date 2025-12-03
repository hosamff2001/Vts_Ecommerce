using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL.Repositories
{
    /// <summary>
    /// Product Repository - ADO.NET CRUD operations for Product entity
    /// Inherits from BaseRepository for common CRUD functionality
    /// Includes inventory management methods with parameterized queries
    /// </summary>
    public class ProductRepository
    {

        /// <summary>
        /// Create a new product
        /// </summary>
        public int Create(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            string query = @"
                INSERT INTO Products (Name, Description, Price, StockQuantity, CategoryId, IsActive)
                VALUES (@Name, @Description, @Price, @StockQuantity, @CategoryId, @IsActive);
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Name", product.Name, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@Description", product.Description, SqlDbType.NVarChar, 1000),
                AdoHelper.CreateParameter("@Price", product.Price, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@StockQuantity", product.StockQuantity, SqlDbType.Int),
                AdoHelper.CreateParameter("@CategoryId", product.CategoryId, SqlDbType.Int),
                AdoHelper.CreateParameter("@IsActive", product.IsActive, SqlDbType.Bit)
            };

            var result = AdoHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        public Product GetById(int id)
        {
            string query = @"
                SELECT Id, Name, Description, Price, StockQuantity, CategoryId, IsActive
                FROM Products
                WHERE Id = @Id";

            var parameters = new[] { AdoHelper.CreateParameter("@Id", id, SqlDbType.Int) };

            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                if (reader.Read())
                {
                    return MapReader(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        public List<Product> GetAll(bool activeOnly = true)
        {
            string query = @"
                SELECT Id, Name, Description, Price, StockQuantity, CategoryId, IsActive
                FROM Products";

            if (activeOnly)
            {
                query += " WHERE IsActive = 1";
            }

            query += " ORDER BY Name ASC";

            var products = new List<Product>();
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text))
            {
                while (reader.Read())
                {
                    products.Add(MapReader(reader));
                }
            }
            return products;
        }

        /// <summary>
        /// Get products by category
        /// </summary>
        public List<Product> GetByCategory(int categoryId, bool activeOnly = true)
        {
            string query = @"
                SELECT Id, Name, Description, Price, StockQuantity, CategoryId, IsActive
                FROM Products
                WHERE CategoryId = @CategoryId";

            if (activeOnly)
            {
                query += " AND IsActive = 1";
            }

            query += " ORDER BY Name ASC";

            var parameters = new[] { AdoHelper.CreateParameter("@CategoryId", categoryId, SqlDbType.Int) };

            var products = new List<Product>();
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                while (reader.Read())
                {
                    products.Add(MapReader(reader));
                }
            }
            return products;
        }

        /// <summary>
        /// Get products by name (like search)
        /// </summary>
        public List<Product> SearchByName(string searchTerm, bool activeOnly = true)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be null or empty", nameof(searchTerm));

            string query = @"
                SELECT Id, Name, Description, Price, StockQuantity, CategoryId, IsActive
                FROM Products
                WHERE Name LIKE @SearchTerm";

            if (activeOnly)
            {
                query += " AND IsActive = 1";
            }

            query += " ORDER BY Name ASC";

            var parameters = new[] { AdoHelper.CreateParameter("@SearchTerm", $"%{searchTerm}%", SqlDbType.NVarChar, 100) };

            var products = new List<Product>();
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                while (reader.Read())
                {
                    products.Add(MapReader(reader));
                }
            }
            return products;
        }

        /// <summary>
        /// Update product information
        /// </summary>
        public bool Update(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (product.Id <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(product.Id));

            string query = @"
                UPDATE Products
                SET Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    StockQuantity = @StockQuantity,
                    CategoryId = @CategoryId,
                    IsActive = @IsActive
                WHERE Id = @Id";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Id", product.Id, SqlDbType.Int),
                AdoHelper.CreateParameter("@Name", product.Name, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@Description", product.Description, SqlDbType.NVarChar, 1000),
                AdoHelper.CreateParameter("@Price", product.Price, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@StockQuantity", product.StockQuantity, SqlDbType.Int),
                AdoHelper.CreateParameter("@CategoryId", product.CategoryId, SqlDbType.Int),
                AdoHelper.CreateParameter("@IsActive", product.IsActive, SqlDbType.Bit)
            };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Delete a product (hard delete - remove row from database)
        /// Note: ensure no FK constraints will block deletion (e.g., invoice items)
        /// </summary>
        public bool Delete(int id)
        {
            string query = @"
                DELETE FROM Products
                WHERE Id = @Id";

            var parameters = new[] { AdoHelper.CreateParameter("@Id", id, SqlDbType.Int) };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

       

        /// <summary>
        /// Check if sufficient stock is available
        /// </summary>
        public bool IsStockAvailable(int productId, int requiredQuantity)
        {
            string query = @"
                SELECT StockQuantity
                FROM Products
                WHERE Id = @Id";

            var parameters = new[] { AdoHelper.CreateParameter("@Id", productId, SqlDbType.Int) };

            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                if (reader.Read())
                {
                    int currentStock = reader.GetInt32(0);
                    return currentStock >= requiredQuantity;
                }
            }
            return false;
        }

        /// <summary>
        /// Map SqlDataReader to Product object
        /// </summary>
        private Product MapReader(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}
