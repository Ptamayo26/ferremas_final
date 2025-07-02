import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api';
import type { CarritoItemDTO, CarritoResumenDTO } from '../../types/api';
import type { ProductoResponseDTO } from '../../types/api';
import ConfirmDialog from '../ui/ConfirmDialog';
import Notification from '../ui/Notification';
import { useNavigate } from 'react-router-dom';
import { checkoutService } from '../../services/checkout';

interface CarritoItem {
  id: number;
  nombre: string;
  precio: number;
  cantidad: number;
  imagenUrl: string;
}

const LOCAL_STORAGE_KEY = 'carrito_anonimo';

interface CarritoProps {
  isOpen: boolean;
  onClose?: () => void;
  modoPagina?: boolean;
}

const Carrito: React.FC<CarritoProps> = ({ isOpen, onClose, modoPagina = false }) => {
  if (!isOpen && !modoPagina) return null;

  const { user, isAuthenticated } = useAuth();
  const [carrito, setCarrito] = useState<CarritoItem[]>([]);
  const [items, setItems] = useState<CarritoItemDTO[]>([]);
  const [resumen, setResumen] = useState<CarritoResumenDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const [updatingItem, setUpdatingItem] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [pagarLoading, setPagarLoading] = useState(false);
  
  // Estados para confirmación y notificaciones
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    item: CarritoItem | CarritoItemDTO | null;
    action: 'delete' | 'clear';
  }>({ isOpen: false, item: null, action: 'delete' });
  
  const [notification, setNotification] = useState<{
    isOpen: boolean;
    type: 'success' | 'error' | 'warning' | 'info';
    title: string;
    message?: string;
  }>({ isOpen: false, type: 'info', title: '' });

  // Estados para descuento y envío
  const [codigoDescuento, setCodigoDescuento] = useState('');
  const [descuentoAplicado, setDescuentoAplicado] = useState(false);
  const [mensajeDescuento, setMensajeDescuento] = useState('');
  const [montoDescuento, setMontoDescuento] = useState(0);

  const [nombreEnvio, setNombreEnvio] = useState('');
  const [direccionEnvio, setDireccionEnvio] = useState('');
  const [ciudadEnvio, setCiudadEnvio] = useState('');
  const [metodoEnvio, setMetodoEnvio] = useState('');
  const [costoEnvio, setCostoEnvio] = useState(0);

  const [productos, setProductos] = useState<ProductoResponseDTO[]>([]);

  const navigate = useNavigate ? useNavigate() : null;

  // Determinar si se debe usar la API o localStorage
  const usarApi = isAuthenticated && user && (user.rol.toLowerCase() === 'cliente' || user.rol.toUpperCase() === 'CLIENT');

  // Agregar estados para RUT y Correo
  const [rutEnvio, setRutEnvio] = useState('');
  const [correoEnvio, setCorreoEnvio] = useState('');
  const [erroresEnvio, setErroresEnvio] = useState<string[]>([]);

  // Cargar carrito según autenticación
  useEffect(() => {
    const esCliente = isAuthenticated && user && (user.rol.toLowerCase() === 'cliente' || user.rol.toUpperCase() === 'CLIENT');
    if (esCliente) {
      fetchCarrito();
    } else {
      // Cargar carrito de localStorage
      const data = localStorage.getItem(LOCAL_STORAGE_KEY);
      console.log('Leyendo de localStorage carrito_anonimo:', data);
      setCarrito(data ? JSON.parse(data) : []);
      setLoading(false);
    }
  }, [isAuthenticated, user]);

  useEffect(() => {
    // Obtener productos al cargar el carrito
    api.getProductos().then(setProductos);
  }, []);

  const fetchCarrito = async () => {
    if (!isAuthenticated) {
      // No mostrar error, solo usar el carrito anónimo
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      const itemsData = await api.getCarrito();
      // Si tienes un método para obtener el resumen, agrégalo aquí
      let resumenData: CarritoResumenDTO | null = null;
      if (api.getCarritoResumen) {
        resumenData = await api.getCarritoResumen();
      }
      setItems(itemsData || []);
      setResumen(resumenData || null);
    } catch (err) {
      setError('No se pudo cargar el carrito');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCantidad = (id: number, cantidad: number) => {
    setCarrito(prev => prev.map(item => item.id === id ? { ...item, cantidad: Math.max(1, cantidad) } : item));
  };

  const handleEliminar = (id: number) => {
    setCarrito(prev => prev.filter(item => item.id !== id));
  };

  const handleLimpiar = () => {
    setCarrito([]);
  };

  const total = carrito.reduce((acc, item) => acc + item.precio * item.cantidad, 0);

  const handleActualizarCantidad = async (item: CarritoItem | CarritoItemDTO, nuevaCantidad: number) => {
    if (!isAuthenticated) {
      setNotification({
        isOpen: true,
        type: 'warning',
        title: 'Inicia sesión',
        message: 'Debes iniciar sesión para modificar el carrito'
      });
      return;
    }

    try {
      setUpdatingItem(item.id);
      if (api.actualizarCantidadCarrito) {
        await api.actualizarCantidadCarrito(item.id, nuevaCantidad);
      } else {
        // Si no tienes el método, deberías implementarlo en api.ts
        throw new Error('Método actualizarCantidadCarrito no implementado');
      }
      await fetchCarrito();
      setNotification({
        isOpen: true,
        type: 'success',
        title: 'Cantidad actualizada',
        message: 'La cantidad ha sido actualizada correctamente.'
      });
    } catch (err: any) {
      const errorMessage = err.response?.data || 'Error al actualizar la cantidad';
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Error',
        message: errorMessage
      });
    } finally {
      setUpdatingItem(null);
    }
  };

  const handleEliminarItem = (item: CarritoItem | CarritoItemDTO) => {
    if (!isAuthenticated) {
      setNotification({
        isOpen: true,
        type: 'warning',
        title: 'Inicia sesión',
        message: 'Debes iniciar sesión para modificar el carrito'
      });
      return;
    }

    setConfirmDialog({
      isOpen: true,
      item: item,
      action: 'delete'
    });
  };

  const handleLimpiarCarrito = () => {
    if (!isAuthenticated) {
      setNotification({
        isOpen: true,
        type: 'warning',
        title: 'Inicia sesión',
        message: 'Debes iniciar sesión para modificar el carrito'
      });
      return;
    }

    setConfirmDialog({
      isOpen: true,
      item: null,
      action: 'clear'
    });
  };

  const confirmEliminarItem = async () => {
    const item = confirmDialog.item;
    if (!item) return;

    try {
      if (api.eliminarItemCarrito) {
        await api.eliminarItemCarrito(item.id);
      } else {
        // Si no tienes el método, deberías implementarlo en api.ts
        throw new Error('Método eliminarItemCarrito no implementado');
      }
      await fetchCarrito();
      setNotification({
        isOpen: true,
        type: 'success',
        title: 'Producto eliminado',
        message: 'El producto ha sido eliminado del carrito.'
      });
    } catch (err: any) {
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Error',
        message: 'No se pudo eliminar el producto del carrito.'
      });
    }
  };

  const confirmLimpiarCarrito = async () => {
    try {
      if (api.limpiarCarrito) {
        await api.limpiarCarrito();
      } else {
        // Si no tienes el método, deberías implementarlo en api.ts
        throw new Error('Método limpiarCarrito no implementado');
      }
      await fetchCarrito();
      setNotification({
        isOpen: true,
        type: 'success',
        title: 'Carrito limpiado',
        message: 'El carrito ha sido limpiado correctamente.'
      });
    } catch (err: any) {
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Error',
        message: 'No se pudo limpiar el carrito.'
      });
    }
  };

  const handleConfirmAction = () => {
    if (confirmDialog.action === 'delete') {
      confirmEliminarItem();
    } else if (confirmDialog.action === 'clear') {
      confirmLimpiarCarrito();
    }
  };

  const handlePagar = async () => {
    const nuevosErrores: string[] = [];
    if (!rutEnvio.trim()) nuevosErrores.push('El RUT es obligatorio.');
    if (!correoEnvio.trim()) nuevosErrores.push('El correo es obligatorio.');
    setErroresEnvio(nuevosErrores);
    if (nuevosErrores.length > 0) return;

    const monto = resumen?.total ?? total;
    setPagarLoading(true);
    try {
      const direccionManual = nombreEnvio || direccionEnvio || ciudadEnvio;
      const checkoutData = {
        clienteId: user?.id ?? 0,
        direccionId: direccionManual ? 0 : 1,
        metodoPago: 'WEBPAY',
        observaciones: '',
        calle: nombreEnvio,
        numero: direccionEnvio,
        departamento: '',
        comuna: ciudadEnvio,
        region: '',
        codigoPostal: '',
        rut: rutEnvio,
        correo: correoEnvio,
        ...(user?.id ? {} : {
          items: carrito.map(item => ({
            id: item.id,
            productoId: item.id,
            productoNombre: item.nombre,
            productoPrecio: item.precio,
            precioOriginal: item.precio,
            precioConDescuento: item.precio,
            productoImagen: item.imagenUrl,
            cantidad: item.cantidad,
            subtotal: item.precio * item.cantidad,
            fechaAgregado: new Date()
          }))
        })
      };
      console.log('Enviando checkoutData:', checkoutData);
      const response = await checkoutService.procesarCheckout(checkoutData);
      const buyOrder = `ORD-${response.pedidoId}`;
      const sessionId = user?.id ? String(user.id) : `anon-${Date.now()}`;
      const returnUrl = window.location.origin + '/confirmacion-pago';
      const resp = await api.crearTransaccionWebpay(monto, buyOrder, sessionId, returnUrl);
      if (resp.url && resp.token) {
        window.location.href = `${resp.url}?token_ws=${resp.token}`;
      } else {
        setNotification({
          isOpen: true,
          type: 'error',
          title: 'Error',
          message: 'No se pudo iniciar el pago con Webpay.'
        });
      }
    } catch (err) {
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Error',
        message: 'No se pudo iniciar el pago con Webpay.'
      });
    } finally {
      setPagarLoading(false);
    }
  };

  // Lógica para aplicar descuento
  const aplicarDescuento = () => {
    if (codigoDescuento.trim().toUpperCase() === 'OFERTA2X1') {
      // Buscar productos de las categorías elegibles
      const categoriasElegibles = ['Herramientas Manuales', 'Herramientas Eléctricas', 'Jardinería'];
      let descuento = 0;
      if (usarApi) {
        descuento = items
          .filter(item => {
            const prod = productos.find(p => p.id === item.productoId);
            return prod && categoriasElegibles.includes(prod.categoriaNombre || '');
          })
          .reduce((acc, item) => {
            const prod = productos.find(p => p.id === item.productoId);
            return acc + (prod ? prod.precio : item.productoPrecio) * item.cantidad;
          }, 0) * 0.5;
      } else {
        descuento = carrito
          .filter(item => {
            const prod = productos.find(p => p.id === item.id);
            return prod && categoriasElegibles.includes(prod.categoriaNombre || '');
          })
          .reduce((acc, item) => {
            const prod = productos.find(p => p.id === item.id);
            return acc + (prod ? prod.precio : item.precio) * item.cantidad;
          }, 0) * 0.5;
      }
      if (descuento > 0) {
        setDescuentoAplicado(true);
        setMontoDescuento(Math.round(descuento));
        setMensajeDescuento('¡Descuento aplicado correctamente!');
      } else {
        setDescuentoAplicado(false);
        setMontoDescuento(0);
        setMensajeDescuento('No hay productos elegibles para el descuento.');
      }
    } else {
      setDescuentoAplicado(false);
      setMontoDescuento(0);
      setMensajeDescuento('Código inválido.');
    }
  };

  // Lógica para costo de envío
  useEffect(() => {
    if (metodoEnvio === 'Chilexpress') setCostoEnvio(4990);
    else if (metodoEnvio === 'Starken') setCostoEnvio(3990);
    else setCostoEnvio(0);
  }, [metodoEnvio]);

  if (loading) return <div>Cargando carrito...</div>;

  // Renderizado diferente según modoPagina
  if (modoPagina) {
    return (
      <div className="max-w-4xl mx-auto my-8 bg-white rounded-lg p-6 shadow-md">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-ferremas-primary">
            🛒 Carrito de Compras
          </h2>
        </div>
        {/* El resto del contenido igual que antes, pero sin overlay ni botón de cerrar */}
        {error ? (
          <div className="text-center py-8">
            <p className="text-red-500">{error}</p>
            <button 
              onClick={fetchCarrito}
              className="btn-primary mt-4"
            >
              Reintentar
            </button>
          </div>
        ) : (usarApi ? items.length === 0 : carrito.length === 0) ? (
          <div className="text-center py-8">
            <p className="text-gray-500 mb-4">Tu carrito está vacío</p>
            <button 
              onClick={() => { window.location.href = '/catalogo'; }}
              className="btn-primary"
            >
              Continuar comprando
            </button>
          </div>
        ) : (
          <>
            {/* Lista de items */}
            <div className="space-y-4 mb-6">
              {(usarApi ? items : carrito).map(item => (
                <div key={item.id} className="flex items-center p-4 border rounded-lg">
                  <img
                    src={
                      usarApi
                        ? ((item as CarritoItemDTO).productoImagen || '/placeholder.png')
                        : ((item as CarritoItem).imagenUrl?.startsWith('http')
                            ? (item as CarritoItem).imagenUrl
                            : `http://localhost:5200${(item as CarritoItem).imagenUrl}`) || '/placeholder.png'
                    }
                    alt={usarApi ? (item as CarritoItemDTO).productoNombre : (item as CarritoItem).nombre}
                    className="w-16 h-16 object-cover rounded mr-4"
                  />
                  <div className="flex-grow">
                    <h3 className="font-semibold text-gray-900">{usarApi ? (item as CarritoItemDTO).productoNombre : (item as CarritoItem).nombre}</h3>
                    {usarApi && (item as CarritoItemDTO).precioConDescuento != null && (item as CarritoItemDTO).precioOriginal != null && (item as CarritoItemDTO).precioConDescuento < (item as CarritoItemDTO).precioOriginal ? (
                      <div className="text-gray-600">
                        <span className="text-gray-400 line-through text-sm mr-2">
                          ${(item as CarritoItemDTO).precioOriginal?.toLocaleString('es-CL')}
                        </span>
                        <span className="text-ferremas-primary font-semibold">
                          ${(item as CarritoItemDTO).precioConDescuento?.toLocaleString('es-CL')}
                        </span>
                      </div>
                    ) : (
                      <p className="text-gray-600">${(usarApi ? (item as CarritoItemDTO).productoPrecio : (item as CarritoItem).precio).toLocaleString('es-CL')}</p>
                    )}
                  </div>
                  <div className="flex items-center space-x-2">
                    {usarApi ? (
                      <>
                        <button
                          onClick={() => handleActualizarCantidad(item, item.cantidad - 1)}
                          disabled={updatingItem === item.id || item.cantidad <= 1}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                        >
                          -
                        </button>
                        <span className="w-12 text-center">{item.cantidad}</span>
                        <button
                          onClick={() => handleActualizarCantidad(item, item.cantidad + 1)}
                          disabled={updatingItem === item.id}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                        >
                          +
                        </button>
                      </>
                    ) : (
                      <>
                        <button
                          onClick={() => handleCantidad(item.id, item.cantidad - 1)}
                          disabled={item.cantidad <= 1}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                        >
                          -
                        </button>
                        <span className="w-12 text-center">{item.cantidad}</span>
                        <button
                          onClick={() => handleCantidad(item.id, item.cantidad + 1)}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                        >
                          +
                        </button>
                      </>
                    )}
                  </div>
                  <div className="text-right ml-4">
                    <p className="font-semibold">${(usarApi ? (item as CarritoItemDTO).subtotal : (item as CarritoItem).precio * (item as CarritoItem).cantidad).toLocaleString('es-CL')}</p>
                    <button
                      onClick={() => handleEliminarItem(item)}
                      className="text-red-500 hover:text-red-700 text-sm"
                    >
                      Eliminar
                    </button>
                  </div>
                </div>
              ))}
            </div>

            {/* Campo para código de descuento */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-1">Código de descuento</label>
              <div className="flex space-x-2">
                <input
                  type="text"
                  value={codigoDescuento}
                  onChange={e => setCodigoDescuento(e.target.value)}
                  placeholder="Ingresa tu código"
                  className="input-field focus-ring"
                />
                <button
                  onClick={aplicarDescuento}
                  className="bg-ferremas-primary text-white px-4 py-2 rounded hover:bg-ferremas-primary-dark"
                >
                  Aplicar
                </button>
              </div>
              {mensajeDescuento && (
                <div className={`mt-2 text-sm ${descuentoAplicado ? 'text-green-600' : 'text-red-500'}`}>{mensajeDescuento}</div>
              )}
            </div>

            {/* Formulario de envío */}
            <div className="mb-4 p-4 border rounded-lg bg-gray-50">
              <h4 className="font-bold mb-2">Datos de Envío</h4>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">RUT <span className="text-red-500">*</span></label>
                <input type="text" value={rutEnvio} onChange={e => setRutEnvio(e.target.value)} className="input-field focus-ring w-full" required />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Correo <span className="text-red-500">*</span></label>
                <input type="email" value={correoEnvio} onChange={e => setCorreoEnvio(e.target.value)} className="input-field focus-ring w-full" required />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Nombre completo</label>
                <input type="text" value={nombreEnvio} onChange={e => setNombreEnvio(e.target.value)} className="input-field focus-ring w-full" />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Dirección</label>
                <input type="text" value={direccionEnvio} onChange={e => setDireccionEnvio(e.target.value)} className="input-field focus-ring w-full" />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Ciudad</label>
                <input type="text" value={ciudadEnvio} onChange={e => setCiudadEnvio(e.target.value)} className="input-field focus-ring w-full" />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Método de envío</label>
                <select value={metodoEnvio} onChange={e => setMetodoEnvio(e.target.value)} className="input-field focus-ring w-full">
                  <option value="">Selecciona un método</option>
                  <option value="Chilexpress">Chilexpress</option>
                  <option value="Starken">Starken</option>
                </select>
              </div>
              {costoEnvio > 0 && (
                <div className="text-sm text-gray-700 mt-2">Costo de envío: <span className="font-bold">${costoEnvio.toLocaleString('es-CL')}</span></div>
              )}
            </div>

            {/* Resumen */}
            {resumen && (
              <div className="border-t pt-4 mb-6">
                <div className="flex justify-between items-center mb-2">
                  <span>Subtotal:</span>
                  <span>${resumen.subtotal?.toLocaleString('es-CL') || '0'}</span>
                </div>
                {descuentoAplicado && (
                  <div className="flex justify-between items-center mb-2 text-green-600">
                    <span>Descuento aplicado:</span>
                    <span>- ${montoDescuento.toLocaleString('es-CL')}</span>
                  </div>
                )}
                {costoEnvio > 0 && (
                  <div className="flex justify-between items-center mb-2 text-blue-600">
                    <span>Envío ({metodoEnvio}):</span>
                    <span>+ ${costoEnvio.toLocaleString('es-CL')}</span>
                  </div>
                )}
                <div className="flex justify-between items-center font-bold text-lg">
                  <span>Total:</span>
                  <span>${(resumen.total - (descuentoAplicado ? montoDescuento : 0) + costoEnvio).toLocaleString('es-CL') || '0'}</span>
                </div>
              </div>
            )}

            {/* Acciones */}
            <div className="flex justify-between items-center mb-4">
              <button onClick={isAuthenticated ? handleLimpiarCarrito : handleLimpiar} className="text-sm text-red-500 hover:underline">Vaciar carrito</button>
              <div className="text-xl font-bold">Total: ${isAuthenticated ? (resumen?.total || 0).toLocaleString() : total.toLocaleString()}</div>
            </div>
            {/* Solo mostrar resumen y costo de envío si autenticado */}
            {isAuthenticated && resumen && (
              <div className="mb-4 text-sm text-gray-600">
                <div>Total a pagar: <span className="font-bold text-lg">${resumen.total?.toLocaleString('es-CL') ?? 0}</span></div>
              </div>
            )}
            {((usarApi ? items.length > 0 : carrito.length > 0)) && (
              <button
                className="btn-primary w-full mt-2"
                onClick={handlePagar}
                disabled={pagarLoading}
              >
                {pagarLoading ? 'Redirigiendo...' : 'Pagar con Webpay'}
              </button>
            )}
          </>
        )}
        {/* Diálogo de confirmación */}
        <ConfirmDialog
          isOpen={confirmDialog.isOpen}
          onClose={() => setConfirmDialog({ ...confirmDialog, isOpen: false })}
          onConfirm={handleConfirmAction}
          title={
            confirmDialog.action === 'delete' 
              ? 'Eliminar producto' 
              : 'Limpiar carrito'
          }
          message={
            confirmDialog.action === 'delete'
              ? `¿Estás seguro de que quieres eliminar "${usarApi ? (confirmDialog.item as CarritoItemDTO)?.productoNombre : (confirmDialog.item as CarritoItem)?.nombre}" del carrito?`
              : '¿Estás seguro de que quieres limpiar todo el carrito?'
          }
        />
        {/* Notificaciones */}
        <Notification
          isOpen={notification.isOpen}
          onClose={() => setNotification({ ...notification, isOpen: false })}
          type={notification.type}
          title={notification.title}
          message={notification.message}
        />
      </div>
    );
  }

  // Modal (comportamiento anterior)
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-4xl mx-4 max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-ferremas-primary">
            🛒 Carrito de Compras
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ✕
          </button>
        </div>
        {error ? (
          <div className="text-center py-8">
            <p className="text-red-500">{error}</p>
            <button 
              onClick={fetchCarrito}
              className="btn-primary mt-4"
            >
              Reintentar
            </button>
          </div>
        ) : (usarApi ? items.length === 0 : carrito.length === 0) ? (
          <div className="text-center py-8">
            <p className="text-gray-500 mb-4">Tu carrito está vacío</p>
            <button 
              onClick={() => {}}
              className="btn-primary"
            >
              Continuar comprando
            </button>
          </div>
        ) : (
          <>
            {/* Lista de items */}
            <div className="space-y-4 mb-6">
              {(usarApi ? items : carrito).map(item => (
                <div key={item.id} className="flex items-center p-4 border rounded-lg">
                  <img
                    src={
                      usarApi
                        ? ((item as CarritoItemDTO).productoImagen || '/placeholder.png')
                        : ((item as CarritoItem).imagenUrl?.startsWith('http')
                            ? (item as CarritoItem).imagenUrl
                            : `http://localhost:5200${(item as CarritoItem).imagenUrl}`) || '/placeholder.png'
                    }
                    alt={usarApi ? (item as CarritoItemDTO).productoNombre : (item as CarritoItem).nombre}
                    className="w-16 h-16 object-cover rounded mr-4"
                  />
                  <div className="flex-grow">
                    <h3 className="font-semibold text-gray-900">{usarApi ? (item as CarritoItemDTO).productoNombre : (item as CarritoItem).nombre}</h3>
                    {usarApi && (item as CarritoItemDTO).precioConDescuento != null && (item as CarritoItemDTO).precioOriginal != null && (item as CarritoItemDTO).precioConDescuento < (item as CarritoItemDTO).precioOriginal ? (
                      <div className="text-gray-600">
                        <span className="text-gray-400 line-through text-sm mr-2">
                          ${(item as CarritoItemDTO).precioOriginal?.toLocaleString('es-CL')}
                        </span>
                        <span className="text-ferremas-primary font-semibold">
                          ${(item as CarritoItemDTO).precioConDescuento?.toLocaleString('es-CL')}
                        </span>
                      </div>
                    ) : (
                      <p className="text-gray-600">${(usarApi ? (item as CarritoItemDTO).productoPrecio : (item as CarritoItem).precio).toLocaleString('es-CL')}</p>
                    )}
                  </div>
                  <div className="flex items-center space-x-2">
                    {usarApi ? (
                      <>
                        <button
                          onClick={() => handleActualizarCantidad(item, item.cantidad - 1)}
                          disabled={updatingItem === item.id || item.cantidad <= 1}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                        >
                          -
                        </button>
                        <span className="w-12 text-center">{item.cantidad}</span>
                        <button
                          onClick={() => handleActualizarCantidad(item, item.cantidad + 1)}
                          disabled={updatingItem === item.id}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                        >
                          +
                        </button>
                      </>
                    ) : (
                      <>
                        <button
                          onClick={() => handleCantidad(item.id, item.cantidad - 1)}
                          disabled={item.cantidad <= 1}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                        >
                          -
                        </button>
                        <span className="w-12 text-center">{item.cantidad}</span>
                        <button
                          onClick={() => handleCantidad(item.id, item.cantidad + 1)}
                          className="px-2 py-1 bg-gray-200 rounded hover:bg-gray-300 disabled:opacity-50"
                        >
                          +
                        </button>
                      </>
                    )}
                  </div>
                  <div className="text-right ml-4">
                    <p className="font-semibold">${(usarApi ? (item as CarritoItemDTO).subtotal : (item as CarritoItem).precio * (item as CarritoItem).cantidad).toLocaleString('es-CL')}</p>
                    <button
                      onClick={() => handleEliminarItem(item)}
                      className="text-red-500 hover:text-red-700 text-sm"
                    >
                      Eliminar
                    </button>
                  </div>
                </div>
              ))}
            </div>

            {/* Campo para código de descuento */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-1">Código de descuento</label>
              <div className="flex space-x-2">
                <input
                  type="text"
                  value={codigoDescuento}
                  onChange={e => setCodigoDescuento(e.target.value)}
                  placeholder="Ingresa tu código"
                  className="input-field focus-ring"
                />
                <button
                  onClick={aplicarDescuento}
                  className="bg-ferremas-primary text-white px-4 py-2 rounded hover:bg-ferremas-primary-dark"
                >
                  Aplicar
                </button>
              </div>
              {mensajeDescuento && (
                <div className={`mt-2 text-sm ${descuentoAplicado ? 'text-green-600' : 'text-red-500'}`}>{mensajeDescuento}</div>
              )}
            </div>

            {/* Formulario de envío */}
            <div className="mb-4 p-4 border rounded-lg bg-gray-50">
              <h4 className="font-bold mb-2">Datos de Envío</h4>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">RUT <span className="text-red-500">*</span></label>
                <input type="text" value={rutEnvio} onChange={e => setRutEnvio(e.target.value)} className="input-field focus-ring w-full" required />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Correo <span className="text-red-500">*</span></label>
                <input type="email" value={correoEnvio} onChange={e => setCorreoEnvio(e.target.value)} className="input-field focus-ring w-full" required />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Nombre completo</label>
                <input type="text" value={nombreEnvio} onChange={e => setNombreEnvio(e.target.value)} className="input-field focus-ring w-full" />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Dirección</label>
                <input type="text" value={direccionEnvio} onChange={e => setDireccionEnvio(e.target.value)} className="input-field focus-ring w-full" />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Ciudad</label>
                <input type="text" value={ciudadEnvio} onChange={e => setCiudadEnvio(e.target.value)} className="input-field focus-ring w-full" />
              </div>
              <div className="mb-2">
                <label className="block text-sm font-medium text-gray-700 mb-1">Método de envío</label>
                <select value={metodoEnvio} onChange={e => setMetodoEnvio(e.target.value)} className="input-field focus-ring w-full">
                  <option value="">Selecciona un método</option>
                  <option value="Chilexpress">Chilexpress</option>
                  <option value="Starken">Starken</option>
                </select>
              </div>
              {costoEnvio > 0 && (
                <div className="text-sm text-gray-700 mt-2">Costo de envío: <span className="font-bold">${costoEnvio.toLocaleString('es-CL')}</span></div>
              )}
            </div>

            {/* Resumen */}
            {resumen && (
              <div className="border-t pt-4 mb-6">
                <div className="flex justify-between items-center mb-2">
                  <span>Subtotal:</span>
                  <span>${resumen.subtotal?.toLocaleString('es-CL') || '0'}</span>
                </div>
                {descuentoAplicado && (
                  <div className="flex justify-between items-center mb-2 text-green-600">
                    <span>Descuento aplicado:</span>
                    <span>- ${montoDescuento.toLocaleString('es-CL')}</span>
                  </div>
                )}
                {costoEnvio > 0 && (
                  <div className="flex justify-between items-center mb-2 text-blue-600">
                    <span>Envío ({metodoEnvio}):</span>
                    <span>+ ${costoEnvio.toLocaleString('es-CL')}</span>
                  </div>
                )}
                <div className="flex justify-between items-center font-bold text-lg">
                  <span>Total:</span>
                  <span>${(resumen.total - (descuentoAplicado ? montoDescuento : 0) + costoEnvio).toLocaleString('es-CL') || '0'}</span>
                </div>
              </div>
            )}

            {/* Acciones */}
            <div className="flex justify-between items-center mb-4">
              <button onClick={isAuthenticated ? handleLimpiarCarrito : handleLimpiar} className="text-sm text-red-500 hover:underline">Vaciar carrito</button>
              <div className="text-xl font-bold">Total: ${isAuthenticated ? (resumen?.total || 0).toLocaleString() : total.toLocaleString()}</div>
            </div>
            {/* Solo mostrar resumen y costo de envío si autenticado */}
            {isAuthenticated && resumen && (
              <div className="mb-4 text-sm text-gray-600">
                <div>Total a pagar: <span className="font-bold text-lg">${resumen.total?.toLocaleString('es-CL') ?? 0}</span></div>
              </div>
            )}
            {((usarApi ? items.length > 0 : carrito.length > 0)) && (
              <button
                className="btn-primary w-full mt-2"
                onClick={handlePagar}
                disabled={pagarLoading}
              >
                {pagarLoading ? 'Redirigiendo...' : 'Pagar con Webpay'}
              </button>
            )}
          </>
        )}
        {/* Diálogo de confirmación */}
        <ConfirmDialog
          isOpen={confirmDialog.isOpen}
          onClose={() => setConfirmDialog({ ...confirmDialog, isOpen: false })}
          onConfirm={handleConfirmAction}
          title={
            confirmDialog.action === 'delete' 
              ? 'Eliminar producto' 
              : 'Limpiar carrito'
          }
          message={
            confirmDialog.action === 'delete'
              ? `¿Estás seguro de que quieres eliminar "${usarApi ? (confirmDialog.item as CarritoItemDTO)?.productoNombre : (confirmDialog.item as CarritoItem)?.nombre}" del carrito?`
              : '¿Estás seguro de que quieres limpiar todo el carrito?'
          }
        />
        {/* Notificaciones */}
        <Notification
          isOpen={notification.isOpen}
          onClose={() => setNotification({ ...notification, isOpen: false })}
          type={notification.type}
          title={notification.title}
          message={notification.message}
        />
      </div>
    </div>
  );
};

export default Carrito; 