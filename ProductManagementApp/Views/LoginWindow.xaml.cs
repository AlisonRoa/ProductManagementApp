using ProductManagementApp.Repositories;
using ProductManagementApp.Views;
using System.Windows;

namespace ProductManagementApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UsersRepository _repo = new UsersRepository();

        public LoginWindow()
        {
            InitializeComponent();
        }

        // ====== PASSWORD PLACEHOLDER HANDLERS ======
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

        // ====== BOTONES ======
        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            var id = TxtEmail.Text?.Trim();
            var pass = TxtPassword.Password;

            bool ok = await _repo.AuthenticateAsync(id, pass);
            if (!ok)
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var main = new MainWindow { Owner = this };
            main.Closed += (_, __) => this.Close();
            this.Hide();
            main.Show();
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
