CREATE DATABASE Supermarket_Management
GO

USE Supermarket_Management
GO

CREATE TABLE UserAccount(
	Username NVARCHAR(50) PRIMARY KEY,
	Password NVARCHAR(50) NOT NULL,
	Role NVARCHAR(50) NOT NULL,
	Name NVARCHAR(50) NOT NULL,
	IsActive NVARCHAR(50),
	Email NVARCHAR(50),
)

CREATE TABLE Supplier(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Supplier NVARCHAR(50) NOT NULL,
	Address TEXT NOT NULL,
	ContactPerson NVARCHAR(50) NOT NULL,
	Phone NVARCHAR(50) NOT NULL,
	Email VARCHAR(50) NOT NULL,
	Fax VARCHAR(50),
)


CREATE TABLE Stock(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Refno NVARCHAR(50),
	Pcode NVARCHAR(50),
	Qty INT DEFAULT(0),
	Stock_date DATE,
	Stockinby NVARCHAR(50),
	Status NVARCHAR(50) DEFAULT('Pending'),
	Supplier_ID INT,
)

CREATE TABLE Product(
	Pcode NVARCHAR(50) PRIMARY KEY,
	Barcode NVARCHAR(50),
	Description NVARCHAR(MAX),
	Brand_ID INT NOT NULL,
	Category_ID INT NOT NULL,
	Price DECIMAL(18,2) NOT NULL,
	Quantity INT DEFAULT(0),
	Reorder INT,
)

CREATE TABLE Brand(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Brand NVARCHAR(50) NOT NULL,
)

CREATE TABLE Category(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Category NVARCHAR(50) NOT NULL,
)

CREATE TABLE Cart(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Transno NVARCHAR(50) NOT NULL,
	Pcode NVARCHAR(50) NOT NULL,
	Price DECIMAL(18,2),
	Qty INT DEFAULT(0),
	Disc_percent DECIMAL(18,2) DEFAULT(0),
	Disc DECIMAL(18,2) DEFAULT(0),
	Total DECIMAL(18,2),
	Sdate DATE,
	Status NVARCHAR(50) DEFAULT('Pending'),
	Cashier NVARCHAR(50),
);


CREATE TRIGGER ComputeTotal
	ON Cart
	AFTER INSERT,UPDATE,DELETE
AS
BEGIN
	SET NOCOUNT ON
	UPDATE Cart SET Disc = Qty * Price * Disc_percent
	UPDATE Cart SET Total = Qty * Price - Disc
END

CREATE TABLE Cancel(
	Id INT IDENTITY(1,1),
	Transno NVARCHAR(50) NOT NULL,
	Pcode NVARCHAR(50) NOT NULL,
	Price DECIMAL(18,2),
	Qty INT,
	Total DECIMAL(18,2),
	Sdate DATETIME,
	CancelledBy NVARCHAR(50),
	Reason TEXT,
	Action NVARCHAR(50),
	PRIMARY KEY(Id)
)


CREATE TABLE Store(
	Store NVARCHAR(50),
	Address NVARCHAR(max),
)

CREATE TABLE Product_Status_Report(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Pcode NVARCHAR(50) NOT NULL,
	Qty INT,
	Status NVARCHAR(50),
	Sdate Datetime,
	Inspector NVARCHAR(50),
)

CREATE TRIGGER Adjustment_Stock_Product  -- Lay so luong product tu stock
ON Product
AFTER UPDATE
AS
BEGIN
	UPDATE Stock SET Qty = Stock.Qty + (Select Quantity from deleted where deleted.Pcode = Stock.Pcode) - (Select Quantity from inserted where inserted.Pcode = Stock.Pcode)
	WHERE Stock.Pcode in (SELECT Pcode from inserted )
END

CREATE TRIGGER Product1_Report -- Lay so luong tu product vo report
ON Product_Status_Report
AFTER INSERT
AS
BEGIN
	UPDATE Product SET Quantity = Product.Quantity - (Select Qty from inserted where inserted.Pcode = Product.Pcode)
	WHERE Product.Pcode in (SELECT Pcode from inserted )
END

CREATE TRIGGER Product_Report -- Lay so luong tu product vo report
ON Product_Status_Report
AFTER UPDATE,DELETE
AS
BEGIN
	UPDATE Product SET Quantity = Product.Quantity + (Select Qty from deleted where deleted.Pcode = Product.Pcode) - (Select Qty from inserted where inserted.Pcode = Product.Pcode)
	WHERE Product.Pcode in (SELECT Pcode from inserted )
END

CREATE TRIGGER Adjustment_Product_Cart -- Lay so luong cart tu product update
ON Cart
AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON
	UPDATE Product SET Quantity = Product.Quantity + d.Qty - i.Qty
	FROM Product INNER JOIN inserted i on Product.Pcode = i.Pcode INNER JOIN deleted d on d.Pcode = Product.Pcode
END

CREATE TRIGGER Adjustment1_Product_Cart -- Lay so luong cart tu product insert 	
ON Cart
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON
	UPDATE Product SET Quantity = Product.Quantity - i.Qty
	FROM Product INNER JOIN inserted i on Product.Pcode = i.Pcode
END

Update Product set Quantity = Quantity - 1 where Pcode = 'MKKM'

SELECT * FROM Product