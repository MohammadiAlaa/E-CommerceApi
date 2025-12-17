namespace E_CommerceApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Item> Items { get; set; }
    }
}
