using System;
using System.Data;
using System.Data.SqlClient;

namespace Vts_Ecommerce.DAL
{
    /// <summary>
    /// Database connection test utility
    /// Tests connectivity to the database and verifies tables exist
    /// </summary>
    public static class DatabaseConnectionTest
    {
        public static void TestConnection(string connectionString)
        {
            Console.WriteLine("\n========== DATABASE CONNECTION TEST ==========\n");
            
            try
            {
                // Test 1: Basic connection
                Console.WriteLine("1. Testing basic SQL connection...");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("   ✓ Connection successful!");
                    Console.WriteLine($"   Database: {connection.Database}");
                    Console.WriteLine($"   Server: {connection.DataSource}");
                    
                    // Test 2: Query tables
                    Console.WriteLine("\n2. Checking database tables...");
                    string query = @"
                        SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_SCHEMA = 'dbo' 
                        ORDER BY TABLE_NAME";
                    
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var tables = new System.Collections.Generic.List<string>();
                            while (reader.Read())
                            {
                                tables.Add(reader.GetString(0));
                            }
                            
                            if (tables.Count > 0)
                            {
                                Console.WriteLine($"   ✓ Found {tables.Count} tables:");
                                foreach (var table in tables)
                                {
                                    Console.WriteLine($"      - {table}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("   ✗ No tables found!");
                            }
                        }
                    }
                    
                    // Test 3: Check table record counts
                    Console.WriteLine("\n3. Checking table record counts...");
                    var tableNames = new[] { "Users", "Categories", "Products", "Customers", "SalesInvoices", "SalesInvoiceItems", "UserSessions" };
                    
                    foreach (var tableName in tableNames)
                    {
                        try
                        {
                            string countQuery = $"SELECT COUNT(*) FROM dbo.[{tableName}]";
                            using (var cmd = new SqlCommand(countQuery, connection))
                            {
                                var count = (int)cmd.ExecuteScalar();
                                Console.WriteLine($"   - {tableName}: {count} records");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"   - {tableName}: Error - {ex.Message}");
                        }
                    }
                    
                    connection.Close();
                }
                
                Console.WriteLine("\n✓ All database connection tests passed!\n");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"\n✗ SQL Connection Error: {ex.Message}\n");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}\n");
                throw;
            }
        }
    }
}
