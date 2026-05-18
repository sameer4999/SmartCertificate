using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartCertificate.Utils
{
    /// <summary>
    /// FileManager handles saving and retrieving files used by the Smart Certificate System.
    /// It organizes files into structured directories under the application storage folder:
    /// /Certificates/, /Transcripts/, /Documents/.
    /// </summary>
    public static class FileManager
    {
        // Base storage folder under the application directory
        private static readonly string BaseStoragePath = Path.Combine(AppContext.BaseDirectory, "Storage");

        // Subfolders
        private const string CertificatesFolder = "Certificates";
        private const string TranscriptsFolder = "Transcripts";
        private const string DocumentsFolder = "Documents";

        static FileManager()
        {
            // Ensure base folders exist on startup
            EnsureDirectory(Path.Combine(BaseStoragePath, CertificatesFolder));
            EnsureDirectory(Path.Combine(BaseStoragePath, TranscriptsFolder));
            EnsureDirectory(Path.Combine(BaseStoragePath, DocumentsFolder));
        }

        #region Save Methods

        /// <summary>
        /// Save a certificate file for a student. Returns the saved file path on success, or null on failure.
        /// </summary>
        public static string SaveCertificateFile(string studentName, string sourceFilePath)
        {
            return SaveFileForStudent(studentName, sourceFilePath, CertificatesFolder);
        }

        /// <summary>
        /// Save a transcript file for a student.
        /// </summary>
        public static string SaveTranscriptFile(string studentName, string sourceFilePath)
        {
            return SaveFileForStudent(studentName, sourceFilePath, TranscriptsFolder);
        }

        /// <summary>
        /// Save a student document (any supporting document).
        /// </summary>
        public static string SaveStudentDocument(string studentName, string sourceFilePath)
        {
            return SaveFileForStudent(studentName, sourceFilePath, DocumentsFolder);
        }

        /// <summary>
        /// Overload: Save file with a custom file name (without extension). Ensures unique naming.
        /// </summary>
        public static string SaveStudentDocument(string studentName, string sourceFilePath, string customFileName)
        {
            return SaveFileForStudent(studentName, sourceFilePath, DocumentsFolder, customFileName);
        }

        /// <summary>
        /// Core helper that copies the source file into the destination folder under a structured path.
        /// Uses FileStream for copying and prevents overwriting by appending timestamps/GUIDs.
        /// </summary>
        private static string SaveFileForStudent(string studentName, string sourceFilePath, string subfolder, string customFileName = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentName)) throw new ArgumentException("studentName is required");
                if (string.IsNullOrWhiteSpace(sourceFilePath)) throw new ArgumentException("sourceFilePath is required");
                if (!File.Exists(sourceFilePath)) throw new FileNotFoundException("Source file not found", sourceFilePath);

                var destFolder = Path.Combine(BaseStoragePath, subfolder, CleanFileName(studentName));
                EnsureDirectory(destFolder);

                var ext = Path.GetExtension(sourceFilePath);
                if (string.IsNullOrWhiteSpace(ext)) ext = ".dat";

                string fileName;
                if (!string.IsNullOrWhiteSpace(customFileName))
                {
                    fileName = CleanFileName(customFileName) + ext;
                }
                else
                {
                    // Use timestamp + GUID to avoid collisions and preserve traceability
                    var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    fileName = $"{CleanFileName(studentName)}_{ts}_{Guid.NewGuid().ToString().Substring(0,8)}{ext}";
                }

                var destPath = Path.Combine(destFolder, fileName);

                // If destination exists, append GUID to avoid overwrite
                if (File.Exists(destPath))
                {
                    destPath = Path.Combine(destFolder, Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString().Substring(0,8) + ext);
                }

                // Copy using FileStream
                using (var sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                using (var destStream = new FileStream(destPath, FileMode.CreateNew, FileAccess.Write))
                {
                    sourceStream.CopyTo(destStream);
                }

                Console.WriteLine($"Saved file to: {destPath}");
                return destPath;
            }
            catch (FileNotFoundException fnf)
            {
                Console.WriteLine($"Save failed: {fnf.Message}");
                return null;
            }
            catch (UnauthorizedAccessException ua)
            {
                Console.WriteLine($"Unauthorized access: {ua.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Retrieve Methods

        /// <summary>
        /// Retrieve all certificate files saved for a student. Returns an empty list if none found.
        /// </summary>
        public static List<string> RetrieveCertificateFile(string studentName)
        {
            return RetrieveFilesForStudent(studentName, CertificatesFolder);
        }

        /// <summary>
        /// Retrieve transcript files for a student.
        /// </summary>
        public static List<string> RetrieveTranscriptFile(string studentName)
        {
            return RetrieveFilesForStudent(studentName, TranscriptsFolder);
        }

        /// <summary>
        /// Retrieve student documents for a student.
        /// </summary>
        public static List<string> RetrieveStudentDocument(string studentName)
        {
            return RetrieveFilesForStudent(studentName, DocumentsFolder);
        }

        /// <summary>
        /// Core helper to list files for a student in the specified subfolder.
        /// </summary>
        private static List<string> RetrieveFilesForStudent(string studentName, string subfolder)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentName)) throw new ArgumentException("studentName is required");
                var folder = Path.Combine(BaseStoragePath, subfolder, CleanFileName(studentName));
                if (!Directory.Exists(folder)) return new List<string>();
                var files = Directory.GetFiles(folder).OrderByDescending(f => File.GetCreationTimeUtc(f)).ToList();
                Console.WriteLine($"Found {files.Count} files in {folder}");
                return files;
            }
            catch (UnauthorizedAccessException ua)
            {
                Console.WriteLine($"Unauthorized access: {ua.Message}");
                return new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving files: {ex.Message}");
                return new List<string>();
            }
        }

        #endregion

        #region Optional Enhancements

        /// <summary>
        /// Deletes a file by path. Returns true if deleted.
        /// </summary>
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("filePath is required");
                if (!File.Exists(filePath)) { Console.WriteLine("File does not exist."); return false; }
                File.Delete(filePath);
                Console.WriteLine($"Deleted file: {filePath}");
                return true;
            }
            catch (UnauthorizedAccessException ua)
            {
                Console.WriteLine($"Unauthorized: {ua.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lists all files in a top-level storage subfolder (e.g. Certificates) optionally limited to a student folder.
        /// </summary>
        public static List<string> ListAllFiles(string subfolder, string studentName = null)
        {
            try
            {
                var folder = Path.Combine(BaseStoragePath, subfolder);
                if (!Directory.Exists(folder)) return new List<string>();
                if (!string.IsNullOrWhiteSpace(studentName)) folder = Path.Combine(folder, CleanFileName(studentName));
                if (!Directory.Exists(folder)) return new List<string>();
                return Directory.GetFiles(folder, "*", SearchOption.AllDirectories).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing files: {ex.Message}");
                return new List<string>();
            }
        }

        #endregion

        #region Utilities

        private static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        // Basic filename sanitizer to avoid invalid characters
        private static string CleanFileName(string input)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var cleaned = new string(input.Where(ch => !invalid.Contains(ch)).ToArray()).Trim();
            if (string.IsNullOrWhiteSpace(cleaned)) cleaned = "unnamed";
            return cleaned.Replace(' ', '_');
        }

        #endregion
    }
}
