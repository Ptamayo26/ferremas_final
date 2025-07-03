import { apiClient } from './api';
import { CHILE_GEO_ENDPOINTS } from '../constants/api';
import type { RegionChile, ComunaChile, ChileGeoResponse } from '../types/api';

class ChileGeoService {
  /**
   * Obtiene todas las regiones de Chile
   */
  async getRegiones(): Promise<RegionChile[]> {
    try {
      const response = await apiClient.get<ChileGeoResponse>(CHILE_GEO_ENDPOINTS.GET_REGIONES);
      
      if (response.data.exito) {
        return response.data.data as RegionChile[];
      } else {
        throw new Error(response.data.mensaje || 'Error al obtener regiones');
      }
    } catch (error: any) {
      console.error('Error al obtener regiones:', error);
      throw new Error('No se pudieron obtener las regiones de Chile');
    }
  }

  /**
   * Obtiene las comunas de una región específica
   */
  async getComunasByRegion(codigoRegion: string): Promise<ComunaChile[]> {
    try {
      const response = await apiClient.get<ChileGeoResponse>(
        CHILE_GEO_ENDPOINTS.GET_COMUNAS_BY_REGION(codigoRegion)
      );
      
      if (response.data.exito) {
        return response.data.data as ComunaChile[];
      } else {
        throw new Error(response.data.mensaje || 'Error al obtener comunas');
      }
    } catch (error: any) {
      console.error('Error al obtener comunas para región:', codigoRegion, error);
      throw new Error('No se pudieron obtener las comunas de la región');
    }
  }

  /**
   * Obtiene todas las comunas de Chile
   */
  async getTodasLasComunas(): Promise<ComunaChile[]> {
    try {
      const response = await apiClient.get<ChileGeoResponse>(CHILE_GEO_ENDPOINTS.GET_TODAS_COMUNAS);
      
      if (response.data.exito) {
        return response.data.data as ComunaChile[];
      } else {
        throw new Error(response.data.mensaje || 'Error al obtener comunas');
      }
    } catch (error: any) {
      console.error('Error al obtener todas las comunas:', error);
      throw new Error('No se pudieron obtener las comunas de Chile');
    }
  }

  /**
   * Busca comunas por nombre (búsqueda parcial)
   */
  async buscarComunas(query: string): Promise<ComunaChile[]> {
    try {
      const response = await apiClient.get<ChileGeoResponse>(
        `${CHILE_GEO_ENDPOINTS.BUSCAR_COMUNAS}?query=${encodeURIComponent(query)}`
      );
      
      if (response.data.exito) {
        return response.data.data as ComunaChile[];
      } else {
        throw new Error(response.data.mensaje || 'Error al buscar comunas');
      }
    } catch (error: any) {
      console.error('Error al buscar comunas:', query, error);
      throw new Error('No se pudieron buscar las comunas');
    }
  }

  /**
   * Obtiene información sobre la API
   */
  async getInfo(): Promise<any> {
    try {
      const response = await apiClient.get(CHILE_GEO_ENDPOINTS.INFO);
      return response.data;
    } catch (error: any) {
      console.error('Error al obtener información de la API:', error);
      throw new Error('No se pudo obtener información de la API');
    }
  }

  /**
   * Carga inicial de datos (regiones y comunas principales)
   */
  async cargarDatosIniciales(): Promise<{
    regiones: RegionChile[];
    comunasPrincipales: ComunaChile[];
  }> {
    try {
      const [regiones, todasLasComunas] = await Promise.all([
        this.getRegiones(),
        this.getTodasLasComunas()
      ]);

      // Filtrar comunas principales (las más pobladas)
      const comunasPrincipales = todasLasComunas.filter(comuna => 
        ['Santiago', 'Providencia', 'Las Condes', 'Ñuñoa', 'Maipú', 'La Florida', 
         'Puente Alto', 'San Bernardo', 'Valparaíso', 'Viña del Mar', 'Concepción', 
         'Talcahuano', 'Chillán'].includes(comuna.nombre)
      );

      return {
        regiones,
        comunasPrincipales
      };
    } catch (error: any) {
      console.error('Error al cargar datos iniciales:', error);
      throw new Error('No se pudieron cargar los datos geográficos');
    }
  }
}

// Exportar una instancia singleton
export const chileGeoService = new ChileGeoService();
export default chileGeoService; 