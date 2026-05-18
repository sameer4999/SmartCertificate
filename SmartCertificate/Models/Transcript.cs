using System;
using System.Collections.Generic;

namespace SmartCertificate.Models
{
    public class Transcript
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public List<int> CourseIds { get; set; } = new List<int>();
        public string GradesSummary { get; set; }
    }
}
