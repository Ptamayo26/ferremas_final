import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';

const DebugAuth: React.FC = () => {
  const { isAuthenticated, user, token, isLoading } = useAuth();
  const [localStorageData, setLocalStorageData] = useState<any>({});

  useEffect(() => {
    const authToken = localStorage.getItem('ferremas_token');
    const userData = localStorage.getItem('ferremas_user');
    setLocalStorageData({
      authToken: authToken ? 'Presente' : 'No encontrado',
      userData: userData ? 'Presente' : 'No encontrado'
    });
  }, []);

  return (
    <div className="p-8 space-y-6">
      <h2 className="text-2xl font-bold text-ferremas-primary">Diagnóstico de Autenticación</h2>
      
      <div className="bg-white p-6 rounded-lg shadow-md">
        <h3 className="text-lg font-semibold mb-4">Estado del AuthContext:</h3>
        <div className="grid grid-cols-2 gap-4 text-sm">
          <div><strong>isAuthenticated:</strong> {isAuthenticated ? 'true' : 'false'}</div>
          <div><strong>isLoading:</strong> {isLoading ? 'true' : 'false'}</div>
          <div><strong>Token:</strong> {token ? 'Presente' : 'No encontrado'}</div>
          <div><strong>Usuario:</strong> {user ? user.nombre : 'No encontrado'}</div>
        </div>
      </div>

      <div className="bg-white p-6 rounded-lg shadow-md">
        <h3 className="text-lg font-semibold mb-4">localStorage:</h3>
        <div className="space-y-2 text-sm">
          <div><strong>authToken:</strong> {localStorageData.authToken}</div>
          <div><strong>userData:</strong> {localStorageData.userData}</div>
        </div>
      </div>

      <div className="bg-blue-50 p-6 rounded-lg">
        <h3 className="text-lg font-semibold text-blue-800 mb-4">Rutas de Prueba:</h3>
        <div className="space-y-2 text-sm">
          <div><strong>Catálogo:</strong> <a href="/catalogo" className="text-blue-600 hover:underline ml-2">/catalogo</a></div>
          <div><strong>Login:</strong> <a href="/login" className="text-blue-600 hover:underline ml-2">/login</a></div>
        </div>
      </div>
    </div>
  );
};

export default DebugAuth; 