using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    class BookingManager
    {

        public bool BookRoomSlot(int userId, int roomSlotId)
        {

            var roomSlot = App.DatabaseService.GetRoomSlot(roomSlotId);
            if (roomSlot == null) { return false; }

            var room = App.DatabaseService.GetRoom(roomSlot.RoomId);
            var user = App.DatabaseService.GetUser(userId);

            var avalday = App.DatabaseService.GetAvailableDay(roomSlot.AvailableDayId);
            if (avalday == null) { return false; }

            var userBookings = App.DatabaseService.GetBookingsForUser(userId);

            var today = DateTime.Today.Date;

            var existingBooking = userBookings.FirstOrDefault(b => b.BookingDate.Date == avalday.Date);
            if (existingBooking != null) { return false; }

            if (roomSlot.BookedCount < roomSlot.Capacity)
            {
                roomSlot.BookedCount += 1;
                App.DatabaseService.UpdateRoomSlot(roomSlot);

                var booking = new Booking
                {
                    UserId = userId,
                    RoomId = roomSlot.RoomId,
                    AvailableDayId = avalday.Id,
                    RoomSlotId = roomSlotId,

                    BookingDate = avalday.Date,
                    CreatedAt = today,
                    BookingStart = roomSlot.StartTime,
                    BookingEnd = roomSlot.EndTime,

                    RoomName = room.RoomName,
                    RoomNumber = room.RoomNumber,
                    Description = room.Description,
                    UserName = user.Username
                };

                App.DatabaseService.SaveBooking(booking);

                return true;
            }

            return false;  // Slot is fully booked
        }
    }
}
