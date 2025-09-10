using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementApp.DTO
{
    /// <summary>
    /// Item para el combo de Estados (viene de PM.StatusCatalog)
    /// [DTO] - Data Transfer Object NO ES UNA ENTIDAD DE BASE DE DATOS
    /// </summary>
    public class StatusItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
