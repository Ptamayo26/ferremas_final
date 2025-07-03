import React, { useState, useEffect } from 'react';
import { api } from '../../../services/api';

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

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: name === 'precio' || name === 'stock' ? Number(value) : name === 'marcaId' || name === 'categoriaId' ? Number(value) : value,
    }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(form);
  };

  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-40">
      <div className="bg-white rounded-lg shadow-lg w-full max-w-md p-6 relative">
        <button
          className="absolute top-2 right-2 text-gray-400 hover:text-gray-700"
          onClick={onClose}
        >
          ×
        </button>
        <h2 className="text-xl font-bold mb-4">{initialData ? 'Editar Producto' : 'Agregar Producto'}</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
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
          <div className="flex justify-end gap-2 mt-4">
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
        </form>
      </div>
    </div>
  );
};

export default ProductoForm; 