using System;

namespace SmartCertificate.Models
{
    public class Student : User
    {
        public string StudentNumber { get; set; }

        public Student() { Role = "Student"; }
        public Student(string username, string email, string studentNumber) : base(username, email)
        {
            StudentNumber = studentNumber;
            Role = "Student";
        }
    }
}
