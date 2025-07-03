import React, { useEffect, useState } from 'react';
import { api } from '../../services/api';
import { checkoutService } from '../../services/checkout';
import { enviosService } from '../../services/envios';
import { API_BASE_URL } from '../../constants/api';
import type { CarritoItemDTO, CarritoResumenDTO } from '../../types/api';
import { useAuth } from '../../context/AuthContext';

const CarritoAutenticado: React.FC = () => {
  const { user, isAuthenticated } = useAuth();
  const [items, setItems] = useState<CarritoItemDTO[]>([]);
  const [resumen, setResumen] = useState<CarritoResumenDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pagarLoading, setPagarLoading] = useState(false);
  // Formulario de envío
  const [rutEnvio, setRutEnvio] = useState('');
  const [correoEnvio, setCorreoEnvio] = useState('');
  const [nombreEnvio, setNombreEnvio] = useState('');
  const [direccionEnvio, setDireccionEnvio] = useState('');
  const [ciudadEnvio, setCiudadEnvio] = useState('');
  const [metodoEnvio, setMetodoEnvio] = useState('');
  const [costoEnvio, setCostoEnvio] = useState(0);
  const [erroresEnvio, setErroresEnvio] = useState<string[]>([]);
  // Estado para descuento
  const [codigoDescuento, setCodigoDescuento] = useState('');
  const [descuentoInfo, setDescuentoInfo] = useState<{tipo: string, valor: number} | null>(null);
  const [descuentoError, setDescuentoError] = useState<string | null>(null);

  useEffect(() => {
    const fetchCarrito = async () => {
      try {
        setLoading(true);
        const itemsData = await api.getCarrito();
        const resumenData = await api.getCarritoResumen?.();
        setItems(itemsData || []);
        setResumen(resumenData || null);
      } catch (err) {
        setError('No se pudo cargar el carrito autenticado');
      } finally {
        setLoading(false);
      }
    };
    fetchCarrito();
  }, []);

  useEffect(() => {
    const costo = enviosService.obtenerCostoEnvio(metodoEnvio);
    setCostoEnvio(costo);
  }, [metodoEnvio]);

  const handleActualizarCantidad = async (item: CarritoItemDTO, nuevaCantidad: number) => {
    try {
      await api.actualizarCantidadCarrito(item.id, nuevaCantidad);
      const itemsData = await api.getCarrito();
      setItems(itemsData || []);
      const resumenData = await api.getCarritoResumen?.();
      setResumen(resumenData || null);
    } catch (err) {
      setError('No se pudo actualizar la cantidad');
    }
  };

  const handleEliminar = async (item: CarritoItemDTO) => {
    try {
      await api.eliminarItemCarrito(item.id);
      const itemsData = await api.getCarrito();
      setItems(itemsData || []);
      const resumenData = await api.getCarritoResumen?.();
      setResumen(resumenData || null);
    } catch (err) {
      setError('No se pudo eliminar el producto');
    }
  };

  const handleLimpiarCarrito = async () => {
    try {
      await api.limpiarCarrito();
      setItems([]);
      setResumen(null);
    } catch (err) {
      setError('No se pudo limpiar el carrito');
    }
  };

  const handlePagar = async () => {
    const nuevosErrores: string[] = [];
    if (!rutEnvio.trim()) nuevosErrores.push('El RUT es obligatorio.');
    if (!correoEnvio.trim()) nuevosErrores.push('El correo es obligatorio.');
    setErroresEnvio(nuevosErrores);
    if (nuevosErrores.length > 0) return;
    const monto = resumen?.total ?? 0;
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
        metodoEnvio: metodoEnvio,
        costoEnvio: costoEnvio
      };
      const response = await checkoutService.procesarCheckout(checkoutData);
      
      // ✅ USAR DIRECTAMENTE LA RESPUESTA DEL CHECKOUT
      if (response.urlPago && response.codigoPago) {
        console.log('Redirigiendo a Webpay:', response.urlPago);
        window.location.href = `${response.urlPago}?token_ws=${response.codigoPago}`;
      } else {
        setError('No se pudo obtener la URL de pago de Webpay.');
      }
    } catch (err: any) {
      const errorMessage = err.response?.data || 'No se pudo iniciar el pago con Webpay.';
      setError(errorMessage);
    } finally {
      setPagarLoading(false);
    }
  };

  // Calcular subtotal, IVA y total usando precioConDescuento
  const subtotalConDescuento = items.reduce((sum, item) => sum + ((item.precioConDescuento ?? item.productoPrecio) * item.cantidad), 0);
  const ivaConDescuento = Math.round(subtotalConDescuento * 0.19);
  const totalConDescuento = subtotalConDescuento + ivaConDescuento;

  // Validar el cupón contra el backend
  const validarDescuento = async () => {
    setDescuentoError(null);
    setDescuentoInfo(null);
    if (!codigoDescuento) {
      setDescuentoError('Ingrese un código de descuento');
      return;
    }
    try {
      // En autenticado, enviar productos del carrito
      const productosParaValidar = items.map(item => ({
        id: item.id,
        productoId: item.productoId,
        productoNombre: item.productoNombre,
        productoPrecio: item.productoPrecio,
        cantidad: item.cantidad
      }));
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
    const subtotal = subtotalConDescuento;
    if (descuentoInfo.tipo === 'porcentaje') {
      return Math.round(subtotal * (descuentoInfo.valor / 100));
    } else if (descuentoInfo.tipo === 'monto') {
      return Math.min(descuentoInfo.valor, subtotal);
    }
    return 0;
  };
  const descuentoReal = calcularDescuentoReal();

  if (loading) return <div>Cargando carrito autenticado...</div>;
  return (
    <div className="max-w-4xl mx-auto my-8 bg-white rounded-lg p-6 shadow-md">
      <div className="mb-6">
        <h2 className="text-2xl font-bold text-ferremas-primary text-center w-full">
          🛒 Carrito de Compras
        </h2>
      </div>
      {error && <div className="text-red-500 text-center mb-4">{error}</div>}
      {items.length === 0 ? (
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
          <div className="space-y-4 mb-6">
            {items.map((item, idx) => (
              <div key={item.id} className="flex items-center p-4 border rounded-lg bg-gray-50 border-gray-300">
                <img
                  src={item.productoImagen && !item.productoImagen.startsWith('http') ? `${API_BASE_URL}${item.productoImagen}` : item.productoImagen || '/placeholder.png'}
                  alt={item.productoNombre}
                  className="w-16 h-16 object-cover rounded mr-4 border"
                />
                <div className="flex-grow">
                  <h3 className="font-semibold text-gray-900">{item.productoNombre}</h3>
                  <p className="text-gray-600">
                    ${item.precioConDescuento?.toLocaleString('es-CL') ?? item.productoPrecio?.toLocaleString('es-CL')}
                    {item.precioConDescuento && item.precioConDescuento < item.productoPrecio && (
                      <span className="line-through text-red-400 ml-2 text-sm">${item.productoPrecio?.toLocaleString('es-CL')}</span>
                    )}
                  </p>
                  <div className="flex items-center mt-1">
                    <button
                      className="px-2 py-1 bg-gray-200 rounded-l hover:bg-gray-300"
                      onClick={() => handleActualizarCantidad(item, Math.max(1, item.cantidad - 1))}
                      disabled={item.cantidad <= 1}
                    >-</button>
                    <input
                      type="number"
                      min={1}
                      value={item.cantidad}
                      onChange={e => {
                        const val = Math.max(1, parseInt(e.target.value) || 1);
                        handleActualizarCantidad(item, val);
                      }}
                      className="w-12 text-center border mx-1 rounded"
                    />
                    <button
                      className="px-2 py-1 bg-gray-200 rounded-r hover:bg-gray-300"
                      onClick={() => handleActualizarCantidad(item, item.cantidad + 1)}
                    >+</button>
                  </div>
                  <span className="text-sm text-gray-500">Cantidad: {item.cantidad}</span>
                </div>
                <div className="text-right ml-4">
                  <p className="font-semibold">${((item.precioConDescuento ?? item.productoPrecio) * item.cantidad)?.toLocaleString('es-CL')}</p>
                  <button
                    className="ml-2 text-red-500 hover:text-red-700 text-xs underline"
                    onClick={() => handleEliminar(item)}
                  >Eliminar</button>
                </div>
              </div>
            ))}
          </div>
          {/* Campo para código de descuento */}
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
            {erroresEnvio.length > 0 && (
              <div className="text-red-500 text-sm mt-2">{erroresEnvio.join(' | ')}</div>
            )}
          </div>
          {/* Desglose de totales MOVIDO ABAJO DEL FORM */}
          <div className="border-t pt-4 mb-6">
            <div className="flex justify-between items-center mb-2">
              <span>Subtotal (Neto):</span>
              <span>${subtotalConDescuento.toLocaleString('es-CL')}</span>
            </div>
            {descuentoInfo && (
              <div className="flex justify-between items-center mb-2 text-green-600">
                <span>Descuento aplicado:</span>
                <span>- ${descuentoReal.toLocaleString('es-CL')}</span>
              </div>
            )}
            <div className="flex justify-between items-center mb-2">
              <span>IVA (19%):</span>
              <span>${ivaConDescuento.toLocaleString('es-CL')}</span>
            </div>
            <div className="flex justify-between items-center font-bold text-lg">
              <span>Total:</span>
              <span>${(totalConDescuento - descuentoReal).toLocaleString('es-CL')}</span>
            </div>
          </div>
          {/* Acciones */}
          <div className="flex justify-between items-center mb-4">
            <button onClick={handleLimpiarCarrito} className="text-sm text-red-500 hover:underline">Vaciar carrito</button>
          </div>
          <button
            className="btn-primary w-full mt-2"
            onClick={handlePagar}
            disabled={pagarLoading}
          >
            {pagarLoading ? 'Redirigiendo...' : 'Pagar con Webpay'}
          </button>
        </>
      )}
    </div>
  );
};

export default CarritoAutenticado; 