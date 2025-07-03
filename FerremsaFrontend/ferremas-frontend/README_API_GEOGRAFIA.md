# API de Geografía de Chile - Ferremas

## Descripción

Esta API permite obtener información geográfica oficial de Chile (regiones y comunas) utilizando datos del gobierno chileno. La implementación incluye un sistema de fallback con datos estáticos en caso de que la API oficial no esté disponible.

## Características

- ✅ **Datos oficiales**: Utiliza la API Digital del Gobierno de Chile
- ✅ **Sistema de fallback**: Datos estáticos como respaldo
- ✅ **Caché inteligente**: Optimización de rendimiento
- ✅ **Validación de datos**: Asegura integridad de la información
- ✅ **Logging completo**: Trazabilidad de operaciones

## Endpoints Disponibles

### 1. Obtener Todas las Regiones
```
GET /api/ChileGeo/regiones
```

**Respuesta:**
```json
{
  "exito": true,
  "mensaje": "Se obtuvieron 16 regiones de Chile",
  "data": [
    {
      "codigo": "13",
      "nombre": "Metropolitana",
      "nombreOficial": "Región Metropolitana de Santiago"
    },
    {
      "codigo": "05",
      "nombre": "Valparaíso",
      "nombreOficial": "Región de Valparaíso"
    }
  ]
}
```

### 2. Obtener Comunas por Región
```
GET /api/ChileGeo/regiones/{codigoRegion}/comunas
```

**Ejemplo:**
```
GET /api/ChileGeo/regiones/13/comunas
```

**Respuesta:**
```json
{
  "exito": true,
  "mensaje": "Se obtuvieron 52 comunas para la región 13",
  "data": [
    {
      "codigo": "13101",
      "nombre": "Santiago",
      "codigoRegion": "13",
      "nombreRegion": "Metropolitana"
    },
    {
      "codigo": "13102",
      "nombre": "Providencia",
      "codigoRegion": "13",
      "nombreRegion": "Metropolitana"
    }
  ]
}
```

### 3. Obtener Todas las Comunas
```
GET /api/ChileGeo/comunas
```

### 4. Buscar Comunas
```
GET /api/ChileGeo/comunas/buscar?query={nombre}
```

**Ejemplo:**
```
GET /api/ChileGeo/comunas/buscar?query=santiago
```

### 5. Información de la API
```
GET /api/ChileGeo/info
```

**Respuesta:**
```json
{
  "nombre": "API de Geografía de Chile",
  "version": "1.0.0",
  "descripcion": "API para obtener regiones y comunas de Chile usando datos oficiales del gobierno",
  "fuente": "API Digital del Gobierno de Chile",
  "endpoints": {
    "regiones": "/api/ChileGeo/regiones",
    "comunasPorRegion": "/api/ChileGeo/regiones/{codigo}/comunas",
    "todasLasComunas": "/api/ChileGeo/comunas",
    "buscarComunas": "/api/ChileGeo/comunas/buscar?query={nombre}"
  },
  "documentacion": "https://apis.digital.gob.cl/dpa/",
  "estado": "Operativo",
  "ultimaActualizacion": "2024-01-15T10:30:00Z"
}
```

## Uso en el Frontend

### 1. Importar el servicio
```typescript
import chileGeoService from '../services/chileGeo';
import type { RegionChile, ComunaChile } from '../types/api';
```

### 2. Cargar regiones
```typescript
const [regiones, setRegiones] = useState<RegionChile[]>([]);

useEffect(() => {
    const cargarRegiones = async () => {
        try {
            const data = await chileGeoService.getRegiones();
            setRegiones(data);
        } catch (error) {
            console.error('Error al cargar regiones:', error);
        }
    };
    
    cargarRegiones();
}, []);
```

### 3. Cargar comunas por región
```typescript
const [comunas, setComunas] = useState<ComunaChile[]>([]);

const handleRegionChange = async (codigoRegion: string) => {
    if (!codigoRegion) {
        setComunas([]);
        return;
    }

    try {
        const data = await chileGeoService.getComunasByRegion(codigoRegion);
        setComunas(data);
    } catch (error) {
        console.error('Error al cargar comunas:', error);
        setComunas([]);
    }
};
```

### 4. Componente de ejemplo
```typescript
const RegionComunaSelector: React.FC = () => {
    const [regiones, setRegiones] = useState<RegionChile[]>([]);
    const [comunas, setComunas] = useState<ComunaChile[]>([]);
    const [regionSeleccionada, setRegionSeleccionada] = useState<string>('');
    const [comunaSeleccionada, setComunaSeleccionada] = useState<string>('');

    // Cargar regiones al montar
    useEffect(() => {
        chileGeoService.getRegiones().then(setRegiones);
    }, []);

    // Cargar comunas cuando cambie la región
    const handleRegionChange = (regionNombre: string) => {
        setRegionSeleccionada(regionNombre);
        setComunaSeleccionada('');
        
        const region = regiones.find(r => r.nombre === regionNombre);
        if (region) {
            chileGeoService.getComunasByRegion(region.codigo).then(setComunas);
        }
    };

    return (
        <div className="grid grid-cols-2 gap-4">
            <div>
                <label>Región</label>
                <select 
                    value={regionSeleccionada}
                    onChange={(e) => handleRegionChange(e.target.value)}
                >
                    <option value="">Seleccione región</option>
                    {regiones.map(region => (
                        <option key={region.codigo} value={region.nombre}>
                            {region.nombre}
                        </option>
                    ))}
                </select>
            </div>
            
            <div>
                <label>Comuna</label>
                <select 
                    value={comunaSeleccionada}
                    onChange={(e) => setComunaSeleccionada(e.target.value)}
                    disabled={!regionSeleccionada}
                >
                    <option value="">Seleccione comuna</option>
                    {comunas.map(comuna => (
                        <option key={comuna.codigo} value={comuna.nombre}>
                            {comuna.nombre}
                        </option>
                    ))}
                </select>
            </div>
        </div>
    );
};
```

## Implementación en el Registro de Clientes

El formulario de registro de clientes ya incluye selects dinámicos para región y comuna:

1. **Carga automática**: Las regiones se cargan al montar el componente
2. **Cascada**: Al seleccionar una región, se cargan automáticamente sus comunas
3. **Validación**: Los campos son obligatorios y se validan antes del envío
4. **UX mejorada**: Estados de carga y manejo de errores

## Beneficios

### Para el Usuario
- ✅ **Datos precisos**: Información oficial y actualizada
- ✅ **Facilidad de uso**: No necesita escribir nombres de regiones/comunas
- ✅ **Evita errores**: Elimina errores de escritura
- ✅ **Experiencia fluida**: Carga dinámica y responsive

### Para el Sistema
- ✅ **Datos consistentes**: Formato estandarizado en la base de datos
- ✅ **Validación automática**: Reduce errores de entrada
- ✅ **Escalabilidad**: Fácil agregar nuevas regiones/comunas
- ✅ **Mantenimiento**: Actualizaciones automáticas desde fuente oficial

## Configuración

### Backend
El servicio está registrado en `Program.cs`:
```csharp
// Registrar ChileGeoService
builder.Services.AddHttpClient<ChileGeoService>();
builder.Services.AddScoped<ChileGeoService>();
```

### Frontend
Los tipos están definidos en `src/types/api.ts`:
```typescript
export interface RegionChile {
  codigo: string;
  nombre: string;
  nombreOficial: string;
}

export interface ComunaChile {
  codigo: string;
  nombre: string;
  codigoRegion: string;
  nombreRegion: string;
}
```

## Manejo de Errores

### Fallback System
Si la API oficial no está disponible, el sistema utiliza datos estáticos:
- 16 regiones de Chile
- Comunas principales por región
- Datos actualizados hasta 2024

### Logging
Todos los errores se registran para monitoreo:
```csharp
_logger.LogError(ex, "Error al obtener regiones de Chile");
```

## Próximas Mejoras

- [ ] **Caché local**: Almacenar datos en localStorage
- [ ] **Búsqueda avanzada**: Filtros por población, área, etc.
- [ ] **Geocodificación**: Convertir direcciones a coordenadas
- [ ] **Validación de códigos postales**: Verificar códigos por comuna
- [ ] **API pública**: Exponer endpoints para terceros

## Referencias

- [API Digital del Gobierno de Chile](https://apis.digital.gob.cl/dpa/)
- [División Político Administrativa](https://www.ine.cl/estadisticas/sociales/censos-de-poblacion-y-vivienda)
- [Documentación oficial](https://apis.digital.gob.cl/dpa/docs) 