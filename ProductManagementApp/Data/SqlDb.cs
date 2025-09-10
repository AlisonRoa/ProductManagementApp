using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProductManagementApp.Data
{
    public static class SqlDb
    {
        private static readonly string _cs = ResolveConnectionString();

        private static string ResolveConnectionString()
        {
            // Intenta PM
            var cs = ConfigurationManager.ConnectionStrings["PM"]?.ConnectionString;

            // Fallback: ProductManagement (el que usas en otros archivos)
            if (string.IsNullOrWhiteSpace(cs))
                cs = ConfigurationManager.ConnectionStrings["ProductManagement"]?.ConnectionString;

            // Fallback: variable de entorno (útil para dev/CI)
            if (string.IsNullOrWhiteSpace(cs))
                cs = Environment.GetEnvironmentVariable("PM_CONNSTR");

            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException(
                    "No se encontró una cadena de conexión. Agrega <connectionStrings> en App.config " +
                    "con name='PM' o name='ProductManagement', o define la variable de entorno PM_CONNSTR.");

            return cs;
        }

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
                await cn.OpenAsync().ConfigureAwait(false);
                var x = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                return Convert.ToInt32(x) == 1;
            }
        }
    }
}
