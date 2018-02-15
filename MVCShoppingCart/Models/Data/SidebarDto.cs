using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCShoppingCart.Models.Data
{
    [Table("tblSidebar")]
    public class SidebarDto
    {
        [Key]
        public int Id { get; set; }
        public string Body { get; set; }
    }
}