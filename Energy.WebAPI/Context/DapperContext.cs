using Microsoft.Data.SqlClient;
using System.Data;

namespace Energy.WebAPI.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configration)
        {
            _configration = configration;
            _connectionString = _configration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
