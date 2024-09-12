using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Models;

namespace BookingApp.Services
{
    public class CleanupService
    {
        public async Task CleanupExpiredData()
        {
            var today = DateTime.Now.Date;

            // Delete past available days and room slots
            await Task.Run(() => App.DatabaseService.DeleteAvailableDaysBefore(today));

            // Move past bookings to BookingHistory
            var pastBookings = App.DatabaseService.GetPastBookings(today);
            foreach (var booking in pastBookings)
            {
                var bookingHistory = new BookingHistory
                {
                    UserId = booking.UserId,
                    UserName = booking.UserName,
                    RoomId = booking.RoomId,
                    RoomName = booking.RoomName,
                    RoomNumber = booking.RoomNumber,
                    Description = booking.Description,
                    StartTime = booking.BookingStart,
                    EndTime = booking.BookingEnd,
                    BookingDate = booking.BookingDate,
                    CreatedAt = booking.CreatedAt,
                };

                // Save to BookingHistory and delete from Bookings table
                App.DatabaseService.SaveBookingHistory(bookingHistory);
                App.DatabaseService.DeleteBooking(booking.Id);
            }
        }
    }
}
