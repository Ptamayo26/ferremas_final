import React, { useState, useEffect } from 'react';
import { checkoutService } from '../../services/checkout';
import type { CheckoutResumenDTO, CheckoutRequestDTO, CheckoutResponseDTO, DireccionDTO } from '../../types/api';
import ConfirmDialog from '../ui/ConfirmDialog';
import Notification from '../ui/Notification';

interface CheckoutProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: (pedido: CheckoutResponseDTO) => void;
}

const Checkout: React.FC<CheckoutProps> = ({ isOpen, onClose, onSuccess }) => {
  const [resumen, setResumen] = useState<CheckoutResumenDTO | null>(null);
  const [loading, setLoading] = useState(true);
  const [processing, setProcessing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedDireccionId, setSelectedDireccionId] = useState<number | null>(null);
  const [metodoPago, setMetodoPago] = useState<string>('efectivo');
  const [observaciones, setObservaciones] = useState<string>('');
  
  // Estados para confirmación y notificaciones
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    title: string;
    message: string;
  }>({ isOpen: false, title: '', message: '' });
  
  const [notification, setNotification] = useState<{
    isOpen: boolean;
    type: 'success' | 'error' | 'warning' | 'info';
    title: string;
    message?: string;
  }>({ isOpen: false, type: 'info', title: '' });

  const fetchResumenCheckout = async () => {
    try {
      setLoading(true);
      const data = await checkoutService.getResumen();
      setResumen(data);
      
      // Seleccionar la dirección principal por defecto
      if (data.direccionEnvio.id) {
        setSelectedDireccionId(data.direccionEnvio.id);
      }
    } catch (err: any) {
      const errorMessage = err.response?.data || 'No se pudo cargar el resumen del checkout';
      setError(errorMessage);
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isOpen) {
      fetchResumenCheckout();
    }
  }, [isOpen]);

  const handleProcesarPedido = async () => {
    if (!selectedDireccionId) {
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Dirección requerida',
        message: 'Debe seleccionar una dirección de envío'
      });
      return;
    }

    if (!resumen) {
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Error',
        message: 'No se pudo cargar el resumen del pedido'
      });
      return;
    }

    setConfirmDialog({
      isOpen: true,
      title: 'Confirmar Pedido',
      message: `¿Está seguro de que desea procesar el pedido por $${resumen.total.toFixed(2)}?\n\nEsta acción no se puede deshacer.`
    });
  };

  const confirmProcesarPedido = async () => {
    if (!resumen || !selectedDireccionId) return;

    try {
      setProcessing(true);
      const checkoutData: CheckoutRequestDTO = {
        clienteId: resumen.cliente.id,
        direccionId: selectedDireccionId,
        metodoPago: metodoPago,
        observaciones: observaciones || undefined
      };

      const response = await checkoutService.procesarCheckout(checkoutData);
      
      setNotification({
        isOpen: true,
        type: 'success',
        title: 'Pedido Procesado',
        message: `Pedido ${response.numeroPedido} creado exitosamente`
      });

      onSuccess(response);
      onClose();
    } catch (err: any) {
      const errorMessage = err.response?.data || 'Error al procesar el pedido';
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Error',
        message: errorMessage
      });
    } finally {
      setProcessing(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-4xl mx-4 max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-ferremas-primary">
            🛒 Finalizar Compra
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ✕
          </button>
        </div>

        {loading ? (
          <div className="text-center py-8">
            <p>Cargando resumen de compra...</p>
          </div>
        ) : error ? (
          <div className="text-center py-8">
            <p className="text-red-500 mb-4">{error}</p>
            <button 
              onClick={fetchResumenCheckout}
              className="btn-primary"
            >
              Reintentar
            </button>
          </div>
        ) : !resumen ? (
          <div className="text-center py-8">
            <p className="text-gray-500">No se pudo cargar el resumen</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Columna izquierda - Información del pedido */}
            <div className="space-y-6">
              {/* Información del cliente */}
              <div className="card">
                <h3 className="text-lg font-semibold mb-4">Información del Cliente</h3>
                <div className="space-y-2">
                  <p><strong>Nombre:</strong> {resumen.cliente.nombre} {resumen.cliente.apellido}</p>
                  <p><strong>RUT:</strong> {resumen.cliente.rut}</p>
                  <p><strong>Email:</strong> {resumen.cliente.email}</p>
                  {resumen.cliente.telefono && (
                    <p><strong>Teléfono:</strong> {resumen.cliente.telefono}</p>
                  )}
                </div>
              </div>

              {/* Dirección de envío */}
              <div className="card">
                <h3 className="text-lg font-semibold mb-4">Dirección de Envío</h3>
                {resumen.direccionEnvio.id ? (
                  <div className="space-y-2">
                    <p className="font-medium">
                      {resumen.direccionEnvio.calle} {resumen.direccionEnvio.numero}
                    </p>
                    {resumen.direccionEnvio.departamento && (
                      <p>Depto: {resumen.direccionEnvio.departamento}</p>
                    )}
                    <p>{resumen.direccionEnvio.comuna}, {resumen.direccionEnvio.region}</p>
                    <p>Código Postal: {resumen.direccionEnvio.codigoPostal}</p>
                    {resumen.direccionEnvio.esPrincipal && (
                      <span className="badge-primary">Dirección Principal</span>
                    )}
                  </div>
                ) : (
                  <p className="text-gray-500">No hay dirección de envío configurada</p>
                )}
              </div>

              {/* Método de pago */}
              <div className="card">
                <h3 className="text-lg font-semibold mb-4">Método de Pago</h3>
                <div className="space-y-3">
                  <label className="flex items-center">
                    <input
                      type="radio"
                      name="metodoPago"
                      value="efectivo"
                      checked={metodoPago === 'efectivo'}
                      onChange={(e) => setMetodoPago(e.target.value)}
                      className="mr-2"
                    />
                    <span>Efectivo</span>
                  </label>
                  <label className="flex items-center">
                    <input
                      type="radio"
                      name="metodoPago"
                      value="transferencia"
                      checked={metodoPago === 'transferencia'}
                      onChange={(e) => setMetodoPago(e.target.value)}
                      className="mr-2"
                    />
                    <span>Transferencia Bancaria</span>
                  </label>
                  <label className="flex items-center">
                    <input
                      type="radio"
                      name="metodoPago"
                      value="mercadopago"
                      checked={metodoPago === 'mercadopago'}
                      onChange={(e) => setMetodoPago(e.target.value)}
                      className="mr-2"
                    />
                    <span>MercadoPago</span>
                  </label>
                </div>
              </div>

              {/* Observaciones */}
              <div className="card">
                <h3 className="text-lg font-semibold mb-4">Observaciones</h3>
                <textarea
                  value={observaciones}
                  onChange={(e) => setObservaciones(e.target.value)}
                  placeholder="Agregar observaciones adicionales..."
                  className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
                  rows={3}
                />
              </div>
            </div>

            {/* Columna derecha - Resumen de productos y totales */}
            <div className="space-y-6">
              {/* Lista de productos */}
              <div className="card">
                <h3 className="text-lg font-semibold mb-4">Productos ({resumen.totalItems})</h3>
                <div className="space-y-3">
                  {resumen.items.map(item => (
                    <div key={item.id} className="flex items-center justify-between p-3 border rounded-lg">
                      <div className="flex items-center space-x-3">
                        <div className="w-12 h-12 bg-gray-200 rounded-lg flex items-center justify-center">
                          {item.productoImagen ? (
                            <img 
                              src={item.productoImagen} 
                              alt={item.productoNombre}
                              className="w-full h-full object-cover rounded-lg"
                            />
                          ) : (
                            <span className="text-gray-400">📦</span>
                          )}
                        </div>
                        <div>
                          <p className="font-medium">{item.productoNombre}</p>
                          <p className="text-sm text-gray-600">
                            {item.cantidad} x ${item.productoPrecio.toFixed(2)}
                          </p>
                        </div>
                      </div>
                      <p className="font-semibold">${item.subtotal.toFixed(2)}</p>
                    </div>
                  ))}
                </div>
              </div>

              {/* Resumen de totales */}
              <div className="card">
                <h3 className="text-lg font-semibold mb-4">Resumen de Totales</h3>
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span>Subtotal:</span>
                    <span>${resumen.subtotal.toFixed(2)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Descuento:</span>
                    <span>-${resumen.descuento.toFixed(2)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>IVA (19%):</span>
                    <span>${resumen.impuestos.toFixed(2)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Envío:</span>
                    <span>${resumen.envio.toFixed(2)}</span>
                  </div>
                  <div className="border-t pt-2 mt-2">
                    <div className="flex justify-between font-bold text-lg">
                      <span>Total:</span>
                      <span className="text-ferremas-primary">${resumen.total.toFixed(2)}</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Botón de procesar */}
              <div className="card">
                <button
                  onClick={handleProcesarPedido}
                  disabled={processing || !selectedDireccionId}
                  className="w-full btn-primary py-3 text-lg font-semibold"
                >
                  {processing ? 'Procesando...' : `Procesar Pedido - $${resumen.total.toFixed(2)}`}
                </button>
                <p className="text-sm text-gray-500 mt-2 text-center">
                  Al procesar el pedido, se creará la orden y se limpiará el carrito
                </p>
              </div>
            </div>
          </div>
        )}

        {/* Diálogos de confirmación */}
        <ConfirmDialog
          isOpen={confirmDialog.isOpen}
          onClose={() => setConfirmDialog({ isOpen: false, title: '', message: '' })}
          onConfirm={confirmProcesarPedido}
          title={confirmDialog.title}
          message={confirmDialog.message}
          confirmText="Confirmar Pedido"
          cancelText="Cancelar"
          type="info"
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

export default Checkout; 