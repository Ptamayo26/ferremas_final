// src/constants/api.ts
// Configuración para conectar con tu backend de Ferremas

// ========================================
// CONFIGURACIÓN BASE DE LA API
// ========================================

// URL base de tu backend (ajusta según tu configuración)
export const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5200';

// Timeout para las peticiones (30 segundos)
export const API_CONFIG = {
TIMEOUT: 30000,
RETRY_ATTEMPTS: 3,
RETRY_DELAY: 1000,
} as const;

// Headers por defecto para todas las peticiones
export const API_HEADERS = {
'Content-Type': 'application/json',
'Accept': 'application/json',
} as const;

// ========================================
// ENDPOINTS DE AUTENTICACIÓN
// ========================================

export const AUTH_ENDPOINTS = {
LOGIN: '/api/Auth/login',
REGISTER: '/api/Auth/register',
RESET_PASSWORD: '/api/Auth/reset-password',
LOGOUT: '/api/Auth/logout',
} as const;

// ========================================
// ENDPOINTS DE PRODUCTOS
// ========================================

export const PRODUCT_ENDPOINTS = {
GET_ALL: '/api/Productos',
GET_BY_ID: (id: number) => `/api/Productos/${id}`,
CREATE: '/api/Productos',
UPDATE: (id: number) => `/api/Productos/${id}`,
DELETE: (id: number) => `/api/Productos/${id}`,
BY_CATEGORY: (categoriaId: number) => `/api/Productos/categoria/${categoriaId}`,
BY_BRAND: (marcaId: number) => `/api/Productos/marca/${marcaId}`,
SEARCH: '/api/Productos/search',
UPDATE_STOCK: (id: number) => `/api/Productos/${id}/stock`,
GET_ACTIVE: '/api/Productos/activos',
} as const;

// ========================================
// ENDPOINTS DE PEDIDOS
// ========================================

export const ORDER_ENDPOINTS = {
GET_ALL: '/api/Pedidos',
GET_MY_ORDERS: '/api/Pedidos/mis',
GET_BY_ID: (id: number) => `/api/Pedidos/${id}`,
CREATE: '/api/Pedidos',
UPDATE_STATUS: (id: number) => `/api/Pedidos/${id}/estado`,
DELETE: (id: number) => `/api/Pedidos/${id}`,
BY_USER: (usuarioId: number) => `/api/Pedidos/usuario/${usuarioId}`,
} as const;

// ========================================
// ENDPOINTS DE COMPARACIÓN
// ========================================

export const COMPARISON_ENDPOINTS = {
COMPARE: '/api/Comparador/comparar',
HISTORY: '/api/Comparador/historial',
ALL_HISTORY: '/api/Comparador/historial/todos',
} as const;

// ========================================
// ENDPOINTS DE MERCADOLIBRE
// ========================================

export const MERCADOLIBRE_ENDPOINTS = {
TEST: '/api/mercadolibre/test',
SEARCH: '/api/mercadolibre/buscar',
COMPARE: '/api/mercadolibre/comparar',
DETAIL: (itemId: string) => `/api/mercadolibre/detalle/${itemId}`,
SUGGESTIONS: '/api/mercadolibre/sugerencias',
} as const;

// ========================================
// ENDPOINTS DE USUARIOS
// ========================================

export const USER_ENDPOINTS = {
GET_ALL: '/api/Usuarios',
GET_PROFILE: '/api/Usuarios/perfil',
GET_BY_ID: (id: number) => `/api/Usuarios/${id}`,
CREATE: '/api/Usuarios',
UPDATE: (id: number) => `/api/Usuarios/${id}`,
UPDATE_PROFILE: '/api/Usuarios/perfil',
DELETE: (id: number) => `/api/Usuarios/${id}`,
REACTIVATE: (id: number) => `/api/Usuarios/reactivar/${id}`,
} as const;

// ========================================
// ENDPOINTS DE CLIENTES
// ========================================

export const CLIENT_ENDPOINTS = {
GET_ALL: '/api/Clientes',
GET_BY_ID: (id: number) => `/api/Clientes/${id}`,
CREATE: '/api/Clientes',
UPDATE: (id: number) => `/api/Clientes/${id}`,
DELETE: (id: number) => `/api/Clientes/${id}`,
} as const;

// ========================================
// ENDPOINTS DE PAGOS
// ========================================

export const PAYMENT_ENDPOINTS = {
GET_ALL: '/api/Pagos',
GET_BY_ID: (id: number) => `/api/Pagos/${id}`,
CREATE: '/api/Pagos',
PROCESS: (pedidoId: number) => `/api/Pagos/procesar/${pedidoId}`,
WEBHOOK: '/api/Pagos/webhook',
DELETE: (id: number) => `/api/Pagos/${id}`,
} as const;

// ========================================
// ENDPOINTS DE ENVÍOS
// ========================================

export const SHIPPING_ENDPOINTS = {
GET_BY_ORDER: (pedidoId: number) => `/api/Envios/${pedidoId}`,
CREATE: '/api/Envios',
UPDATE_STATUS: (envioId: number) => `/api/Envios/${envioId}/estado`,
} as const;

// ========================================
// ENDPOINTS DE GEOGRAFÍA DE CHILE
// ========================================

export const CHILE_GEO_ENDPOINTS = {
GET_REGIONES: '/api/ChileGeo/regiones',
GET_COMUNAS_BY_REGION: (codigoRegion: string) => `/api/ChileGeo/regiones/${codigoRegion}/comunas`,
GET_TODAS_COMUNAS: '/api/ChileGeo/comunas',
BUSCAR_COMUNAS: '/api/ChileGeo/comunas/buscar',
INFO: '/api/ChileGeo/info',
} as const;

// ========================================
// ENDPOINTS DE REPORTES
// ========================================

export const REPORT_ENDPOINTS = {
SALES: '/api/reportes/ventas',
SHIPMENTS: '/api/reportes/envios',
} as const;

// ========================================
// CONSTANTES DE ESTADO
// ========================================

export const ORDER_STATUSES = {
PENDING: 'Pendiente',
IN_PROCESS: 'En Proceso',
SHIPPED: 'Enviado',
DELIVERED: 'Entregado',
CANCELLED: 'Cancelado',
} as const;

export const USER_ROLES = {
ADMIN: 'administrador',
SELLER: 'vendedor',
CLIENT: 'cliente',
WAREHOUSE: 'bodeguero',
ACCOUNTANT: 'contador',
DELIVERY: 'repartidor',
} as const;

// ========================================
// CONFIGURACIÓN DE ALMACENAMIENTO LOCAL
// ========================================

export const STORAGE_KEYS = {
AUTH_TOKEN: 'ferremas_token',
USER_DATA: 'ferremas_user',
USER_ROLE: 'ferremas_user_role',
THEME: 'ferremas_theme',
LANGUAGE: 'ferremas_language',
} as const;