using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using connections.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connections.Controllers
{
    public class PostController : Controller
    {
        private Context _context { get; set; }

        public PostController (Context c)
        {
            _context = c;
        }

        [HttpPost ("user/{userId}/post")]
        public async Task<IActionResult> CreatePost (int userId, Post p)
        {
            int? sessionUserId = HttpContext.Session.GetInt32 ("userId");
            if (sessionUserId == null || userId == (int) sessionUserId)
            {
                Redirect ("/");
            }
            Post newPost = new Post ()
            {
                Title = p.Title,
                Content = p.Content,
                UserId = p.UserId
            };
            if (p.FormImage != null)
            {
                using (var ms = new MemoryStream ())
                {
                    await p.FormImage.CopyToAsync (ms);
                    // 1 MB max file upload size 
                    if (ms.Length < 1048576)
                    {
                        newPost.Image = ms.ToArray ();
                    }
                    else
                    {
                        return Json (new { msg = "not ok 2", error = "Post Image must be 1 MB or less!" });
                    }
                }
            }
            _context.Create (newPost);
            // return View("_Post", newPost);
            return Json (new { msg = "ok", post = newPost });
        }

    }
}