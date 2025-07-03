# Registro de Clientes - Ferremas E-commerce

## Descripción

Esta funcionalidad permite a nuevos usuarios registrarse como clientes en la plataforma de Ferremas, una ferretería online. El registro incluye validaciones completas y auto-login después del registro exitoso.

## Características

### ✅ Funcionalidades Implementadas

1. **Formulario de Registro Completo**
   - Nombre y apellido (validación de longitud mínima)
   - Correo electrónico (validación de formato)
   - RUT chileno (validación de dígito verificador)
   - Teléfono móvil (validación de formato chileno)
   - **Dirección completa de envío** (calle, número, departamento, comuna, región, código postal)
   - Contraseña (mínimo 6 caracteres)
   - Confirmación de contraseña

2. **Validaciones en Tiempo Real**
   - Validación de RUT chileno con algoritmo oficial
   - Validación de teléfono móvil (formato 9XXXXXXXX)
   - Validación de email con regex
   - Validación de dirección completa (campos requeridos)
   - Validación de contraseñas coincidentes
   - Limpieza automática de errores al escribir

3. **Experiencia de Usuario**
   - Auto-login después del registro exitoso
   - Redirección automática según el rol
   - Mensajes de error descriptivos
   - Estados de carga durante el proceso
   - Diseño responsive y accesible

4. **Integración con Backend**
   - Endpoint: `POST /api/Auth/register`
   - DTO: `UsuarioCreateDTO` (incluye `DireccionDTO`)
   - Rol predeterminado: "cliente"
   - Validación de duplicados (email y RUT)
   - **Creación automática de dirección en la base de datos**

## Estructura de Archivos

```
src/
├── pages/auth/
│   ├── Register.tsx          # Página principal de registro
│   └── Login.tsx             # Página de login (actualizada con enlace)
├── components/layout/
│   └── Navigation.tsx        # Navegación (actualizada con enlace)
├── services/
│   └── auth.ts              # Servicio de autenticación
└── constants/
    └── api.ts               # Configuración de endpoints
```

## Rutas

- **GET** `/register` - Página de registro de clientes
- **POST** `/api/Auth/register` - Endpoint de registro (backend)

## Campos de Dirección

### Campos Requeridos
- **Calle**: Nombre de la calle o avenida
- **Número**: Número de la casa o edificio
- **Comuna**: Comuna de la dirección
- **Región**: Región de Chile

### Campos Opcionales
- **Departamento**: Número o letra del departamento
- **Código Postal**: Código postal de la zona

## Validaciones Implementadas

### RUT Chileno
```typescript
const validateRut = (rut: string): boolean => {
    const cleanRut = rut.replace(/[.-]/g, '');
    if (cleanRut.length < 8 || cleanRut.length > 9) return false;
    
    const dv = cleanRut.slice(-1);
    const rutNumber = cleanRut.slice(0, -1);
    
    if (!/^\d+$/.test(rutNumber)) return false;
    
    let sum = 0;
    let multiplier = 2;
    
    for (let i = rutNumber.length - 1; i >= 0; i--) {
        sum += parseInt(rutNumber[i]) * multiplier;
        multiplier = multiplier === 7 ? 2 : multiplier + 1;
    }
    
    const expectedDv = 11 - (sum % 11);
    const calculatedDv = expectedDv === 11 ? '0' : expectedDv === 10 ? 'K' : expectedDv.toString();
    
    return dv.toUpperCase() === calculatedDv;
};
```

### Teléfono Chileno
```typescript
const validatePhone = (phone: string): boolean => {
    const cleanPhone = phone.replace(/[+\s-]/g, '');
    return /^9\d{8}$/.test(cleanPhone);
};
```

## Flujo de Registro

1. **Usuario accede** a `/register` desde:
   - Enlace en la navegación principal
   - Enlace en la página de login
   - URL directa

2. **Completa el formulario** con:
   - Datos personales
   - Información de contacto
   - **Dirección completa de envío**
   - Credenciales de acceso

3. **Validaciones automáticas** verifican:
   - Formato de email
   - RUT válido
   - Teléfono móvil chileno
   - **Campos de dirección requeridos**
   - Contraseñas coincidentes

4. **Envío al backend** con:
   - Datos del formulario
   - **Información de dirección**
   - Rol "cliente" predeterminado

5. **Procesamiento en el servidor**:
   - Creación del usuario
   - **Creación automática de la dirección**
   - Asignación como dirección principal

6. **Respuesta del servidor**:
   - ✅ Éxito: Auto-login y redirección
   - ❌ Error: Mensaje descriptivo

7. **Auto-login** después del registro exitoso

## Beneficios para el Cliente

- **Acceso completo** al catálogo de productos
- **Realizar pedidos** online
- **Seguimiento** de compras
- **Descuentos exclusivos**
- **Historial** de compras
- **Gestión de perfil** y direcciones
- **Dirección de envío** configurada automáticamente

## Configuración del Backend

### Roles Permitidos
```csharp
private static readonly HashSet<string> RolesPermitidos = new HashSet<string> 
{ 
    "administrador", "vendedor", "bodeguero", "contador", "repartidor", "cliente" 
};
```

### Validaciones del Servidor
- Verificación de email único
- Verificación de RUT único
- Hash seguro de contraseñas (BCrypt)
- Generación de JWT token
- **Creación automática de dirección** para clientes

## Estilos y Diseño

### Colores de Ferremas
- **Primario**: Azul corporativo
- **Secundario**: Naranja de acento
- **Peligro**: Rojo para errores
- **Éxito**: Verde para confirmaciones

### Componentes Utilizados
- Formularios con validación visual
- Botones con estados de carga
- Mensajes de error contextuales
- Diseño responsive con Tailwind CSS

## Próximas Mejoras

- [ ] Verificación de email por correo
- [ ] Captcha para prevenir bots
- [ ] Términos y condiciones obligatorios
- [ ] Integración con redes sociales
- [ ] Validación de edad mínima
- [ ] Notificaciones push de bienvenida

## Testing

### Casos de Prueba Recomendados

1. **Registro exitoso** con datos válidos
2. **Validación de RUT** con diferentes formatos
3. **Validación de teléfono** con formatos chilenos
4. **Validación de dirección** con campos requeridos
5. **Duplicados** de email y RUT
6. **Contraseñas** que no coinciden
7. **Campos requeridos** vacíos
8. **Formato de email** inválido
9. **Auto-login** después del registro
10. **Verificación de dirección** en la base de datos

## Soporte

Para problemas o consultas sobre la funcionalidad de registro:
- Revisar logs del navegador (F12)
- Verificar conectividad con el backend
- Comprobar que el backend esté ejecutándose
- Validar configuración de CORS si es necesario 