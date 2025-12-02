using System;
using System.Data;
using System.Data.SqlClient;

namespace Vts_Ecommerce.DAL
{
    /// <summary>
    /// ADO.NET Database Helper Class
    /// Provides methods for database operations using SqlConnection, SqlCommand, SqlDataReader
    /// </summary>
    public static class AdoHelper
    {
        private static string _connectionString;

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

        
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

       
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

        public static SqlDataReader ExecuteReader(string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            var connection = GetConnection();
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

        public static int ExecuteNonQueryWithTransaction(SqlTransaction transaction, string commandText, CommandType commandType = CommandType.Text, params SqlParameter[] parameters)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            var command = new SqlCommand(commandText, transaction.Connection, transaction);
            command.CommandType = commandType;
            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }
            return command.ExecuteNonQuery();
        }

        public static SqlParameter CreateParameter(string parameterName, object value, SqlDbType dbType)
        {
            // Ensure parameter name starts with @
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }

            var parameter = new SqlParameter(parameterName, dbType);
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        public static SqlParameter CreateParameter(string parameterName, object value, SqlDbType dbType, int size)
        {
            // Ensure parameter name starts with @
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }

            var parameter = new SqlParameter(parameterName, dbType, size);
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        public static SqlTransaction BeginTransaction()
        {
            var connection = GetConnection();
            connection.Open();
            return connection.BeginTransaction();
        }
    }
}


