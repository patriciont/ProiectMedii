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

        // Create a room with no slots (slots will be added later)
        public void CreateRoom(string name, string number, string description, string fieldOfStudy, int capacity, List<AvailableDay> availableDays)
        {
            var room = new Room
            {
                RoomName = name,
                RoomNumber = number,
                Description = description,
                FieldOfStudy = fieldOfStudy,
                Capacity = capacity
            };

            // Save room
            SaveRoom(room);

            // Save available days and slots
            foreach (var day in availableDays)
            {
                day.RoomId = room.Id; // Link the day to the room
                _databaseService.SaveAvailableDay(day);

                foreach (var slot in day.Slots)
                {
                    slot.AvailableDayId = day.Id; // Link the slot to the available day
                    _databaseService.SaveRoomSlot(slot);
                }
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
