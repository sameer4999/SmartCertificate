using System;
using System.Collections.Generic;
using System.Linq;
using SmartCertificate.Models;

namespace SmartCertificate.Services
{
    /// <summary>
    /// Search and sorting utilities for students and certificates.
    /// Uses LINQ and in-memory collections.
    /// </summary>
    public class SearchSortService
    {
        private readonly List<SimpleStudent> _students;
        private readonly List<SimpleCertificate> _certificates;

        public SearchSortService(List<SimpleStudent> students = null, List<SimpleCertificate> certificates = null)
        {
            _students = students ?? new List<SimpleStudent>
            {
                new SimpleStudent(1, "John Doe", 3.5),
                new SimpleStudent(2, "Jane Smith", 3.8),
                new SimpleStudent(3, "Alice Johnson", 3.8),
                new SimpleStudent(4, "Bob Brown", 2.9)
            };

            _certificates = certificates ?? new List<SimpleCertificate>
            {
                new SimpleCertificate(1001, "John Doe"),
                new SimpleCertificate(1002, "Jane Smith"),
                new SimpleCertificate(1003, "Alice Johnson")
            };
        }

        #region Search

        /// <summary>
        /// Search student by exact or partial name (case-insensitive). Returns list of matches.
        /// </summary>
        public List<SimpleStudent> SearchStudentByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name)) return new List<SimpleStudent>();
                name = name.Trim();
                var results = _students.Where(s => s.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                return results;
            }
            catch (Exception)
            {
                return new List<SimpleStudent>();
            }
        }

        /// <summary>
        /// Search certificate by id. Returns the certificate or null if not found.
        /// </summary>
        public SimpleCertificate SearchCertificateById(int certificateId)
        {
            try
            {
                return _certificates.FirstOrDefault(c => c.CertificateId == certificateId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Sort

        /// <summary>
        /// Sort students by GPA in descending order. Stable sort to preserve name order on tie.
        /// </summary>
        public List<SimpleStudent> SortStudentsByGPA(List<SimpleStudent> students = null)
        {
            var src = students ?? _students;
            return src.OrderByDescending(s => s.GPA).ThenBy(s => s.Name).ToList();
        }

        /// <summary>
        /// Sort students alphabetically A-Z by Name.
        /// </summary>
        public List<SimpleStudent> SortStudentsAlphabetically(List<SimpleStudent> students = null)
        {
            var src = students ?? _students;
            return src.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        #endregion

        #region Utilities

        public void PrintStudents(IEnumerable<SimpleStudent> list)
        {
            foreach (var s in list) Console.WriteLine(s);
        }

        public void PrintCertificate(SimpleCertificate cert)
        {
            if (cert == null) Console.WriteLine("Certificate not found."); else Console.WriteLine(cert);
        }

        #endregion
    }
}
