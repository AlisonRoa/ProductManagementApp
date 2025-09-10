using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementApp.Models
{
    [Table("StatusCatalog")]
    public class StatusCatalog
    {
        public int Id { get; set; }
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
    }
}

