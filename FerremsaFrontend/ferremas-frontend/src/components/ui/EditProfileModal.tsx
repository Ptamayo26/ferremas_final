import React, { useState } from 'react';
import { apiClient } from '../../services/api';

interface EditProfileModalProps {
  open: boolean;
  onClose: () => void;
  perfil: any;
  onUpdated: (perfil: any) => void;
}

const EditProfileModal: React.FC<EditProfileModalProps> = ({ open, onClose, perfil, onUpdated }) => {
  const [form, setForm] = useState({
    nombre: perfil?.nombre || '',
    apellido: perfil?.apellido || '',
    telefono: perfil?.telefono || '',
    correoElectronico: perfil?.correoElectronico || ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!open) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const res = await apiClient.put(`/api/clientes/${perfil.id}`, form);
      onUpdated(res.data);
      onClose();
    } catch (err: any) {
      setError('Error al actualizar el perfil');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <h2 className="text-lg font-bold mb-4">Editar Perfil</h2>
        <form onSubmit={handleSubmit} className="space-y-3">
          <input name="nombre" value={form.nombre} onChange={handleChange} className="input-field w-full" placeholder="Nombre" required />
          <input name="apellido" value={form.apellido} onChange={handleChange} className="input-field w-full" placeholder="Apellido" />
          <input name="telefono" value={form.telefono} onChange={handleChange} className="input-field w-full" placeholder="Teléfono" />
          <input name="correoElectronico" value={form.correoElectronico} onChange={handleChange} className="input-field w-full" placeholder="Correo" type="email" required />
          {error && <div className="text-red-500 text-sm">{error}</div>}
          <div className="flex justify-end gap-2 mt-4">
            <button type="button" className="btn-secondary" onClick={onClose}>Cancelar</button>
            <button type="submit" className="btn-primary" disabled={loading}>{loading ? 'Guardando...' : 'Guardar'}</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditProfileModal; 