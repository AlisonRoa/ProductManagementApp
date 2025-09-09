-- Estados
INSERT INTO PM.StatusCatalog (StatusCode, StatusName) VALUES ('STA-001','Active'), ('STA-002','Inactive');

-- Tipo proveedor (mínimo)
INSERT INTO PM.SupplierType (SupplierTypeCode, SupplierTypeName) VALUES ('STY-001','Mayorista');

-- Proveedor
INSERT INTO PM.Supplier (SupplierCode, SupplierName, TelephoneNumber, Email, StatusId, SupplierTypeId)
VALUES ('SUP-001','Proveedor Demo','0000-0000','prov@demo.com', 1, 1);

-- Productos (10 aprox.)
INSERT INTO PM.Products (ProductCode, ProductName, PricePerUnit, BasicUnit, StatusId, SupplierId)
VALUES ('SKU-001','Vaso',1.50,'Unidad',1,1), ('SKU-002','Vaso Premium',2.30,'Unidad',1,1);

-- Stock
INSERT INTO PM.Stock (InStock, LastUpdate, ProductId) VALUES (50, SYSUTCDATETIME(), 1), (30, SYSUTCDATETIME(), 2);

-- Opciones
INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductId)
VALUES ('OPT-001','Pequeño',1,1), ('OPT-002','Mediano',1,1), ('OPT-003','Grande',1,1);