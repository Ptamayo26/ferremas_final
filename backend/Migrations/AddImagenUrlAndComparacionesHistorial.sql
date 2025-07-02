-- Temporarily disable safe update mode
SET SQL_SAFE_UPDATES = 0;

-- First update any NULL values in imagen_url to 'Sin imagen'
UPDATE productos SET imagen_url = 'Sin imagen' WHERE imagen_url IS NULL;

-- Re-enable safe update mode
SET SQL_SAFE_UPDATES = 1;

-- Rename imagen_url to ImagenUrl if it exists
SET @dbname = 'ferremas_integrada';
SET @tablename = 'productos';
SET @oldcolumn = 'imagen_url';
SET @newcolumn = 'ImagenUrl';

SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE 
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @oldcolumn)
  ) > 0,
  CONCAT("ALTER TABLE ", @tablename, " CHANGE ", @oldcolumn, " ", @newcolumn, " VARCHAR(200) NOT NULL DEFAULT 'Sin imagen'"),
  "SELECT 'Column does not exist'"
));
PREPARE alterIfExists FROM @preparedStatement;
EXECUTE alterIfExists;
DEALLOCATE PREPARE alterIfExists;

-- Create comparaciones_historial table if it doesn't exist
CREATE TABLE IF NOT EXISTS `comparaciones_historial` (
  `id` int NOT NULL AUTO_INCREMENT,
  `usuario_id` int NOT NULL,
  `nombre_producto` varchar(200) NOT NULL,
  `precio_ferremas` decimal(18,2) NOT NULL,
  `resultado_json` text NOT NULL,
  `fecha` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `usuario_id` (`usuario_id`),
  CONSTRAINT `comparaciones_historial_ibfk_1` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci; 