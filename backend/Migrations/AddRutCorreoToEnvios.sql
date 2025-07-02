-- Migración: Agregar columnas rut y correo a la tabla envios
ALTER TABLE envios ADD COLUMN rut VARCHAR(20) DEFAULT '';
ALTER TABLE envios ADD COLUMN correo VARCHAR(100) DEFAULT ''; 