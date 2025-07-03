import React, { useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api';
import type { AgregarAlCarritoDTO } from '../../types/api';
import Notification from './Notification';

interface AddToCartButtonProps {
  productoId: number;
  productoNombre: string;
  stockDisponible: number;
  className?: string;
  onSuccess?: () => void;
  precio?: number;
  imagenUrl?: string;
  cantidad?: number;
}

const LOCAL_STORAGE_KEY = 'carrito_anonimo';

const AddToCartButton: React.FC<AddToCartButtonProps> = ({ 
  productoId, 
  productoNombre, 
  stockDisponible, 
  className = '',
  onSuccess,
  precio = 0,
  imagenUrl = '',
  cantidad: cantidadProp,
}) => {
  const { isAuthenticated, user } = useAuth();
  const [cantidad, setCantidad] = useState(cantidadProp ?? 1);
  const [loading, setLoading] = useState(false);
  const [showQuantitySelector, setShowQuantitySelector] = useState(false);
  
  const [notification, setNotification] = useState<{
    isOpen: boolean;
    type: 'success' | 'error' | 'warning' | 'info';
    title: string;
    message?: string;
  }>({ isOpen: false, type: 'info', title: '' });

  const handleAddToCart = async () => {
    const cantidadFinal = cantidadProp ?? cantidad;
    if (cantidadFinal <= 0 || cantidadFinal > stockDisponible) {
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Cantidad inválida',
        message: `La cantidad debe estar entre 1 y ${stockDisponible}`
      });
      return;
    }

    if (!isAuthenticated || !user) {
      // Carrito anónimo en localStorage
      const data = localStorage.getItem(LOCAL_STORAGE_KEY);
      let carrito = data ? JSON.parse(data) : [];
      // Si ya existe el producto, suma la cantidad
      const idx = carrito.findIndex((item: any) => item.id === productoId);
      if (idx >= 0) {
        carrito[idx].cantidad += cantidadFinal;
        carrito[idx].precio = precio;
        carrito[idx].imagenUrl = imagenUrl;
      } else {
        carrito.push({ id: productoId, nombre: productoNombre, precio, cantidad: cantidadFinal, imagenUrl });
      }
      console.log('Guardando en localStorage carrito_anonimo:', carrito);
      localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(carrito));
      window.dispatchEvent(new Event('carrito_anonimo_actualizado'));
      setNotification({
        isOpen: true,
        type: 'success',
        title: 'Producto agregado',
        message: `${cantidadFinal} ${cantidadFinal === 1 ? 'unidad' : 'unidades'} de "${productoNombre}" agregada${cantidadFinal === 1 ? '' : 's'} al carrito`
      });
      setCantidad(cantidadProp ?? 1);
      setShowQuantitySelector(false);
      onSuccess?.();
      return;
    } else {
      // Carrito autenticado
      setLoading(true);
      await api.agregarAlCarrito(productoId, cantidadFinal);
      setNotification({
        isOpen: true,
        type: 'success',
        title: 'Producto agregado',
        message: `${cantidadFinal} ${cantidadFinal === 1 ? 'unidad' : 'unidades'} de "${productoNombre}" agregada${cantidadFinal === 1 ? '' : 's'} al carrito`
      });
      setCantidad(cantidadProp ?? 1);
      setShowQuantitySelector(false);
      onSuccess?.();
    }
  };

  const handleQuickAdd = () => {
    if (stockDisponible === 1) {
      setCantidad(1);
      handleAddToCart();
    } else {
      setShowQuantitySelector(true);
    }
  };

  if (stockDisponible <= 0) {
    return (
      <button
        disabled
        className={`btn-secondary opacity-50 cursor-not-allowed ${className}`}
        title="Sin stock disponible"
      >
        Sin Stock
      </button>
    );
  }

  if (showQuantitySelector) {
    return (
      <div className={`flex items-center space-x-2 ${className}`}>
        <div className="flex items-center border rounded-lg">
          <button
            onClick={() => setCantidad(Math.max(1, cantidad - 1))}
            disabled={cantidad <= 1}
            className="px-3 py-2 hover:bg-gray-100 disabled:opacity-50"
          >
            -
          </button>
          <span className="px-3 py-2 min-w-[3rem] text-center">
            {cantidad}
          </span>
          <button
            onClick={() => setCantidad(Math.min(stockDisponible, cantidad + 1))}
            disabled={cantidad >= stockDisponible}
            className="px-3 py-2 hover:bg-gray-100 disabled:opacity-50"
          >
            +
          </button>
        </div>
        
        <button
          onClick={handleAddToCart}
          disabled={loading}
          className="btn-primary text-sm"
        >
          {loading ? 'Agregando...' : 'Agregar'}
        </button>
        
        <button
          onClick={() => setShowQuantitySelector(false)}
          disabled={loading}
          className="btn-secondary text-sm"
        >
          Cancelar
        </button>
      </div>
    );
  }

  return (
    <>
      <button
        onClick={handleQuickAdd}
        disabled={loading}
        className={`btn-primary ${className}`}
        title="Agregar al carrito"
      >
        {loading ? 'Agregando...' : '🛒 Agregar'}
      </button>

      <Notification
        isOpen={notification.isOpen}
        onClose={() => setNotification({ ...notification, isOpen: false })}
        type={notification.type}
        title={notification.title}
        message={notification.message}
      />
    </>
  );
};

export default AddToCartButton; 