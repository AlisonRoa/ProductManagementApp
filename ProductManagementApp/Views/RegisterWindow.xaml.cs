using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using ProductManagementApp.Models;
using ProductManagementApp.Repositories;
using ProductManagementApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

using DAValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace ProductManagementApp.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly IUsersService _svc;

        public RegisterWindow()
        {
            InitializeComponent();
            _svc = new UsersService(new UsersRepository());
        }

        // Muestra/oculta el hint del primer PasswordBox
        private void UpdatePwdHint()
        {
            if (PwdHint == null || TxtPassword == null) return;

            bool empty = string.IsNullOrEmpty(TxtPassword.Password);
            bool focused = TxtPassword.IsKeyboardFocused;
            PwdHint.Visibility = (empty && !focused) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Muestra/oculta el hint del segundo PasswordBox (confirmación)
        private void UpdatePwd2Hint()
        {
            if (Pwd2Hint == null || TxtPassword2 == null) return;

            bool empty = string.IsNullOrEmpty(TxtPassword2.Password);
            bool focused = TxtPassword2.IsKeyboardFocused;
            Pwd2Hint.Visibility = (empty && !focused) ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var (reg, hasErrors) = BuildAndValidateForView();
                if (hasErrors) return; // ya se pintaron los mensajes debajo

                // Si todo OK visualmente, llama al servicio/repositorio
                int id = await _svc.RegisterAsync(reg);

                MessageBox.Show($"Usuario creado (ID {id}).", "Registro");
                DialogResult = true;
                Close();
            }
            catch (ValidationException vex)
            {
                MessageBox.Show("Corrige los siguientes campos:\n\n" + vex.Message, "Validación");
            }
            catch (Exception ex)
            {
                var msg = ex.Message?.ToLowerInvariant() ?? "";
                if (msg.Contains("usuario"))
                    SetError(TxtUsername, ErrUsername, ex.Message);
                else if (msg.Contains("correo") || msg.Contains("email"))
                    SetError(TxtEmail, ErrEmail, ex.Message);
                else
                    MessageBox.Show("No se pudo registrar: " + ex.Message, "Error");
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
        private void BackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e) { DialogResult = false; Close(); }
        private void Close_Click(object sender, RoutedEventArgs e) => Close();


        // ====== Handlers que pide el XAML ======
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e) => UpdatePwdHint();
        private void TxtPassword_GotKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePwdHint();
        private void TxtPassword_LostKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePwdHint();

        private void TxtPassword2_PasswordChanged(object sender, RoutedEventArgs e) => UpdatePwd2Hint();
        private void TxtPassword2_GotKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePwd2Hint();
        private void TxtPassword2_LostKeyboardFocus(object sender, RoutedEventArgs e) => UpdatePwd2Hint();

        private void TxtLastNames_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }

        // Colores de borde (normal = el del estilo #E3E8EF)
        private readonly Brush _errorBrush = Brushes.Red;
        private readonly Brush _normalBrush = (Brush)new BrushConverter().ConvertFromString("#E3E8EF");

        // Limpia todos los errores visuales
        private void ClearErrors()
        {
            // TextBoxes
            TxtUsername.BorderBrush = _normalBrush; ErrUsername.Visibility = Visibility.Collapsed;
            TxtNames.BorderBrush = _normalBrush; ErrNames.Visibility = Visibility.Collapsed;
            TxtLastNames.BorderBrush = _normalBrush; ErrLastNames.Visibility = Visibility.Collapsed;
            TxtPhone.BorderBrush = _normalBrush; ErrPhone.Visibility = Visibility.Collapsed;
            TxtEmail.BorderBrush = _normalBrush; ErrEmail.Visibility = Visibility.Collapsed;
            // Passwords
            TxtPassword.BorderBrush = _normalBrush; ErrPassword.Visibility = Visibility.Collapsed;
            TxtPassword2.BorderBrush = _normalBrush; ErrPassword2.Visibility = Visibility.Collapsed;
        }

        // Pinta borde rojo + muestra mensaje debajo
        private void SetError(Control ctrl, TextBlock err, string message)
        {
            ctrl.BorderBrush = _errorBrush;
            err.Text = message;
            err.Visibility = Visibility.Visible;
        }

        // Aplica errores de DataAnnotations por campo
        private void ApplyAnnotationErrors(IList<DAValidationResult> errors)
        {
            foreach (var err in errors)
            {
                var member = err.MemberNames?.FirstOrDefault() ?? "";
                var msg = err.ErrorMessage ?? "Dato inválido.";

                switch (member)
                {
                    case nameof(UserRegistration.Username):
                        SetError(TxtUsername, ErrUsername, msg); break;

                    case nameof(UserRegistration.Names):
                        SetError(TxtNames, ErrNames, msg); break;

                    case nameof(UserRegistration.LastNames):
                        SetError(TxtLastNames, ErrLastNames, msg); break;

                    case nameof(UserRegistration.TelephoneNumber):
                        SetError(TxtPhone, ErrPhone, msg); break;

                    case nameof(UserRegistration.Email):
                        SetError(TxtEmail, ErrEmail, msg); break;

                    case nameof(UserRegistration.Password):
                        SetError(TxtPassword, ErrPassword, msg); break;

                    default:
                        // Si no hay MemberName, muéstralo de forma general (elige dónde)
                        SetError(TxtUsername, ErrUsername, msg); break;
                }
            }
        }

        // Construye el DTO y valida SOLO para mostrar errores en la vista.
        // Devuelve (reg, tieneErrores)
        private (UserRegistration reg, bool hasErrors) BuildAndValidateForView()
        {
            ClearErrors();

            var reg = new UserRegistration
            {
                Username = (TxtUsername.Text ?? "").Trim(),
                Password = TxtPassword.Password ?? "",
                Names = (TxtNames.Text ?? "").Trim(),
                LastNames = string.IsNullOrWhiteSpace(TxtLastNames.Text) ? "-" : TxtLastNames.Text.Trim(),
                TelephoneNumber = (TxtPhone.Text ?? "").Trim(),
                Email = (TxtEmail.Text ?? "").Trim()
            };

            // 1) Valida con DataAnnotations (las reglas ya están en el modelo)
            var ctx = new ValidationContext(reg);
            var results = new List<DAValidationResult>();
            Validator.TryValidateObject(reg, ctx, results, validateAllProperties: true);

            // 2) Validación de confirmación de contraseña (no es DataAnnotation aquí)
            if (TxtPassword.Password != TxtPassword2.Password)
                results.Add(new DAValidationResult("Las contraseñas no coinciden.", new[] { nameof(UserRegistration.Password) }));

            if (results.Count > 0)
                ApplyAnnotationErrors(results);

            return (reg, results.Count > 0);
        }

    }
}
