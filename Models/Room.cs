using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace BookingApp.Models
{
    public class Room
    {
        [PrimaryKey, AutoIncrement]

        // Unique ID
        public int Id { get; set; }

        // Room number

        [SQLite.MaxLength(100)]
        public string? RoomNumber { get; set; }

        // Room name

        [SQLite.MaxLength(200)]
        public string? RoomName { get; set; }

        // Room capacity

        public int Capacity { get; set; }

        // Field of study 

        [SQLite.MaxLength(100)]
        public string? FieldOfStudy { get; set; }  

        // Description or disclaimer

        [SQLite.MaxLength(200)]
        public string? Description { get; set; }


        // Foreign key to Admin (Contact Person)
        //public int AdminId { get; set; }


        [Ignore] // application logic
        // Person responsible for the room
        public Admin ? ContactPerson { get; set; } 

        [Ignore]  // application logic
        // List of RoomSlots 
        public List<RoomSlot> ? AvailabilitySlots { get; set; }

        [Ignore]  // Application logic
        public List<AvailableDay>? AvailableDays { get; set; }

    }
}
