namespace Mango.Services.ShoppingCartApi.Models.Dto
{
    public class CouponDto
    {
        public int Id { get; set; }

        public string CouponCode { get; set; }

        public decimal DiscountAmount { get; set; }
    }
}
