import React, { useState, useEffect } from 'react';
import { enviosService } from '../../services/envios';
import type { CotizacionEnvioRequest, CotizacionEnvioResponse } from '../../services/envios';

interface CotizacionEnvioModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSelectCosto: (costo: number, metodo: string) => void;
  direccionDestino: string;
}

const CotizacionEnvioModal: React.FC<CotizacionEnvioModalProps> = ({
  isOpen,
  onClose,
  onSelectCosto,
  direccionDestino
}) => {
  const [cotizaciones, setCotizaciones] = useState<CotizacionEnvioResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen && direccionDestino) {
      obtenerCotizaciones();
    }
  }, [isOpen, direccionDestino]);

  const obtenerCotizaciones = async () => {
    setLoading(true);
    setError(null);
    
    try {
      const request: CotizacionEnvioRequest = {
        origen: 'Santiago',
        destino: direccionDestino,
        peso: 1, // Peso base de 1kg
        alto: 10,
        ancho: 10,
        largo: 10
      };

      const response = await enviosService.obtenerCotizaciones(request);
      setCotizaciones(response);
    } catch (err: any) {
      setError(err.message || 'Error al obtener cotizaciones');
    } finally {
      setLoading(false);
    }
  };

  const handleSeleccionarCosto = (costo: number, metodo: string) => {
    onSelectCosto(costo, metodo);
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 max-w-2xl w-full mx-4 max-h-[80vh] overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-gray-900">Cotizaciones de Envío</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 text-2xl font-bold"
          >
            ×
          </button>
        </div>

        {loading && (
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-2 text-gray-600">Obteniendo cotizaciones...</p>
          </div>
        )}

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4 mb-4">
            <p className="text-red-600">{error}</p>
            <button
              onClick={obtenerCotizaciones}
              className="mt-2 text-sm text-red-600 hover:text-red-800 underline"
            >
              Reintentar
            </button>
          </div>
        )}

        {cotizaciones && !loading && (
          <div className="space-y-4">
            {/* Cotizaciones de Shipit */}
            {cotizaciones.shipit.quotations.length > 0 && (
              <div>
                <h3 className="font-semibold text-lg mb-3 text-blue-600">Shipit</h3>
                <div className="space-y-2">
                  {cotizaciones.shipit.quotations.map((cotizacion, index) => (
                    <div
                      key={index}
                      className="border border-gray-200 rounded-lg p-4 hover:border-blue-300 cursor-pointer transition-colors"
                      onClick={() => handleSeleccionarCosto(cotizacion.price, `Shipit - ${cotizacion.service}`)}
                    >
                      <div className="flex justify-between items-center">
                        <div>
                          <p className="font-medium">{cotizacion.service}</p>
                          <p className="text-sm text-gray-600">
                            {cotizacion.deliveryTimeText} • {cotizacion.courier}
                          </p>
                        </div>
                        <div className="text-right">
                          <p className="font-bold text-lg">${cotizacion.price.toLocaleString('es-CL')}</p>
                          <p className="text-xs text-gray-500">CLP</p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Cotizaciones de Chilexpress */}
            {cotizaciones.chilexpress.length > 0 && (
              <div>
                <h3 className="font-semibold text-lg mb-3 text-green-600">Chilexpress</h3>
                <div className="space-y-2">
                  {cotizaciones.chilexpress.map((cotizacion, index) => (
                    <div
                      key={index}
                      className="border border-gray-200 rounded-lg p-4 hover:border-green-300 cursor-pointer transition-colors"
                      onClick={() => handleSeleccionarCosto(cotizacion.precio, `Chilexpress - ${cotizacion.servicio}`)}
                    >
                      <div className="flex justify-between items-center">
                        <div>
                          <p className="font-medium">{cotizacion.servicio}</p>
                          <p className="text-sm text-gray-600">{cotizacion.plazoEntrega}</p>
                        </div>
                        <div className="text-right">
                          <p className="font-bold text-lg">${cotizacion.precio.toLocaleString('es-CL')}</p>
                          <p className="text-xs text-gray-500">CLP</p>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Opciones manuales */}
            <div>
              <h3 className="font-semibold text-lg mb-3 text-gray-600">Opciones Manuales</h3>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-2">
                <button
                  onClick={() => handleSeleccionarCosto(4990, 'Chilexpress')}
                  className="border border-gray-200 rounded-lg p-3 hover:border-blue-300 transition-colors text-left"
                >
                  <p className="font-medium">Chilexpress</p>
                  <p className="text-sm text-gray-600">Envío estándar</p>
                  <p className="font-bold">$4.990</p>
                </button>
                <button
                  onClick={() => handleSeleccionarCosto(3990, 'Starken')}
                  className="border border-gray-200 rounded-lg p-3 hover:border-blue-300 transition-colors text-left"
                >
                  <p className="font-medium">Starken</p>
                  <p className="text-sm text-gray-600">Envío económico</p>
                  <p className="font-bold">$3.990</p>
                </button>
                <button
                  onClick={() => handleSeleccionarCosto(3500, 'Shipit')}
                  className="border border-gray-200 rounded-lg p-3 hover:border-blue-300 transition-colors text-left"
                >
                  <p className="font-medium">Shipit</p>
                  <p className="text-sm text-gray-600">Envío express</p>
                  <p className="font-bold">$3.500</p>
                </button>
              </div>
            </div>
          </div>
        )}

        <div className="mt-6 flex justify-end">
          <button
            onClick={onClose}
            className="px-4 py-2 text-gray-600 hover:text-gray-800 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
          >
            Cancelar
          </button>
        </div>
      </div>
    </div>
  );
};

export default CotizacionEnvioModal; 