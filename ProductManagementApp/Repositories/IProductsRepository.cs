using ProductManagementApp.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagementApp.Models;

namespace ProductManagementApp.Repositories
{
    public interface IProductsRepository
    {
        Task<List<ProductListItem>> GetProductsAsync();
        Task<List<StatusItem>> GetStatusesAsync();

        Task<IList<OptionItem>> GetOptionsByProductAsync(int productId);
        Task<OptionItem> SaveOptionAsync(OptionItem option);
    }
}