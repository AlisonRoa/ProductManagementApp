using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementApp.Models
{
    [Table ("Suppliers")]
    public class Suppliers
    {
        public int Id { get; set; }
        public string SupplierCode { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public string TelephoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public int StatusId { get; set; }
        public int SupplierTypesId { get; set; }
    }
}
