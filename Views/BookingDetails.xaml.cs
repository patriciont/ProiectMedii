namespace BookingApp.Views;
using BookingApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using static BookingApp.Models.User;

public partial class BookingDetails : ContentPage
{
    private Room _selectedRoom;
    private AvailableDay _selectedDay;
    private BookingManager _bookingManager;

    public ObservableCollection<AvailableDay> AvailableDays { get; set; }

    public BookingDetails(Room selectedRoom)
	{
		InitializeComponent();

        _selectedRoom = selectedRoom;
        RoomNameLabel.Text = $"Room: {_selectedRoom.RoomName}";

        _bookingManager = new BookingManager();

        LoadAvailableDays();
    }

    // LOAD

    // Load Days
    private void LoadAvailableDays()
    {
        var availableDays = App.DatabaseService.GetAvailableDays(_selectedRoom.Id);

        // Debug output for available days
        foreach (var day in availableDays)
        {
            Debug.WriteLine($"Available Day - Date: {day.Date}");
        }

        // Set AvailableDays and update CollectionView
        _selectedRoom.AvailableDays = availableDays;
        DaysCollectionView.ItemsSource = _selectedRoom.AvailableDays;
    }

    // Load Slots
    private void LoadSlotsForSelectedDay(int availableDayId)
    {
        var slots = App.DatabaseService.GetRoomSlots(availableDayId);

        // Log the retrieved slots
        Debug.WriteLine($"Loading slots for Day ID: {availableDayId}");
        foreach (var slot in slots)
        {
            Debug.WriteLine($"Slot - Start: {slot.StartTime}, End: {slot.EndTime}");
        }

        // Update UI with slots
        SlotsCollectionView.ItemsSource = slots;
    }

    // TRIGGERS

    // Day seleceted
    private async void OnDaySelected(object sender, SelectionChangedEventArgs e)
    {
        Debug.WriteLine("OnDaySelected triggered");

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
            await DisplayAlert("Booking Confirmed", $"You have successfully booked: {selectedRoom.RoomName}, on the {selectedDay.Date} from {selectedTimeSlot.StartTime} to {selectedTimeSlot.EndTime}.", "OK");

            LoadSlotsForSelectedDay(selectedTimeSlot.AvailableDayId);
        }
        else
        {
            await DisplayAlert("Booking Failed", "This timeslot is fully booked. Please select another timeslot.", "OK");
        }
    }
}