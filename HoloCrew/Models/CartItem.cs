using CommunityToolkit.Mvvm.ComponentModel;

namespace HoloCrew.Models
{
    // Un producto dentro del carrito de compras.
    // Guarda el nombre, precio, cantidad, talla, color, etc.
    // Subtotal = precio * cantidad.
    // Las propiedades con [ObservableProperty] se pueden usar en la interfaz y se actualizan solas.
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

        // compatibilidad con código antiguo
        [ObservableProperty]
        private Product _product;

        [ObservableProperty]
        private string _selectedVariant;

        // UnitPrice es lo mismo que Price, solo que con otro nombre para compatibilidad
        public decimal UnitPrice
        {
            get => Price;
            set => Price = value;
        }

        // total del item sin sumar otros (precio x cantidad)
        public decimal Subtotal => Price * Quantity;
    }
}