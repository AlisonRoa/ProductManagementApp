using System.ComponentModel.DataAnnotations;

namespace ProductManagementApp.Models
{
    /// <summary>
    /// Proyección para el login
    /// [DTO] - Data Transfer Object NO ES UNA ENTIDAD DE BASE DE DATOS
    /// </summary>
    public class LoginRequest
    {
        [Required, MinLength(3)]
        public string Identifier { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}