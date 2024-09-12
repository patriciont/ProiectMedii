using BookingApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Admin>();
            _database.CreateTable<User>();
            _database.CreateTable<Room>();
            _database.CreateTable<AvailableDay>();
            _database.CreateTable<RoomSlot>();
            _database.CreateTable<Booking>();
        }

        // Room methods
        public int SaveRoom(Room room) => _database.Insert(room);
        public Room GetRoom(int id) => _database.Table<Room>().FirstOrDefault(r => r.Id == id);
        public List<Room> GetAllRooms() => _database.Table<Room>().ToList();

        public List<Room> GetRoomsByFieldOfStudy(string fieldOfStudy)
        {
            return _database.Table<Room>().Where(r => r.FieldOfStudy == fieldOfStudy || r.FieldOfStudy == "Commons").ToList();
        }

        // AvailableDay methods
        public int SaveAvailableDay(AvailableDay availableDay) => _database.Insert(availableDay);
        public AvailableDay GetAvailableDay(int id)
        {
            var availableDay = _database.Table<AvailableDay>().FirstOrDefault(d => d.Id == id);
            if (availableDay != null)
            {
                availableDay.Slots = GetRoomSlots(availableDay.Id); // Load slots for the available day
            }
            return availableDay;
        }

        public List<AvailableDay> GetAvailableDays(int roomId) => _database.Table<AvailableDay>().Where(d => d.RoomId == roomId).ToList();

        public List<AvailableDay> GetAvailableDaysWithin(int roomId, DateTime startDate, DateTime endDate)
        {
            return _database.Table<AvailableDay>()
                            .Where(d => d.RoomId == roomId && d.Date >= startDate && d.Date <= endDate)
                            .ToList();
        }

        // RoomSlot methods

        // Save
        public async Task<int> SaveRoomSlot(RoomSlot slot)
        {
            return await Task.Run(() => _database.Insert(slot));
        }

        // Update
        public int UpdateRoomSlot(RoomSlot roomSlot)
        {
            return _database.Update(roomSlot);
        }

        // Get room slot by id
        public RoomSlot GetRoomSlot(int id) => _database.Table<RoomSlot>().FirstOrDefault(s => s.Id == id);
        // get all slots for available day
        public List<RoomSlot> GetRoomSlots(int availableDayId) => _database.Table<RoomSlot>().Where(s => s.AvailableDayId == availableDayId).ToList();

        // Get roomslot by room id
        public IEnumerable<RoomSlot> GetRoomSlotsByRoomId(int roomId)
        {
            return _database.Table<RoomSlot>().Where(slot => slot.RoomId == roomId).ToList();
        }

        // Booking methods

        public int SaveBooking(Booking booking)
        {
            if (booking.Id != 0)
            {
                return _database.Update(booking);
            }
            else
            {
                return _database.Insert(booking);
            }
        }

        // Get a booking by ID
        public Booking GetBooking(int id)
        {
            return _database.Table<Booking>().FirstOrDefault(b => b.Id == id);
        }

        public List<Booking> GetUserBookingsByRoomId(int roomId)
        {
            // Query the database for all user bookings with the matching RoomId
            return _database.Table<Booking>().Where(b => b.RoomId == roomId).ToList();
        }

        // Get all bookings for a specific user
        public List<Booking> GetBookingsForUser(int userId)
        {
            return _database.Table<Booking>().Where(b => b.UserId == userId).ToList();
        }

        public List<Booking> GetAllBookings() => _database.Table<Booking>().ToList();


        // Get all bookings for a specific room slot
        public List<Booking> GetBookingsForRoomSlot(int roomSlotId)
        {
            return _database.Table<Booking>().Where(b => b.RoomSlotId == roomSlotId).ToList();
        }

        // Delete a booking
        public int DeleteBooking(int id)
        {
            return _database.Delete<Booking>(id);
        }

        // Admin methods
        public int SaveAdmin(Admin admin) => _database.Insert(admin);
        public Admin GetAdmin(int id) => _database.Table<Admin>().FirstOrDefault(a => a.Id == id);

        // User methods
        public int SaveUser(User user) => _database.Insert(user);
        public User GetUser(int id) => _database.Table<User>().FirstOrDefault(u => u.Id == id);
        public List<User> GetAllUsers() => _database.Table<User>().ToList();

        // Get a user by username
        public User GetUserByUsername(string username)
        {
            return _database.Table<User>().FirstOrDefault(u => u.Username == username);
        }

        // Methods for deleting users, rooms, slots and days
        public int DeleteUser(int id) => _database.Delete<User>(id);
        public int DeleteRoom(int id) => _database.Delete<Room>(id);
        public void DeleteAllUsers() => _database.DeleteAll<User>();
        public void DeleteAllRooms() => _database.DeleteAll<Room>();

        public void DeleteRoomSlot(int roomSlotId)
        {
            var roomSlot = _database.Table<RoomSlot>().FirstOrDefault(rs => rs.Id == roomSlotId);
            if (roomSlot != null)
            {
                _database.Delete(roomSlot);
            }
        }

        public void DeleteAvailableDay(int availableDayId)
        {
            var availableDay = _database.Table<AvailableDay>().FirstOrDefault(ad => ad.Id == availableDayId);
            if (availableDay != null)
            {
                _database.Delete(availableDay);
            }
        }

        // CLEAN UP METHODS 

        // Fetch past bookings
        public List<Booking> GetPastBookings(DateTime cutoffDate)
        {
            return _database.Table<Booking>().Where(b => b.BookingDate < cutoffDate).ToList();
        }

        // Save BookingHistory
        public int SaveBookingHistory(BookingHistory bookingHistory)
        {
            return _database.Insert(bookingHistory);
        }

        // Delete all available days before a certain date
        public void DeleteAvailableDaysBefore(DateTime date)
        {
            var daysToDelete = _database.Table<AvailableDay>().Where(d => d.Date < date).ToList();
            foreach (var day in daysToDelete)
            {
                DeleteAvailableDay(day.Id);

                // Also delete associated RoomSlots
                var slotsToDelete = _database.Table<RoomSlot>().Where(s => s.AvailableDayId == day.Id).ToList();
                foreach (var slot in slotsToDelete)
                {
                    DeleteRoomSlot(slot.Id);
                }
            }
        }
    }
}
