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
    public class HomeController : Controller
    {
        private Context _context { get; set; }
        private PasswordHasher<User> _registerHasher = new PasswordHasher<User> ();
        private PasswordHasher<LoginUser> _loginHasher = new PasswordHasher<LoginUser> ();

        public HomeController (Context c)
        {
            _context = c;
        }

        [HttpGet ("")]
        public IActionResult Index ()
        {
            return View ();
        }

        [HttpPost ("register")]
        public IActionResult Register (User newUser)
        {
            if (ModelState.IsValid)
            {
                newUser.Password = _registerHasher.HashPassword (newUser, newUser.Password);
                _context.Create (newUser);
                HttpContext.Session.SetInt32 ("userId", newUser.UserId);
                return Redirect ($"/user/{newUser.UserId}");
            }
            else
            {
                return View ("Index");
            }
        }

        [HttpPost ("login")]
        public IActionResult Login (LoginUser newUser)
        {
            if (ModelState.IsValid)
            {
                User u = _context.FindUserByEmail (newUser.LoginEmail);
                if (u == null)
                {
                    ModelState.AddModelError ("LoginEmail", "Invalid Email/Password");
                }
                else
                {
                    var result = _loginHasher.VerifyHashedPassword (newUser, u.Password, newUser.LoginPassword);
                    if (result == 0)
                    {
                        ModelState.AddModelError ("LoginEmail", "Invalid Email/Password");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32 ("userId", u.UserId);
                        return Redirect ($"/user/{u.UserId}");
                    }
                }
                return View ("Index");

            }
            else
            {
                return View ("Index");
            }
        }

        [HttpGet ("logout")]
        public IActionResult Logout ()
        {
            HttpContext.Session.Clear ();
            return Redirect ("/");
        }

        [HttpGet ("user/{userId}")]
        public IActionResult Profile (int userId)
        {
            int? sessionId = HttpContext.Session.GetInt32 ("userId");
            if (sessionId == null)
            {
                return Redirect ("/");
            }
            ViewBag.sessionId = (int) sessionId;
            User user = _context.Users
                .Include (u => u.Posts)
                .Include (u => u.Followers)
                .ThenInclude (c => c.Follower)
                .Include (u => u.UsersFollowed)
                .ThenInclude (c => c.UserFollowed)
                .FirstOrDefault (u => u.UserId == (int) userId);
            List<User> myFollows = user.UsersFollowed.Select (c => c.UserFollowed).ToList ();
            List<User> myFollowers = user.Followers.Select (c => c.Follower).ToList ();
            List<User> myConnections = myFollows.Intersect (myFollowers).ToList ();
            List<User> connectionRequests = myFollowers.Except (myConnections).ToList ();
            ViewBag.MyConnections = myConnections;
            ViewBag.ConnectionRequests = connectionRequests;
            return View (user);
        }

    }
}