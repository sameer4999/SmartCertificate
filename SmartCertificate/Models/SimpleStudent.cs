using System;

namespace SmartCertificate.Models
{
    /// <summary>
    /// Simple student model used for search and sort operations.
    /// </summary>
    public class SimpleStudent
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public double GPA { get; set; }

        public SimpleStudent() { }

        public SimpleStudent(int id, string name, double gpa)
        {
            StudentId = id;
            Name = name;
            GPA = gpa;
        }

        public override string ToString()
        {
            return $"[{StudentId}] {Name} | GPA: {GPA:F2}";
        }
    }
}
