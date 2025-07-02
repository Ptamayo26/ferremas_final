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
  productos?: { nombre: string; cantidad: number; precio: number }[];
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
        setLoading(false);
      } catch (e: any) {
        setError(e.message || 'No se pudo obtener la información del pedido.');
        setLoading(false);
      }
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
            <span className="font-bold text-xl text-ferremas-primary">${pedido.total.toLocaleString('es-CL')}</span>
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
          {pedido.productos?.map((prod, idx) => (
            <li key={idx} className="py-2 flex justify-between">
              <span>{prod.nombre} <span className="text-gray-500">x{prod.cantidad}</span></span>
              <span>${prod.precio.toLocaleString('es-CL')}</span>
            </li>
          ))}
        </ul>
      </div>
      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        {pedido.urlBoleta && (
          <a href={pedido.urlBoleta} className="btn-secondary flex-1" download>
            Descargar Boleta
          </a>
        )}
        {pedido.urlFactura && (
          <a href={pedido.urlFactura} className="btn-secondary flex-1" download>
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