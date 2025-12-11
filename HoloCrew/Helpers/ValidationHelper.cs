using System;
using System.Text.RegularExpressions;

namespace HoloCrew.Helpers
{
    /// <summary>
    /// Helper para validación de formularios
    /// </summary>
    public static class ValidationHelper
    {
        // Regex para validación de email
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
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
        /// </summary>
        public static bool IsValidPassword(string password, int minLength = 6)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= minLength;
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
        /// Valida si un número de teléfono es válido (formato básico)
        /// </summary>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Permitir solo números, espacios, guiones y paréntesis
            var cleanPhone = Regex.Replace(phone, @"[\s\-\(\)]", "");
            return cleanPhone.Length >= 9 && cleanPhone.Length <= 15 && Regex.IsMatch(cleanPhone, @"^\d+$");
        }

        /// <summary>
        /// Valida código postal español (5 dígitos)
        /// </summary>
        public static bool IsValidSpanishPostalCode(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return false;

            return Regex.IsMatch(postalCode, @"^\d{5}$");
        }
    }
}