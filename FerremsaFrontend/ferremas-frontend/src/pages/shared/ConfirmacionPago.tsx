import React, { useEffect, useState } from 'react';
import { useLocation, Link } from 'react-router-dom';
import { api } from '../../services/api';

interface PedidoDetalle {
  numeroPedido: string;
  total: number;
  estado: string;
  fechaCreacion: string;
  tiempoEnvio?: string;
  urlBoleta?: string;
  urlFactura?: string;
  productos?: { nombre: string; cantidad: number; precio: number; precioConDescuento?: number; precioOriginal?: number }[];
  tipoDocumento: string;
  subtotal?: number;
  descuento?: number;
  impuestos?: number;
  envio?: number;
  descuentoBase?: number;
  descuentoCupon?: number;
  totalFinal?: number;
}

const ConfirmacionPago: React.FC = () => {
  const location = useLocation();
  const [pedido, setPedido] = useState<PedidoDetalle | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Extraer token_ws de la URL
  const params = new URLSearchParams(location.search);
  const token = params.get('token_ws');

  useEffect(() => {
    const fetchPedido = async () => {
      setLoading(true);
      setError(null);
      try {
        if (!token) throw new Error('Token de pago no encontrado.');
        const resp = await api.confirmarTransaccionWebpay(token);
        if (!resp || !resp.pedido) throw new Error('No se pudo obtener la información del pedido.');
        setPedido(resp.pedido);
        // Log para depuración de productos recibidos
        if (resp.pedido.productos) {
          console.log('Pedido recibido en confirmación:', resp.pedido);
        }
      } catch (e: any) {
        setError(e.message || 'No se pudo obtener la información del pedido.');
        setLoading(false);
      }
      setLoading(false);
    };
    if (token) fetchPedido();
    else {
      setError('Token de pago no encontrado.');
      setLoading(false);
    }
  }, [token]);

  if (loading) return <div className="text-center py-16">Cargando detalles del pago...</div>;
  if (error) return <div className="text-center py-16 text-red-500">{error}</div>;
  if (!pedido) return null;

  return (
    <div className="max-w-2xl mx-auto my-12 bg-white rounded-lg shadow-lg p-8">
      <div className="text-center mb-6">
        <div className="mx-auto flex items-center justify-center h-16 w-16 rounded-full bg-green-100 mb-4">
          <svg className="h-8 w-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <h2 className="text-2xl font-bold text-gray-900 mb-2">¡Pago Exitoso!</h2>
        <p className="text-ferremas-primary font-semibold">Tu pedido ha sido registrado correctamente.</p>
      </div>
      <div className="bg-gray-50 rounded-lg p-6 mb-6">
        <div className="space-y-2">
          <div className="flex justify-between items-center">
            <span className="text-gray-600">Número de Pedido:</span>
            <span className="font-semibold text-lg">{pedido.numeroPedido}</span>
          </div>
          <div className="flex justify-between items-center">
            <span className="text-gray-600">Total:</span>
            <span className="font-bold text-xl text-ferremas-primary">${Math.round(pedido.totalFinal ?? pedido.total).toLocaleString('es-CL')}</span>
          </div>
          <div className="flex justify-between items-center">
            <span className="text-gray-600">Estado:</span>
            <span className="badge-success">{pedido.estado}</span>
          </div>
          <div className="flex justify-between items-center">
            <span className="text-gray-600">Fecha:</span>
            <span className="font-medium">{new Date(pedido.fechaCreacion).toLocaleDateString('es-CL', { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' })}</span>
          </div>
          {pedido.tiempoEnvio && (
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Tiempo estimado de envío:</span>
              <span className="font-medium">{pedido.tiempoEnvio}</span>
            </div>
          )}
        </div>
      </div>
      <div className="mb-6">
        <h3 className="text-lg font-semibold mb-2">Productos:</h3>
        <ul className="divide-y divide-gray-200">
          {pedido.productos?.map((prod, idx) => {
            console.log("Renderizando producto:", prod);
            return (
              <li key={idx} className="py-2 flex justify-between items-center">
                <span>{prod.nombre} <span className="text-gray-500">x{prod.cantidad}</span></span>
                <span>
                  {prod.precioConDescuento && prod.precioConDescuento > 0 && prod.precioOriginal && prod.precioOriginal > prod.precioConDescuento ? (
                    <>
                      <span className="line-through text-gray-400 mr-2">${prod.precioOriginal.toLocaleString('es-CL')}</span>
                      <span className="text-ferremas-primary font-bold">${prod.precioConDescuento.toLocaleString('es-CL')}</span>
                    </>
                  ) : (
                    <span className="text-ferremas-primary font-bold">${prod.precio.toLocaleString('es-CL')}</span>
                  )}
                </span>
              </li>
            );
          })}
        </ul>
      </div>
      {/* Desglose de totales */}
      <div className="bg-gray-50 rounded-lg p-4 mb-6">
        <div className="space-y-1">
          {pedido.subtotal !== undefined && (
            <div className="flex justify-between">
              <span>Subtotal:</span>
              <span>${Math.round(pedido.subtotal).toLocaleString('es-CL')}</span>
            </div>
          )}
          {(pedido.descuentoBase !== undefined && pedido.descuentoBase > 0) && (
            <div className="flex justify-between text-green-700">
              <span>Descuento base:</span>
              <span>- ${Math.round(pedido.descuentoBase).toLocaleString('es-CL')}</span>
            </div>
          )}
          {(pedido.descuentoCupon !== undefined && pedido.descuentoCupon > 0) && (
            <div className="flex justify-between text-green-700">
              <span>Descuento cupón:</span>
              <span>- ${Math.round(pedido.descuentoCupon).toLocaleString('es-CL')}</span>
            </div>
          )}
          {pedido.impuestos !== undefined && pedido.impuestos > 0 && (
            <div className="flex justify-between">
              <span>Impuestos:</span>
              <span>${Math.round(pedido.impuestos).toLocaleString('es-CL')}</span>
            </div>
          )}
          {pedido.envio !== undefined && pedido.envio > 0 && (
            <div className="flex justify-between">
              <span>Envío:</span>
              <span>${Math.round(pedido.envio).toLocaleString('es-CL')}</span>
            </div>
          )}
          <div className="flex justify-between font-bold text-ferremas-primary text-lg mt-2">
            <span>Total pagado:</span>
            <span>${Math.round(pedido.totalFinal ?? pedido.total).toLocaleString('es-CL')}</span>
          </div>
        </div>
        <p className="text-xs text-gray-500 mt-2">El total pagado incluye todos los descuentos, impuestos y envío.</p>
      </div>
      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        {pedido.tipoDocumento !== 'factura' && (
          <a
            href={`/api/facturas/boleta/${pedido.numeroPedido}`}
            className="btn-secondary flex-1"
            target="_blank"
            rel="noopener noreferrer"
          >
            Descargar Boleta
          </a>
        )}
        {pedido.tipoDocumento === 'factura' && (
          <a
            href={`/api/facturas/factura/${pedido.numeroPedido}`}
            className="btn-secondary flex-1"
            target="_blank"
            rel="noopener noreferrer"
          >
            Descargar Factura
          </a>
        )}
      </div>
      <div className="flex flex-col sm:flex-row gap-3">
        <Link to="/catalogo" className="btn-primary flex-1">
          Seguir Comprando
        </Link>
        <Link to="/mi-cuenta" className="btn-secondary flex-1">
          Ver Mis Pedidos
        </Link>
      </div>
    </div>
  );
};

export default ConfirmacionPago; 