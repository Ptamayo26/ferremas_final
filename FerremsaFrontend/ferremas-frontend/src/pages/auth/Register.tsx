import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { getRedirectPathByRole, getRoleDisplayName } from '../../utils/roleRedirect';
import chileGeoService from '../../services/chileGeo';
import type { RegionChile, ComunaChile } from '../../types/api';

interface RegisterFormData {
    nombre: string;
    apellido: string;
    email: string;
    password: string;
    confirmPassword: string;
    rut: string;
    telefono: string;
    direccion: {
        calle: string;
        numero: string;
        departamento: string;
        comuna: string;
        region: string;
        codigoPostal: string;
    };
}

const Register: React.FC = () => {
    const [formData, setFormData] = useState<RegisterFormData>({
        nombre: '',
        apellido: '',
        email: '',
        password: '',
        confirmPassword: '',
        rut: '',
        telefono: '',
        direccion: {
            calle: '',
            numero: '',
            departamento: '',
            comuna: '',
            region: '',
            codigoPostal: ''
        }
    });
    
    const [errors, setErrors] = useState<Partial<RegisterFormData> & { direccion?: Partial<RegisterFormData['direccion']> }>({});
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    
    // Estados para datos geográficos
    const [regiones, setRegiones] = useState<RegionChile[]>([]);
    const [comunas, setComunas] = useState<ComunaChile[]>([]);
    const [loadingGeo, setLoadingGeo] = useState(false);
    
    const { login } = useAuth();
    const navigate = useNavigate();

    // Cargar datos geográficos al montar el componente
    useEffect(() => {
        const cargarDatosGeograficos = async () => {
            setLoadingGeo(true);
            try {
                const regionesData = await chileGeoService.getRegiones();
                setRegiones(regionesData);
            } catch (error) {
                console.error('Error al cargar regiones:', error);
                setError('No se pudieron cargar las regiones de Chile');
            } finally {
                setLoadingGeo(false);
            }
        };

        cargarDatosGeograficos();
    }, []);

    // Cargar comunas cuando cambie la región
    const handleRegionChange = async (codigoRegion: string) => {
        if (!codigoRegion) {
            setComunas([]);
            return;
        }

        try {
            const comunasData = await chileGeoService.getComunasByRegion(codigoRegion);
            setComunas(comunasData);
        } catch (error) {
            console.error('Error al cargar comunas:', error);
            setComunas([]);
        }
    };

    // Validaciones
    const validateForm = (): boolean => {
        const newErrors: Partial<RegisterFormData> = {};

        // Validar nombre
        if (!formData.nombre.trim()) {
            newErrors.nombre = 'El nombre es requerido';
        } else if (formData.nombre.length < 2) {
            newErrors.nombre = 'El nombre debe tener al menos 2 caracteres';
        } else if (formData.nombre.length > 100) {
            newErrors.nombre = 'El nombre no puede exceder 100 caracteres';
        }

        // Validar apellido
        if (!formData.apellido.trim()) {
            newErrors.apellido = 'El apellido es requerido';
        } else if (formData.apellido.length < 2) {
            newErrors.apellido = 'El apellido debe tener al menos 2 caracteres';
        } else if (formData.apellido.length > 100) {
            newErrors.apellido = 'El apellido no puede exceder 100 caracteres';
        }

        // Validar email
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!formData.email.trim()) {
            newErrors.email = 'El correo electrónico es requerido';
        } else if (!emailRegex.test(formData.email)) {
            newErrors.email = 'Ingrese un correo electrónico válido';
        } else if (formData.email.length > 100) {
            newErrors.email = 'El correo electrónico no puede exceder 100 caracteres';
        }

        // Validar contraseña
        if (!formData.password) {
            newErrors.password = 'La contraseña es requerida';
        } else if (formData.password.length < 6) {
            newErrors.password = 'La contraseña debe tener al menos 6 caracteres';
        } else if (formData.password.length > 100) {
            newErrors.password = 'La contraseña no puede exceder 100 caracteres';
        }

        // Validar confirmación de contraseña
        if (!formData.confirmPassword) {
            newErrors.confirmPassword = 'Confirme su contraseña';
        } else if (formData.password !== formData.confirmPassword) {
            newErrors.confirmPassword = 'Las contraseñas no coinciden';
        }

        // Validar RUT
        if (!formData.rut.trim()) {
            newErrors.rut = 'El RUT es requerido';
        } else if (!validateRut(formData.rut)) {
            newErrors.rut = 'Ingrese un RUT válido';
        } else if (formData.rut.length > 20) {
            newErrors.rut = 'El RUT no puede exceder 20 caracteres';
        }

        // Validar teléfono
        if (!formData.telefono.trim()) {
            newErrors.telefono = 'El teléfono es requerido';
        } else if (!validatePhone(formData.telefono)) {
            newErrors.telefono = 'Ingrese un teléfono válido';
        } else if (formData.telefono.length > 20) {
            newErrors.telefono = 'El teléfono no puede exceder 20 caracteres';
        }

        // Validar dirección
        if (!formData.direccion.calle.trim()) {
            newErrors.direccion = { ...newErrors.direccion, calle: 'La calle es requerida' };
        } else if (formData.direccion.calle.length > 200) {
            newErrors.direccion = { ...newErrors.direccion, calle: 'La calle no puede exceder 200 caracteres' };
        }
        
        if (!formData.direccion.numero.trim()) {
            newErrors.direccion = { ...newErrors.direccion, numero: 'El número es requerido' };
        } else if (formData.direccion.numero.length > 20) {
            newErrors.direccion = { ...newErrors.direccion, numero: 'El número no puede exceder 20 caracteres' };
        }
        
        if (formData.direccion.departamento.length > 100) {
            newErrors.direccion = { ...newErrors.direccion, departamento: 'El departamento no puede exceder 100 caracteres' };
        }
        
        if (!formData.direccion.comuna.trim()) {
            newErrors.direccion = { ...newErrors.direccion, comuna: 'La comuna es requerida' };
        } else if (formData.direccion.comuna.length > 100) {
            newErrors.direccion = { ...newErrors.direccion, comuna: 'La comuna no puede exceder 100 caracteres' };
        }
        
        if (!formData.direccion.region.trim()) {
            newErrors.direccion = { ...newErrors.direccion, region: 'La región es requerida' };
        } else if (formData.direccion.region.length > 100) {
            newErrors.direccion = { ...newErrors.direccion, region: 'La región no puede exceder 100 caracteres' };
        }
        
        // Validar código postal (máximo 10 caracteres según el backend)
        if (formData.direccion.codigoPostal.length > 10) {
            newErrors.direccion = { ...newErrors.direccion, codigoPostal: 'El código postal no puede exceder 10 caracteres' };
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    // Validar RUT chileno
    const validateRut = (rut: string): boolean => {
        // Limpiar el RUT de puntos y guión
        const cleanRut = rut.replace(/[.-]/g, '');
        
        if (cleanRut.length < 8 || cleanRut.length > 9) return false;
        
        // Obtener dígito verificador
        const dv = cleanRut.slice(-1);
        const rutNumber = cleanRut.slice(0, -1);
        
        if (!/^\d+$/.test(rutNumber)) return false;
        
        // Calcular dígito verificador
        let sum = 0;
        let multiplier = 2;
        
        for (let i = rutNumber.length - 1; i >= 0; i--) {
            sum += parseInt(rutNumber[i]) * multiplier;
            multiplier = multiplier === 7 ? 2 : multiplier + 1;
        }
        
        const expectedDv = 11 - (sum % 11);
        const calculatedDv = expectedDv === 11 ? '0' : expectedDv === 10 ? 'K' : expectedDv.toString();
        
        return dv.toUpperCase() === calculatedDv;
    };

    // Validar teléfono chileno
    const validatePhone = (phone: string): boolean => {
        const cleanPhone = phone.replace(/[+\s-]/g, '');
        return /^9\d{8}$/.test(cleanPhone);
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        
        // Manejar campos de dirección
        if (name.startsWith('direccion.')) {
            const fieldName = name.split('.')[1] as keyof RegisterFormData['direccion'];
            setFormData(prev => ({
                ...prev,
                direccion: {
                    ...prev.direccion,
                    [fieldName]: value
                }
            }));
            
            // Limpiar error del campo de dirección
            if (errors.direccion?.[fieldName]) {
                setErrors(prev => ({
                    ...prev,
                    direccion: {
                        ...prev.direccion,
                        [fieldName]: undefined
                    }
                }));
            }
        } else {
            setFormData(prev => ({
                ...prev,
                [name]: value
            }));
            
            // Limpiar error del campo cuando el usuario empiece a escribir
            if (errors[name as keyof RegisterFormData]) {
                setErrors(prev => ({
                    ...prev,
                    [name]: undefined
                }));
            }
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        
        if (!validateForm()) {
            return;
        }
        
        setLoading(true);
        
        try {
            const { authService } = await import('../../services/auth');
            
            const registerData = {
                nombre: formData.nombre,
                apellido: formData.apellido,
                email: formData.email,
                password: formData.password,
                rut: formData.rut,
                telefono: formData.telefono,
                rol: 'cliente',
                direccion: {
                    calle: formData.direccion.calle,
                    numero: formData.direccion.numero,
                    departamento: formData.direccion.departamento,
                    comuna: formData.direccion.comuna,
                    region: formData.direccion.region,
                    codigoPostal: formData.direccion.codigoPostal,
                    esPrincipal: true
                }
            };
            
            const response = await authService.register(registerData as any);
            
            if (response.exito) {
                // Auto-login después del registro exitoso
                const loginResponse = await login({ 
                    email: formData.email, 
                    password: formData.password 
                });
                
                // Obtener la ruta de redirección basada en el rol
                const redirectPath = getRedirectPathByRole(loginResponse.usuario?.rol || '');
                const roleDisplayName = getRoleDisplayName(loginResponse.usuario?.rol || '');
                
                console.log(`✅ Registro exitoso - Rol: ${roleDisplayName} - Redirigiendo a: ${redirectPath}`);
                
                // Redirigir según el rol
                navigate(redirectPath);
            } else {
                setError(response.mensaje || 'Error en el registro. Por favor, inténtelo de nuevo.');
            }
            
        } catch (error: any) {
            console.error('Error en registro:', error);
            
            // Manejar errores específicos del backend
            if (error.response?.data?.errors) {
                const backendErrors = error.response.data.errors;
                const errorMessages: string[] = [];
                
                Object.keys(backendErrors).forEach(key => {
                    if (Array.isArray(backendErrors[key])) {
                        errorMessages.push(...backendErrors[key]);
                    }
                });
                
                setError(errorMessages.join(', '));
            } else {
                setError(error.message || 'Error en el registro. Por favor, inténtelo de nuevo.');
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
            <div className="sm:mx-auto sm:w-full sm:max-w-md">
                <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
                    Crear cuenta de cliente
                </h2>
                <p className="mt-2 text-center text-sm text-gray-600">
                    Regístrate para acceder al catálogo y realizar pedidos
                </p>
            </div>

            <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
                <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
                    <form className="space-y-6" onSubmit={handleSubmit}>
                        {/* Información Personal */}
                        <div className="space-y-4">
                            <h3 className="text-lg font-medium text-gray-900 border-b pb-2">
                                Información Personal
                            </h3>
                            
                            {/* Nombre */}
                            <div>
                                <label htmlFor="nombre" className="block text-sm font-medium text-gray-700">
                                    Nombre *
                                </label>
                                <input
                                    id="nombre"
                                    name="nombre"
                                    type="text"
                                    required
                                    maxLength={100}
                                    value={formData.nombre}
                                    onChange={handleInputChange}
                                    className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                        errors.nombre ? 'border-red-300' : 'border-gray-300'
                                    }`}
                                    placeholder="Ingrese su nombre"
                                    disabled={loading}
                                />
                                {errors.nombre && (
                                    <p className="mt-1 text-sm text-red-600">{errors.nombre}</p>
                                )}
                            </div>

                            {/* Apellido */}
                            <div>
                                <label htmlFor="apellido" className="block text-sm font-medium text-gray-700">
                                    Apellido *
                                </label>
                                <input
                                    id="apellido"
                                    name="apellido"
                                    type="text"
                                    required
                                    maxLength={100}
                                    value={formData.apellido}
                                    onChange={handleInputChange}
                                    className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                        errors.apellido ? 'border-red-300' : 'border-gray-300'
                                    }`}
                                    placeholder="Ingrese su apellido"
                                    disabled={loading}
                                />
                                {errors.apellido && (
                                    <p className="mt-1 text-sm text-red-600">{errors.apellido}</p>
                                )}
                            </div>

                            {/* Email */}
                            <div>
                                <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                                    Correo electrónico *
                                </label>
                                <input
                                    id="email"
                                    name="email"
                                    type="email"
                                    autoComplete="email"
                                    required
                                    maxLength={100}
                                    value={formData.email}
                                    onChange={handleInputChange}
                                    className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                        errors.email ? 'border-red-300' : 'border-gray-300'
                                    }`}
                                    placeholder="ejemplo@correo.com"
                                    disabled={loading}
                                />
                                {errors.email && (
                                    <p className="mt-1 text-sm text-red-600">{errors.email}</p>
                                )}
                            </div>

                            {/* RUT */}
                            <div>
                                <label htmlFor="rut" className="block text-sm font-medium text-gray-700">
                                    RUT *
                                </label>
                                <input
                                    id="rut"
                                    name="rut"
                                    type="text"
                                    required
                                    maxLength={20}
                                    value={formData.rut}
                                    onChange={handleInputChange}
                                    className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                        errors.rut ? 'border-red-300' : 'border-gray-300'
                                    }`}
                                    placeholder="12.345.678-9"
                                    disabled={loading}
                                />
                                {errors.rut && (
                                    <p className="mt-1 text-sm text-red-600">{errors.rut}</p>
                                )}
                            </div>

                            {/* Teléfono */}
                            <div>
                                <label htmlFor="telefono" className="block text-sm font-medium text-gray-700">
                                    Teléfono *
                                </label>
                                <input
                                    id="telefono"
                                    name="telefono"
                                    type="tel"
                                    required
                                    maxLength={20}
                                    value={formData.telefono}
                                    onChange={handleInputChange}
                                    className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                        errors.telefono ? 'border-red-300' : 'border-gray-300'
                                    }`}
                                    placeholder="912345678"
                                    disabled={loading}
                                />
                                {errors.telefono && (
                                    <p className="mt-1 text-sm text-red-600">{errors.telefono}</p>
                                )}
                            </div>
                        </div>

                        {/* Dirección */}
                        <div className="space-y-4">
                            <h3 className="text-lg font-medium text-gray-900 border-b pb-2">
                                Dirección de Envío
                            </h3>
                            
                            {/* Calle y Número */}
                            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                                <div className="md:col-span-2">
                                    <label htmlFor="direccion.calle" className="block text-sm font-medium text-gray-700">
                                        Calle *
                                    </label>
                                    <input
                                        id="direccion.calle"
                                        name="direccion.calle"
                                        type="text"
                                        required
                                        maxLength={200}
                                        value={formData.direccion.calle}
                                        onChange={handleInputChange}
                                        className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                            errors.direccion?.calle ? 'border-red-300' : 'border-gray-300'
                                        }`}
                                        placeholder="Av. Providencia"
                                        disabled={loading}
                                    />
                                    {errors.direccion?.calle && (
                                        <p className="mt-1 text-sm text-red-600">{errors.direccion.calle}</p>
                                    )}
                                </div>
                                
                                <div>
                                    <label htmlFor="direccion.numero" className="block text-sm font-medium text-gray-700">
                                        Número *
                                    </label>
                                    <input
                                        id="direccion.numero"
                                        name="direccion.numero"
                                        type="text"
                                        required
                                        maxLength={20}
                                        value={formData.direccion.numero}
                                        onChange={handleInputChange}
                                        className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                            errors.direccion?.numero ? 'border-red-300' : 'border-gray-300'
                                        }`}
                                        placeholder="1234"
                                        disabled={loading}
                                    />
                                    {errors.direccion?.numero && (
                                        <p className="mt-1 text-sm text-red-600">{errors.direccion.numero}</p>
                                    )}
                                </div>
                            </div>

                            {/* Departamento */}
                            <div>
                                <label htmlFor="direccion.departamento" className="block text-sm font-medium text-gray-700">
                                    Departamento (opcional)
                                </label>
                                <input
                                    id="direccion.departamento"
                                    name="direccion.departamento"
                                    type="text"
                                    maxLength={100}
                                    value={formData.direccion.departamento}
                                    onChange={handleInputChange}
                                    className="mt-1 appearance-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                                    placeholder="Depto 45"
                                    disabled={loading}
                                />
                                {errors.direccion?.departamento && (
                                    <p className="mt-1 text-sm text-red-600">{errors.direccion.departamento}</p>
                                )}
                            </div>

                            {/* Comuna y Región */}
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div>
                                    <label htmlFor="direccion.region" className="block text-sm font-medium text-gray-700">
                                        Región *
                                    </label>
                                    <select
                                        id="direccion.region"
                                        name="direccion.region"
                                        required
                                        value={formData.direccion.region}
                                        onChange={(e) => {
                                            const region = regiones.find(r => r.nombre === e.target.value);
                                            setFormData(prev => ({
                                                ...prev,
                                                direccion: {
                                                    ...prev.direccion,
                                                    region: e.target.value,
                                                    comuna: ''
                                                }
                                            }));
                                            if (region) {
                                                handleRegionChange(region.codigo);
                                            }
                                        }}
                                        className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                            errors.direccion?.region ? 'border-red-300' : 'border-gray-300'
                                        }`}
                                        disabled={loading || loadingGeo}
                                    >
                                        <option value="">Seleccione una región</option>
                                        {regiones.map((region) => (
                                            <option key={region.codigo} value={region.nombre}>
                                                {region.nombre}
                                            </option>
                                        ))}
                                    </select>
                                    {errors.direccion?.region && (
                                        <p className="mt-1 text-sm text-red-600">{errors.direccion.region}</p>
                                    )}
                                </div>
                                
                                <div>
                                    <label htmlFor="direccion.comuna" className="block text-sm font-medium text-gray-700">
                                        Comuna *
                                    </label>
                                    <select
                                        id="direccion.comuna"
                                        name="direccion.comuna"
                                        required
                                        value={formData.direccion.comuna}
                                        onChange={(e) => {
                                            setFormData(prev => ({
                                                ...prev,
                                                direccion: {
                                                    ...prev.direccion,
                                                    comuna: e.target.value
                                                }
                                            }));
                                        }}
                                        className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                            errors.direccion?.comuna ? 'border-red-300' : 'border-gray-300'
                                        }`}
                                        disabled={loading || !formData.direccion.region || comunas.length === 0}
                                    >
                                        <option value="">Seleccione una comuna</option>
                                        {comunas.map((comuna) => (
                                            <option key={comuna.codigo} value={comuna.nombre}>
                                                {comuna.nombre}
                                            </option>
                                        ))}
                                    </select>
                                    {errors.direccion?.comuna && (
                                        <p className="mt-1 text-sm text-red-600">{errors.direccion.comuna}</p>
                                    )}
                                </div>
                            </div>

                            {/* Código Postal */}
                            <div>
                                <label htmlFor="direccion.codigoPostal" className="block text-sm font-medium text-gray-700">
                                    Código Postal (opcional)
                                </label>
                                <input
                                    id="direccion.codigoPostal"
                                    name="direccion.codigoPostal"
                                    type="text"
                                    maxLength={10}
                                    value={formData.direccion.codigoPostal}
                                    onChange={handleInputChange}
                                    className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                        errors.direccion?.codigoPostal ? 'border-red-300' : 'border-gray-300'
                                    }`}
                                    placeholder="7500000"
                                    disabled={loading}
                                />
                                {errors.direccion?.codigoPostal && (
                                    <p className="mt-1 text-sm text-red-600">{errors.direccion.codigoPostal}</p>
                                )}
                            </div>
                        </div>

                        {/* Contraseñas */}
                        <div className="space-y-4">
                            <h3 className="text-lg font-medium text-gray-900 border-b pb-2">
                                Seguridad
                            </h3>
                            
                            {/* Contraseña */}
                            <div>
                                <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                                    Contraseña *
                                </label>
                                <input
                                    id="password"
                                    name="password"
                                    type="password"
                                    autoComplete="new-password"
                                    required
                                    maxLength={100}
                                    value={formData.password}
                                    onChange={handleInputChange}
                                    className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                        errors.password ? 'border-red-300' : 'border-gray-300'
                                    }`}
                                    placeholder="Mínimo 6 caracteres"
                                    disabled={loading}
                                />
                                {errors.password && (
                                    <p className="mt-1 text-sm text-red-600">{errors.password}</p>
                                )}
                            </div>

                            {/* Confirmar Contraseña */}
                            <div>
                                <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700">
                                    Confirmar contraseña *
                                </label>
                                <input
                                    id="confirmPassword"
                                    name="confirmPassword"
                                    type="password"
                                    autoComplete="new-password"
                                    required
                                    value={formData.confirmPassword}
                                    onChange={handleInputChange}
                                    className={`mt-1 appearance-none relative block w-full px-3 py-2 border placeholder-gray-500 text-gray-900 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm ${
                                        errors.confirmPassword ? 'border-red-300' : 'border-gray-300'
                                    }`}
                                    placeholder="Confirme su contraseña"
                                    disabled={loading}
                                />
                                {errors.confirmPassword && (
                                    <p className="mt-1 text-sm text-red-600">{errors.confirmPassword}</p>
                                )}
                            </div>
                        </div>

                        {error && (
                            <div className="text-red-600 text-sm text-center bg-red-50 p-3 rounded-lg border border-red-200">
                                {error}
                            </div>
                        )}

                        <div>
                            <button
                                type="submit"
                                disabled={loading}
                                className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors duration-200"
                            >
                                {loading ? (
                                    <div className="flex items-center">
                                        <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                                        Creando cuenta...
                                    </div>
                                ) : (
                                    'Crear cuenta'
                                )}
                            </button>
                        </div>

                        <div className="text-center">
                            <p className="text-sm text-gray-600">
                                ¿Ya tienes una cuenta?{' '}
                                <Link to="/login" className="font-medium text-blue-600 hover:text-blue-500">
                                    Inicia sesión
                                </Link>
                            </p>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default Register; 