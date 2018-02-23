﻿using System;
using System.Collections.Generic;

namespace MVCShoppingCart.Areas.Admin.Models.ViewModels.Shop
{
    public class OrdersForAdminViewModel
    {
        public int OrderNumber { get; set; }
        public string Username { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, int> ProductsAndQty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}