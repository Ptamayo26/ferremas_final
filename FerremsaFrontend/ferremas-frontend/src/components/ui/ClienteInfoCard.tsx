import React from 'react';
import { useAuth } from '../../context/AuthContext';

interface ClienteInfoCardProps {
  className?: string;
}

const ClienteInfoCard: React.FC<ClienteInfoCardProps> = ({ className = '' }) => {
  const { user } = useAuth();

  const getRoleSpecificInfo = () => {
    if (!user) return null;

    const role = user.rol.toLowerCase();
    
    if (role.includes('admin') || role.includes('administrador')) {
      return {
        title: 'Administrador + Cliente',
        description: 'Tienes acceso completo al sistema administrativo y todas las funcionalidades de cliente.',
        features: [
          'Gestión completa de usuarios y productos',
          'Reportes y estadísticas del sistema',
          'Todas las funcionalidades de cliente',
          'Acceso a todos los dashboards'
        ]
      };
    } else if (role.includes('vendedor') || role.includes('seller')) {
      return {
        title: 'Vendedor + Cliente',
        description: 'Puedes gestionar ventas y también realizar compras como cliente.',
        features: [
          'Gestión de clientes y ventas',
          'Procesamiento de pedidos',
          'Todas las funcionalidades de cliente',
          'Acceso al catálogo completo'
        ]
      };
    } else if (role.includes('bodeguero') || role.includes('warehouse')) {
      return {
        title: 'Bodeguero + Cliente',
        description: 'Gestionas el inventario y puedes realizar compras como cliente.',
        features: [
          'Gestión de inventario y stock',
          'Control de productos',
          'Todas las funcionalidades de cliente',
          'Acceso al catálogo completo'
        ]
      };
    } else if (role.includes('contador') || role.includes('accountant')) {
      return {
        title: 'Contador + Cliente',
        description: 'Acceso a funciones contables y todas las funcionalidades de cliente.',
        features: [
          'Reportes financieros',
          'Gestión de facturas',
          'Todas las funcionalidades de cliente',
          'Acceso al catálogo completo'
        ]
      };
    } else if (role.includes('repartidor') || role.includes('delivery')) {
      return {
        title: 'Repartidor + Cliente',
        description: 'Gestionas entregas y puedes realizar compras como cliente.',
        features: [
          'Gestión de entregas',
          'Seguimiento de pedidos',
          'Todas las funcionalidades de cliente',
          'Acceso al catálogo completo'
        ]
      };
    } else {
      return {
        title: 'Cliente',
        description: 'Acceso completo a todas las funcionalidades de cliente.',
        features: [
          'Ver y gestionar pedidos',
          'Agregar productos al carrito',
          'Marcar productos como favoritos',
          'Realizar compras',
          'Ver historial de compras'
        ]
      };
    }
  };

  const roleInfo = getRoleSpecificInfo();

  if (!roleInfo) return null;

  return (
    <div className={`bg-gradient-to-br from-ferremas-blue-50 to-white border border-ferremas-blue-200 rounded-lg p-6 ${className}`}>
      <div className="flex items-start justify-between mb-4">
        <div>
          <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
            👤 {roleInfo.title}
          </h3>
          <p className="text-ferremas-gray-700 text-sm">
            {roleInfo.description}
          </p>
        </div>
        <div className="text-right">
          <span className="inline-block bg-ferremas-primary text-white text-xs px-2 py-1 rounded-full">
            {user?.rol || 'Usuario'}
          </span>
        </div>
      </div>
      
      <div className="space-y-2">
        <h4 className="font-medium text-ferremas-primary text-sm">Funcionalidades disponibles:</h4>
        <ul className="space-y-1">
          {roleInfo.features.map((feature, index) => (
            <li key={index} className="flex items-center text-sm text-ferremas-gray-600">
              <span className="text-ferremas-success mr-2">✓</span>
              {feature}
            </li>
          ))}
        </ul>
      </div>
      
      <div className="mt-4 pt-4 border-t border-ferremas-blue-200">
        <p className="text-xs text-ferremas-gray-500">
          💡 <strong>Nota:</strong> Cualquier usuario autenticado puede acceder a funcionalidades de cliente, 
          independientemente de su rol en el sistema.
        </p>
      </div>
    </div>
  );
};

export default ClienteInfoCard; 