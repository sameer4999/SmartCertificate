using System;

namespace SmartCertificate.Models
{
    public class SimpleCertificate
    {
        public int CertificateId { get; set; }
        public string StudentName { get; set; }

        public SimpleCertificate() { }
        public SimpleCertificate(int id, string studentName)
        {
            CertificateId = id; StudentName = studentName;
        }

        public override string ToString() => $"[Cert:{CertificateId}] {StudentName}";
    }
}
