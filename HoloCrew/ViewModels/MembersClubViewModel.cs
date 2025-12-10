using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

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
                    MinimumLevel = MembershipLevel.Gold
                },
                new MembershipReward
                {
                    Id = 5,
                    Title = "Evento VIP",
                    Description = "Entrada a un evento exclusivo",
                    Icon = "🎟️",
                    PointsCost = 1000,
                    MinimumLevel = MembershipLevel.Gold
                },
                new MembershipReward
                {
                    Id = 6,
                    Title = "Producto Exclusivo",
                    Description = "Acceso a producto de edición limitada",
                    Icon = "⭐",
                    PointsCost = 1500,
                    MinimumLevel = MembershipLevel.Platinum
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
                // TODO: Mostrar mensaje "No tienes suficientes puntos"
                return;
            }

            // Verificar nivel mínimo
            if (UserMembership.CurrentLevel < reward.MinimumLevel)
            {
                // TODO: Mostrar mensaje "Necesitas nivel X"
                return;
            }

            // Canjear recompensa
            UserMembership.TotalPoints -= reward.PointsCost;
            reward.IsRedeemed = true;
            reward.RedeemedDate = DateTime.Now;

            // TODO: Guardar en base de datos
            // TODO: Mostrar mensaje "¡Recompensa canjeada!"
        }

        [RelayCommand]
        private void ViewAllBenefits()
        {
            // TODO: Mostrar modal con todos los beneficios
        }

        [RelayCommand]
        private void ViewHistory()
        {
            // TODO: Navegar a historial de puntos y recompensas
        }

        [RelayCommand]
        private void ShareMembership()
        {
            // TODO: Compartir código de referido
        }

        [RelayCommand]
        private void ShopNow()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }
    }
}