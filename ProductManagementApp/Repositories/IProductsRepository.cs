using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductManagementApp.Repositories
{
    public interface IProductsRepository
    {
        Task<List<ProductListItem>> GetProductsAsync();
        Task<List<StatusItem>> GetStatusesAsync();
        Task<IList<OptionItem>> GetOptionsByProductAsync(int productId);
        Task<OptionItem> SaveOptionAsync(OptionItem option);
        Task DeleteOptionAsync(int optionId);
        Task<bool> OptionCodeExistsAsync(string code, int? excludeId = null);
        Task<string> GetNextOtpCodeAsync();
    }
}