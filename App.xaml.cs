using BookingApp.Services;
using BookingApp.Models;
using BookingApp.Views;
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

            Application.Current.UserAppTheme = AppTheme.Light;

            // Set up the path for the SQLite database
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bookingappdb.db3");

            DatabaseService = new DatabaseService(dbPath);

            CheckAdmin();

            var cleanupService = new CleanupService();
            Task.Run(() => cleanupService.CleanupExpiredData());

            MainPage = new NavigationPage(new LoginPage());
        }

        public void CheckAdmin()
        {
            var adminUser = DatabaseService.GetUserByUsername("admin");

            if (adminUser == null)
            {
                var newAdminUser = new User
                {
                    Username = "admin",
                    Password = "admin", 
                    Email = "admin@outlook.com",
                    FieldOfStudy = "ADMIN",
                    PermissionsLevel = 1
                };

                DatabaseService.SaveUser(newAdminUser);
            }
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
