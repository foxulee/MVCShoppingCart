using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCShoppingCart.Models.Data
{
    [Table("tblUsers")]
    public class UserDto
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }
}