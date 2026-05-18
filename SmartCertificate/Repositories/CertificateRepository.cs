using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SmartCertificate.Data;
using SmartCertificate.Models;

namespace SmartCertificate.Repositories
{
    public class CertificateRepository
    {
        private readonly Database _db;

        public CertificateRepository(Database db)
        {
            _db = db;
        }

        public void EnsureTable()
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Certificates' and xtype='U')
+BEGIN
+    CREATE TABLE Certificates(
+        Id INT IDENTITY(1,1) PRIMARY KEY,
+        SerialNumber NVARCHAR(100) UNIQUE,
+        HolderName NVARCHAR(200),
+        Issuer NVARCHAR(200),
+        IssueDate DATETIME,
+        ExpiryDate DATETIME,
+        IsRevoked BIT
+    )
+END";
            cmd.ExecuteNonQuery();
        }

        public List<Certificate> GetAll()
        {
            var list = new List<Certificate>();
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, SerialNumber, HolderName, Issuer, IssueDate, ExpiryDate, IsRevoked FROM Certificates";
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(ReadCertificate(rd));
            }
            return list;
        }

        public Certificate GetById(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, SerialNumber, HolderName, Issuer, IssueDate, ExpiryDate, IsRevoked FROM Certificates WHERE Id=@id";
            var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = id; cmd.Parameters.Add(p);
            using var rd = cmd.ExecuteReader();
            if (rd.Read()) return ReadCertificate(rd);
            return null;
        }

        public Certificate GetBySerial(string serial)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, SerialNumber, HolderName, Issuer, IssueDate, ExpiryDate, IsRevoked FROM Certificates WHERE SerialNumber=@s";
            var p = cmd.CreateParameter(); p.ParameterName = "@s"; p.Value = serial; cmd.Parameters.Add(p);
            using var rd = cmd.ExecuteReader();
            if (rd.Read()) return ReadCertificate(rd);
            return null;
        }

        public int Add(Certificate cert)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Certificates(SerialNumber,HolderName,Issuer,IssueDate,ExpiryDate,IsRevoked) OUTPUT INSERTED.Id VALUES(@sn,@h,@i,@iss,@exp,@rev)";
            AddParam(cmd, "@sn", cert.SerialNumber);
            AddParam(cmd, "@h", cert.HolderName);
            AddParam(cmd, "@i", cert.Issuer ?? "");
            AddParam(cmd, "@iss", cert.IssueDate);
            AddParam(cmd, "@exp", cert.ExpiryDate);
            AddParam(cmd, "@rev", cert.IsRevoked);
            var id = (int)cmd.ExecuteScalar();
            cert.Id = id;
            return id;
        }

        public void Update(Certificate cert)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Certificates SET SerialNumber=@sn,HolderName=@h,Issuer=@i,IssueDate=@iss,ExpiryDate=@exp,IsRevoked=@rev WHERE Id=@id";
            AddParam(cmd, "@sn", cert.SerialNumber);
            AddParam(cmd, "@h", cert.HolderName);
            AddParam(cmd, "@i", cert.Issuer ?? "");
            AddParam(cmd, "@iss", cert.IssueDate);
            AddParam(cmd, "@exp", cert.ExpiryDate);
            AddParam(cmd, "@rev", cert.IsRevoked);
            AddParam(cmd, "@id", cert.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Certificates WHERE Id=@id";
            AddParam(cmd, "@id", id);
            cmd.ExecuteNonQuery();
        }

        private Certificate ReadCertificate(IDataReader rd)
        {
            return new Certificate
            {
                Id = rd.GetInt32(0),
                SerialNumber = rd.GetString(1),
                HolderName = rd.IsDBNull(2) ? null : rd.GetString(2),
                Issuer = rd.IsDBNull(3) ? null : rd.GetString(3),
                IssueDate = rd.IsDBNull(4) ? DateTime.MinValue : rd.GetDateTime(4),
                ExpiryDate = rd.IsDBNull(5) ? DateTime.MinValue : rd.GetDateTime(5),
                IsRevoked = !rd.IsDBNull(6) && rd.GetBoolean(6)
            };
        }

        private void AddParam(IDbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
