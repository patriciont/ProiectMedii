using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    class RoomSlot
    {
        [PrimaryKey, AutoIncrement]

        // Unique ID
        public int Id { get; set; }

        // Foreign key to Room
        public int RoomId { get; set; } 

        // Start time
        public DateTime StartTime { get; set; }  

        // End time
        public DateTime EndTime { get; set; }


        [Ignore] // Aplication logic

        // List of user IDs who have booked this slot
        public List<int> BookedUserIds { get; set; } = new List<int>();

        // The current number of bookings for this slot
        public int BookedCount { get; set; } 
    }
}
