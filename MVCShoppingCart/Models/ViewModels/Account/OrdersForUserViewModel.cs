﻿using System;
using System.Collections.Generic;

namespace MVCShoppingCart.Models.ViewModels.Account
{
    public class OrdersForUserViewModel
    {
        public int OrderNumber { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, int> ProductsAndQty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}