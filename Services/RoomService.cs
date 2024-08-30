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
        public Room CreateRoom(string name, string number, string description, string fieldOfStudy, int capacity, List<AvailableDay> availableDays)
        {
            var room = new Room
            {
                RoomName = name,
                RoomNumber = number,
                Description = description,
                FieldOfStudy = fieldOfStudy,
                Capacity = capacity,
                //AdminId = adminId
            };

            App.DatabaseService.SaveRoom(room);

            // Save available days and slots
            foreach (var day in availableDays)
            {
                day.RoomId = room.Id; // Link the day to the room
                App.DatabaseService.SaveAvailableDay(day);

                foreach (var slot in day.Slots)
                {
                    slot.AvailableDayId = day.Id;
                    App.DatabaseService.SaveRoomSlot(slot);
                }
            }

            return room;
        }

        // Get all rooms from the database
        public List<Room> GetAllRooms()
        {
            return _databaseService.GetAllRooms();
        }
    }
}
