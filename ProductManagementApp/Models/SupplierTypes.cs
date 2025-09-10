using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementApp.Models        
{
    [Table("SupplierTypes")]
    public class SupplierTypes 
    {
        public int Id { get; set; }
        public string SupplierTypeCode { get; set; } = "";
        public string SupplierTypeName { get; set; } = "";
    }
}
