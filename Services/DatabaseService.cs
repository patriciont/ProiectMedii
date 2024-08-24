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

        // Methods for saving and retrieving data
        public int SaveRoom(Room room) => _database.Insert(room);
        public Room GetRoom(int id) => _database.Table<Room>().FirstOrDefault(r => r.Id == id);
        public List<Room> GetAllRooms() => _database.Table<Room>().ToList();
        public int SaveRoomSlot(RoomSlot slot) => _database.Insert(slot);
        public RoomSlot GetRoomSlot(int id) => _database.Table<RoomSlot>().FirstOrDefault(s => s.Id == id);
        public List<RoomSlot> GetRoomSlots(int roomId) => _database.Table<RoomSlot>().Where(s => s.RoomId == roomId).ToList();
        public int SaveAdmin(Admin admin) => _database.Insert(admin);
        public Admin GetAdmin(int id) => _database.Table<Admin>().FirstOrDefault(a => a.Id == id);
    }
}
