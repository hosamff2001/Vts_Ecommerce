using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL.Repositories
{
    /// <summary>
    /// ADO.NET Repository for SalesInvoiceItem entity operations
    /// Provides CRUD operations using parameterized SQL queries
    /// Designed to work with InvoiceRepository for transaction support
    /// </summary>
    public class InvoiceLineRepository
    {
        /// <summary>
        /// Create a new invoice line item
        /// </summary>
        public int Create(SalesInvoiceItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            string query = @"
                INSERT INTO SalesInvoiceItems 
                    (InvoiceId, ProductId, Quantity, UnitPrice, ItemDiscount, LineTotal)
                VALUES 
                    (@InvoiceId, @ProductId, @Quantity, @UnitPrice, @ItemDiscount, @LineTotal);
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@InvoiceId", item.InvoiceId, SqlDbType.Int),
                AdoHelper.CreateParameter("@ProductId", item.ProductId, SqlDbType.Int),
                AdoHelper.CreateParameter("@Quantity", item.Quantity, SqlDbType.Int),
                AdoHelper.CreateParameter("@UnitPrice", item.UnitPrice, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@ItemDiscount", item.ItemDiscount, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@LineTotal", item.LineTotal, SqlDbType.Decimal)
            };

            var result = AdoHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

 
        /// <summary>
        /// Get line item by ID
        /// </summary>
        public SalesInvoiceItem GetById(int id)
        {
            string query = @"
                SELECT Id, InvoiceId, ProductId, Quantity, UnitPrice, ItemDiscount, LineTotal
                FROM SalesInvoiceItems
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
        /// Get all line items for an invoice
        /// </summary>
        public List<SalesInvoiceItem> GetByInvoice(int invoiceId)
        {
            string query = @"
                SELECT Id, InvoiceId, ProductId, Quantity, UnitPrice, ItemDiscount, LineTotal
                FROM SalesInvoiceItems
                WHERE InvoiceId = @InvoiceId
                ORDER BY Id ASC";

            var parameters = new[] { AdoHelper.CreateParameter("@InvoiceId", invoiceId, SqlDbType.Int) };

            var items = new List<SalesInvoiceItem>();
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                while (reader.Read())
                {
                    items.Add(MapReader(reader));
                }
            }
            return items;
        }

        /// <summary>
        /// Update line item
        /// </summary>
        public bool Update(SalesInvoiceItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            string query = @"
                UPDATE SalesInvoiceItems
                SET Quantity = @Quantity,
                    UnitPrice = @UnitPrice,
                    ItemDiscount = @ItemDiscount,
                    LineTotal = @LineTotal
                WHERE Id = @Id";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Id", item.Id, SqlDbType.Int),
                AdoHelper.CreateParameter("@Quantity", item.Quantity, SqlDbType.Int),
                AdoHelper.CreateParameter("@UnitPrice", item.UnitPrice, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@ItemDiscount", item.ItemDiscount, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@LineTotal", item.LineTotal, SqlDbType.Decimal)
            };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Delete line item
        /// </summary>
        public bool Delete(int id)
        {
            string query = "DELETE FROM SalesInvoiceItems WHERE Id = @Id";
            var parameters = new[] { AdoHelper.CreateParameter("@Id", id, SqlDbType.Int) };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

      
        /// <summary>
        /// Get total line items in database
        /// </summary>
        public int GetTotalCount()
        {
            string query = "SELECT COUNT(1) FROM SalesInvoiceItems";
            var result = AdoHelper.ExecuteScalar(query, CommandType.Text);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Get total quantity of a product sold across all invoices
        /// </summary>
        public int GetTotalQuantitySold(int productId)
        {
            string query = @"
                SELECT ISNULL(SUM(Quantity), 0)
                FROM SalesInvoiceItems
                WHERE ProductId = @ProductId";

            var parameters = new[] { AdoHelper.CreateParameter("@ProductId", productId, SqlDbType.Int) };

            var result = AdoHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Map SqlDataReader to SalesInvoiceItem object
        /// </summary>
        private SalesInvoiceItem MapReader(SqlDataReader reader)
        {
            return new SalesInvoiceItem
            {
                Id = reader.GetInt32(0),
                InvoiceId = reader.GetInt32(1),
                ProductId = reader.GetInt32(2),
                Quantity = reader.GetInt32(3),
                UnitPrice = reader.GetDecimal(4),
                ItemDiscount = reader.GetDecimal(5),
                LineTotal = reader.GetDecimal(6)
            };
        }

        public dynamic GetByInvoiceId(int id)
        {
           var query = @"
                SELECT sii.Id, sii.InvoiceId, sii.ProductId, p.Name AS ProductName, sii.Quantity, sii.UnitPrice, sii.ItemDiscount, sii.LineTotal
                FROM SalesInvoiceItems sii
                INNER JOIN Products p ON sii.ProductId = p.Id
                WHERE sii.InvoiceId = @InvoiceId";

            var parameters = new[] { AdoHelper.CreateParameter("@InvoiceId", id, SqlDbType.Int) };

            var items = new List<dynamic>();
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                while (reader.Read())
                {
                    items.Add(new
                    {
                        Id = reader.GetInt32(0),
                        InvoiceId = reader.GetInt32(1),
                        ProductId = reader.GetInt32(2),
                        ProductName = reader.GetString(3),
                        Quantity = reader.GetInt32(4),
                        UnitPrice = reader.GetDecimal(5),
                        ItemDiscount = reader.GetDecimal(6),
                        LineTotal = reader.GetDecimal(7)
                    });
                }
            }
            return items;
        }
    }
}
