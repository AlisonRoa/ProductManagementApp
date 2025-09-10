using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagementApp.Models
{
    [Table ("Stocks")]
    public class Stocks
    {
        public int Id { get; set; }
        public decimal InStock { get; set; }
        public DateTime LastUpdate { get; set; }
        public int ProductsId { get; set; }
    }
}
