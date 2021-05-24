IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ProductDb')
BEGIN
	CREATE DATABASE [ProductDb]
END
GO
	USE [ProductDb]
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'product' AND xtype = 'U')
BEGIN
	CREATE TABLE product
	(
		id INT PRIMARY KEY NOT NULL IDENTITY(1,1),
		name VARCHAR(100) NOT NULL,
		code VARCHAR(100) NOT NULL, 
		start_validity_date DATE NOT NULL,
		end_validity_date DATE NOT NULL
	)
END