

DECLARE @CategoryID INT;

INSERT INTO Categories(
    CategoryName,
    Description
)
VALUES(
    'Prueba TX',
    'Categoría para probar transacciones'
);

SET @CategoryID = SCOPE_IDENTITY();

INSERT INTO Products(
    ProductName,
    UnitPrice,
    CategoryID,
    Discontinued
)
VALUES('Producto 1', 10, @CategoryID, 0);

INSERT INTO Products(
    ProductName,
    UnitPrice,
    CategoryID,
    Discontinued
)
VALUES('Producto 2', 10, @CategoryID, 0);

INSERT INTO Products(
    ProductName,
    UnitPrice,
    CategoryID,
    Discontinued
)
VALUES('Producto 3', 10, @CategoryID, 0);


SELECT *
FROM Products
WHERE CategoryID = @CategoryID;

BEGIN TRANSACTION;

DELETE FROM Products
OUTPUT
    DELETED.ProductID,
    DELETED.ProductName,
    DELETED.UnitPrice,
    DELETED.CategoryID
WHERE CategoryID = @CategoryID;

ROLLBACK;

SELECT *
FROM Products
WHERE CategoryID = @CategoryID;


BEGIN TRANSACTION;

SELECT *
FROM Categories WITH (XLOCK, HOLDLOCK)
WHERE CategoryID = 17;


ROLLBACK;
