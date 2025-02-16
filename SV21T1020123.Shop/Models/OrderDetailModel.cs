﻿using SV21T1020123.DomainModels;

namespace SV21T1020123.Shop.Models
{
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
