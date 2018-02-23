using MVCShoppingCart.Models.Data;
using System;

namespace MVCShoppingCart.Models.ViewModels.Shop
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {

        }

        public OrderViewModel(OrderDto orderDto)
        {
            OrderId = orderDto.OrderId;
            UserId = orderDto.UserId;
            CreatedAt = orderDto.CreatedAt;
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}