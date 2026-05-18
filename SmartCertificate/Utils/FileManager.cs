using System;
using System.IO;

namespace SmartCertificate.Utils
{
    public static class FileManager
    {
        public static void WriteText(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        public static string ReadText(string path)
        {
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }
    }
}
