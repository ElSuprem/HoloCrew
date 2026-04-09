using System;
using System.Text.RegularExpressions;
using HoloCrew.Constants;

// Valida emails, contraseñas (mínimo 8 caracteres), teléfonos españoles,
// códigos postales españoles, nombres (2-100 caracteres) y campos vacíos.
// Los patrones regex están en AppConstants.

namespace HoloCrew.Helpers
{
    public static class ValidationHelper
    {
        // los patrones regex (email, telefono, codigo postal) están definidos en AppConstants
        // así se pueden cambiar desde un solo sitio si hace falta
        private static readonly Regex EmailRegex = new Regex(
            AppConstants.RegexPatterns.Email,
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        private static readonly Regex PhoneRegex = new Regex(
            AppConstants.RegexPatterns.Phone,
            RegexOptions.Compiled
        );

        private static readonly Regex PostalCodeRegex = new Regex(
            AppConstants.RegexPatterns.PostalCode,
            RegexOptions.Compiled
        );

        // comprueba si el email tiene un formato válido (ejemplo: nombre@dominio.com)
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }

        // comprueba si la contraseña tiene la longitud mínima (por defecto usa la constante de AppConstants.MinPasswordLength que son 8 caracteres)
        public static bool IsValidPassword(string password, int minLength = 0)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            int effectiveMinLength = minLength > 0 ? minLength : AppConstants.MinPasswordLength;

            return password.Length >= effectiveMinLength;
        }

        // comprueba que la contraseña y su confirmación sean iguales (para formularios de registro)
        public static bool PasswordsMatch(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                return false;

            return password == confirmPassword;
        }

        // comprueba que un texto no esté vacío (para campos obligatorios)
        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        // comprueba si el teléfono tiene formato español válido (usa el patrón de AppConstants)
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return PhoneRegex.IsMatch(phone);
        }

        // comprueba si el código postal español es válido (5 dígitos, provincias 01-52)
        public static bool IsValidSpanishPostalCode(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return false;

            return PostalCodeRegex.IsMatch(postalCode);
        }

        // comprueba que el nombre tenga entre 2 y 100 caracteres (los límites están en AppConstants)
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return name.Length >= AppConstants.MinNameLength &&
                   name.Length <= AppConstants.MaxNameLength;
        }
    }
}