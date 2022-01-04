using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartApi.Models.Dto
{
    public class CartHeaderDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string CouponCode { get; set; }
    }
}
