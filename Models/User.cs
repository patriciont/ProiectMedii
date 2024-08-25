using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace BookingApp.Models
{
    public class User
    {

        [PrimaryKey, AutoIncrement]

        // Unique ID
        public int Id { get; set; }

        // Username

        [SQLite.MaxLength(100)]
        public string? Username { get; set; }

        // Email

        [SQLite.MaxLength(100)]
        public string? Email { get; set; }

        // Password

        [SQLite.MaxLength(100)]
        public string? Password { get; set; }

        [SQLite.MaxLength(100)]
        public string? FieldOfStudy { get; set; }

        public int PermissionsLevel { get; set; }

        public static class CurrentUser
        {
            public static User LoggedInUser { get; set; }
        }
    }
}
