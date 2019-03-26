using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AngloRota.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "User Name is required")]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
