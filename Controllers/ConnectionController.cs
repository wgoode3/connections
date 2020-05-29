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
    public class ConnectionController : Controller
    {
        private Context _context { get; set; }

        public ConnectionController (Context c)
        {
            _context = c;
        }

        [HttpGet ("connections/new")]
        public IActionResult NewConnections ()
        {
            int? userId = HttpContext.Session.GetInt32 ("userId");
            if (userId == null)
            {
                return Redirect ("/");
            }
            ViewBag.sessionId = (int) userId;
            List<User> all = _context.Users
                .Include (u => u.Followers)
                .Where (u => u.UserId != userId)
                .ToList ();
            List<User> usersFollowed = _context.Users
                .Include (u => u.UsersFollowed)
                .ThenInclude (c => c.UserFollowed)
                .FirstOrDefault (u => u.UserId == (int) userId)
                .UsersFollowed.Select (c => c.UserFollowed)
                .ToList ();
            List<User> usersToFollow = all.Except (usersFollowed).ToList ();
            return View (usersToFollow);
        }

        [HttpGet ("connect/{followId}")]
        public IActionResult Connect (int followId, [FromQuery (Name = "prev")] string prev)
        {
            int? follower = HttpContext.Session.GetInt32 ("userId");
            Console.WriteLine (prev);
            if (follower == null)
            {
                return Redirect ("/");
            }
            _context.Create ((int) follower, followId);
            return Redirect ($"/{prev}");
        }

        [HttpGet ("ignore/{followId}")]
        public IActionResult Ignore (int followId)
        {
            int? userId = HttpContext.Session.GetInt32 ("userId");
            Connection request = _context.Connections
                .Where (c => c.UserFollowedId == (int) userId)
                .FirstOrDefault (c => c.FollowerId == followId);
            _context.Connections.Remove (request);
            _context.SaveChanges ();
            return Redirect ($"/user/{userId}");
        }

    }
}