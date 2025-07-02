import React, { useState, useEffect } from 'react';
import { apiClient } from '../services/api';
import type { PedidoResponseDTO, ProductoResponseDTO } from '../types/api';

const ClienteView: React.FC = () => {
  const [pedidos, setPedidos] = useState<PedidoResponseDTO[]>([]);
  const [productosFavoritos, setProductosFavoritos] = useState<ProductoResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [stats, setStats] = useState({
    totalPedidos: 0,
    pedidosPendientes: 0,
    totalGastado: 0
  });
  const [productosCatalogo, setProductosCatalogo] = useState<ProductoResponseDTO[]>([]);

  useEffect(() => {
    const cargarDatos = async () => {
      try {
        setLoading(true);
        setError(null);

        // Intentar cargar datos de la API
        try {
          const [pedidosResponse, productosResponse] = await Promise.all([
            apiClient.get<any>('/api/Pedidos/mis-pedidos'),
            apiClient.get<any>('/api/Productos/favoritos')
          ]);

          // Extraer datos de manera flexible
          const pedidosData = pedidosResponse.data?.datos || pedidosResponse.data || [];
          const productosData = productosResponse.data?.datos || productosResponse.data || [];

          setPedidos(Array.isArray(pedidosData) ? pedidosData : []);
          setProductosFavoritos(Array.isArray(productosData) ? productosData : []);

          // Calcular estadísticas
          const pedidosPendientes = pedidosData.filter((p: any) => 
            p.estado === 'PENDIENTE' || p.estado === 'EN_PROCESO'
          ).length;

          const totalGastado = pedidosData
            .filter((p: any) => p.estado === 'COMPLETADO')
            .reduce((sum: number, p: any) => sum + (p.total || 0), 0);

          setStats({
            totalPedidos: pedidosData.length || 0,
            pedidosPendientes,
            totalGastado
          });

        } catch (apiError) {
          console.warn('Error al cargar datos de la API, usando datos de demostración:', apiError);
          
          // Datos de demostración
          setPedidos([
            {
              id: 1,
              usuarioId: 1,
              usuarioNombre: 'Cliente',
              fechaPedido: new Date(),
              total: 125000,
              estado: 'PENDIENTE',
              fechaCreacion: new Date(),
              activo: true,
              detalles: []
            },
            {
              id: 2,
              usuarioId: 1,
              usuarioNombre: 'Cliente',
              fechaPedido: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000), // 7 días atrás
              total: 89000,
              estado: 'COMPLETADO',
              fechaCreacion: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
              activo: true,
              detalles: []
            },
            {
              id: 3,
              usuarioId: 1,
              usuarioNombre: 'Cliente',
              fechaPedido: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000), // 14 días atrás
              total: 156000,
              estado: 'COMPLETADO',
              fechaCreacion: new Date(Date.now() - 14 * 24 * 60 * 60 * 1000),
              activo: true,
              detalles: []
            }
          ]);

          setProductosFavoritos([
            {
              id: 1,
              codigo: 'MART001',
              nombre: 'Martillo Stanley',
              descripcion: 'Martillo de carpintero profesional',
              precio: 25000,
              stock: 15,
              categoriaId: 1,
              categoriaNombre: 'Herramientas Manuales',
              marcaId: 1,
              marcaNombre: 'Stanley',
              fechaCreacion: new Date(),
              activo: true
            },
            {
              id: 2,
              codigo: 'DEST002',
              nombre: 'Destornillador Phillips',
              descripcion: 'Destornillador Phillips #2',
              precio: 5000,
              stock: 25,
              categoriaId: 1,
              categoriaNombre: 'Herramientas Manuales',
              marcaId: 2,
              marcaNombre: 'DeWalt',
              fechaCreacion: new Date(),
              activo: true
            }
          ]);

          setStats({
            totalPedidos: 3,
            pedidosPendientes: 1,
            totalGastado: 245000
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
        <h1 className="text-2xl font-bold text-ferremas-primary">Mi Panel de Cliente</h1>
        <p className="text-ferremas-gray-600 mt-2">Gestiona tus pedidos y favoritos</p>
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
            Total de Pedidos
          </h3>
          <p className="text-3xl font-bold text-ferremas-blue-600">
            {stats.totalPedidos}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            pedidos realizados
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
            en proceso
          </p>
        </div>

        <div className="card bg-gradient-to-br from-ferremas-green-50 to-white">
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            Total Gastado
          </h3>
          <p className="text-3xl font-bold text-ferremas-green-600">
            ${stats.totalGastado?.toLocaleString() || '0'}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            en compras completadas
          </p>
        </div>
      </div>

      {/* Mis Pedidos */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Mis Pedidos Recientes
        </h2>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead>
              <tr className="bg-ferremas-gray-50">
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">ID</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Fecha</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Total</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Estado</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {pedidos.length > 0 ? (
                pedidos.slice(0, 5).map((pedido) => (
                  <tr key={pedido.id} className="border-b border-ferremas-gray-100">
                    <td className="px-4 py-3">#{pedido.id}</td>
                    <td className="px-4 py-3">
                      {new Date(pedido.fechaCreacion).toLocaleDateString()}
                    </td>
                    <td className="px-4 py-3">${pedido.total?.toLocaleString() || '0'}</td>
                    <td className="px-4 py-3">
                      <span className={`badge-${getEstadoColor(pedido.estado)}`}>
                        {pedido.estado}
                      </span>
                    </td>
                    <td className="px-4 py-3">
                      <button className="text-ferremas-primary hover:text-ferremas-primary-dark text-sm">
                        Ver Detalles
                      </button>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No tienes pedidos registrados
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      {/* Productos Favoritos */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Mis Productos Favoritos
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {productosFavoritos.length > 0 ? (
            productosFavoritos.slice(0, 6).map((producto) => (
              <div key={producto.id} className="border border-ferremas-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow">
                <div className="flex justify-between items-start mb-3">
                  <h3 className="font-semibold text-ferremas-primary">{producto.nombre}</h3>
                  <button className="text-red-500 hover:text-red-700">
                    ❤️
                  </button>
                </div>
                <p className="text-ferremas-gray-600 text-sm mb-3 line-clamp-2">
                  {producto.descripcion}
                </p>
                <div className="flex justify-between items-center mb-3">
                  <span className="text-lg font-bold text-ferremas-primary">
                    ${producto.precio?.toLocaleString() || '0'}
                  </span>
                  <span className={`text-sm px-2 py-1 rounded-full ${
                    producto.stock > 10 
                      ? 'bg-green-100 text-green-800' 
                      : producto.stock > 0 
                      ? 'bg-yellow-100 text-yellow-800'
                      : 'bg-red-100 text-red-800'
                  }`}>
                    {producto.stock > 0 ? `${producto.stock} disponibles` : 'Agotado'}
                  </span>
                </div>
                <button className="w-full btn-primary text-sm">
                  Agregar al Carrito
                </button>
              </div>
            ))
          ) : (
            <div className="col-span-full text-center py-8 text-ferremas-gray-500">
              No tienes productos favoritos
            </div>
          )}
        </div>
      </section>

      {/* Catálogo de Productos */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Catálogo de Productos
        </h2>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead>
              <tr className="bg-ferremas-gray-50">
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Código</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Producto</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Descripción</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Precio</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Stock</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Acción</th>
              </tr>
            </thead>
            <tbody>
              {productosCatalogo && productosCatalogo.length > 0 ? (
                productosCatalogo.map((producto) => (
                  <tr key={producto.id} className="border-b border-ferremas-gray-100">
                    <td className="px-4 py-3">{producto.codigo}</td>
                    <td className="px-4 py-3">{producto.nombre}</td>
                    <td className="px-4 py-3">{producto.descripcion}</td>
                    <td className="px-4 py-3">${producto.precio?.toLocaleString() || '0'}</td>
                    <td className="px-4 py-3">{producto.stock}</td>
                    <td className="px-4 py-3">
                      <button className="btn-primary btn-xs" onClick={() => alert(`Agregar ${producto.nombre} al carrito`)}>
                        Agregar al Carrito
                      </button>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={6} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No hay productos en el catálogo
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      {/* Acciones Rápidas */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Acciones Rápidas
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <button className="p-4 border border-ferremas-gray-200 rounded-lg hover:bg-ferremas-gray-50 transition-colors">
            <div className="text-2xl mb-2">🛒</div>
            <h3 className="font-semibold text-ferremas-primary">Ver Catálogo</h3>
            <p className="text-sm text-ferremas-gray-600">Explora nuestros productos</p>
          </button>
          
          <button className="p-4 border border-ferremas-gray-200 rounded-lg hover:bg-ferremas-gray-50 transition-colors">
            <div className="text-2xl mb-2">📋</div>
            <h3 className="font-semibold text-ferremas-primary">Mis Pedidos</h3>
            <p className="text-sm text-ferremas-gray-600">Revisa el historial completo</p>
          </button>
          
          <button className="p-4 border border-ferremas-gray-200 rounded-lg hover:bg-ferremas-gray-50 transition-colors">
            <div className="text-2xl mb-2">⚙️</div>
            <h3 className="font-semibold text-ferremas-primary">Configuración</h3>
            <p className="text-sm text-ferremas-gray-600">Gestiona tu cuenta</p>
          </button>
        </div>
      </section>
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

export default ClienteView; 