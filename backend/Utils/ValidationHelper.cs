using System;
using System.Text.RegularExpressions;
using Ferremas.Api.Exceptions;

namespace Ferremas.Api.Utils
{
    public static class ValidationHelper
    {
        public static void ValidateRut(string rut)
        {
            if (string.IsNullOrEmpty(rut))
                throw new BusinessException("El RUT no puede estar vacío");

            // Eliminar puntos y guión
            rut = rut.Replace(".", "").Replace("-", "");

            // Validar formato
            if (!Regex.IsMatch(rut, @"^\d{7,8}[0-9kK]$"))
                throw new BusinessException("El formato del RUT no es válido");

            // Validar dígito verificador
            string rutSinDv = rut.Substring(0, rut.Length - 1);
            char dv = char.ToUpper(rut[rut.Length - 1]);

            int suma = 0;
            int multiplicador = 2;

            for (int i = rutSinDv.Length - 1; i >= 0; i--)
            {
                suma += int.Parse(rutSinDv[i].ToString()) * multiplicador;
                multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
            }

            int dvEsperado = 11 - (suma % 11);
            char dvCalculado = dvEsperado == 11 ? '0' : dvEsperado == 10 ? 'K' : (char)(dvEsperado + '0');

            if (dv != dvCalculado)
                throw new BusinessException("El dígito verificador del RUT no es válido");
        }

        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new BusinessException("El email no puede estar vacío");

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new BusinessException("El formato del email no es válido");
        }

        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new BusinessException("La contraseña no puede estar vacía");

            if (password.Length < 8)
                throw new BusinessException("La contraseña debe tener al menos 8 caracteres");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                throw new BusinessException("La contraseña debe contener al menos una letra mayúscula");

            if (!Regex.IsMatch(password, @"[a-z]"))
                throw new BusinessException("La contraseña debe contener al menos una letra minúscula");

            if (!Regex.IsMatch(password, @"[0-9]"))
                throw new BusinessException("La contraseña debe contener al menos un número");

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                throw new BusinessException("La contraseña debe contener al menos un carácter especial");
        }

        public static void ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new BusinessException("El número de teléfono no puede estar vacío");

            // Eliminar espacios y guiones
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            // Validar formato chileno
            if (!Regex.IsMatch(phoneNumber, @"^\+?56?9\d{8}$"))
                throw new BusinessException("El formato del número de teléfono no es válido");
        }
    }
} 