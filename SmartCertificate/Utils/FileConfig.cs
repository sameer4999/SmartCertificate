using System;
using System.IO;

namespace SmartCertificate.Utils
{
    public static class FileConfig
    {
        // Loads connection string from appsettings.txt if present, otherwise returns LocalDB default
        public static string LoadConnectionString()
        {
            var cfg = "appsettings.txt";
            if (File.Exists(cfg))
            {
                var txt = File.ReadAllText(cfg).Trim();
                if (!string.IsNullOrWhiteSpace(txt)) return txt;
            }
            // Default to LocalDB (change as needed)
            return "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database=SmartCertificateDb;";
        }
    }
}
