using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace HoloCrew.ViewModels
{
    public partial class MembersClubViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private UserMembership _userMembership;

        [ObservableProperty]
        private ObservableCollection<MembershipTier> _membershipTiers = new();

        [ObservableProperty]
        private ObservableCollection<MembershipReward> _availableRewards = new();

        [ObservableProperty]
        private MembershipTier _currentTier;

        [ObservableProperty]
        private MembershipTier _nextTier;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _showStatusMessage = false;

        [ObservableProperty]
        private bool _isStatusSuccess = true;

        public MembersClubViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Title = "Members Club";

            InitializeMembershipData();
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            LoadUserMembership();
        }

        private void InitializeMembershipData()
        {
            // Definir los 4 niveles de membresía
            MembershipTiers = new ObservableCollection<MembershipTier>
            {
                new MembershipTier
                {
                    Level = MembershipLevel.Bronze,
                    Name = "BRONZE",
                    Icon = "🥉",
                    Color = "#CD7F32",
                    PointsRequired = 0,
                    DiscountPercentage = 5,
                    FreeShipping = false,
                    EarlyAccess = false,
                    BirthdayGift = false,
                    ExclusiveProducts = false,
                    PointsMultiplier = 1,
                    Benefits = new List<string>
                    {
                        "5% de descuento en todos los productos",
                        "Gana 1 punto por cada €1 gastado",
                        "Acceso a ofertas exclusivas",
                        "Newsletter mensual"
                    }
                },
                new MembershipTier
                {
                    Level = MembershipLevel.Silver,
                    Name = "SILVER",
                    Icon = "🥈",
                    Color = "#C0C0C0",
                    PointsRequired = 500,
                    DiscountPercentage = 10,
                    FreeShipping = true,
                    EarlyAccess = false,
                    BirthdayGift = true,
                    ExclusiveProducts = false,
                    PointsMultiplier = 2,
                    Benefits = new List<string>
                    {
                        "10% de descuento en todos los productos",
                        "Gana 2 puntos por cada €1 gastado",
                        "Envío GRATIS en todos los pedidos",
                        "Regalo de cumpleaños",
                        "Devoluciones gratis extendidas (60 días)"
                    }
                },
                new MembershipTier
                {
                    Level = MembershipLevel.Gold,
                    Name = "GOLD",
                    Icon = "🥇",
                    Color = "#FFD700",
                    PointsRequired = 1500,
                    DiscountPercentage = 15,
                    FreeShipping = true,
                    EarlyAccess = true,
                    BirthdayGift = true,
                    ExclusiveProducts = false,
                    PointsMultiplier = 3,
                    Benefits = new List<string>
                    {
                        "15% de descuento en todos los productos",
                        "Gana 3 puntos por cada €1 gastado",
                        "Acceso anticipado a nuevos lanzamientos",
                        "Envío express gratis",
                        "Soporte prioritario 24/7",
                        "Invitaciones a eventos exclusivos"
                    }
                },
                new MembershipTier
                {
                    Level = MembershipLevel.Platinum,
                    Name = "PLATINUM",
                    Icon = "💎",
                    Color = "#E5E4E2",
                    PointsRequired = 3000,
                    DiscountPercentage = 20,
                    FreeShipping = true,
                    EarlyAccess = true,
                    BirthdayGift = true,
                    ExclusiveProducts = true,
                    PointsMultiplier = 4,
                    Benefits = new List<string>
                    {
                        "20% de descuento en TODOS los productos",
                        "Gana 4 puntos por cada €1 gastado",
                        "Productos exclusivos de edición limitada",
                        "Servicio de styling personal gratuito",
                        "Acceso VIP a eventos y lanzamientos",
                        "Regalos sorpresa trimestrales",
                        "Concierge personal para compras"
                    }
                }
            };

            // Recompensas disponibles
            AvailableRewards = new ObservableCollection<MembershipReward>
            {
                new MembershipReward
                {
                    Id = 1,
                    Title = "Cupón €10",
                    Description = "Descuento de €10 en tu próxima compra",
                    Icon = "💰",
                    PointsCost = 200,
                    MinimumLevel = MembershipLevel.Bronze
                },
                new MembershipReward
                {
                    Id = 2,
                    Title = "Envío Express Gratis",
                    Description = "Un envío express gratuito",
                    Icon = "🚀",
                    PointsCost = 150,
                    MinimumLevel = MembershipLevel.Silver
                },
                new MembershipReward
                {
                    Id = 3,
                    Title = "Cupón €25",
                    Description = "Descuento de €25 en tu próxima compra",
                    Icon = "💵",
                    PointsCost = 450,
                    MinimumLevel = MembershipLevel.Silver
                },
                new MembershipReward
                {
                    Id = 4,
                    Title = "Producto Gratis",
                    Description = "Elige cualquier producto hasta €50",
                    Icon = "🎁",
                    PointsCost = 800,
                    MinimumLevel = MembershipLevel.Silver
                },
                new MembershipReward
                {
                    Id = 5,
                    Title = "Evento VIP",
                    Description = "Entrada a un evento exclusivo",
                    Icon = "🎟️",
                    PointsCost = 1000,
                    MinimumLevel = MembershipLevel.Silver
                },
                new MembershipReward
                {
                    Id = 6,
                    Title = "Producto Exclusivo",
                    Description = "Acceso a producto de edición limitada",
                    Icon = "⭐",
                    PointsCost = 1200,
                    MinimumLevel = MembershipLevel.Silver
                }
            };
        }

        private void LoadUserMembership()
        {
            // ⭐ DATOS MOCK - Usuario en nivel SILVER con progreso hacia GOLD
            UserMembership = new UserMembership
            {
                UserId = 1,
                CurrentLevel = MembershipLevel.Silver,
                TotalPoints = 1250,
                CurrentLevelPoints = 750, // Puntos desde que alcanzó Silver (500)
                PointsToNextLevel = 250,  // Necesita 1500 para Gold (1250 actual)
                TotalSpent = 1250.00m,
                TotalOrders = 15,
                MemberSince = DateTime.Now.AddMonths(-8),
                ProgressPercentage = 75.0, // 750/1000 * 100
                NextLevel = MembershipLevel.Gold
            };

            // Actualizar tier actual y siguiente
            CurrentTier = MembershipTiers.FirstOrDefault(t => t.Level == UserMembership.CurrentLevel);
            NextTier = MembershipTiers.FirstOrDefault(t => t.Level == UserMembership.NextLevel);
        }

        [RelayCommand]
        private void RedeemReward(MembershipReward reward)
        {
            if (reward == null) return;

            // Verificar si tiene suficientes puntos
            if (UserMembership.TotalPoints < reward.PointsCost)
            {
                ShowStatus($"No tienes suficientes puntos. Necesitas {reward.PointsCost} puntos.", false);
                return;
            }

            // Verificar nivel mínimo (comparar valores numéricos del enum)
            if ((int)UserMembership.CurrentLevel < (int)reward.MinimumLevel)
            {
                ShowStatus($"Necesitas nivel {reward.MinimumLevel} para canjear esta recompensa.", false);
                return;
            }

            // Verificar si ya fue canjeado
            if (reward.IsRedeemed)
            {
                ShowStatus("Esta recompensa ya fue canjeada.", false);
                return;
            }

            // ⭐ CANJEAR RECOMPENSA - Actualizar puntos
            var newPoints = UserMembership.TotalPoints - reward.PointsCost;

            // Crear nuevo objeto UserMembership para forzar actualización de UI
            UserMembership = new UserMembership
            {
                UserId = UserMembership.UserId,
                CurrentLevel = UserMembership.CurrentLevel,
                TotalPoints = newPoints,
                CurrentLevelPoints = UserMembership.CurrentLevelPoints,
                PointsToNextLevel = UserMembership.PointsToNextLevel + reward.PointsCost,
                TotalSpent = UserMembership.TotalSpent,
                TotalOrders = UserMembership.TotalOrders,
                MemberSince = UserMembership.MemberSince,
                ProgressPercentage = CalculateProgressPercentage(newPoints),
                NextLevel = UserMembership.NextLevel
            };

            // Marcar recompensa como canjeada
            reward.IsRedeemed = true;
            reward.RedeemedDate = DateTime.Now;

            // Actualizar la lista de recompensas para refrescar UI
            var index = AvailableRewards.IndexOf(reward);
            if (index >= 0)
            {
                AvailableRewards.RemoveAt(index);
                AvailableRewards.Insert(index, reward);
            }

            ShowStatus($"🎉 ¡{reward.Title} canjeado! Te quedan {newPoints} puntos.", true);
        }

        private double CalculateProgressPercentage(int currentPoints)
        {
            // Silver (500) -> Gold (1500) = 1000 puntos de diferencia
            var pointsInCurrentLevel = currentPoints - 500; // Puntos desde Silver
            var pointsNeeded = 1000; // De Silver a Gold
            return Math.Min(100, (pointsInCurrentLevel / (double)pointsNeeded) * 100);
        }

        private async void ShowStatus(string message, bool isSuccess)
        {
            StatusMessage = message;
            IsStatusSuccess = isSuccess;
            ShowStatusMessage = true;

            // Ocultar mensaje después de 3 segundos
            await Task.Delay(3000);
            ShowStatusMessage = false;
        }

        [RelayCommand]
        private void ViewAllBenefits()
        {
            MessageBox.Show(
                "🥉 BRONZE: 5% descuento, 1x puntos\n" +
                "🥈 SILVER: 10% descuento, 2x puntos, envío gratis\n" +
                "🥇 GOLD: 15% descuento, 3x puntos, acceso anticipado\n" +
                "💎 PLATINUM: 20% descuento, 4x puntos, productos exclusivos",
                "Todos los Beneficios",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ViewHistory()
        {
            MessageBox.Show(
                "📜 Historial de Puntos\n\n" +
                "• +150 pts - Pedido #12345 (15/01/2025)\n" +
                "• +200 pts - Pedido #12340 (10/01/2025)\n" +
                "• -200 pts - Cupón €10 canjeado (05/01/2025)\n" +
                "• +100 pts - Pedido #12335 (28/12/2024)\n" +
                "• +500 pts - Bono bienvenida Silver (01/12/2024)",
                "Historial de Puntos",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ShareMembership()
        {
            MessageBox.Show(
                "🎁 ¡Comparte tu código de referido!\n\n" +
                "Tu código: HOLOCREW-USER123\n\n" +
                "Tus amigos obtienen 10% de descuento\n" +
                "Tú ganas 100 puntos por cada referido",
                "Programa de Referidos",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ShopNow()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }
    }
}