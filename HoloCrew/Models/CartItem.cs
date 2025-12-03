namespace HoloCrew.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string SelectedVariant { get; set; }
        public decimal UnitPrice { get; set; }

        // Propiedad calculada
        public decimal Subtotal => UnitPrice * Quantity;
    }
}