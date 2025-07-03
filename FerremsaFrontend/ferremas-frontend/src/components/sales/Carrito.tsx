import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api';
import { enviosService } from '../../services/envios';
import type { CarritoItemDTO, CarritoResumenDTO } from '../../types/api';
import type { ProductoResponseDTO } from '../../types/api';
import ConfirmDialog from '../ui/ConfirmDialog';
import Notification from '../ui/Notification';
import { useNavigate } from 'react-router-dom';
import { checkoutService } from '../../services/checkout';
import { API_BASE_URL } from '../../constants/api';

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
  console.log('[DEBUG Carrito.tsx] isOpen:', isOpen, 'modoPagina:', modoPagina);

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
  const [descuentoInfo, setDescuentoInfo] = useState<{tipo: string, valor: number} | null>(null);
  const [descuentoError, setDescuentoError] = useState<string | null>(null);

  const [nombreEnvio, setNombreEnvio] = useState('');
  const [direccionEnvio, setDireccionEnvio] = useState('');
  const [ciudadEnvio, setCiudadEnvio] = useState('');
  const [metodoEnvio, setMetodoEnvio] = useState('');
  const [costoEnvio, setCostoEnvio] = useState(0);

  const [productos, setProductos] = useState<ProductoResponseDTO[]>([]);

  const navigate = useNavigate ? useNavigate() : null;

  // Determinar si se debe usar la API o localStorage
  const usarApi = isAuthenticated && user;

  // Agregar estados para RUT y Correo
  const [rutEnvio, setRutEnvio] = useState('');
  const [correoEnvio, setCorreoEnvio] = useState('');
  const [erroresEnvio, setErroresEnvio] = useState<string[]>([]);

  // Cargar carrito según autenticación
  useEffect(() => {
    // Determinar si se debe usar la API o localStorage
    const usarApi = isAuthenticated && user;
    
    if (usarApi) {
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

  // Calcular costo de envío cuando cambie el método
  useEffect(() => {
    const costo = enviosService.obtenerCostoEnvio(metodoEnvio);
    setCostoEnvio(costo);
  }, [metodoEnvio]);

  const fetchCarrito = async () => {
    if (!isAuthenticated) {
      setLoading(false);
      return;
    }
    try {
      setLoading(true);
      const itemsData = await api.getCarrito();
      console.log('API getCarrito:', itemsData);
      let resumenData = null;
      if (api.getCarritoResumen) {
        resumenData = await api.getCarritoResumen();
        console.log('API getCarritoResumen:', resumenData);
      }
      setItems(itemsData || []);
      setResumen(resumenData || null);
      setLoading(false);
    } catch (err) {
      setError('No se pudo cargar el carrito');
      console.error(err);
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

  // Validar el cupón contra el backend
  const validarDescuento = async () => {
    setDescuentoError(null);
    setDescuentoInfo(null);
    if (!codigoDescuento) {
      setDescuentoError('Ingrese un código de descuento');
      return;
    }
    try {
      // Siempre enviar productos del carrito, autenticado o no
      const productosParaValidar = isAuthenticated ? items.map(item => ({
        id: item.id,
        productoId: item.productoId,
        productoNombre: item.productoNombre,
        productoPrecio: item.productoPrecio,
        cantidad: item.cantidad
      })) : carrito;
      const data = await checkoutService.validarCodigoDescuentoAnonimo(codigoDescuento, productosParaValidar);
      setDescuentoInfo({ tipo: data.tipo, valor: data.valor });
    } catch (err) {
      setDescuentoError('Código de descuento inválido o expirado');
    }
  };

  const quitarDescuento = () => {
    setCodigoDescuento('');
    setDescuentoInfo(null);
    setDescuentoError(null);
  };

  // Calcular descuento real basado en el tipo de cupón
  const calcularDescuentoReal = () => {
    if (!descuentoInfo) return 0;
    
    const subtotal = isAuthenticated ? (resumen?.subtotal || 0) : (carrito.reduce((sum, item) => sum + (item.precio * item.cantidad), 0));
    
    if (descuentoInfo.tipo === 'porcentaje') {
      return Math.round(subtotal * (descuentoInfo.valor / 100));
    } else if (descuentoInfo.tipo === 'monto') {
      return Math.min(descuentoInfo.valor, subtotal);
    }
    return 0;
  };

  const descuentoReal = calcularDescuentoReal();

  // Modificar handlePagar para enviar el código de descuento real al backend
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
        ...(user?.id ? { clienteId: user.id } : {}),
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
        codigoDescuento: descuentoInfo ? codigoDescuento : undefined,
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
      if (response.urlPago && response.codigoPago) {
        window.location.href = `${response.urlPago}?token_ws=${response.codigoPago}`;
      } else {
        setNotification({
          isOpen: true,
          type: 'error',
          title: 'Error',
          message: 'No se pudo obtener la URL de pago de Webpay.'
        });
      }
    } catch (err: any) {
      const errorMessage = err.response?.data || 'No se pudo iniciar el pago con Webpay.';
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Error',
        message: errorMessage
      });
    } finally {
      setPagarLoading(false);
    }
  };

  // Lógica para costo de envío
  useEffect(() => {
    if (metodoEnvio === 'Chilexpress') setCostoEnvio(4990);
    else if (metodoEnvio === 'Starken') setCostoEnvio(3990);
    else setCostoEnvio(0);
  }, [metodoEnvio]);

  // Escuchar evento personalizado para recargar carrito anónimo
  useEffect(() => {
    if (!isAuthenticated) {
      const handler = () => {
        const data = localStorage.getItem(LOCAL_STORAGE_KEY);
        setCarrito(data ? JSON.parse(data) : []);
      };
      window.addEventListener('carrito_anonimo_actualizado', handler);
      return () => window.removeEventListener('carrito_anonimo_actualizado', handler);
    }
  }, [isAuthenticated]);

  // Depuración: mostrar el contenido del carrito local en consola
  useEffect(() => {
    if (!isAuthenticated) {
      console.log('Carrito local renderizado:', carrito);
    }
  }, [carrito, isAuthenticated]);

  // Cálculos para desglose de carrito anónimo
  const neto = Math.round(total / 1.19);
  const iva = total - neto;
  const totalConDescuento = total - descuentoReal;
  const netoConDescuento = Math.round(totalConDescuento / 1.19);
  const ivaConDescuento = totalConDescuento - netoConDescuento;

  // Cálculos para desglose de carrito autenticado (usando solo propiedades válidas)
  const resumenNeto = resumen?.subtotal ?? 0;
  const resumenTotal = resumen?.total ?? totalConDescuento;
  const resumenIva = resumen ? resumenTotal - resumenNeto : 0;

  // Mostrar desglose solo si hay productos en el carrito o en el resumen
  const hayProductos = isAuthenticated ? items.length > 0 : carrito.length > 0;

  // Depuración: mostrar el contenido de items antes de renderizar
  console.log('DEBUG: items a renderizar:', items);
  if (items && items.length > 0) {
    console.log('DEBUG: primer item:', items[0]);
  }

  // Depuración: mostrar el contenido de los estados clave antes del renderizado del desglose
  useEffect(() => {
    console.log('--- DEPURACIÓN CARRITO ---');
    console.log('carrito:', carrito);
    console.log('items:', items);
    console.log('resumen:', resumen);
    console.log('total:', total);
    console.log('neto:', neto);
    console.log('iva:', iva);
    console.log('totalConDescuento:', totalConDescuento);
    console.log('resumenNeto:', resumenNeto);
    console.log('resumenIva:', resumenIva);
    console.log('resumenTotal:', resumenTotal);
    console.log('--------------------------');
  }, [carrito, items, resumen, total, neto, iva, totalConDescuento, resumenNeto, resumenIva, resumenTotal]);

  // Log para depuración del valor total mostrado
  const totalMostrado = isAuthenticated ? resumenTotal : totalConDescuento;
  useEffect(() => {
    console.log('Valor totalMostrado:', totalMostrado);
    if (totalMostrado === 0) {
      console.warn('ADVERTENCIA: El total mostrado es 0, revisar cálculos y estado.');
    }
  }, [totalMostrado]);

  // Sincronizar props con estado local en modoPagina
  useEffect(() => {
    if (modoPagina && (typeof (window as any).carritoPagina !== 'undefined')) {
      // Si tienes una variable global temporal para depuración
      setItems((window as any).carritoPagina.items || []);
      setCarrito((window as any).carritoPagina.carrito || []);
    }
    // Si recibes por props, descomenta y usa esto:
    // if (modoPagina && props.items) setItems(props.items);
    // if (modoPagina && props.carrito) setCarrito(props.carrito);
  }, [modoPagina]);

  // Cálculo de subtotal y totales para autenticados usando precioConDescuento
  const subtotalAutenticado = items.reduce((sum, item) => sum + ((item.precioConDescuento ?? item.productoPrecio) * item.cantidad), 0);
  const ivaAutenticado = subtotalAutenticado * 0.19;
  const totalAutenticado = subtotalAutenticado + ivaAutenticado;

  if (loading) return <div>Cargando carrito...</div>;

  // Renderizado diferente según modoPagina
  if (modoPagina) {
    return (
      <div className="max-w-4xl mx-auto my-8 bg-white rounded-lg p-6 shadow-md">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-ferremas-primary text-center w-full">
            🛒 Carrito de Compras
          </h2>
        </div>
        {/* Campo para código de descuento SIEMPRE visible para ambos tipos de usuario */}
        <div className="mb-4">
          <label className="block text-sm font-medium text-gray-700 mb-1">Código de descuento</label>
          <div className="flex gap-2">
            <input
              type="text"
              className="input input-bordered w-48"
              value={codigoDescuento}
              onChange={e => setCodigoDescuento(e.target.value)}
              placeholder="Ingresa tu código"
              disabled={!!descuentoInfo}
            />
            {!descuentoInfo ? (
              <button
                className="btn btn-secondary"
                onClick={validarDescuento}
                type="button"
              >
                Validar
              </button>
            ) : (
              <button
                className="btn btn-danger"
                onClick={quitarDescuento}
                type="button"
              >
                Quitar
              </button>
            )}
          </div>
          {descuentoInfo && (
            <div className="text-green-600 text-sm mt-1">
              ¡{descuentoInfo.tipo === 'porcentaje' ? `${descuentoInfo.valor}% de descuento aplicado!` : `$${descuentoInfo.valor} de descuento aplicado!`}
              <br />
              <span className="font-bold">Descuento: -${descuentoReal.toLocaleString('es-CL')}</span>
            </div>
          )}
          {descuentoError && (
            <div className="text-red-500 text-sm mt-1">{descuentoError}</div>
          )}
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
        ) : (isAuthenticated ? items.length === 0 : carrito.length === 0) ? (
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
            {/* Depuración visual y lista de productos autenticados */}
            {items.length > 0 && (
              <>
                <div style={{ color: 'red', fontWeight: 'bold', fontSize: '18px' }}>
                  [DEBUG] Bloque de productos autenticados renderizado
                </div>
                <div className="space-y-4 mb-6">
                  {items.map(item => (
                    <div key={item.id} className="flex items-center p-4 border rounded-lg" style={{ background: '#ffeaea', border: '2px solid red' }}>
                      PROD
                      <img
                        src={item.productoImagen || '/placeholder.png'}
                        alt={item.productoNombre}
                        className="w-16 h-16 object-cover rounded mr-4"
                      />
                      <div className="flex-grow">
                        <h3 className="font-semibold text-gray-900">{item.productoNombre}</h3>
                        <p className="text-gray-600">${(item.precioConDescuento ?? item.productoPrecio)?.toLocaleString('es-CL')}</p>
                        <span className="text-sm text-gray-500">Cantidad: {item.cantidad}</span>
                      </div>
                      <div className="text-right ml-4">
                        <p className="font-semibold">${((item.precioConDescuento ?? item.productoPrecio) * item.cantidad)?.toLocaleString('es-CL')}</p>
                      </div>
                    </div>
                  ))}
                </div>
              </>
            )}
            {/* Renderizado de productos del carrito anónimo (no autenticado) */}
            {!isAuthenticated && carrito.length > 0 && (
              <>
                <div className="space-y-4 mb-6">
                  {carrito.map((item, idx) => (
                    <div key={item.id} className="flex items-center p-4 border rounded-lg bg-green-50 border-green-400">
                      <img
                        src={item.imagenUrl && !item.imagenUrl.startsWith('http') ? `${API_BASE_URL}${item.imagenUrl}` : item.imagenUrl || '/placeholder.png'}
                        alt={item.nombre}
                        className="w-16 h-16 object-cover rounded mr-4 border"
                      />
                      <div className="flex-grow">
                        <h3 className="font-semibold text-gray-900">{item.nombre}</h3>
                        <p className="text-gray-600">${item.precio?.toLocaleString('es-CL')}</p>
                        <div className="flex items-center mt-1">
                          <button
                            className="px-2 py-1 bg-gray-200 rounded-l hover:bg-gray-300"
                            onClick={() => {
                              const nuevaCantidad = Math.max(1, item.cantidad - 1);
                              setCarrito(prev => prev.map((it, i) => i === idx ? { ...it, cantidad: nuevaCantidad } : it));
                              localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(carrito.map((it, i) => i === idx ? { ...it, cantidad: nuevaCantidad } : it)));
                            }}
                            disabled={item.cantidad <= 1}
                          >-</button>
                          <input
                            type="number"
                            min={1}
                            value={item.cantidad}
                            onChange={e => {
                              const val = Math.max(1, parseInt(e.target.value) || 1);
                              setCarrito(prev => prev.map((it, i) => i === idx ? { ...it, cantidad: val } : it));
                              localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(carrito.map((it, i) => i === idx ? { ...it, cantidad: val } : it)));
                            }}
                            className="w-12 text-center border mx-1 rounded"
                          />
                          <button
                            className="px-2 py-1 bg-gray-200 rounded-r hover:bg-gray-300"
                            onClick={() => {
                              const nuevaCantidad = item.cantidad + 1;
                              setCarrito(prev => prev.map((it, i) => i === idx ? { ...it, cantidad: nuevaCantidad } : it));
                              localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(carrito.map((it, i) => i === idx ? { ...it, cantidad: nuevaCantidad } : it)));
                            }}
                          >+</button>
                        </div>
                        <span className="text-sm text-gray-500">Cantidad: {item.cantidad}</span>
                      </div>
                      <div className="text-right ml-4">
                        <p className="font-semibold">${(item.precio * item.cantidad)?.toLocaleString('es-CL')}</p>
                        <button
                          className="ml-2 text-red-500 hover:text-red-700 text-xs underline"
                          onClick={() => {
                            const nuevoCarrito = carrito.filter((_, i) => i !== idx);
                            setCarrito(nuevoCarrito);
                            localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(nuevoCarrito));
                          }}
                        >Eliminar</button>
                      </div>
                    </div>
                  ))}
                </div>
              </>
            )}

            {/* Desglose */}
            {false && !isAuthenticated && carrito.length > 0 && (
              <div className="border-t pt-4 mb-6">
                <div className="flex justify-between items-center mb-2">
                  <span>Subtotal (Neto):</span>
                  <span>${neto.toLocaleString('es-CL')}</span>
                </div>
                <div className="flex justify-between items-center mb-2">
                  <span>IVA (19%):</span>
                  <span>${iva.toLocaleString('es-CL')}</span>
                </div>
                <div className="flex justify-between items-center font-bold text-lg">
                  <span>Total:</span>
                  <span>${total.toLocaleString('es-CL')}</span>
                </div>
              </div>
            )}
            {false && isAuthenticated && items.length > 0 && resumen && (
              <div className="border-t pt-4 mb-6">
                <div className="flex justify-between items-center mb-2">
                  <span>Subtotal (Neto):</span>
                  <span>${resumen.subtotal.toLocaleString('es-CL')}</span>
                </div>
                <div className="flex justify-between items-center mb-2">
                  <span>IVA (19%):</span>
                  <span>${(resumen.total - resumen.subtotal).toLocaleString('es-CL')}</span>
                </div>
                <div className="flex justify-between items-center font-bold text-lg">
                  <span>Total:</span>
                  <span>${resumen.total.toLocaleString('es-CL')}</span>
                </div>
              </div>
            )}

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
                  <option value="Shipit">Shipit</option>
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
                {descuentoInfo && (
                  <div className="flex justify-between items-center mb-2 text-green-600">
                    <span>Descuento aplicado:</span>
                    <span>- ${descuentoReal.toLocaleString('es-CL')}</span>
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
                  <span>${(resumen.total - (descuentoInfo ? descuentoReal : 0) + costoEnvio).toLocaleString('es-CL') || '0'}</span>
                </div>
              </div>
            )}

            {/* Desglose para ambos tipos de usuario */}
            {hayProductos ? (
              <div className="border-t pt-4 mb-6">
                <div className="flex justify-between items-center mb-2">
                  <span>Subtotal (Neto):</span>
                  <span>${(isAuthenticated ? resumenNeto : (descuentoInfo ? netoConDescuento : neto)).toLocaleString('es-CL')}</span>
                </div>
                <div className="flex justify-between items-center mb-2">
                  <span>IVA (19%):</span>
                  <span>${(isAuthenticated ? resumenIva : (descuentoInfo ? ivaConDescuento : iva)).toLocaleString('es-CL')}</span>
                </div>
                {descuentoInfo && (
                  <div className="flex justify-between items-center mb-2 text-green-600">
                    <span>Descuento aplicado:</span>
                    <span>- ${descuentoReal.toLocaleString('es-CL')}</span>
                  </div>
                )}
                <div className="flex justify-between items-center font-bold text-lg">
                  <span>Total:</span>
                  <span>${(isAuthenticated ? resumenTotal : totalConDescuento).toLocaleString('es-CL')}</span>
                </div>
              </div>
            ) : (
              <div className="flex justify-between items-center font-bold text-lg border-t pt-4 mb-6">
                <span>Total:</span>
                <span>$0</span>
              </div>
            )}

            {/* Acciones */}
            <div className="flex justify-between items-center mb-4">
              <button onClick={isAuthenticated ? handleLimpiarCarrito : handleLimpiar} className="text-sm text-red-500 hover:underline">Vaciar carrito</button>
              <div className="text-xl font-bold">Total: ${(isAuthenticated ? resumenTotal : totalConDescuento).toLocaleString('es-CL')}</div>
            </div>
            {((isAuthenticated ? items.length > 0 : carrito.length > 0)) && (
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
              ? `¿Estás seguro de que quieres eliminar "${isAuthenticated ? (confirmDialog.item as CarritoItemDTO)?.productoNombre : (confirmDialog.item as CarritoItem)?.nombre}" del carrito?`
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
};

export default Carrito; 