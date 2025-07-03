import React, { useEffect, useState } from 'react';
import { api, apiClient } from '../../../services/api';
import ProductoForm from './ProductoForm';
import type { ProductoFormData } from './ProductoForm';
import type { ProductoResponseDTO } from '../../../types/api';

const CatalogoProductosAdmin: React.FC = () => {
  const [productos, setProductos] = useState<ProductoResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editData, setEditData] = useState<ProductoResponseDTO | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [mensajeCodigos, setMensajeCodigos] = useState<string | null>(null);

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
        </div>
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