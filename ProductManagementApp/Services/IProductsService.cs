using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductManagementApp.Services
{
    public interface IProductsService
    {
        Task<IReadOnlyList<ProductListItem>> GetProductsAsync();
        Task<IReadOnlyList<StatusItem>> GetStatusesAsync();
    }
}
