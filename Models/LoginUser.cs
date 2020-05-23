using System.ComponentModel.DataAnnotations;

namespace connections.Models
{
    public class LoginUser
    {
        [Required (ErrorMessage = "Email is required!")]
        [EmailAddress (ErrorMessage = "Please enter a valid Email Address!")]
        public string LoginEmail { get; set; }

        [DataType (DataType.Password)]
        [Required (ErrorMessage = "Password is required!")]
        [MinLength (8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string LoginPassword { get; set; }

    }
}