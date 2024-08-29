using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    public class AvailableDay
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int RoomId { get; set; }  // Foreign key to Room

        public DateTime Date { get; set; }  // Date of availability


        [Ignore]  // Application logic
        public List<RoomSlot>? Slots { get; set; }  // Slots available on this day

        public bool IsSelected { get; set; }
    }
}
