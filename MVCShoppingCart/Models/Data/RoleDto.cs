using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCShoppingCart.Models.Data
{
    [Table("tblRoles")]
    public class RoleDto
    {
        [Key]
        public int Id { get; set; }


        public string Name { get; set; }
    }
}