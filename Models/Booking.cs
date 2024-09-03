using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    public class Booking
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public TimeSpan BookingStart { get; set; }
        public TimeSpan BookingEnd { get; set; }


        public int UserId { get; set; }
        public int RoomId { get; set; }
        public int AvailableDayId { get; set; }
        public int RoomSlotId { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
