import React, { useState, useEffect } from 'react';
import { apiClient } from '../../services/api';
import type { DireccionDTO } from '../../types/api';

interface DireccionFormProps {
  isOpen: boolean;
  onClose: () => void;
  usuarioId: number;
  direccion?: DireccionDTO;
  onSave: () => void;
}

const DireccionForm: React.FC<DireccionFormProps> = ({ 
  isOpen, 
  onClose, 
  usuarioId, 
  direccion, 
  onSave 
}) => {
  const [formData, setFormData] = useState({
    calle: '',
    numero: '',
    departamento: '',
    comuna: '',
    region: '',
    codigoPostal: '',
    esPrincipal: false
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isEditing = !!direccion;

  useEffect(() => {
    if (direccion) {
      setFormData({
        calle: direccion.calle,
        numero: direccion.numero,
        departamento: direccion.departamento,
        comuna: direccion.comuna,
        region: direccion.region,
        codigoPostal: direccion.codigoPostal,
        esPrincipal: direccion.esPrincipal
      });
    } else {
      setFormData({
        calle: '',
        numero: '',
        departamento: '',
        comuna: '',
        region: '',
        codigoPostal: '',
        esPrincipal: false
      });
    }
    setError(null);
  }, [direccion, isOpen]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      if (isEditing && direccion) {
        await apiClient.put(`/api/Direcciones/${direccion.id}`, formData);
      } else {
        await apiClient.post(`/api/Direcciones/usuario/${usuarioId}`, formData);
      }
      
      onSave();
      onClose();
    } catch (err: any) {
      setError(err.response?.data || 'Error al guardar la dirección');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!direccion || !window.confirm('¿Está seguro de que desea eliminar esta dirección?')) {
      return;
    }

    setLoading(true);
    setError(null);

    try {
      await apiClient.delete(`/api/Direcciones/${direccion.id}`);
      onSave();
      onClose();
    } catch (err: any) {
      setError(err.response?.data || 'Error al eliminar la dirección');
    } finally {
      setLoading(false);
    }
  };

  const handleSetPrincipal = async () => {
    if (!direccion) return;

    setLoading(true);
    setError(null);

    try {
      await apiClient.put(`/api/Direcciones/${direccion.id}/principal`);
      onSave();
    } catch (err: any) {
      setError(err.response?.data || 'Error al marcar como principal');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md mx-4">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-ferremas-primary">
            {isEditing ? 'Editar Dirección' : 'Nueva Dirección'}
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700"
          >
            ✕
          </button>
        </div>

        {error && (
          <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Calle *
            </label>
            <input
              type="text"
              name="calle"
              value={formData.calle}
              onChange={handleInputChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Número *
            </label>
            <input
              type="text"
              name="numero"
              value={formData.numero}
              onChange={handleInputChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Departamento
            </label>
            <input
              type="text"
              name="departamento"
              value={formData.departamento}
              onChange={handleInputChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Comuna *
            </label>
            <input
              type="text"
              name="comuna"
              value={formData.comuna}
              onChange={handleInputChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Región *
            </label>
            <input
              type="text"
              name="region"
              value={formData.region}
              onChange={handleInputChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Código Postal *
            </label>
            <input
              type="text"
              name="codigoPostal"
              value={formData.codigoPostal}
              onChange={handleInputChange}
              required
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
            />
          </div>

          <div className="flex items-center">
            <input
              type="checkbox"
              name="esPrincipal"
              checked={formData.esPrincipal}
              onChange={handleInputChange}
              className="h-4 w-4 text-ferremas-primary focus:ring-ferremas-primary border-gray-300 rounded"
            />
            <label className="ml-2 block text-sm text-gray-900">
              Marcar como dirección principal
            </label>
          </div>

          <div className="flex justify-end space-x-3 pt-4">
            {isEditing && (
              <>
                {!direccion.esPrincipal && (
                  <button
                    type="button"
                    onClick={handleSetPrincipal}
                    disabled={loading}
                    className="btn-secondary"
                  >
                    Marcar Principal
                  </button>
                )}
                <button
                  type="button"
                  onClick={handleDelete}
                  disabled={loading}
                  className="btn-danger"
                >
                  Eliminar
                </button>
              </>
            )}
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              className="btn-secondary"
            >
              Cancelar
            </button>
            <button
              type="submit"
              disabled={loading}
              className="btn-primary"
            >
              {loading ? 'Guardando...' : (isEditing ? 'Actualizar' : 'Crear')}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default DireccionForm; 