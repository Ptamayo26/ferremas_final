-- Script de prueba para verificar refresh tokens
-- Ejecutar después de crear la tabla refresh_tokens

USE ferremas_integrada;

-- Verificar que la tabla existe
SHOW TABLES LIKE 'refresh_tokens';

-- Verificar estructura de la tabla
DESCRIBE refresh_tokens;

-- Verificar que hay usuarios en la base de datos
SELECT id, nombre, email, rol FROM usuarios LIMIT 5;

-- Verificar que no hay refresh tokens (debería estar vacía inicialmente)
SELECT COUNT(*) as total_refresh_tokens FROM refresh_tokens;

-- Verificar índices
SHOW INDEX FROM refresh_tokens;

-- Verificar restricciones de clave foránea
SELECT 
    CONSTRAINT_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
WHERE TABLE_NAME = 'refresh_tokens' 
AND REFERENCED_TABLE_NAME IS NOT NULL; 