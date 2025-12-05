using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.DAL.Repositories
{
    /// <summary>
    /// User Repository - ADO.NET CRUD operations for User entity
    /// Inherits from BaseRepository for common CRUD functionality
    /// Uses parameterized SQL queries to prevent SQL injection
    /// </summary>
    public class UserRepository
    {

        /// <summary>
        /// Create a new user in the database
        /// </summary>
        public int Create(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username cannot be empty", nameof(user.Username));

            string query = @"
                INSERT INTO Users (Username, Password, Email, IsActive, CreatedDate)
                VALUES (@Username, @Password, @Email, @IsActive, @CreatedDate);";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Username", user.Username, SqlDbType.NVarChar, 50),
                AdoHelper.CreateParameter("@Password", user.Password, SqlDbType.NVarChar, 500),
                AdoHelper.CreateParameter("@Email", user.Email, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@IsActive", user.IsActive, SqlDbType.Bit),
                AdoHelper.CreateParameter("@CreatedDate", user.CreatedDate, SqlDbType.DateTime2)
            };

            var result = AdoHelper.ExecuteScalar(query, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Update user information
        /// </summary>
        public bool Update(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (user.Id <= 0)
                throw new ArgumentException("User ID must be greater than 0", nameof(user.Id));

            string query = @"
                UPDATE Users
                SET Username = @Username,
                    Password = @Password,
                    Email = @Email,
                    IsActive = @IsActive
                WHERE Id = @Id";

            var parameters = new[]
            {
                AdoHelper.CreateParameter("@Id", user.Id, SqlDbType.Int),
                AdoHelper.CreateParameter("@Username", user.Username, SqlDbType.NVarChar, 50),
                AdoHelper.CreateParameter("@Password", user.Password, SqlDbType.NVarChar, 500),
                AdoHelper.CreateParameter("@Email", user.Email, SqlDbType.NVarChar, 100),
                AdoHelper.CreateParameter("@IsActive", user.IsActive, SqlDbType.Bit)
            };

            int rowsAffected = AdoHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Get a user by username
        /// </summary>
        public User GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            string query = @"
                SELECT Id, Username, Password, Email, IsActive, CreatedDate
                FROM Users
                WHERE Username = @Username";

            var parameters = new[] { AdoHelper.CreateParameter("@Username", username, SqlDbType.NVarChar, 50) };

            return ExecuteSingleQuery(query, parameters);
        }
  
       
     
        /// <summary>
        /// Execute a query that returns a single User row
        /// </summary>
        private User ExecuteSingleQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlDataReader reader = AdoHelper.ExecuteReader(query, CommandType.Text, parameters))
            {
                if (reader.Read())
                {
                    return MapReader(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// Map SqlDataReader to User object
        /// </summary>
        private User MapReader(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Password = reader.GetString(reader.GetOrdinal("Password")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
            };
        }

        public int GetTotalCount()
        {
            string query = "SELECT COUNT(1) FROM Users";
            var result = AdoHelper.ExecuteScalar(query, CommandType.Text);
            return Convert.ToInt32(result);
        }

        public List<User> GetAll()
        {
            var users = new List<User>();
            string query = @"
                SELECT Id, Username, Password, Email, IsActive, CreatedDate
                FROM Users";
            using (var reader = AdoHelper.ExecuteReader(query, CommandType.Text))
            {
                while (reader.Read())
                {
                    users.Add(MapReader(reader));
                }
            }
            return users;
        }
    }
}
