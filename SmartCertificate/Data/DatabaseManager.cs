using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SmartCertificate.Data
{
    public class DatabaseManager
    {
        private readonly string _connectionString;
        public DatabaseManager(string connectionString) { _connectionString = connectionString; }
        public IDbConnection GetConnection()
        {
            var c = new SqlConnection(_connectionString);
            c.Open();
            return c;
        }
    }
}
