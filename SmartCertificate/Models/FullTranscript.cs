using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartCertificate.Models
{
    /// <summary>
    /// Represents a full transcript for a student, containing courses and grades.
    /// </summary>
    public class FullTranscript
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
        public Dictionary<string, double> Grades { get; set; } = new Dictionary<string, double>();
        public double GPA { get; set; }

        public override string ToString()
        {
            var lines = new List<string>
            {
                $"Transcript for {StudentName} (ID: {StudentId})",
                "Courses:"
            };
            foreach (var c in Courses)
                lines.Add($" - {c.CourseId}: {c.CourseName} ({c.Credits} credits)");
            lines.Add("Grades:");
            foreach (var kv in Grades)
                lines.Add($" - {kv.Key}: {kv.Value:F2}");
            lines.Add($"GPA: {GPA:F2}");
            return string.Join(Environment.NewLine, lines);
        }
    }
}
