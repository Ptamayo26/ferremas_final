import React, { useState } from 'react';
import { apiClient } from '../../services/api';

interface CambiarPasswordModalProps {
  open: boolean;
  onClose: () => void;
}

const CambiarPasswordModal: React.FC<CambiarPasswordModalProps> = ({ open, onClose }) => {
  const [form, setForm] = useState({ actual: '', nueva: '', repetir: '' });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  if (!open) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(false);
    if (form.nueva !== form.repetir) {
      setError('Las contraseñas no coinciden');
      setLoading(false);
      return;
    }
    try {
      await apiClient.post('/api/usuarios/cambiar-password', { actual: form.actual, nueva: form.nueva });
      setSuccess(true);
      setForm({ actual: '', nueva: '', repetir: '' });
    } catch {
      setError('Error al cambiar la contraseña');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-30 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <h2 className="text-lg font-bold mb-4">Cambiar Contraseña</h2>
        <form onSubmit={handleSubmit} className="space-y-3">
          <input name="actual" value={form.actual} onChange={handleChange} className="input-field w-full" placeholder="Contraseña actual" type="password" required />
          <input name="nueva" value={form.nueva} onChange={handleChange} className="input-field w-full" placeholder="Nueva contraseña" type="password" required />
          <input name="repetir" value={form.repetir} onChange={handleChange} className="input-field w-full" placeholder="Repetir nueva contraseña" type="password" required />
          {error && <div className="text-red-500 text-sm">{error}</div>}
          {success && <div className="text-green-600 text-sm">Contraseña cambiada correctamente</div>}
          <div className="flex justify-end gap-2 mt-4">
            <button type="button" className="btn-secondary" onClick={onClose}>Cancelar</button>
            <button type="submit" className="btn-primary" disabled={loading}>{loading ? 'Guardando...' : 'Guardar'}</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CambiarPasswordModal; 