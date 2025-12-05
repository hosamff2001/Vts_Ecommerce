-- Create Vts_EcommerceDB Database
-- Run this script in SQL Server Management Studio or via sqlcmd

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Vts_EcommerceDB')
BEGIN
    CREATE DATABASE Vts_EcommerceDB;
END
GO

USE Vts_EcommerceDB;
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE dbo.Users (
        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        Username NVARCHAR(50) NOT NULL,
        Password NVARCHAR(500) NOT NULL,
        Email NVARCHAR(100),
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE()
    );
    
    CREATE UNIQUE INDEX IX_Users_Username ON dbo.Users(Username);
END
GO

-- Create Categories table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
BEGIN
    CREATE TABLE dbo.Categories (
        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        IsActive BIT NOT NULL DEFAULT 1
    );
END
GO

-- Create Products table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
BEGIN
    CREATE TABLE dbo.Products (
        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(1000),
        CostPrice DECIMAL(18, 2) NOT NULL DEFAULT 0,
        SellingPrice DECIMAL(18, 2) NOT NULL DEFAULT 0,
        StockQuantity INT NOT NULL DEFAULT 0,
        CategoryId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id)
    );
    
    CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
END
GO

-- Create Customers table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Customers')
BEGIN
    CREATE TABLE dbo.Customers (
        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100),
        Phone NVARCHAR(20),
        Address NVARCHAR(500),
        Notes NVARCHAR(500),
        IsActive BIT NOT NULL DEFAULT 1
    );
END
GO

-- Create SalesInvoices table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SalesInvoices')
BEGIN
    CREATE TABLE dbo.SalesInvoices (
        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
        CustomerId INT NOT NULL,
        InvoiceDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        SubTotal DECIMAL(18, 2) NOT NULL,
        ItemDiscount DECIMAL(18, 2) NOT NULL DEFAULT 0,
        InvoiceDiscount DECIMAL(18, 2) NOT NULL DEFAULT 0,
        Total DECIMAL(18, 2) NOT NULL,
        CreatedBy INT NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_SalesInvoices_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id),
        CONSTRAINT FK_SalesInvoices_Users FOREIGN KEY (CreatedBy) REFERENCES dbo.Users(Id)
    );
    
    CREATE INDEX IX_SalesInvoices_CustomerId ON dbo.SalesInvoices(CustomerId);
    CREATE INDEX IX_SalesInvoices_CreatedBy ON dbo.SalesInvoices(CreatedBy);
    CREATE INDEX IX_SalesInvoices_InvoiceNumber ON dbo.SalesInvoices(InvoiceNumber);
END
GO

-- Create SalesInvoiceItems table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SalesInvoiceItems')
BEGIN
    CREATE TABLE dbo.SalesInvoiceItems (
        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        InvoiceId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        ItemDiscount DECIMAL(18, 2) NOT NULL DEFAULT 0,
        LineTotal DECIMAL(18, 2) NOT NULL,
        CONSTRAINT FK_SalesInvoiceItems_SalesInvoices FOREIGN KEY (InvoiceId) REFERENCES dbo.SalesInvoices(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SalesInvoiceItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
    );
    
    CREATE INDEX IX_SalesInvoiceItems_InvoiceId ON dbo.SalesInvoiceItems(InvoiceId);
    CREATE INDEX IX_SalesInvoiceItems_ProductId ON dbo.SalesInvoiceItems(ProductId);
END
GO

-- Create UserSessions table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserSessions')
BEGIN
    CREATE TABLE dbo.UserSessions (
        Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
        UserId INT NOT NULL,
        SessionId NVARCHAR(100) NOT NULL UNIQUE,
        DeviceInfo NVARCHAR(500),
        LoginTime DATETIME2 NOT NULL DEFAULT GETDATE(),
        LastActivityTime DATETIME2 NOT NULL DEFAULT GETDATE(),
        IsActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_UserSessions_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
    
    CREATE INDEX IX_UserSessions_UserId ON dbo.UserSessions(UserId);
    CREATE INDEX IX_UserSessions_SessionId ON dbo.UserSessions(SessionId);
END
GO

PRINT 'Database Vts_EcommerceDB created successfully!'
