using BookingApp.Services;
using System.Diagnostics;
using System.IO;

namespace BookingApp
{
    public partial class App : Application
    {

        public static DatabaseService? DatabaseService { get; private set; }

        public App()
        {
            InitializeComponent();

            // Set up the path for the SQLite database
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bookingappdb.db3");

            DatabaseService = new DatabaseService(dbPath);

            CheckDatabaseContent();

            MainPage = new NavigationPage(new MainPage());
        }

        public static void PrintDatabasePath()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bookingappdb.db3");
            Debug.WriteLine($"Database path: {dbPath}");
        }

        public void CheckDatabaseContent()
        {
            if (DatabaseService != null)
            {
                var rooms = DatabaseService.GetAllRooms();
                var availableDays = DatabaseService.GetAvailableDays(1); 
                var roomSlots = DatabaseService.GetRoomSlots(1); 

                Debug.WriteLine($"Rooms count: {rooms.Count}");
                Debug.WriteLine($"AvailableDays count: {availableDays.Count}");
                Debug.WriteLine($"RoomSlots count: {roomSlots.Count}");
            }
            else
            {
                Debug.WriteLine("DatabaseService is not initialized.");
            }
        }
    }
}
