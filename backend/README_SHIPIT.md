# 🚚 Servicio Shipit - Ferremas Backend

## 📋 Descripción
Integración completa con la API de Shipit para gestión de envíos en Ferremas.

## ✅ Funcionalidades Implementadas

### 1. **Crear Envío** 
- **Endpoint**: `POST /api/logistica/crear-envio`
- **Descripción**: Crea un nuevo envío en Shipit
- **Body**:
```json
{
  "Courier": "Chilexpress",
  "NombreDestinatario": "Juan Pérez",
  "Direccion": "Av. Siempre Viva 123",
  "ComunaDestino": "Santiago",
  "Correo": "juan@example.com",
  "Telefono": "+56912345678",
  "ItemsCount": 1,
  "Largo": 30,
  "Ancho": 20,
  "Alto": 10,
  "Peso": 500,
  "ValorDeclarado": 10000,
  "Referencia": "PEDIDO-001",
  "Contenido": "Herramientas de construcción"
}
```

### 2. **Cotizar Envío**
- **Endpoint**: `POST /api/logistica/cotizar-envio`
- **Descripción**: Obtiene cotizaciones de envío
- **Body**:
```json
{
  "Origen": {
    "Calle": "Av. Providencia",
    "Numero": "1234",
    "Complemento": "Oficina 5",
    "ComunaId": 2
  },
  "Destino": {
    "Calle": "Av. Las Condes",
    "Numero": "5678",
    "Complemento": "Depto 10",
    "ComunaId": 3
  },
  "Peso": 2.5,
  "Alto": 15,
  "Ancho": 20,
  "Largo": 30,
  "ValorSeguro": 50000,
  "Contenido": "Herramientas"
}
```

### 3. **Obtener Couriers**
- **Endpoint**: `GET /api/logistica/couriers`
- **Descripción**: Lista todos los couriers disponibles

### 4. **Verificar Estado**
- **Endpoint**: `GET /api/logistica/status`
- **Descripción**: Verifica si el servicio Shipit está disponible

## 🔧 Configuración

### appsettings.json
```json
"Shipit": {
  "Email": "tu_email@dominio.cl",
  "Token": "TU_TOKEN_DE_API_SHIPIT",
  "DefaultSettings": {
    "IsPayable": false,
    "Packing": "Sin empaque",
    "ShippingType": "Normal",
    "Destiny": "domicilio",
    "CourierForClient": "chilexpress"
  }
}
```

## 📁 Archivos Implementados

1. **`Services/ShipitService.cs`** - Servicio principal con métodos:
   - `CrearEnvioAsync()` - Crear envío
   - `CotizarEnvioAsync()` - Cotizar envío
   - `ObtenerCouriersAsync()` - Listar couriers
   - `IsServiceAvailableAsync()` - Verificar estado

2. **`DTOs/ShipitEnvioRequestDto.cs`** - DTO para solicitudes de envío

3. **`Controllers/LogisticaController.cs`** - Controlador con endpoints REST

## 🗺️ Comunas Soportadas

El servicio incluye mapeo automático de comunas:
- Santiago (ID: 1)
- Providencia (ID: 2)
- Las Condes (ID: 3)
- Ñuñoa (ID: 4)
- Maipú (ID: 5)
- Puente Alto (ID: 6)
- La Florida (ID: 7)
- San Bernardo (ID: 8)
- Temuco (ID: 9)
- Concepción (ID: 10)
- Valparaíso (ID: 11)
- Viña del Mar (ID: 12)
- Antofagasta (ID: 13)
- Puerto Montt (ID: 14)
- La Serena (ID: 15)
- Rancagua (ID: 16)
- Talca (ID: 17)
- Chillán (ID: 18)
- Iquique (ID: 19)
- Arica (ID: 20)
- Los Ángeles (ID: 21)
- Osorno (ID: 22)
- Punta Arenas (ID: 23)
- Coyhaique (ID: 24)
- Calama (ID: 25)

## 🧪 Pruebas

### Ejemplo con Postman:

**Crear Envío:**
```
POST http://localhost:5200/api/logistica/crear-envio
Content-Type: application/json

{
  "Courier": "Chilexpress",
  "NombreDestinatario": "María González",
  "Direccion": "Av. Apoquindo 1234",
  "ComunaDestino": "Las Condes",
  "Correo": "maria@example.com",
  "Telefono": "+56987654321",
  "ItemsCount": 2,
  "Largo": 25,
  "Ancho": 15,
  "Alto": 8,
  "Peso": 800,
  "ValorDeclarado": 25000,
  "Referencia": "FERREMAS-20241201-001",
  "Contenido": "Taladro y accesorios"
}
```

## 📊 Respuestas

### Respuesta Exitosa (Crear Envío):
```json
{
  "success": true,
  "message": "Envío creado exitosamente",
  "data": {
    "success": true,
    "trackingNumber": "SH123456789CL",
    "labelUrl": "https://api.shipit.cl/labels/123456",
    "cost": 8500,
    "courier": "chilexpress",
    "service": "normal",
    "deliveryTime": 2,
    "deliveryTimeText": "2 días hábiles"
  }
}
```

### Respuesta de Error:
```json
{
  "success": false,
  "message": "Error al crear envío",
  "error": "Comuna no válida: ComunaInexistente"
}
```

## 🔍 Logging

El servicio incluye logging detallado:
- Información de solicitudes recibidas
- Respuestas de la API de Shipit
- Errores y advertencias
- Números de tracking generados

## 🚀 Uso en Frontend

```typescript
// Ejemplo de llamada desde el frontend
const crearEnvio = async (envioData: ShipitEnvioRequestDto) => {
  try {
    const response = await fetch('/api/logistica/crear-envio', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(envioData)
    });
    
    const result = await response.json();
    
    if (result.success) {
      console.log('Envío creado:', result.data);
      return result.data;
    } else {
      throw new Error(result.error);
    }
  } catch (error) {
    console.error('Error al crear envío:', error);
    throw error;
  }
};
```

## ⚠️ Notas Importantes

1. **Token de API**: Asegúrate de tener un token válido de Shipit
2. **Comunas**: Solo las comunas listadas están soportadas
3. **Peso**: Debe estar en gramos
4. **Dimensiones**: Deben estar en centímetros
5. **Valor Declarado**: Debe estar en pesos chilenos

## 🔗 Documentación API Shipit

Para más detalles sobre la API de Shipit, consulta:
- [Documentación Oficial Shipit](https://shipit.cl/api)
- [Guía de Integración](https://shipit.cl/docs) 