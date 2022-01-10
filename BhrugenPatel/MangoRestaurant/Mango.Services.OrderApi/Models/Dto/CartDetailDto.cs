﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderApi.Models.Dto
{
    public class CartDetailDto
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public int Count { get; set; }

        public ProductDto Product { get; set; }
    }
}
