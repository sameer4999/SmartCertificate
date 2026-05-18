using System;
using SmartCertificate.Models;

namespace SmartCertificate.Services
{
    public class TranscriptService
    {
        public string Summarize(Transcript t) => t.GradesSummary ?? "No grades";
    }
}
