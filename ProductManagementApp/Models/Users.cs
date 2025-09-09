using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementApp.Models
{
    namespace ProductManagementApp.Models
    {
        public class User
        {
            public int Id { get; set; }              
            public string Username { get; set; } = "";
            public string PasswordHash { get; set; } = "";
            public string Names { get; set; } = "";
            public string LastNames { get; set; } = "";
            public string TelephoneNumber { get; set; } = "";
            public string Email { get; set; } = "";
            public System.DateTime CreationDate { get; set; }   
        }
    }

}
