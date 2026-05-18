using System;
using System.Collections.Generic;

namespace SmartCertificate.Models
{
    /// <summary>
    /// Represents an academic course with modules and enrolled students.
    /// </summary>
    public class Course
    {
        /// <summary>Unique course identifier</summary>
        public int CourseId { get; set; }

        /// <summary>Course display name</summary>
        public string CourseName { get; set; }

        /// <summary>Number of credits the course carries</summary>
        public int Credits { get; set; }

        /// <summary>List of module names that form the course</summary>
        public List<string> Modules { get; set; } = new List<string>();

        /// <summary>List of enrolled student ids (can be adapted to List&lt;StudentRecord&gt;)</summary>
        public List<int> EnrolledStudents { get; set; } = new List<int>();

        public Course() { }

        public Course(int courseId, string courseName, int credits)
        {
            CourseId = courseId;
            CourseName = courseName;
            Credits = credits;
        }

        public override string ToString()
        {
            return $"[{CourseId}] {CourseName} | Credits: {Credits} | Modules: {Modules.Count} | Enrolled: {EnrolledStudents.Count}";
        }
    }
}
