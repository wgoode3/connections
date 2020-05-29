using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace connections.Models
{
    public class Context : DbContext
    {
        public Context (DbContextOptions options) : base (options) { }

        // this is the variable we will use to connect to the MySQL table, Lizards
        public DbSet<User> Users { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Post> Posts { get; set; }

        public void Create (User u)
        {
            Users.Add (u);
            SaveChanges ();
        }

        public void Create (Post p)
        {
            Posts.Add (p);
            SaveChanges ();
        }

        public void Create (int followerId, int userFollowedId)
        {
            Connection c = new Connection ()
            {
                FollowerId = followerId,
                UserFollowedId = userFollowedId
            };
            System.Console.WriteLine (c.FollowerId);
            System.Console.WriteLine (c.UserFollowedId);
            Connections.Add (c);
            SaveChanges ();
        }

        public User FindUserByEmail (string email)
        {
            return Users.FirstOrDefault (u => u.Email == email);
        }

    }
}