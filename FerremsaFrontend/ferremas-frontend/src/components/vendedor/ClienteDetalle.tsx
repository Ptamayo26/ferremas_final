import React, { useState, useEffect } from 'react';
import { apiClient } from '../../services/api';
import type { ClienteResponseDTO, PedidoResponseDTO, DireccionDTO } from '../../types/api';
import DireccionForm from './DireccionForm';

interface ClienteDetalleProps {
  cliente: ClienteResponseDTO;
  onBack: () => void;
  onClienteUpdate: () => void;
}

const ClienteDetalle: React.FC<ClienteDetalleProps> = ({ cliente, onBack, onClienteUpdate }) => {
  const [pedidos, setPedidos] = useState<PedidoResponseDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [direccionFormOpen, setDireccionFormOpen] = useState(false);
  const [direccionSeleccionada, setDireccionSeleccionada] = useState<DireccionDTO | undefined>();

  useEffect(() => {
    const fetchPedidos = async () => {
      if (!cliente.usuarioId) {
        setPedidos([]);
        setLoading(false);
        return;
      }
      try {
        setLoading(true);
        const response = await apiClient.get<any>(`/api/Pedidos/usuario/${cliente.usuarioId}`);
        setPedidos(response.data.datos || []);
      } catch (err) {
        setError('No se pudo cargar el historial de compras.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchPedidos();
  }, [cliente.usuarioId]);

  const handleNuevaDireccion = () => {
    setDireccionSeleccionada(undefined);
    setDireccionFormOpen(true);
  };

  const handleEditarDireccion = (direccion: DireccionDTO) => {
    setDireccionSeleccionada(direccion);
    setDireccionFormOpen(true);
  };

  const handleDireccionSaved = () => {
    onClienteUpdate(); // Esto actualizará la lista de clientes y por ende las direcciones
  };

  const handleSetPrincipal = async (direccion: DireccionDTO) => {
    try {
      await apiClient.put(`/api/Direcciones/${direccion.id}/principal`);
      onClienteUpdate();
    } catch (err) {
      console.error('Error al marcar como principal:', err);
    }
  };

  const handleDeleteDireccion = async (direccion: DireccionDTO) => {
    if (!window.confirm('¿Está seguro de que desea eliminar esta dirección?')) {
      return;
    }

    try {
      await apiClient.delete(`/api/Direcciones/${direccion.id}`);
      onClienteUpdate();
    } catch (err: any) {
      alert(err.response?.data || 'Error al eliminar la dirección');
    }
  };

  return (
    <div className="p-6 space-y-6">
      <button onClick={onBack} className="btn-secondary mb-4">
        &larr; Volver a la lista
      </button>

      <div className="card">
        <h2 className="text-xl font-bold text-ferremas-primary mb-4">Datos del Cliente</h2>
        <p><strong>Nombre:</strong> {cliente.nombre} {cliente.apellido}</p>
        <p><strong>RUT:</strong> {cliente.rut}</p>
        <p><strong>Email:</strong> {cliente.correoElectronico}</p>
        <p><strong>Teléfono:</strong> {cliente.telefono ?? 'No especificado'}</p>
      </div>

      <div className="card">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold text-ferremas-primary">Direcciones de Envío</h2>
          <button
            onClick={handleNuevaDireccion}
            className="btn-primary"
          >
            + Nueva Dirección
          </button>
        </div>
        
        {cliente.direcciones && cliente.direcciones.length > 0 ? (
          <ul className="space-y-4">
            {cliente.direcciones.map(dir => (
              <li key={dir.id} className="p-4 border rounded-lg">
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <p className="font-semibold">
                      {dir.calle} {dir.numero}, {dir.departamento ? `${dir.departamento}, ` : ''} {dir.comuna}, {dir.region}
                      {dir.esPrincipal && <span className="ml-2 badge-primary">Principal</span>}
                    </p>
                    <p className="text-sm text-gray-600">Código Postal: {dir.codigoPostal}</p>
                  </div>
                  <div className="flex space-x-2 ml-4">
                    {!dir.esPrincipal && (
                      <button
                        onClick={() => handleSetPrincipal(dir)}
                        className="btn-secondary text-sm"
                        title="Marcar como principal"
                      >
                        Principal
                      </button>
                    )}
                    <button
                      onClick={() => handleEditarDireccion(dir)}
                      className="btn-secondary text-sm"
                      title="Editar dirección"
                    >
                      Editar
                    </button>
                    <button
                      onClick={() => handleDeleteDireccion(dir)}
                      className="btn-danger text-sm"
                      title="Eliminar dirección"
                    >
                      Eliminar
                    </button>
                  </div>
                </div>
              </li>
            ))}
          </ul>
        ) : (
          <p>No hay direcciones registradas para este cliente.</p>
        )}
      </div>

      <div className="card">
        <h2 className="text-xl font-bold text-ferremas-primary mb-4">Historial de Compras</h2>
        {loading && <p>Cargando historial...</p>}
        {error && <p className="text-red-500">{error}</p>}
        {!loading && !error && (
          <table>
            <thead>
              <tr>
                <th>ID Pedido</th>
                <th>Fecha</th>
                <th>Total</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              {pedidos.map(pedido => (
                <tr key={pedido.id}>
                  <td>{pedido.id}</td>
                  <td>{new Date(pedido.fechaPedido).toLocaleDateString()}</td>
                  <td>${pedido.total.toFixed(2)}</td>
                  <td>{pedido.estado}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <DireccionForm
        isOpen={direccionFormOpen}
        onClose={() => setDireccionFormOpen(false)}
        usuarioId={cliente.usuarioId!}
        direccion={direccionSeleccionada}
        onSave={handleDireccionSaved}
      />
    </div>
  );
};

export default ClienteDetalle; 