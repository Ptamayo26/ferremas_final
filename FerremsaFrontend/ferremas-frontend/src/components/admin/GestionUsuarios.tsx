import React, { useState, useEffect } from 'react';
import { apiClient } from '../../services/api';
import type { UsuarioResponseDTO, UsuarioCreateDTO } from '../../types/api';
import UsuarioForm from './UsuarioForm';

const GestionUsuarios: React.FC = () => {
  const [usuarios, setUsuarios] = useState<UsuarioResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedUsuario, setSelectedUsuario] = useState<UsuarioResponseDTO | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  const fetchUsuarios = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get<UsuarioResponseDTO[]>('/api/Usuarios');
      setUsuarios(response.data || []);
      setError(null);
    } catch (err) {
      setError('No se pudo cargar la lista de usuarios. Por favor, inténtelo de nuevo más tarde.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsuarios();
  }, []);

  const handleCreate = () => {
    setSelectedUsuario(null);
    setFormError(null);
    setIsModalOpen(true);
  };

  const handleEdit = (usuario: UsuarioResponseDTO) => {
    setSelectedUsuario(usuario);
    setFormError(null);
    setIsModalOpen(true);
  };

  const handleDelete = async (userId: number) => {
    if (window.confirm('¿Estás seguro de que quieres desactivar este usuario?')) {
      try {
        await apiClient.delete(`/api/Usuarios/${userId}`);
        fetchUsuarios();
      } catch (err: any) {
        console.error('Error al desactivar usuario:', err);
        setError(err.details || 'No se pudo desactivar el usuario.');
      }
    }
  };

  const handleSave = async (data: UsuarioCreateDTO | Partial<UsuarioResponseDTO>) => {
    setIsSaving(true);
    setFormError(null);
    try {
      if ('id' in data && data.id) {
        await apiClient.put(`/api/Usuarios/${data.id}`, data);
      } else {
        await apiClient.post('/api/Usuarios', data);
      }
      setIsModalOpen(false);
      fetchUsuarios();
    } catch (err: any) {
      console.error('Error al guardar usuario:', err);
      const errorMessage = err.details || err.message || 'No se pudo guardar el usuario.';
      setFormError(errorMessage);
    } finally {
      setIsSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="animate-spin text-4xl">⚙️</div>
        <p className="ml-4 text-lg">Cargando usuarios...</p>
      </div>
    );
  }

  if (error) {
    return <div className="p-4 text-center text-red-600 bg-red-100 rounded-lg">{error}</div>;
  }

  return (
    <div className="card p-6">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold text-ferremas-primary">
          Gestión de Usuarios
        </h2>
        <button onClick={handleCreate} className="btn-primary">
          <span className="mr-2">➕</span>
          Crear Usuario
        </button>
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-ferremas-gray-200">
          <thead className="bg-ferremas-gray-50">
            <tr>
              <th className="th-cell">Nombre</th>
              <th className="th-cell">Email</th>
              <th className="th-cell">RUT</th>
              <th className="th-cell">Rol</th>
              <th className="th-cell">Estado</th>
              <th className="th-cell">Acciones</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-ferremas-gray-200">
            {usuarios.map((usuario) => (
              <tr key={usuario.id} className="hover:bg-ferremas-gray-50">
                <td className="td-cell">
                  {usuario.nombre} {usuario.apellido}
                </td>
                <td className="td-cell">{usuario.email}</td>
                <td className="td-cell">{usuario.rut}</td>
                <td className="td-cell">
                  <span className="badge-info capitalize">{usuario.rol}</span>
                </td>
                <td className="td-cell">
                  <span className={`badge-${usuario.activo ? 'success' : 'danger'}`}>
                    {usuario.activo ? 'Activo' : 'Inactivo'}
                  </span>
                </td>
                <td className="td-cell">
                  <div className="flex space-x-2">
                    <button onClick={() => handleEdit(usuario)} className="text-ferremas-blue-600 hover:text-ferremas-blue-800">
                      ✏️
                    </button>
                    <button onClick={() => handleDelete(usuario.id)} className="text-ferremas-danger hover:text-red-700">
                      🗑️
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {isModalOpen && (
        <UsuarioForm
          usuario={selectedUsuario}
          onSave={handleSave}
          onCancel={() => setIsModalOpen(false)}
          isLoading={isSaving}
          error={formError}
        />
      )}
    </div>
  );
};

export default GestionUsuarios; 