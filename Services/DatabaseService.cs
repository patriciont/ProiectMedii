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
            _database.CreateTable<RoomSlot>();
        }

        // Room methods
        public int SaveRoom(Room room) => _database.Insert(room);
        public Room GetRoom(int id) => _database.Table<Room>().FirstOrDefault(r => r.Id == id);
        public List<Room> GetAllRooms() => _database.Table<Room>().ToList();


        // RoomSlot methods
        public int SaveRoomSlot(RoomSlot slot) => _database.Insert(slot);
        public RoomSlot GetRoomSlot(int id) => _database.Table<RoomSlot>().FirstOrDefault(s => s.Id == id);
        public List<RoomSlot> GetRoomSlots(int roomId) => _database.Table<RoomSlot>().Where(s => s.RoomId == roomId).ToList();

        // Admin methods
        public int SaveAdmin(Admin admin) => _database.Insert(admin);
        public Admin GetAdmin(int id) => _database.Table<Admin>().FirstOrDefault(a => a.Id == id);

        // User methods
        public int SaveUser(User user) => _database.Insert(user);
        public User GetUser(int id) => _database.Table<User>().FirstOrDefault(u => u.Id == id);
        public List<User> GetAllUsers() => _database.Table<User>().ToList();


        // Methods for deleting users and rooms
        public int DeleteUser(int id)
        {
            return _database.Delete<User>(id);
        }

        public int DeleteRoom(int id)
        {
            return _database.Delete<Room>(id);
        }

        public void DeleteAllUsers() => _database.DeleteAll<User>();
        public void DeleteAllRooms() => _database.DeleteAll<Room>();
    }
}
