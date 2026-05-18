using System;
using System.Collections.Generic;
using System.Linq;
using SmartCertificate.Models;

namespace SmartCertificate.Services
{
    /// <summary>
    /// Service responsible for verifying certificates using in-memory list as data store.
    /// Demonstrates method overloading, input validation, exception handling, and LINQ.
    /// </summary>
    public class CertificateVerificationService
    {
        // In-memory "database" of verifiable certificates
        private readonly List<VerifiableCertificate> _certificates;

        /// <summary>
        /// Constructor - seeds sample data for quick testing.
        /// </summary>
        public CertificateVerificationService()
        {
            _certificates = new List<VerifiableCertificate>
            {
                new VerifiableCertificate { CertificateId = 1001, StudentName = "John Doe", DateOfBirth = new DateTime(1995,5,20), AwardTitle = "Diploma in Computer Science", CompletionDate = new DateTime(2018,6,30), TranscriptAvailable = true },
                new VerifiableCertificate { CertificateId = 1002, StudentName = "Jane Smith", DateOfBirth = new DateTime(1993,11,3), AwardTitle = "Bachelor of Arts", CompletionDate = new DateTime(2015,5,25), TranscriptAvailable = false },
                new VerifiableCertificate { CertificateId = 1003, StudentName = "Alan Turing", DateOfBirth = new DateTime(1912,6,23), AwardTitle = "PhD in Mathematics", CompletionDate = new DateTime(1936,7,1), TranscriptAvailable = true }
            };
        }

        /// <summary>
        /// Verifies a certificate by id, name and date of birth. Returns a formatted result string.
        /// </summary>
        /// <param name="certificateId">Certificate identifier</param>
        /// <param name="studentName">Student name to match (case-insensitive)</param>
        /// <param name="dob">Date of birth to match</param>
        /// <returns>Formatted verification result</returns>
        public string VerifyCertificate(int certificateId, string studentName, DateTime dob)
        {
            try
            {
                // Input validation
                if (certificateId <= 0) throw new ArgumentException("CertificateId must be a positive integer.");
                if (string.IsNullOrWhiteSpace(studentName)) throw new ArgumentException("Student name is required.");
                if (dob == default) throw new ArgumentException("Valid date of birth is required.");

                // LINQ search - match id, name (case-insensitive) and DOB
                var match = _certificates.FirstOrDefault(c => c.CertificateId == certificateId
                                                               && string.Equals(c.StudentName, studentName, StringComparison.OrdinalIgnoreCase)
                                                               && c.DateOfBirth.Date == dob.Date);

                if (match == null)
                {
                    return BuildInvalidResult(certificateId);
                }

                return BuildValidResult(match);
            }
            catch (Exception ex)
            {
                // Exception handling - return a user-friendly message
                return $"Error during verification: {ex.Message}";
            }
        }

        /// <summary>
        /// Overloaded: Verify by certificate id only. Performs a basic existence check.
        /// </summary>
        /// <param name="certificateId">Certificate identifier</param>
        /// <returns>Formatted result string</returns>
        public string VerifyCertificate(int certificateId)
        {
            try
            {
                if (certificateId <= 0) throw new ArgumentException("CertificateId must be a positive integer.");
                var match = _certificates.FirstOrDefault(c => c.CertificateId == certificateId);
                if (match == null) return BuildInvalidResult(certificateId);
                return BuildValidResult(match, showName: false, showDob: false);
            }
            catch (Exception ex)
            {
                return $"Error during verification: {ex.Message}";
            }
        }

        #region Helpers

        private string BuildValidResult(VerifiableCertificate cert, bool showName = true, bool showDob = true)
        {
            // Compose a clean multi-line output
            var parts = new List<string> { "Certificate Valid: Valid" };
            if (showName) parts.Add($"Student Name: {cert.StudentName}");
            if (showDob) parts.Add($"Date of Birth: {cert.DateOfBirth:yyyy-MM-dd}");
            parts.Add($"Award Title: {cert.AwardTitle}");
            parts.Add($"Completion Date: {cert.CompletionDate:yyyy-MM-dd}");
            parts.Add($"Transcript Available: {(cert.TranscriptAvailable ? "Yes" : "No")}");
            return string.Join(Environment.NewLine, parts);
        }

        private string BuildInvalidResult(int certificateId)
        {
            return $"Certificate (Id: {certificateId}) Invalid or Not Found";
        }

        #endregion
    }
}
