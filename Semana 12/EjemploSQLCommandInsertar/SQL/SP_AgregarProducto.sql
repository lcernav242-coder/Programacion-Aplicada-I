USE Northwind;
GO
CREATE OR ALTER PROCEDURE dbo.SP_AgregarProducto
    @Nombre NVARCHAR(40),
    @Precio MONEY,
    @Categoria NVARCHAR(15)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Products WHERE ProductName = @Nombre)
    BEGIN
        ;THROW 50001, 'El producto ya existe en la base de datos.', 1;
        RETURN;
    END

    DECLARE @IdCategoria INT;

    SELECT @IdCategoria = CategoryID 
    FROM dbo.Categories 
    WHERE CategoryName = @Categoria;

    IF @IdCategoria IS NULL
    BEGIN
        INSERT INTO dbo.Categories (CategoryName)
        VALUES (@Categoria);

        SELECT @IdCategoria = CategoryID 
        FROM dbo.Categories 
        WHERE CategoryName = @Categoria;
    END;

    INSERT INTO dbo.Products (ProductName, UnitPrice, CategoryID)
    VALUES (@Nombre, @Precio, @IdCategoria);
END;
GO