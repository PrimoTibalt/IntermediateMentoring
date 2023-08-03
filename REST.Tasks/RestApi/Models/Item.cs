using System.ComponentModel.DataAnnotations;

namespace RestApi.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;
    }
}
