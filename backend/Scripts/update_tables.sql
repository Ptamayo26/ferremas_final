USE ferremas_integrada;

-- Actualizar tabla categorias
ALTER TABLE categorias
ADD COLUMN activo BOOLEAN DEFAULT TRUE,
ADD COLUMN fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN fecha_modificacion DATETIME;

-- Actualizar tabla marcas
ALTER TABLE marcas
ADD COLUMN activo BOOLEAN DEFAULT TRUE,
ADD COLUMN fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
ADD COLUMN fecha_modificacion DATETIME;

-- Actualizar datos existentes
UPDATE categorias SET activo = TRUE WHERE activo IS NULL;
UPDATE marcas SET activo = TRUE WHERE activo IS NULL; 