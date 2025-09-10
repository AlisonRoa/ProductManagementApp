using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ProductManagementApp.Repositories
{
    public interface IProductsRepository
    {
        Task<List<ProductListItem>> GetProductsAsync();
        Task<List<StatusItem>> GetStatusesAsync();
    }
}
