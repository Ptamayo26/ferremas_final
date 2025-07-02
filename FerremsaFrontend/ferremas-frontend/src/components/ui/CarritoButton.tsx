import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { apiClient } from '../../services/api';
import type { CarritoResumenDTO } from '../../types/api';

interface CarritoButtonProps {
  onClick: () => void;
  className?: string;
}

const CarritoButton: React.FC<CarritoButtonProps> = ({ onClick, className = '' }) => {
  const { isAuthenticated } = useAuth();
  const [itemCount, setItemCount] = useState(0);
  const [loading, setLoading] = useState(false);

  const fetchCarritoResumen = async () => {
    if (!isAuthenticated) {
      setItemCount(0);
      return;
    }

    try {
      setLoading(true);
      const response = await apiClient.get<CarritoResumenDTO>('/api/Carrito/resumen');
      setItemCount(response.data.totalItems || 0);
    } catch (err) {
      console.error('Error al cargar resumen del carrito:', err);
      setItemCount(0);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCarritoResumen();
    
    // Solo actualizar si está autenticado
    if (isAuthenticated) {
      const interval = setInterval(fetchCarritoResumen, 30000);
      return () => clearInterval(interval);
    }
  }, [isAuthenticated]);

  const handleClick = () => {
    onClick();
  };

  return (
    <button
      onClick={handleClick}
      disabled={loading}
      className={`relative bg-ferremas-primary hover:bg-ferremas-primary-dark text-white px-4 py-2 rounded-lg flex items-center space-x-2 transition-colors duration-200 ${className}`}
      title="Ver carrito"
    >
      <span className="text-xl">🛒</span>
      <span className="font-medium">Carrito</span>
      
      {isAuthenticated && itemCount > 0 && (
        <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs font-bold rounded-full w-6 h-6 flex items-center justify-center">
          {itemCount > 99 ? '99+' : itemCount}
        </span>
      )}
      
      {loading && (
        <div className="absolute inset-0 bg-white bg-opacity-20 rounded-lg flex items-center justify-center">
          <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
        </div>
      )}
    </button>
  );
};

export default CarritoButton; 