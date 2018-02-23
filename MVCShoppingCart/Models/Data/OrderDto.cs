using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCShoppingCart.Models.Data
{
    [Table("tblOrders")]
    public class OrderDto
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDto Users { get; set; }

    }
}