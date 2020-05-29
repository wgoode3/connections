using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using connections.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace connections.Controllers
{
    public class UserController : Controller
    {
        private Context _context { get; set; }

        public UserController (Context c)
        {
            _context = c;
        }

        [HttpGet ("user/{userId}/edit")]
        public IActionResult UserEdit (int userId)
        {
            int? sessionId = HttpContext.Session.GetInt32 ("userId");
            if (sessionId == null)
            {
                return Redirect ("/");
            }
            ViewBag.sessionId = (int) sessionId;
            User user = _context.Users
                .FirstOrDefault (u => u.UserId == userId);
            UpdateUser update = new UpdateUser ()
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Location = user.Location,
                Description = user.Description,
                Image = user.Image
            };
            return View (update);
        }

        [HttpPost ("user/{userId}/update")]
        public async Task<IActionResult> UserPost (int userId, UpdateUser u)
        {
            // get userid from session
            int? sessionUserId = HttpContext.Session.GetInt32 ("userId");
            if (sessionUserId == null)
            {
                Redirect ("/");
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View ("UserEdit", u);
                }
                if (userId == (int) sessionUserId)
                {
                    User oldUser = _context.Users.FirstOrDefault (user => user.UserId == userId);
                    oldUser.Name = u.Name;
                    oldUser.Email = u.Email;
                    oldUser.Location = u.Location;
                    oldUser.Description = u.Description;
                    oldUser.UpdatedAt = DateTime.Now;
                    if (u.Image != null)
                    {
                        using (var ms = new MemoryStream ())
                        {
                            await u.Image.CopyToAsync (ms);
                            // 1 MB max file upload size 
                            if (ms.Length < 1048576)
                            {
                                oldUser.Avatar = ms.ToArray ();
                            }
                            else
                            {
                                ModelState.AddModelError ("Image", "User Avatar must be 1 MB or less!");
                                return View ("UserEdit", u);
                            }
                        }
                    }
                    _context.SaveChanges ();
                }
            }
            return Redirect ($"/user/{userId}");
        }

        [HttpPost ("user/{userId}/delete")]
        public IActionResult Delete (int userId)
        {
            int? sessionUserId = HttpContext.Session.GetInt32 ("userId");
            if (sessionUserId == null)
            {
                Redirect ("/");
            }
            if (userId == (int) sessionUserId)
            {
                User toDelete = _context.Users.FirstOrDefault (u => u.UserId == userId);
                _context.Users.Remove (toDelete);
                _context.SaveChanges ();
            }
            return Redirect ("/");
        }

        [HttpPost ("user/{userId}/change_password")]
        public IActionResult ChangePassword (int userId)
        {
            // TODO - add in this functionality
            return Redirect ($"/user/{userId}");
        }

    }
}