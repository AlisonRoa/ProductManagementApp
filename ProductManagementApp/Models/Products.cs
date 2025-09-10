using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementApp.Models
{
    [Table("Products")]
    public class Products
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal PricePerUnit { get; set; }
        public string BasicUnit { get; set; } = "";
        public int StatusId { get; set; }
        public int SuppliersId { get; set; }
    }
}
