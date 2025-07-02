import React, { useState, useEffect } from 'react';
import { apiClient } from '../../services/api';
import type { ClienteResponseDTO } from '../../types/api';
import ClienteDetalle from './ClienteDetalle';
import ClienteForm from './ClienteForm';
import ConfirmDialog from '../ui/ConfirmDialog';
import Notification from '../ui/Notification';

const GestionClientes: React.FC = () => {
  const [clientes, setClientes] = useState<ClienteResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCliente, setSelectedCliente] = useState<ClienteResponseDTO | null>(null);
  const [clienteFormOpen, setClienteFormOpen] = useState(false);
  const [clienteSeleccionado, setClienteSeleccionado] = useState<ClienteResponseDTO | undefined>();
  const [deletingCliente, setDeletingCliente] = useState<number | null>(null);
  
  // Estados para confirmación y notificaciones
  const [confirmDialog, setConfirmDialog] = useState<{
    isOpen: boolean;
    cliente: ClienteResponseDTO | null;
  }>({ isOpen: false, cliente: null });
  
  const [notification, setNotification] = useState<{
    isOpen: boolean;
    type: 'success' | 'error' | 'warning' | 'info';
    title: string;
    message?: string;
  }>({ isOpen: false, type: 'info', title: '' });

  const fetchClientes = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get<ClienteResponseDTO[]>('/api/Clientes');
      setClientes(response.data || []);
    } catch (err) {
      setError('No se pudo cargar la lista de clientes.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchClientes();
  }, []);

  const filteredClientes = clientes.filter(cliente =>
    cliente.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
    cliente.rut.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSelectCliente = (cliente: ClienteResponseDTO) => {
    setSelectedCliente(cliente);
  };

  const handleBack = () => {
    setSelectedCliente(null);
  };

  const handleNuevoCliente = () => {
    setClienteSeleccionado(undefined);
    setClienteFormOpen(true);
  };

  const handleEditarCliente = (cliente: ClienteResponseDTO) => {
    setClienteSeleccionado(cliente);
    setClienteFormOpen(true);
  };

  const handleEliminarCliente = (cliente: ClienteResponseDTO) => {
    setConfirmDialog({
      isOpen: true,
      cliente: cliente
    });
  };

  const confirmEliminarCliente = async () => {
    const cliente = confirmDialog.cliente;
    if (!cliente) return;

    try {
      setDeletingCliente(cliente.id);
      await apiClient.delete(`/api/Clientes/${cliente.id}`);
      
      // Mostrar notificación de éxito
      setNotification({
        isOpen: true,
        type: 'success',
        title: 'Cliente eliminado',
        message: `El cliente "${cliente.nombre} ${cliente.apellido}" ha sido eliminado exitosamente.`
      });
      
      // Recargar la lista
      fetchClientes();
    } catch (err: any) {
      const errorMessage = err.response?.data || 'Error al eliminar el cliente';
      setNotification({
        isOpen: true,
        type: 'error',
        title: 'Error al eliminar',
        message: errorMessage
      });
      console.error('Error al eliminar cliente:', err);
    } finally {
      setDeletingCliente(null);
    }
  };

  const handleClienteSaved = () => {
    fetchClientes(); // Recargar la lista de clientes
    setNotification({
      isOpen: true,
      type: 'success',
      title: 'Cliente guardado',
      message: 'El cliente ha sido guardado exitosamente.'
    });
  };

  const handleClienteUpdate = () => {
    fetchClientes(); // Recargar la lista de clientes
  };

  if (loading) return <p>Cargando clientes...</p>;
  if (error) return <p className="text-red-500">{error}</p>;

  if (selectedCliente) {
    return <ClienteDetalle cliente={selectedCliente} onBack={handleBack} onClienteUpdate={handleClienteUpdate} />;
  }

  return (
    <div className="p-6 space-y-6">
      <h1 className="text-2xl font-bold text-ferremas-primary">Gestión de Clientes</h1>
      
      <div className="flex justify-between items-center">
        <input
          type="text"
          placeholder="Buscar por nombre o RUT..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="input-field w-1/3"
        />
        <button 
          onClick={handleNuevoCliente}
          className="btn-primary"
        >
          Nuevo Cliente
        </button>
      </div>

      <div className="card">
        <table className="min-w-full">
          <thead>
            <tr>
              <th className="th-cell">Nombre</th>
              <th className="th-cell">RUT</th>
              <th className="th-cell">Email</th>
              <th className="th-cell">Teléfono</th>
              <th className="th-cell">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {filteredClientes.map(cliente => (
              <tr key={cliente.id}>
                <td className="td-cell">{cliente.nombre} {cliente.apellido}</td>
                <td className="td-cell">{cliente.rut}</td>
                <td className="td-cell">{cliente.correoElectronico}</td>
                <td className="td-cell">{cliente.telefono ?? 'No especificado'}</td>
                <td className="td-cell">
                  <div className="flex space-x-2">
                    <button 
                      onClick={() => handleSelectCliente(cliente)} 
                      className="btn-secondary text-sm"
                      title="Ver detalles del cliente"
                    >
                      Ver Detalles
                    </button>
                    <button 
                      onClick={() => handleEditarCliente(cliente)} 
                      className="btn-secondary text-sm"
                      title="Editar cliente"
                    >
                      Editar
                    </button>
                    <button 
                      onClick={() => handleEliminarCliente(cliente)}
                      disabled={deletingCliente === cliente.id}
                      className="btn-danger text-sm"
                      title="Eliminar cliente"
                    >
                      {deletingCliente === cliente.id ? 'Eliminando...' : 'Eliminar'}
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <ClienteForm
        isOpen={clienteFormOpen}
        onClose={() => setClienteFormOpen(false)}
        cliente={clienteSeleccionado}
        onSave={handleClienteSaved}
      />

      <ConfirmDialog
        isOpen={confirmDialog.isOpen}
        onClose={() => setConfirmDialog({ isOpen: false, cliente: null })}
        onConfirm={confirmEliminarCliente}
        title="Confirmar eliminación"
        message={`¿Está seguro de que desea eliminar al cliente "${confirmDialog.cliente?.nombre} ${confirmDialog.cliente?.apellido}"?\n\nEsta acción no se puede deshacer.`}
        confirmText="Eliminar"
        cancelText="Cancelar"
        type="danger"
      />

      <Notification
        isOpen={notification.isOpen}
        onClose={() => setNotification({ ...notification, isOpen: false })}
        type={notification.type}
        title={notification.title}
        message={notification.message}
      />
    </div>
  );
};

export default GestionClientes; 