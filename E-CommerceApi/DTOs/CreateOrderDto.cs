namespace E_CommerceApi.DTOs
{
    public class CreateOrderDto
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
