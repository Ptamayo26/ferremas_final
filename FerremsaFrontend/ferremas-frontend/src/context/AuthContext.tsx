import React, { createContext, useState, useContext, useEffect, type ReactNode } from 'react';
import { authService } from '../services/auth';
import type { LoginDTO, UsuarioCreateDTO, AuthResponse } from '../types/api';
import type { Usuario } from '../types/user';

interface AuthContextType {
  isAuthenticated: boolean;
  user: Usuario | null;
  token: string | null;
  isLoading: boolean;
  login: (data: LoginDTO) => Promise<AuthResponse>;
  logout: () => void;
  register: (data: UsuarioCreateDTO) => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<Usuario | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const initAuth = () => {
      const storedToken = authService.getToken();
      const storedUser = authService.getCurrentUser();
      if (storedToken && storedUser) {
        setToken(storedToken);
        const appUser: Usuario = {
            id: storedUser.id,
            nombre: storedUser.nombre,
            email: storedUser.email,
            rol: storedUser.rol.toString()
        };
        setUser(appUser);
        setIsAuthenticated(true);
      } else {
        setUser(null);
        setToken(null);
        setIsAuthenticated(false);
      }
      setIsLoading(false);
    };
    initAuth();
  }, []);

  const login = async (data: LoginDTO): Promise<AuthResponse> => {
    const response = await authService.login(data);
    if (response.exito && response.token && response.usuario) {
        setToken(response.token);
        const appUser: Usuario = {
            id: response.usuario.id,
            nombre: response.usuario.nombre,
            email: response.usuario.email,
            rol: response.usuario.rol.toString()
        };
        setUser(appUser);
        setIsAuthenticated(true);

        // --- MIGRAR CARRITO ANÓNIMO AL AUTENTICADO ---
        try {
          const LOCAL_STORAGE_KEY = 'carrito_anonimo';
          const anonCart = localStorage.getItem(LOCAL_STORAGE_KEY);
          console.log('Contenido de carrito_anonimo antes de migrar:', anonCart);
          if (anonCart) {
            const items = JSON.parse(anonCart);
            if (Array.isArray(items) && items.length > 0) {
              // Importar api dinámicamente para evitar import circular
              const { api } = await import('../services/api');
              for (const item of items) {
                // Usar productoId si existe, si no usar id (pero solo si es válido)
                const productoId = item.productoId || item.id;
                if (productoId && item.cantidad > 0) {
                  try {
                    console.log('Intentando migrar producto:', item);
                    await api.agregarAlCarrito(productoId, item.cantidad);
                    console.log('Producto migrado correctamente:', productoId);
                  } catch (e) {
                    // Ignorar errores individuales (por ejemplo, stock insuficiente)
                    console.error('Error migrando producto:', item, e);
                  }
                } else {
                  console.warn('Item de carrito anónimo ignorado por falta de productoId:', item);
                }
              }
              localStorage.removeItem(LOCAL_STORAGE_KEY);
              // window.location.reload(); // Eliminado para evitar recarga
            } else {
              localStorage.removeItem(LOCAL_STORAGE_KEY);
            }
          }
        } catch (e) {
          console.error('Error migrando carrito anónimo:', e);
        }
        // --- FIN MIGRACIÓN CARRITO ---
    } else {
        throw new Error(response.mensaje || 'Error al iniciar sesión');
    }
    return response;
  };

  const logout = () => {
    authService.logout();
    setUser(null);
    setToken(null);
    setIsAuthenticated(false);
  };

  const register = async (data: UsuarioCreateDTO) => {
    await authService.register(data);
    // Opcional: iniciar sesión automáticamente después del registro
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, user, token, isLoading, login, logout, register }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth debe ser usado dentro de un AuthProvider');
  }
  return context;
};
