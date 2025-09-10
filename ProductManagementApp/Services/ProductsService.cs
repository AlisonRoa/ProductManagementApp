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

        public ProductsService(IProductsRepository repo)
        {
            _repo = repo;
        }

        public async Task<IReadOnlyList<ProductListItem>> GetProductsAsync()
        {
            var list = await _repo.GetProductsAsync();
            return list;
        }

        public async Task<IReadOnlyList<StatusItem>> GetStatusesAsync()
        {
            var list = await _repo.GetStatusesAsync();
            return list;
        }
    }
}
