using System;

namespace SmartCertificate.Models
{
    /// <summary>
    /// Represents a certificate record used by the Certificate Verification module.
    /// This class is separate from other Certificate models in the project to avoid
    /// field/name collisions and to show a compact, focused verification model.
    /// </summary>
    public class VerifiableCertificate
    {
        /// <summary>Unique certificate identifier</summary>
        public int CertificateId { get; set; }

        /// <summary>Name of the student as recorded on the certificate</summary>
        public string StudentName { get; set; }

        /// <summary>Date of birth of the student</summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>Title of the award/qualification</summary>
        public string AwardTitle { get; set; }

        /// <summary>Date when the course/award was completed</summary>
        public DateTime CompletionDate { get; set; }

        /// <summary>Indicates if the transcript document is available</summary>
        public bool TranscriptAvailable { get; set; }

        public override string ToString()
        {
            return $"[Id:{CertificateId}] {StudentName} - {AwardTitle} (Completed: {CompletionDate:yyyy-MM-dd}) Transcript: {(TranscriptAvailable ? "Yes" : "No")}";
        }
    }
}
