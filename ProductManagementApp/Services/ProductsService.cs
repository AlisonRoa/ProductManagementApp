using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using ProductManagementApp.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductManagementApp.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _repo;
        public ProductsService(IProductsRepository repo) => _repo = repo;

        public async Task<IReadOnlyList<ProductListItem>> GetProductsAsync()
            => await _repo.GetProductsAsync();

        public async Task<IReadOnlyList<StatusItem>> GetStatusesAsync()
            => await _repo.GetStatusesAsync();

        public Task<IList<OptionItem>> GetOptionsByProductAsync(int productId)
            => _repo.GetOptionsByProductAsync(productId);

        public Task<OptionItem> SaveOptionAsync(OptionItem option)
            => _repo.SaveOptionAsync(option);

        public Task DeleteOptionAsync(int optionId)
            => _repo.DeleteOptionAsync(optionId);

        public Task<bool> OptionCodeExistsAsync(string code, int? excludeId = null)
            => _repo.OptionCodeExistsAsync(code, excludeId);

        public Task<string> GetNextOtpCodeAsync()
            => _repo.GetNextOtpCodeAsync();
    }
}
