using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderApi.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Count { get; set; }

        public decimal Price { get; set; }


        [ForeignKey("OrderId")]
        public OrderHeader OrderHeader { get; set; }
    }
}
