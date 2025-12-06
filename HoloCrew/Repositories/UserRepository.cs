using HoloCrew.Models;
using HoloCrew.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.Repositories
{
    /// <summary>
    /// Implementación del repositorio de usuarios
    /// NOTA: Usa datos MOCK en memoria
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private static List<User> _users;
        private static int _nextId = 1;

        public UserRepository()
        {
            if (_users == null)
            {
                InitializeMockData();
            }
        }

        public Task<User> GetByIdAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User> GetByEmailAsync(string email)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<User> CreateAsync(User user)
        {
            user.Id = _nextId++;
            user.CreatedAt = DateTime.Now;

            // Inicializar listas vacías si son null
            user.Addresses ??= new List<Address>();
            user.PaymentMethods ??= new List<PaymentMethod>();
            user.NotificationPreferences ??= new NotificationSettings
            {
                OrderUpdatesEnabled = true,
                PromotionsEnabled = true,
                NewProductsEnabled = true,
                PriceAlertsEnabled = false,
                EmailNotificationsEnabled = true,
                PushNotificationsEnabled = true
            };

            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> UpdateAsync(User user)
        {
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                var index = _users.IndexOf(existing);
                _users[index] = user;
                return Task.FromResult(user);
            }
            return Task.FromResult<User>(null);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _users.Remove(user);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            var exists = _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exists);
        }

        public Task<List<User>> GetAllAsync()
        {
            return Task.FromResult(_users.ToList());
        }

        public Task UpdateLastLoginAsync(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.Now; // ⭐ Ahora funciona con la nueva propiedad
            }
            return Task.CompletedTask;
        }

        // ⭐ AGREGADO - Método necesario para AuthenticationService
        public Task<User> ValidateCredentialsAsync(string email, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password); // En producción debería comparar hash

            return Task.FromResult(user);
        }

        private void InitializeMockData()
        {
            _users = new List<User>
            {
                new User
                {
                    Id = _nextId++,
                    FullName = "Juan Pérez",
                    Email = "juan@example.com",
                    Password = "demo123", // ⭐ AGREGADO - Password para login
                    PhoneNumber = "+34 600 123 456",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    CreatedAt = DateTime.Now.AddYears(-2),
                    Addresses = new List<Address>
                    {
                        new Address
                        {
                            Id = 1,
                            Label = "Casa",
                            FullName = "Juan Pérez",
                            PhoneNumber = "+34 600 123 456",
                            AddressLine1 = "Calle Mayor 123",
                            City = "Madrid",
                            State = "Madrid",
                            PostalCode = "28001",
                            Country = "España",
                            IsDefault = true
                        }
                    },
                    PaymentMethods = new List<PaymentMethod>
                    {
                        new PaymentMethod
                        {
                            Id = 1,
                            Type = PaymentType.CreditCard,
                            CardholderName = "Juan Pérez",
                            CardNumberMasked = "**** **** **** 1234",
                            ExpirationDate = "12/25",
                            IsDefault = true
                        }
                    },
                    NotificationPreferences = new NotificationSettings
                    {
                        OrderUpdatesEnabled = true,
                        PromotionsEnabled = true,
                        NewProductsEnabled = true,
                        PriceAlertsEnabled = false,
                        EmailNotificationsEnabled = true,
                        PushNotificationsEnabled = true
                    }
                },
                new User
                {
                    Id = _nextId++,
                    FullName = "María García",
                    Email = "maria@example.com",
                    Password = "demo123", // ⭐ AGREGADO - Password para login
                    PhoneNumber = "+34 600 654 321",
                    DateOfBirth = new DateTime(1985, 8, 22),
                    CreatedAt = DateTime.Now.AddYears(-1),
                    Addresses = new List<Address>(),
                    PaymentMethods = new List<PaymentMethod>(),
                    NotificationPreferences = new NotificationSettings
                    {
                        OrderUpdatesEnabled = true,
                        PromotionsEnabled = false,
                        NewProductsEnabled = true,
                        PriceAlertsEnabled = true,
                        EmailNotificationsEnabled = true,
                        PushNotificationsEnabled = false
                    }
                }
            };
        }
    }
}