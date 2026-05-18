using System;
using SmartCertificate.Models;
using SmartCertificate.Repositories;

namespace SmartCertificate.Services
{
    public class CertificateService
    {
        private readonly CertificateRepository _repo;

        public CertificateService(CertificateRepository repo)
        {
            _repo = repo;
        }

        // Overload 1: Verify by Id
        public string VerifyCertificate(int id)
        {
            var cert = _repo.GetById(id);
            return VerifyCertificate(cert);
        }

        // Overload 2: Verify by Serial
        public string VerifyCertificate(string serial)
        {
            var cert = _repo.GetBySerial(serial);
            return VerifyCertificate(cert);
        }

        // Common verify logic
        private string VerifyCertificate(Certificate cert)
        {
            try
            {
                if (cert == null) return "NotFound";
                if (cert.IsRevoked) return "Revoked";
                if (DateTime.UtcNow < cert.IssueDate) return "NotIssuedYet";
                if (DateTime.UtcNow > cert.ExpiryDate) return "Expired";
                return "Valid";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}
