using Microsoft.Data.SqlClient;
using System.Configuration;

namespace ProjectTaskManagement.DAL
{
    public class DatabaseAccess
    {
        protected string connectionString = "Server=localhost;Database=GarageDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
