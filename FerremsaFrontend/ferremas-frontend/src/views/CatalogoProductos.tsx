import React, { useEffect, useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { publicApiClient } from '../services/api';
import type { ProductoResponseDTO } from '../types/api';
import AddToCartButton from '../components/ui/AddToCartButton';
import Carrito from '../components/sales/Carrito';
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import { useAuth } from '../context/AuthContext';

const BASE_URL = "http://localhost:5200"; // Cambia esto si tu backend está en otra URL

// Mock de imágenes y categorías destacadas (puedes reemplazar por tus datos reales)
// const categoriasDestacadas = [
//   { nombre: 'Calefacción', icono: '/vite.svg' },
//   { nombre: 'Muebles', icono: '/vite.svg' },
//   { nombre: 'Decohogar', icono: '/vite.svg' },
//   { nombre: 'Herramientas', icono: '/vite.svg' },
//   { nombre: 'Baño', icono: '/vite.svg' },
//   { nombre: 'Línea Blanca', icono: '/vite.svg' },
//   { nombre: 'Jardín', icono: '/vite.svg' },
// ];

// Mapeo de categorías reales de ferretería con ícono y descripción
const iconoDefault = "/vite.svg";
const categoriasInfo: Record<string, {icono: string, descripcion: string}> = {
  "Herramientas Eléctricas": { icono: "http://localhost:5200/images/productos/Taladro%20percutor%20650W.webp", descripcion: "Taladros, sierras, esmeriles y más." },
  "Herramientas Manuales": { icono: "http://localhost:5200/images/productos/MARTILLO%20de%20carpintero.webp", descripcion: "Martillos, destornilladores, llaves, etc." },
  "Jardinería": { icono: "http://localhost:5200/images/productos/tijera%20para%20podar.webp", descripcion: "Tijeras, palas, herramientas para jardín." },
  "Construcción": { icono: "http://localhost:5200/images/productos/cemento%20portland.jpeg", descripcion: "Cemento, mezclas, herramientas de obra." },
  "Electricidad": { icono: "http://localhost:5200/images/productos/cable%20electrico%20(cordón).webp", descripcion: "Cables, enchufes, iluminación." },
  "Plomería": { icono: "http://localhost:5200/images/productos/Llave%20Stilson%2014.webp", descripcion: "Llaves, tubos, accesorios de agua." },
  "Pintura y Acabados": { icono: "http://localhost:5200/images/productos/Rodillo%20para%20Pintura%209.webp", descripcion: "Rodillos, brochas, pinturas." },
  "Seguridad Industrial": { icono: "http://localhost:5200/images/productos/Casco%20de%20Seguridad%20Amarillo.webp", descripcion: "Cascos, guantes, protección personal." },
};

const especialesTemporada = [
  {
    titulo: 'Herramientas Eléctricas',
    desc: categoriasInfo['Herramientas Eléctricas'].descripcion,
    ahorro: 'Hasta 40% de ahorro',
    color: 'bg-gray-200',
    img: categoriasInfo['Herramientas Eléctricas'].icono
  },
  {
    titulo: 'Herramientas Manuales',
    desc: categoriasInfo['Herramientas Manuales'].descripcion,
    ahorro: 'Hasta 35% de ahorro',
    color: 'bg-orange-100',
    img: categoriasInfo['Herramientas Manuales'].icono
  },
  {
    titulo: 'Jardinería',
    desc: categoriasInfo['Jardinería'].descripcion,
    ahorro: 'Hasta 25% de ahorro',
    color: 'bg-yellow-100',
    img: categoriasInfo['Jardinería'].icono
  }
];

const productosDestacadosMock = [
  { id: 1, nombre: 'Regadera plástica', precio: 6290, imagenUrl: '/vite.svg', marca: 'ERGO', rating: 4.2 },
  { id: 2, nombre: 'Rastrillo acero', precio: 17990, imagenUrl: '/vite.svg', marca: 'PLASMET', rating: 4.8 },
  { id: 3, nombre: 'Teflón premium', precio: 5990, imagenUrl: '/vite.svg', marca: 'TOPEX', rating: 4.9 },
  { id: 4, nombre: 'Llave de paso', precio: 11430, imagenUrl: '/vite.svg', marca: 'STRETTO', rating: 4.7 },
  { id: 5, nombre: 'Codo PVC', precio: 1390, imagenUrl: '/vite.svg', marca: 'VINILIT', rating: 4.7 },
  { id: 6, nombre: 'Toma corriente', precio: 4490, imagenUrl: '/vite.svg', marca: 'SCHNEIDER', rating: 4.8 },
];

// Mapeo de imágenes reales de productos
const imagenesProductos: Record<string, string> = {
  "Taladro Percutor 650W": "/images/productos/Taladro percutor 650W.webp",
  "Martillo de Carpintero": "/images/productos/MARTILLO de carpintero.webp",
  "Tijera de Podar": "/images/productos/tijera para podar.webp",
  "Cemento Portland 25kg": "/images/productos/cemento portland.jpeg",
  "Cable Eléctrico 2x1.5mm": "/images/productos/cable electrico (cordón).webp",
  "Llave Stilson 14\"": "/images/productos/Llave Stilson 14.webp",
  "Rodillo para Pintura 9\"": "/images/productos/Rodillo para Pintura 9.webp",
  "Casco de Seguridad Amarillo": "/images/productos/Casco de Seguridad Amarillo.webp",
  "Alicate": "/images/productos/Alicate.webp",
  "Destornillador": "/images/productos/Destornillador.webp",
  "Llave Inglesa": "/images/productos/Llave Inglesa.webp",
  "Martillo": "/images/productos/martillo acero.jpeg",
  // Agrega más productos según tus imágenes...
};
const placeholder = "/images/productos/placeholder.png";

const CatalogoProductos: React.FC = () => {
  const [productos, setProductos] = useState<ProductoResponseDTO[]>([]);
  const [productosFiltrados, setProductosFiltrados] = useState<ProductoResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  // Estados para búsqueda y filtros
  const [busqueda, setBusqueda] = useState('');
  const [categoriaFiltro, setCategoriaFiltro] = useState('');
  const [precioMin, setPrecioMin] = useState('');
  const [precioMax, setPrecioMax] = useState('');
  const [ordenarPor, setOrdenarPor] = useState('nombre');
  
  // Estados para el carrito
  const [carritoAbierto, setCarritoAbierto] = useState(false);
  const [checkoutAbierto, setCheckoutAbierto] = useState(false);

  // Estados para paginación
  const [paginaActual, setPaginaActual] = useState(1);
  const productosPorPagina = 6;

  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, user } = useAuth();

  useEffect(() => {
    const fetchProductos = async () => {
      console.log('1. Iniciando fetch de productos...');
      setLoading(true);
      setError(null);
      try {
        const response = await publicApiClient.get<any>('/api/Productos');
        console.log('2. Respuesta recibida del backend:', response);
        
        let productosData: ProductoResponseDTO[] = [];
        if (Array.isArray(response.data.productos)) {
          productosData = response.data.productos;
        } else if (Array.isArray(response.data)) {
          productosData = response.data;
        } else {
          setError('La respuesta del backend no contiene productos.');
          setProductos([]);
          setProductosFiltrados([]);
          return;
        }
        console.log('3. Datos de productos extraídos:', productosData);

        setProductos(productosData);
        setProductosFiltrados(productosData);
        if (productosData.length === 0) {
          console.warn('4. No hay productos disponibles en la respuesta.');
        } else {
          console.log('4. Productos cargados en el estado.');
        }
      } catch (err: any) {
        console.error('Error en el bloque try-catch:', err);
        setError('No se pudieron cargar los productos. Revisa la consola para más detalles.');
        setProductos([]);
        setProductosFiltrados([]);
      } finally {
        console.log('5. Finalizando fetch, setLoading a false.');
        setLoading(false);
      }
    };
    fetchProductos();
  }, []);

  useEffect(() => {
    // Si hay categoría en la query string, filtrarla automáticamente
    const params = new URLSearchParams(location.search);
    const cat = params.get('categoria');
    if (cat) setCategoriaFiltro(cat);
  }, [location.search]);

  // Filtrar productos cuando cambien los filtros
  useEffect(() => {
    let filtrados = [...productos];

    // Filtro por búsqueda
    if (busqueda) {
      filtrados = filtrados.filter(producto =>
        producto.nombre.toLowerCase().includes(busqueda.toLowerCase()) ||
        producto.descripcion.toLowerCase().includes(busqueda.toLowerCase()) ||
        producto.codigo.toLowerCase().includes(busqueda.toLowerCase())
      );
    }

    // Filtro por categoría
    if (categoriaFiltro) {
      filtrados = filtrados.filter(producto =>
        producto.categoriaNombre === categoriaFiltro
      );
    }

    // Filtro por precio mínimo
    if (precioMin) {
      filtrados = filtrados.filter(producto =>
        producto.precio >= parseFloat(precioMin)
      );
    }

    // Filtro por precio máximo
    if (precioMax) {
      filtrados = filtrados.filter(producto =>
        producto.precio <= parseFloat(precioMax)
      );
    }

    // Ordenar productos
    filtrados.sort((a, b) => {
      switch (ordenarPor) {
        case 'precio-asc':
          return a.precio - b.precio;
        case 'precio-desc':
          return b.precio - a.precio;
        case 'nombre':
          return a.nombre.localeCompare(b.nombre);
        case 'stock':
          return b.stock - a.stock;
        default:
          return 0;
      }
    });

    setProductosFiltrados(filtrados);
  }, [productos, busqueda, categoriaFiltro, precioMin, precioMax, ordenarPor]);

  // Obtener categorías únicas
  const categorias = [...new Set(productos.map(p => p.categoriaNombre).filter(Boolean))].sort();

  const limpiarFiltros = () => {
    setBusqueda('');
    setCategoriaFiltro('');
    setPrecioMin('');
    setPrecioMax('');
    setOrdenarPor('nombre');
  };

  const handleProductoAgregado = () => {
    // Aquí podrías actualizar el contador del carrito si es necesario
    console.log('Producto agregado al carrito');
  };

  // Calcular productos a mostrar según la página
  const indiceUltimoProducto = paginaActual * productosPorPagina;
  const indicePrimerProducto = indiceUltimoProducto - productosPorPagina;
  const productosPagina = productosFiltrados.slice(indicePrimerProducto, indiceUltimoProducto);
  const totalPaginas = Math.ceil(productosFiltrados.length / productosPorPagina);

  // Configuración del carrusel
  const sliderSettings = {
    dots: true,
    infinite: true,
    speed: 500,
    slidesToShow: 4,
    slidesToScroll: 1,
    responsive: [
      { breakpoint: 1280, settings: { slidesToShow: 3 } },
      { breakpoint: 900, settings: { slidesToShow: 2 } },
      { breakpoint: 600, settings: { slidesToShow: 1 } }
    ]
  };

  // Agrupar productos por categoría
  const productosPorCategoria: { [categoria: string]: ProductoResponseDTO[] } = {};
  productosFiltrados.forEach(producto => {
    const categoria = producto.categoriaNombre || 'Sin categoría';
    if (!productosPorCategoria[categoria]) productosPorCategoria[categoria] = [];
    productosPorCategoria[categoria].push(producto);
  });

  if (loading) return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="text-center">
        <div className="loading-spinner h-12 w-12 mx-auto mb-4"></div>
        <p className="text-lg text-gray-600">Cargando productos...</p>
      </div>
    </div>
  );

  if (error) return (
    <div className="empty-state">
      <div className="empty-state-icon">⚠️</div>
      <h3 className="empty-state-title">Error al cargar productos</h3>
      <p className="empty-state-description">{error}</p>
      <button 
        onClick={() => window.location.reload()}
        className="btn-primary"
      >
        Reintentar
      </button>
    </div>
  );

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-4xl font-extrabold text-ferremas-primary mb-6 tracking-wide">
        Catálogo de Productos
      </h1>
      {/* Aquí puedes agregar banners, sliders o categorías destacadas si lo deseas */}
      {/* Eliminar cualquier referencia a CarritoButton aquí */}

      {/* Texto de bienvenida y botón fuera del banner */}
      <div className="mb-4">
        <h2 className="text-xl md:text-2xl font-bold text-ferremas-primary mb-2">¡Bienvenido a Ferremas!</h2>
        <p className="text-base md:text-lg mb-4">Descubre las mejores ofertas y productos destacados de la temporada.</p>
        <button
          className="bg-ferremas-primary text-white font-semibold px-4 py-2 rounded shadow hover:bg-ferremas-primary-dark transition w-max"
          onClick={() => {
            document.getElementById('productos')?.scrollIntoView({ behavior: 'smooth' });
          }}
        >
          Ver productos
        </button>
      </div>

      {/* Banner superior */}
      <div className="w-full mb-8">
        <div className="relative rounded-lg overflow-hidden shadow-md h-[120px] md:h-[180px] lg:h-[220px] flex items-center mb-8 bg-transparent">
          {/* Imagen de fondo */}
          <img
            src="http://localhost:5200/images/productos/barra.png"
            alt="Banner"
            className="absolute inset-0 w-full h-full object-cover z-0"
            style={{ objectPosition: 'center' }}
          />
        </div>
      </div>

      {/* Código de descuento */}
      <div className="w-full flex justify-center mb-4">
        <div className="bg-yellow-100 border border-yellow-400 text-yellow-800 px-4 py-2 rounded shadow text-center font-semibold">
          Código de descuento: <span className="font-bold">OFERTA2X1</span> — ¡Utilízalo en el pago del carrito!
        </div>
      </div>

      {/* Productos destacados / Últimos vistos */}
      <div className="mb-8">
        <h3 className="text-xl font-bold mb-4 text-ferremas-primary">Productos Destacados</h3>
        <div className="overflow-x-auto">
          <div className="flex space-x-4">
            {productos.slice(0, 6).map((prod) => (
              <div
                key={prod.id}
                className="bg-white rounded-lg shadow-md p-4 min-w-[180px] flex flex-col items-center cursor-pointer transition hover:scale-105"
                onClick={() => navigate(`/producto/${prod.id}`)}
              >
                <img
                  src={prod.imagenUrl && !prod.imagenUrl.startsWith('http') ? `${BASE_URL}${prod.imagenUrl}` : prod.imagenUrl || '/placeholder.png'}
                  alt={prod.nombre}
                  className="h-20 object-contain mb-2"
                  onError={e => { e.currentTarget.src = '/placeholder.png'; }}
                />
                <span className="text-ferremas-primary font-bold text-sm mb-1">{prod.nombre}</span>
                <span className="text-xs text-gray-500 mb-1">{prod.marcaNombre || 'Sin marca'}</span>
                <span className="text-ferremas-primary font-semibold mb-1">
                  {prod.precioConDescuento != null && prod.precioOriginal != null && prod.precioConDescuento < prod.precioOriginal ? (
                    <>
                      <span className="text-gray-500 line-through mr-2 text-sm">
                        ${prod.precioOriginal?.toLocaleString()}
                      </span>
                      <span className="text-ferremas-primary font-bold text-lg">
                        ${prod.precioConDescuento?.toLocaleString()}
                      </span>
                    </>
                  ) : (
                    <span className="text-ferremas-primary font-bold text-lg">
                      ${prod.precioOriginal?.toLocaleString()}
                    </span>
                  )}
                </span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Especiales de temporada */}
      <div className="mb-8">
        <h3 className="text-xl font-bold mb-4 text-ferremas-primary">Especiales de temporada</h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {especialesTemporada.map((esp) => (
            <div
              key={esp.titulo}
              className={`rounded-lg shadow-md p-6 flex flex-col items-start cursor-pointer transition hover:scale-[1.02] ${esp.color}`}
              onClick={() => navigate(`/catalogo?categoria=${encodeURIComponent(esp.titulo)}`)}
            >
              <div className="flex items-center mb-2">
                <img src={esp.img} alt={esp.titulo} className="w-12 h-12 object-contain mr-3" />
                <span className="text-lg font-bold text-ferremas-primary">{esp.titulo}</span>
              </div>
              <span className="text-sm text-gray-700 mb-2">{esp.desc}</span>
              <span className="bg-ferremas-primary text-white font-bold px-3 py-1 rounded">Hasta {esp.ahorro} de ahorro</span>
            </div>
          ))}
        </div>
      </div>

      {/* Header con título, carrito y mi cuenta */}
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold text-ferremas-primary">Catálogo de Productos</h1>
        <div className="flex items-center space-x-4">
          {isAuthenticated && (
            <button
              onClick={() => navigate('/mi-cuenta')}
              className="bg-ferremas-secondary text-white font-semibold px-4 py-2 rounded shadow hover:bg-ferremas-secondary-dark transition"
            >
              Mi Cuenta
            </button>
          )}
        </div>
      </div>

      {/* Filtros y búsqueda */}
      <div className="filters-container">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
          {/* Búsqueda */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Buscar productos
            </label>
            <input
              type="text"
              value={busqueda}
              onChange={(e) => setBusqueda(e.target.value)}
              placeholder="Nombre, descripción o código..."
              className="input-field focus-ring"
            />
          </div>

          {/* Filtro por categoría */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Categoría
            </label>
            <select
              value={categoriaFiltro}
              onChange={(e) => setCategoriaFiltro(e.target.value)}
              className="input-field focus-ring"
            >
              <option value="">Todas las categorías</option>
              {categorias.map(categoria => (
                <option key={categoria} value={categoria}>{categoria}</option>
              ))}
            </select>
          </div>

          {/* Filtro por precio mínimo */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Precio mínimo
            </label>
            <input
              type="number"
              value={precioMin}
              onChange={(e) => setPrecioMin(e.target.value)}
              placeholder="0"
              className="input-field focus-ring"
            />
          </div>

          {/* Filtro por precio máximo */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Precio máximo
            </label>
            <input
              type="number"
              value={precioMax}
              onChange={(e) => setPrecioMax(e.target.value)}
              placeholder="Sin límite"
              className="input-field focus-ring"
            />
          </div>
        </div>

        <div className="flex justify-between items-center">
          <div className="flex items-center space-x-4">
            <label className="text-sm font-medium text-gray-700">Ordenar por:</label>
            <select
              value={ordenarPor}
              onChange={(e) => setOrdenarPor(e.target.value)}
              className="input-field focus-ring"
            >
              <option value="nombre">Nombre</option>
              <option value="precio-asc">Precio: Menor a Mayor</option>
              <option value="precio-desc">Precio: Mayor a Menor</option>
              <option value="stock">Stock</option>
            </select>
          </div>

          <button
            onClick={limpiarFiltros}
            className="text-ferremas-primary hover:text-ferremas-primary-dark font-medium transition-colors duration-200"
          >
            Limpiar filtros
          </button>
        </div>
      </div>

      {/* Resultados */}
      <div className="mb-4">
        <p className="text-gray-600">
          Mostrando {productosFiltrados.length} de {productos.length} productos
        </p>
      </div>

      {/* Renderizado de productos por categoría en grid */}
      <div id="productos" className="mt-8 space-y-12">
        {Object.entries(productosPorCategoria).map(([categoria, productosCat]) => (
          <div key={categoria}>
            <h2 className="text-2xl font-bold mb-4 text-ferremas-primary border-b-2 border-ferremas-primary pb-2">{categoria}</h2>
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
              {productosCat.map(producto => (
                <div
                  key={producto.id}
                  className="bg-white rounded-lg shadow-md overflow-hidden flex flex-col items-center hover:scale-105 transition-transform cursor-pointer"
                  onClick={() => navigate(`/producto/${producto.id}`)}
                >
                  <img
                    src={producto.imagenUrl && !producto.imagenUrl.startsWith('http') ? `${BASE_URL}${producto.imagenUrl}` : producto.imagenUrl || '/placeholder.png'}
                    alt={producto.nombre}
                    className="h-48 object-contain mb-4"
                    onError={e => { e.currentTarget.src = '/placeholder.png'; }}
                  />
                  <div className="p-4 w-full">
                    <h2 className="text-xl font-bold mb-2">{producto.nombre}</h2>
                    <p className="text-gray-600 mb-2">{producto.descripcion}</p>
                    {producto.precioConDescuento != null && producto.precioOriginal != null && producto.precioConDescuento < producto.precioOriginal ? (
                      <div className="mb-2">
                        <span className="text-gray-500 line-through mr-2 text-base">
                          ${producto.precioOriginal?.toLocaleString()}
                        </span>
                        <span className="text-ferremas-primary font-bold text-xl">
                          ${producto.precioConDescuento?.toLocaleString()}
                        </span>
                      </div>
                    ) : (
                      <div className="mb-2">
                        <span className="text-ferremas-primary font-bold text-xl">
                          ${producto.precioOriginal?.toLocaleString()}
                        </span>
                      </div>
                    )}
                    <AddToCartButton 
                      productoId={producto.id}
                      productoNombre={producto.nombre}
                      stockDisponible={producto.stock}
                      precio={producto.precio}
                      imagenUrl={producto.imagenUrl}
                      onSuccess={handleProductoAgregado}
                    />
                  </div>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>

      {/* Carrito modal */}
      <Carrito
        isOpen={carritoAbierto}
        onClose={() => setCarritoAbierto(false)}
        onCheckout={() => {
          setCarritoAbierto(false);
          setCheckoutAbierto(true);
        }}
      />
    </div>
  );
};

export default CatalogoProductos; 