using System;
using System.Collections.Generic;
using System.Linq;
using SmartCertificate.Models;

namespace SmartCertificate.Services
{
    /// <summary>
    /// Service responsible for managing courses and enrollments.
    /// Designed for Admin operations in the console app.
    /// </summary>
    public class CourseService
    {
        private readonly List<Course> _courses = new List<Course>();

        public CourseService()
        {
            // Seed sample courses for testing
            var c1 = new Course(101, "Computer Science 101", 3);
            c1.Modules.AddRange(new[] { "Intro to CS", "Programming Basics", "Data Structures" });
            c1.EnrolledStudents.AddRange(new[] { 1, 2 });

            var c2 = new Course(102, "Mathematics 101", 4);
            c2.Modules.AddRange(new[] { "Calculus I", "Linear Algebra" });
            c2.EnrolledStudents.Add(2);

            _courses.Add(c1);
            _courses.Add(c2);
        }

        /// <summary>
        /// Adds a new course after validation.
        /// </summary>
        public void AddCourse(Course course)
        {
            try
            {
                if (course == null) throw new ArgumentNullException(nameof(course));
                if (_courses.Any(c => c.CourseId == course.CourseId))
                    throw new InvalidOperationException($"CourseId {course.CourseId} already exists.");
                if (string.IsNullOrWhiteSpace(course.CourseName))
                    throw new ArgumentException("CourseName is required.");
                if (course.Credits <= 0) throw new ArgumentException("Credits must be positive.");

                _courses.Add(course);
                Console.WriteLine($"Course {course.CourseName} added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding course: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds a module to an existing course ensuring no duplicate modules.
        /// </summary>
        public void AddModuleToCourse(int courseId, string moduleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(moduleName)) throw new ArgumentException("Module name required.");
                var course = _courses.FirstOrDefault(c => c.CourseId == courseId);
                if (course == null) throw new KeyNotFoundException($"CourseId {courseId} not found.");
                if (course.Modules.Any(m => string.Equals(m, moduleName, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Module already exists in course.");
                course.Modules.Add(moduleName.Trim());
                Console.WriteLine($"Module '{moduleName}' added to course {course.CourseName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding module: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Assigns a student (by id) to a course after checking for duplicates.
        /// </summary>
        public void AssignStudentToCourse(int courseId, int studentId)
        {
            try
            {
                if (studentId <= 0) throw new ArgumentException("StudentId must be positive.");
                var course = _courses.FirstOrDefault(c => c.CourseId == courseId);
                if (course == null) throw new KeyNotFoundException($"CourseId {courseId} not found.");
                if (course.EnrolledStudents.Contains(studentId)) throw new InvalidOperationException("Student already enrolled in course.");
                course.EnrolledStudents.Add(studentId);
                Console.WriteLine($"Student {studentId} assigned to course {course.CourseName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning student: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns a list of enrolled student ids for a course.
        /// </summary>
        public List<int> GetEnrolledStudents(int courseId)
        {
            var course = _courses.FirstOrDefault(c => c.CourseId == courseId);
            if (course == null) throw new KeyNotFoundException($"CourseId {courseId} not found.");
            return course.EnrolledStudents.ToList();
        }

        /// <summary>
        /// Lists all managed courses.
        /// </summary>
        public List<Course> GetAllCourses() => _courses.ToList();

        /// <summary>
        /// Optional search by course name or id.
        /// </summary>
        public List<Course> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<Course>();
            query = query.Trim();
            if (int.TryParse(query, out int id)) return _courses.Where(c => c.CourseId == id).ToList();
            return _courses.Where(c => c.CourseName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }
    }
}
