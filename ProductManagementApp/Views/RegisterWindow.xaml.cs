using System;
using System.Windows;
using ProductManagementApp.Models;
using ProductManagementApp.Repositories;

namespace ProductManagementApp.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UsersRepository _repo = new UsersRepository();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        // ====== Watermark de PasswordBoxes ======
        private void UpdatePwdHint()
        {
            if (PwdHint == null || TxtPassword == null) return;

            bool empty = string.IsNullOrEmpty(TxtPassword.Password);
            bool focused = TxtPassword.IsKeyboardFocused;
            PwdHint.Visibility = (empty && !focused) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdatePwd2Hint()
        {
            if (Pwd2Hint == null || TxtPassword2 == null) return;

            bool empty = string.IsNullOrEmpty(TxtPassword2.Password);
            bool focused = TxtPassword2.IsKeyboardFocused;
            Pwd2Hint.Visibility = (empty && !focused) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e) => UpdatePwdHint();
        private void TxtPassword_GotKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePwdHint();
        private void TxtPassword_LostKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePwdHint();

        private void TxtPassword2_PasswordChanged(object sender, RoutedEventArgs e) => UpdatePwd2Hint();
        private void TxtPassword2_GotKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePwd2Hint();
        private void TxtPassword2_LostKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePwd2Hint();

        // ====== Botones ======
        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxtPassword.Password != TxtPassword2.Password)
                    throw new ArgumentException("Las contraseñas no coinciden.");

                var reg = new UserRegistration
                {
                    Username = TxtUsername.Text?.Trim(),
                    Password = TxtPassword.Password,
                    Names = TxtNames.Text?.Trim(),
                    LastNames = TxtLastNames.Text?.Trim(),
                    TelephoneNumber = TxtPhone.Text?.Trim(),
                    Email = TxtEmail.Text?.Trim()
                };
                int id = await _repo.RegisterAsync(reg);

                MessageBox.Show($"Usuario creado (ID {id}).", "Registro");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar: " + ex.Message, "Error");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void BackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DialogResult = false; // opcional
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void TxtLastNames_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
    }
}
