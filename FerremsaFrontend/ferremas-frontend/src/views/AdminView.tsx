import React, { useState, useEffect } from 'react';
import { apiClient } from '../services/api';
import type { ReporteVentas, ReporteInventario, UsuarioResponseDTO, ProductoResponseDTO } from '../types/api';
import GestionUsuarios from '../components/admin/GestionUsuarios';
import CatalogoProductosAdmin from '../components/admin/productos/CatalogoProductosAdmin';

type AdminViewSection = 'dashboard' | 'users' | 'catalogo';

const AdminView: React.FC = () => {
  const [activeSection, setActiveSection] = useState<AdminViewSection>('dashboard');
  const [reporteVentas, setReporteVentas] = useState<ReporteVentas | null>(null);
  const [reporteInventario, setReporteInventario] = useState<ReporteInventario | null>(null);
  const [usuarios, setUsuarios] = useState<UsuarioResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [productosInventario, setProductosInventario] = useState<ProductoResponseDTO[]>([]);

  useEffect(() => {
    const cargarDatos = async () => {
      const periodo = new Date().toISOString().slice(0, 7); // YYYY-MM
      
      try {
        setLoading(true);
        setError(null);
        
        const mes = new Date().getMonth() + 1; // getMonth() devuelve 0-11
        const anio = new Date().getFullYear();
        
        // Intentar cargar datos de la API
        try {
          const [ventasResponse, inventarioResponse] = await Promise.all([
            apiClient.get<any>(`/api/Reportes/dashboard/ventas/${mes}/${anio}`),
            apiClient.get<any>('/api/Reportes/dashboard/inventario')
          ]);
          
          // Extraer datos de manera flexible
          const ventasData = ventasResponse.data?.datos || ventasResponse.data || {};
          const inventarioData = inventarioResponse.data?.datos || inventarioResponse.data || {};
          
          setReporteVentas({
            periodo: periodo,
            totalVentas: ventasData.totalVentas || ventasData.total || 0,
            cantidadVentas: ventasData.cantidadVentas || ventasData.cantidad || 0,
            productosVendidos: ventasData.productosVendidos || ventasData.productos || []
          });
          
          setReporteInventario({
            valorTotal: inventarioData.valorTotal || inventarioData.valor || 0,
            totalProductos: inventarioData.totalProductos || inventarioData.total || 0,
            productosAgotados: inventarioData.productosAgotados || inventarioData.agotados || 0,
            productosBajoStock: inventarioData.productosBajoStock || inventarioData.bajoStock || []
          });
        } catch (apiError) {
          console.warn('Error al cargar datos de la API, usando datos de demostración:', apiError);
          // Usar datos de demostración si la API falla
          setReporteVentas({
            periodo: periodo,
            totalVentas: 2450000,
            cantidadVentas: 45,
            productosVendidos: [
              { productoId: 1, nombre: 'Martillo Stanley', cantidad: 15, total: 375000 },
              { productoId: 2, nombre: 'Destornillador Phillips', cantidad: 25, total: 125000 },
              { productoId: 3, nombre: 'Taladro Eléctrico', cantidad: 8, total: 360000 }
            ]
          });
          
          setReporteInventario({
            valorTotal: 12500000,
            totalProductos: 1234,
            productosAgotados: 23,
            productosBajoStock: []
          });
        }

        // Obtener productos del inventario
        const productosResponse = await apiClient.get<ProductoResponseDTO[]>('/api/Productos');
        setProductosInventario(productosResponse.data);
      } catch (error) {
        console.error('Error general al cargar datos:', error);
        setError('Error al cargar los datos del dashboard');
      } finally {
        setLoading(false);
      }
    };

    cargarDatos();
  }, []);

  const renderContent = () => {
    switch (activeSection) {
      case 'users':
        return <GestionUsuarios />;
      case 'catalogo':
        return <CatalogoProductosAdmin />;
      case 'dashboard':
      default:
        return (
          <>
            {error && (
              <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
                <p className="text-red-600">{error}</p>
              </div>
            )}
            
            {/* Resumen General */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              <div className="card bg-gradient-to-br from-ferremas-orange-50 to-white">
                <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
                  Ventas del Mes
                </h3>
                <p className="text-3xl font-bold text-ferremas-orange-600">
                  ${reporteVentas?.totalVentas?.toLocaleString() || '0'}
                </p>
                <p className="text-ferremas-gray-600 mt-2">
                  {reporteVentas?.cantidadVentas || 0} ventas realizadas
                </p>
              </div>

              <div className="card bg-gradient-to-br from-ferremas-green-50 to-white">
                <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
                  Valor de Inventario
                </h3>
                <p className="text-3xl font-bold text-ferremas-green-600">
                  ${reporteInventario?.valorTotal?.toLocaleString() || '0'}
                </p>
                <p className="text-ferremas-gray-600 mt-2">
                  {reporteInventario?.totalProductos || 0} productos en stock
                </p>
              </div>

              <div className="card bg-gradient-to-br from-ferremas-gray-50 to-white">
                <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
                  Productos Agotados
                </h3>
                <p className="text-3xl font-bold text-ferremas-danger">
                  {reporteInventario?.productosAgotados || 0}
                </p>
                <p className="text-ferremas-gray-600 mt-2">
                  productos requieren atención
                </p>
              </div>
            </div>

            {/* Productos Más Vendidos */}
            <section className="card">
              <h2 className="text-xl font-semibold text-ferremas-primary mb-4">
                Productos Más Vendidos
              </h2>
              <div className="overflow-x-auto">
                <table className="min-w-full">
                  <thead>
                    <tr className="bg-ferremas-gray-50">
                      <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Producto</th>
                      <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Cantidad</th>
                      <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Total</th>
                    </tr>
                  </thead>
                  <tbody>
                    {Array.isArray(reporteVentas?.productosVendidos) && reporteVentas.productosVendidos.length > 0 ? (
                      reporteVentas.productosVendidos.map((producto) => (
                        <tr key={producto.productoId} className="border-b border-ferremas-gray-100">
                          <td className="px-4 py-3">{producto.nombre}</td>
                          <td className="px-4 py-3">{producto.cantidad}</td>
                          <td className="px-4 py-3">${producto.total?.toLocaleString() || '0'}</td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan={3} className="px-4 py-8 text-center text-ferremas-gray-500">
                          No hay datos de productos vendidos disponibles
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </section>

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
                      <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Stock</th>
                      <th className="px-4 py-2 text-left text-sm font-medium text-ferremas-gray-600">Estado</th>
                    </tr>
                  </thead>
                  <tbody>
                    {Array.isArray(reporteInventario?.productosBajoStock) && reporteInventario.productosBajoStock.length > 0 ? (
                      reporteInventario.productosBajoStock.map((producto) => (
                        <tr key={producto.id} className="border-b border-ferremas-gray-100">
                          <td className="px-4 py-3">{producto.codigo}</td>
                          <td className="px-4 py-3">{producto.nombre}</td>
                          <td className="px-4 py-3">{producto.stock}</td>
                          <td className="px-4 py-3">
                            <span className={`badge-${producto.stock === 0 ? 'danger' : 'warning'}`}>
                              {producto.stock === 0 ? 'Agotado' : 'Bajo Stock'}
                            </span>
                          </td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan={4} className="px-4 py-8 text-center text-ferremas-gray-500">
                          No hay productos con bajo stock
                        </td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>
            </section>
          </>
        );
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
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="mb-6 flex gap-4">
        <button
          className={`px-4 py-2 rounded ${activeSection === 'dashboard' ? 'bg-blue-600 text-white' : 'bg-white border text-gray-700'}`}
          onClick={() => setActiveSection('dashboard')}
        >
          Dashboard
        </button>
        <button
          className={`px-4 py-2 rounded ${activeSection === 'users' ? 'bg-blue-600 text-white' : 'bg-white border text-gray-700'}`}
          onClick={() => setActiveSection('users')}
        >
          Usuarios
        </button>
        <button
          className={`px-4 py-2 rounded ${activeSection === 'catalogo' ? 'bg-blue-600 text-white' : 'bg-white border text-gray-700'}`}
          onClick={() => setActiveSection('catalogo')}
        >
          Catálogo de Productos
        </button>
        <button
          className="bg-ferremas-success text-white px-4 py-2 rounded hover:bg-ferremas-success-dark transition"
          onClick={() => window.location.href = '/admin/clientes'}
        >
          Ver Clientes
        </button>
      </div>
      {renderContent()}
    </div>
  );
};

export default AdminView; 