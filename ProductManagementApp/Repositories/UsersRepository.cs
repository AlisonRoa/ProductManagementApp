using ProductManagementApp.Data;
using ProductManagementApp.Models;
using ProductManagementApp.Security;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProductManagementApp.Repositories
{
    public class UsersRepository
    {
        public async Task<int> RegisterAsync(UserRegistration reg)
        {
            if (reg == null) throw new ArgumentNullException(nameof(reg));
            if (string.IsNullOrWhiteSpace(reg.Username))
                throw new ArgumentException("Username es obligatorio.");
            if (string.IsNullOrEmpty(reg.Password))
                throw new ArgumentException("Password es obligatorio.");

            string hash = PasswordHasher.Sha256Hex(reg.Password);

            using (var cn = SqlDb.GetOpen())
            using (var cmd = new SqlCommand(@"INSERT INTO PM.Users (Username, PasswordHash, Names, LastNames, TelephoneNumber, Email, CreationDate)
                                                    VALUES (@u, @h, @n, @ln, @ph, @em, SYSUTCDATETIME());
                                                    SELECT SCOPE_IDENTITY();", cn))
            {
                cmd.Parameters.AddWithValue("@u", reg.Username.Trim());
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.Parameters.AddWithValue("@n", string.IsNullOrWhiteSpace(reg.Names) ? "-" : reg.Names.Trim());
                cmd.Parameters.AddWithValue("@ln", string.IsNullOrWhiteSpace(reg.LastNames) ? "-" : reg.LastNames.Trim());
                cmd.Parameters.AddWithValue("@ph", reg.TelephoneNumber ?? "");
                cmd.Parameters.AddWithValue("@em", reg.Email ?? "");

                var id = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(id);
            }
        }

        public async Task<bool> AuthenticateAsync(string userOrEmail, string passwordPlain)
        {
            if (string.IsNullOrWhiteSpace(userOrEmail) || string.IsNullOrEmpty(passwordPlain))
                return false;

            string hash = PasswordHasher.Sha256Hex(passwordPlain);

            using (var cn = SqlDb.GetOpen())
            using (var cmd = new SqlCommand(@"SELECT TOP 1 1
                                                    FROM PM.Users
                                                    WHERE (Username = @id OR Email = @id) AND PasswordHash = @h;", cn))
            {
                cmd.Parameters.AddWithValue("@id", userOrEmail.Trim());
                cmd.Parameters.AddWithValue("@h", hash);
                var r = await cmd.ExecuteScalarAsync();
                return r != null;
            }
        }

}
}
