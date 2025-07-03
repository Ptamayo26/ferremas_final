import { apiClient } from './api';
import type { 
  CheckoutResumenDTO, 
  CheckoutRequestDTO, 
  CheckoutResponseDTO,
  ProcesarPagoRequestDTO,
  ProcesarPagoResponseDTO 
} from '../types/api';

export const checkoutService = {
  // Obtener resumen del checkout
  async getResumen(): Promise<CheckoutResumenDTO> {
    const response = await apiClient.get<CheckoutResumenDTO>('/api/Checkout/resumen');
    return response.data;
  },

  // Procesar checkout
  async procesarCheckout(data: CheckoutRequestDTO): Promise<CheckoutResponseDTO> {
    const response = await apiClient.post<CheckoutResponseDTO>('/api/Checkout', data);
    return response.data;
  },

  // Procesar pago
  async procesarPago(data: ProcesarPagoRequestDTO): Promise<ProcesarPagoResponseDTO> {
    const response = await apiClient.post<ProcesarPagoResponseDTO>('/api/Checkout/pago', data);
    return response.data;
  },

  // Validar código de descuento
  async validarCodigoDescuento(codigo: string) {
    const response = await fetch(`/api/Descuentos/codigo/${codigo}`);
    if (!response.ok) throw new Error('Código de descuento inválido');
    return await response.json();
  },

  // Validar código de descuento para carrito anónimo
  async validarCodigoDescuentoAnonimo(codigo: string, productos: any[]) {
    const response = await fetch('/api/Descuentos/validar-anonimo', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ codigo, productos }),
    });
    if (!response.ok) throw new Error('Código de descuento inválido o expirado');
    return await response.json();
  }
}; 