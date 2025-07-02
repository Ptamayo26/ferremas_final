import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { api } from '../../services/api';
import type { ProductoResponseDTO } from '../../types/api';
import { useAuth } from '../../context/AuthContext';
import AddToCartButton from '../../components/ui/AddToCartButton';
import CarritoButton from '../../components/ui/CarritoButton';
import Carrito from '../../components/sales/Carrito';
import { SimuladorDespacho } from '../../components/ui/SimuladorDespacho';

const ProductoDetalle: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const { isAuthenticated } = useAuth();
    const [producto, setProducto] = useState<ProductoResponseDTO | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [carritoAbierto, setCarritoAbierto] = useState(false);
    const [checkoutAbierto, setCheckoutAbierto] = useState(false);
    const [cantidad, setCantidad] = useState(1);
    const [modalCotizar, setModalCotizar] = useState(false);

    useEffect(() => {
        const fetchProducto = async () => {
            if (!id) return;
            try {
                setLoading(true);
                const fetchedProducto = await api.getProductoById(parseInt(id, 10));
                setProducto(fetchedProducto);
                setError(null);
            } catch (err) {
                setError('Error al cargar el producto.');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchProducto();
    }, [id]);

    const handleProductoAgregado = () => {
        console.log('Producto agregado al carrito');
    };

    // Simula un descuento del 20% si el precio es mayor a 5000
    const tieneDescuento = producto && producto.precio > 5000;
    const precioDescuento = tieneDescuento ? Math.round(producto!.precio * 0.8) : producto?.precio;

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-ferremas-primary mx-auto mb-4"></div>
                    <p className="text-lg text-gray-600">Cargando producto...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="container mx-auto p-4">
                <div className="text-center py-16">
                    <div className="text-red-500 text-6xl mb-4">⚠️</div>
                    <h2 className="text-2xl font-semibold text-gray-800 mb-4">Error al cargar el producto</h2>
                    <p className="text-gray-600 mb-6">{error}</p>
                    <Link to="/catalogo" className="btn-primary">
                        Volver al Catálogo
                    </Link>
                </div>
            </div>
        );
    }

    if (!producto) {
        return (
            <div className="container mx-auto p-4">
                <div className="text-center py-16">
                    <div className="text-gray-400 text-6xl mb-4">🔍</div>
                    <h2 className="text-2xl font-semibold text-gray-800 mb-4">Producto no encontrado</h2>
                    <p className="text-gray-600 mb-6">El producto que buscas no existe o ha sido removido.</p>
                    <Link to="/catalogo" className="btn-primary">
                        Volver al Catálogo
                    </Link>
                </div>
            </div>
        );
    }

    return (
        <div className="container mx-auto px-2 md:px-8 py-8">
            {/* Header con navegación */}
            <div className="flex justify-between items-center mb-8">
                <div className="flex items-center space-x-4">
                    <Link 
                        to="/catalogo" 
                        className="text-ferremas-primary hover:text-ferremas-primary-dark font-medium"
                    >
                        ← Volver al Catálogo
                    </Link>
                    <span className="text-gray-400">/</span>
                    <span className="text-gray-600">{producto.nombre}</span>
                </div>
            </div>

            {/* Layout principal */}
            <div className="bg-white rounded-lg shadow-md p-4 md:p-8 flex flex-col md:flex-row gap-8">
                {/* Imagen grande */}
                <div className="flex-1 flex flex-col items-center justify-center">
                    <div className="relative w-full max-w-xs md:max-w-sm lg:max-w-md aspect-square bg-gray-50 rounded-lg flex items-center justify-center overflow-hidden">
                        <img
                            src={`http://localhost:5200${producto.imagenUrl}` || '/placeholder.png'}
                            alt={producto.nombre}
                            className="w-full h-full object-contain"
                            onError={e => {
                                if (e.currentTarget.src.includes('placeholder.png')) {
                                    e.currentTarget.onerror = null;
                                    return;
                                }
                                e.currentTarget.src = '/placeholder.png';
                            }}
                        />
                        {tieneDescuento && (
                            <span className="absolute top-3 left-3 bg-red-500 text-white text-xs font-bold px-3 py-1 rounded-full shadow-lg">20% DCTO</span>
                        )}
                    </div>
                </div>

                {/* Info producto */}
                <div className="flex-1 flex flex-col justify-between">
                    <div>
                        <h1 className="text-2xl md:text-3xl font-bold text-gray-900 mb-2">{producto.nombre}</h1>
                        <div className="text-gray-500 text-sm mb-2">SKU: {producto.codigo}</div>
                        <div className="mb-4">
                            <p className="text-gray-700 text-lg leading-relaxed">{producto.descripcion}</p>
                        </div>
                        {/* Precio */}
                        <div className="mb-4 flex items-end gap-4">
                            <div className="text-4xl font-bold text-red-500">${precioDescuento?.toLocaleString('es-CL')}</div>
                            {tieneDescuento && (
                                <div className="text-lg text-gray-400 line-through">${producto.precio.toLocaleString('es-CL')}</div>
                            )}
                        </div>
                        {/* Selector de cantidad */}
                        <div className="mb-4 flex items-center gap-2">
                            <span className="font-semibold">Cantidad:</span>
                            <button
                                className="w-8 h-8 rounded bg-gray-200 hover:bg-gray-300 text-lg font-bold"
                                onClick={() => setCantidad(c => Math.max(1, c - 1))}
                            >-</button>
                            <span className="w-8 text-center">{cantidad}</span>
                            <button
                                className="w-8 h-8 rounded bg-gray-200 hover:bg-gray-300 text-lg font-bold"
                                onClick={() => setCantidad(c => c + 1)}
                            >+</button>
                        </div>
                        {/* Botón agregar al carrito */}
                        <div className="mb-4">
                            <AddToCartButton
                                productoId={producto.id}
                                productoNombre={producto.nombre}
                                stockDisponible={producto.stock}
                                precio={producto.precio}
                                imagenUrl={producto.imagenUrl}
                                cantidad={cantidad}
                                onSuccess={handleProductoAgregado}
                                className="w-full text-lg py-4"
                            />
                        </div>
                        {/* Stock */}
                        <div className="mb-4 flex items-center gap-2">
                            <span className="text-gray-600">Stock disponible:</span>
                            <span className={`font-semibold ${
                                producto.stock === 0 ? 'text-red-600' : 
                                producto.stock <= 5 ? 'text-orange-600' : 'text-green-600'
                            }`}>
                                {producto.stock} unidades
                            </span>
                        </div>
                        {/* Métodos de entrega */}
                        <div className="mb-4">
                            <h3 className="text-md font-semibold mb-2">Disponibilidad y métodos de entrega</h3>
                            <div className="flex flex-col gap-2 text-gray-700 text-sm">
                                <div className="flex items-center gap-2"><span>🏬</span> Retiro en tienda</div>
                                <div className="flex items-center gap-2"><span>🚚</span> Despacho a domicilio <button type="button" className="text-blue-500 underline" onClick={() => setModalCotizar(true)}>Simular costo de despacho</button></div>
                                <div className="flex items-center gap-2"><span>📦</span> Ver stock</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <SimuladorDespacho
                isOpen={modalCotizar}
                onClose={() => setModalCotizar(false)}
                productoPeso={1}
                productoLargo={30}
                productoAncho={20}
                productoAlto={10}
            />
        </div>
    );
};

export default ProductoDetalle; 