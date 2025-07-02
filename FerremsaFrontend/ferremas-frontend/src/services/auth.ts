// src/services/auth.ts - Servicio de autenticación corregido
import axios from 'axios';
import type { 
  LoginDTO, 
  AuthResponse, 
  UsuarioCreateDTO, 
  ResetPasswordDTO,
  UsuarioResponseDTO 
} from '../types/api';
import { API_BASE_URL, AUTH_ENDPOINTS, STORAGE_KEYS, USER_ROLES } from '../constants/api';

// Creamos un cliente axios específico para auth que no tendrá interceptores conflictivos
const authApiClient = axios.create({
    baseURL: API_BASE_URL,
});

// Servicio de autenticación que conecta directamente con tu AuthController
class AuthService {
  // Método para iniciar sesión (conecta con POST /api/auth/login)
  async login(credentials: LoginDTO): Promise<AuthResponse> {
    try {
      console.log('🔐 Iniciando sesión para:', credentials.email);
      
      // Usamos el cliente de auth específico
      const response = await authApiClient.post<AuthResponse>(
        AUTH_ENDPOINTS.LOGIN,
        credentials
      );

      console.log('📥 Respuesta del backend:', response.data);
      console.log('📥 Tipo de respuesta:', typeof response.data);
      console.log('📥 response.exito:', response.data.exito);
      console.log('📥 response.mensaje:', response.data.mensaje);

      // Si el login es exitoso, guardar el token y la información del usuario
      if (response.data.exito && response.data.token) {
        this.handleSuccessfulLogin(response.data);
        console.log('✅ Login exitoso para:', credentials.email);
      }

      return response.data;
    } catch (error) {
      console.error('❌ Error en login:', error);
      throw this.handleAuthError(error);
    }
  }

  // Método para registrar nuevos usuarios (conecta con POST /api/auth/register)
  async register(userData: UsuarioCreateDTO): Promise<AuthResponse> {
    try {
      console.log('👤 Registrando nuevo usuario:', userData.email);
      
      const response = await authApiClient.post<AuthResponse>(
        AUTH_ENDPOINTS.REGISTER,
        userData
      );

      // Si el registro incluye auto-login, manejar el token
      if (response.data.exito && response.data.token) {
        this.handleSuccessfulLogin(response.data);
      }

      return response.data;
    } catch (error) {
      console.error('❌ Error en registro:', error);
      throw this.handleAuthError(error);
    }
  }

  // Método para reset de contraseña (conecta con POST /api/auth/reset-password)
  async resetPassword(resetData: ResetPasswordDTO): Promise<AuthResponse> {
    try {
      console.log('🔄 Solicitando reset de contraseña para:', resetData.email);
      
      const response = await authApiClient.post<AuthResponse>(
        AUTH_ENDPOINTS.RESET_PASSWORD,
        resetData
      );

      return response.data;
    } catch (error) {
      console.error('❌ Error en reset de contraseña:', error);
      throw this.handleAuthError(error);
    }
  }

  // Método para cerrar sesión
  logout(): void {
    console.log('👋 Cerrando sesión');
    
    // Limpiar todos los datos de autenticación
    localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.USER_DATA);
    localStorage.removeItem(STORAGE_KEYS.USER_ROLE);
    
    console.log('✅ Sesión cerrada exitosamente');
  }

  // Verificar si el usuario está autenticado
  isAuthenticated(): boolean {
    const token = this.getToken();
    const user = this.getCurrentUser();
    
    // Verificar que tanto el token como el usuario existan
    const isAuth = !!(token && user);
    
    if (isAuth) {
      // Verificar si el token no ha expirado (opcional, depende de tu implementación)
      if (this.isTokenExpired(token)) {
        console.log('⚠️ Token expirado, cerrando sesión');
        this.logout();
        return false;
      }
    }
    
    return isAuth;
  }

  // Obtener el usuario actual desde localStorage
  getCurrentUser(): UsuarioResponseDTO | null {
    try {
      const userStr = localStorage.getItem(STORAGE_KEYS.USER_DATA);
      return userStr ? JSON.parse(userStr) : null;
    } catch (error) {
      console.error('❌ Error al obtener usuario actual:', error);
      return null;
    }
  }

  // Obtener el token actual
  getToken(): string | null {
    return localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
  }

  // Obtener el rol del usuario actual
  getCurrentUserRole(): string | null {
    const user = this.getCurrentUser();
    return user?.rol || null;
  }

  // Verificar si el usuario tiene un rol específico
  hasRole(role: string): boolean {
    const currentRole = this.getCurrentUserRole();
    return currentRole === role;
  }

  // Verificar si el usuario tiene alguno de los roles especificados
  hasAnyRole(roles: string[]): boolean {
    const currentRole = this.getCurrentUserRole();
    return currentRole ? roles.includes(currentRole) : false;
  }

  // Métodos privados para manejo interno

  private handleSuccessfulLogin(response: AuthResponse): void {
    // Guardar token en localStorage
    if (response.token) {
      localStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, response.token);
    }

    // Guardar información del usuario
    if (response.usuario) {
      localStorage.setItem(STORAGE_KEYS.USER_DATA, JSON.stringify(response.usuario));
      localStorage.setItem(STORAGE_KEYS.USER_ROLE, response.usuario.rol);
    }

    console.log('💾 Datos de autenticación guardados exitosamente');
  }

  private handleAuthError(error: any): Error {
    // Normalizar errores de autenticación para manejo consistente en los componentes
    let message = 'Error de autenticación';
    
    if (error.message) {
      message = error.message;
    } else if (error.response?.data?.message) {
      message = error.response.data.message;
    }

    return new Error(message);
  }

  private isTokenExpired(token: string): boolean {
    try {
      const base64Url = token.split('.')[1];
      if (!base64Url) return true;
      
      const payload = JSON.parse(atob(base64Url));
      const currentTime = Date.now() / 1000;
      
      return payload.exp < currentTime;
    } catch (error) {
      console.error('❌ Error al verificar expiración del token:', error);
      return true; // Si hay error, considerar el token como expirado
    }
  }

  // Método para refrescar el token (si implementas refresh tokens en el futuro)
  async refreshToken(): Promise<boolean> {
    // Implementación futura para refresh tokens
    // Por ahora, simplemente redirigir al login si el token expira
    console.log('🔄 Función de refresh token no implementada aún');
    return false;
  }

  // Método para obtener el nombre de usuario para mostrar
  getUserDisplayName(): string {
    const user = this.getCurrentUser();
    return user ? user.nombre : 'Usuario';
  }

  // Método para verificar permisos específicos
  canAccess(permission: string): boolean {
    const userRole = this.getCurrentUserRole();
    if (!userRole) return false;

    const permissionMap: Record<string, string[]> = {
      productos: ['administrador', 'vendedor', 'bodeguero'],
      pedidos: ['administrador', 'vendedor'],
      reportes: ['administrador', 'contador'],
      usuarios: ['administrador'],
      envios: ['administrador', 'repartidor']
    };

    const allowedRoles = permissionMap[permission];
    return allowedRoles ? allowedRoles.includes(userRole) : false;
  }
}

// Exportar una instancia única del servicio de autenticación
export const authService = new AuthService();

// También exportar la clase para casos especiales
export { AuthService };