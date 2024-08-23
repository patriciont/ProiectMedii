using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Models
{
    class BookingManager
    {

        public bool IsRoomAvailable(Room room, DateTime startTime, DateTime endTime)
        {
            foreach (var slot in room.AvailabilitySlots)
            {
                if (slot.StartTime < endTime && startTime < slot.EndTime && slot.BookedCount >= room.Capacity)
                {
                    return false; // Slot is fully booked
                }
            }
            return true; // No conflict, room has available capacity
        }

        public void BookRoom(Room room, RoomSlot newSlot, int userId)
        {
            var slot = room.AvailabilitySlots.Find(s => s.StartTime == newSlot.StartTime && s.EndTime == newSlot.EndTime);

            if (slot != null && slot.BookedCount < room.Capacity)
            {
                slot.BookedUserIds.Add(userId);
                slot.BookedCount++;

                // Save the updated slot to the database
                // db.Update(slot);
            }
            else if (slot == null)
            {
                newSlot.BookedUserIds.Add(userId);
                newSlot.BookedCount = 1;
                room.AvailabilitySlots.Add(newSlot);

                // Save the new slot to the database
                // db.Insert(newSlot);
            }
            else
            {
                throw new Exception("Room slot is fully booked.");
            }
        }
    }
}
