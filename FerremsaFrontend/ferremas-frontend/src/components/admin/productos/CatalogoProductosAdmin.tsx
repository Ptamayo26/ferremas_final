import React, { useEffect, useState } from 'react';
import { api, apiClient } from '../../../services/api';
import ProductoForm from './ProductoForm';
import type { ProductoFormData } from './ProductoForm';
import type { ProductoResponseDTO } from '../../../types/api';
import { STORAGE_KEYS } from '../../../constants/api';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5200';
const getImagenUrl = (url: string) => {
  if (!url) return '';
  if (url.startsWith('http')) return url;
  return API_URL + url;
};

const CatalogoProductosAdmin: React.FC = () => {
  const [productos, setProductos] = useState<ProductoResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editData, setEditData] = useState<ProductoResponseDTO | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [mensajeCodigos, setMensajeCodigos] = useState<string | null>(null);
  // Estado para carga masiva
  const [csvPreview, setCsvPreview] = useState<any[]>([]);
  const [csvErrores, setCsvErrores] = useState<string[]>([]);
  const [csvFile, setCsvFile] = useState<File | null>(null);
  const [csvLoading, setCsvLoading] = useState(false);
  const [csvSuccess, setCsvSuccess] = useState<string | null>(null);

  const cargarProductos = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await api.getProductos();
      setProductos(data);
    } catch (err) {
      setError('Error al cargar productos');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    cargarProductos();
  }, []);

  const handleAgregarProducto = async (form: ProductoFormData) => {
    setError(null);
    try {
      const nuevoProducto = {
        codigo: '', // El backend lo asigna
        nombre: form.nombre,
        descripcion: 'Sin descripción',
        precio: form.precio,
        stock: form.stock,
        categoriaId: 1,
        marcaId: 1,
        imagenUrl: null,
        especificaciones: null,
      };
      const response = await apiClient.post('/api/Productos', nuevoProducto);
      const productoCreado = response.data;
      await cargarProductos();
      // Buscar el producto recién creado en la lista (por ID)
      const producto = await api.getProductoById(productoCreado.id);
      setEditData(producto);
      setShowForm(true); // Abrir modal en modo edición para mostrar el código
    } catch (err) {
      setError('Error al agregar producto');
    }
  };

  const handleEditarProducto = (producto: ProductoResponseDTO) => {
    setEditData(producto);
    setShowForm(true);
  };

  const handleActualizarProducto = async (form: ProductoFormData) => {
    if (!editData) return;
    setError(null);
    try {
      const actualizado = {
        ...editData,
        nombre: form.nombre,
        precio: form.precio,
        stock: form.stock,
        marcaId: form.marcaId,
        categoriaId: form.categoriaId,
        imagenUrl: form.imagenUrl,
        // otros campos si es necesario
      };
      await apiClient.put(`/api/Productos/${editData.id}`, actualizado);
      setShowForm(false);
      setEditData(null);
      cargarProductos();
    } catch (err) {
      setError('Error al actualizar producto');
    }
  };

  const actualizarCodigos = async () => {
    setMensajeCodigos(null);
    setError(null);
    try {
      const response = await apiClient.post('/api/Productos/actualizar-codigos');
      setMensajeCodigos(response.data.mensaje || 'Códigos actualizados correctamente');
      await cargarProductos();
    } catch (err) {
      setError('Error al actualizar los códigos');
    }
  };

  // Descargar plantilla CSV (descarga directa)
  const descargarPlantilla = async () => {
    try {
      const token = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
      const response = await fetch(`${API_URL}/api/Productos/descargar-plantilla-csv`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
        credentials: 'include',
      });
      if (!response.ok) throw new Error('No se pudo descargar la plantilla CSV');
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'plantilla_productos.csv';
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      setError('No se pudo descargar la plantilla CSV');
    }
  };

  // Descargar plantilla Excel con listas desplegables
  const descargarPlantillaExcel = async () => {
    try {
      const token = localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
      const response = await fetch(`${API_URL}/api/Productos/descargar-plantilla-excel`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
        credentials: 'include',
      });
      if (!response.ok) throw new Error('No se pudo descargar la plantilla Excel');
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'plantilla_productos.xlsx';
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      setError('No se pudo descargar la plantilla Excel');
    }
  };

  // Previsualizar CSV
  const previsualizarCsv = async (file: File) => {
    setCsvLoading(true);
    setCsvErrores([]);
    setCsvPreview([]);
    setCsvSuccess(null);
    const formData = new FormData();
    formData.append('archivoCsv', file);
    try {
      const response = await apiClient.post('/api/Productos/previsualizar-carga-csv', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      setCsvPreview(response.data.productos);
      setCsvErrores(response.data.errores || []);
    } catch (err: any) {
      setCsvErrores([err.response?.data || 'Error al previsualizar el archivo CSV']);
    } finally {
      setCsvLoading(false);
    }
  };

  // Confirmar y guardar carga masiva
  const guardarCsv = async () => {
    if (!csvFile) return;
    setCsvLoading(true);
    setCsvErrores([]);
    setCsvSuccess(null);
    const formData = new FormData();
    formData.append('archivoCsv', csvFile);
    try {
      const response = await apiClient.post('/api/Productos/guardar-carga-csv', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      setCsvSuccess(response.data.mensaje || 'Productos guardados exitosamente');
      setCsvPreview([]);
      setCsvFile(null);
      cargarProductos();
    } catch (err: any) {
      setCsvErrores(err.response?.data?.errores || [err.response?.data || 'Error al guardar productos']);
    } finally {
      setCsvLoading(false);
    }
  };

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Catálogo de Productos</h1>
        <div className="flex gap-2">
          <button
            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
            onClick={() => { setShowForm(true); setEditData(null); }}
          >
            + Agregar Producto
          </button>
          <button
            className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700 transition-colors"
            onClick={actualizarCodigos}
          >
            Actualizar Códigos
          </button>
          {/* <button
            className="bg-gray-700 text-white px-4 py-2 rounded hover:bg-gray-800 transition-colors"
            onClick={descargarPlantilla}
          >
            Descargar Plantilla CSV
          </button> */}
          <button
            className="bg-purple-600 text-white px-4 py-2 rounded hover:bg-purple-700 transition-colors"
            onClick={descargarPlantillaExcel}
          >
            Descargar Plantilla Excel
          </button>
        </div>
      </div>
      {/* Carga masiva CSV */}
      <div className="mb-4 p-4 border rounded bg-gray-50">
        <h2 className="font-semibold mb-2">Carga Masiva de Productos</h2>
        <input
          type="file"
          accept=".csv,.xlsx"
          onChange={e => {
            if (e.target.files && e.target.files[0]) {
              setCsvFile(e.target.files[0]);
              previsualizarCsv(e.target.files[0]);
            }
          }}
          className="mb-2"
        />
        {csvLoading && <div className="text-blue-600">Procesando archivo...</div>}
        {csvErrores.length > 0 && (
          <div className="text-red-600 mb-2">
            <b>Errores:</b>
            <ul className="list-disc ml-6">
              {(csvErrores as string[]).map((err, i) => <li key={i}>{err}</li>)}
            </ul>
          </div>
        )}
        {csvPreview.length > 0 && (
          <div className="overflow-x-auto mb-2">
            <table className="min-w-full bg-white border border-gray-200 rounded shadow">
              <thead>
                <tr>
                  {Object.keys(csvPreview[0]).map((col) => (
                    <th key={col} className="px-2 py-1 border-b">{col}</th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {csvPreview.map((row, i) => (
                  <tr key={i} className="hover:bg-gray-50">
                    {Object.values(row).map((val, j) => (
                      <td key={j} className="px-2 py-1 border-b">{val}</td>
                    ))}
                  </tr>
                ))}
              </tbody>
            </table>
            <button
              className="mt-2 bg-green-700 text-white px-4 py-2 rounded hover:bg-green-800 transition-colors"
              onClick={guardarCsv}
              disabled={csvLoading}
            >
              Confirmar y Guardar Productos
            </button>
          </div>
        )}
        {csvSuccess && <div className="text-green-700 mb-2">{csvSuccess}</div>}
      </div>
      {error && <div className="text-red-600 mb-2">{error}</div>}
      {mensajeCodigos && <div className="text-green-700 mb-2">{mensajeCodigos}</div>}
      {loading ? (
        <div>Cargando productos...</div>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full bg-white border border-gray-200 rounded shadow">
            <thead>
              <tr>
                <th className="px-4 py-2 border-b">Imagen</th>
                <th className="px-4 py-2 border-b">Nombre</th>
                <th className="px-4 py-2 border-b">Código</th>
                <th className="px-4 py-2 border-b">Categoría</th>
                <th className="px-4 py-2 border-b">Precio</th>
                <th className="px-4 py-2 border-b">Stock</th>
                <th className="px-4 py-2 border-b">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {productos.map((producto) => (
                <tr key={producto.id} className="hover:bg-gray-50">
                  <td className="px-4 py-2 border-b">
                    {producto.imagenUrl && producto.imagenUrl !== 'Sin imagen' ? (
                      <img src={getImagenUrl(producto.imagenUrl)} alt={producto.nombre} className="w-16 h-16 object-contain mx-auto" />
                    ) : (
                      <span className="text-gray-400">Sin imagen</span>
                    )}
                  </td>
                  <td className="px-4 py-2 border-b">{producto.nombre}</td>
                  <td className="px-4 py-2 border-b">{producto.codigo}</td>
                  <td className="px-4 py-2 border-b">{producto.categoriaNombre || '-'}</td>
                  <td className="px-4 py-2 border-b">${producto.precio.toLocaleString()}</td>
                  <td className="px-4 py-2 border-b">{producto.stock}</td>
                  <td className="px-4 py-2 border-b">
                    <button className="text-blue-600 hover:underline mr-2" onClick={() => handleEditarProducto(producto)}>Editar</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      <ProductoForm
        open={showForm}
        onClose={() => { setShowForm(false); setEditData(null); }}
        onSubmit={editData ? handleActualizarProducto : handleAgregarProducto}
        initialData={editData ? {
          nombre: editData.nombre,
          codigo: editData.codigo,
          categoriaId: editData.categoriaId,
          precio: editData.precio,
          stock: editData.stock,
          marcaId: editData.marcaId,
        } : undefined}
      />
    </div>
  );
};

export default CatalogoProductosAdmin; 