using System;

// Reseña que deja un usuario sobre un producto.
// Puntuación del 1 al 5, título, comentario, y si ha comprado el producto realmente.

namespace HoloCrew.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Rating { get; set; } // del 1 al 5
        public string Title { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; } // true si el usuario realmente compró el producto
        public int HelpfulCount { get; set; }        // cuánta gente marcó que le sirvió la reseña
    }
}