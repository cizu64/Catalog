using System.ComponentModel.DataAnnotations;

namespace Catalog.Auth.ViewModel
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Firstname is required")]
        [MaxLength(20,ErrorMessage ="Your name should not be more than 20 characters long")]
        public string Fullname { get; set; }
        
    }
}
