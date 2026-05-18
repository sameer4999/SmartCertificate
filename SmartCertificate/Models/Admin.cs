using System;

namespace SmartCertificate.Models
{
    public class Admin : User
    {
        public string Department { get; set; }

        public Admin() { Role = "Admin"; }
        public Admin(string username, string email, string department) : base(username, email)
        {
            Department = department;
            Role = "Admin";
        }
    }
}
