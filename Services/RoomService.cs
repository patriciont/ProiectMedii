using BookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class RoomService
    {

        private readonly DatabaseService _databaseService;

        public RoomService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public bool BookRoomSlot(int userId, int roomSlotId)
        {
            var roomSlot = App.DatabaseService.GetRoomSlot(roomSlotId);

            if (roomSlot.BookedCount < roomSlot.Capacity)
            {
                roomSlot.BookedCount += 1;
                App.DatabaseService.UpdateRoomSlot(roomSlot);

                var booking = new Booking
                {
                    UserId = userId,
                    RoomSlotId = roomSlotId,
                    // Add any other relevant properties
                };
                App.DatabaseService.SaveBooking(booking);

                return true;
            }

            return false;  // Slot is fully booked
        }

        // Save a room to the database
        public void SaveRoom(Room room)
        {
            try
            {
                // Save the room and get the generated ID
                var roomId = _databaseService.SaveRoom(room);

                // Ensure room ID is properly assigned
                if (roomId > 0)
                {
                    room.Id = roomId; // Update room with the generated ID

                    // Save available days and slots
                    if (room.AvailableDays != null)
                    {
                        foreach (var day in room.AvailableDays)
                        {
                            // Use AddAvailableDay to save each day and its slots
                            AddAvailableDay(room, day.Date, day.Slots);
                        }
                    }
                }
                else
                {
                    // Handle error: room ID not generated
                    Console.WriteLine("Failed to save room. Room ID is not generated.");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error saving room: {ex.Message}");
            }
        }

        // Add available days and slots to a room
        public void AddAvailableDay(Room room, DateTime date, List<RoomSlot> slots)
        {
            var availableDay = new AvailableDay
            {
                RoomId = room.Id,
                Date = date
                // Initialize the Slots list
            };

            // Save the available day and get the ID
            var availableDayId = _databaseService.SaveAvailableDay(availableDay);
            if (availableDayId > 0)
            {
                availableDay.Id = availableDayId;

                foreach (var slot in slots)
                {
                    slot.AvailableDayId = availableDay.Id; // Ensure this is set correctly
                    _databaseService.SaveRoomSlot(slot);
                }

                // Optionally, update the in-memory list of available days
                room.AvailableDays ??= new List<AvailableDay>();
                room.AvailableDays.Add(availableDay);
            }
        }

        // Get all rooms from the database
        public List<Room> GetAllRooms()
        {
            var rooms = _databaseService.GetAllRooms();

            foreach (var room in rooms)
            {
                room.AvailableDays = _databaseService.GetAvailableDays(room.Id);

                foreach (var day in room.AvailableDays)
                {
                    day.Slots = _databaseService.GetRoomSlots(day.Id);
                }
            }

            return rooms;
        }
    }
}
