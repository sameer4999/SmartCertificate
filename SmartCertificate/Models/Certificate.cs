using System;

namespace SmartCertificate.Models
{
    public class Certificate
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string HolderName { get; set; }
        public string Issuer { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {SerialNumber} - {HolderName} (Issuer: {Issuer ?? "N/A"}) Issued: {IssueDate:yyyy-MM-dd} Expires: {ExpiryDate:yyyy-MM-dd} Revoked: {IsRevoked}";
        }
    }
}
