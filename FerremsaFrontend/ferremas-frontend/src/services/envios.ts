import { apiClient } from './api';

export interface CoberturaResult {
  countyCode: string;
  countyName: string;
  regionCode: string;
  ineCountyCode: number;
  queryMode: number;
  coverageName: string;
}

export interface CotizacionRequest {
  codigoCoberturaOrigen: string;
  codigoCoberturaDestino: string;
  peso: number;
  largo: number;
  ancho: number;
  alto: number;
}

export interface CotizacionResult {
  servicio: string;
  precio: number;
  plazoEntrega: string;
}

export interface CotizacionEnvioRequest {
  origen: string;
  destino: string;
  peso?: number;
  alto?: number;
  ancho?: number;
  largo?: number;
}

export interface CotizacionEnvioResponse {
  shipit: {
    quotations: Array<{
      id: number;
      courier: string;
      service: string;
      price: number;
      deliveryTime: number;
      deliveryTimeText: string;
    }>;
  };
  chilexpress: Array<{
    servicio: string;
    precio: number;
    plazoEntrega: string;
  }>;
}

class EnviosService {
  /**
   * Obtiene cotizaciones de envío desde múltiples proveedores
   */
  async obtenerCotizaciones(request: CotizacionEnvioRequest): Promise<CotizacionEnvioResponse> {
    try {
      const params = new URLSearchParams({
        origen: request.origen,
        destino: request.destino,
        peso: (request.peso || 1).toString(),
        alto: (request.alto || 10).toString(),
        ancho: (request.ancho || 10).toString(),
        largo: (request.largo || 10).toString()
      });

      const response = await apiClient.get<CotizacionEnvioResponse>(`/api/Envios/cotizaciones?${params}`);
      return response.data;
    } catch (error: any) {
      console.error('Error al obtener cotizaciones de envío:', error);
      throw new Error('No se pudieron obtener las cotizaciones de envío');
    }
  }

  /**
   * Busca coberturas en Shipit
   */
  async buscarCoberturasShipit(query: string) {
    try {
      const response = await apiClient.get(`/api/Envios/shipit/coberturas?query=${encodeURIComponent(query)}`);
      return response.data;
    } catch (error: any) {
      console.error('Error al buscar coberturas en Shipit:', error);
      throw new Error('No se pudieron buscar las coberturas');
    }
  }

  /**
   * Obtiene el costo de envío basado en el método seleccionado
   */
  obtenerCostoEnvio(metodoEnvio: string): number {
    switch (metodoEnvio) {
      case 'Chilexpress':
        return 4990;
      case 'Starken':
        return 3990;
      case 'Shipit':
        return 3500; // Precio base para Shipit
      default:
        return 0;
    }
  }
}

export const enviosService = new EnviosService(); 