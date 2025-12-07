using System.Data;
using System.Data.SqlClient;

namespace Vts_Ecommerce.DAL
{
    /// <summary>
    /// ADO.NET Database Helper Class
    /// Provides methods for database operations using SqlConnection, SqlCommand, SqlDataReader
    /// Supports parameterized queries, and connection pooling
    /// </summary>
    public static class AdoHelper
    {
        private static string? _connectionString;

        /// <summary>
        /// Initialize the connection string (call this in Program.cs)
        /// </summary>
        public static void Initialize(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
            }
            _connectionString = connectionString;
        }

        private static string GetConnectionString()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("AdoHelper has not been initialized. Call AdoHelper.Initialize() in Program.cs");
            }
            return _connectionString;
        }

        /// <summary>
        /// Get a new SQL connection from the connection string
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// Execute a non-query command (INSERT, UPDATE, DELETE)
        /// </summary>
        public static int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Execute a query and return a SqlDataReader
        /// Note: Caller must dispose the reader to close the connection
        /// </summary>
        public static SqlDataReader ExecuteReader(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var connection = GetConnection();
            try
            {
                connection.Open();
                var command = new SqlCommand(commandText, connection);
                command.CommandType = commandType;
                if (parameters != null && parameters.Length > 0)
                {
                    command.Parameters.AddRange(parameters);
                }
                // CommandBehavior.CloseConnection ensures connection closes when reader is disposed
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                connection?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Execute a scalar query (returns single value)
        /// </summary>
        public static object ExecuteScalar(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandType = commandType;
                    if (parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return command.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Create a SqlParameter with null handling
        /// </summary>
        public static SqlParameter CreateParameter(string parameterName, object value, SqlDbType dbType)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }

            var parameter = new SqlParameter(parameterName, dbType)
            {
                Value = value ?? DBNull.Value
            };
            return parameter;
        }

        /// <summary>
        /// Create a SqlParameter with size constraint and null handling
        /// </summary>
        public static SqlParameter CreateParameter(string parameterName, object value, SqlDbType dbType, int size)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }

            var parameter = new SqlParameter(parameterName, dbType, size)
            {
                Value = value ?? DBNull.Value
            };
            return parameter;
        }

     
    }
}


