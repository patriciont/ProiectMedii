using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    public class RoomSlot
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int RoomId { get; set; } // Foreign key

        public int AvailableDayId { get; set; } // Foreign key 

        [Ignore]  // Application logic
        public List<int> BookedUserIds { get; set; } = new List<int>();

        public int BookedCount { get; set; }
        public int Capacity { get; set; }

        public string SlotDetails => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";


    }
}
