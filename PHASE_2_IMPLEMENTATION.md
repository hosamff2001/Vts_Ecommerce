# Phase 2: Database Models & EF Code First
## Detailed Implementation Guide

---

## 📋 Overview

This phase creates all entity models, ApplicationDbContext, and sets up Entity Framework Code First for database schema generation. **Important:** Entity Framework is used ONLY for schema generation. All CRUD operations will be implemented using ADO.NET in later phases.

**Duration:** 4 commits  
**Dependencies:** Phase 1 (Project Setup)  
**Prerequisites:** 
- Entity Framework 6.x NuGet package installed
- SQL Server or LocalDB installed
- Visual Studio with Package Manager Console

---

## Step 1: Install Entity Framework 6.x NuGet Package

### Commit Message
```
feat: Install Entity Framework 6.x for Code First migrations
```

### Description
Install Entity Framework 6.x NuGet package which is required for Code First migrations in .NET Framework projects.

### Commands to Run

**Package Manager Console:**
```powershell
Install-Package EntityFramework -Version 6.4.4
```

**Or via NuGet Package Manager UI:**
- Right-click project → Manage NuGet Packages
- Search for "EntityFramework"
- Install version 6.4.4

### Files Modified
- `Vts_Ecommerce.csproj` - Adds EntityFramework package reference
- `packages.config` - Package configuration (if using packages.config)
- `Web.config` - Adds Entity Framework configuration section

### Notes
- Entity Framework 6.x is the latest version for .NET Framework
- This package is ONLY for schema generation, not for data operations

---

## Step 2: Create Entity Models

### Commit Message
```
feat: Add Entity Framework Code First models for all entities
```

### Description
Create all entity models that represent database tables. These models will be used ONLY for Entity Framework Code First migrations to generate the database schema. All CRUD operations will use ADO.NET.

### Files Created

**Models Created:**
- `Models/User.cs` - User entity for authentication
- `Models/UserSession.cs` - User session tracking for single-device login
- `Models/Category.cs` - Category entity for product categorization
- `Models/Product.cs` - Product entity for inventory
- `Models/Customer.cs` - Customer entity for customer management
- `Models/SalesInvoice.cs` - Sales invoice entity
- `Models/SalesInvoiceItem.cs` - Invoice line items entity

### Entity Relationships

1. **User → UserSession** (1 to Many)
   - One user can have multiple sessions
   - Used for single-device login enforcement

2. **User → SalesInvoice** (1 to Many)
   - One user can create multiple invoices
   - Tracks who created each invoice

3. **Category → Product** (1 to Many)
   - One category can have multiple products
   - Products must belong to a category

4. **Customer → SalesInvoice** (1 to Many)
   - One customer can have multiple invoices

5. **SalesInvoice → SalesInvoiceItem** (1 to Many)
   - One invoice can have multiple line items
   - Cascade delete: deleting invoice deletes items

6. **Product → SalesInvoiceItem** (1 to Many)
   - One product can appear in multiple invoice items

### Code

All model files have been created. See individual model files for complete code.

**Key Points:**
- All models use Data Annotations for configuration
- Navigation properties use `virtual` for lazy loading (though we won't use EF for queries)
- Foreign keys are explicitly defined
- Decimal fields use `decimal(18,2)` precision
- String lengths are specified for all string properties

---

## Step 3: Create ApplicationDbContext

### Commit Message
```
feat: Add ApplicationDbContext with Fluent API configurations
```

### Description
Create the DbContext class that defines all DbSets and configures entity relationships using Fluent API. This context will be used ONLY for migrations.

### Files Created

- `DAL/ApplicationDbContext.cs` - Main DbContext class

### Code

The ApplicationDbContext includes:
- DbSets for all entities
- Fluent API configurations in `OnModelCreating` method
- Relationship configurations (1-to-many, foreign keys)
- Index configurations (unique indexes for Username, SessionId, InvoiceNumber)
- Precision settings for decimal fields
- Cascade delete settings

**Key Features:**
- Disables automatic database initialization
- Configures all relationships
- Sets up unique indexes
- Configures decimal precision
- Sets cascade delete behavior

### Fluent API Configurations

**Unique Indexes:**
- `Users.Username` - Unique index
- `UserSessions.SessionId` - Unique index
- `SalesInvoices.InvoiceNumber` - Unique index

**Relationships:**
- User → UserSession (1:Many, no cascade)
- User → SalesInvoice (1:Many, no cascade)
- Category → Product (1:Many, no cascade)
- Customer → SalesInvoice (1:Many, no cascade)
- SalesInvoice → SalesInvoiceItem (1:Many, cascade delete)
- Product → SalesInvoiceItem (1:Many, no cascade)

---

## Step 4: Enable Migrations and Create Initial Migration

### Commit Message
```
feat: Enable Entity Framework migrations and create initial migration
```

### Description
Enable Entity Framework Code First migrations and generate the initial migration that will create all database tables.

### Commands to Run

**Step 4.1: Enable Migrations**
```powershell
Enable-Migrations -ContextTypeName Vts_Ecommerce.DAL.ApplicationDbContext
```

This command will:
- Create `Migrations` folder
- Create `Configuration.cs` file
- Set up migration infrastructure

**Step 4.2: Create Initial Migration**
```powershell
Add-Migration InitialCreate
```

This command will:
- Generate migration file: `Migrations/YYYYMMDDHHMMSS_InitialCreate.cs`
- Include all table creation scripts
- Include all indexes and relationships

**Step 4.3: Review Migration File**
- Open the generated migration file
- Verify all tables are included
- Check that relationships are correct

### Files Created

- `Migrations/Configuration.cs` - Migration configuration
- `Migrations/YYYYMMDDHHMMSS_InitialCreate.cs` - Initial migration file

### Migration File Structure

The migration file will contain:
- `Up()` method - Creates all tables, indexes, and relationships
- `Down()` method - Drops all tables (for rollback)

---

## Step 5: Update Database

### Commit Message
```
feat: Apply initial migration to create database schema
```

### Description
Apply the initial migration to create the database and all tables in SQL Server.

### Commands to Run

**Update Database:**
```powershell
Update-Database
```

This command will:
- Create database `Vts_EcommerceDB` (if it doesn't exist)
- Create all tables: Users, UserSessions, Categories, Products, Customers, SalesInvoices, SalesInvoiceItems
- Create all indexes and foreign keys
- Create all relationships

### Verification

After running `Update-Database`, verify:

1. **Database Created:**
   - Check SQL Server Object Explorer
   - Database `Vts_EcommerceDB` should exist

2. **Tables Created:**
   - `Users`
   - `UserSessions`
   - `Categories`
   - `Products`
   - `Customers`
   - `SalesInvoices`
   - `SalesInvoiceItems`

3. **Relationships Created:**
   - Foreign keys between related tables
   - Cascade delete on SalesInvoiceItems

4. **Indexes Created:**
   - Unique index on `Users.Username`
   - Unique index on `UserSessions.SessionId`
   - Unique index on `SalesInvoices.InvoiceNumber`

### Troubleshooting

**If migration fails:**
1. Check connection string in `Web.config`
2. Ensure SQL Server/LocalDB is running
3. Check database permissions
4. Review error messages in Package Manager Console

**To rollback:**
```powershell
Update-Database -TargetMigration:0
```

This will drop all tables and remove the database.

---

## 📊 Database Schema Summary

### Tables Created

1. **Users**
   - Id (PK, Identity)
   - Username (Unique, Required, MaxLength: 50)
   - Password (Required, MaxLength: 500)
   - Email (MaxLength: 100)
   - IsActive (Required, Default: true)
   - CreatedDate (Required, Default: Now)

2. **UserSessions**
   - Id (PK, Identity)
   - UserId (FK to Users, Required)
   - SessionId (Unique, Required, MaxLength: 100)
   - DeviceInfo (MaxLength: 500)
   - LoginTime (Required, Default: Now)
   - LastActivityTime (Required, Default: Now)
   - IsActive (Required, Default: true)

3. **Categories**
   - Id (PK, Identity)
   - Name (Required, MaxLength: 100)
   - Description (MaxLength: 500)
   - IsActive (Required, Default: true)

4. **Products**
   - Id (PK, Identity)
   - Name (Required, MaxLength: 100)
   - Description (MaxLength: 1000)
   - Price (Required, decimal(18,2))
   - StockQuantity (Required, Default: 0)
   - CategoryId (FK to Categories, Required)
   - IsActive (Required, Default: true)

5. **Customers**
   - Id (PK, Identity)
   - Name (Required, MaxLength: 100)
   - Email (MaxLength: 100)
   - Phone (MaxLength: 20)
   - Address (MaxLength: 500)
   - IsActive (Required, Default: true)

6. **SalesInvoices**
   - Id (PK, Identity)
   - InvoiceNumber (Unique, Required, MaxLength: 50)
   - CustomerId (FK to Customers, Required)
   - InvoiceDate (Required, Default: Now)
   - SubTotal (Required, decimal(18,2), Default: 0)
   - ItemDiscount (Required, decimal(18,2), Default: 0)
   - InvoiceDiscount (Required, decimal(18,2), Default: 0)
   - Total (Required, decimal(18,2), Default: 0)
   - CreatedBy (FK to Users, Required)
   - CreatedDate (Required, Default: Now)

7. **SalesInvoiceItems**
   - Id (PK, Identity)
   - InvoiceId (FK to SalesInvoices, Required)
   - ProductId (FK to Products, Required)
   - Quantity (Required, Default: 1)
   - UnitPrice (Required, decimal(18,2))
   - ItemDiscount (Required, decimal(18,2), Default: 0)
   - LineTotal (Required, decimal(18,2))

### Relationships

- Users → UserSessions (1:Many, No Cascade)
- Users → SalesInvoices (1:Many, No Cascade)
- Categories → Products (1:Many, No Cascade)
- Customers → SalesInvoices (1:Many, No Cascade)
- SalesInvoices → SalesInvoiceItems (1:Many, Cascade Delete)
- Products → SalesInvoiceItems (1:Many, No Cascade)

### Indexes

- `IX_Username` on `Users.Username` (Unique)
- `IX_SessionId` on `UserSessions.SessionId` (Unique)
- `IX_InvoiceNumber` on `SalesInvoices.InvoiceNumber` (Unique)

---

## ✅ Phase 2 Completion Checklist

- [x] Install Entity Framework 6.x NuGet package
- [x] Create User entity model
- [x] Create UserSession entity model
- [x] Create Category entity model
- [x] Create Product entity model
- [x] Create Customer entity model
- [x] Create SalesInvoice entity model
- [x] Create SalesInvoiceItem entity model
- [x] Create ApplicationDbContext
- [x] Configure Fluent API relationships
- [x] Configure unique indexes
- [x] Enable migrations
- [x] Create initial migration
- [x] Update database
- [x] Verify database schema

---

## 🚀 Next Steps

After completing Phase 2, proceed to **Phase 3: Helper Classes & Utilities**:
- Implement EncryptionService (AES encryption)
- Implement SessionManager (session management)
- Add validation helpers

---

## 📝 Git Commit Sequence

```bash
# Step 1: Install EF Package
git add packages.config Vts_Ecommerce.csproj Web.config
git commit -m "feat: Install Entity Framework 6.x for Code First migrations"

# Step 2: Create Models
git add Models/User.cs Models/UserSession.cs Models/Category.cs Models/Product.cs Models/Customer.cs Models/SalesInvoice.cs Models/SalesInvoiceItem.cs
git commit -m "feat: Add Entity Framework Code First models for all entities"

# Step 3: Create DbContext
git add DAL/ApplicationDbContext.cs
git commit -m "feat: Add ApplicationDbContext with Fluent API configurations"

# Step 4: Enable Migrations
git add Migrations/
git commit -m "feat: Enable Entity Framework migrations and create initial migration"

# Step 5: Update Database (no code changes, just run command)
# Note: Database changes are not committed to Git
# Run: Update-Database
```

---

## 🔍 Verification Commands

**Check Migration Status:**
```powershell
Get-Migrations
```

**Check Database Connection:**
```powershell
Update-Database -Verbose
```

**View Generated SQL:**
```powershell
Update-Database -Script
```

**Rollback Migration:**
```powershell
Update-Database -TargetMigration:0
```

---

## ⚠️ Important Notes

1. **Entity Framework is ONLY for Schema Generation**
   - Do NOT use DbContext for data operations
   - All CRUD will use ADO.NET (AdoHelper)
   - Models are only for migrations

2. **Database Changes**
   - Database files are NOT committed to Git
   - Only migration files are committed
   - Each developer runs `Update-Database` locally

3. **Future Migrations**
   - If schema changes are needed, create new migrations
   - Use `Add-Migration MigrationName`
   - Review migration before applying

4. **Production Deployment**
   - Run migrations on production database
   - Backup database before migration
   - Test migrations on staging first

---

**Phase 2 Status:** ✅ Complete

