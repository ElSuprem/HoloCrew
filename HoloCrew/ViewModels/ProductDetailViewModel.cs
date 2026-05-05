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

// ViewModel de la página de detalle de un producto.
// Muestra la información del producto, imágenes, selección de color/talla, cantidad,
// añadir al carrito, comprar ahora, wishlist, reseñas y productos relacionados.
// Se conecta con ProductService, CartService, WishlistService y NavigationService.

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

        // URL de la imagen actualmente mostrada en grande (bindea contra el converter)
        [ObservableProperty]
        private string _selectedImage = string.Empty;

        [ObservableProperty]
        private int _selectedImageIndex = 0;

        // Lista de miniaturas reales del producto (cada una sabe si está seleccionada)
        [ObservableProperty]
        private ObservableCollection<ProductImageItem> _images = new();

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private ObservableCollection<Review> _reviews = new();

        [ObservableProperty]
        private ObservableCollection<Product> _relatedProducts = new();

        [ObservableProperty]
        private bool _isInWishlist;

        // selección de color
        [ObservableProperty]
        private string _selectedColor = "Black";

        [ObservableProperty]
        private ObservableCollection<ColorOption> _availableColors = new();

        // selección de talla
        [ObservableProperty]
        private string _selectedSize = "M";

        [ObservableProperty]
        private ObservableCollection<SizeOption> _availableSizes = new();

        // guía de tallas (popup)
        [ObservableProperty]
        private bool _isSizeGuideOpen = false;

        // escribir reseña (popup)
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
                    // Imágenes reales del producto. Sin placeholders rotos: si solo hay 1
                    // se muestra 1, si hay 5 se muestran 5. Si por algún motivo ImageUrls
                    // viene vacío, usamos MainImageUrl como fallback para no dejar la
                    // galería en blanco.
                    var imageList = Product.ImageUrls?
                                        .Where(u => !string.IsNullOrWhiteSpace(u))
                                        .Distinct()
                                        .ToList()
                                    ?? new List<string>();

                    if (imageList.Count == 0 && !string.IsNullOrWhiteSpace(Product.MainImageUrl))
                    {
                        imageList.Add(Product.MainImageUrl);
                    }

                    Images.Clear();
                    for (int i = 0; i < imageList.Count; i++)
                    {
                        Images.Add(new ProductImageItem
                        {
                            Url = imageList[i],
                            IsSelected = i == 0
                        });
                    }

                    SelectedImageIndex = 0;
                    SelectedImage = imageList.FirstOrDefault()
                                    ?? Product.MainImageUrl
                                    ?? string.Empty;

                    SetupAvailableColors();
                    SetupAvailableSizes();

                    var related = await _productService.GetRelatedProductsAsync(productId);
                    RelatedProducts = new ObservableCollection<Product>(related);

                    IsInWishlist = await _wishlistService.IsInWishlistAsync(productId);

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
                bool isAvailable = productSizes.Contains(size);

                AvailableSizes.Add(new SizeOption
                {
                    Size = size,
                    IsAvailable = isAvailable,
                    IsSelected = size == SelectedSize && isAvailable
                });
            }

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
            ApplyImageSelection();
        }

        [RelayCommand]
        private void PreviousImage()
        {
            if (Images.Count == 0) return;

            SelectedImageIndex = SelectedImageIndex - 1;
            if (SelectedImageIndex < 0) SelectedImageIndex = Images.Count - 1;
            ApplyImageSelection();
        }

        // Acepta tanto el item completo (cuando viene del ItemsControl de miniaturas)
        // como un índice (int o string) por compatibilidad con código previo.
        [RelayCommand]
        private void SelectImage(object parameter)
        {
            int index = -1;

            if (parameter is ProductImageItem item)
            {
                index = Images.IndexOf(item);
            }
            else if (parameter is int i)
            {
                index = i;
            }
            else if (parameter is string s && int.TryParse(s, out int parsed))
            {
                index = parsed;
            }

            if (index >= 0 && index < Images.Count)
            {
                SelectedImageIndex = index;
                ApplyImageSelection();
            }
        }

        // Sincroniza el flag IsSelected de cada miniatura y actualiza la imagen grande.
        private void ApplyImageSelection()
        {
            for (int i = 0; i < Images.Count; i++)
            {
                Images[i].IsSelected = i == SelectedImageIndex;
            }

            if (SelectedImageIndex >= 0 && SelectedImageIndex < Images.Count)
            {
                SelectedImage = Images[SelectedImageIndex].Url;
            }
        }

        #endregion

        #region Commands - Color Selection

        [RelayCommand]
        private void SelectColor(string colorName)
        {
            if (string.IsNullOrEmpty(colorName)) return;

            SelectedColor = colorName;

            foreach (var color in AvailableColors)
            {
                color.IsSelected = color.Name == colorName;
            }

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

            foreach (var s in AvailableSizes)
            {
                s.IsSelected = s.Size == size;
            }

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

            Reviews.Insert(0, review);
            IsWriteReviewOpen = false;

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

    // miniatura del producto (URL + flag de seleccionada para el highlight de la galería)
    public partial class ProductImageItem : ObservableObject
    {
        [ObservableProperty]
        private string _url = string.Empty;

        [ObservableProperty]
        private bool _isSelected;
    }

    // opción de color para la interfaz (nombre, código hex, si está seleccionado)
    public partial class ColorOption : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _hexCode;

        [ObservableProperty]
        private bool _isSelected;
    }

    // opción de talla para la interfaz (talla, disponible, si está seleccionada)
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
