using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ferremas.Api.DTOs
{
    /// <summary>
    /// Modelo base para respuestas de la API
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool Exito { get; set; }

        /// <summary>
        /// Mensaje descriptivo de la respuesta
        /// </summary>
        public string Mensaje { get; set; } = string.Empty;

        /// <summary>
        /// Mensaje de error si la operación falló
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// Modelo para respuestas de la API con datos genéricos
    /// </summary>
    /// <typeparam name="T">Tipo de datos de la respuesta</typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        /// <summary>
        /// Datos de la respuesta
        /// </summary>
        public T? Datos { get; set; }

        /// <summary>
        /// Fecha y hora de la respuesta
        /// </summary>
        public DateTime FechaHora { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Lista de errores detallados
        /// </summary>
        public List<string> Errores { get; set; } = new List<string>();

        /// <summary>
        /// Metadatos adicionales de la respuesta
        /// </summary>
        public Dictionary<string, object> Metadatos { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Crea una respuesta exitosa
        /// </summary>
        public static ApiResponse<T> CrearExito(T datos, string mensaje = "Operación exitosa")
        {
            return new ApiResponse<T>
            {
                Exito = true,
                Mensaje = mensaje,
                Datos = datos
            };
        }

        /// <summary>
        /// Crea una respuesta de error
        /// </summary>
        public static ApiResponse<T> CrearError(string mensaje)
        {
            return new ApiResponse<T>
            {
                Exito = false,
                Mensaje = mensaje
            };
        }

        /// <summary>
        /// Agrega un error a la lista de errores
        /// </summary>
        public ApiResponse<T> AgregarError(string error)
        {
            Errores.Add(error);
            return this;
        }

        /// <summary>
        /// Agrega un metadato a la respuesta
        /// </summary>
        public ApiResponse<T> AgregarMetadato(string clave, object valor)
        {
            Metadatos[clave] = valor;
            return this;
        }
    }
} 