using BookingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    internal class RoomService
    {

        private readonly DatabaseService _databaseService;

        public RoomService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Save a room to the database
        public void SaveRoom(Room room)
        {
            _databaseService.SaveRoom(room);
        }

        // Add available days and slots to a room
        public void AddAvailableDay(Room room, DateTime date, List<RoomSlot> slots)
        {
            var availableDay = new AvailableDay
            {
                RoomId = room.Id,
                Date = date,
                Slots = slots
            };

            room.AvailableDays ??= new List<AvailableDay>();
            room.AvailableDays.Add(availableDay);

            // Save the available day and its slots in the database
            _databaseService.SaveAvailableDay(availableDay);

            foreach (var slot in slots)
            {
                slot.AvailableDayId = availableDay.Id;
                _databaseService.SaveRoomSlot(slot);
            }
        }

        // Create a room with no slots (slots will be added later)
        public Room CreateRoom(string roomName, string roomNumber, string description, string fieldOfStudy, int capacity, int adminId, List<RoomSlot> roomSlots)
        {
            var room = new Room
            {
                RoomName = roomName.Trim(),
                RoomNumber = roomNumber.Trim(),
                Description = description?.Trim(),
                FieldOfStudy = fieldOfStudy,
                Capacity = capacity,
                AdminId = adminId,
                AvailableDays = new List<AvailableDay>()
            };

            SaveRoom(room);
            return room;
        }

        // Get all rooms from the database
        public List<Room> GetAllRooms()
        {
            return _databaseService.GetAllRooms();
        }
    }
}
