using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    class BookingManager
    {

        // Book a room slot
        public void BookRoom(Room room, RoomSlot newSlot, int userId)
        {
            var slot = App.DatabaseService.GetRoomSlots(room.Id).Find(s => s.StartTime == newSlot.StartTime && s.EndTime == newSlot.EndTime);

            if (slot != null && slot.BookedCount < room.Capacity)
            {
                slot.BookedUserIds.Add(userId);
                slot.BookedCount++;
                App.DatabaseService.SaveRoomSlot(slot); // Update slot in the database
            }
            else if (slot == null)
            {
                newSlot.BookedUserIds.Add(userId);
                newSlot.BookedCount = 1;
                App.DatabaseService.SaveRoomSlot(newSlot); // Save new slot in the database
            }
            else
            {
                throw new Exception("Room slot is fully booked.");
            }
        }

        // Get available room slots
        public List<RoomSlot> GetAvailableRoomSlots(Room room)
        {
            var allSlots = App.DatabaseService.GetRoomSlots(room.Id);
            var availableSlots = new List<RoomSlot>();

            foreach (var slot in allSlots)
            {
                // Check if the slot is available (not fully booked)
                if (slot.BookedCount < room.Capacity)
                {
                    availableSlots.Add(slot);
                }
            }

            return availableSlots;
        }
    }
}
