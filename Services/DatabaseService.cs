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

        #region Room Methods

        public int SaveRoom(Room room) => _database.Insert(room);
        public Room GetRoom(int id) => _database.Table<Room>().FirstOrDefault(r => r.Id == id);
        public List<Room> GetAllRooms() => _database.Table<Room>().ToList();

        public List<Room> GetRoomsByFieldOfStudy(string fieldOfStudy)
        {
            return _database.Table<Room>()
                            .Where(r => r.FieldOfStudy == fieldOfStudy || r.FieldOfStudy == "Commons")
                            .ToList();
        }

        public int DeleteRoom(int id) => _database.Delete<Room>(id);
        public void DeleteAllRooms() => _database.DeleteAll<Room>();

        #endregion

        #region AvailableDay Methods

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

        public void DeleteAvailableDay(int availableDayId)
        {
            var availableDay = _database.Table<AvailableDay>().FirstOrDefault(ad => ad.Id == availableDayId);
            if (availableDay != null)
            {
                _database.Delete(availableDay);
            }
        }

        public void DeleteAvailableDaysBefore(DateTime date)
        {
            var daysToDelete = _database.Table<AvailableDay>().Where(d => d.Date < date).ToList();
            foreach (var day in daysToDelete)
            {
                DeleteAvailableDay(day.Id);
                // Delete associated RoomSlots
                var slotsToDelete = _database.Table<RoomSlot>().Where(s => s.AvailableDayId == day.Id).ToList();
                foreach (var slot in slotsToDelete)
                {
                    DeleteRoomSlot(slot.Id);
                }
            }
        }

        #endregion

        #region RoomSlot Methods

        public async Task<int> SaveRoomSlot(RoomSlot slot)
        {
            return await Task.Run(() => _database.Insert(slot));
        }

        public int UpdateRoomSlot(RoomSlot roomSlot)
        {
            return _database.Update(roomSlot);
        }

        public RoomSlot GetRoomSlot(int id) => _database.Table<RoomSlot>().FirstOrDefault(s => s.Id == id);

        public List<RoomSlot> GetRoomSlots(int availableDayId) => _database.Table<RoomSlot>().Where(s => s.AvailableDayId == availableDayId).ToList();

        public IEnumerable<RoomSlot> GetRoomSlotsByRoomId(int roomId)
        {
            return _database.Table<RoomSlot>().Where(slot => slot.RoomId == roomId).ToList();
        }

        public void DeleteRoomSlot(int roomSlotId)
        {
            var roomSlot = _database.Table<RoomSlot>().FirstOrDefault(rs => rs.Id == roomSlotId);
            if (roomSlot != null)
            {
                _database.Delete(roomSlot);
            }
        }

        #endregion

        #region Booking Methods

        public int SaveBooking(Booking booking)
        {
            return booking.Id != 0 ? _database.Update(booking) : _database.Insert(booking);
        }

        public Booking GetBooking(int id) => _database.Table<Booking>().FirstOrDefault(b => b.Id == id);

        public List<Booking> GetUserBookingsByRoomId(int roomId)
        {
            return _database.Table<Booking>().Where(b => b.RoomId == roomId).ToList();
        }

        public List<Booking> GetBookingsForUser(int userId)
        {
            return _database.Table<Booking>().Where(b => b.UserId == userId).ToList();
        }

        public List<Booking> GetAllBookings() => _database.Table<Booking>().ToList();

        public List<Booking> GetBookingsForRoomSlot(int roomSlotId)
        {
            return _database.Table<Booking>().Where(b => b.RoomSlotId == roomSlotId).ToList();
        }

        public List<Booking> GetPastBookings(DateTime cutoffDate)
        {
            return _database.Table<Booking>().Where(b => b.BookingDate < cutoffDate).ToList();
        }

        public int DeleteBooking(int id) => _database.Delete<Booking>(id);

        #endregion

        #region BookingHistory Methods

        public int SaveBookingHistory(BookingHistory bookingHistory)
        {
            return _database.Insert(bookingHistory);
        }

        #endregion

        #region Admin Methods

        public int SaveAdmin(Admin admin) => _database.Insert(admin);
        public Admin GetAdmin(int id) => _database.Table<Admin>().FirstOrDefault(a => a.Id == id);

        #endregion

        #region User Methods

        public int SaveUser(User user) => _database.Insert(user);
        public User GetUser(int id) => _database.Table<User>().FirstOrDefault(u => u.Id == id);
        public List<User> GetAllUsers() => _database.Table<User>().ToList();

        public User GetUserByUsername(string username)
        {
            return _database.Table<User>().FirstOrDefault(u => u.Username == username);
        }

        public int UpdateUser(User user) => _database.Update(user);

        public int DeleteUser(int id) => _database.Delete<User>(id);
        public void DeleteAllUsers() => _database.DeleteAll<User>();

        #endregion
    }
}
