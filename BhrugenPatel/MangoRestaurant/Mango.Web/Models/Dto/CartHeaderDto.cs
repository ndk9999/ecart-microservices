using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.Dto
{
    public class CartHeaderDto
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string CouponCode { get; set; }

        public decimal OrderTotal { get; set; }
    }
}
