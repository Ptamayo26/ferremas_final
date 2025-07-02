import React, { useState, useEffect } from 'react';
import { RolUsuario } from '../../types/api';
import type { UsuarioResponseDTO, UsuarioCreateDTO } from '../../types/api';

interface UsuarioFormProps {
  usuario: UsuarioResponseDTO | null;
  onSave: (data: UsuarioCreateDTO | Partial<UsuarioResponseDTO>) => void;
  onCancel: () => void;
  isLoading: boolean;
  error: string | null;
}

const UsuarioForm: React.FC<UsuarioFormProps> = ({ usuario, onSave, onCancel, isLoading, error }) => {
  const [formData, setFormData] = useState({
    nombre: '',
    apellido: '',
    email: '',
    rut: '',
    rol: 'vendedor' as string, // Default rol
    password: '',
  });

  const isEditMode = usuario !== null;

  useEffect(() => {
    if (isEditMode && usuario) {
      setFormData({
        nombre: usuario.nombre || '',
        apellido: usuario.apellido || '',
        email: usuario.email || '',
        rut: usuario.rut || '',
        rol: usuario.rol || 'vendedor',
        password: '', // Password no se precarga por seguridad
      });
    }
  }, [usuario, isEditMode]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const dataToSend = {
      ...formData,
      rol: formData.rol as RolUsuario,
    };

    if (isEditMode) {
      onSave({ ...dataToSend, id: usuario?.id });
    } else {
      onSave(dataToSend);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
      <div className="bg-white p-8 rounded-lg shadow-xl w-full max-w-md">
        <h2 className="text-2xl font-bold text-ferremas-primary mb-6">
          {isEditMode ? 'Editar Usuario' : 'Crear Nuevo Usuario'}
        </h2>
        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mb-4" role="alert">
            <strong className="font-bold">Error: </strong>
            <span className="block sm:inline">{error}</span>
          </div>
        )}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="nombre">Nombre</label>
            <input type="text" name="nombre" value={formData.nombre} onChange={handleChange} required className="input-field" />
          </div>
          <div>
            <label htmlFor="apellido">Apellido</label>
            <input type="text" name="apellido" value={formData.apellido} onChange={handleChange} className="input-field" />
          </div>
          <div>
            <label htmlFor="email">Email</label>
            <input type="email" name="email" value={formData.email} onChange={handleChange} required className="input-field" />
          </div>
          <div>
            <label htmlFor="rut">RUT</label>
            <input type="text" name="rut" value={formData.rut} onChange={handleChange} className="input-field" />
          </div>
          <div>
            <label htmlFor="rol">Rol</label>
            <select name="rol" value={formData.rol} onChange={handleChange} className="input-field">
              <option value="administrador">Administrador</option>
              <option value="vendedor">Vendedor</option>
              <option value="bodeguero">Bodeguero</option>
              <option value="contador">Contador</option>
            </select>
          </div>
          {!isEditMode && (
            <div>
              <label htmlFor="password">Contraseña</label>
              <input type="password" name="password" value={formData.password} onChange={handleChange} required={!isEditMode} className="input-field" />
            </div>
          )}
          <div className="pt-4 flex justify-end space-x-4">
            <button type="button" onClick={onCancel} className="btn-secondary">
              Cancelar
            </button>
            <button type="submit" className="btn-primary" disabled={isLoading}>
              {isLoading ? 'Guardando...' : 'Guardar Cambios'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default UsuarioForm; 