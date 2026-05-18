using System;
using System.Collections.Generic;
using System.Linq;
using SmartCertificate.Models;

namespace SmartCertificate.Services
{
    /// <summary>
    /// Transcript management service: generates transcripts, calculates GPA and displays grades.
    /// GPA calculation: simple weighted average based on course credits. If credits are not available,
    /// a simple arithmetic mean is used. GPA is rounded to 2 decimal places.
    /// </summary>
    public class TranscriptManagementService
    {
        private readonly List<StudentRecord> _students;
        private readonly List<FullTranscript> _transcripts;
        private readonly List<Course> _courses;

        public TranscriptManagementService(List<StudentRecord> students = null, List<Course> courses = null)
        {
            // Use provided data or seed sample data
            _students = students ?? new List<StudentRecord>
            {
                new StudentRecord(1, "John Doe", "john.doe@example.com", new DateTime(1998,4,12), 3.5),
                new StudentRecord(2, "Jane Smith", "jane.smith@example.com", new DateTime(1997,8,23), 3.8)
            };

            _courses = courses ?? new List<Course>
            {
                new Course(101, "Computer Science 101", 3),
                new Course(102, "Mathematics 101", 4)
            };

            // seed transcripts list
            _transcripts = new List<FullTranscript>
            {
                new FullTranscript
                {
                    StudentId = 1,
                    StudentName = "John Doe",
                    Courses = new List<Course> { _courses[0], _courses[1] },
                    Grades = new Dictionary<string,double> { { "Computer Science 101", 3.7 }, { "Mathematics 101", 3.3 } }
                }
            };

            // calculate GPA for seeded transcripts
            foreach (var t in _transcripts)
            {
                t.GPA = CalculateGPA(t.Grades, t.Courses);
            }
        }

        /// <summary>
        /// Generates a transcript for the specified student id. Throws if student not found.
        /// </summary>
        public FullTranscript GenerateTranscript(int studentId)
        {
            var student = _students.FirstOrDefault(s => s.StudentId == studentId);
            if (student == null) throw new KeyNotFoundException($"StudentId {studentId} not found.");

            // Try to find an existing transcript
            var existing = _transcripts.FirstOrDefault(t => t.StudentId == studentId);
            if (existing != null) return existing;

            // Create a blank transcript (no grades) and return
            var newT = new FullTranscript
            {
                StudentId = student.StudentId,
                StudentName = student.Name,
                Courses = new List<Course>()
            };
            newT.GPA = 0.0;
            _transcripts.Add(newT);
            return newT;
        }

        /// <summary>
        /// Overload: generate transcript by student name (first match).
        /// </summary>
        public FullTranscript GenerateTranscript(string studentName)
        {
            if (string.IsNullOrWhiteSpace(studentName)) throw new ArgumentException("Student name required.");
            var student = _students.FirstOrDefault(s => string.Equals(s.Name, studentName, StringComparison.OrdinalIgnoreCase));
            if (student == null) throw new KeyNotFoundException($"Student '{studentName}' not found.");
            return GenerateTranscript(student.StudentId);
        }

        /// <summary>
        /// Calculates GPA from grades dictionary and course list using weighted average by credits.
        /// If course credit information is missing or empty, an arithmetic mean is used.
        /// </summary>
        public double CalculateGPA(Dictionary<string, double> grades, List<Course> courses = null)
        {
            if (grades == null || grades.Count == 0) throw new ArgumentException("Grades are required to calculate GPA.");

            // Attempt weighted average by matching course names to course credits
            double totalPoints = 0.0;
            double totalCredits = 0.0;

            foreach (var kv in grades)
            {
                var courseName = kv.Key;
                var grade = kv.Value; // assume grade is on 0.0 - 4.0 scale

                // find course for credits
                var course = (courses ?? _courses).FirstOrDefault(c => string.Equals(c.CourseName, courseName, StringComparison.OrdinalIgnoreCase) || string.Equals(c.CourseName, courseName, StringComparison.OrdinalIgnoreCase));
                double credits = course?.Credits ?? 0;

                if (credits > 0)
                {
                    totalPoints += grade * credits;
                    totalCredits += credits;
                }
                else
                {
                    // if no credits, treat as 1 credit for averaging
                    totalPoints += grade * 1;
                    totalCredits += 1;
                }
            }

            if (totalCredits == 0) throw new InvalidOperationException("Total credits calculated as zero.");

            var gpa = totalPoints / totalCredits;
            return Math.Round(gpa, 2);
        }

        /// <summary>
        /// Displays grades for a student in a formatted manner.
        /// </summary>
        public string DisplayGrades(int studentId)
        {
            try
            {
                var transcript = _transcripts.FirstOrDefault(t => t.StudentId == studentId);
                if (transcript == null) return $"No transcript found for StudentId {studentId}.";

                // Optional: sort grades highest to lowest
                var sorted = transcript.Grades.OrderByDescending(g => g.Value).ToList();

                var lines = new List<string>
                {
                    $"Transcript for {transcript.StudentName} (ID: {transcript.StudentId})",
                    "Courses and Grades:"
                };
                foreach (var kv in sorted)
                {
                    lines.Add($" - {kv.Key}: {kv.Value:F2}");
                }
                lines.Add($"GPA: {transcript.GPA:F2}");

                return string.Join(Environment.NewLine, lines);
            }
            catch (Exception ex)
            {
                return $"Error displaying grades: {ex.Message}";
            }
        }
    }
}
