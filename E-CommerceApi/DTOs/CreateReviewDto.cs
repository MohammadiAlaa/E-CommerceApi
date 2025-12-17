using System.ComponentModel.DataAnnotations;

namespace E_CommerceApi.DTOs
{
    public class CreateReviewDto
    {
        public int ItemId { get; set; }
        public string Comment { get; set; }

        [Range(1, 5)] 
        public int Rating { get; set; }

    }
}
