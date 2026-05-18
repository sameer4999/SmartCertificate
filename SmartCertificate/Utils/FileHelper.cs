using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SmartCertificate.Models;

namespace SmartCertificate.Utils
{
    public static class FileHelper
    {
        public static List<Certificate> ReadCertificatesFromCsv(string path)
        {
            var list = new List<Certificate>();
            if (!File.Exists(path)) return list;
            using var sr = new StreamReader(path);
            string header = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                try
                {
                    var cert = new Certificate
                    {
                        SerialNumber = parts[0].Trim(),
                        HolderName = parts[1].Trim(),
                        Issuer = parts.Length>2?parts[2].Trim():"",
                        IssueDate = DateTime.ParseExact(parts[3].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ExpiryDate = DateTime.ParseExact(parts[4].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        IsRevoked = parts.Length>5 && bool.TryParse(parts[5].Trim(), out var r) && r
                    };
                    list.Add(cert);
                }
                catch { continue; }
            }
            return list;
        }

        public static void WriteLog(string path, string message)
        {
            using var fs = new FileStream(path, FileMode.Append, FileAccess.Write);
            using var sw = new StreamWriter(fs);
            sw.WriteLine(message);
        }
    }
}
