export type EstadoPedido = 
  | 'PENDIENTE'
  | 'PROCESANDO'
  | 'ENVIADO'
  | 'ENTREGADO'
  | 'CANCELADO';

export interface Pedido {
  id: number;
  fechaPedido: string; // O Date, dependiendo de cómo se serialice
  estado: EstadoPedido;
  total: number;
  // Añadir más campos si es necesario
}
