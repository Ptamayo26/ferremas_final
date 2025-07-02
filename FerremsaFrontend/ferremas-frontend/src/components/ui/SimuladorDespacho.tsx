import React, { useState, useEffect } from 'react';
import { enviosService } from '../../services/envios';
import type { CoberturaResult, CotizacionRequest, CotizacionResult } from '../../services/envios';

interface SimuladorDespachoProps {
  isOpen: boolean;
  onClose: () => void;
  productoPeso?: number;
  productoLargo?: number;
  productoAncho?: number;
  productoAlto?: number;
}

const ORIGEN_NOMBRE = 'Santiago'; // Puedes cambiarlo por el nombre exacto de tu sucursal

export const SimuladorDespacho: React.FC<SimuladorDespachoProps> = ({
  isOpen,
  onClose,
  productoPeso = 1,
  productoLargo = 30,
  productoAncho = 20,
  productoAlto = 10
}) => {
  const [origen, setOrigen] = useState(ORIGEN_NOMBRE);
  const [destino, setDestino] = useState('');
  const [origenSugerencias, setOrigenSugerencias] = useState<CoberturaResult[]>([]);
  const [destinoSugerencias, setDestinoSugerencias] = useState<CoberturaResult[]>([]);
  const [origenSeleccionado, setOrigenSeleccionado] = useState<CoberturaResult | null>(null);
  const [destinoSeleccionado, setDestinoSeleccionado] = useState<CoberturaResult | null>(null);
  const [peso, setPeso] = useState(productoPeso);
  const [largo, setLargo] = useState(productoLargo);
  const [ancho, setAncho] = useState(productoAncho);
  const [alto, setAlto] = useState(productoAlto);
  const [cotizaciones, setCotizaciones] = useState<CotizacionResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [busquedaCobertura, setBusquedaCobertura] = useState('');
  const [resultadosBusqueda, setResultadosBusqueda] = useState<CoberturaResult[]>([]);
  const [buscando, setBuscando] = useState(false);

  // Buscar y seleccionar automáticamente la cobertura de origen al abrir el modal
  useEffect(() => {
    if (isOpen) {
      (async () => {
        try {
          const resultados = await enviosService.buscarCoberturas(ORIGEN_NOMBRE);
          if (resultados.length > 0 && resultados[0]) {
            setOrigenSeleccionado(resultados[0] as CoberturaResult);
            setOrigen(resultados[0].nombre);
          }
        } catch (e) {
          setError('No se pudo obtener la cobertura de origen');
        }
      })();
      setDestino('');
      setDestinoSeleccionado(null);
      setCotizaciones([]);
      setError('');
    }
  }, [isOpen]);

  useEffect(() => {
    if (destino.length >= 3) {
      buscarCoberturas(destino, setDestinoSugerencias);
    } else {
      setDestinoSugerencias([]);
    }
  }, [destino]);

  const buscarCoberturas = async (nombre: string, setSugerencias: (sugerencias: CoberturaResult[]) => void) => {
    try {
      const resultados = await enviosService.buscarCoberturas(nombre);
      setSugerencias(resultados);
    } catch (error) {
      console.error('Error al buscar coberturas:', error);
    }
  };

  const cotizarEnvio = async () => {
    if (!origenSeleccionado || !destinoSeleccionado) {
      setError('Debes seleccionar origen y destino');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const request: CotizacionRequest = {
        codigoCoberturaOrigen: origenSeleccionado.codigo,
        codigoCoberturaDestino: destinoSeleccionado.codigo,
        peso,
        largo,
        ancho,
        alto
      };

      const resultados = await enviosService.cotizarEnvio(request);
      setCotizaciones(resultados);
    } catch (error) {
      setError('Error al cotizar el envío. Intenta nuevamente.');
      console.error('Error al cotizar:', error);
    } finally {
      setLoading(false);
    }
  };

  const seleccionarDestino = (cobertura: CoberturaResult) => {
    setDestinoSeleccionado(cobertura);
    setDestino(cobertura.nombre);
    setDestinoSugerencias([]);
  };

  // Función temporal para buscar coberturas manualmente
  const buscarCoberturaManual = async () => {
    setBuscando(true);
    setResultadosBusqueda([]);
    try {
      const resultados = await enviosService.buscarCoberturas(busquedaCobertura);
      setResultadosBusqueda(resultados);
    } catch (e) {
      setResultadosBusqueda([]);
    } finally {
      setBuscando(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Simular Costo de Despacho</h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 text-2xl"
          >
            ×
          </button>
        </div>

        {/* Herramienta temporal para buscar coberturas */}
        <div className="mb-4 p-2 border border-yellow-300 bg-yellow-50 rounded">
          <div className="font-semibold text-yellow-800 mb-1">Buscar cobertura Chilexpress (solo para admins)</div>
          <div className="flex gap-2 mb-2">
            <input
              type="text"
              value={busquedaCobertura}
              onChange={e => setBusquedaCobertura(e.target.value)}
              placeholder="Ej: Santiago, Santiago Centro, Estación Central..."
              className="p-2 border border-gray-300 rounded w-full"
            />
            <button
              onClick={buscarCoberturaManual}
              className="bg-yellow-500 text-white px-3 py-1 rounded hover:bg-yellow-600"
              disabled={buscando || !busquedaCobertura.trim()}
            >Buscar</button>
          </div>
          {buscando && <div className="text-yellow-700">Buscando...</div>}
          {resultadosBusqueda.length > 0 && (
            <div className="max-h-32 overflow-y-auto border border-yellow-200 rounded bg-white">
              {resultadosBusqueda.map((c, i) => (
                <div key={i} className="p-2 text-sm text-gray-800 border-b border-yellow-100">
                  <span className="font-mono text-blue-700">{c.codigo}</span> - {c.nombre}
                </div>
              ))}
            </div>
          )}
        </div>
        {/* Fin herramienta temporal */}

        <div className="space-y-4">
          {/* Origen (bloqueado) */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Origen
            </label>
            <input
              type="text"
              value={origen}
              disabled
              className="w-full p-2 border border-gray-300 rounded-md bg-gray-100 cursor-not-allowed"
            />
          </div>

          {/* Destino */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Destino
            </label>
            <input
              type="text"
              value={destino}
              onChange={(e) => setDestino(e.target.value)}
              placeholder="Ingresa la ciudad de destino"
              className="w-full p-2 border border-gray-300 rounded-md"
            />
            {destinoSugerencias.length > 0 && (
              <div className="mt-1 border border-gray-300 rounded-md max-h-32 overflow-y-auto">
                {destinoSugerencias.map((sugerencia, index) => (
                  <div
                    key={index}
                    onClick={() => seleccionarDestino(sugerencia)}
                    className="p-2 hover:bg-gray-100 cursor-pointer"
                  >
                    {sugerencia.nombre}
                  </div>
                ))}
              </div>
            )}
          </div>

          {error && (
            <div className="text-red-600 text-sm">{error}</div>
          )}

          <button
            onClick={cotizarEnvio}
            disabled={loading || !origenSeleccionado || !destinoSeleccionado}
            className="w-full bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed"
          >
            {loading ? 'Cotizando...' : 'Cotizar Envío'}
          </button>

          {/* Resultados */}
          {cotizaciones.length > 0 && (
            <div className="mt-4">
              <h3 className="text-lg font-semibold mb-2">Opciones de Envío</h3>
              <div className="space-y-2">
                {cotizaciones.map((cotizacion, index) => (
                  <div key={index} className="border border-gray-200 rounded-md p-3">
                    <div className="flex justify-between items-center">
                      <div>
                        <div className="font-medium">{cotizacion.servicio}</div>
                        <div className="text-sm text-gray-600">
                          Plazo de entrega: {cotizacion.plazoEntrega}
                        </div>
                      </div>
                      <div className="text-lg font-bold text-green-600">
                        ${cotizacion.precio.toLocaleString('es-CL')}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}; 