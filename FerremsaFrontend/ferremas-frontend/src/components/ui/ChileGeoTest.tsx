import React, { useState, useEffect } from 'react';
import chileGeoService from '../../services/chileGeo';
import type { RegionChile, ComunaChile } from '../../types/api';

const ChileGeoTest: React.FC = () => {
    const [regiones, setRegiones] = useState<RegionChile[]>([]);
    const [comunas, setComunas] = useState<ComunaChile[]>([]);
    const [regionSeleccionada, setRegionSeleccionada] = useState<string>('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [info, setInfo] = useState<any>(null);

    useEffect(() => {
        cargarRegiones();
        cargarInfo();
    }, []);

    const cargarRegiones = async () => {
        setLoading(true);
        try {
            const data = await chileGeoService.getRegiones();
            setRegiones(data);
            setError(null);
        } catch (err: any) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    const cargarComunas = async (codigoRegion: string) => {
        if (!codigoRegion) {
            setComunas([]);
            return;
        }

        setLoading(true);
        try {
            const data = await chileGeoService.getComunasByRegion(codigoRegion);
            setComunas(data);
            setError(null);
        } catch (err: any) {
            setError(err.message);
            setComunas([]);
        } finally {
            setLoading(false);
        }
    };

    const cargarInfo = async () => {
        try {
            const data = await chileGeoService.getInfo();
            setInfo(data);
        } catch (err: any) {
            console.error('Error al cargar info:', err);
        }
    };

    const handleRegionChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const region = regiones.find(r => r.nombre === e.target.value);
        setRegionSeleccionada(e.target.value);
        if (region) {
            cargarComunas(region.codigo);
        } else {
            setComunas([]);
        }
    };

    return (
        <div className="max-w-4xl mx-auto p-6 bg-white rounded-lg shadow-lg">
            <h2 className="text-2xl font-bold text-ferremas-primary mb-6">
                Prueba de API de Geografía de Chile
            </h2>

            {/* Información de la API */}
            {info && (
                <div className="mb-6 p-4 bg-ferremas-gray-50 rounded-lg">
                    <h3 className="text-lg font-semibold text-ferremas-primary mb-2">
                        Información de la API
                    </h3>
                    <div className="text-sm text-ferremas-gray-600">
                        <p><strong>Nombre:</strong> {info.nombre}</p>
                        <p><strong>Versión:</strong> {info.version}</p>
                        <p><strong>Fuente:</strong> {info.fuente}</p>
                        <p><strong>Estado:</strong> {info.estado}</p>
                    </div>
                </div>
            )}

            {/* Selector de Región */}
            <div className="mb-6">
                <label className="block text-sm font-medium text-ferremas-primary mb-2">
                    Seleccionar Región
                </label>
                <select
                    value={regionSeleccionada}
                    onChange={handleRegionChange}
                    className="w-full px-3 py-2 border border-ferremas-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-orange-400"
                    disabled={loading}
                >
                    <option value="">Seleccione una región</option>
                    {regiones.map((region) => (
                        <option key={region.codigo} value={region.nombre}>
                            {region.nombre} ({region.codigo})
                        </option>
                    ))}
                </select>
            </div>

            {/* Lista de Comunas */}
            {regionSeleccionada && (
                <div className="mb-6">
                    <h3 className="text-lg font-semibold text-ferremas-primary mb-3">
                        Comunas de {regionSeleccionada}
                    </h3>
                    {loading ? (
                        <div className="text-center py-4">
                            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-ferremas-primary mx-auto"></div>
                            <p className="mt-2 text-ferremas-gray-600">Cargando comunas...</p>
                        </div>
                    ) : comunas.length > 0 ? (
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
                            {comunas.map((comuna) => (
                                <div
                                    key={comuna.codigo}
                                    className="p-3 bg-ferremas-gray-50 rounded-lg border border-ferremas-gray-200"
                                >
                                    <div className="font-medium text-ferremas-primary">
                                        {comuna.nombre}
                                    </div>
                                    <div className="text-sm text-ferremas-gray-600">
                                        Código: {comuna.codigo}
                                    </div>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <p className="text-ferremas-gray-600">No se encontraron comunas para esta región.</p>
                    )}
                </div>
            )}

            {/* Estadísticas */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                <div className="p-4 bg-ferremas-orange-50 rounded-lg border border-ferremas-orange-200">
                    <h4 className="font-semibold text-ferremas-primary mb-2">Regiones</h4>
                    <p className="text-2xl font-bold text-ferremas-orange-600">{regiones.length}</p>
                    <p className="text-sm text-ferremas-gray-600">Total de regiones cargadas</p>
                </div>
                <div className="p-4 bg-ferremas-blue-50 rounded-lg border border-ferremas-blue-200">
                    <h4 className="font-semibold text-ferremas-primary mb-2">Comunas</h4>
                    <p className="text-2xl font-bold text-ferremas-blue-600">{comunas.length}</p>
                    <p className="text-sm text-ferremas-gray-600">
                        Comunas en {regionSeleccionada || 'todas las regiones'}
                    </p>
                </div>
            </div>

            {/* Error */}
            {error && (
                <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
                    <p className="text-red-800 font-medium">Error:</p>
                    <p className="text-red-600">{error}</p>
                </div>
            )}

            {/* Botones de acción */}
            <div className="flex gap-4">
                <button
                    onClick={cargarRegiones}
                    disabled={loading}
                    className="px-4 py-2 bg-ferremas-primary text-white rounded-md hover:bg-ferremas-primary-dark disabled:opacity-50"
                >
                    Recargar Regiones
                </button>
                <button
                    onClick={() => {
                        setRegionSeleccionada('');
                        setComunas([]);
                    }}
                    className="px-4 py-2 bg-ferremas-gray-500 text-white rounded-md hover:bg-ferremas-gray-600"
                >
                    Limpiar Selección
                </button>
            </div>
        </div>
    );
};

export default ChileGeoTest; 