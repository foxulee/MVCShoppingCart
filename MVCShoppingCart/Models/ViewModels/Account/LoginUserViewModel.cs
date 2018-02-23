using System.ComponentModel.DataAnnotations;

namespace MVCShoppingCart.Models.ViewModels.Account
{
    public class LoginUserViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}