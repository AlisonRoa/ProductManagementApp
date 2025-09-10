/* ===========================================================================================================
   Sistema: Product Management (Prueba T�cnica)
   Autor:   Alison Judith Macis Roa
   
   Notas de dise�o:
     - Se separa el cat�logo de estados (StatusCatalog) para reutilizarlo en Productos, Opciones y Proveedores.
     - Se modela Proveedor (Supplier) y Tipo de Proveedor (SupplierType) para enriquecer el dato �nombre del proveedor�.
     - La existencia (stock) se maneja en una tabla separada (Stock) para permitir actualizaciones de inventario sin
       afectar los metadatos del producto (precio, proveedor, etc.).
     - Las validaciones de formato (correo .com y tel�fono dddd-dddd) se implementar�n en la CAPA DE APLICACI�N,
       tal como se acord� (no en la BD).

   Requisitos cubiertos (resumen):
     1a. Productos: c�digo, nombre, existencia (via PM.Stock), estado (FK a PM.StatusCatalog), nombre del proveedor (via FK a PM.Supplier).
     1b. Usuarios: nombre de usuario, contrase�a (hash), nombre, apellido(s), correo, tel�fono, fecha de creaci�n.
     1c. Opciones: nombre de la opci�n, producto relacionado, estado.
     2-6. Se soporta: Login, Registro, Listado de productos con filtros por estado, y CRUD de opciones por producto (en la app).
     7. GitHub con commits por punto (esto se gestiona fuera de BD).

=========================================================================================================== */

-- ==========================================
-- 1) Creaci�n de Base de Datos
-- ==========================================
CREATE DATABASE ProductManagement;
GO

USE ProductManagement;
GO

-- ==========================================
-- 2) Creaci�n de esquema l�gico
-- ==========================================
CREATE SCHEMA PM;
GO


-- ===========================================================================================================
-- 3) Cat�logo de Estados (Activo / Inactivo)
-- ===========================================================================================================
CREATE TABLE PM.StatusCatalog (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode  VARCHAR(20)  NOT NULL,
    StatusName  VARCHAR(80)  NOT NULL
);
GO


-- ===========================================================================================================
-- 4) Tipo de Proveedor (opcional, para clasificaci�n)
--    - �til para segmentar proveedores (Mayorista, Minorista, Internacional, etc.)
-- ===========================================================================================================

CREATE TABLE PM.SupplierTypes (
    Id                INT IDENTITY(1,1) PRIMARY KEY,
    SupplierTypeCode  VARCHAR(20) NOT NULL,
    SupplierTypeName  VARCHAR(80) NOT NULL
);
GO


-- ===========================================================================================================
-- 5) Proveedores
-- ===========================================================================================================
CREATE TABLE PM.Suppliers (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    SupplierCode    VARCHAR(20) NOT NULL,
    SupplierName    VARCHAR(80) NOT NULL,
    TelephoneNumber VARCHAR(20) NOT NULL,
    Email           VARCHAR(80) NOT NULL,
    StatusId        INT NOT NULL CONSTRAINT FK_Suppliers_Status REFERENCES PM.StatusCatalog(Id),
    SupplierTypesId  INT NOT NULL CONSTRAINT FK_Suppliers_SupplierTypes REFERENCES PM.SupplierTypes(Id)
);
GO


-- ===========================================================================================================
-- 6) Productos
-- ===========================================================================================================
CREATE TABLE PM.Products (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode   VARCHAR(20)  NOT NULL,
    ProductName   VARCHAR(80)  NOT NULL,
    PricePerUnit  DECIMAL(8,2) NOT NULL,
    BasicUnit     VARCHAR(80)  NOT NULL,
    StatusId      INT NOT NULL CONSTRAINT FK_Products_Status REFERENCES PM.StatusCatalog(Id),
    SuppliersId    INT NOT NULL CONSTRAINT FK_Products_Suppliers REFERENCES PM.Suppliers(Id)
);
GO


-- ===========================================================================================================
-- 7) Stock (Existencia)
--    - Maneja las existencias por producto (Inventario).
--    - Se separa de Productos para simplificar operaciones de actualizaci�n de inventario.
--    - 'LastUpdate' permite registrar el momento de la �ltima modificaci�n.
-- ===========================================================================================================
CREATE TABLE PM.Stocks (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    InStock    DECIMAL(8,2) NOT NULL,
    LastUpdate DATETIME2    NOT NULL,
    ProductsId  INT NOT NULL CONSTRAINT FK_Stocks_Products REFERENCES PM.Products(Id)
);
GO


-- ===========================================================================================================
-- 8) Opciones por Producto
--    - Permite definir variaciones/opciones para cada producto (p. ej., Grande/Mediano/Peque�o).
--    - La validaci�n de duplicados por (ProductId, OptionName) se controlar� desde la aplicaci�n.
-- ===========================================================================================================
CREATE TABLE PM.Options (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    OptionCode VARCHAR(20)  NOT NULL,
    OptionName VARCHAR(80)  NOT NULL,
    StatusId   INT NOT NULL CONSTRAINT FK_Options_Status REFERENCES PM.StatusCatalog(Id),
    ProductsId  INT NOT NULL CONSTRAINT FK_Options_Products REFERENCES PM.Products(Id)
);
GO

-- ===========================================================================================================
-- 9) Usuarios
--     - Autenticaci�n b�sica y metadatos del usuario.
--     - 'PasswordHash' almacenar� la contrase�a hasheada desde la app (no en texto plano).
--     - La validaci�n de formatos (correo .com, tel�fono dddd-dddd) se har� en la app.
-- ===========================================================================================================
CREATE TABLE PM.Users (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Username        VARCHAR(80)  NOT NULL, -- Nombre de usuario (�nico: validar en app)
    PasswordHash    VARCHAR(80)  NOT NULL, -- Hash de la contrase�a
    Names           VARCHAR(80)  NOT NULL, -- Nombres
    LastNames       VARCHAR(80)  NOT NULL, -- Apellidos
    TelephoneNumber VARCHAR(20)  NOT NULL, -- Tel�fono (validaci�n en app)
    Email           VARCHAR(80)  NOT NULL, -- Email    (validaci�n en app)
    CreationDate    DATETIME2    NOT NULL, -- Fecha de creaci�n del registro
);
GO


