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

        [HttpGet ("logout")]
        public IActionResult Logout ()
        {
            HttpContext.Session.Clear ();
            return Redirect ("/");
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

        [HttpGet ("user/{userId}")]
        public IActionResult UserProfile (int userId)
        {
            int? sessionId = HttpContext.Session.GetInt32 ("userId");
            if (sessionId == null)
            {
                return Redirect ("/");
            }
            ViewBag.sessionId = (int) sessionId;
            User user = _context.Users
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
                if(!ModelState.IsValid)
                {
                    return View("UserEdit", u);
                }
                if (userId == (int) sessionUserId)
                {
                    User oldUser = _context.Users.FirstOrDefault (user => user.UserId == userId);
                    oldUser.Name = u.Name;
                    oldUser.Email = u.Email;
                    oldUser.Location = u.Location;
                    oldUser.Description = u.Description;
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
                                return View("UserEdit", u);
                            }
                        }
                    }
                    _context.SaveChanges ();
                }
            }
            return Redirect ($"/user/{userId}");
        }

    }
}