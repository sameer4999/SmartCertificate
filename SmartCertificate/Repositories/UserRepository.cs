using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SmartCertificate.Data;
using SmartCertificate.Models;

namespace SmartCertificate.Repositories
{
    public class UserRepository
    {
        private readonly Database _db;

        public UserRepository(Database db)
        {
            _db = db;
        }

        public void EnsureTable()
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"IF OBJECT_ID('dbo.Users','U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) UNIQUE,
        Email NVARCHAR(200),
        PasswordHash NVARCHAR(200),
        Role NVARCHAR(50),
        CreatedAt DATETIME
    );
END";
            cmd.ExecuteNonQuery();
        }

        public int Add(User user)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users(Username,Email,PasswordHash,Role,CreatedAt) OUTPUT INSERTED.Id VALUES(@u,@e,@p,@r,@c)";
            AddParam(cmd, "@u", user.Username);
            AddParam(cmd, "@e", user.Email);
            AddParam(cmd, "@p", user.PasswordHash);
            AddParam(cmd, "@r", user.Role);
            AddParam(cmd, "@c", user.CreatedAt);
            var id = (int)cmd.ExecuteScalar();
            user.Id = id;
            return id;
        }

        public User GetByUsername(string username)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id,Username,Email,PasswordHash,Role,CreatedAt FROM Users WHERE Username=@u";
            AddParam(cmd, "@u", username);
            using var rd = cmd.ExecuteReader();
            if (rd.Read()) return ReadUser(rd);
            return null;
        }

        public void AddParam(IDbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value ?? DBNull.Value; cmd.Parameters.Add(p);
        }

        private User ReadUser(IDataReader rd)
        {
            return new BaseUser
            {
                Id = rd.GetInt32(0),
                Username = rd.IsDBNull(1)?null:rd.GetString(1),
                Email = rd.IsDBNull(2)?null:rd.GetString(2),
                PasswordHash = rd.IsDBNull(3)?null:rd.GetString(3),
                Role = rd.IsDBNull(4)?null:rd.GetString(4),
                CreatedAt = rd.IsDBNull(5)?DateTime.MinValue:rd.GetDateTime(5)
            };
        }

        public List<User> GetAll()
        {
            var list = new List<User>();
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id,Username,Email,PasswordHash,Role,CreatedAt FROM Users";
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) list.Add(ReadUser(rd));
            return list;
        }

        public User GetById(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id,Username,Email,PasswordHash,Role,CreatedAt FROM Users WHERE Id=@id";
            AddParam(cmd, "@id", id);
            using var rd = cmd.ExecuteReader();
            if (rd.Read()) return ReadUser(rd);
            return null;
        }

        public void UpdateRole(string username, string role)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Users SET Role=@r WHERE Username=@u";
            AddParam(cmd, "@r", role);
            AddParam(cmd, "@u", username);
            cmd.ExecuteNonQuery();
        }

        public void DeleteByUsername(string username)
        {
            using var conn = _db.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Users WHERE Username=@u";
            AddParam(cmd, "@u", username);
            cmd.ExecuteNonQuery();
        }

        // Simple base user for read purposes
        private class BaseUser : User { }
    }
}
