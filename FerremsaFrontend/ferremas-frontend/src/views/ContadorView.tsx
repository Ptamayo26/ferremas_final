import React, { useEffect, useState } from 'react';
import { apiClient } from '../services/api';
import { useAuth } from '../context/AuthContext';

function getEstadoColor(estado: string) {
  switch (estado) {
    case 'ENTREGADO': return 'text-green-600 font-bold';
    case 'EN_PREPARACION': return 'text-yellow-600 font-bold';
    case 'PENDIENTE': return 'text-red-600 font-bold';
    default: return 'text-gray-600';
  }
}

function formatFecha(fecha: string) {
  if (!fecha) return '-';
  try {
    return new Date(fecha).toLocaleString();
  } catch {
    return fecha;
  }
}

const AccordionSection: React.FC<{
  title: string;
  colorClass: string;
  children: React.ReactNode;
  defaultOpen?: boolean;
}> = ({ title, colorClass, children, defaultOpen = false }) => {
  const [open, setOpen] = useState(defaultOpen);
  return (
    <div className="mb-4 bg-white rounded-xl shadow-lg">
      <button
        className={`w-full flex justify-between items-center px-6 py-4 text-lg font-semibold focus:outline-none ${colorClass}`}
        onClick={() => setOpen((v) => !v)}
        aria-expanded={open}
      >
        <span>{title}</span>
        <span className="ml-2 text-xl">{open ? '▲' : '▼'}</span>
      </button>
      {open && <div className="px-6 pb-6">{children}</div>}
    </div>
  );
};

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
    <div className="p-8 bg-gray-50 min-h-screen max-w-5xl mx-auto">
      <h1 className="text-3xl font-bold mb-8 text-gray-800">Panel de Contador</h1>
      <AccordionSection title="Confirmación de pagos Transbank" colorClass="text-blue-700" defaultOpen={false}>
        {loading ? <div>Cargando...</div> : error ? <div className="text-red-500">{error}</div> : (
          <div className="overflow-x-auto">
            <table className="min-w-full text-sm rounded-lg overflow-hidden">
              <thead>
                <tr className="bg-blue-100 text-blue-900">
                  <th className="px-3 py-2 text-left">ID</th>
                  <th className="px-3 py-2 text-left">Pedido</th>
                  <th className="px-3 py-2 text-left">Cliente</th>
                  <th className="px-3 py-2 text-left">Monto</th>
                  <th className="px-3 py-2 text-left">Fecha</th>
                  <th className="px-3 py-2 text-left">Estado</th>
                </tr>
              </thead>
              <tbody>
                {pagos.length === 0 ? <tr><td colSpan={6} className="text-center py-4">No hay pagos.</td></tr> : pagos.map((p: any, idx) => (
                  <tr key={p.id} className={idx % 2 === 0 ? "bg-white" : "bg-gray-50"}>
                    <td className="px-3 py-2">{p.id}</td>
                    <td className="px-3 py-2">{p.pedidoId}</td>
                    <td className="px-3 py-2">{p.pedido?.usuario?.nombre || '-'}</td>
                    <td className="px-3 py-2">${p.monto?.toLocaleString()}</td>
                    <td className="px-3 py-2">{formatFecha(p.fechaPago)}</td>
                    <td className={`px-3 py-2 ${getEstadoColor(p.estado)}`}>{p.estado}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </AccordionSection>
      <AccordionSection title="Registro de entregas realizadas" colorClass="text-green-700" defaultOpen={false}>
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm rounded-lg overflow-hidden">
            <thead>
              <tr className="bg-green-100 text-green-900">
                <th className="px-3 py-2 text-left">ID</th>
                <th className="px-3 py-2 text-left">Pedido</th>
                <th className="px-3 py-2 text-left">Cliente</th>
                <th className="px-3 py-2 text-left">Repartidor</th>
                <th className="px-3 py-2 text-left">Fecha</th>
                <th className="px-3 py-2 text-left">Estado</th>
              </tr>
            </thead>
            <tbody>
              {entregas.length === 0 ? <tr><td colSpan={6} className="text-center py-4">No hay entregas.</td></tr> : entregas.map((e: any, idx) => (
                <tr key={e.id} className={idx % 2 === 0 ? "bg-white" : "bg-gray-50"}>
                  <td className="px-3 py-2">{e.id}</td>
                  <td className="px-3 py-2">{e.pedidoId}</td>
                  <td className="px-3 py-2">{e.cliente}</td>
                  <td className="px-3 py-2">{e.repartidor || <span className="text-gray-400">-</span>}</td>
                  <td className="px-3 py-2">{formatFecha(e.fecha)}</td>
                  <td className={`px-3 py-2 ${getEstadoColor(e.estadoEnvio)}`}>{e.estadoEnvio}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </AccordionSection>
      <AccordionSection title="Reportes financieros y balances" colorClass="text-purple-700" defaultOpen={false}>
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm rounded-lg overflow-hidden">
            <thead>
              <tr className="bg-purple-100 text-purple-900">
                <th className="px-3 py-2">Fecha</th>
                <th className="px-3 py-2">Total Ventas</th>
                <th className="px-3 py-2">Total Ingresos</th>
                <th className="px-3 py-2">Total Egresos</th>
                <th className="px-3 py-2">Utilidad</th>
              </tr>
            </thead>
            <tbody>
              {finanzas.length === 0 ? <tr><td colSpan={5} className="text-center py-4">No hay datos.</td></tr> : finanzas.map((f: any, idx) => (
                <tr key={f.fecha} className={idx % 2 === 0 ? "bg-white" : "bg-gray-50"}>
                  <td className="px-3 py-2">{formatFecha(f.fecha)}</td>
                  <td className="px-3 py-2 font-bold text-blue-600">{f.totalVentas}</td>
                  <td className="px-3 py-2">${f.totalIngresos?.toLocaleString()}</td>
                  <td className="px-3 py-2">${f.totalEgresos?.toLocaleString()}</td>
                  <td className="px-3 py-2 font-bold text-green-600">${f.utilidad?.toLocaleString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </AccordionSection>
      <AccordionSection title="Integración con transporte y logística" colorClass="text-orange-700" defaultOpen={false}>
        <div className="overflow-x-auto">
          <table className="min-w-full text-sm rounded-lg overflow-hidden">
            <thead>
              <tr className="bg-orange-100 text-orange-900">
                <th className="px-3 py-2 text-left">ID</th>
                <th className="px-3 py-2 text-left">Pedido</th>
                <th className="px-3 py-2 text-left">Cliente</th>
                <th className="px-3 py-2 text-left">Empresa</th>
                <th className="px-3 py-2 text-left">Estado</th>
                <th className="px-3 py-2 text-left">Fecha</th>
              </tr>
            </thead>
            <tbody>
              {logistica.length === 0 ? <tr><td colSpan={6} className="text-center py-4">No hay despachos.</td></tr> : logistica.map((l: any, idx) => (
                <tr key={l.id} className={idx % 2 === 0 ? "bg-white" : "bg-gray-50"}>
                  <td className="px-3 py-2">{l.id}</td>
                  <td className="px-3 py-2">{l.pedidoId}</td>
                  <td className="px-3 py-2">{l.cliente}</td>
                  <td className="px-3 py-2">{l.empresaTransporte}</td>
                  <td className={`px-3 py-2 ${getEstadoColor(l.estadoEnvio)}`}>{l.estadoEnvio}</td>
                  <td className="px-3 py-2">{formatFecha(l.fecha)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </AccordionSection>
    </div>
  );
};

export default ContadorView; 