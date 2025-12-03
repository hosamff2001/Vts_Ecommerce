using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL.Repositories
{
    /// <summary>
    /// Customer Repository - ADO.NET CRUD operations for Customer entity
    /// Inherits from BaseRepository for common CRUD functionality
    /// Uses parameterized SQL queries to prevent SQL injection
    /// </summary>
    public class CustomerRepository
    {

        /// <summary>
        /// Create a new customer
        /// </summary>
        public int Create(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            string query = @"
                INSERT INTO Customers (Name, Email, Phone, Address, IsActive)
                VALUES (@Name, @Email, @Phone, @Address, @IsActive);
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Name", customer.Name, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@Email", customer.Email, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@Phone", customer.Phone, SqlDbType.NVarChar, 20),
                AdoHelper.CreateParameter("@Address", customer.Address, SqlDbType.NVarChar, 500),
                AdoHelper.CreateParameter("@IsActive", customer.IsActive, SqlDbType.Bit)
            };

            var result = AdoHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        public Customer GetById(int id)
        {
            string query = @"
                SELECT Id, Name, Email, Phone, Address, IsActive
                FROM Customers
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
        /// Get all customers
        /// </summary>
        public List<Customer> GetAll(bool activeOnly = true)
        {
            string query = @"
                SELECT Id, Name, Email, Phone, Address, IsActive
                FROM Customers";

            if (activeOnly)
            {
                query += " WHERE IsActive = 1";
            }

            query += " ORDER BY Name ASC";

            var customers = new List<Customer>();
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text))
            {
                while (reader.Read())
                {
                    customers.Add(MapReader(reader));
                }
            }
            return customers;
        }

        /// <summary>
        /// Get customer by name
        /// </summary>
        public Customer GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Customer name cannot be null or empty", nameof(name));

            string query = @"
                SELECT Id, Name, Email, Phone, Address, IsActive
                FROM Customers
                WHERE Name = @Name";

            var parameters = new[] { AdoHelper.CreateParameter("@Name", name, SqlDbType.NVarChar, 100) };

            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                if (reader.Read())
                    return MapReader(reader);
            }
            return null;
        }

        /// <summary>
        /// Update customer information
        /// </summary>
        public bool Update(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (customer.Id <= 0)
                throw new ArgumentException("Customer ID must be greater than 0", nameof(customer.Id));

            string query = @"
                UPDATE Customers
                SET Name = @Name,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address,
                    IsActive = @IsActive
                WHERE Id = @Id";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Id", customer.Id, SqlDbType.Int),
                AdoHelper.CreateParameter("@Name", customer.Name, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@Email", customer.Email, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@Phone", customer.Phone, SqlDbType.NVarChar, 20),
                AdoHelper.CreateParameter("@Address", customer.Address, SqlDbType.NVarChar, 500),
                AdoHelper.CreateParameter("@IsActive", customer.IsActive, SqlDbType.Bit)
            };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Delete a customer (hard delete - set IsActive to false)
        /// </summary>
        public bool Delete(int id)
        {
            string query = @"
                Delete FROM Customers
                WHERE Id = @Id";

            var parameters = new[] { AdoHelper.CreateParameter("@Id", id, SqlDbType.Int) };
            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Map SqlDataReader to Customer object
        /// </summary>
        private Customer MapReader(SqlDataReader reader)
        {
            return new Customer
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}
