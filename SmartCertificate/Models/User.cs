using System;

namespace SmartCertificate.Models
{
    // Base user class - derived roles live in separate files
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }

        public User() { }

        public User(string username, string email)
        {
            Username = username;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }

        public virtual string GetRole()
        {
            return Role ?? "User";
        }
    }
}
