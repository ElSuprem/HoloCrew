namespace HoloCrew.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Label { get; set; } // "Casa", "Trabajo", etc.
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool IsDefault { get; set; }
    }
}