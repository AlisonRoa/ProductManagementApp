using System;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementApp.Models
{
    /// <summary>
    /// Proyección para el registro de usuario
    /// [DTO] - Data Transfer Object NO ES UNA ENTIDAD DE BASE DE DATOS
    /// </summary>
    public class UserRegistration
    {
        [Required, MinLength(3), MaxLength(30)]
        public string Username { get; set; }

        [Required, MinLength(6), MaxLength(128)]
        public string Password { get; set; }

        [Required, MinLength(2), MaxLength(80)]
        public string Names { get; set; }

        [MaxLength(80)]
        public string LastNames { get; set; }

        // Teléfono: 0000-0000
        [Required, RegularExpression(@"^\d{4}-\d{4}$",
             ErrorMessage = "El teléfono debe tener el formato 0000-0000.")]
        public string TelephoneNumber { get; set; }

        // Email: usuario@dominio.com (solo .com)
        [Required, RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.com$",
             ErrorMessage = "El correo debe ser usuario@dominio.com (solo .com).")]
        public string Email { get; set; }
    }

}