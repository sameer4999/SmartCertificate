using System;
using System.Security.Cryptography;
using System.Text;
using SmartCertificate.Models;
using SmartCertificate.Repositories;

namespace SmartCertificate.Services
{
    public class AuthService
    {
        private readonly UserRepository _repo;
        private User _currentUser;

        public AuthService(UserRepository repo)
        {
            _repo = repo;
        }

        public User CurrentUser => _currentUser;

        public string Register(string username, string email, string password, string role = "Student")
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username and password required");
            var exists = _repo.GetByUsername(username);
            if (exists != null) return "UserExists";
            User u = new User(username, email) { Role = role, PasswordHash = HashPassword(password), CreatedAt = DateTime.UtcNow };
            // assign derived type when role is specific
            if (role == "Student") u = new Student(username, email, null) { PasswordHash = HashPassword(password), CreatedAt = DateTime.UtcNow };
            if (role == "Admin") u = new Admin(username, email, null) { PasswordHash = HashPassword(password), CreatedAt = DateTime.UtcNow };
            if (role == "Employer") u = new Employer(username, email, null) { PasswordHash = HashPassword(password), CreatedAt = DateTime.UtcNow };
            _repo.Add(u);
            return "Registered";
        }

        public string Login(string username, string password)
        {
            var user = _repo.GetByUsername(username);
            if (user == null) return "NotFound";
            if (VerifyHash(password, user.PasswordHash))
            {
                _currentUser = user;
                return "Success";
            }
            return "InvalidCredentials";
        }

        public void Logout() => _currentUser = null;

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyHash(string password, string hash)
        {
            var computed = HashPassword(password);
            return computed == hash;
        }

        // no inner helper user type required
    }
}
