using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCShoppingCart.Models.Data
{
    [Table("tblOrderDetails")]
    public class OrderDetailsDto
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDto Users { get; set; }


        [ForeignKey("OrderId")]
        public virtual OrderDto Orders { get; set; }


        [ForeignKey("ProductId")]
        public virtual ProductDto Products { get; set; }
    }
}