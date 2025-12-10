using CommunityToolkit.Mvvm.ComponentModel;

namespace HoloCrew.Models
{
    /// <summary>
    /// Modelo de item en el carrito
    /// Compatible con ambas versiones (vieja y nueva)
    /// </summary>
    public partial class CartItem : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private int _productId;

        [ObservableProperty]
        private string _productName;

        [ObservableProperty]
        private decimal _price;

        [ObservableProperty]
        private int _quantity;

        [ObservableProperty]
        private string _size;

        [ObservableProperty]
        private string _color;

        [ObservableProperty]
        private string _imageUrl;

        // ⭐ Propiedades adicionales para compatibilidad con código antiguo
        [ObservableProperty]
        private Product _product;

        [ObservableProperty]
        private string _selectedVariant;

        /// <summary>
        /// UnitPrice - Alias de Price para compatibilidad
        /// </summary>
        public decimal UnitPrice
        {
            get => Price;
            set => Price = value;
        }

        /// <summary>
        /// Subtotal del item (Price * Quantity)
        /// </summary>
        public decimal Subtotal => Price * Quantity;
    }
}