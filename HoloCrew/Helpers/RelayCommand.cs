using System.Text.RegularExpressions;

namespace HoloCrew.Helpers
{
    /// <summary>
    /// Funciones de validación reutilizables
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Valida formato de email
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida formato de teléfono español
        /// </summary>
        public static bool IsValidSpanishPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Formato: +34 XXX XXX XXX o similar
            var regex = new Regex(@"^(\+34|0034|34)?[ -]?[6-9]\d{2}[ -]?\d{3}[ -]?\d{3}$");
            return regex.IsMatch(phone);
        }

        /// <summary>
        /// Valida código postal español
        /// </summary>
        public static bool IsValidSpanishPostalCode(string postalCode)
        {
            if (string.IsNullOrWhiteSpace(postalCode))
                return false;

            // Formato: 5 dígitos (01000 - 52999)
            var regex = new Regex(@"^(?:0[1-9]|[1-4]\d|5[0-2])\d{3}$");
            return regex.IsMatch(postalCode);
        }

        /// <summary>
        /// Valida contraseña segura
        /// Mínimo 8 caracteres, al menos una mayúscula, una minúscula y un número
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasUpper && hasLower && hasDigit;
        }

        /// <summary>
        /// Valida número de tarjeta de crédito (algoritmo de Luhn)
        /// </summary>
        public static bool IsValidCreditCard(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            // Remover espacios y guiones
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            // Debe tener entre 13 y 19 dígitos
            if (cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            // Algoritmo de Luhn
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(cardNumber[i]))
                    return false;

                int digit = cardNumber[i] - '0';

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }

        /// <summary>
        /// Limpia un string dejando solo números
        /// </summary>
        public static string ExtractNumbers(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return Regex.Replace(input, @"[^\d]", "");
        }
    }
}