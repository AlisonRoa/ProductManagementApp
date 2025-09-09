using System.ComponentModel.DataAnnotations;

namespace ProductManagementApp.Models
{
    public class LoginRequest
    {
        [Required, MinLength(3)]
        public string Identifier { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}