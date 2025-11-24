-- Script de creación de base de datos para Orders API
-- Clean Architecture - Database Schema

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OrdersDB')
BEGIN
    CREATE DATABASE OrdersDB;
END
GO

USE OrdersDB;
GO

-- Crear tabla Orders
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CustomerName NVARCHAR(100) NOT NULL,
        ProductName NVARCHAR(100) NOT NULL,
        Quantity INT NOT NULL CHECK (Quantity > 0),
        UnitPrice DECIMAL(18, 2) NOT NULL CHECK (UnitPrice >= 0),
        TotalPrice DECIMAL(18, 2) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        INDEX IX_Orders_CreatedAt (CreatedAt DESC),
        INDEX IX_Orders_CustomerName (CustomerName)
    );
END
GO

-- Insertar datos de ejemplo
INSERT INTO Orders (CustomerName, ProductName, Quantity, UnitPrice, TotalPrice, CreatedAt)
VALUES 
    ('John Doe', 'Laptop', 1, 999.99, 999.99, GETUTCDATE()),
    ('Jane Smith', 'Mouse', 2, 29.99, 59.98, GETUTCDATE()),
    ('Bob Johnson', 'Keyboard', 1, 79.99, 79.99, GETUTCDATE());
GO

PRINT 'Database setup completed successfully!';
