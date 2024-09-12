using System.Collections.ObjectModel;
using BookingApp.Models;

namespace BookingApp.Views;

public partial class AdminOverviewPage : ContentPage
{

    private List<Booking> allBookings;
    private List<Booking> filteredBookings;

    public AdminOverviewPage()
	{
		InitializeComponent();

        LoadBookings();
    }

    private void LoadBookings()
    {
        allBookings = App.DatabaseService.GetAllBookings(); // Fetch all bookings from DB
        filteredBookings = new List<Booking>(allBookings);
        BookingsList.ItemsSource = filteredBookings; // Show all bookings by default
    }

    private void OnRoomFilterChanged(object sender, EventArgs e)
    {
        var selectedRoom = RoomPicker.SelectedItem as Room;
        if (selectedRoom != null)
        {
            filteredBookings = allBookings.Where(b => b.RoomId == selectedRoom.Id).ToList();
        }
        else
        {
            filteredBookings = new List<Booking>(allBookings); // Show all if no room selected
        }
        BookingsList.ItemsSource = filteredBookings;
    }

    // Filter by date
    private void OnDateFilterChanged(object sender, DateChangedEventArgs e)
    {
        DateTime selectedDate = e.NewDate;
        filteredBookings = allBookings.Where(b => b.BookingDate.Date == selectedDate.Date).ToList();
        BookingsList.ItemsSource = filteredBookings;
    }

    // Filter by user
    private void OnUserFilterChanged(object sender, EventArgs e)
    {
        var selectedUser = UserPicker.SelectedItem as User;
        if (selectedUser != null)
        {
            filteredBookings = allBookings.Where(b => b.UserId == selectedUser.Id).ToList();
        }
        else
        {
            filteredBookings = new List<Booking>(allBookings); // Show all if no user selected
        }
        BookingsList.ItemsSource = filteredBookings;
    }

    // Clear filters and show all bookings
    private void ResetFilters()
    {
        filteredBookings = new List<Booking>(allBookings);
        BookingsList.ItemsSource = filteredBookings;
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
        var profilePage = new ManageBookings();
        Application.Current.MainPage = new NavigationPage(profilePage);
    }

}