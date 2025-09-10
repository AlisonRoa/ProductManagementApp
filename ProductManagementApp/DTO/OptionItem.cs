using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementApp.DTO
{
    /// <summary>
    /// Proyección para un ítem de opción
    /// [DTO] - Data Transfer Object NO ES UNA ENTIDAD DE BASE DE DATOS
    /// </summary>
    public class OptionItem
    {
        public int Id { get; set; }
        public string OptionCode { get; set; } = "";
        public string OptionName { get; set; } = "";
        public int StatusId { get; set; }
        public string StatusName { get; set; } = "";
        public int ProductsId { get; set; }
    }
}
