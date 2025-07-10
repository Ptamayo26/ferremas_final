# Ferremas - Sistema de Gestión de Ferretería

## Descripción

Ferremas es un sistema completo de gestión para ferreterías que incluye un backend desarrollado en .NET 8 y un frontend en React con TypeScript. El sistema permite la gestión de productos, clientes, pedidos, pagos y envíos.

## Características

- **Gestión de Productos**: Catálogo completo con imágenes, precios y stock
- **Gestión de Clientes**: Registro y seguimiento de clientes
- **Sistema de Pedidos**: Creación y seguimiento de pedidos
- **Módulo de Pagos**: Integración con múltiples métodos de pago
- **Sistema de Envíos**: Cálculo de costos de envío
- **Panel de Administración**: Gestión de usuarios y reportes
- **Comparador de Precios**: Comparación con MercadoLibre
- **Sistema de Notificaciones**: Notificaciones en tiempo real

## Tecnologías Utilizadas

### Backend
- .NET 8
- Entity Framework Core
- MySQL
- JWT Authentication
- BCrypt para encriptación
- Swagger/OpenAPI

### Frontend
- React 18
- TypeScript
- Vite
- Tailwind CSS
- React Router
- Axios

## Estructura del Proyecto

```
Ferremas/
├── backend/                 # Backend .NET 8
│   ├── Controllers/        # Controladores de la API
│   ├── Models/            # Modelos de datos
│   ├── Services/          # Servicios de negocio
│   ├── Data/              # Contexto de base de datos
│   ├── DTOs/              # Objetos de transferencia de datos
│   └── Migrations/        # Migraciones de Entity Framework
├── FerremsaFrontend/      # Frontend React
│   └── ferremas-frontend/
│       ├── src/
│       │   ├── components/ # Componentes React
│       │   ├── pages/      # Páginas de la aplicación
│       │   ├── services/   # Servicios de API
│       │   └── types/      # Tipos TypeScript
│       └── public/         # Archivos públicos
└── bd_ferremas.sql        # Script de base de datos
```

## Instalación

### Prerrequisitos

- .NET 8 SDK
- Node.js 18+
- MySQL 8.0+
- Git

### Backend

1. Clona el repositorio:
```bash
git clone https://github.com/Ptamayo26/ferremas_final.git
cd ferremas_final
```

2. Configura la base de datos:
```bash
# Importa el script de base de datos
mysql -u root -p < bd_ferremas.sql
```

3. Configura la conexión a la base de datos en `backend/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ferremas;User=tu_usuario;Password=tu_password;"
  }
}
```

4. Ejecuta las migraciones:
```bash
cd backend
dotnet ef database update
```

5. Ejecuta el backend:
```bash
dotnet run
```

El backend estará disponible en `https://localhost:7001`

### Frontend

1. Instala las dependencias:
```bash
cd FerremsaFrontend/ferremas-frontend
npm install
```

2. Configura las variables de entorno:
```bash
# Crea un archivo .env.local
VITE_API_URL=https://localhost:7001/api
```

3. Ejecuta el frontend:
```bash
npm run dev
```

El frontend estará disponible en `http://localhost:5173`

## Configuración

### Variables de Entorno

#### Backend (.NET)
- `ConnectionStrings:DefaultConnection`: Cadena de conexión a MySQL
- `JwtSettings:SecretKey`: Clave secreta para JWT
- `JwtSettings:Issuer`: Emisor del token JWT
- `JwtSettings:Audience`: Audiencia del token JWT

#### Frontend (React)
- `VITE_API_URL`: URL del backend API

## Uso

### Roles de Usuario

1. **Administrador**: Acceso completo al sistema
2. **Vendedor**: Gestión de clientes y pedidos
3. **Bodeguero**: Gestión de inventario
4. **Cliente**: Realizar compras y ver pedidos

### Funcionalidades Principales

- **Gestión de Productos**: Agregar, editar y eliminar productos
- **Gestión de Clientes**: Registrar y gestionar clientes
- **Carrito de Compras**: Agregar productos y realizar pedidos
- **Sistema de Pagos**: Procesar pagos con múltiples métodos
- **Envíos**: Calcular costos y gestionar envíos
- **Reportes**: Generar reportes de ventas e inventario

## API Endpoints

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario
- `POST /api/auth/refresh` - Renovar token

### Productos
- `GET /api/productos` - Obtener productos
- `POST /api/productos` - Crear producto
- `PUT /api/productos/{id}` - Actualizar producto
- `DELETE /api/productos/{id}` - Eliminar producto

### Clientes
- `GET /api/clientes` - Obtener clientes
- `POST /api/clientes` - Crear cliente
- `PUT /api/clientes/{id}` - Actualizar cliente

### Pedidos
- `GET /api/pedidos` - Obtener pedidos
- `POST /api/pedidos` - Crear pedido
- `PUT /api/pedidos/{id}/estado` - Actualizar estado

## Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## Contacto

- Desarrollador: Paloma Tamayo / Cristian Solis
- Proyecto: [https://github.com/Ptamayo26/ferremas_final](https://github.com/Ptamayo26/ferremas_final) 
