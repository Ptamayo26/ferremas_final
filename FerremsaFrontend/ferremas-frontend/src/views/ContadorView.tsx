import React, { useEffect, useState } from 'react';
import { apiClient } from '../services/api';
import { useAuth } from '../context/AuthContext';

const ContadorView: React.FC = () => {
  const { user } = useAuth();
  const [pagos, setPagos] = useState<any[]>([]);
  const [entregas, setEntregas] = useState<any[]>([]);
  const [finanzas, setFinanzas] = useState<any[]>([]);
  const [logistica, setLogistica] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user || !['contador', 'administrador'].includes(user.rol)) return;
    setLoading(true);
    setError(null);
    Promise.all([
      apiClient.get('/api/pagos'),
      apiClient.get('/api/envios/entregas'),
      apiClient.get('/api/reportes/finanzas'),
      apiClient.get('/api/envios/logistica')
    ]).then(([pagosRes, entregasRes, finanzasRes, logisticaRes]) => {
      setPagos(pagosRes.data.datos || []);
      setEntregas(entregasRes.data.datos || []);
      setFinanzas(finanzasRes.data.datos || []);
      setLogistica(logisticaRes.data.datos || []);
    }).catch(() => setError('Error al cargar datos')).finally(() => setLoading(false));
  }, [user]);

  if (!user || !['contador', 'administrador'].includes(user.rol)) return null;

  return (
    <div className="p-8">
      <h1 className="text-3xl font-bold mb-6">Panel de Contador</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {/* Pagos Transbank */}
        <section className="bg-white rounded shadow p-6 col-span-1 md:col-span-2">
          <h2 className="text-xl font-semibold mb-2">Confirmación de pagos Transbank</h2>
          {loading ? <div>Cargando...</div> : error ? <div className="text-red-500">{error}</div> : (
            <div className="overflow-x-auto">
              <table className="min-w-full border text-sm">
                <thead>
                  <tr className="bg-ferremas-gray-100">
                    <th>ID</th><th>Pedido</th><th>Cliente</th><th>Monto</th><th>Fecha</th><th>Estado</th>
                  </tr>
                </thead>
                <tbody>
                  {pagos.length === 0 ? <tr><td colSpan={6}>No hay pagos.</td></tr> : pagos.map((p: any) => (
                    <tr key={p.id}>
                      <td>{p.id}</td>
                      <td>{p.pedidoId}</td>
                      <td>{p.pedido?.usuario?.nombre || '-'}</td>
                      <td>${p.monto?.toLocaleString()}</td>
                      <td>{p.fechaPago ? new Date(p.fechaPago).toLocaleString() : '-'}</td>
                      <td>{p.estado}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </section>
        {/* Entregas realizadas */}
        <section className="bg-white rounded shadow p-6">
          <h2 className="text-xl font-semibold mb-2">Registro de entregas realizadas</h2>
          <div className="overflow-x-auto">
            <table className="min-w-full border text-sm">
              <thead>
                <tr className="bg-ferremas-gray-100">
                  <th>ID</th><th>Pedido</th><th>Cliente</th><th>Repartidor</th><th>Fecha</th><th>Estado</th>
                </tr>
              </thead>
              <tbody>
                {entregas.length === 0 ? <tr><td colSpan={6}>No hay entregas.</td></tr> : entregas.map((e: any) => (
                  <tr key={e.id}>
                    <td>{e.id}</td>
                    <td>{e.pedidoId}</td>
                    <td>{e.cliente}</td>
                    <td>{e.repartidor}</td>
                    <td>{e.fecha ? new Date(e.fecha).toLocaleString() : '-'}</td>
                    <td>{e.estadoEnvio}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
        {/* Reportes financieros */}
        <section className="bg-white rounded shadow p-6">
          <h2 className="text-xl font-semibold mb-2">Reportes financieros y balances</h2>
          <div className="overflow-x-auto">
            <table className="min-w-full border text-sm">
              <thead>
                <tr className="bg-ferremas-gray-100">
                  <th>Fecha</th><th>Total Ventas</th><th>Total Ingresos</th><th>Total Egresos</th><th>Utilidad</th>
                </tr>
              </thead>
              <tbody>
                {finanzas.length === 0 ? <tr><td colSpan={5}>No hay datos.</td></tr> : finanzas.map((f: any) => (
                  <tr key={f.fecha}>
                    <td>{new Date(f.fecha).toLocaleString()}</td>
                    <td>{f.totalVentas}</td>
                    <td>${f.totalIngresos?.toLocaleString()}</td>
                    <td>${f.totalEgresos?.toLocaleString()}</td>
                    <td>${f.utilidad?.toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
        {/* Logística */}
        <section className="bg-white rounded shadow p-6">
          <h2 className="text-xl font-semibold mb-2">Integración con transporte y logística</h2>
          <div className="overflow-x-auto">
            <table className="min-w-full border text-sm">
              <thead>
                <tr className="bg-ferremas-gray-100">
                  <th>ID</th><th>Pedido</th><th>Cliente</th><th>Empresa</th><th>Estado</th><th>Fecha</th>
                </tr>
              </thead>
              <tbody>
                {logistica.length === 0 ? <tr><td colSpan={6}>No hay despachos.</td></tr> : logistica.map((l: any) => (
                  <tr key={l.id}>
                    <td>{l.id}</td>
                    <td>{l.pedidoId}</td>
                    <td>{l.cliente}</td>
                    <td>{l.empresaTransporte}</td>
                    <td>{l.estadoEnvio}</td>
                    <td>{l.fecha ? new Date(l.fecha).toLocaleString() : '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </div>
  );
};

export default ContadorView; 