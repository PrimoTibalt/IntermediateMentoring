using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
