import React, { useState, useEffect } from 'react';
import { api, apiClient } from '../../../services/api';

interface ProductoFormProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: ProductoFormData) => void;
  initialData?: ProductoFormData;
}

export interface ProductoFormData {
  nombre: string;
  codigo: string;
  categoriaId?: number;
  precio: number;
  stock: number;
  marcaId?: number;
  descripcion?: string;
  imagenUrl?: string;
}

const ProductoForm: React.FC<ProductoFormProps> = ({ open, onClose, onSubmit, initialData }) => {
  const [form, setForm] = useState<ProductoFormData>(
    initialData || {
      nombre: '',
      codigo: '',
      categoriaId: undefined,
      precio: 0,
      stock: 0,
      marcaId: undefined,
    }
  );
  const [marcas, setMarcas] = useState<{ id: number; nombre: string }[]>([]);
  const [categorias, setCategorias] = useState<{ id: number; nombre: string }[]>([]);
  const [imagen, setImagen] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(initialData?.imagenUrl || null);
  const [subiendoImagen, setSubiendoImagen] = useState(false);

  useEffect(() => {
    if (initialData) {
      setForm(initialData);
    } else {
      setForm({ nombre: '', codigo: '', categoriaId: undefined, precio: 0, stock: 0, marcaId: undefined });
    }
  }, [initialData, open]);

  useEffect(() => {
    if (open) {
      api.getMarcas().then(setMarcas);
      api.getCategorias().then(setCategorias);
    }
  }, [open]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: name === 'precio' || name === 'stock' ? Number(value) : name === 'marcaId' || name === 'categoriaId' ? Number(value) : value,
    }));
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    setImagen(file || null);
    if (file) {
      setPreviewUrl(URL.createObjectURL(file));
    } else {
      setPreviewUrl(null);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    let imagenUrl = initialData?.imagenUrl || '';
    if (imagen) {
      setSubiendoImagen(true);
      const formData = new FormData();
      formData.append('file', imagen);
      try {
        const res = await apiClient.post('/api/upload/producto', formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        });
        imagenUrl = res.data.url;
      } catch (err) {
        alert('Error al subir la imagen');
        setSubiendoImagen(false);
        return;
      }
      setSubiendoImagen(false);
    }
    onSubmit({ ...form, imagenUrl });
  };

  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-40 p-2">
      <div className="bg-white rounded-lg shadow-lg w-full max-w-md md:max-w-lg lg:max-w-xl max-h-[90vh] flex flex-col relative">
        <button
          className="absolute top-2 right-2 text-gray-400 hover:text-gray-700 z-10"
          onClick={onClose}
        >
          ×
        </button>
        <h2 className="text-xl font-bold mb-4 px-6 pt-6">{initialData ? 'Editar Producto' : 'Agregar Producto'}</h2>
        <form onSubmit={handleSubmit} className="flex-1 flex flex-col overflow-y-auto px-6 pb-4 space-y-4">
          {form.codigo && (
            <div>
              <label className="block text-sm font-medium mb-1">Código asignado</label>
              <input
                type="text"
                name="codigo"
                value={form.codigo}
                readOnly
                className="w-full border rounded px-3 py-2 bg-gray-100 text-gray-700 cursor-not-allowed"
              />
            </div>
          )}
          <div>
            <label className="block text-sm font-medium mb-1">Nombre</label>
            <input
              type="text"
              name="nombre"
              value={form.nombre}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Descripción / Reseña</label>
            <textarea
              name="descripcion"
              value={form.descripcion || ''}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
              rows={3}
              placeholder="Agrega una reseña o descripción del producto"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Marca</label>
            <select
              name="marcaId"
              value={form.marcaId ?? ''}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
              required
            >
              <option value="">Seleccione una marca</option>
              {marcas.map((marca) => (
                <option key={marca.id} value={marca.id}>{marca.nombre}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Categoría</label>
            <select
              name="categoriaId"
              value={form.categoriaId ?? ''}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
              required
            >
              <option value="">Seleccione una categoría</option>
              {categorias.map((cat) => (
                <option key={cat.id} value={cat.id}>{cat.nombre}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Precio</label>
            <input
              type="number"
              name="precio"
              value={form.precio}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
              min={0}
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Stock</label>
            <input
              type="number"
              name="stock"
              value={form.stock}
              onChange={handleChange}
              className="w-full border rounded px-3 py-2"
              min={0}
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Imagen</label>
            <input
              type="file"
              accept="image/*"
              onChange={handleImageChange}
            />
            {previewUrl && (
              <img src={previewUrl} alt="Previsualización" className="w-32 h-32 object-contain mt-2" />
            )}
            {subiendoImagen && <div className="text-sm text-blue-600 mt-1">Subiendo imagen...</div>}
          </div>
        </form>
        <div className="flex justify-end gap-2 mt-4 sticky bottom-0 bg-white pt-4 pb-2 z-10">
          <button
            type="button"
            className="px-4 py-2 rounded bg-gray-200 hover:bg-gray-300"
            onClick={onClose}
          >
            Cancelar
          </button>
          <button
            type="submit"
            className="px-4 py-2 rounded bg-blue-600 text-white hover:bg-blue-700"
          >
            {initialData ? 'Guardar Cambios' : 'Agregar'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default ProductoForm; 