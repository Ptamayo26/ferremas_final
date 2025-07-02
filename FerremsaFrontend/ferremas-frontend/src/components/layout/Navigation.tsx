import React from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { getRoleDisplayName, isWorkerRole } from '../../utils/roleRedirect';
import CarritoButton from '../ui/CarritoButton';
import { FaBars } from 'react-icons/fa';
import { FiLogOut } from 'react-icons/fi';

const Navigation: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const location = useLocation();
  const [carritoAbierto, setCarritoAbierto] = React.useState(false);
  const [menuCategoriasAbierto, setMenuCategoriasAbierto] = React.useState(false);
  const navigate = useNavigate();

  const isActive = (path: string) => {
    return location.pathname === path;
  };

  const getDashboardLink = () => {
    if (!user) return '/login';
    
    const role = user.rol.toLowerCase();
    if (role.includes('admin') || role.includes('administrador')) {
      return '/admin/dashboard';
    } else if (role.includes('vendedor') || role.includes('seller')) {
      return '/vendedor/dashboard';
    } else if (role.includes('bodeguero') || role.includes('warehouse')) {
      return '/bodeguero/dashboard';
    } else if (role.includes('contador') || role.includes('accountant')) {
      return '/contador/dashboard';
    } else if (role.includes('repartidor') || role.includes('delivery')) {
      return '/repartidor/dashboard';
    } else {
      return '/mi-cuenta'; // Cliente por defecto
    }
  };

  const categoriasNavbar = [
    'Herramientas Eléctricas',
    'Herramientas Manuales',
    'Jardinería',
    'Construcción',
    'Electricidad',
    'Plomería',
    'Pintura y Acabados',
    'Seguridad Industrial',
  ];

  return (
    <header className="bg-ferremas-primary shadow-md">
      <nav className="container mx-auto px-2 py-2">
        <div className="flex items-center justify-between gap-2 w-full">
          {/* Logo, nombre y botón de categorías */}
          <div className="flex items-center gap-2 min-w-max">
            <Link to="/catalogo" className="text-2xl font-extrabold text-ferremas-secondary hover:text-ferremas-accent transition-colors tracking-wide leading-tight">
              Ferremas
            </Link>
            <button
              className="ml-2 flex items-center gap-1 bg-ferremas-accent text-white rounded-full w-8 h-8 justify-center focus:outline-none relative"
              onClick={() => setMenuCategoriasAbierto((v) => !v)}
              onBlur={() => setTimeout(() => setMenuCategoriasAbierto(false), 150)}
            >
              <FaBars size={18} />
              {/* Menú desplegable de categorías */}
              {menuCategoriasAbierto && (
                <div className="absolute left-0 top-10 z-50 bg-white shadow-lg rounded-lg py-2 w-48 min-w-max">
                  {categoriasNavbar.map((cat) => (
                    <button
                      key={cat}
                      className="block w-full text-left px-4 py-2 text-ferremas-primary hover:bg-ferremas-accent hover:text-white transition-colors text-sm"
                      onClick={() => {
                        navigate(`/catalogo?categoria=${encodeURIComponent(cat)}`);
                        setMenuCategoriasAbierto(false);
                      }}
                    >
                      {cat}
                    </button>
                  ))}
                </div>
              )}
            </button>
            <span className="ml-2 text-base font-semibold text-ferremas-secondary hidden sm:inline">Categorías</span>
          </div>

          {/* Buscador centrado */}
          <div className="flex-1 flex justify-center">
            <input
              type="text"
              placeholder="Buscar productos, marcas..."
              className="input-field w-full max-w-lg h-9 text-base px-3 py-1"
            />
          </div>

          {/* Acciones de usuario y carrito a la derecha */}
          <div className="flex items-center gap-3 min-w-max">
            {isAuthenticated && user && (
              <>
                <Link
                  to="/mi-cuenta"
                  className="btn-primary text-sm px-3 py-1 h-8"
                >
                  Mi Perfil
                </Link>
                <button
                  onClick={logout}
                  className="ml-2 flex items-center gap-1 px-3 py-1 h-8 rounded bg-ferremas-danger text-white hover:bg-ferremas-danger/80 transition-colors text-sm"
                  title="Cerrar sesión"
                >
                  <FiLogOut size={18} />
                  Cerrar sesión
                </button>
              </>
            )}
            {!isAuthenticated && (
              <Link 
                to="/login" 
                className="btn-primary text-sm px-3 py-1 h-8"
              >
                Iniciar Sesión
              </Link>
            )}
            <CarritoButton onClick={() => navigate('/carrito')} />
          </div>
        </div>

        {/* Indicador de rol para trabajadores */}
        {isAuthenticated && user && isWorkerRole(user.rol) && (
          <div className="mt-2 pt-2 border-t border-ferremas-gray-100">
            <div className="flex items-center space-x-4 text-xs text-ferremas-gray-200">
              <span className="flex items-center">
                <span className="w-2 h-2 bg-ferremas-success rounded-full mr-2"></span>
                Panel de Trabajador
              </span>
              <span>•</span>
              <span>{getRoleDisplayName(user.rol)}</span>
              <span>•</span>
              <span>Ferremas Staff</span>
            </div>
          </div>
        )}
      </nav>
    </header>
  );
};

export default Navigation; 