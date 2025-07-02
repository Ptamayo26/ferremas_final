import React from 'react';
import { Link } from 'react-router-dom';
import type { CheckoutResponseDTO } from '../../types/api';

interface ConfirmacionPedidoProps {
  isOpen: boolean;
  onClose: () => void;
  pedido: CheckoutResponseDTO | null;
}

const ConfirmacionPedido: React.FC<ConfirmacionPedidoProps> = ({ isOpen, onClose, pedido }) => {
  if (!isOpen || !pedido) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-8 w-full max-w-2xl mx-4">
        <div className="text-center">
          {/* Icono de éxito */}
          <div className="mx-auto flex items-center justify-center h-16 w-16 rounded-full bg-green-100 mb-6">
            <svg className="h-8 w-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </div>

          {/* Título */}
          <h2 className="text-2xl font-bold text-gray-900 mb-4">
            ¡Pedido Confirmado!
          </h2>

          {/* Información del pedido */}
          <div className="bg-gray-50 rounded-lg p-6 mb-6">
            <div className="space-y-4">
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Número de Pedido:</span>
                <span className="font-semibold text-lg">{pedido.numeroPedido}</span>
              </div>
              
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Total:</span>
                <span className="font-bold text-xl text-ferremas-primary">
                  ${pedido.total.toLocaleString('es-CL')}
                </span>
              </div>
              
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Estado:</span>
                <span className="badge-success">{pedido.estado}</span>
              </div>
              
              <div className="flex justify-between items-center">
                <span className="text-gray-600">Fecha:</span>
                <span className="font-medium">
                  {new Date(pedido.fechaCreacion).toLocaleDateString('es-CL', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                  })}
                </span>
              </div>
            </div>
          </div>

          {/* Información adicional */}
          <div className="text-left mb-6">
            <h3 className="text-lg font-semibold mb-3">Próximos pasos:</h3>
            <div className="space-y-2 text-sm text-gray-600">
              <div className="flex items-start space-x-2">
                <span className="text-green-500 mt-0.5">✓</span>
                <span>Recibirás un email de confirmación con los detalles del pedido</span>
              </div>
              <div className="flex items-start space-x-2">
                <span className="text-blue-500 mt-0.5">📧</span>
                <span>Te notificaremos cuando tu pedido esté listo para envío</span>
              </div>
              <div className="flex items-start space-x-2">
                <span className="text-purple-500 mt-0.5">🚚</span>
                <span>Recibirás información de seguimiento cuando se envíe</span>
              </div>
            </div>
          </div>

          {/* Información de contacto */}
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6">
            <h4 className="font-semibold text-blue-900 mb-2">¿Necesitas ayuda?</h4>
            <p className="text-sm text-blue-700 mb-2">
              Si tienes alguna pregunta sobre tu pedido, no dudes en contactarnos:
            </p>
            <div className="text-sm text-blue-700 space-y-1">
              <p>📞 Teléfono: +56 2 2345 6789</p>
              <p>📧 Email: pedidos@ferremas.cl</p>
              <p>💬 WhatsApp: +56 9 8765 4321</p>
            </div>
          </div>

          {/* Botones de acción */}
          <div className="flex flex-col sm:flex-row gap-3">
            <Link
              to="/catalogo"
              className="btn-secondary flex-1"
              onClick={onClose}
            >
              Continuar Comprando
            </Link>
            
            <Link
              to="/mi-cuenta"
              className="btn-primary flex-1"
              onClick={onClose}
            >
              Ver Mis Pedidos
            </Link>
          </div>

          {/* Botón cerrar */}
          <button
            onClick={onClose}
            className="mt-4 text-gray-500 hover:text-gray-700 text-sm"
          >
            Cerrar
          </button>
        </div>
      </div>
    </div>
  );
};

export default ConfirmacionPedido; 