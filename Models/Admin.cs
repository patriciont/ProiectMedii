using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace BookingApp.Models
{
    public class Admin
    {

        [PrimaryKey, AutoIncrement]

        // Unique ID
        public int Id { get; set; }

        // Name

        [SQLite.MaxLength(100)]
        public string? Name { get; set; } 

        // Email

        [SQLite.MaxLength(100)]
        public string? Email { get; set; }  

        // Phone Number

        [SQLite.MaxLength(100)]
        public string? PhoneNumber { get; set; }  
    }
}
