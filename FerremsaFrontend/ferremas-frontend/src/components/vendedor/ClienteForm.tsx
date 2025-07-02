import React, { useState, useEffect } from 'react';
import { apiClient } from '../../services/api';
import type { ClienteResponseDTO, ClienteCreateDTO, ClienteUpdateDTO, DireccionDTO } from '../../types/api';

interface ClienteFormProps {
  isOpen: boolean;
  onClose: () => void;
  cliente?: ClienteResponseDTO;
  onSave: () => void;
}

const ClienteForm: React.FC<ClienteFormProps> = ({ 
  isOpen, 
  onClose, 
  cliente, 
  onSave 
}) => {
  const [formData, setFormData] = useState({
    nombre: '',
    apellido: '',
    rut: '',
    correoElectronico: '',
    telefono: ''
  });
  const [direcciones, setDirecciones] = useState<Omit<DireccionDTO, 'id' | 'fechaModificacion'>[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isEditing = !!cliente;

  useEffect(() => {
    if (cliente) {
      setFormData({
        nombre: cliente.nombre,
        apellido: cliente.apellido,
        rut: cliente.rut,
        correoElectronico: cliente.correoElectronico,
        telefono: cliente.telefono || ''
      });
      setDirecciones([]);
    } else {
      setFormData({
        nombre: '',
        apellido: '',
        rut: '',
        correoElectronico: '',
        telefono: ''
      });
      setDirecciones([]);
    }
    setError(null);
  }, [cliente, isOpen]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleDireccionChange = (index: number, field: keyof Omit<DireccionDTO, 'id' | 'fechaModificacion'>, value: string | boolean) => {
    setDirecciones(prev => prev.map((dir, i) => 
      i === index ? { ...dir, [field]: value } : dir
    ));
  };

  const addDireccion = () => {
    setDirecciones(prev => [...prev, {
      calle: '',
      numero: '',
      departamento: '',
      comuna: '',
      region: '',
      codigoPostal: '',
      esPrincipal: prev.length === 0
    }]);
  };

  const removeDireccion = (index: number) => {
    setDirecciones(prev => prev.filter((_, i) => i !== index));
  };

  const setDireccionPrincipal = (index: number) => {
    setDirecciones(prev => prev.map((dir, i) => ({
      ...dir,
      esPrincipal: i === index
    })));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      if (isEditing && cliente) {
        const updateData: ClienteUpdateDTO = {
          nombre: formData.nombre,
          apellido: formData.apellido,
          rut: formData.rut,
          correoElectronico: formData.correoElectronico,
          telefono: formData.telefono || undefined
        };
        await apiClient.put(`/api/Clientes/${cliente.id}`, updateData);
      } else {
        if (direcciones.length === 0) {
          throw new Error('Debe ingresar al menos una dirección para el cliente.');
        }

        const createData: ClienteCreateDTO = {
          nombre: formData.nombre,
          apellido: formData.apellido,
          rut: formData.rut,
          correoElectronico: formData.correoElectronico,
          telefono: formData.telefono || undefined,
          direcciones: direcciones
        };
        await apiClient.post('/api/Clientes', createData);
      }
      
      onSave();
      onClose();
    } catch (err: any) {
      setError(err.response?.data || err.message || 'Error al guardar el cliente');
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-2xl mx-4 max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-ferremas-primary">
            {isEditing ? 'Editar Cliente' : 'Nuevo Cliente'}
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

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Nombre *
              </label>
              <input
                type="text"
                name="nombre"
                value={formData.nombre}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Apellido *
              </label>
              <input
                type="text"
                name="apellido"
                value={formData.apellido}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                RUT *
              </label>
              <input
                type="text"
                name="rut"
                value={formData.rut}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Email *
              </label>
              <input
                type="email"
                name="correoElectronico"
                value={formData.correoElectronico}
                onChange={handleInputChange}
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Teléfono
              </label>
              <input
                type="tel"
                name="telefono"
                value={formData.telefono}
                onChange={handleInputChange}
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
              />
            </div>
          </div>

          {!isEditing && (
            <div className="border-t pt-4">
              <div className="flex justify-between items-center mb-4">
                <h3 className="text-lg font-semibold text-gray-800">Direcciones</h3>
                <button
                  type="button"
                  onClick={addDireccion}
                  className="btn-secondary"
                >
                  + Agregar Dirección
                </button>
              </div>

              {direcciones.length === 0 && (
                <p className="text-gray-500 text-sm">Debe agregar al menos una dirección para el cliente.</p>
              )}

              {direcciones.map((direccion, index) => (
                <div key={index} className="border rounded-lg p-4 mb-4">
                  <div className="flex justify-between items-center mb-3">
                    <h4 className="font-medium">Dirección {index + 1}</h4>
                    <div className="flex space-x-2">
                      <button
                        type="button"
                        onClick={() => setDireccionPrincipal(index)}
                        className={`text-sm px-2 py-1 rounded ${
                          direccion.esPrincipal 
                            ? 'bg-ferremas-primary text-white' 
                            : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                        }`}
                      >
                        {direccion.esPrincipal ? 'Principal' : 'Marcar Principal'}
                      </button>
                      {direcciones.length > 1 && (
                        <button
                          type="button"
                          onClick={() => removeDireccion(index)}
                          className="text-sm px-2 py-1 bg-red-100 text-red-700 rounded hover:bg-red-200"
                        >
                          Eliminar
                        </button>
                      )}
                    </div>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Calle *
                      </label>
                      <input
                        type="text"
                        value={direccion.calle}
                        onChange={(e) => handleDireccionChange(index, 'calle', e.target.value)}
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
                        value={direccion.numero}
                        onChange={(e) => handleDireccionChange(index, 'numero', e.target.value)}
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
                        value={direccion.departamento}
                        onChange={(e) => handleDireccionChange(index, 'departamento', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
                      />
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1">
                        Comuna *
                      </label>
                      <input
                        type="text"
                        value={direccion.comuna}
                        onChange={(e) => handleDireccionChange(index, 'comuna', e.target.value)}
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
                        value={direccion.region}
                        onChange={(e) => handleDireccionChange(index, 'region', e.target.value)}
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
                        value={direccion.codigoPostal}
                        onChange={(e) => handleDireccionChange(index, 'codigoPostal', e.target.value)}
                        required
                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-ferremas-primary"
                      />
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}

          <div className="flex justify-end space-x-3 pt-4 border-t">
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

export default ClienteForm; 