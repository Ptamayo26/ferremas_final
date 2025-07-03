import React, { useEffect, useState } from 'react';
import { apiClient } from '../services/api';
import type { ClienteResponseDTO } from '../types/api';

const ClientesAdmin: React.FC = () => {
  const [clientes, setClientes] = useState<ClienteResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  // Estado para edición
  const [editCliente, setEditCliente] = useState<ClienteResponseDTO | null>(null);
  const [editForm, setEditForm] = useState({ nombre: '', apellido: '', rut: '', correoElectronico: '', telefono: '' });
  const [editLoading, setEditLoading] = useState(false);
  const [editError, setEditError] = useState<string | null>(null);

  const fetchClientes = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get<ClienteResponseDTO[]>('/api/Clientes');
      setClientes(response.data || []);
      setError(null);
    } catch (err) {
      setError('No se pudo cargar la lista de clientes.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchClientes();
  }, []);

  const handleEdit = (cliente: ClienteResponseDTO) => {
    setEditCliente(cliente);
    setEditForm({
      nombre: cliente.nombre,
      apellido: cliente.apellido,
      rut: cliente.rut,
      correoElectronico: cliente.correoElectronico,
      telefono: cliente.telefono || ''
    });
    setEditError(null);
  };

  const handleEditChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setEditForm({ ...editForm, [e.target.name]: e.target.value });
  };

  const handleEditSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editCliente) return;
    setEditLoading(true);
    setEditError(null);
    try {
      await apiClient.put(`/api/Clientes/${editCliente.id}`, {
        nombre: editForm.nombre,
        apellido: editForm.apellido,
        rut: editForm.rut,
        correoElectronico: editForm.correoElectronico,
        telefono: editForm.telefono || undefined
      });
      setEditCliente(null);
      fetchClientes();
    } catch (err: any) {
      setEditError(err.response?.data || 'No se pudo actualizar el cliente.');
    } finally {
      setEditLoading(false);
    }
  };

  const handleEditCancel = () => {
    setEditCliente(null);
    setEditError(null);
  };

  const handleDelete = async (clienteId: number) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar este cliente?')) {
      try {
        await apiClient.delete(`/api/Clientes/${clienteId}`);
        fetchClientes();
      } catch (err) {
        alert('No se pudo eliminar el cliente.');
      }
    }
  };

  const handleView = (cliente: ClienteResponseDTO) => {
    alert(`Datos de contacto:\nNombre: ${cliente.nombre} ${cliente.apellido}\nCorreo: ${cliente.correoElectronico}\nRUT: ${cliente.rut}\nTeléfono: ${cliente.telefono || 'No registrado'}`);
    // Aquí puedes mostrar un modal con más detalles si lo deseas
  };

  if (loading) {
    return <div className="flex items-center justify-center p-8"><div className="animate-spin text-4xl">⚙️</div><p className="ml-4 text-lg">Cargando clientes...</p></div>;
  }

  if (error) {
    return <div className="p-4 text-center text-red-600 bg-red-100 rounded-lg">{error}</div>;
  }

  return (
    <div className="card p-6 max-w-6xl mx-auto mt-8">
      <h2 className="text-2xl font-bold text-ferremas-primary mb-6">Gestión de Clientes</h2>
      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-ferremas-gray-200">
          <thead className="bg-ferremas-gray-50">
            <tr>
              <th className="th-cell">Nombre</th>
              <th className="th-cell">Correo</th>
              <th className="th-cell">RUT</th>
              <th className="th-cell">Teléfono</th>
              <th className="th-cell">Estado</th>
              <th className="th-cell">Acciones</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-ferremas-gray-200">
            {clientes.map((cliente) => (
              <tr key={cliente.id} className="hover:bg-ferremas-gray-50">
                <td className="td-cell">{cliente.nombre} {cliente.apellido}</td>
                <td className="td-cell">{cliente.correoElectronico}</td>
                <td className="td-cell">{cliente.rut}</td>
                <td className="td-cell">{cliente.telefono || '-'}</td>
                <td className="td-cell">
                  <span className={`badge-${cliente.activo ? 'success' : 'danger'}`}>{cliente.activo ? 'Activo' : 'Inactivo'}</span>
                </td>
                <td className="td-cell">
                  <div className="flex space-x-2">
                    <button onClick={() => handleView(cliente)} className="text-ferremas-primary hover:underline">👁️</button>
                    <button onClick={() => handleEdit(cliente)} className="text-ferremas-blue-600 hover:text-ferremas-blue-800">✏️</button>
                    <button onClick={() => handleDelete(cliente.id)} className="text-ferremas-danger hover:text-red-700">🗑️</button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Modal de edición */}
      {editCliente && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md mx-4">
            <h3 className="text-xl font-bold mb-4 text-ferremas-primary">Editar Cliente</h3>
            {editError && <div className="mb-2 text-red-600">{editError}</div>}
            <form onSubmit={handleEditSave} className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Nombre</label>
                <input type="text" name="nombre" value={editForm.nombre} onChange={handleEditChange} required className="w-full border rounded px-3 py-2" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Apellido</label>
                <input type="text" name="apellido" value={editForm.apellido} onChange={handleEditChange} required className="w-full border rounded px-3 py-2" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">RUT</label>
                <input type="text" name="rut" value={editForm.rut} onChange={handleEditChange} required className="w-full border rounded px-3 py-2" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Correo electrónico</label>
                <input type="email" name="correoElectronico" value={editForm.correoElectronico} onChange={handleEditChange} required className="w-full border rounded px-3 py-2" />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Teléfono</label>
                <input type="text" name="telefono" value={editForm.telefono} onChange={handleEditChange} className="w-full border rounded px-3 py-2" />
              </div>
              <div className="flex justify-end space-x-2 mt-4">
                <button type="button" onClick={handleEditCancel} className="btn-secondary">Cancelar</button>
                <button type="submit" className="btn-primary" disabled={editLoading}>{editLoading ? 'Guardando...' : 'Guardar'}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default ClientesAdmin; 