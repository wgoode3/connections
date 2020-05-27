using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace connections.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required (ErrorMessage = "Name is required!")]
        public string Name { get; set; }

        public string Location { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }
        public byte[] Avatar { get; set; }

        public string Description { get; set; }

        [Required (ErrorMessage = "Email is required!")]
        [EmailAddress (ErrorMessage = "Please enter a valid Email Address!")]
        public string Email { get; set; }

        [DataType (DataType.Password)]
        [Required (ErrorMessage = "Password is required!")]
        [MinLength (8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // We us the NotMapped Annotation so that this variable doesn't end up in our database.
        [NotMapped]
        [Compare ("Password", ErrorMessage = "Confirm Password must match Password!")]
        [DataType (DataType.Password)]
        public string Confirm { get; set; }

        [InverseProperty ("UserFollowed")]
        public List<Connection> Followers { get; set; }

        [InverseProperty ("Follower")]
        public List<Connection> UsersFollowed { get; set; }
    }
}