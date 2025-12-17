namespace E_CommerceApi.DTOs
{
    public class CreateItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; } 
    }
}
