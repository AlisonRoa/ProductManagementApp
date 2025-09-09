using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;


namespace ProductManagementApp.Data
{
    public static class SqlDb
    {
        private static readonly string _cs = ConfigurationManager.ConnectionStrings["PM"].ConnectionString;

        public static SqlConnection GetOpen()
        {
            var cn = new SqlConnection(_cs);
            cn.Open();
            return cn;
        }

        public static async Task<bool> TestAsync()
        {
            using (var cn = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT 1", cn))
            {
                await cn.OpenAsync();
                var x = await cmd.ExecuteScalarAsync();
                return (int)x == 1;
            }
        }
    }
}
