﻿using Mango.MessageBus;

namespace Mango.Services.PaymentApi.Models
{
    public class PaymentRequestMessage : BaseMessage
    {
        public int OrderId { get; set; }

        public string Name { get; set; }

        public string CardNumber { get; set; }

        public string CVV { get; set; }

        public string ExpiryMonthYear { get; set; }

        public decimal OrderTotal { get; set; }
    }
}
