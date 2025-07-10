import React, { useEffect, useState } from 'react';
import { api } from '../../services/api';
import { checkoutService } from '../../services/checkout';
import { enviosService } from '../../services/envios';
import { API_BASE_URL } from '../../constants/api';
import type { CarritoItemDTO, CarritoResumenDTO } from '../../types/api';
import { useAuth } from '../../context/AuthContext';
import DireccionesModal from '../ui/DireccionesModal';
import chileGeoService from '../../services/chileGeo';
import { apiClient } from '../../services/api';

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
  const [perfil, setPerfil] = useState<any>(null);
  const [direcciones, setDirecciones] = useState<any[]>([]);
  const [direccionSeleccionada, setDireccionSeleccionada] = useState<any>(null);
  const [showDireccionesModal, setShowDireccionesModal] = useState(false);
  const [regiones, setRegiones] = useState<any[]>([]);
  const [comunas, setComunas] = useState<any[]>([]);
  const [region, setRegion] = useState('');
  const [comuna, setComuna] = useState('');

  // Obtener perfil completo al cargar
  useEffect(() => {
    const fetchPerfil = async () => {
      if (user?.id) {
        try {
          const res = await apiClient.get('/api/Clientes/mi-perfil');
          setPerfil(res.data);
          setDirecciones(res.data.direcciones || []);
          setRutEnvio(res.data.rut || '');
          setCorreoEnvio(res.data.correoElectronico || '');
          setNombreEnvio(res.data.nombre ? `${res.data.nombre} ${res.data.apellido || ''}` : '');
        } catch (e) {
          setPerfil(null);
          setDirecciones([]);
        }
      }
    };
    fetchPerfil();
  }, [user]);

  // Cargar regiones al montar
  useEffect(() => {
    chileGeoService.getRegiones().then(setRegiones);
  }, []);

  // Cargar comunas al cambiar región
  useEffect(() => {
    if (region) {
      chileGeoService.getComunasByRegion(region).then(setComunas);
    } else {
      setComunas([]);
    }
  }, [region]);

  // Cuando selecciona una dirección guardada
  useEffect(() => {
    if (direccionSeleccionada && direccionSeleccionada.id) {
      setDireccionEnvio(`${direccionSeleccionada.calle} ${direccionSeleccionada.numero}${direccionSeleccionada.departamento ? ', ' + direccionSeleccionada.departamento : ''}`);
      // Buscar el código de la región por nombre
      const regionEncontrada = regiones.find(r => r.nombre === direccionSeleccionada.region);
      setRegion(regionEncontrada?.codigo || direccionSeleccionada.region);
      setComuna(direccionSeleccionada.comuna);
      setCiudadEnvio(direccionSeleccionada.comuna || ''); // <--- Asegura que ciudadEnvio se setea
    }
  }, [direccionSeleccionada, regiones]);

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
        costoEnvio: 0,
        codigoDescuento: descuentoInfo ? codigoDescuento : undefined,
        items: items.map(item => ({
          productoId: item.productoId,
          precioConDescuento: item.precioConDescuento ?? 0,
          precioOriginal: item.precioOriginal ?? item.productoPrecio,
          cantidad: item.cantidad,
          // Campos extra para cumplir con el tipado del frontend
          id: item.id ?? 0,
          productoNombre: item.productoNombre ?? '',
          productoPrecio: item.productoPrecio ?? 0,
          subtotal: (item.precioConDescuento ?? item.productoPrecio) * item.cantidad,
          fechaAgregado: item.fechaAgregado ?? new Date()
        }))
      };
      if (!checkoutData.items || checkoutData.items.length === 0) {
        setErroresEnvio(['No hay productos en el carrito para procesar el pedido.']);
        return;
      }
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
  const totalConIVA = items.reduce((sum, item) => sum + ((item.precioConDescuento ?? item.productoPrecio) * item.cantidad), 0);
  const neto = Math.round(totalConIVA / 1.19);
  const iva = totalConIVA - neto;

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
    const subtotal = totalConIVA;
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
              <div className="flex gap-2">
                <select
                  className="input-field w-full"
                  value={direccionSeleccionada?.id || ''}
                  onChange={e => {
                    const id = e.target.value;
                    const dir = direcciones.find(d => d.id == id);
                    setDireccionSeleccionada(dir || null);
                  }}
                >
                  <option value="">Selecciona una dirección guardada</option>
                  {direcciones.map(dir => (
                    <option key={dir.id} value={dir.id}>
                      {dir.calle} {dir.numero}, {dir.comuna}, {dir.region}
                    </option>
                  ))}
                </select>
                <button type="button" className="btn-secondary" onClick={() => setShowDireccionesModal(true)}>
                  Añadir/Editar
                </button>
              </div>
              {showDireccionesModal && (
                <DireccionesModal
                  open={showDireccionesModal}
                  onClose={() => setShowDireccionesModal(false)}
                  direcciones={direcciones}
                  onUpdated={d => { setDirecciones(d); setShowDireccionesModal(false); }}
                  usuarioId={user?.rol === 'ADMIN' || user?.rol === 'VENDEDOR' ? perfil?.id : null}
                />
              )}
            </div>
            {/* Si no selecciona dirección, permitir ingresar manualmente */}
            {!direccionSeleccionada && (
              <>
                <div className="mb-2">
                  <input type="text" value={direccionEnvio} onChange={e => setDireccionEnvio(e.target.value)} className="input-field focus-ring w-full" placeholder="Calle y número" />
                </div>
                <div className="mb-2">
                  <label className="block text-sm font-medium text-gray-700 mb-1">Región</label>
                  <select
                    className="input-field w-full"
                    value={region}
                    onChange={e => setRegion(e.target.value)}
                  >
                    <option value="">Selecciona una región</option>
                    {regiones.map(r => (
                      <option key={r.codigo} value={r.codigo}>{r.nombre}</option>
                    ))}
                  </select>
                </div>
                <div className="mb-2">
                  <label className="block text-sm font-medium text-gray-700 mb-1">Comuna</label>
                  <select
                    className="input-field w-full"
                    value={comuna}
                    onChange={e => { setComuna(e.target.value); setCiudadEnvio(e.target.value); }}
                    disabled={!region}
                  >
                    <option value="">Selecciona una comuna</option>
                    {comunas.map(c => (
                      <option key={c.codigo} value={c.nombre}>{c.nombre}</option>
                    ))}
                  </select>
                </div>
              </>
            )}
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
              <span>Neto:</span>
              <span>${neto.toLocaleString('es-CL')}</span>
            </div>
            <div className="flex justify-between items-center mb-2">
              <span>IVA (19%):</span>
              <span>${iva.toLocaleString('es-CL')}</span>
            </div>
            {descuentoInfo && (
              <div className="flex justify-between items-center mb-2 text-green-600">
                <span>Descuento aplicado:</span>
                <span>- ${descuentoReal.toLocaleString('es-CL')}</span>
              </div>
            )}
            <div className="flex justify-between items-center font-bold text-lg">
              <span>Total:</span>
              <span>${(totalConIVA - descuentoReal).toLocaleString('es-CL')}</span>
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