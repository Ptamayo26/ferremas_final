-- Script para agregar columna de descuento por porcentaje a la tabla categorias
-- Ejecutar en la base de datos ferremas

-- 1. Agregar la columna descuento_porcentaje a la tabla categorias
ALTER TABLE categorias 
ADD COLUMN descuento_porcentaje DECIMAL(5,2) DEFAULT 0.00 COMMENT 'Porcentaje de descuento aplicable a todos los productos de esta categoría';

-- 2. Actualizar algunas categorías con descuentos de ejemplo
UPDATE categorias 
SET descuento_porcentaje = 15.00 
WHERE nombre = 'Herramientas Eléctricas';

UPDATE categorias 
SET descuento_porcentaje = 10.00 
WHERE nombre = 'Herramientas Manuales';

UPDATE categorias 
SET descuento_porcentaje = 20.00 
WHERE nombre = 'Jardinería';

UPDATE categorias 
SET descuento_porcentaje = 5.00 
WHERE nombre = 'Construcción';

UPDATE categorias 
SET descuento_porcentaje = 12.00 
WHERE nombre = 'Electricidad';

UPDATE categorias 
SET descuento_porcentaje = 8.00 
WHERE nombre = 'Plomería';

UPDATE categorias 
SET descuento_porcentaje = 25.00 
WHERE nombre = 'Pintura y Acabados';

UPDATE categorias 
SET descuento_porcentaje = 18.00 
WHERE nombre = 'Seguridad Industrial';

-- 3. Verificar que se aplicaron los cambios
SELECT id, nombre, descuento_porcentaje 
FROM categorias 
ORDER BY descuento_porcentaje DESC;

-- 4. Verificar que los productos muestran los precios con descuento
SELECT 
    p.id,
    p.nombre as producto,
    p.precio as precio_original,
    c.nombre as categoria,
    c.descuento_porcentaje,
    ROUND(p.precio * (1 - (c.descuento_porcentaje / 100)), 0) as precio_con_descuento
FROM productos p
LEFT JOIN categorias c ON p.categoria_id = c.id
WHERE c.descuento_porcentaje > 0
ORDER BY c.descuento_porcentaje DESC; 