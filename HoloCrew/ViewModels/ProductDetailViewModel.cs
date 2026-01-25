using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class ProductDetailViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;
        private readonly INavigationService _navigationService;

        #region Observable Properties

        [ObservableProperty]
        private Product _product;

        [ObservableProperty]
        private string _selectedImage;

        [ObservableProperty]
        private int _selectedImageIndex = 0;

        [ObservableProperty]
        private ObservableCollection<string> _images = new();

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private ObservableCollection<Review> _reviews = new();

        [ObservableProperty]
        private ObservableCollection<Product> _relatedProducts = new();

        [ObservableProperty]
        private bool _isInWishlist;

        // ══════════════════════════════════════════════
        // SELECCIÓN DE COLOR
        // ══════════════════════════════════════════════
        [ObservableProperty]
        private string _selectedColor = "Black";

        [ObservableProperty]
        private ObservableCollection<ColorOption> _availableColors = new();

        // ══════════════════════════════════════════════
        // SELECCIÓN DE TALLA
        // ══════════════════════════════════════════════
        [ObservableProperty]
        private string _selectedSize = "M";

        [ObservableProperty]
        private ObservableCollection<SizeOption> _availableSizes = new();

        // ══════════════════════════════════════════════
        // SIZE GUIDE
        // ══════════════════════════════════════════════
        [ObservableProperty]
        private bool _isSizeGuideOpen = false;

        // ══════════════════════════════════════════════
        // WRITE REVIEW
        // ══════════════════════════════════════════════
        [ObservableProperty]
        private bool _isWriteReviewOpen = false;

        [ObservableProperty]
        private int _newReviewRating = 5;

        [ObservableProperty]
        private string _newReviewTitle = string.Empty;

        [ObservableProperty]
        private string _newReviewText = string.Empty;

        #endregion

        public ProductDetailViewModel(
            IProductService productService,
            ICartService cartService,
            IWishlistService wishlistService,
            INavigationService navigationService)
        {
            _productService = productService;
            _cartService = cartService;
            _wishlistService = wishlistService;
            _navigationService = navigationService;

            Title = "Product Detail";
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is int productId)
            {
                await LoadProductAsync(productId);
            }
        }

        private async Task LoadProductAsync(int productId)
        {
            await ExecuteAsync(async () =>
            {
                Product = await _productService.GetProductByIdAsync(productId);

                if (Product != null)
                {
                    // Imágenes - asegurar que siempre hay 4
                    var imageList = Product.ImageUrls?.ToList() ?? new List<string>();
                    while (imageList.Count < 4)
                    {
                        imageList.Add($"placeholder{imageList.Count + 1}");
                    }
                    Images = new ObservableCollection<string>(imageList.Take(4));
                    SelectedImage = Images.FirstOrDefault() ?? "placeholder1";
                    SelectedImageIndex = 0;

                    // Colores disponibles
                    SetupAvailableColors();

                    // Tallas disponibles
                    SetupAvailableSizes();

                    // Productos relacionados
                    var related = await _productService.GetRelatedProductsAsync(productId);
                    RelatedProducts = new ObservableCollection<Product>(related);

                    // Wishlist
                    IsInWishlist = await _wishlistService.IsInWishlistAsync(productId);

                    // Reset quantity
                    Quantity = 1;

                    SetSuccess();
                }
                else
                {
                    SetError(AppConstants.Errors.ProductNotAvailable);
                }
            });
        }

        private void SetupAvailableColors()
        {
            var colors = Product.AvailableColors ?? new List<string> { "Black", "White", "Gray", "Navy" };

            AvailableColors.Clear();
            foreach (var color in colors)
            {
                AvailableColors.Add(new ColorOption
                {
                    Name = color,
                    HexCode = GetColorHex(color),
                    IsSelected = color == SelectedColor
                });
            }

            // Si el color seleccionado no está disponible, seleccionar el primero
            if (!colors.Contains(SelectedColor) && colors.Any())
            {
                SelectColor(colors.First());
            }
        }

        private void SetupAvailableSizes()
        {
            var productSizes = Product.AvailableSizes ?? new List<string> { "XS", "S", "M", "L", "XL" };
            var allSizes = new[] { "XS", "S", "M", "L", "XL", "XXL" };

            AvailableSizes.Clear();
            foreach (var size in allSizes)
            {
                // Una talla está disponible si está en la lista del producto
                bool isAvailable = productSizes.Contains(size);

                AvailableSizes.Add(new SizeOption
                {
                    Size = size,
                    IsAvailable = isAvailable,
                    IsSelected = size == SelectedSize && isAvailable
                });
            }

            // Si la talla seleccionada no está disponible, seleccionar la primera disponible
            var currentSelection = AvailableSizes.FirstOrDefault(s => s.Size == SelectedSize);
            if (currentSelection == null || !currentSelection.IsAvailable)
            {
                var firstAvailable = AvailableSizes.FirstOrDefault(s => s.IsAvailable);
                if (firstAvailable != null)
                {
                    SelectedSize = firstAvailable.Size;
                    firstAvailable.IsSelected = true;
                }
            }
        }

        private string GetColorHex(string colorName) => colorName.ToLower() switch
        {
            "black" => "#0A0A0A",
            "white" => "#FFFFFF",
            "gray" or "grey" => "#6B7280",
            "navy" => "#1E3A5F",
            "red" => "#DC2626",
            "green" => "#16A34A",
            "blue" => "#2563EB",
            "beige" => "#D4C4A8",
            "brown" => "#78350F",
            _ => "#6B7280"
        };

        #region Commands - Cart & Wishlist

        [RelayCommand]
        private async Task AddToCartAsync()
        {
            if (Product == null || Quantity < 1) return;

            try
            {
                await _cartService.AddToCartAsync(Product, Quantity);
                // TODO: Mostrar notificación de éxito
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task BuyNowAsync()
        {
            await AddToCartAsync();
            _navigationService.NavigateTo<CheckoutViewModel>();
        }

        [RelayCommand]
        private async Task ToggleWishlistAsync()
        {
            if (Product == null) return;

            IsInWishlist = !IsInWishlist;

            try
            {
                if (IsInWishlist)
                    await _wishlistService.AddToWishlistAsync(Product);
                else
                    await _wishlistService.RemoveFromWishlistAsync(Product.Id);
            }
            catch (Exception ex)
            {
                IsInWishlist = !IsInWishlist;
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        #endregion

        #region Commands - Quantity

        [RelayCommand]
        private void IncreaseQuantity()
        {
            if (Product != null && Quantity < Product.Stock)
            {
                Quantity++;
            }
        }

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
        }

        #endregion

        #region Commands - Image Gallery

        [RelayCommand]
        private void NextImage()
        {
            if (Images.Count == 0) return;

            SelectedImageIndex = (SelectedImageIndex + 1) % Images.Count;
            SelectedImage = Images[SelectedImageIndex];
            UpdateThumbnailSelection();
        }

        [RelayCommand]
        private void PreviousImage()
        {
            if (Images.Count == 0) return;

            SelectedImageIndex = SelectedImageIndex - 1;
            if (SelectedImageIndex < 0) SelectedImageIndex = Images.Count - 1;
            SelectedImage = Images[SelectedImageIndex];
            UpdateThumbnailSelection();
        }

        [RelayCommand]
        private void SelectImage(object indexObj)
        {
            int index = 0;
            if (indexObj is int i)
                index = i;
            else if (indexObj is string s && int.TryParse(s, out int parsed))
                index = parsed;

            if (index >= 0 && index < Images.Count)
            {
                SelectedImageIndex = index;
                SelectedImage = Images[index];
                UpdateThumbnailSelection();
            }
        }

        private void UpdateThumbnailSelection()
        {
            // Notificar cambio para actualizar UI de thumbnails
            OnPropertyChanged(nameof(SelectedImageIndex));
        }

        #endregion

        #region Commands - Color Selection

        [RelayCommand]
        private void SelectColor(string colorName)
        {
            if (string.IsNullOrEmpty(colorName)) return;

            SelectedColor = colorName;

            // Actualizar selección visual
            foreach (var color in AvailableColors)
            {
                color.IsSelected = color.Name == colorName;
            }

            // Forzar actualización de la colección
            OnPropertyChanged(nameof(AvailableColors));
        }

        #endregion

        #region Commands - Size Selection

        [RelayCommand]
        private void SelectSize(string size)
        {
            if (string.IsNullOrEmpty(size)) return;

            var sizeOption = AvailableSizes.FirstOrDefault(s => s.Size == size);
            if (sizeOption == null || !sizeOption.IsAvailable) return;

            SelectedSize = size;

            // Actualizar selección visual
            foreach (var s in AvailableSizes)
            {
                s.IsSelected = s.Size == size;
            }

            // Forzar actualización de la colección
            OnPropertyChanged(nameof(AvailableSizes));
        }

        [RelayCommand]
        private void ToggleSizeGuide()
        {
            IsSizeGuideOpen = !IsSizeGuideOpen;
        }

        #endregion

        #region Commands - Reviews

        [RelayCommand]
        private void ToggleWriteReview()
        {
            IsWriteReviewOpen = !IsWriteReviewOpen;

            // Reset form cuando se abre
            if (IsWriteReviewOpen)
            {
                NewReviewRating = 5;
                NewReviewTitle = string.Empty;
                NewReviewText = string.Empty;
            }
        }

        [RelayCommand]
        private void SetReviewRating(string ratingStr)
        {
            if (int.TryParse(ratingStr, out int rating) && rating >= 1 && rating <= 5)
            {
                NewReviewRating = rating;
            }
        }

        [RelayCommand]
        private async Task SubmitReviewAsync()
        {
            if (string.IsNullOrWhiteSpace(NewReviewTitle) || string.IsNullOrWhiteSpace(NewReviewText))
            {
                return;
            }

            // Crear review (en producción esto iría al backend)
            var review = new Review
            {
                ProductId = Product.Id,
                Rating = NewReviewRating,
                Title = NewReviewTitle,
                Comment = NewReviewText,
                CreatedAt = DateTime.Now,
                IsVerifiedPurchase = true,
                UserId = 1,
                User = new User { FullName = "You" }
            };

            // Añadir al principio de la lista
            Reviews.Insert(0, review);

            // Cerrar formulario
            IsWriteReviewOpen = false;

            // TODO: Enviar al backend
            await Task.CompletedTask;
        }

        #endregion

        #region Commands - Navigation

        [RelayCommand]
        private void NavigateHome()
        {
            _navigationService.NavigateTo<HomeViewModel>();
        }

        [RelayCommand]
        private void NavigateToCatalog()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        [RelayCommand]
        private void ViewRelatedProduct(Product product)
        {
            if (product == null) return;
            _navigationService.NavigateTo<ProductDetailViewModel>(product.Id);
        }

        #endregion
    }

    #region Helper Classes

    public partial class ColorOption : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _hexCode;

        [ObservableProperty]
        private bool _isSelected;
    }

    public partial class SizeOption : ObservableObject
    {
        [ObservableProperty]
        private string _size;

        [ObservableProperty]
        private bool _isAvailable = true;

        [ObservableProperty]
        private bool _isSelected;
    }

    #endregion
}