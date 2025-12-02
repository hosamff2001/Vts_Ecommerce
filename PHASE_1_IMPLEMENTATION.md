# Phase 1: Project Setup & Folder Structure
## Detailed Implementation Guide

---

## 📋 Overview

This phase sets up the project foundation, creates the folder structure, and establishes base helper classes for the ASP.NET MVC (.NET Framework) Sales Management Web Application.

**Duration:** 3 commits  
**Dependencies:** None  
**Prerequisites:** Visual Studio with .NET Framework 4.8 installed

---

## Step 1: Create Project Folder Structure

### Commit Message
```
feat: Add project folder structure for organized codebase
```

### Description
Create the complete folder structure for the application. This includes folders for Models, Data Access Layer (DAL), Services, Helpers, ViewModels, and Resources. This organization will help maintain clean separation of concerns throughout the project.

### Files Created

**Folders to Create:**
- `Models/` - Entity models and ViewModels (may already exist)
- `DAL/` - Data Access Layer for ADO.NET repositories (may already exist)
- `Services/` - Business logic services (may already exist)
- `Helpers/` - Utility classes (may already exist)
- `ViewModels/` - ViewModel classes for data transfer
- `Resources/` - RESX localization files (may already exist)

**Placeholder Files:**
- `ViewModels/.gitkeep` - Placeholder to ensure folder is tracked in Git
- `DAL/.gitkeep` - Placeholder (if folder is empty)
- `Services/.gitkeep` - Placeholder (if folder is empty)
- `Helpers/.gitkeep` - Placeholder (if folder is empty)
- `Resources/.gitkeep` - Placeholder (if folder is empty)

### Code

**ViewModels/.gitkeep**
```
# ViewModels folder placeholder
```

**DAL/.gitkeep**
```
# DAL folder placeholder
```

**Services/.gitkeep**
```
# Services folder placeholder
```

**Helpers/.gitkeep**
```
# Helpers folder placeholder
```

**Resources/.gitkeep**
```
# Resources folder placeholder
```

### Notes
- If folders already exist, skip creation
- Use `.gitkeep` files to ensure empty folders are tracked in Git
- Folder structure follows MVC best practices

---

## Step 2: Configure Web.config with Connection String and Settings

### Commit Message
```
feat: Configure Web.config for database connection and application settings
```

### Description
Configure `Web.config` with connection string for SQL Server, session timeout settings (3 hours), idle timeout (2 minutes), and encryption key configuration. This establishes the foundation for database connectivity and application-wide settings.

### Files Created/Modified

**Files Created:**
- `Web.config` - Main configuration file (if not exists)

**Files Modified:**
- `Web.config` - Add connection strings, appSettings, and system.web configuration

### Code

**Web.config**
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <!-- Configuration sections -->
  </configSections>
  
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Vts_EcommerceDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <appSettings>
    <!-- Session timeout in minutes (3 hours = 180 minutes) -->
    <add key="SessionTimeout" value="180" />
    <!-- Idle timeout in minutes (2 minutes) -->
    <add key="IdleTimeout" value="2" />
    <!-- Encryption key for AES (32 bytes = 256 bits) - CHANGE THIS IN PRODUCTION -->
    <add key="EncryptionKey" value="Your32ByteEncryptionKeyHere!!" />
  </appSettings>
  
  <system.web>
    <compilation debug="true" targetFramework="4.8" />
    <httpRuntime targetFramework="4.8" />
    
    <!-- Session configuration: 3-hour timeout -->
    <sessionState timeout="180" mode="InProc" />
    
    <!-- Forms authentication configuration -->
    <authentication mode="Forms">
      <forms loginUrl="~/Account/Login" timeout="180" />
    </authentication>
    
    <!-- Custom error pages -->
    <customErrors mode="Off" />
    
    <!-- Globalization settings -->
    <globalization culture="en-US" uiCulture="en-US" />
  </system.web>
  
  <system.webServer>
    <defaultDocument>
      <files>
        <clear />
        <add value="Home/Index" />
      </files>
    </defaultDocument>
  </system.webServer>
</configuration>
```

### Configuration Details

**Connection String:**
- Uses LocalDB for development
- Database name: `Vts_EcommerceDB`
- Integrated Security (Windows Authentication)
- Change connection string for production environment

**App Settings:**
- `SessionTimeout`: 180 minutes (3 hours)
- `IdleTimeout`: 2 minutes (for idle detection)
- `EncryptionKey`: 32-byte key for AES encryption (MUST be changed in production)

**Session State:**
- InProc mode (stored in memory)
- 180-minute timeout
- Supports session-based authentication

**Forms Authentication:**
- Login URL: `~/Account/Login`
- Timeout: 180 minutes

### Notes
- ⚠️ **IMPORTANT**: Change `EncryptionKey` to a secure random 32-byte key in production
- Update connection string for production SQL Server
- Session mode can be changed to SQL Server or State Server for web farms

---

## Step 3: Create Base Helper Classes

### Commit Message
```
feat: Add base helper classes (AdoHelper, EncryptionService, SessionManager)
```

### Description
Create the foundation helper classes that will be used throughout the application. These include:
- `AdoHelper`: ADO.NET database operations helper
- `EncryptionService`: AES encryption service (empty skeleton)
- `SessionManager`: Session management helper (empty skeleton)

These classes provide the infrastructure for database access, encryption, and session management.

### Files Created

**Files Created:**
- `DAL/AdoHelper.cs` - ADO.NET database helper with full implementation
- `Helpers/EncryptionService.cs` - Encryption service skeleton
- `Helpers/SessionManager.cs` - Session manager skeleton

### Code

#### DAL/AdoHelper.cs

```csharp
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Vts_Ecommerce.DAL
{
    /// <summary>
    /// ADO.NET Database Helper Class
    /// Provides methods for database operations using SqlConnection, SqlCommand, SqlDataReader
    /// All CRUD operations will use this helper class
    /// </summary>
    public static class AdoHelper
    {
        /// <summary>
        /// Gets the connection string from Web.config
        /// </summary>
        private static string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"];
            if (connectionString == null)
            {
                throw new ConfigurationErrorsException("DefaultConnection connection string not found in Web.config");
            }
            return connectionString.ConnectionString;
        }

        /// <summary>
        /// Creates and returns a new SqlConnection
        /// </summary>
        /// <returns>New SqlConnection instance</returns>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// Executes a non-query command (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="commandText">SQL command text or stored procedure name</param>
        /// <param name="commandType">Command type (Text or StoredProcedure)</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Number of rows affected</returns>
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
        /// Executes a query and returns a SqlDataReader
        /// Note: The connection is closed when the reader is disposed
        /// </summary>
        /// <param name="commandText">SQL command text or stored procedure name</param>
        /// <param name="commandType">Command type (Text or StoredProcedure)</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>SqlDataReader instance</returns>
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

        /// <summary>
        /// Executes a query and returns a single value (first column of first row)
        /// </summary>
        /// <param name="commandText">SQL command text or stored procedure name</param>
        /// <param name="commandType">Command type (Text or StoredProcedure)</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Object value or null</returns>
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
        /// Executes a command within an existing transaction
        /// Used for multi-statement operations that must succeed or fail together
        /// </summary>
        /// <param name="transaction">Active SqlTransaction</param>
        /// <param name="commandText">SQL command text or stored procedure name</param>
        /// <param name="commandType">Command type (Text or StoredProcedure)</param>
        /// <param name="parameters">SQL parameters</param>
        /// <returns>Number of rows affected</returns>
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

        /// <summary>
        /// Creates a SqlParameter with specified name and value
        /// </summary>
        /// <param name="parameterName">Parameter name (with or without @)</param>
        /// <param name="value">Parameter value</param>
        /// <param name="dbType">SQL Server data type</param>
        /// <returns>SqlParameter instance</returns>
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

        /// <summary>
        /// Creates a SqlParameter with specified name, value, and size
        /// Used for string parameters with specific length
        /// </summary>
        /// <param name="parameterName">Parameter name (with or without @)</param>
        /// <param name="value">Parameter value</param>
        /// <param name="dbType">SQL Server data type</param>
        /// <param name="size">Parameter size (for strings)</param>
        /// <returns>SqlParameter instance</returns>
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

        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        /// <returns>SqlTransaction instance</returns>
        public static SqlTransaction BeginTransaction()
        {
            var connection = GetConnection();
            connection.Open();
            return connection.BeginTransaction();
        }
    }
}
```

#### Helpers/EncryptionService.cs

```csharp
using System;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace Vts_Ecommerce.Helpers
{
    /// <summary>
    /// Encryption Service for Two-Way AES Encryption
    /// Used for password encryption/decryption
    /// Implements AES-256 encryption using built-in .NET classes
    /// </summary>
    public static class EncryptionService
    {
        // TODO: Implement AES encryption methods
        // Methods to implement:
        // - Encrypt(string plainText) - Encrypts plain text using AES
        // - Decrypt(string cipherText) - Decrypts encrypted text
        // - GetEncryptionKey() - Retrieves encryption key from Web.config
        // - GenerateIV() - Generates initialization vector for AES
        // - ConvertToBase64(byte[] data) - Helper for base64 encoding
        // - ConvertFromBase64(string base64) - Helper for base64 decoding
        
        /// <summary>
        /// Gets the encryption key from Web.config
        /// Key must be exactly 32 bytes (256 bits) for AES-256
        /// </summary>
        /// <returns>Encryption key as byte array</returns>
        private static byte[] GetEncryptionKey()
        {
            // TODO: Implement
            // Get key from Web.config appSettings["EncryptionKey"]
            // Convert string to byte array (UTF8 encoding)
            // Validate key length is 32 bytes
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encrypts plain text using AES-256 encryption
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <returns>Encrypted text as Base64 string</returns>
        public static string Encrypt(string plainText)
        {
            // TODO: Implement AES encryption
            // 1. Get encryption key
            // 2. Generate random IV (16 bytes)
            // 3. Create AesCryptoServiceProvider
            // 4. Encrypt plain text
            // 5. Combine IV + encrypted data
            // 6. Convert to Base64 string
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decrypts encrypted text using AES-256 decryption
        /// </summary>
        /// <param name="cipherText">Encrypted text as Base64 string</param>
        /// <returns>Decrypted plain text</returns>
        public static string Decrypt(string cipherText)
        {
            // TODO: Implement AES decryption
            // 1. Get encryption key
            // 2. Convert Base64 string to byte array
            // 3. Extract IV (first 16 bytes)
            // 4. Extract encrypted data (remaining bytes)
            // 5. Create AesCryptoServiceProvider
            // 6. Decrypt data
            // 7. Convert to string (UTF8)
            throw new NotImplementedException();
        }
    }
}
```

#### Helpers/SessionManager.cs

```csharp
using System;
using System.Web;

namespace Vts_Ecommerce.Helpers
{
    /// <summary>
    /// Session Manager for handling user sessions
    /// Manages 3-hour sessions, auto-login, and single-device login
    /// </summary>
    public static class SessionManager
    {
        // Session keys
        private const string SESSION_USER_ID = "UserId";
        private const string SESSION_USERNAME = "Username";
        private const string SESSION_LAST_ACTIVITY = "LastActivity";
        private const string SESSION_ID = "SessionId";

        // TODO: Implement session management methods
        // Methods to implement:
        // - CreateSession(int userId, string username, string sessionId) - Creates new session
        // - GetCurrentUserId() - Gets current user ID from session
        // - GetCurrentUsername() - Gets current username from session
        // - GetCurrentSessionId() - Gets current session ID
        // - IsSessionValid() - Checks if session is valid and not expired
        // - CheckAutoLogin() - Checks if user can auto-login (within 3 hours)
        // - ClearSession() - Clears all session data
        // - UpdateLastActivity() - Updates last activity timestamp
        // - CheckSingleDeviceLogin(int userId, string sessionId) - Validates single-device login
        // - GetSessionTimeout() - Gets session timeout from Web.config

        /// <summary>
        /// Creates a new user session
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="username">Username</param>
        /// <param name="sessionId">Unique session ID</param>
        public static void CreateSession(int userId, string username, string sessionId)
        {
            // TODO: Implement
            // Store userId, username, sessionId, and LastActivity in session
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current user ID from session
        /// </summary>
        /// <returns>User ID or 0 if not logged in</returns>
        public static int GetCurrentUserId()
        {
            // TODO: Implement
            // Return session[SESSION_USER_ID] or 0
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current username from session
        /// </summary>
        /// <returns>Username or empty string if not logged in</returns>
        public static string GetCurrentUsername()
        {
            // TODO: Implement
            // Return session[SESSION_USERNAME] or string.Empty
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the current session ID
        /// </summary>
        /// <returns>Session ID or empty string</returns>
        public static string GetCurrentSessionId()
        {
            // TODO: Implement
            // Return session[SESSION_ID] or string.Empty
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the current session is valid and not expired
        /// </summary>
        /// <returns>True if session is valid, false otherwise</returns>
        public static bool IsSessionValid()
        {
            // TODO: Implement
            // Check if session exists and LastActivity is within timeout period
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears all session data
        /// </summary>
        public static void ClearSession()
        {
            // TODO: Implement
            // Clear all session variables
            // Abandon session
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the last activity timestamp
        /// </summary>
        public static void UpdateLastActivity()
        {
            // TODO: Implement
            // Update session[SESSION_LAST_ACTIVITY] to current DateTime
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets session timeout in minutes from Web.config
        /// </summary>
        /// <returns>Session timeout in minutes</returns>
        private static int GetSessionTimeout()
        {
            // TODO: Implement
            // Get from Web.config appSettings["SessionTimeout"]
            // Default to 180 if not found
            throw new NotImplementedException();
        }
    }
}
```

### Code Details

**AdoHelper.cs:**
- ✅ **Fully implemented** - Complete ADO.NET helper with all necessary methods
- Provides connection management, parameter creation, and transaction support
- Methods for ExecuteNonQuery, ExecuteReader, ExecuteScalar
- Transaction support for multi-statement operations
- Parameter creation helpers

**EncryptionService.cs:**
- ⚠️ **Skeleton only** - Contains TODO comments for implementation
- Will implement AES-256 encryption in later phase
- Methods outlined: Encrypt, Decrypt, GetEncryptionKey

**SessionManager.cs:**
- ⚠️ **Skeleton only** - Contains TODO comments for implementation
- Will implement session management in later phase
- Methods outlined: CreateSession, GetCurrentUserId, IsSessionValid, etc.

### Notes

- `AdoHelper` is fully functional and ready to use
- `EncryptionService` and `SessionManager` are placeholders for Phase 12 (Security & Session Management)
- All helper classes use static methods for utility functions
- Error handling will be added in later phases

---

## ✅ Phase 1 Completion Checklist

- [x] Create folder structure (Models, DAL, Services, Helpers, ViewModels, Resources)
- [x] Configure Web.config with connection string
- [x] Configure Web.config with app settings (SessionTimeout, IdleTimeout, EncryptionKey)
- [x] Configure Web.config with session state and authentication
- [x] Create AdoHelper.cs with full ADO.NET implementation
- [x] Create EncryptionService.cs skeleton
- [x] Create SessionManager.cs skeleton

---

## 🚀 Next Steps

After completing Phase 1, proceed to **Phase 2: Database Models & Schema**:
- Create Entity Framework Code First models
- Create ApplicationDbContext
- Generate initial migration
- Add UserSession model for single-device login

---

## 📝 Git Commit Sequence

```bash
# Step 1: Folder Structure
git add Models/ DAL/ Services/ Helpers/ ViewModels/ Resources/
git commit -m "feat: Add project folder structure for organized codebase"

# Step 2: Web.config Configuration
git add Web.config
git commit -m "feat: Configure Web.config for database connection and application settings"

# Step 3: Base Helper Classes
git add DAL/AdoHelper.cs Helpers/EncryptionService.cs Helpers/SessionManager.cs
git commit -m "feat: Add base helper classes (AdoHelper, EncryptionService, SessionManager)"
```

---

## 🔍 Verification

After completing Phase 1, verify:

1. ✅ All folders exist: Models, DAL, Services, Helpers, ViewModels, Resources
2. ✅ Web.config contains connection string and app settings
3. ✅ AdoHelper.cs compiles without errors
4. ✅ EncryptionService.cs and SessionManager.cs exist (skeletons)
5. ✅ Project builds successfully

---

**Phase 1 Status:** ✅ Complete

