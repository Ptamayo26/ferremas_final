import React, { useState, useEffect } from 'react';
import GestionClientes from '../components/vendedor/GestionClientes';
import Carrito from '../components/sales/Carrito';
import Checkout from '../components/sales/Checkout';
import ConfirmacionPedido from '../components/sales/ConfirmacionPedido';
import OpenCarritoButton from '../components/ui/OpenCarritoButton';
import type { CheckoutResponseDTO, ClienteResponseDTO, PedidoResponseDTO } from '../types/api';
import { apiClient } from '../services/api';

const VendedorView: React.FC = () => {
  const [carritoOpen, setCarritoOpen] = useState(false);
  const [checkoutOpen, setCheckoutOpen] = useState(false);
  const [confirmacionOpen, setConfirmacionOpen] = useState(false);
  const [pedidoConfirmado, setPedidoConfirmado] = useState<CheckoutResponseDTO | null>(null);
  const [clientes, setClientes] = useState<ClienteResponseDTO[]>([]);
  const [pedidos, setPedidos] = useState<PedidoResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [stats, setStats] = useState({
    totalClientes: 0,
    pedidosPendientes: 0,
    ventasHoy: 0
  });

  const handleCheckoutSuccess = (pedido: CheckoutResponseDTO) => {
    setPedidoConfirmado(pedido);
    setConfirmacionOpen(true);
    setCarritoOpen(false);
    setCheckoutOpen(false);
  };

  const handleCloseConfirmacion = () => {
    setConfirmacionOpen(false);
    setPedidoConfirmado(null);
  };

  useEffect(() => {
    const cargarDatos = async () => {
      try {
        setLoading(true);
        setError(null);

        // Intentar cargar datos de la API
        try {
          const [clientesResponse, pedidosResponse] = await Promise.all([
            apiClient.get<any>('/api/Clientes'),
            apiClient.get<any>('/api/Pedidos')
          ]);

          // Extraer datos de manera flexible
          const clientesData = clientesResponse.data?.datos || clientesResponse.data || [];
          const pedidosData = pedidosResponse.data?.datos || pedidosResponse.data || [];

          setClientes(Array.isArray(clientesData) ? clientesData : []);
          setPedidos(Array.isArray(pedidosData) ? pedidosData : []);

          // Calcular estadísticas
          const pedidosPendientes = pedidosData.filter((p: any) => 
            p.estado === 'PENDIENTE' || p.estado === 'EN_PROCESO'
          ).length;

          const hoy = new Date().toISOString().split('T')[0];
          const ventasHoy = pedidosData.filter((p: any) => 
            p.fechaCreacion?.includes(hoy) && p.estado === 'COMPLETADO'
          ).reduce((sum: number, p: any) => sum + (p.total || 0), 0);

          setStats({
            totalClientes: clientesData.length || 0,
            pedidosPendientes,
            ventasHoy
          });

        } catch (apiError) {
          console.warn('Error al cargar datos de la API, usando datos de demostración:', apiError);
          
          // Datos de demostración
          setClientes([
            {
              id: 1,
              nombre: 'Juan',
              apellido: 'Pérez',
              rut: '12345678-9',
              correoElectronico: 'juan.perez@email.com',
              telefono: '+56912345678',
              fechaCreacion: new Date(),
              activo: true
            },
            {
              id: 2,
              nombre: 'María',
              apellido: 'González',
              rut: '98765432-1',
              correoElectronico: 'maria.gonzalez@email.com',
              telefono: '+56987654321',
              fechaCreacion: new Date(),
              activo: true
            }
          ]);

          setPedidos([
            {
              id: 1,
              usuarioId: 1,
              usuarioNombre: 'Juan Pérez',
              fechaPedido: new Date(),
              total: 125000,
              estado: 'PENDIENTE',
              fechaCreacion: new Date(),
              activo: true,
              detalles: []
            },
            {
              id: 2,
              usuarioId: 2,
              usuarioNombre: 'María González',
              fechaPedido: new Date(),
              total: 89000,
              estado: 'COMPLETADO',
              fechaCreacion: new Date(),
              activo: true,
              detalles: []
            }
          ]);

          setStats({
            totalClientes: 2,
            pedidosPendientes: 1,
            ventasHoy: 89000
          });
        }

      } catch (error) {
        console.error('Error general al cargar datos:', error);
        setError('Error al cargar los datos del dashboard');
      } finally {
        setLoading(false);
      }
    };

    cargarDatos();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin text-4xl">⚙️</div>
        <p className="ml-4 text-lg">Cargando dashboard...</p>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6">
      <header className="mb-8">
        <h1 className="text-2xl font-bold text-ferremas-primary">Panel de Vendedor</h1>
        <p className="text-ferremas-gray-600 mt-2">Gestiona clientes y pedidos</p>
      </header>

      {error && (
        <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-red-600">{error}</p>
        </div>
      )}

      {/* Estadísticas */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="card bg-gradient-to-br from-ferremas-blue-50 to-white">
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            Total de Clientes
          </h3>
          <p className="text-3xl font-bold text-ferremas-blue-600">
            {stats.totalClientes}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            clientes registrados
          </p>
        </div>

        <div className="card bg-gradient-to-br from-ferremas-orange-50 to-white">
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            Pedidos Pendientes
          </h3>
          <p className="text-3xl font-bold text-ferremas-orange-600">
            {stats.pedidosPendientes}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            requieren atención
          </p>
        </div>

        <div className="card bg-gradient-to-br from-ferremas-green-50 to-white">
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            Ventas de Hoy
          </h3>
          <p className="text-3xl font-bold text-ferremas-green-600">
            ${stats.ventasHoy?.toLocaleString() || '0'}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            en ventas realizadas
          </p>
        </div>
      </div>

      {/* Clientes Recientes */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Clientes Recientes
        </h2>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead>
              <tr className="bg-ferremas-gray-50">
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Nombre</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">RUT</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Email</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Teléfono</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Estado</th>
              </tr>
            </thead>
            <tbody>
              {clientes.length > 0 ? (
                clientes.slice(0, 5).map((cliente) => (
                  <tr key={cliente.id} className="border-b border-ferremas-gray-100">
                    <td className="px-4 py-3">
                      {cliente.nombre} {cliente.apellido}
                    </td>
                    <td className="px-4 py-3">{cliente.rut}</td>
                    <td className="px-4 py-3">{cliente.correoElectronico}</td>
                    <td className="px-4 py-3">{cliente.telefono || 'N/A'}</td>
                    <td className="px-4 py-3">
                      <span className={`badge-${cliente.activo ? 'success' : 'danger'}`}>
                        {cliente.activo ? 'Activo' : 'Inactivo'}
                      </span>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No hay clientes registrados
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      {/* Pedidos Recientes */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Pedidos Recientes
        </h2>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead>
              <tr className="bg-ferremas-gray-50">
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">ID</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Cliente</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Fecha</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Total</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Estado</th>
              </tr>
            </thead>
            <tbody>
              {pedidos.length > 0 ? (
                pedidos.slice(0, 5).map((pedido) => (
                  <tr key={pedido.id} className="border-b border-ferremas-gray-100">
                    <td className="px-4 py-3">#{pedido.id}</td>
                    <td className="px-4 py-3">{pedido.usuarioNombre || 'Cliente'}</td>
                    <td className="px-4 py-3">
                      {new Date(pedido.fechaCreacion).toLocaleDateString()}
                    </td>
                    <td className="px-4 py-3">${pedido.total?.toLocaleString() || '0'}</td>
                    <td className="px-4 py-3">
                      <span className={`badge-${getEstadoColor(pedido.estado)}`}>
                        {pedido.estado}
                      </span>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No hay pedidos registrados
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      {/* Inventario Completo */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Inventario Completo
        </h2>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead>
              <tr className="bg-ferremas-gray-50">
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Código</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Producto</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Stock</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Precio</th>
              </tr>
            </thead>
            <tbody>
              {productos && productos.length > 0 ? (
                productos.map((producto) => (
                  <tr key={producto.id} className="border-b border-ferremas-gray-100">
                    <td className="px-4 py-3">{producto.codigo}</td>
                    <td className="px-4 py-3">{producto.nombre}</td>
                    <td className="px-4 py-3">{producto.stock}</td>
                    <td className="px-4 py-3">${producto.precio?.toLocaleString() || '0'}</td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={4} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No hay productos en inventario
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      {/* Pedidos Pendientes */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Pedidos Pendientes
        </h2>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead>
              <tr className="bg-ferremas-gray-50">
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">ID</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Cliente</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Fecha</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Total</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Estado</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {pedidos.filter(p => p.estado === 'PENDIENTE' || p.estado === 'EN_PROCESO').length > 0 ? (
                pedidos
                  .filter(p => p.estado === 'PENDIENTE' || p.estado === 'EN_PROCESO')
                  .map((pedido) => (
                    <tr key={pedido.id} className="border-b border-ferremas-gray-100">
                      <td className="px-4 py-3">#{pedido.id}</td>
                      <td className="px-4 py-3">{pedido.usuarioNombre || 'Cliente'}</td>
                      <td className="px-4 py-3">{new Date(pedido.fechaCreacion).toLocaleDateString()}</td>
                      <td className="px-4 py-3">${pedido.total?.toLocaleString() || '0'}</td>
                      <td className="px-4 py-3">
                        <span className={`badge-${getEstadoColor(pedido.estado)}`}>
                          {pedido.estado}
                        </span>
                      </td>
                      <td className="px-4 py-3 space-x-2">
                        <button className="btn-primary btn-xs" onClick={() => alert(`Aprobar pedido #${pedido.id}`)}>
                          Aprobar
                        </button>
                        <button className="btn-danger btn-xs" onClick={() => alert(`Rechazar pedido #${pedido.id}`)}>
                          Rechazar
                        </button>
                        <button className="btn-secondary btn-xs" onClick={() => alert(`Enviar a bodega pedido #${pedido.id}`)}>
                          Enviar a Bodega
                        </button>
                      </td>
                    </tr>
                  ))
              ) : (
                <tr>
                  <td colSpan={6} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No hay pedidos pendientes
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      {/* Componentes de ventas */}
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-ferremas-primary">
          Panel de Vendedor
        </h1>
        <OpenCarritoButton onOpen={() => setCarritoOpen(true)} />
      </div>
      
      <GestionClientes />
      
      <Carrito 
        isOpen={carritoOpen}
        onClose={() => setCarritoOpen(false)}
        onCheckout={() => {
          setCarritoOpen(false);
          setCheckoutOpen(true);
        }}
      />
      
      <Checkout
        isOpen={checkoutOpen}
        onClose={() => setCheckoutOpen(false)}
        onSuccess={handleCheckoutSuccess}
      />

      <ConfirmacionPedido
        isOpen={confirmacionOpen}
        onClose={handleCloseConfirmacion}
        pedido={pedidoConfirmado}
      />
    </div>
  );
};

const getEstadoColor = (estado: string): string => {
  switch (estado?.toUpperCase()) {
    case 'PENDIENTE':
      return 'warning';
    case 'EN_PROCESO':
      return 'info';
    case 'COMPLETADO':
      return 'success';
    case 'CANCELADO':
      return 'danger';
    default:
      return 'secondary';
  }
};

export default VendedorView; 