using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_CommerceApi.Models
{
    public class Shipping
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ShippingMethod { get; set; }

        public DateTime? LastUpdated { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Shipped, Delivered

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
