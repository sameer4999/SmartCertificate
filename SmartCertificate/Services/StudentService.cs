using System;
using System.Collections.Generic;
using SmartCertificate.Models;

namespace SmartCertificate.Services
{
    public class StudentService
    {
        // placeholder for student-related operations
        public void Enroll(Student s, Course c) { /* ... */ }
        public Transcript GenerateTranscript(Student s) => new Transcript { StudentId = s.Id };
    }
}
