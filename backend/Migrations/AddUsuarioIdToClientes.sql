-- Agregar columna usuario_id a la tabla clientes
ALTER TABLE clientes
ADD COLUMN usuario_id INT NULL,
ADD CONSTRAINT fk_clientes_usuarios
FOREIGN KEY (usuario_id) REFERENCES usuarios(id);

-- Crear índice para mejorar el rendimiento de las búsquedas
CREATE INDEX idx_clientes_usuario_id ON clientes(usuario_id); 