-- SQL script to create database and table
-- Run in SQL Server Management Studio if not using LocalDB
-- Uncomment and edit below if you need to create the database:
-- IF DB_ID('SmartCertificateDb') IS NULL
-- BEGIN
--     CREATE DATABASE SmartCertificateDb;
-- END
-- GO
-- USE SmartCertificateDb;

IF OBJECT_ID('dbo.Certificates','U') IS NULL
BEGIN
    CREATE TABLE dbo.Certificates (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SerialNumber NVARCHAR(100) UNIQUE,
        HolderName NVARCHAR(200),
        Issuer NVARCHAR(200),
        IssueDate DATETIME,
        ExpiryDate DATETIME,
        IsRevoked BIT
    );
END
