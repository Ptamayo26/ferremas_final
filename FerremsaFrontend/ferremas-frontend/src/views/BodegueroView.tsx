import React, { useState, useEffect } from 'react';
import { apiClient } from '../services/api';
import type { PedidoResponseDTO, ProductoResponseDTO } from '../types/api';

// Tipos adicionales para la funcionalidad del bodeguero
interface Ubicacion {
  id: number;
  nombre: string;
  descripcion: string;
  capacidad: number;
  productosAsignados: number;
}

interface Proveedor {
  id: number;
  nombre: string;
  contacto: string;
  telefono: string;
  email: string;
  productos: number;
}

interface MovimientoInventario {
  id: number;
  productoId: number;
  productoNombre: string;
  tipo: 'ENTRADA' | 'SALIDA' | 'AJUSTE';
  cantidad: number;
  cantidadAnterior: number;
  cantidadNueva: number;
  motivo: string;
  fecha: string;
  usuario: string;
}

const BodegueroView: React.FC = () => {
  const [productos, setProductos] = useState<ProductoResponseDTO[]>([]);
  const [pedidos, setPedidos] = useState<PedidoResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [stats, setStats] = useState({
    totalProductos: 0,
    productosBajoStock: 0,
    productosAgotados: 0,
    pedidosPendientes: 0
  });
  const [activeTab, setActiveTab] = useState('dashboard');
  const [selectedProducto, setSelectedProducto] = useState<ProductoResponseDTO | null>(null);
  const [showModal, setShowModal] = useState(false);
  const [modalType, setModalType] = useState<'recepcion' | 'ajuste' | 'ubicacion'>('recepcion');
  const [verTodosPedidos, setVerTodosPedidos] = useState(false);

  // Estados para formularios
  const [formData, setFormData] = useState({
    cantidad: '',
    motivo: '',
    ubicacionId: '',
    proveedorId: ''
  });

  useEffect(() => {
    const cargarDatos = async () => {
      try {
        setLoading(true);
        setError(null);

        // Intentar cargar datos de la API
        try {
          const [productosResponse, pedidosResponse] = await Promise.all([
            apiClient.get<any>('/api/Productos'),
            apiClient.get<any>('/api/Pedidos')
          ]);

          // Extraer datos de manera flexible y robusta
          const productosData = productosResponse.data?.datos
            || productosResponse.data?.productos
            || productosResponse.data?.Datos
            || productosResponse.data?.Productos
            || productosResponse.data
            || [];
          const pedidosData = pedidosResponse.data?.datos
            || pedidosResponse.data?.pedidos
            || pedidosResponse.data?.Datos
            || pedidosResponse.data?.Pedidos
            || pedidosResponse.data
            || [];

          setProductos(Array.isArray(productosData) ? productosData : []);
          setPedidos(Array.isArray(pedidosData) ? pedidosData : []);

          // Calcular estadísticas
          const productosBajoStock = Array.isArray(productosData) ? productosData.filter((p: any) => 
            p.stock > 0 && p.stock <= 10
          ).length : 0;

          const productosAgotados = Array.isArray(productosData) ? productosData.filter((p: any) => 
            p.stock === 0
          ).length : 0;

          const pedidosPendientes = Array.isArray(pedidosData) ? pedidosData.filter((p: any) => {
            const estado = p.estado?.toLowerCase().replace(/_/g, ' ');
            return estado === 'pendiente' || estado === 'en proceso';
          }).length : 0;

          setStats({
            totalProductos: Array.isArray(productosData) ? productosData.length : 0,
            productosBajoStock,
            productosAgotados,
            pedidosPendientes
          });

        } catch (apiError) {
          console.warn('Error al cargar datos de la API, usando datos de demostración:', apiError);
          
          // Datos de demostración
          setProductos([
            {
              id: 1,
              codigo: 'MART001',
              nombre: 'Martillo Stanley',
              descripcion: 'Martillo de carpintero profesional',
              precio: 25000,
              stock: 0,
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
              stock: 5,
              categoriaId: 1,
              categoriaNombre: 'Herramientas Manuales',
              marcaId: 2,
              marcaNombre: 'DeWalt',
              fechaCreacion: new Date(),
              activo: true
            },
            {
              id: 3,
              codigo: 'TALAD003',
              nombre: 'Taladro Eléctrico',
              descripcion: 'Taladro inalámbrico 18V',
              precio: 45000,
              stock: 15,
              categoriaId: 2,
              categoriaNombre: 'Herramientas Eléctricas',
              marcaId: 3,
              marcaNombre: 'Makita',
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
            }
          ]);

          setStats({
            totalProductos: 3,
            productosBajoStock: 1,
            productosAgotados: 1,
            pedidosPendientes: 1
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

  const handleRecepcion = (producto: ProductoResponseDTO) => {
    setSelectedProducto(producto);
    setModalType('recepcion');
    setShowModal(true);
  };

  const handleAjuste = (producto: ProductoResponseDTO) => {
    setSelectedProducto(producto);
    setModalType('ajuste');
    setShowModal(true);
  };

  const handleUbicacion = (producto: ProductoResponseDTO) => {
    setSelectedProducto(producto);
    setModalType('ubicacion');
    setShowModal(true);
  };

  const handleSubmit = async () => {
    // Aquí iría la lógica para procesar el formulario
    console.log('Procesando:', modalType, formData);
    setShowModal(false);
    setFormData({ cantidad: '', motivo: '', ubicacionId: '', proveedorId: '' });
  };

  return (
    <div className="p-6 space-y-6">
      <header className="mb-8">
        <h1 className="text-2xl font-bold text-ferremas-primary">Panel de Bodeguero</h1>
        <p className="text-ferremas-gray-600 mt-2">Gestiona inventario y pedidos</p>
      </header>

      {error && (
        <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-red-600">{error}</p>
        </div>
      )}

      {/* Estadísticas */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div className="card bg-gradient-to-br from-ferremas-blue-50 to-white">
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            Total de Productos
          </h3>
          <p className="text-3xl font-bold text-ferremas-blue-600">
            {stats.totalProductos}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            productos en inventario
          </p>
        </div>

        <div className="card bg-gradient-to-br from-ferremas-orange-50 to-white">
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            Bajo Stock
          </h3>
          <p className="text-3xl font-bold text-ferremas-orange-600">
            {stats.productosBajoStock}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            requieren reposición
          </p>
        </div>

        <div className="card bg-gradient-to-br from-ferremas-red-50 to-white">
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            Agotados
          </h3>
          <p className="text-3xl font-bold text-ferremas-danger">
            {stats.productosAgotados}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            sin stock disponible
          </p>
        </div>

        <div className="card bg-gradient-to-br from-ferremas-green-50 to-white">
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            Pedidos Pendientes
          </h3>
          <p className="text-3xl font-bold text-ferremas-green-600">
            {stats.pedidosPendientes}
          </p>
          <p className="text-ferremas-gray-600 mt-2">
            requieren atención
          </p>
        </div>
      </div>

      {/* Productos Bajo Stock */}
      <section className="card">
        <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
          Productos Bajo Stock
        </h2>
        <div className="overflow-x-auto">
          <table className="min-w-full">
            <thead>
              <tr className="bg-ferremas-gray-50">
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Código</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Producto</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Categoría</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Stock</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Precio</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Estado</th>
              </tr>
            </thead>
            <tbody>
              {productos.filter(p => p.stock <= 10).length > 0 ? (
                productos
                  .filter(p => p.stock <= 10)
                  .map((producto) => (
                    <tr key={producto.id} className="border-b border-ferremas-gray-100">
                      <td className="px-4 py-3">{producto.codigo}</td>
                      <td className="px-4 py-3">{producto.nombre}</td>
                      <td className="px-4 py-3">{producto.categoriaNombre || 'N/A'}</td>
                      <td className="px-4 py-3">{producto.stock}</td>
                      <td className="px-4 py-3">${producto.precio?.toLocaleString() || '0'}</td>
                      <td className="px-4 py-3">
                        <span className={`badge-${producto.stock === 0 ? 'danger' : 'warning'}`}>
                          {producto.stock === 0 ? 'Agotado' : 'Bajo Stock'}
                        </span>
                      </td>
                    </tr>
                  ))
              ) : (
                <tr>
                  <td colSpan={6} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No hay productos con bajo stock
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
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Categoría</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Stock</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Precio</th>
              </tr>
            </thead>
            <tbody>
              {productos.length > 0 ? (
                productos.map((producto) => (
                  <tr key={producto.id} className="border-b border-ferremas-gray-100">
                    <td className="px-4 py-3">{producto.codigo}</td>
                    <td className="px-4 py-3">{producto.nombre}</td>
                    <td className="px-4 py-3">{producto.categoriaNombre || 'N/A'}</td>
                    <td className="px-4 py-3">{producto.stock}</td>
                    <td className="px-4 py-3">${producto.precio?.toLocaleString() || '0'}</td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No hay productos en inventario
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      {/* Pedidos Pendientes o Todos los Pedidos */}
      <section className="card">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-semibold text-ferremas-primary">
            {verTodosPedidos ? 'Todos los Pedidos' : 'Pedidos Pendientes'}
          </h2>
          <button
            className="btn-secondary btn-xs"
            onClick={() => setVerTodosPedidos(v => !v)}
          >
            {verTodosPedidos ? 'Ver solo pendientes' : 'Ver todos los pedidos'}
          </button>
        </div>
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
              {(verTodosPedidos ? pedidos : pedidos.filter(p => {
                const estado = p.estado?.toLowerCase().replace(/_/g, ' ');
                return estado === 'pendiente' || estado === 'en proceso';
              })).length > 0 ? (
                (verTodosPedidos ? pedidos : pedidos.filter(p => {
                  const estado = p.estado?.toLowerCase().replace(/_/g, ' ');
                  return estado === 'pendiente' || estado === 'en proceso';
                })).map((pedido) => (
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
                      <button className="btn-primary btn-xs" onClick={() => alert(`Preparar pedido #${pedido.id}`)}>
                        Preparar
                      </button>
                      <button className="btn-secondary btn-xs" onClick={() => alert(`Entregar pedido #${pedido.id}`)}>
                        Entregar
                      </button>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={6} className="px-4 py-8 text-center text-ferremas-gray-500">
                    No hay pedidos {verTodosPedidos ? 'registrados' : 'pendientes'}
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      {/* Contenido del Dashboard */}
      {activeTab === 'dashboard' && (
        <div className="space-y-6">
          {/* Tarjetas de resumen */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div className="card">
              <div className="flex items-center">
                <div className="p-2 bg-ferremas-green-100 rounded-lg">
                  <span className="text-2xl">📦</span>
                </div>
                <div className="ml-4">
                  <p className="text-sm font-medium text-ferremas-gray-600">Total Productos</p>
                  <p className="text-2xl font-bold text-ferremas-primary">{Array.isArray(productos) ? productos.length : 0}</p>
                </div>
              </div>
            </div>

            <div className="card">
              <div className="flex items-center">
                <div className="p-2 bg-ferremas-orange-100 rounded-lg">
                  <span className="text-2xl">⚠️</span>
                </div>
                <div className="ml-4">
                  <p className="text-sm font-medium text-ferremas-gray-600">Bajo Stock</p>
                  <p className="text-2xl font-bold text-ferremas-orange-600">{stats.productosBajoStock}</p>
                </div>
              </div>
            </div>

            <div className="card">
              <div className="flex items-center">
                <div className="p-2 bg-red-100 rounded-lg">
                  <span className="text-2xl">🚫</span>
                </div>
                <div className="ml-4">
                  <p className="text-sm font-medium text-ferremas-gray-600">Agotados</p>
                  <p className="text-2xl font-bold text-red-600">{stats.productosAgotados}</p>
                </div>
              </div>
            </div>

            <div className="card">
              <div className="flex items-center">
                <div className="p-2 bg-ferremas-blue-100 rounded-lg">
                  <span className="text-2xl">📋</span>
                </div>
                <div className="ml-4">
                  <p className="text-sm font-medium text-ferremas-gray-600">Pedidos Pendientes</p>
                  <p className="text-2xl font-bold text-ferremas-blue-600">{stats.pedidosPendientes}</p>
                </div>
              </div>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Panel de Pedidos Pendientes */}
            <section className="card">
              <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
                Pedidos Pendientes
              </h2>
              <div className="space-y-4">
                {Array.isArray(pedidos) ? pedidos.slice(0, 5).map((pedido) => (
                  <div 
                    key={pedido.id}
                    className="p-4 bg-white rounded-lg border border-ferremas-gray-100 hover:shadow-md transition-shadow"
                  >
                    <div className="flex justify-between items-center">
                      <span className="font-medium">Pedido #{pedido.id}</span>
                      <span className="badge-warning">
                        {pedido.estado}
                      </span>
                    </div>
                    <div className="mt-2 text-ferremas-gray-600">
                      <p>Total: ${pedido.total.toFixed(2)}</p>
                      <p>Fecha: {new Date(pedido.fechaCreacion).toLocaleDateString()}</p>
                    </div>
                    <div className="mt-3 flex space-x-2">
                      <button className="btn-primary text-sm flex-1">
                        Recibir
                      </button>
                      <button className="btn-secondary text-sm flex-1">
                        Detalles
                      </button>
                    </div>
                  </div>
                )) : null}
              </div>
              <button className="btn-primary w-full mt-4">
                Ver Todos los Pedidos
              </button>
            </section>

            {/* Panel de Productos Bajo Stock */}
            <section className="card">
              <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
                Productos Bajo Stock
              </h2>
              <div className="space-y-4">
                {Array.isArray(productos) ? productos.slice(0, 5).map((producto: any) => (
                  <div 
                    key={producto.id}
                    className="p-4 bg-white rounded-lg border border-ferremas-gray-100 hover:shadow-md transition-shadow"
                  >
                    <div className="flex justify-between items-center">
                      <span className="font-medium">{producto.nombre}</span>
                      <span className={`badge-${producto.stock === 0 ? 'danger' : 'warning'}`}>
                        Stock: {producto.stock}
                      </span>
                    </div>
                    <div className="mt-2 text-ferremas-gray-600">
                      <p>Código: {producto.codigo}</p>
                    </div>
                    <div className="mt-3 flex space-x-2">
                      <button 
                        onClick={() => handleRecepcion(producto)}
                        className="btn-primary text-sm flex-1"
                      >
                        Recibir
                      </button>
                      <button 
                        onClick={() => handleAjuste(producto)}
                        className="btn-secondary text-sm flex-1"
                      >
                        Ajustar
                      </button>
                    </div>
                  </div>
                )) : null}
              </div>
              <button className="btn-primary w-full mt-4">
                Ver Inventario Completo
              </button>
            </section>
          </div>
        </div>
      )}

      {/* Contenido de Reportes */}
      {activeTab === 'reportes' && (
        <div className="space-y-6">
          <h2 className="text-2xl font-bold text-ferremas-primary">Reportes de Inventario</h2>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Reporte de Stock */}
            <div className="card">
              <h3 className="text-lg font-semibold text-ferremas-primary mb-4">Estado del Stock</h3>
              <div className="space-y-4">
                <div className="flex justify-between items-center">
                  <span className="text-ferremas-gray-600">Productos con stock normal</span>
                  <span className="font-bold text-ferremas-green-600">
                    {productos.filter(p => p.stock >= 10).length}
                  </span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-ferremas-gray-600">Productos bajo stock</span>
                  <span className="font-bold text-ferremas-orange-600">
                    {stats.productosBajoStock}
                  </span>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-ferremas-gray-600">Productos agotados</span>
                  <span className="font-bold text-red-600">
                    {stats.productosAgotados}
                  </span>
                </div>
              </div>
              <button className="btn-primary w-full mt-4">Generar Reporte</button>
            </div>

            {/* Reporte de Valor */}
            <div className="card">
              <h3 className="text-lg font-semibold text-ferremas-primary mb-4">Valor del Inventario</h3>
              <div className="space-y-4">
                <div className="text-center">
                  <p className="text-3xl font-bold text-ferremas-primary">
                    ${productos.reduce((total, p) => total + (p.precio * p.stock), 0).toLocaleString()}
                  </p>
                  <p className="text-ferremas-gray-600">Valor total en inventario</p>
                </div>
                <div className="text-center">
                  <p className="text-2xl font-bold text-ferremas-green-600">
                    {productos.length}
                  </p>
                  <p className="text-ferremas-gray-600">Productos diferentes</p>
                </div>
              </div>
              <button className="btn-primary w-full mt-4">Exportar Reporte</button>
            </div>
          </div>

          {/* Gráfico de movimientos */}
          <div className="card">
            <h3 className="text-lg font-semibold text-ferremas-primary mb-4">Movimientos Recientes</h3>
            <div className="bg-ferremas-gray-50 p-8 rounded-lg text-center">
              <p className="text-ferremas-gray-600">Aquí irá el gráfico de movimientos de inventario</p>
            </div>
          </div>
        </div>
      )}

      {/* Modal para acciones */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg shadow-xl p-6 max-w-md w-full">
            <h3 className="text-lg font-semibold text-ferremas-primary mb-4">
              {modalType === 'recepcion' && 'Recepción de Mercancía'}
              {modalType === 'ajuste' && 'Ajuste de Stock'}
              {modalType === 'ubicacion' && 'Cambiar Ubicación'}
            </h3>
            
            {selectedProducto && (
              <div className="mb-4 p-3 bg-ferremas-gray-50 rounded-lg">
                <p className="font-medium">{selectedProducto.nombre}</p>
                <p className="text-sm text-ferremas-gray-600">Stock actual: {selectedProducto.stock}</p>
              </div>
            )}

            <div className="space-y-4">
              {modalType === 'recepcion' && (
                <>
                  <div className="form-group">
                    <label className="text-sm font-medium text-ferremas-gray-700">Cantidad a recibir</label>
                    <input 
                      type="number" 
                      value={formData.cantidad}
                      onChange={(e) => setFormData({...formData, cantidad: e.target.value})}
                      className="input-field"
                      placeholder="0"
                    />
                  </div>
                  <div className="form-group">
                    <label className="text-sm font-medium text-ferremas-gray-700">Proveedor</label>
                    <select 
                      value={formData.proveedorId}
                      onChange={(e) => setFormData({...formData, proveedorId: e.target.value})}
                      className="input-field"
                    >
                      <option value="">Seleccionar proveedor</option>
                      {/* Add proveedor options here */}
                    </select>
                  </div>
                </>
              )}

              {modalType === 'ajuste' && (
                <>
                  <div className="form-group">
                    <label className="text-sm font-medium text-ferremas-gray-700">Cantidad de ajuste</label>
                    <input 
                      type="number" 
                      value={formData.cantidad}
                      onChange={(e) => setFormData({...formData, cantidad: e.target.value})}
                      className="input-field"
                      placeholder="0 (positivo para agregar, negativo para restar)"
                    />
                  </div>
                  <div className="form-group">
                    <label className="text-sm font-medium text-ferremas-gray-700">Motivo</label>
                    <textarea 
                      value={formData.motivo}
                      onChange={(e) => setFormData({...formData, motivo: e.target.value})}
                      className="input-field"
                      rows={3}
                      placeholder="Explicar el motivo del ajuste..."
                    />
                  </div>
                </>
              )}

              {modalType === 'ubicacion' && (
                <div className="form-group">
                  <label className="text-sm font-medium text-ferremas-gray-700">Nueva ubicación</label>
                  <select 
                    value={formData.ubicacionId}
                    onChange={(e) => setFormData({...formData, ubicacionId: e.target.value})}
                    className="input-field"
                  >
                    <option value="">Seleccionar ubicación</option>
                    {/* Add ubicacion options here */}
                  </select>
                </div>
              )}
            </div>

            <div className="flex justify-end space-x-3 mt-6">
              <button 
                onClick={() => setShowModal(false)}
                className="btn-secondary"
              >
                Cancelar
              </button>
              <button 
                onClick={handleSubmit}
                className="btn-primary"
              >
                Confirmar
              </button>
            </div>
          </div>
        </div>
      )}
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

export default BodegueroView; 