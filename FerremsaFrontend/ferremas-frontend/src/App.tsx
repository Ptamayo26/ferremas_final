// src/App.tsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { AppProvider } from './context/AppContext';

// Páginas de autenticación
import Login from './pages/auth/Login';

// Vistas principales
import CatalogoProductos from './views/CatalogoProductos';
import ProductoDetalle from './pages/shared/ProductoDetalle';

// Dashboards existentes por rol
import AdminView from './views/AdminView';
import VendedorView from './views/VendedorView';
import BodegueroView from './views/BodegueroView';
import ClienteView from './views/ClienteView';

// Componentes de prueba y diagnóstico
// import ColorTest from './components/ui/ColorTest';
// import ApiTest from './components/ui/ApiTest';
import DebugAuth from './components/ui/DebugAuth';
import SimpleCatalogo from './components/ui/SimpleCatalogo';
import Carrito from './components/sales/Carrito';
import ConfirmacionPago from './pages/shared/ConfirmacionPago';

// Componentes de layout
import Navigation from './components/layout/Navigation';

// importar la nueva vista
import ClientesAdmin from './views/ClientesAdmin';

// Componente de protección de rutas
const ProtectedRoute = ({ children, allowedRoles }: { children: React.ReactNode, allowedRoles?: string[] }) => {
  const { isAuthenticated, user, isLoading } = useAuth();
  
  if (isLoading) {
    return <div className="flex items-center justify-center min-h-screen">
      <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-ferremas-primary"></div>
    </div>;
  }
  
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  if (allowedRoles && user && !allowedRoles.includes(user.rol)) {
    return <Navigate to="/unauthorized" replace />;
  }
  
  return <>{children}</>;
};

const AppContent: React.FC = () => {
  return (
    <div className="flex flex-col min-h-screen bg-ferremas-background">
      {/* Forzar generación de clases de color de Ferremas */}
      <div className="hidden bg-ferremas-primary bg-ferremas-secondary bg-ferremas-accent bg-ferremas-success bg-ferremas-warning bg-ferremas-danger bg-ferremas-background bg-ferremas-surface text-ferremas-primary text-ferremas-secondary text-ferremas-accent text-ferremas-success text-ferremas-warning text-ferremas-danger text-ferremas-background text-ferremas-surface border-ferremas-gray-100 border-ferremas-gray-200 border-ferremas-gray-300 text-ferremas-gray-600 text-ferremas-gray-500 text-ferremas-gray-400 bg-ferremas-gray-100 bg-ferremas-gray-200 bg-ferremas-gray-300 bg-ferremas-gray-50 bg-ferremas-green-100 text-ferremas-green-800 bg-ferremas-orange-100 text-ferremas-orange-800"></div>
      <Navigation />
      
      <main className="flex-grow bg-gradient-to-b from-ferremas-background via-ferremas-surface to-ferremas-background">
        <Routes>
          {/* Rutas públicas */}
          <Route path="/" element={<Navigate to="/catalogo" replace />} />
          <Route path="/catalogo" element={<CatalogoProductos />} />
          <Route path="/producto/:id" element={<ProductoDetalle />} />
          <Route path="/login" element={<Login />} />
          
          {/* Rutas de prueba y diagnóstico */}
          {/* <Route path="/test-colors" element={<ColorTest />} /> */}
          {/* <Route path="/test-api" element={<ApiTest />} /> */}
          <Route path="/debug-auth" element={<DebugAuth />} />
          <Route path="/simple-catalogo" element={<SimpleCatalogo />} />
          
          {/* Rutas protegidas - Mi Cuenta (accesible para cualquier usuario autenticado) */}
          <Route path="/mi-cuenta" element={
            <ProtectedRoute allowedRoles={['cliente', 'CLIENT', 'administrador', 'ADMIN']}>
              <ClienteView />
            </ProtectedRoute>
          } />
          
          {/* Rutas protegidas - Vendedor */}
          <Route path="/vendedor/dashboard" element={
            <ProtectedRoute allowedRoles={['vendedor', 'SELLER']}>
              <VendedorView />
            </ProtectedRoute>
          } />
          
          {/* Rutas protegidas - Bodeguero */}
          <Route path="/bodeguero/dashboard" element={
            <ProtectedRoute allowedRoles={['bodeguero', 'WAREHOUSE']}>
              <BodegueroView />
            </ProtectedRoute>
          } />
          
          {/* Rutas protegidas - Administrador */}
          <Route path="/admin/dashboard" element={
            <ProtectedRoute allowedRoles={['administrador', 'ADMIN']}>
              <AdminView />
            </ProtectedRoute>
          } />
          
          <Route path="/admin/clientes" element={
            <ProtectedRoute allowedRoles={['administrador', 'ADMIN']}>
              <ClientesAdmin />
            </ProtectedRoute>
          } />
          
          {/* Rutas para otros roles */}
          <Route path="/contador/dashboard" element={
            <ProtectedRoute allowedRoles={['contador', 'ACCOUNTANT']}>
              <div className="max-w-7xl mx-auto px-4 py-8">
                <h1 className="text-3xl font-bold text-ferremas-primary">Panel de Contador</h1>
                <p className="text-ferremas-gray-600 mt-2">Funcionalidad en desarrollo...</p>
              </div>
            </ProtectedRoute>
          } />
          
          <Route path="/repartidor/dashboard" element={
            <ProtectedRoute allowedRoles={['repartidor', 'DELIVERY']}>
              <div className="max-w-7xl mx-auto px-4 py-8">
                <h1 className="text-3xl font-bold text-ferremas-primary">Panel de Repartidor</h1>
                <p className="text-ferremas-gray-600 mt-2">Funcionalidad en desarrollo...</p>
              </div>
            </ProtectedRoute>
          } />
          
          <Route path="/carrito" element={<Carrito isOpen={true} modoPagina={true} />} />
          
          <Route path="/confirmacion-pago" element={<ConfirmacionPago />} />
          
          {/* Ruta para usuarios no autorizados */}
          <Route path="/unauthorized" element={
            <div className="flex items-center justify-center min-h-screen">
              <div className="text-center">
                <h1 className="text-2xl font-bold text-ferremas-danger mb-4">Acceso No Autorizado</h1>
                <p className="text-ferremas-gray-600 mb-4">
                  No tienes permisos para acceder a esta página.
                </p>
                <button 
                  onClick={() => window.history.back()}
                  className="btn-primary"
                >
                  Volver
                </button>
              </div>
            </div>
          } />
          
          {/* Ruta 404 */}
          <Route path="*" element={
            <div className="flex items-center justify-center min-h-screen">
              <div className="text-center">
                <h1 className="text-2xl font-bold text-ferremas-primary mb-4">Página No Encontrada</h1>
                <p className="text-ferremas-gray-600 mb-4">
                  La página que buscas no existe.
                </p>
                <button 
                  onClick={() => window.location.href = '/catalogo'}
                  className="btn-primary"
                >
                  Ir al Catálogo
                </button>
              </div>
            </div>
          } />
        </Routes>
      </main>

      <footer className="bg-ferremas-surface py-4 mt-8 border-t border-ferremas-gray-100">
        <div className="container mx-auto px-6 text-center text-ferremas-gray-600">
          &copy; 2025 Ferremas. Todos los derechos reservados.
        </div>
      </footer>
    </div>
  );
};

function App() {
  return (
    <AuthProvider>
      <AppProvider>
        <Router>
          <AppContent />
        </Router>
      </AppProvider>
    </AuthProvider>
  );
}

export default App;