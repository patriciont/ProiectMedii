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
        public async Task<int> SaveRoomSlot(RoomSlot slot)
        {
            return await Task.Run(() => _database.Insert(slot));
        }

        public RoomSlot GetRoomSlot(int id) => _database.Table<RoomSlot>().FirstOrDefault(s => s.Id == id);
        public List<RoomSlot> GetRoomSlots(int availableDayId) => _database.Table<RoomSlot>().Where(s => s.AvailableDayId == availableDayId).ToList();

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

        // Methods for deleting users and rooms
        public int DeleteUser(int id) => _database.Delete<User>(id);
        public int DeleteRoom(int id) => _database.Delete<Room>(id);
        public void DeleteAllUsers() => _database.DeleteAll<User>();
        public void DeleteAllRooms() => _database.DeleteAll<Room>();
    }
}
