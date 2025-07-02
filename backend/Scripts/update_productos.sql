USE ferremas_integrada;

-- Agregar columna stock a la tabla productos
ALTER TABLE productos
ADD COLUMN stock INT NOT NULL DEFAULT 0 AFTER precio;
 
-- Actualizar el stock de los productos existentes
UPDATE productos p
JOIN inventario i ON p.id = i.producto_id
SET p.stock = i.stock; 