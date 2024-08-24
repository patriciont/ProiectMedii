using BookingApp.Services;
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
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db3");
            DatabaseService = new DatabaseService(dbPath);

            MainPage = new NavigationPage(new MainPage());
        }
    }
}
