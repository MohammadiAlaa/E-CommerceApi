namespace E_CommerceApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ItemId { get; set; }
        public Item Item { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
