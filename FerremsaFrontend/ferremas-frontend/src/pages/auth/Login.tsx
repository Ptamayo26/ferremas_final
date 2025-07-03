import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { getRedirectPathByRole, getRoleDisplayName } from '../../utils/roleRedirect';

const Login: React.FC = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);
    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setLoading(true);
        
        try {
            const response = await login({ email, password });
            
            // Obtener la ruta de redirección basada en el rol
            const redirectPath = getRedirectPathByRole(response.usuario?.rol || '');
            const roleDisplayName = getRoleDisplayName(response.usuario?.rol || '');
            
            console.log(`✅ Login exitoso - Rol: ${roleDisplayName} - Redirigiendo a: ${redirectPath}`);
            
            // Redirigir según el rol
            navigate(redirectPath);
            
        } catch (err: any) {
            console.error('❌ Error en login:', err);
            setError(err.message || 'Error al iniciar sesión. Por favor, inténtelo de nuevo.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full space-y-8 p-10 bg-white shadow-lg rounded-xl">
                <div>
                    <h2 className="mt-6 text-center text-3xl font-extrabold text-ferremas-primary">
                        Inicia sesión en tu cuenta
                    </h2>
                    <p className="mt-2 text-center text-sm text-ferremas-gray-600">
                        Accede a tu panel según tu rol en Ferremas
                    </p>
                </div>
                
                <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
                    <input type="hidden" name="remember" value="true" />
                    <div className="rounded-md shadow-sm -space-y-px">
                        <div>
                            <label htmlFor="email-address" className="sr-only">Correo electrónico</label>
                            <input
                                id="email-address"
                                name="email"
                                type="email"
                                autoComplete="email"
                                required
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                className="appearance-none rounded-none relative block w-full px-3 py-2 border border-ferremas-gray-200 placeholder-ferremas-gray-500 text-ferremas-primary rounded-t-md focus:outline-none focus:ring-2 focus:ring-ferremas-orange-400 focus:border-ferremas-orange-400 focus:z-10 sm:text-sm"
                                placeholder="Correo electrónico"
                                disabled={loading}
                            />
                        </div>
                        <div>
                            <label htmlFor="password" className="sr-only">Contraseña</label>
                            <input
                                id="password"
                                name="password"
                                type="password"
                                autoComplete="current-password"
                                required
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                className="appearance-none rounded-none relative block w-full px-3 py-2 border border-ferremas-gray-200 placeholder-ferremas-gray-500 text-ferremas-primary rounded-b-md focus:outline-none focus:ring-2 focus:ring-ferremas-orange-400 focus:border-ferremas-orange-400 focus:z-10 sm:text-sm"
                                placeholder="Contraseña"
                                disabled={loading}
                            />
                        </div>
                    </div>

                    {error && (
                        <div className="text-ferremas-danger text-sm text-center bg-red-50 p-3 rounded-lg">
                            {error}
                        </div>
                    )}

                    <div>
                        <button
                            type="submit"
                            disabled={loading}
                            className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-ferremas-primary hover:bg-ferremas-primary-dark focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-ferremas-orange-400 disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
                        >
                            {loading ? (
                                <div className="flex items-center">
                                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                                    Iniciando sesión...
                                </div>
                            ) : (
                                'Iniciar sesión'
                            )}
                        </button>
                    </div>

                    <div className="text-center space-y-2">
                        <p className="text-sm text-ferremas-gray-600">
                            ¿No tienes una cuenta?{' '}
                            <Link 
                                to="/register" 
                                className="text-ferremas-secondary hover:text-ferremas-accent font-medium transition-colors duration-200"
                            >
                                Regístrate aquí
                            </Link>
                        </p>
                        <Link 
                            to="/catalogo" 
                            className="text-ferremas-secondary hover:text-ferremas-accent text-sm transition-colors duration-200"
                        >
                            ← Volver al catálogo
                        </Link>
                    </div>
                </form>

                {/* Información sobre roles */}
                <div className="mt-6 bg-ferremas-gray-50 p-4 rounded-lg">
                    <h3 className="text-sm font-medium text-ferremas-primary mb-2">Roles del sistema:</h3>
                    <div className="text-xs text-ferremas-gray-600 space-y-1">
                        <div><strong>Cliente:</strong> Acceso a catálogo y gestión de pedidos</div>
                        <div><strong>Vendedor:</strong> Gestión de ventas y clientes</div>
                        <div><strong>Bodeguero:</strong> Gestión de inventario</div>
                        <div><strong>Administrador:</strong> Acceso completo al sistema</div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Login; 