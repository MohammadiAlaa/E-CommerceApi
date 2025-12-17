using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // Pending, Cancelled, Completed

        // ربط الطلب بالمستخدم المسجل
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public Shipping Shipping { get; set; } // One-to-One
        public Payment Payment { get; set; }   // One-to-One
        public List<OrderItem> OrderItems { get; set; }
    }
}
