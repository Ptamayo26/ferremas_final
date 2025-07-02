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

export const enviosService = {
  async buscarCoberturas(nombre: string): Promise<CoberturaResult[]> {
    const response = await apiClient.get(`/api/envios/coberturas?nombre=${encodeURIComponent(nombre)}`);
    return response.data;
  },

  async cotizarEnvio(request: CotizacionRequest): Promise<CotizacionResult[]> {
    const response = await apiClient.post('/api/envios/cotizar', request);
    return response.data;
  }
}; 