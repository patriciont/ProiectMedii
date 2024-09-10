namespace BookingApp.Views;
using BookingApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static BookingApp.Models.User;

public partial class BookingDetails : ContentPage
{
    // Objects
    private Room _selectedRoom;
    private AvailableDay _selectedDay;
    private BookingManager _bookingManager;

    // Lists
    public ObservableCollection<AvailableDay> AvailableDays { get; set; }

    // View available days variables
    private DateTime _currentStartDate;
    private const int DaysPerPage = 7;

    public BookingDetails(Room selectedRoom)
	{
		InitializeComponent();

        _selectedRoom = selectedRoom;
        RoomNameLabel.Text = $"Room: {_selectedRoom.RoomName}";

        _bookingManager = new BookingManager();

        _currentStartDate = DateTime.Today;

        // Load the first 7 days
        LoadAvailableDays(_currentStartDate);
    }

    // LOAD

    // Load Days
    private void LoadAvailableDays(DateTime startDate)
    {
        var availableDays = App.DatabaseService.GetAvailableDays(_selectedRoom.Id);

        // Filter available days
        var upcomingDays = availableDays.Where(day => day.Date >= startDate && day.Date <= DateTime.Today.AddDays(DaysPerPage)).OrderBy(day => day.Date).ToList();

        // Set AvailableDays and update CollectionView
        _selectedRoom.AvailableDays = upcomingDays;
        DaysCollectionView.ItemsSource = _selectedRoom.AvailableDays;

        DateRangeLabel.Text = $"{startDate:MMM d} - {startDate.AddDays(DaysPerPage - 1):MMM d}";

        _currentStartDate = startDate;
    }

    private void OnNextPageClicked(object sender, EventArgs e)
    {
        var nextStartDate = _currentStartDate.AddDays(DaysPerPage);
        LoadAvailableDays(nextStartDate);
    }

    private void OnPreviousPageClicked(object sender, EventArgs e)
    {
        var previousStartDate = _currentStartDate.AddDays(-DaysPerPage);
        if (previousStartDate >= DateTime.Today)
        {
            LoadAvailableDays(previousStartDate);
        }
        else
        {
            LoadAvailableDays(DateTime.Today);
        }
    }

    // Load Slots
    private void LoadSlotsForSelectedDay(int availableDayId)
    {
        var slots = App.DatabaseService.GetRoomSlots(availableDayId);
        SlotsCollectionView.ItemsSource = slots;
    }

    // TRIGGERS

    // Day seleceted
    private async void OnDaySelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedDay = e.CurrentSelection.FirstOrDefault() as AvailableDay;

        LoadSlotsForSelectedDay(selectedDay.Id);
    }

    // Book button
    private async void OnBookButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var selectedTimeSlot = (RoomSlot)button.BindingContext;
        var selectedRoom = App.DatabaseService.GetRoom(selectedTimeSlot.RoomId);
        var selectedDay = App.DatabaseService.GetAvailableDay(selectedTimeSlot.AvailableDayId);

        // Attempt to book the selected slot
        bool isBooked = _bookingManager.BookRoomSlot(CurrentUser.LoggedInUser.Id, selectedTimeSlot.Id);

        if (isBooked)
        {
            ShowNotification($"You have successfully booked: {selectedRoom.RoomName}, on the {selectedDay.Date} from {selectedTimeSlot.StartTime} to {selectedTimeSlot.EndTime}.");

            LoadSlotsForSelectedDay(selectedTimeSlot.AvailableDayId);
        }
        else
        {
            ShowNotification($"Booking Failed for {selectedRoom.RoomName} on {selectedDay.Date}. Check your bookings for overlaps or contact staff.");
        }
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new BookingPage(CurrentUser.LoggedInUser.FieldOfStudy);
    }

    // NOTIFICATION BAR

    private async void ShowNotification(string message)
    {
        NotificationLabel.Text = message;
        NotificationFrame.IsVisible = true;

        await Task.Delay(5000);

        NotificationFrame.IsVisible = false;
    }

    private void CloseNotificationClicked(object sender, EventArgs e)
    {
        NotificationFrame.IsVisible = false;
    }
}