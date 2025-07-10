-- Migración para agregar tabla de refresh tokens
-- Ejecutar en la base de datos ferremas_integrada

CREATE TABLE IF NOT EXISTS refresh_tokens (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Token VARCHAR(500) NOT NULL,
    ExpiryDate DATETIME NOT NULL,
    UsuarioId INT NOT NULL,
    IsRevoked BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UsuarioId) REFERENCES usuarios(Id) ON DELETE CASCADE,
    INDEX idx_token (Token),
    INDEX idx_usuario_id (UsuarioId),
    INDEX idx_expiry_date (ExpiryDate)
);

-- Limpiar tokens expirados (opcional)
DELETE FROM refresh_tokens WHERE ExpiryDate < NOW(); 