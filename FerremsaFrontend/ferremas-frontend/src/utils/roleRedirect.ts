import { USER_ROLES } from '../constants/api';

export interface RoleRedirectConfig {
  [key: string]: string;
}

// Configuración de redirecciones por rol
export const ROLE_REDIRECTS: RoleRedirectConfig = {
  [USER_ROLES.ADMIN]: '/admin/dashboard',
  [USER_ROLES.SELLER]: '/vendedor/dashboard',
  [USER_ROLES.WAREHOUSE]: '/bodeguero/dashboard',
  [USER_ROLES.ACCOUNTANT]: '/contador/dashboard',
  [USER_ROLES.DELIVERY]: '/repartidor/dashboard',
  [USER_ROLES.CLIENT]: '/mi-cuenta',
  // Fallback para roles no reconocidos
  'default': '/catalogo'
};

/**
 * Obtiene la ruta de redirección basada en el rol del usuario
 * @param role - El rol del usuario
 * @returns La ruta a la que debe ser redirigido
 */
export const getRedirectPathByRole = (role: string): string => {
  // Normalizar el rol (convertir a minúsculas y quitar espacios)
  const normalizedRole = role.toLowerCase().trim();
  
  // Buscar la redirección específica para el rol
  const redirectPath = ROLE_REDIRECTS[normalizedRole];
  
  if (redirectPath) {
    return redirectPath;
  }
  
  // Si no se encuentra, verificar si es un rol de trabajador
  const workerRoles = [
    USER_ROLES.ADMIN,
    USER_ROLES.SELLER,
    USER_ROLES.WAREHOUSE,
    USER_ROLES.ACCOUNTANT,
    USER_ROLES.DELIVERY
  ];
  
  const isWorker = workerRoles.some(workerRole => 
    normalizedRole.includes(workerRole.toLowerCase())
  );
  
  if (isWorker) {
    // Si es un trabajador pero no tiene ruta específica, redirigir al dashboard de admin
    return '/admin/dashboard';
  }
  
  // Por defecto, redirigir al catálogo
  return ROLE_REDIRECTS.default || '/catalogo';
};

/**
 * Verifica si un usuario tiene permisos para acceder a una ruta específica
 * @param userRole - El rol del usuario
 * @param requiredRoles - Los roles requeridos para acceder
 * @returns true si el usuario tiene permisos
 */
export const hasRoleAccess = (userRole: string, requiredRoles: string[]): boolean => {
  const normalizedUserRole = userRole.toLowerCase().trim();
  
  return requiredRoles.some(requiredRole => 
    normalizedUserRole.includes(requiredRole.toLowerCase())
  );
};

/**
 * Obtiene el nombre legible del rol
 * @param role - El rol del usuario
 * @returns El nombre legible del rol
 */
export const getRoleDisplayName = (role: string): string => {
  const roleNames: { [key: string]: string } = {
    [USER_ROLES.ADMIN]: 'Administrador',
    [USER_ROLES.SELLER]: 'Vendedor',
    [USER_ROLES.CLIENT]: 'Cliente',
    [USER_ROLES.WAREHOUSE]: 'Bodeguero',
    [USER_ROLES.ACCOUNTANT]: 'Contador',
    [USER_ROLES.DELIVERY]: 'Repartidor'
  };
  
  const normalizedRole = role.toLowerCase().trim();
  return roleNames[normalizedRole] || role;
};

/**
 * Verifica si un rol es de trabajador (no cliente)
 * @param role - El rol del usuario
 * @returns true si es un trabajador
 */
export const isWorkerRole = (role: string): boolean => {
  const workerRoles = [
    USER_ROLES.ADMIN,
    USER_ROLES.SELLER,
    USER_ROLES.WAREHOUSE,
    USER_ROLES.ACCOUNTANT,
    USER_ROLES.DELIVERY
  ];
  
  const normalizedRole = role.toLowerCase().trim();
  
  return workerRoles.some(workerRole => 
    normalizedRole.includes(workerRole.toLowerCase())
  );
};

/**
 * Verifica si un rol es de cliente
 * @param role - El rol del usuario
 * @returns true si es un cliente
 */
export const isClientRole = (role: string): boolean => {
  const clientRoles = [USER_ROLES.CLIENT, 'cliente'];
  const normalizedRole = role.toLowerCase().trim();
  
  return clientRoles.some(clientRole => 
    normalizedRole.includes(clientRole.toLowerCase())
  );
}; 