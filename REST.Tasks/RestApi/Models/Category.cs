namespace RestApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public IList<Item> Items { get; set; } = new List<Item>();
    }
}
