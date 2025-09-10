using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagementApp.Models;

using DAValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace ProductManagementApp.Services
{
    public interface IUsersService
    {
        Task<int> RegisterAsync(UserRegistration reg);
        Task<(int id, IList<DAValidationResult> errors)> RegisterWithValidationAsync(UserRegistration reg);
        Task<bool> AuthenticateAsync(string userOrEmail, string passwordPlain);
    }
}