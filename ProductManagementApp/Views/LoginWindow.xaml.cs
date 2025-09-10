using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProductManagementApp.Services;
using ProductManagementApp.Repositories;

namespace ProductManagementApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IUsersService _svc;

        public LoginWindow()
        {
            InitializeComponent();
            _svc = new UsersService(new UsersRepository());
        }

        // ====== PASSWORD PLACEHOLDER ======
        private void UpdatePasswordHintVisibility()
        {
            if (PwdHint == null || TxtPassword == null) return;
            bool empty = string.IsNullOrEmpty(TxtPassword.Password);
            bool focused = TxtPassword.IsKeyboardFocused;
            PwdHint.Visibility = (empty && !focused) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e) => UpdatePasswordHintVisibility();
        private void TxtPassword_GotKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePasswordHintVisibility();
        private void TxtPassword_LostKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePasswordHintVisibility();

        // ====== Helpers visuales ======
        private void ClearFieldErrors()
        {
            Action<Control> Clear = c => { if (c == null) return; c.BorderBrush = null; c.ToolTip = null; };
            Clear(TxtEmail); // aquí usas user/email en el mismo TextBox
            Clear(TxtPassword);
        }
        private void MarkError(Control c, string msg)
        {
            if (c == null) return;
            c.BorderBrush = Brushes.Red;
            c.ToolTip = msg;
        }

        // ====== BOTONES ======
        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearFieldErrors();

                var id = (TxtEmail.Text ?? "").Trim();   // puede ser username o email
                var pass = TxtPassword.Password ?? "";

                bool okLocal = true;
                if (string.IsNullOrWhiteSpace(id)) { MarkError(TxtEmail, "Obligatorio"); okLocal = false; }
                if (string.IsNullOrEmpty(pass)) { MarkError(TxtPassword, "Obligatorio"); okLocal = false; }
                if (!okLocal) { MessageBox.Show("Completa usuario/email y contraseña.", "Login"); return; }

                // Evita doble click
                SignInButton.IsEnabled = false;

                bool ok = await _svc.AuthenticateAsync(id, pass);
                if (!ok)
                {
                    MarkError(TxtEmail, "Revisa usuario/email");
                    MarkError(TxtPassword, "Revisa contraseña");
                    TxtPassword.Clear();
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Login",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var main = new MainWindow { Owner = this };
                main.Closed += (_, __) => this.Close();
                this.Hide();
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo iniciar sesión: " + ex.Message, "Error");
            }
            finally
            {
                SignInButton.IsEnabled = true;
            }
        }

        private void SignInGoogle_Click(object sender, RoutedEventArgs e)
            => MessageBox.Show("Google Sign-in (demo visual)");

        private void ForgotPassword_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => MessageBox.Show("Recuperación de contraseña (demo)");

        private void SignUp_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var reg = new RegisterWindow { Owner = this };
            reg.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
