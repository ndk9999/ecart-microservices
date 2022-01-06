﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponApi.Models
{
    public class Coupon
    {
        [Key, ]
        public int Id { get; set; }

        public string CouponCode { get; set; }

        public decimal DiscountAmount { get; set; }
    }
}
