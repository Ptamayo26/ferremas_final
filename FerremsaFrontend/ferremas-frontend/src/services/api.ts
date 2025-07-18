// src/services/api.ts
import axios from 'axios';
import type { LoginDTO, Pedido, ProductoResponseDTO, UsuarioCreateDTO, CarritoItemDTO, CarritoResumenDTO } from '../types/api';
import { STORAGE_KEYS } from '../constants/api';

// En desarrollo, usar URL relativa para que funcione el proxy de Vite
// En producción, usar la URL completa del backend
const API_URL = import.meta.env.VITE_API_URL || (import.meta.env.DEV ? '' : 'http://localhost:5200');

// Cliente de API público para endpoints que no requieren autenticación
export const publicApiClient = axios.create({
  baseURL: API_URL,
});

// Cliente de API autenticado
export const apiClient = axios.create({
    baseURL: API_URL,
});

// Interceptor para agregar el token de autenticación SOLO a las peticiones autenticadas
apiClient.interceptors.request.use(
  (config) => {
    // Obtenemos el token directamente de localStorage usando la clave correcta
    const token = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
    // Solo agregamos Authorization si el token existe y no es carrito anónimo
    if (token && token !== 'null' && token !== 'undefined') {
      config.headers.Authorization = `Bearer ${token}`;
    } else {
      // Eliminamos la cabecera Authorization si el usuario es anónimo
      delete config.headers.Authorization;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// El publicApiClient NO debe tener interceptores de autenticación
// para evitar enviar tokens en endpoints públicos

// Interceptor de respuesta para publicApiClient (solo logging, no limpiar tokens)
publicApiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      console.warn('Endpoint público devolvió 401 - verificar configuración del backend');
    }
    return Promise.reject(error);
  }
);

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Intentar refrescar el token automáticamente
      const { authService } = await import('./auth');
      const refreshSuccess = await authService.refreshToken();
      
      if (refreshSuccess) {
        // Reintentar la petición original con el nuevo token
        const originalRequest = error.config;
        const token = authService.getToken();
        if (token) {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return apiClient(originalRequest);
        }
      } else {
        // Si no se puede refrescar, limpiar datos de autenticación
        localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN);
        localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
        localStorage.removeItem(STORAGE_KEYS.USER_DATA);
        localStorage.removeItem(STORAGE_KEYS.USER_ROLE);
        console.log('Token inválido y no se pudo refrescar, datos de autenticación limpiados');
      }
    }
    return Promise.reject(error);
  }
);

// Función para convertir fechas de string a Date
const convertCarritoItem = (item: any): CarritoItemDTO => ({
  ...item,
  fechaAgregado: new Date(item.fechaAgregado)
});

export const api = {
  // Productos
  async getProductos(): Promise<ProductoResponseDTO[]> {
    const response = await publicApiClient.get<{productos: ProductoResponseDTO[]}>('/api/Productos');
    return response.data.productos;
  },

  async getProductoById(id: number): Promise<ProductoResponseDTO> {
    const response = await publicApiClient.get<ProductoResponseDTO>(`/api/Productos/${id}`);
    return response.data;
  },

  // Pedidos
  async getMisPedidos(): Promise<Pedido[]> {
    const response = await apiClient.get<any>('/api/pedidos/mis');
    return response.data.datos || response.data;
  },

  // Carrito
  async agregarAlCarrito(productoId: number, cantidad: number): Promise<CarritoItemDTO> {
    const response = await apiClient.post<any>('/api/carrito', { productoId, cantidad });
    return convertCarritoItem(response.data);
  },

  async getCarrito(): Promise<CarritoItemDTO[]> {
    const response = await apiClient.get<any[]>('/api/carrito');
    return response.data.map(convertCarritoItem);
  },

  async getCarritoResumen(): Promise<CarritoResumenDTO> {
    const response = await apiClient.get<CarritoResumenDTO>('/api/carrito/resumen');
    return response.data;
  },

  async actualizarCantidadCarrito(itemId: number, cantidad: number): Promise<any> {
    const response = await apiClient.put(`/api/carrito/${itemId}`, { cantidad });
    return response.data;
  },

  async eliminarItemCarrito(itemId: number): Promise<any> {
    const response = await apiClient.delete(`/api/carrito/${itemId}`);
    return response.data;
  },

  async limpiarCarrito(): Promise<any> {
    const response = await apiClient.delete('/api/carrito');
    return response.data;
  },


  async confirmarTransaccionWebpay(token: string): Promise<any> {
    const response = await apiClient.post('/api/Webpay/confirmar-transaccion', { Token: token });
    return response.data;
  },

  // Marcas
  async getMarcas(): Promise<{ id: number; nombre: string; descripcion?: string; logoUrl?: string }[]> {
    const response = await publicApiClient.get('/api/Marcas');
    return response.data;
  },

  // Categorías
  async getCategorias(): Promise<{ id: number; nombre: string; descripcion?: string; codigo?: string }[]> {
    const response = await publicApiClient.get('/api/Categorias');
    return response.data;
  },

  // Facturas
  async getFacturaByPedidoId(pedidoId: number): Promise<any> {
    const response = await apiClient.get(`/api/facturas/factura/${pedidoId}`);
    return response.data;
  }
};