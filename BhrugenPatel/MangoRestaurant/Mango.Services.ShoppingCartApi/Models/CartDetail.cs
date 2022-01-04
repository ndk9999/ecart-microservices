using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartApi.Models
{
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public int Count { get; set; }


        [ForeignKey("CartId")]
        public CartHeader CartHeader { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
