CREATE DATABASE furniture_store;
USE furniture_store;

CREATE TABLE Products (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  name NVARCHAR(255) NOT NULL,
  description nvarchar(MAX),
  price DECIMAL(10,2) NOT NULL
);
CREATE TABLE Invoices (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  date DATETIME NOT NULL,
  customer_id INT NOT NULL FOREIGN KEY REFERENCES Customers(id),
  employee_id INT NOT NULL FOREIGN KEY REFERENCES Employees(id)
);

CREATE TABLE InvoiceDetails (
  invoice_id INT NOT NULL,
  product_id INT NOT NULL,
  quantity INT NOT NULL,
  price DECIMAL(10,2) NOT NULL,
  PRIMARY KEY (invoice_id, product_id),
  FOREIGN KEY (invoice_id) REFERENCES Invoices(id),
  FOREIGN KEY (product_id) REFERENCES Products(id)
);

CREATE TABLE ProductCategories (
  id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  name NVARCHAR(255) NOT NULL,
);
ALTER TABLE Products ADD category_id INT NOT NULL FOREIGN KEY REFERENCES ProductCategories(id);
ALTER TABLE Invoices ADD customer_id nvarchar(128) NOT NULL FOREIGN KEY REFERENCES AspNetUsers(Id);
