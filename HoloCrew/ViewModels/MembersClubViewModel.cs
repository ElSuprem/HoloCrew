using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

// ViewModel de la página del club de miembros (Members Club).
// Muestra los niveles de membresía (Bronce, Plata, Oro, Platino), puntos del usuario,
// recompensas disponibles para canjear, y progreso hacia el siguiente nivel.
// Se conecta con NavigationService.

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
            // los 4 niveles de membresía
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
                        "5% discount on all products",
                        "Earn 1 point per €1 spent",
                        "Access to exclusive offers",
                        "Monthly newsletter"
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
                        "10% discount on all products",
                        "Earn 2 points per €1 spent",
                        "FREE shipping on all orders",
                        "Birthday gift",
                        "Extended free returns (60 days)"
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
                        "15% discount on all products",
                        "Earn 3 points per €1 spent",
                        "Early access to new launches",
                        "Free express shipping",
                        "Priority 24/7 support",
                        "Invitations to exclusive events"
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
                        "20% discount on ALL products",
                        "Earn 4 points per €1 spent",
                        "Exclusive limited edition products",
                        "Free personal styling service",
                        "VIP access to events and launches",
                        "Quarterly surprise gifts",
                        "Personal shopping concierge"
                    }
                }
            };

            // recompensas disponibles para canjear con puntos
            AvailableRewards = new ObservableCollection<MembershipReward>
            {
                new MembershipReward { Id = 1, Title = "€10 Coupon", Description = "€10 discount on your next purchase", Icon = "💰", PointsCost = 200, MinimumLevel = MembershipLevel.Bronze },
                new MembershipReward { Id = 2, Title = "Free Express Shipping", Description = "One free express delivery", Icon = "🚀", PointsCost = 150, MinimumLevel = MembershipLevel.Silver },
                new MembershipReward { Id = 3, Title = "€25 Coupon", Description = "€25 discount on your next purchase", Icon = "💵", PointsCost = 450, MinimumLevel = MembershipLevel.Silver },
                new MembershipReward { Id = 4, Title = "Free Product", Description = "Choose any product up to €50", Icon = "🎁", PointsCost = 800, MinimumLevel = MembershipLevel.Silver },
                new MembershipReward { Id = 5, Title = "VIP Event", Description = "Ticket to an exclusive event", Icon = "🎟️", PointsCost = 1000, MinimumLevel = MembershipLevel.Silver },
                new MembershipReward { Id = 6, Title = "Exclusive Product", Description = "Access to limited edition product", Icon = "★", PointsCost = 1200, MinimumLevel = MembershipLevel.Silver }
            };
        }

        // datos de ejemplo del usuario (nivel Silver con progreso hacia Gold)
        private void LoadUserMembership()
        {
            UserMembership = new UserMembership
            {
                UserId = 1,
                CurrentLevel = MembershipLevel.Silver,
                TotalPoints = 1250,
                CurrentLevelPoints = 750,
                PointsToNextLevel = 250,
                TotalSpent = 1250.00m,
                TotalOrders = 15,
                MemberSince = DateTime.Now.AddMonths(-8),
                ProgressPercentage = 75.0,
                NextLevel = MembershipLevel.Gold
            };

            CurrentTier = MembershipTiers.FirstOrDefault(t => t.Level == UserMembership.CurrentLevel);
            NextTier = MembershipTiers.FirstOrDefault(t => t.Level == UserMembership.NextLevel);
        }

        [RelayCommand]
        private void RedeemReward(MembershipReward reward)
        {
            if (reward == null) return;

            if (UserMembership.TotalPoints < reward.PointsCost)
            {
                ShowStatus($"Not enough points. You need {reward.PointsCost} points.", false);
                return;
            }

            if ((int)UserMembership.CurrentLevel < (int)reward.MinimumLevel)
            {
                ShowStatus($"You need level {reward.MinimumLevel} to redeem this reward.", false);
                return;
            }

            if (reward.IsRedeemed)
            {
                ShowStatus("This reward has already been redeemed.", false);
                return;
            }

            var newPoints = UserMembership.TotalPoints - reward.PointsCost;

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

            reward.IsRedeemed = true;
            reward.RedeemedDate = DateTime.Now;

            var index = AvailableRewards.IndexOf(reward);
            if (index >= 0)
            {
                AvailableRewards.RemoveAt(index);
                AvailableRewards.Insert(index, reward);
            }

            ShowStatus($"{reward.Title} redeemed! You have {newPoints} points remaining.", true);
        }

        private double CalculateProgressPercentage(int currentPoints)
        {
            var pointsInCurrentLevel = currentPoints - 500;
            var pointsNeeded = 1000;
            return Math.Min(100, (pointsInCurrentLevel / (double)pointsNeeded) * 100);
        }

        private async void ShowStatus(string message, bool isSuccess)
        {
            StatusMessage = message;
            IsStatusSuccess = isSuccess;
            ShowStatusMessage = true;

            await Task.Delay(3000);
            ShowStatusMessage = false;
        }

        [RelayCommand]
        private void ViewAllBenefits()
        {
            MessageBox.Show(
                "BRONZE: 5% discount, 1x points\n" +
                "SILVER: 10% discount, 2x points, free shipping\n" +
                "GOLD: 15% discount, 3x points, early access\n" +
                "PLATINUM: 20% discount, 4x points, exclusive products",
                "All Benefits",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ViewHistory()
        {
            MessageBox.Show(
                "Points History\n\n" +
                "• +150 pts - Pedido #12345 (15/01/2025)\n" +
                "• +200 pts - Pedido #12340 (10/01/2025)\n" +
                "• -200 pts - €10 Coupon canjeado (05/01/2025)\n" +
                "• +100 pts - Pedido #12335 (28/12/2024)\n" +
                "• +500 pts - Silver welcome bonus (01/12/2024)",
                "Points History",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ShareMembership()
        {
            MessageBox.Show(
                "Share your referral code!\n\n" +
                "Your code: HOLOCREW-USER123\n\n" +
                "Your friends get 10% off\n" +
                "You earn 100 points per referral",
                "Referral Program",
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