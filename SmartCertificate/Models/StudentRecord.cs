using System;

namespace SmartCertificate.Models
{
    /// <summary>
    /// Student record model used for student management (separate from authentication User/Student models).
    /// </summary>
    public class StudentRecord
    {
        /// <summary>Unique student identifier</summary>
        public int StudentId { get; set; }

        /// <summary>Full name of the student</summary>
        public string Name { get; set; }

        /// <summary>Contact email</summary>
        public string Email { get; set; }

        /// <summary>Date of birth</summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>Grade point average (0.0 - 4.0)</summary>
        public double GPA { get; set; }

        public StudentRecord() { }

        public StudentRecord(int id, string name, string email, DateTime dob, double gpa)
        {
            StudentId = id;
            Name = name;
            Email = email;
            DateOfBirth = dob;
            GPA = gpa;
        }

        public override string ToString()
        {
            return $"[{StudentId}] {Name} | Email: {Email} | DOB: {DateOfBirth:yyyy-MM-dd} | GPA: {GPA:F2}";
        }
    }
}
