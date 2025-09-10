using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductManagementApp.Services
{
    public interface IProductsService
    {
        // Para productos y estados
        Task<IReadOnlyList<ProductListItem>> GetProductsAsync();
        Task<IReadOnlyList<StatusItem>> GetStatusesAsync();

        // Para opciones
        Task<IList<OptionItem>> GetOptionsByProductAsync(int productId);
        Task<OptionItem> SaveOptionAsync(OptionItem option);
    }
}
