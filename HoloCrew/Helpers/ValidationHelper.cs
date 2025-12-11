using System;
using System.Text.RegularExpressions;
using HoloCrew.Constants;

namespace HoloCrew.Helpers
{
    /// <summary>
    /// Helper para validación de formularios
    /// </summary>
    public static class ValidationHelper
    {
        // Regex para validación de email
        private static readonly Regex EmailRegex = new Regex(
            AppConstants.RegexPatterns.Email,
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // Regex para validación de teléfono español
        private static readonly Regex PhoneRegex = new Regex(
            AppConstants.RegexPatterns.Phone,
            RegexOptions.Compiled
        );

        // Regex para código postal español
        private static readonly Regex PostalCodeRegex = new Regex(
            AppConstants.RegexPatterns.PostalCode,
            RegexOptions.Compiled
        );

        /// <summary>
        /// Valida si un email tiene formato correcto
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }

        /// <summary>
        /// Valida si una contraseña cumple los requisitos mínimos
        /// ⭐ CORREGIDO: Usa AppConstants.MinPasswordLength por defecto
        /// </summary>
        public static bool IsValidPassword(string password, int minLength = 0)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Si no se especifica minLength, usar la constante
            int effectiveMinLength = minLength > 0 ? minLength : AppConstants.MinPasswordLength;

            return password.Length >= effectiveMinLength;
        }

        /// <summary>
        /// Valida si dos contraseñas coinciden
        /// </summary>
        public static bool PasswordsMatch(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                return false;

            return password == confirmPassword;
        }

        /// <summary>
        /// Valida si un string no está vacío
        /// </summary>
        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Valida si un número de teléfono español es válido
        /// </summary>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Usar el regex de AppConstants para teléfonos españoles
            return PhoneRegex.IsMatch(phone);
        }

        /// <summary>
        /// Valida código postal español (5 dígitos, provincias válidas 01-52)
        /// </summary>
        public static bool IsValidSpanishPostalCode(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return false;

            return PostalCodeRegex.IsMatch(postalCode);
        }

        /// <summary>
        /// Valida si un nombre cumple los requisitos de longitud
        /// </summary>
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return name.Length >= AppConstants.MinNameLength &&
                   name.Length <= AppConstants.MaxNameLength;
        }
    }
}