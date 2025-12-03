using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL.Repositories
{
    /// <summary>
    /// Category Repository - ADO.NET CRUD operations for Category entity
    /// Inherits from BaseRepository for common CRUD functionality
    /// Uses parameterized SQL queries to prevent SQL injection
    /// </summary>
    public class CategoryRepository
    {

        /// <summary>
        /// Create a new category in the database
        /// </summary>
        public int Create(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            string query = @"
                INSERT INTO Categories (Name, Description, IsActive)
                VALUES (@Name, @Description, @IsActive);
                SELECT SCOPE_IDENTITY();";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Name", category.Name, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@Description", category.Description, SqlDbType.NVarChar, 500),
                AdoHelper.CreateParameter("@IsActive", category.IsActive, SqlDbType.Bit)
            };

            var result = AdoHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        public Category GetById(int id)
        {
            string query = @"
                SELECT Id, Name, Description, IsActive
                FROM Categories
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
        /// Update category information
        /// </summary>
        public bool Update(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category.Id <= 0)
                throw new ArgumentException("Category ID must be greater than 0", nameof(category.Id));

            string query = @"
                UPDATE Categories
                SET Name = @Name,
                    Description = @Description,
                    IsActive = @IsActive
                WHERE Id = @Id";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Id", category.Id, SqlDbType.Int),
                AdoHelper.CreateParameter("@Name", category.Name, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@Description", category.Description, SqlDbType.NVarChar, 500),
                AdoHelper.CreateParameter("@IsActive", category.IsActive, SqlDbType.Bit)
            };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Delete a category (hard delete - set IsActive to false)
        /// </summary>
        public bool Delete(int id)
        {
            string query = @"
                DELETE FROM Categories
                WHERE Id = @Id";

            var parameters = new[] { AdoHelper.CreateParameter("@Id", id, SqlDbType.Int) };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Map SqlDataReader to Category object
        /// </summary>
        private Category MapReader(SqlDataReader reader)
        {
            return new Category
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsActive = reader.GetBoolean(3)
            };
        }

        public int GetTotalCount()
        {
            string query = "SELECT COUNT(1) FROM Categories";
            var result = AdoHelper.ExecuteScalar(query, CommandType.Text);
            return Convert.ToInt32(result);
        }
        public List<Category> GetAll()
        {
            var categories = new List<Category>();
            string query = @"
                SELECT Id, Name, Description, IsActive
                FROM Categories";

            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text))
            {
                while (reader.Read())
                {
                    categories.Add(MapReader(reader));
                }
            }
            return categories;
        }
    }
}
