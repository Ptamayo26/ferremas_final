import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { apiClient, api } from '../services/api';
import type { PedidoResponseDTO, ProductoResponseDTO } from '../types/api';
import { useAuth } from '../context/AuthContext';
import ClienteInfoCard from '../components/ui/ClienteInfoCard';
import EditProfileModal from '../components/ui/EditProfileModal';
import DireccionesModal from '../components/ui/DireccionesModal';
import CambiarPasswordModal from '../components/ui/CambiarPasswordModal';
import { getFavoritos } from '../services/products';

// NUEVO: Tipo para los datos de perfil
interface PerfilCliente {
  nombre: string;
  apellido?: string;
  rut?: string;
  correoElectronico?: string;
  telefono?: string;
  direcciones?: any[];
  puntos?: number;
}

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
  const [perfil, setPerfil] = useState<PerfilCliente | null>(null);
  const [showEditProfile, setShowEditProfile] = useState(false);
  const [showDirecciones, setShowDirecciones] = useState(false);
  const [showCambiarPassword, setShowCambiarPassword] = useState(false);
  
  const navigate = useNavigate();
  const { user } = useAuth();

  useEffect(() => {
    const cargarDatos = async () => {
      try {
        setLoading(true);
        setError(null);

        // NUEVO: Obtener datos de perfil
        try {
          const perfilResponse = await apiClient.get<any>('/api/Clientes/mi-perfil');
          setPerfil(perfilResponse.data);
        } catch (e) {
          setPerfil(null);
        }

        // Intentar cargar datos de la API
        try {
          const [pedidosResponse, favoritosResponse] = await Promise.all([
            apiClient.get<any>('/api/Pedidos/mis'),
            getFavoritos()
          ]);

          // Extraer datos de manera flexible
          const pedidosData = pedidosResponse.data?.datos || pedidosResponse.data || [];
          setPedidos(Array.isArray(pedidosData) ? pedidosData : []);
          setProductosFavoritos(Array.isArray(favoritosResponse) ? favoritosResponse.map((p: any) => ({
            id: p.id,
            codigo: p.codigo || '',
            nombre: p.nombre,
            descripcion: p.descripcion || '',
            precio: p.precio,
            stock: p.stock,
            categoriaId: p.categoriaId || 0,
            categoriaNombre: p.categoriaNombre || '',
            marcaId: p.marcaId || 0,
            marcaNombre: p.marcaNombre || '',
            imagenUrl: p.imagenUrl || '',
            especificaciones: p.especificaciones || '',
            fechaCreacion: p.fechaCreacion || new Date(),
            fechaModificacion: p.fechaModificacion,
            activo: true,
            precioOriginal: p.precioOriginal || p.precio,
            precioConDescuento: p.precioConDescuento || p.precio
          })) : []);

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
          setPedidos([]);
          setProductosFavoritos([]);
          setStats({
            totalPedidos: 0,
            pedidosPendientes: 0,
            totalGastado: 0
          });
        }

        // NUEVO: Cargar productos del catálogo para administración
        try {
          const productosResponse = await apiClient.get<any>('/api/Productos');
          const productosData = Array.isArray(productosResponse.data.productos)
            ? productosResponse.data.productos
            : Array.isArray(productosResponse.data)
            ? productosResponse.data
            : [];
          setProductosCatalogo(productosData);
        } catch (e) {
          setProductosCatalogo([]);
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

  // Función para ver detalles del pedido
  const verFactura = (pedido: any) => {
    alert(`Detalles del pedido #${pedido.id}\nEstado: ${pedido.estado}\nTotal: $${pedido.total?.toLocaleString()}`);
  };

  // Función para eliminar producto
  const eliminarProducto = async (id: number) => {
    if (!window.confirm('¿Estás seguro de que deseas eliminar este producto? Esta acción no se puede deshacer.')) return;
    try {
      await apiClient.delete(`/api/Productos/${id}`);
      setProductosCatalogo(productosCatalogo.filter(p => p.id !== id));
      alert('Producto eliminado exitosamente');
    } catch (e) {
      alert('Error al eliminar el producto');
    }
  };

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
        <div className="flex justify-between items-start">
          <div>
            <h1 className="text-2xl font-bold text-ferremas-primary">
              Mi Cuenta - {perfil?.nombre || user?.nombre || 'Usuario'}
            </h1>
            <p className="text-ferremas-gray-600 mt-2">
              Gestiona tus pedidos, favoritos y accede a funcionalidades de cliente
            </p>
            <p className="text-sm text-ferremas-gray-500 mt-1">
              Rol: {user?.rol || 'Usuario'}
            </p>
          </div>
          <div className="flex space-x-2">
            <button
              onClick={() => navigate('/catalogo')}
              className="bg-ferremas-primary text-white px-4 py-2 rounded hover:bg-ferremas-primary-dark transition"
            >
              Ver Catálogo
            </button>
            {user?.rol && user.rol !== 'cliente' && user.rol !== 'CLIENT' && (
              <button
                onClick={() => {
                  // Navegar al dashboard correspondiente al rol
                  const dashboardRoutes: { [key: string]: string } = {
                    'administrador': '/admin/dashboard',
                    'ADMIN': '/admin/dashboard',
                    'vendedor': '/vendedor/dashboard',
                    'SELLER': '/vendedor/dashboard',
                    'bodeguero': '/bodeguero/dashboard',
                    'WAREHOUSE': '/bodeguero/dashboard',
                    'contador': '/contador/dashboard',
                    'ACCOUNTANT': '/contador/dashboard',
                    'repartidor': '/repartidor/dashboard',
                    'DELIVERY': '/repartidor/dashboard'
                  };
                  const route = dashboardRoutes[user.rol];
                  if (route) {
                    navigate(route);
                  }
                }}
                className="bg-ferremas-secondary text-white px-4 py-2 rounded hover:bg-ferremas-secondary-dark transition"
              >
                Mi Dashboard
              </button>
            )}
            {/* BOTONES SOLO PARA ADMINISTRADOR */}
            {user?.rol && (user.rol.toLowerCase() === 'administrador' || user.rol.toUpperCase() === 'ADMIN') && (
              <></>
            )}
          </div>
        </div>
        {/* NUEVO: Datos personales con acciones */}
        {perfil && (
          <div className="mt-4 mb-6 bg-ferremas-blue-50 border border-ferremas-blue-200 rounded-lg p-4">
            <div className="flex justify-between items-center mb-2">
              <span className="text-lg font-semibold text-ferremas-primary">Datos personales</span>
              <div className="flex gap-2">
                <button className="btn-secondary btn-xs" onClick={() => setShowEditProfile(true)}>Editar</button>
                <button className="btn-secondary btn-xs" onClick={() => setShowCambiarPassword(true)}>Cambiar contraseña</button>
              </div>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <div><span className="font-semibold">Nombre:</span> {perfil.nombre} {perfil.apellido || ''}</div>
                {perfil.rut && <div><span className="font-semibold">RUT:</span> {perfil.rut}</div>}
                {perfil.correoElectronico && <div><span className="font-semibold">Correo:</span> {perfil.correoElectronico}</div>}
                {perfil.telefono && <div><span className="font-semibold">Teléfono:</span> {perfil.telefono}</div>}
                {typeof perfil.puntos === 'number' && <div><span className="font-semibold">Puntos acumulados:</span> {perfil.puntos}</div>}
              </div>
            </div>
            <EditProfileModal open={showEditProfile} onClose={() => setShowEditProfile(false)} perfil={perfil} onUpdated={setPerfil} />
            <CambiarPasswordModal open={showCambiarPassword} onClose={() => setShowCambiarPassword(false)} />
          </div>
        )}
        {/* NUEVO: Sección de direcciones */}
        {perfil && (
          <div className="mb-6 bg-ferremas-blue-50 border border-ferremas-blue-200 rounded-lg p-4">
            <div className="flex justify-between items-center mb-2">
              <span className="text-lg font-semibold text-ferremas-primary">Direcciones</span>
              <button className="btn-secondary btn-xs" onClick={() => setShowDirecciones(true)}>Gestionar</button>
            </div>
            {perfil.direcciones && perfil.direcciones.length > 0 ? (
              <ul className="list-disc ml-6">
                {perfil.direcciones.map((dir, idx) => (
                  <li key={idx} className="text-sm">
                    {dir.Calle || dir.calle || ''} {dir.Numero || dir.numero || ''}, {dir.Comuna || dir.comuna || ''}, {dir.Region || dir.region || ''} {dir.EsPrincipal || dir.esPrincipal ? <span className="ml-2 text-xs text-ferremas-success">(Principal)</span> : null}
                  </li>
                ))}
              </ul>
            ) : <div className="text-ferremas-gray-500 text-sm">No tienes direcciones registradas.</div>}
            <DireccionesModal 
              open={showDirecciones} 
              onClose={() => setShowDirecciones(false)} 
              direcciones={perfil.direcciones || []} 
              onUpdated={d => setPerfil(p => p ? { ...p, direcciones: d } : p)}
              usuarioId={user?.id || 1}
            />
          </div>
        )}
      </header>

      {error && (
        <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-red-600">{error}</p>
        </div>
      )}

      {/* Información de Funcionalidades */}
      <ClienteInfoCard className="mb-6" />

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
                      <button
                        className="text-ferremas-primary hover:text-ferremas-primary-dark text-sm"
                        onClick={() => verFactura(pedido)}
                      >
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
                {producto.nombre === 'Taladro Percutor 650W' ? (
                  <img
                    src={'http://localhost:5200/images/productos/Taladro percutor 650W.webp'}
                    alt={producto.nombre}
                    className="w-full h-32 object-contain mb-2"
                    onError={e => { e.currentTarget.src = '/images/productos/default.png'; }}
                  />
                ) : (
                  producto.imagenUrl && (
                    <img
                      src={producto.imagenUrl}
                      alt={producto.nombre}
                      className="w-full h-32 object-contain mb-2"
                      onError={e => { e.currentTarget.src = '/images/productos/default.png'; }}
                    />
                  )
                )}
                <p className="text-ferremas-gray-600 text-sm mb-3 line-clamp-2">
                  {producto.descripcion || 'Sin descripción'}
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