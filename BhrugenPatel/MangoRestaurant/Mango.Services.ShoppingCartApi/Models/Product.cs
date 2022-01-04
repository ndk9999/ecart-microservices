using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartApi.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, 1000)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string ImageUrl { get; set; }
    }
}
