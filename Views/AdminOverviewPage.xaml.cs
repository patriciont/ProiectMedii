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
        LoadPickers();
    }

    private void LoadBookings()
    {
        allBookings = App.DatabaseService.GetAllBookings(); 
        filteredBookings = new List<Booking>(allBookings);
        BookingsList.ItemsSource = filteredBookings;
    }

    private void LoadPickers()
    {
        var rooms = App.DatabaseService.GetAllRooms(); 
        var users = App.DatabaseService.GetAllUsers();

        // Set the ItemsSource for the pickers
        RoomPicker.ItemsSource = rooms;
        UserPicker.ItemsSource = users;
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
            filteredBookings = new List<Booking>(allBookings); 
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

    // Show all bookings (reset filters)
    private void OnShowAllClicked(object sender, EventArgs e)
    {
        RoomPicker.SelectedItem = null; // Clear room picker
        DateFilter.Date = DateTime.Today; // Reset date picker
        UserPicker.SelectedItem = null; // Clear user picker

        ResetFilters();
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