using ProductManagementApp.Data;
using ProductManagementApp.Models;
using ProductManagementApp.Security;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProductManagementApp.Repositories
{
    public class UsersRepository
    {
        // #region Validaciones de formato
        private static bool IsEmailCom(string email) =>
            !string.IsNullOrWhiteSpace(email) &&
            Regex.IsMatch(email.Trim(), @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.com$", RegexOptions.IgnoreCase);

        private static bool IsPhoneNic(string phone) =>
            !string.IsNullOrWhiteSpace(phone) &&
            Regex.IsMatch(phone.Trim(), @"^\d{4}-\d{4}$");

        // #endregion
        // #region Chequeos de duplicados
        public async Task<bool> IsUsernameTakenAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            using (var cn = SqlDb.GetOpen())
            using (var cmd = new SqlCommand(
                @"SELECT TOP 1 1 FROM PM.Users WHERE Username = @u;", cn))
            {
                cmd.Parameters.AddWithValue("@u", username.Trim());
                var r = await cmd.ExecuteScalarAsync();
                return r != null;
            }
        }
        
        public async Task<bool> IsEmailTakenAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            using (var cn = SqlDb.GetOpen())
            using (var cmd = new SqlCommand(
                @"SELECT TOP 1 1 FROM PM.Users WHERE LOWER(Email) = @e;", cn))
            {
                cmd.Parameters.AddWithValue("@e", email.Trim().ToLowerInvariant());
                var r = await cmd.ExecuteScalarAsync();
                return r != null;
            }
        }
        // #endregion

        // #region Registro y Autenticación
        public async Task<int> RegisterAsync(UserRegistration reg)
        {
            if (reg == null) throw new ArgumentNullException(nameof(reg));
            if (string.IsNullOrWhiteSpace(reg.Username))
                throw new ArgumentException("Username es obligatorio.");
            if (string.IsNullOrEmpty(reg.Password))
                throw new ArgumentException("Password es obligatorio.");
            if (string.IsNullOrWhiteSpace(reg.Names))
                throw new ArgumentException("El nombre es obligatorio.");

            // 1) Validación de formato
            if (!IsEmailCom(reg.Email ?? ""))
                throw new ArgumentException("Correo inválido. Debe ser usuario@dominio.com (solo .com).");
            if (!IsPhoneNic(reg.TelephoneNumber ?? ""))
                throw new ArgumentException("Teléfono inválido. Debe ser 0000-0000.");

            // 2) Chequeo de duplicados
            if (await IsUsernameTakenAsync(reg.Username))
                throw new ArgumentException("Ese nombre de usuario ya está en uso.");
            if (await IsEmailTakenAsync(reg.Email))
                throw new ArgumentException("Ese correo ya está registrado.");

            // 3) Insert
            string hash = PasswordHasher.Sha256Hex(reg.Password);

            using (var cn = SqlDb.GetOpen())
            using (var cmd = new SqlCommand(@"
                INSERT INTO PM.Users
                    (Username, PasswordHash, Names, LastNames, TelephoneNumber, Email, CreationDate)
                VALUES
                    (@u, @h, @n, @ln, @ph, @em, SYSUTCDATETIME());
                SELECT SCOPE_IDENTITY();", cn))
            {
                cmd.Parameters.AddWithValue("@u", reg.Username.Trim());
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.Parameters.AddWithValue("@n", reg.Names.Trim());
                cmd.Parameters.AddWithValue("@ln", string.IsNullOrWhiteSpace(reg.LastNames) ? "-" : reg.LastNames.Trim());
                cmd.Parameters.AddWithValue("@ph", reg.TelephoneNumber.Trim());
                cmd.Parameters.AddWithValue("@em", reg.Email.Trim());

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
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 1
                FROM PM.Users
                WHERE (Username = @id OR Email = @id) AND PasswordHash = @h;", cn))
            {
                cmd.Parameters.AddWithValue("@id", userOrEmail.Trim());
                cmd.Parameters.AddWithValue("@h", hash);
                var r = await cmd.ExecuteScalarAsync();
                return r != null;
            }
        }
        // #endregion
    }
}
