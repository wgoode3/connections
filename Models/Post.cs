using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace connections.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required (ErrorMessage="Your Post must have a Title!")]
        public string Title { get; set; }
        [NotMapped]
        public IFormFile FormImage { get; set; }
        public byte[] Image { get; set; }
        [Required (ErrorMessage="Your Post must have Content!")]
        public string Content { get; set; }
        public int UserId { get; set; }
        public User OriginalPoster { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}