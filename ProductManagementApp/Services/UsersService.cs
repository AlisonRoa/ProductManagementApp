using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ProductManagementApp.Common;
using ProductManagementApp.Models;
using ProductManagementApp.Repositories;

using DAValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace ProductManagementApp.Services
{
    public class UsersService : IUsersService
    {
        private readonly UsersRepository _repo;
        public UsersService(UsersRepository repo) { _repo = repo; }

        public async Task<int> RegisterAsync(UserRegistration reg)
        {
            var errs = ValidationHelper.Validate(reg);
            if (errs.Any())
                throw new ValidationException(string.Join(Environment.NewLine, errs.Select(e => "• " + e.ErrorMessage)));

            return await _repo.RegisterAsync(reg);
        }

        public async Task<(int id, IList<DAValidationResult> errors)> RegisterWithValidationAsync(UserRegistration reg)
        {
            if (reg == null) throw new ArgumentNullException(nameof(reg));

            var errors = ValidationHelper.Validate(reg);
            if (errors.Any()) return (0, errors);

            if (await _repo.IsUsernameTakenAsync(reg.Username?.Trim())) 
                return (0, new[] { new DAValidationResult("Ese nombre de usuario ya está en uso.", new[] { nameof(UserRegistration.Username) }) });
            if (await _repo.IsEmailTakenAsync(reg.Email?.Trim().ToLowerInvariant())) 
                return (0, new[] { new DAValidationResult("Ese correo ya está registrado.", new[] { nameof(UserRegistration.Email) }) });

            int id = await _repo.RegisterAsync(reg);
            return (id, Array.Empty<DAValidationResult>());
        }

        public Task<bool> AuthenticateAsync(string userOrEmail, string passwordPlain)
            => _repo.AuthenticateAsync(userOrEmail, passwordPlain);
    }
}
