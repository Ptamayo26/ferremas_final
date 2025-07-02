import React from 'react';
import { useAuth } from '../../context/AuthContext';

const SimpleCatalogo: React.FC = () => {
  const { isAuthenticated, user } = useAuth();

  // Datos de prueba
  const productosPrueba = [
    {
      id: 1,
      nombre: "Martillo Profesional",
      descripcion: "Martillo de alta calidad para trabajos profesionales",
      precio: 25000,
      stock: 15,
      categoriaNombre: "Herramientas Manuales"
    },
    {
      id: 2,
      nombre: "Taladro Eléctrico",
      descripcion: "Taladro potente para uso doméstico y profesional",
      precio: 45000,
      stock: 8,
      categoriaNombre: "Herramientas Eléctricas"
    },
    {
      id: 3,
      nombre: "Destornillador Phillips",
      descripcion: "Destornillador de precisión con punta Phillips",
      precio: 5000,
      stock: 25,
      categoriaNombre: "Herramientas Manuales"
    }
  ];

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-ferremas-primary">Catálogo Simple (Prueba)</h1>
        <p className="text-ferremas-gray-600 mt-2">
          Estado de autenticación: {isAuthenticated ? 'Autenticado' : 'No autenticado'}
          {user && ` - Usuario: ${user.nombre}`}
        </p>
      </div>

      {/* Productos */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {productosPrueba.map(producto => (
          <div key={producto.id} className="bg-white rounded-lg shadow-md p-6 border border-ferremas-gray-200">
            <h3 className="text-lg font-semibold text-ferremas-primary mb-2">{producto.nombre}</h3>
            <p className="text-ferremas-gray-600 text-sm mb-3">{producto.descripcion}</p>
            
            <div className="flex justify-between items-center mb-3">
              <span className="text-xl font-bold text-ferremas-secondary">
                ${producto.precio.toLocaleString('es-CL')}
              </span>
              <span className="text-sm text-ferremas-gray-500">
                Stock: {producto.stock}
              </span>
            </div>
            
            <div className="mb-3">
              <span className="inline-block bg-ferremas-primary text-white px-2 py-1 rounded text-xs">
                {producto.categoriaNombre}
              </span>
            </div>
            
            <button 
              className="w-full btn-primary"
              onClick={() => alert(`Agregando ${producto.nombre} al carrito`)}
            >
              🛒 Agregar al Carrito
            </button>
          </div>
        ))}
      </div>

      {/* Información de estado */}
      <div className="mt-8 bg-blue-50 p-4 rounded-lg">
        <h3 className="font-semibold text-blue-800 mb-2">Información de Estado:</h3>
        <div className="text-sm space-y-1">
          <p><strong>isAuthenticated:</strong> {isAuthenticated.toString()}</p>
          <p><strong>Usuario:</strong> {user ? user.nombre : 'No hay usuario'}</p>
          <p><strong>Email:</strong> {user ? user.email : 'No hay email'}</p>
          <p><strong>Rol:</strong> {user ? user.rol : 'No hay rol'}</p>
        </div>
      </div>
    </div>
  );
};

export default SimpleCatalogo; 