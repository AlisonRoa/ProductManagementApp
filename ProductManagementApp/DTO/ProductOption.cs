using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementApp.Models
{
    /// <summary>
    /// Proyección para el listado (lo que muestra el DataGrid)
    /// [DTO] - Data Transfer Object NO ES UNA ENTIDAD DE BASE DE DATOS
    /// </summary>
    public class ProductOption
    {
        public int Id { get; set; }
        public string OptionCode { get; set; } = "";
        public string OptionName { get; set; } = "";
        public int StatusId { get; set; }
        public int ProductId { get; set; }
    }
}
