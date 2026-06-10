CREATE TABLE Transactions
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    TransactionDate DATE NOT NULL,
    Type NVARCHAR(20) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Description NVARCHAR(200)
);
