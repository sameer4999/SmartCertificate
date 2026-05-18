using System;

namespace SmartCertificate.Models
{
    public class Employer : User
    {
        public string Company { get; set; }

        public Employer() { Role = "Employer"; }
        public Employer(string username, string email, string company) : base(username, email)
        {
            Company = company;
            Role = "Employer";
        }
    }
}
