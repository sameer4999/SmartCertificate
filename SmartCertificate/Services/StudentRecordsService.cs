using System;
using System.Collections.Generic;
using System.Linq;
using SmartCertificate.Models;

namespace SmartCertificate.Services
{
    /// <summary>
    /// Service for managing student records in-memory. Suitable for Admin operations.
    /// </summary>
    public class StudentRecordsService
    {
        private readonly List<StudentRecord> _students = new List<StudentRecord>();

        public StudentRecordsService()
        {
            // Seed with sample data for testing
            _students.Add(new StudentRecord(1, "John Doe", "john.doe@example.com", new DateTime(1998, 4, 12), 3.5));
            _students.Add(new StudentRecord(2, "Jane Smith", "jane.smith@example.com", new DateTime(1997, 8, 23), 3.8));
        }

        /// <summary>
        /// Adds a new student after validation. Throws exceptions for invalid input.
        /// </summary>
        public void AddStudent(StudentRecord student)
        {
            try
            {
                ValidateStudent(student);
                if (_students.Any(s => s.StudentId == student.StudentId))
                    throw new InvalidOperationException($"StudentId {student.StudentId} already exists.");
                _students.Add(student);
                Console.WriteLine($"Student {student.Name} added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding student: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing student record.
        /// </summary>
        public void UpdateStudent(int studentId, StudentRecord updatedStudent)
        {
            try
            {
                ValidateStudent(updatedStudent);
                var existing = _students.FirstOrDefault(s => s.StudentId == studentId);
                if (existing == null) throw new KeyNotFoundException($"StudentId {studentId} not found.");

                // Prevent StudentId change to a duplicate
                if (updatedStudent.StudentId != studentId && _students.Any(s => s.StudentId == updatedStudent.StudentId))
                    throw new InvalidOperationException($"Target StudentId {updatedStudent.StudentId} already exists.");

                existing.Name = updatedStudent.Name;
                existing.Email = updatedStudent.Email;
                existing.DateOfBirth = updatedStudent.DateOfBirth;
                existing.GPA = updatedStudent.GPA;

                Console.WriteLine($"Student {studentId} updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating student: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a student record by id.
        /// </summary>
        public void DeleteStudent(int studentId)
        {
            try
            {
                var existing = _students.FirstOrDefault(s => s.StudentId == studentId);
                if (existing == null) throw new KeyNotFoundException($"StudentId {studentId} not found.");
                _students.Remove(existing);
                Console.WriteLine($"Student {studentId} deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting student: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns all students as a read-only list.
        /// </summary>
        public List<StudentRecord> GetAllStudents()
        {
            return _students.ToList();
        }

        /// <summary>
        /// Optional helper: search by name (case-insensitive) or id.
        /// </summary>
        public List<StudentRecord> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<StudentRecord>();
            query = query.Trim();
            if (int.TryParse(query, out int id))
            {
                var r = _students.Where(s => s.StudentId == id).ToList();
                return r;
            }
            return _students.Where(s => s.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        /// <summary>
        /// Validates a student record (internal helper).
        /// </summary>
        private void ValidateStudent(StudentRecord s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (s.StudentId <= 0) throw new ArgumentException("StudentId must be a positive integer.");
            if (string.IsNullOrWhiteSpace(s.Name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(s.Email)) throw new ArgumentException("Email is required.");
            if (s.GPA < 0.0 || s.GPA > 4.0) throw new ArgumentException("GPA must be between 0.0 and 4.0.");
        }
    }
}
