using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace connections.Models
{
    public class UpdateUser
    {
        public int UserId { get; set; }

        [Required (ErrorMessage = "Name is required!")]
        public string Name { get; set; }

        public string Location { get; set; }

        public IFormFile Image { get; set; }
        public byte[] Avatar { get; set; }

        public string Description { get; set; }

        [Required (ErrorMessage = "Email is required!")]
        [EmailAddress (ErrorMessage = "Please enter a valid Email Address!")]
        public string Email { get; set; }
    }
}