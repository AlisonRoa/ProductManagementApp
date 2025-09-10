/* ===========================================================================================================
   Sistema: Product Management (Prueba Técnica)
   Autor:   Alison Judith Macis Roa
   
   Notas de diseño:
     - Se separa el catálogo de estados (StatusCatalog) para reutilizarlo en Productos, Opciones y Proveedores.
     - Se modela Proveedor (Supplier) y Tipo de Proveedor (SupplierType) para enriquecer el dato “nombre del proveedor”.
     - La existencia (stock) se maneja en una tabla separada (Stock) para permitir actualizaciones de inventario sin
       afectar los metadatos del producto (precio, proveedor, etc.).
     - Las validaciones de formato (correo .com y teléfono dddd-dddd) se implementarán en la CAPA DE APLICACIÓN,
       tal como se acordó (no en la BD).

   Requisitos cubiertos (resumen):
     1a. Productos: código, nombre, existencia (via PM.Stock), estado (FK a PM.StatusCatalog), nombre del proveedor (via FK a PM.Supplier).
     1b. Usuarios: nombre de usuario, contraseña (hash), nombre, apellido(s), correo, teléfono, fecha de creación.
     1c. Opciones: nombre de la opción, producto relacionado, estado.
     2-6. Se soporta: Login, Registro, Listado de productos con filtros por estado, y CRUD de opciones por producto (en la app).
     7. GitHub con commits por punto (esto se gestiona fuera de BD).

=========================================================================================================== */

-- ==========================================
-- 1) Creación de Base de Datos
-- ==========================================
CREATE DATABASE ProductManagement;
GO

USE ProductManagement;
GO

-- ==========================================
-- 2) Creación de esquema lógico
-- ==========================================
CREATE SCHEMA PM;
GO


-- ===========================================================================================================
-- 3) Catálogo de Estados (Activo / Inactivo)
-- ===========================================================================================================
CREATE TABLE PM.StatusCatalog (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode  VARCHAR(20)  NOT NULL,
    StatusName  VARCHAR(80)  NOT NULL
);
GO


-- ===========================================================================================================
-- 4) Tipo de Proveedor (opcional, para clasificación)
--    - Útil para segmentar proveedores (Mayorista, Minorista, Internacional, etc.)
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
--    - Se separa de Productos para simplificar operaciones de actualización de inventario.
--    - 'LastUpdate' permite registrar el momento de la última modificación.
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
--    - Permite definir variaciones/opciones para cada producto (p. ej., Grande/Mediano/Pequeño).
--    - La validación de duplicados por (ProductId, OptionName) se controlará desde la aplicación.
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
--     - Autenticación básica y metadatos del usuario.
--     - 'PasswordHash' almacenará la contraseña hasheada desde la app (no en texto plano).
--     - La validación de formatos (correo .com, teléfono dddd-dddd) se hará en la app.
-- ===========================================================================================================
CREATE TABLE PM.Users (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Username        VARCHAR(80)  NOT NULL, -- Nombre de usuario (único: validar en app)
    PasswordHash    VARCHAR(80)  NOT NULL, -- Hash de la contraseña
    Names           VARCHAR(80)  NOT NULL, -- Nombres
    LastNames       VARCHAR(80)  NOT NULL, -- Apellidos
    TelephoneNumber VARCHAR(20)  NOT NULL, -- Teléfono (validación en app)
    Email           VARCHAR(80)  NOT NULL, -- Email    (validación en app)
    CreationDate    DATETIME2    NOT NULL, -- Fecha de creación del registro
);
GO


