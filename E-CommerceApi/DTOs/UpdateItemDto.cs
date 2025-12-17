using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.DTOs
{
    public class UpdateItemDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Range(0.01, 100000.00, ErrorMessage = "Price must be between 0.01 and 100000")]
        public decimal? Price { get; set; }

        [Range(0, 100000, ErrorMessage = "Quantity must be a positive number")]
        public int? Quantity { get; set; }

        public int? CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
