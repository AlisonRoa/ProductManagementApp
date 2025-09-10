using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementApp.Models
{
    /// <summary>
    /// Proyección para el listado (lo que muestra el DataGrid)
    /// [DTO] - Data Transfer Object NO ES UNA ENTIDAD DE BASE DE DATOS
    /// </summary>
    public class ProductListItem
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string StatusName { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public decimal InStock { get; set; }
        public decimal PricePerUnit { get; set; }
        public string BasicUnit { get; set; } = "";
        public DateTime CreatedDate { get; set; }
    }
}
