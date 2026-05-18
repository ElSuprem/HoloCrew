using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// ViewModel del Members Club.
// Página informativa con dos modos: sin sesión (CTA para registrarse) y con sesión
// (zona personal con tier actual, créditos, lifetime points y progreso al siguiente nivel).
// Se conecta con IMembershipService, IAuthenticationService y INavigationService.

namespace HoloCrew.ViewModels
{
    public partial class MembersClubViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IMembershipService _membershipService;
        private readonly IAuthenticationService _authService;

        #region Observable Properties

        // Estado de sesión: controla qué se muestra (CTA público vs zona personal).
        [ObservableProperty]
        private bool _isLoggedIn;

        // Datos del usuario logueado (null si no hay sesión).
        [ObservableProperty]
        private User? _currentUser;

        // Los 4 tiers (Bronze, Silver, Gold, Platinum), cargados desde Supabase.
        [ObservableProperty]
        private ObservableCollection<MembershipTierConfig> _allTiers = new();

        // Tier actual del usuario (null si no hay sesión).
        [ObservableProperty]
        private MembershipTierConfig? _currentTier;

        // Siguiente tier (null si ya está en el más alto o no hay sesión).
        [ObservableProperty]
        private MembershipTierConfig? _nextTier;

        // Progreso al siguiente tier en porcentaje (0-100).
        [ObservableProperty]
        private double _progressPercentage;

        // Puntos que faltan para subir de tier.
        [ObservableProperty]
        private int _pointsToNextTier;

        #endregion

        public MembersClubViewModel(
            INavigationService navigationService,
            IMembershipService membershipService,
            IAuthenticationService authService)
        {
            _navigationService = navigationService;
            _membershipService = membershipService;
            _authService = authService;

            Title = "Members Club";

            // Reaccionar a login/logout para refrescar la vista automáticamente.
            _authService.AuthStateChanged += OnAuthStateChanged;
        }


        public override async void OnNavigatedTo(object? parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadAsync();
        }


        // Carga los tiers desde Supabase y, si hay sesión, los datos personales del usuario.
        private async Task LoadAsync()
        {
            await ExecuteAsync(async () =>
            {
                LoadingMessage = "Loading membership info...";

                // Los tiers se cargan siempre (visibles para todos los usuarios).
                var tiers = await _membershipService.GetAllTiersAsync();
                AllTiers = new ObservableCollection<MembershipTierConfig>(tiers);

                IsLoggedIn = await _authService.IsAuthenticatedAsync();

                if (IsLoggedIn)
                {
                    CurrentUser = _authService.GetCurrentUser();

                    if (CurrentUser != null)
                    {
                        CurrentTier = _membershipService.GetCurrentTier(CurrentUser.LifetimePoints, tiers);
                        NextTier = _membershipService.GetNextTier(CurrentUser.LifetimePoints, tiers);
                        ProgressPercentage = _membershipService.GetProgressPercentage(CurrentUser.LifetimePoints, tiers);
                        PointsToNextTier = _membershipService.GetPointsToNextTier(CurrentUser.LifetimePoints, tiers);
                    }
                }
                else
                {
                    CurrentUser = null;
                    CurrentTier = null;
                    NextTier = null;
                    ProgressPercentage = 0;
                    PointsToNextTier = 0;
                }

                SetSuccess();
            });
        }


        // Cuando el usuario hace login o logout, recargamos para reflejar el cambio
        // (cambia el modo público <-> personal).
        private async void OnAuthStateChanged(object? sender, EventArgs e)
        {
            await LoadAsync();
        }


        #region Commands

        [RelayCommand]
        private void GoToLogin() => _navigationService.NavigateTo<LoginViewModel>();

        [RelayCommand]
        private void GoToRegister() => _navigationService.NavigateTo<RegisterViewModel>();

        [RelayCommand]
        private void ShopNow() => _navigationService.NavigateTo<ProductCatalogViewModel>();

        #endregion
    }
}