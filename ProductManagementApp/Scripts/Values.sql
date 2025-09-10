/* ===========================================================
   DEMO SEED DATA - ProductManagement
   Idempotente: puedes ejecutarlo varias veces sin duplicar.
   Crea (si faltan): Status, SupplierType, Supplier, Products,
   Stock, Options y un usuario de prueba.
   ----------------------------------------------------------- */

-- 0) Asegúrate de estar en la BD correcta
IF DB_ID('ProductManagement') IS NULL
BEGIN
    PRINT 'Creando BD ProductManagement...';
    CREATE DATABASE ProductManagement;
END
GO

USE ProductManagement;
GO

-- 0.1) Asegura el esquema PM (por si no existe)
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'PM')
BEGIN
    EXEC('CREATE SCHEMA PM AUTHORIZATION dbo;');
END
GO

/* 1) StatusCatalog ------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM PM.StatusCatalog WHERE StatusCode = 'STA-001')
    INSERT INTO PM.StatusCatalog (StatusCode, StatusName) VALUES ('STA-001','Active');
IF NOT EXISTS (SELECT 1 FROM PM.StatusCatalog WHERE StatusCode = 'STA-002')
    INSERT INTO PM.StatusCatalog (StatusCode, StatusName) VALUES ('STA-002','Inactive');
IF NOT EXISTS (SELECT 1 FROM PM.StatusCatalog WHERE StatusCode = 'STA-003')
    INSERT INTO PM.StatusCatalog (StatusCode, StatusName) VALUES ('STA-003','Draft');

DECLARE @IdStatusActive   INT = (SELECT TOP 1 Id FROM PM.StatusCatalog WHERE StatusCode='STA-001');
DECLARE @IdStatusInactive INT = (SELECT TOP 1 Id FROM PM.StatusCatalog WHERE StatusCode='STA-002');
DECLARE @IdStatusDraft    INT = (SELECT TOP 1 Id FROM PM.StatusCatalog WHERE StatusCode='STA-003');

/* 2) SupplierType ------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM PM.SupplierTypes WHERE SupplierTypeCode='STY-001')
    INSERT INTO PM.SupplierTypes (SupplierTypeCode, SupplierTypeName) VALUES ('STY-001','Mayorista');
IF NOT EXISTS (SELECT 1 FROM PM.SupplierTypes WHERE SupplierTypeCode='STY-002')
    INSERT INTO PM.SupplierTypes (SupplierTypeCode, SupplierTypeName) VALUES ('STY-002','Minorista');
IF NOT EXISTS (SELECT 1 FROM PM.SupplierTypes WHERE SupplierTypeCode='STY-003')
    INSERT INTO PM.SupplierTypes (SupplierTypeCode, SupplierTypeName) VALUES ('STY-003','Internacional');

DECLARE @IdTypeMayorista  INT = (SELECT TOP 1 Id FROM PM.SupplierTypes WHERE SupplierTypeCode='STY-001');
DECLARE @IdTypeMinorista  INT = (SELECT TOP 1 Id FROM PM.SupplierTypes WHERE SupplierTypeCode='STY-002');
DECLARE @IdTypeIntl       INT = (SELECT TOP 1 Id FROM PM.SupplierTypes WHERE SupplierTypeCode='STY-003');

/* 3) Suppliers ---------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM PM.Suppliers WHERE SupplierCode='SUP-001')
    INSERT INTO PM.Suppliers (SupplierCode, SupplierName, TelephoneNumber, Email, StatusId, SupplierTypesId)
    VALUES ('SUP-001','Proveedor Demo','0000-0000','prov@demo.com', @IdStatusActive, @IdTypeMayorista);

IF NOT EXISTS (SELECT 1 FROM PM.Suppliers WHERE SupplierCode='SUP-002')
    INSERT INTO PM.Suppliers (SupplierCode, SupplierName, TelephoneNumber, Email, StatusId, SupplierTypesId)
    VALUES ('SUP-002','Ingram','2222-2222','ingram@demo.com', @IdStatusActive, @IdTypeMayorista);

IF NOT EXISTS (SELECT 1 FROM PM.Suppliers WHERE SupplierCode='SUP-003')
    INSERT INTO PM.Suppliers (SupplierCode, SupplierName, TelephoneNumber, Email, StatusId, SupplierTypesId)
    VALUES ('SUP-003','TechData','3333-3333','techdata@demo.com', @IdStatusActive, @IdTypeMayorista);

IF NOT EXISTS (SELECT 1 FROM PM.Suppliers WHERE SupplierCode='SUP-004')
    INSERT INTO PM.Suppliers (SupplierCode, SupplierName, TelephoneNumber, Email, StatusId, SupplierTypesId)
    VALUES ('SUP-004','Acme Intl.','4444-4444','acme@demo.com', @IdStatusActive, @IdTypeIntl);

IF NOT EXISTS (SELECT 1 FROM PM.Suppliers WHERE SupplierCode='SUP-005')
    INSERT INTO PM.Suppliers (SupplierCode, SupplierName, TelephoneNumber, Email, StatusId, SupplierTypesId)
    VALUES ('SUP-005','Logística CR','5555-5555','logistica@demo.com', @IdStatusInactive, @IdTypeMinorista);

DECLARE @SUP1 INT = (SELECT TOP 1 Id FROM PM.Suppliers WHERE SupplierCode='SUP-001');
DECLARE @SUP2 INT = (SELECT TOP 1 Id FROM PM.Suppliers WHERE SupplierCode='SUP-002');
DECLARE @SUP3 INT = (SELECT TOP 1 Id FROM PM.Suppliers WHERE SupplierCode='SUP-003');
DECLARE @SUP4 INT = (SELECT TOP 1 Id FROM PM.Suppliers WHERE SupplierCode='SUP-004');
DECLARE @SUP5 INT = (SELECT TOP 1 Id FROM PM.Suppliers WHERE SupplierCode='SUP-005');

/* 4) Products ----------------------------------------------------------- */
;WITH P(ProductCode, ProductName, PricePerUnit, BasicUnit, StatusId, SuppliersId) AS
(
    SELECT 'SKU-001','Vaso',                1.50,'Unidad', @IdStatusActive,   @SUP1 UNION ALL
    SELECT 'SKU-002','Vaso Premium',        2.30,'Unidad', @IdStatusActive,   @SUP1 UNION ALL
    SELECT 'SKU-003','Plato plástico',      0.80,'Unidad', @IdStatusActive,   @SUP2 UNION ALL
    SELECT 'SKU-004','Plato de cartón',     0.65,'Unidad', @IdStatusActive,   @SUP2 UNION ALL
    SELECT 'SKU-005','Cubiertos set x10',   2.90,'Paquete',@IdStatusActive,   @SUP3 UNION ALL
    SELECT 'SKU-006','Servilletas 100u',    1.20,'Paquete',@IdStatusActive,   @SUP3 UNION ALL
    SELECT 'SKU-007','Vaso térmico',        3.40,'Unidad', @IdStatusInactive, @SUP4 UNION ALL
    SELECT 'SKU-008','Tapa para vaso',      0.30,'Unidad', @IdStatusActive,   @SUP4 UNION ALL
    SELECT 'SKU-009','Caja para alimentos', 4.10,'Unidad', @IdStatusActive,   @SUP5 UNION ALL
    SELECT 'SKU-010','Pajillas pack 50',    1.75,'Paquete',@IdStatusActive,   @SUP5 UNION ALL
    SELECT 'SKU-011','Tenedor reforzado',   0.40,'Unidad', @IdStatusActive,   @SUP2 UNION ALL
    SELECT 'SKU-012','Cuchillo reforzado',  0.40,'Unidad', @IdStatusActive,   @SUP2 UNION ALL
    SELECT 'SKU-013','Cuchara reforzada',   0.40,'Unidad', @IdStatusActive,   @SUP2 UNION ALL
    SELECT 'SKU-014','Bandeja multiuso',    2.10,'Unidad', @IdStatusActive,   @SUP3 UNION ALL
    SELECT 'SKU-015','Compostable bowl',    3.90,'Unidad', @IdStatusDraft,    @SUP4 UNION ALL
    SELECT 'SKU-016','Taza cerámica',       5.50,'Unidad', @IdStatusActive,   @SUP1 UNION ALL
    SELECT 'SKU-017','Caja pizza 12"',      0.95,'Unidad', @IdStatusActive,   @SUP5 UNION ALL
    SELECT 'SKU-018','Caja pizza 14"',      1.10,'Unidad', @IdStatusActive,   @SUP5 UNION ALL
    SELECT 'SKU-019','Bandeja aluminio',    2.60,'Unidad', @IdStatusInactive, @SUP3 UNION ALL
    SELECT 'SKU-020','Film plástico 30cm',  4.80,'Unidad', @IdStatusActive,   @SUP2
)
INSERT INTO PM.Products (ProductCode, ProductName, PricePerUnit, BasicUnit, StatusId, SuppliersId)
SELECT p.ProductCode, p.ProductName, p.PricePerUnit, p.BasicUnit, p.StatusId, p.SuppliersId
FROM P p
WHERE NOT EXISTS (SELECT 1 FROM PM.Products x WHERE x.ProductCode = p.ProductCode);

-- IDs por código para referencias posteriores
DECLARE @P_VASO     INT = (SELECT TOP 1 Id FROM PM.Products WHERE ProductCode='SKU-001');
DECLARE @P_VASOPREM INT = (SELECT TOP 1 Id FROM PM.Products WHERE ProductCode='SKU-002');
DECLARE @P_TAPA     INT = (SELECT TOP 1 Id FROM PM.Products WHERE ProductCode='SKU-008');

/* 5) Stock (una foto de inventario por producto) ----------------------- */
;WITH S(ProductCode, InStock) AS
(
    SELECT 'SKU-001', 50 UNION ALL
    SELECT 'SKU-002', 30 UNION ALL
    SELECT 'SKU-003', 120 UNION ALL
    SELECT 'SKU-004', 85 UNION ALL
    SELECT 'SKU-005', 40 UNION ALL
    SELECT 'SKU-006', 60 UNION ALL
    SELECT 'SKU-007', 0 UNION ALL
    SELECT 'SKU-008', 200 UNION ALL
    SELECT 'SKU-009', 15 UNION ALL
    SELECT 'SKU-010', 90 UNION ALL
    SELECT 'SKU-011', 300 UNION ALL
    SELECT 'SKU-012', 280 UNION ALL
    SELECT 'SKU-013', 310 UNION ALL
    SELECT 'SKU-014', 22 UNION ALL
    SELECT 'SKU-015', 10 UNION ALL
    SELECT 'SKU-016', 5 UNION ALL
    SELECT 'SKU-017', 40 UNION ALL
    SELECT 'SKU-018', 35 UNION ALL
    SELECT 'SKU-019', 0 UNION ALL
    SELECT 'SKU-020', 18
)
INSERT INTO PM.Stocks (InStock, LastUpdate, ProductsId)
SELECT s.InStock, SYSUTCDATETIME(), p.Id
FROM S s
JOIN PM.Products p ON p.ProductCode = s.ProductCode
WHERE NOT EXISTS (
    SELECT 1 FROM PM.Stocks st WHERE st.ProductsId = p.Id
);

-- Si quieres simular un histórico, agrega otra fila de stock por algunos productos
;WITH S2(ProductCode, InStock, LastUpdate) AS
(
    SELECT 'SKU-001', 10, DATEADD(day,-10,SYSUTCDATETIME()) UNION ALL
    SELECT 'SKU-002',  5, DATEADD(day, -5,SYSUTCDATETIME())
)
INSERT INTO PM.Stocks (InStock, LastUpdate, ProductsId)
SELECT s.InStock, s.LastUpdate, p.Id
FROM S2 s
JOIN PM.Products p ON p.ProductCode = s.ProductCode
WHERE NOT EXISTS (
    SELECT 1 FROM PM.Stocks st WHERE st.ProductsId = p.Id AND st.LastUpdate = s.LastUpdate
);

/* 6) Options por producto ---------------------------------------------- */
-- Opciones para SKU-001 (Vaso)
IF @P_VASO IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-001')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-001','Pequeño', @IdStatusActive, @P_VASO);
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-002')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-002','Mediano', @IdStatusActive, @P_VASO);
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-003')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-003','Grande',  @IdStatusActive, @P_VASO);
END

-- Opciones para SKU-002 (Vaso Premium)
IF @P_VASOPREM IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-101')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-101','Pequeño', @IdStatusActive, @P_VASOPREM);
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-102')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-102','Mediano', @IdStatusActive, @P_VASOPREM);
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-103')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-103','Grande',  @IdStatusActive, @P_VASOPREM);
END

-- Opciones para SKU-008 (Tapa para vaso)
IF @P_TAPA IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-201')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-201','Chica',  @IdStatusActive, @P_TAPA);
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-202')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-202','Mediana',@IdStatusActive, @P_TAPA);
    IF NOT EXISTS (SELECT 1 FROM PM.Options WHERE OptionCode='OPT-203')
        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
        VALUES ('OPT-203','Grande', @IdStatusActive, @P_TAPA);
END

/* 7) Usuario de prueba -------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM PM.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO PM.Users
    (
        Username, PasswordHash, Names, LastNames,
        TelephoneNumber, Email, CreationDate
    )
    VALUES
    (
        'admin',
        'admin123',           -- coloca tu hash real desde la app
        'Admin',
        'Demo',
        '0000-0000',                 -- la app valida dddd-dddd
        'admin@demo.com',            -- la app valida .com
        SYSUTCDATETIME()
    );
END

PRINT 'Seed de demo completado.';
