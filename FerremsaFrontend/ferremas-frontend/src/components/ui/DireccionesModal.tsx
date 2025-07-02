import React, { useState } from 'react';
import { apiClient } from '../../services/api';

interface DireccionesModalProps {
  open: boolean;
  onClose: () => void;
  direcciones: any[];
  onUpdated: (direcciones: any[]) => void;
  usuarioId: number;
}

const DireccionesModal: React.FC<DireccionesModalProps> = ({ open, onClose, direcciones, onUpdated, usuarioId }) => {
  const [list, setList] = useState<any[]>(direcciones);
  const [form, setForm] = useState<any>({ calle: '', numero: '', comuna: '', region: '', departamento: '', codigoPostal: '', esPrincipal: false });
  const [editingIdx, setEditingIdx] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  if (!open) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSave = async () => {
    // Validación de campos obligatorios
    if (!form.calle || !form.numero || !form.comuna || !form.region) {
      setError('Completa todos los campos obligatorios: Calle, Número, Comuna y Región.');
      return;
    }
    try {
      // Siempre enviar los campos requeridos
      const payload = {
        calle: form.calle,
        numero: form.numero,
        comuna: form.comuna,
        region: form.region,
        departamento: form.departamento || '',
        codigoPostal: form.codigoPostal || '',
        esPrincipal: form.esPrincipal || false
      };
      if (editingIdx !== null) {
        const res = await apiClient.put(`/api/direcciones/${list[editingIdx].id}`, payload);
        const updated = [...list];
        updated[editingIdx] = res.data;
        setList(updated);
        setEditingIdx(null);
      } else {
        const res = await apiClient.post(`/api/direcciones/usuario/${usuarioId}`, payload);
        setList([...list, res.data]);
      }
      setForm({ calle: '', numero: '', comuna: '', region: '', departamento: '', codigoPostal: '', esPrincipal: false });
      setError(null);
    } catch (err) {
      setError('Error al guardar la dirección');
    }
  };

  const handleEdit = (idx: number) => {
    setForm(list[idx]);
    setEditingIdx(idx);
  };

  const handleDelete = async (idx: number) => {
    try {
      await apiClient.delete(`/api/direcciones/${list[idx].id}`);
      setList(list.filter((_, i) => i !== idx));
    } catch {
      setError('Error al eliminar la dirección');
    }
  };

  const handleClose = () => {
    onUpdated(list);
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-lg">
        <h2 className="text-lg font-bold mb-4">Gestionar Direcciones</h2>
        <ul className="mb-4">
          {list.map((dir, idx) => (
            <li key={idx} className="mb-2 flex justify-between items-center">
              <span>{dir.calle || dir.Calle} {dir.numero || dir.Numero}, {dir.comuna || dir.Comuna}, {dir.region || dir.Region}</span>
              <div className="flex gap-2">
                <button className="btn-secondary btn-xs" onClick={() => handleEdit(idx)}>Editar</button>
                <button className="btn-danger btn-xs" onClick={() => handleDelete(idx)}>Eliminar</button>
              </div>
            </li>
          ))}
        </ul>
        <div className="mb-2 font-semibold">{editingIdx !== null ? 'Editar Dirección' : 'Agregar Nueva Dirección'}</div>
        <div className="grid grid-cols-2 gap-2 mb-2">
          <input name="calle" value={form.calle} onChange={handleChange} className="input-field" placeholder="Calle" />
          <input name="numero" value={form.numero} onChange={handleChange} className="input-field" placeholder="Número" />
          <input name="departamento" value={form.departamento} onChange={handleChange} className="input-field col-span-2" placeholder="Departamento (opcional)" />
          <input name="comuna" value={form.comuna} onChange={handleChange} className="input-field col-span-2" placeholder="Comuna" />
          <input name="region" value={form.region} onChange={handleChange} className="input-field col-span-2" placeholder="Región" />
          <input name="codigoPostal" value={form.codigoPostal} onChange={handleChange} className="input-field col-span-2" placeholder="Código Postal (opcional)" />
        </div>
        {error && <div className="text-red-500 text-sm mb-2">{error}</div>}
        <div className="flex justify-end gap-2 mt-4">
          <button className="btn-secondary" onClick={handleClose}>Cerrar</button>
          <button className="btn-primary" onClick={handleSave}>{editingIdx !== null ? 'Guardar Cambios' : 'Agregar'}</button>
        </div>
      </div>
    </div>
  );
};

export default DireccionesModal; 