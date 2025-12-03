using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL.Repositories
{
    /// <summary>
    /// ADO.NET Repository for SalesInvoice entity operations
    /// Provides CRUD operations using parameterized SQL queries
    /// Non-transactional: operations that previously used transactions now use best-effort compensation.
    /// </summary>
    public class InvoiceRepository
    {
        /// <summary>
        /// Create a new sales invoice (without line items)
        /// </summary>
        public int Create(SalesInvoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            string query = @"
                INSERT INTO SalesInvoices 
                    (InvoiceNumber, CustomerId, InvoiceDate, SubTotal, ItemDiscount, InvoiceDiscount, Total, CreatedBy, CreatedDate)
                VALUES 
                    (@InvoiceNumber, @CustomerId, @InvoiceDate, @SubTotal, @ItemDiscount, @InvoiceDiscount, @Total, @CreatedBy, @CreatedDate);
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@InvoiceNumber", invoice.InvoiceNumber, SqlDbType.NVarChar, 50),
                AdoHelper.CreateParameter("@CustomerId", invoice.CustomerId, SqlDbType.Int),
                AdoHelper.CreateParameter("@InvoiceDate", invoice.InvoiceDate, SqlDbType.DateTime2),
                AdoHelper.CreateParameter("@SubTotal", invoice.SubTotal, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@ItemDiscount", invoice.ItemDiscount, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@InvoiceDiscount", invoice.InvoiceDiscount, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@Total", invoice.Total, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@CreatedBy", invoice.CreatedBy, SqlDbType.Int),
                AdoHelper.CreateParameter("@CreatedDate", invoice.CreatedDate, SqlDbType.DateTime2)
            };

            var result = AdoHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Create invoice and its line items without transactions.
        /// If any line insertion fails, attempts to roll back inserted lines and the invoice (best-effort compensation).
        /// </summary>
        public int CreateInvoiceWithItems(SalesInvoice invoice, IEnumerable<SalesInvoiceItem> items)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));

            var lineRepo = new InvoiceLineRepository();
            int invoiceId = 0;
            var createdLineIds = new List<int>();

            try
            {
                invoiceId = Create(invoice);

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        item.InvoiceId = invoiceId;
                        var lineId = lineRepo.Create(item);
                        createdLineIds.Add(lineId);
                    }
                }

                return invoiceId;
            }
            catch
            {
                // Compensation: delete any created lines, then delete invoice
                try
                {
                    foreach (var lid in createdLineIds)
                    {
                        try { lineRepo.Delete(lid); } catch { /* continue cleanup */ }
                    }

                    if (invoiceId > 0)
                    {
                        Delete(invoiceId);
                    }
                }
                catch { }
                throw;
            }
        }

        /// <summary>
        /// Get invoice by ID (includes line items)
        /// </summary>
        public SalesInvoice GetById(int id)
        {
            string query = @"
                SELECT Id, InvoiceNumber, CustomerId, InvoiceDate, SubTotal, ItemDiscount, InvoiceDiscount, Total, CreatedBy, CreatedDate
                FROM SalesInvoices
                WHERE Id = @Id";

            var parameters = new[] { AdoHelper.CreateParameter("@Id", id, SqlDbType.Int) };

            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                if (reader.Read())
                {
                    var invoice = MapReader(reader);
                    // populate invoice items
                    var lineRepo = new InvoiceLineRepository();
                    var items = lineRepo.GetByInvoice(invoice.Id);
                    invoice.InvoiceItems = items ?? new List<SalesInvoiceItem>();
                    return invoice;
                }
            }
            return null;
        }


        /// <summary>
        /// Get all invoices
        /// </summary>
        public List<SalesInvoice> GetAll()
        {
            string query = @"
                SELECT Id, InvoiceNumber, CustomerId, InvoiceDate, SubTotal, ItemDiscount, InvoiceDiscount, Total, CreatedBy, CreatedDate
                FROM SalesInvoices
                ORDER BY InvoiceDate DESC, Id DESC";

            var invoices = new List<SalesInvoice>();
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text))
            {
                while (reader.Read())
                {
                    invoices.Add(MapReader(reader));
                }
            }
            return invoices;
        }

        /// <summary>
        /// Get invoices for a specific customer
        /// </summary>
        public List<SalesInvoice> GetByCustomer(int customerId)
        {
            string query = @"
                SELECT Id, InvoiceNumber, CustomerId, InvoiceDate, SubTotal, ItemDiscount, InvoiceDiscount, Total, CreatedBy, CreatedDate
                FROM SalesInvoices
                WHERE CustomerId = @CustomerId
                ORDER BY InvoiceDate DESC";

            var parameters = new[] { AdoHelper.CreateParameter("@CustomerId", customerId, SqlDbType.Int) };

            var invoices = new List<SalesInvoice>();
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                while (reader.Read())
                {
                    invoices.Add(MapReader(reader));
                }
            }
            return invoices;
        }

      

        /// <summary>
        /// Update invoice
        /// </summary>
        public bool Update(SalesInvoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            string query = @"
                UPDATE SalesInvoices
                SET InvoiceNumber = @InvoiceNumber,
                    CustomerId = @CustomerId,
                    InvoiceDate = @InvoiceDate,
                    SubTotal = @SubTotal,
                    ItemDiscount = @ItemDiscount,
                    InvoiceDiscount = @InvoiceDiscount,
                    Total = @Total
                WHERE Id = @Id";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Id", invoice.Id, SqlDbType.Int),
                AdoHelper.CreateParameter("@InvoiceNumber", invoice.InvoiceNumber, SqlDbType.NVarChar, 50),
                AdoHelper.CreateParameter("@CustomerId", invoice.CustomerId, SqlDbType.Int),
                AdoHelper.CreateParameter("@InvoiceDate", invoice.InvoiceDate, SqlDbType.DateTime2),
                AdoHelper.CreateParameter("@SubTotal", invoice.SubTotal, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@ItemDiscount", invoice.ItemDiscount, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@InvoiceDiscount", invoice.InvoiceDiscount, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@Total", invoice.Total, SqlDbType.Decimal)
            };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Update invoice totals
        /// </summary>
        public bool UpdateTotals(int invoiceId, decimal subTotal, decimal itemDiscount, decimal invoiceDiscount, decimal total)
        {
            string query = @"
                UPDATE SalesInvoices
                SET SubTotal = @SubTotal,
                    ItemDiscount = @ItemDiscount,
                    InvoiceDiscount = @InvoiceDiscount,
                    Total = @Total
                WHERE Id = @Id";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Id", invoiceId, SqlDbType.Int),
                AdoHelper.CreateParameter("@SubTotal", subTotal, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@ItemDiscount", itemDiscount, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@InvoiceDiscount", invoiceDiscount, SqlDbType.Decimal),
                AdoHelper.CreateParameter("@Total", total, SqlDbType.Decimal)
            };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Delete invoice (hard delete - removes child lines first)
        /// </summary>
        public bool Delete(int id)
        {
            // delete child line items first
            var lineRepo = new InvoiceLineRepository();
            var lines = lineRepo.GetByInvoice(id);
            if (lines != null)
            {
                foreach (var line in lines)
                {
                    try { lineRepo.Delete(line.Id); } catch { /* ignore individual failures */ }
                }
            }

            string query = "DELETE FROM SalesInvoices WHERE Id = @Id";
            var parameters = new[] { AdoHelper.CreateParameter("@Id", id, SqlDbType.Int) };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Get total invoice count
        /// </summary>
        public int GetTotalCount()
        {
            string query = "SELECT COUNT(1) FROM SalesInvoices";
            var result = AdoHelper.ExecuteScalar(query, CommandType.Text);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Get total sales amount
        /// </summary>
        public decimal GetTotalSalesAmount()
        {
            string query = "SELECT ISNULL(SUM(Total), 0) FROM SalesInvoices";
            var result = AdoHelper.ExecuteScalar(query, CommandType.Text);
            return Convert.ToDecimal(result);
        }

        /// <summary>
        /// Map SqlDataReader to SalesInvoice object
        /// </summary>
        private SalesInvoice MapReader(SqlDataReader reader)
        {
            return new SalesInvoice
            {
                Id = reader.GetInt32(0),
                InvoiceNumber = reader.GetString(1),
                CustomerId = reader.GetInt32(2),
                InvoiceDate = reader.GetDateTime(3),
                SubTotal = reader.GetDecimal(4),
                ItemDiscount = reader.GetDecimal(5),
                InvoiceDiscount = reader.GetDecimal(6),
                Total = reader.GetDecimal(7),
                CreatedBy = reader.GetInt32(8),
                CreatedDate = reader.GetDateTime(9)
            };
        }
    }
}
