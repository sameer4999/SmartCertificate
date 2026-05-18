using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Microsoft.Data.Sqlite;
using SmartCertificate.Models;
using System.Linq;

namespace SmartCertificate.Data
{
    /// <summary>
    /// DatabaseManager using SQLite for lightweight local storage.
    /// Provides CRUD operations for Students and Certificates with parameterized queries.
    /// </summary>
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
            EnsureDatabase();
        }

        /// <summary>
        /// Ensures the SQLite database tables exist.
        /// </summary>
        public void EnsureDatabase()
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                var createStudents = @"CREATE TABLE IF NOT EXISTS Students (
                    StudentId INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    DateOfBirth TEXT,
                    GPA REAL
                );";

                var createCertificates = @"CREATE TABLE IF NOT EXISTS Certificates (
                    CertificateId INTEGER PRIMARY KEY,
                    StudentName TEXT NOT NULL,
                    DateOfBirth TEXT,
                    AwardTitle TEXT,
                    CompletionDate TEXT,
                    TranscriptAvailable INTEGER
                );";

                using var cmd = conn.CreateCommand();
                cmd.CommandText = createStudents;
                cmd.ExecuteNonQuery();

                cmd.CommandText = createCertificates;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring database: {ex.Message}");
                throw;
            }
        }

        #region Student CRUD

        public bool AddStudent(StudentRecord student)
        {
            try
            {
                if (student == null) throw new ArgumentNullException(nameof(student));
                if (student.StudentId <= 0) throw new ArgumentException("StudentId must be positive");
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();

                // check duplicate
                using (var chk = conn.CreateCommand())
                {
                    chk.CommandText = "SELECT COUNT(1) FROM Students WHERE StudentId = @id";
                    chk.Parameters.AddWithValue("@id", student.StudentId);
                    var exists = Convert.ToInt32(chk.ExecuteScalar());
                    if (exists > 0)
                    {
                        Console.WriteLine($"Student with id {student.StudentId} already exists.");
                        return false;
                    }
                }

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Students(StudentId, Name, Email, DateOfBirth, GPA) VALUES(@id,@name,@email,@dob,@gpa)";
                cmd.Parameters.AddWithValue("@id", student.StudentId);
                cmd.Parameters.AddWithValue("@name", student.Name);
                cmd.Parameters.AddWithValue("@email", student.Email);
                cmd.Parameters.AddWithValue("@dob", student.DateOfBirth.ToString("o", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@gpa", student.GPA);
                cmd.ExecuteNonQuery();
                Console.WriteLine($"Student {student.Name} added.");
                return true;
            }
            catch (SqliteException sq)
            {
                Console.WriteLine($"SQLite error adding student: {sq.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding student: {ex.Message}");
                return false;
            }
        }

        public List<StudentRecord> GetAllStudents()
        {
            var list = new List<StudentRecord>();
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT StudentId, Name, Email, DateOfBirth, GPA FROM Students";
                using var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    var id = rd.GetInt32(0);
                    var name = rd.IsDBNull(1) ? string.Empty : rd.GetString(1);
                    var email = rd.IsDBNull(2) ? string.Empty : rd.GetString(2);
                    var dob = rd.IsDBNull(3) ? DateTime.MinValue : DateTime.Parse(rd.GetString(3), null, DateTimeStyles.RoundtripKind);
                    var gpa = rd.IsDBNull(4) ? 0.0 : rd.GetDouble(4);
                    list.Add(new StudentRecord(id, name, email, dob, gpa));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading students: {ex.Message}");
            }
            return list;
        }

        public StudentRecord GetStudentById(int id)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT StudentId, Name, Email, DateOfBirth, GPA FROM Students WHERE StudentId = @id";
                cmd.Parameters.AddWithValue("@id", id);
                using var rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    var sid = rd.GetInt32(0);
                    var name = rd.IsDBNull(1) ? string.Empty : rd.GetString(1);
                    var email = rd.IsDBNull(2) ? string.Empty : rd.GetString(2);
                    var dob = rd.IsDBNull(3) ? DateTime.MinValue : DateTime.Parse(rd.GetString(3), null, DateTimeStyles.RoundtripKind);
                    var gpa = rd.IsDBNull(4) ? 0.0 : rd.GetDouble(4);
                    return new StudentRecord(sid, name, email, dob, gpa);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching student: {ex.Message}");
            }
            return null;
        }

        public bool UpdateStudent(StudentRecord student)
        {
            try
            {
                if (student == null) throw new ArgumentNullException(nameof(student));
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE Students SET Name=@name, Email=@email, DateOfBirth=@dob, GPA=@gpa WHERE StudentId=@id";
                cmd.Parameters.AddWithValue("@name", student.Name);
                cmd.Parameters.AddWithValue("@email", student.Email);
                cmd.Parameters.AddWithValue("@dob", student.DateOfBirth.ToString("o", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@gpa", student.GPA);
                cmd.Parameters.AddWithValue("@id", student.StudentId);
                var rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? $"Student {student.StudentId} updated." : $"Student {student.StudentId} not found.");
                return rows > 0;
            }
            catch (SqliteException sq)
            {
                Console.WriteLine($"SQLite error updating student: {sq.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating student: {ex.Message}");
                return false;
            }
        }

        public bool DeleteStudent(int studentId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Students WHERE StudentId = @id";
                cmd.Parameters.AddWithValue("@id", studentId);
                var rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? $"Student {studentId} deleted." : $"Student {studentId} not found.");
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting student: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Certificate CRUD

        public bool AddCertificate(VerifiableCertificate cert)
        {
            try
            {
                if (cert == null) throw new ArgumentNullException(nameof(cert));
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var chk = conn.CreateCommand();
                chk.CommandText = "SELECT COUNT(1) FROM Certificates WHERE CertificateId = @id";
                chk.Parameters.AddWithValue("@id", cert.CertificateId);
                var exists = Convert.ToInt32(chk.ExecuteScalar());
                if (exists > 0)
                {
                    Console.WriteLine($"Certificate with id {cert.CertificateId} already exists.");
                    return false;
                }

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Certificates(CertificateId, StudentName, DateOfBirth, AwardTitle, CompletionDate, TranscriptAvailable) VALUES(@id,@name,@dob,@award,@comp,@trans)";
                cmd.Parameters.AddWithValue("@id", cert.CertificateId);
                cmd.Parameters.AddWithValue("@name", cert.StudentName);
                cmd.Parameters.AddWithValue("@dob", cert.DateOfBirth.ToString("o", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@award", cert.AwardTitle ?? string.Empty);
                cmd.Parameters.AddWithValue("@comp", cert.CompletionDate.ToString("o", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@trans", cert.TranscriptAvailable ? 1 : 0);
                cmd.ExecuteNonQuery();
                Console.WriteLine($"Certificate {cert.CertificateId} added.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding certificate: {ex.Message}");
                return false;
            }
        }

        public List<VerifiableCertificate> GetAllCertificates()
        {
            var list = new List<VerifiableCertificate>();
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT CertificateId, StudentName, DateOfBirth, AwardTitle, CompletionDate, TranscriptAvailable FROM Certificates";
                using var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    var id = rd.GetInt32(0);
                    var name = rd.IsDBNull(1) ? string.Empty : rd.GetString(1);
                    var dob = rd.IsDBNull(2) ? DateTime.MinValue : DateTime.Parse(rd.GetString(2), null, DateTimeStyles.RoundtripKind);
                    var award = rd.IsDBNull(3) ? string.Empty : rd.GetString(3);
                    var comp = rd.IsDBNull(4) ? DateTime.MinValue : DateTime.Parse(rd.GetString(4), null, DateTimeStyles.RoundtripKind);
                    var trans = !rd.IsDBNull(5) && rd.GetInt32(5) == 1;
                    list.Add(new VerifiableCertificate { CertificateId = id, StudentName = name, DateOfBirth = dob, AwardTitle = award, CompletionDate = comp, TranscriptAvailable = trans });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading certificates: {ex.Message}");
            }
            return list;
        }

        public bool UpdateCertificate(VerifiableCertificate cert)
        {
            try
            {
                if (cert == null) throw new ArgumentNullException(nameof(cert));
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE Certificates SET StudentName=@name, DateOfBirth=@dob, AwardTitle=@award, CompletionDate=@comp, TranscriptAvailable=@trans WHERE CertificateId=@id";
                cmd.Parameters.AddWithValue("@name", cert.StudentName);
                cmd.Parameters.AddWithValue("@dob", cert.DateOfBirth.ToString("o", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@award", cert.AwardTitle ?? string.Empty);
                cmd.Parameters.AddWithValue("@comp", cert.CompletionDate.ToString("o", CultureInfo.InvariantCulture));
                cmd.Parameters.AddWithValue("@trans", cert.TranscriptAvailable ? 1 : 0);
                cmd.Parameters.AddWithValue("@id", cert.CertificateId);
                var rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? $"Certificate {cert.CertificateId} updated." : $"Certificate {cert.CertificateId} not found.");
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating certificate: {ex.Message}");
                return false;
            }
        }

        public bool DeleteCertificate(int certificateId)
        {
            try
            {
                using var conn = new SqliteConnection(_connectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Certificates WHERE CertificateId = @id";
                cmd.Parameters.AddWithValue("@id", certificateId);
                var rows = cmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? $"Certificate {certificateId} deleted." : $"Certificate {certificateId} not found.");
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting certificate: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region LINQ Examples

        // Example: filter students with LINQ after fetching from DB
        public List<StudentRecord> GetStudentsWithMinGPA(double minGpa)
        {
            var all = GetAllStudents();
            return all.Where(s => s.GPA >= minGpa).OrderByDescending(s => s.GPA).ToList();
        }

        #endregion
    }
}
