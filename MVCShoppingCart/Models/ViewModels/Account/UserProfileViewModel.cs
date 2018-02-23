using MVCShoppingCart.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace MVCShoppingCart.Models.ViewModels.Account
{
    public class UserProfileViewModel
    {
        public UserProfileViewModel()
        {

        }

        public UserProfileViewModel(UserDto userDto)
        {
            Id = userDto.Id;
            FirstName = userDto.FirstName;
            LastName = userDto.LastName;
            EmailAddress = userDto.EmailAddress;
            Username = userDto.Username;
            Password = userDto.Password;
        }

        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }


        [Required]
        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}