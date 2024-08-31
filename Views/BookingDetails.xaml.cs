namespace BookingApp.Views;
using BookingApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

public partial class BookingDetails : ContentPage
{
    private Room _selectedRoom;
    private AvailableDay _selectedDay;

    public ObservableCollection<AvailableDay> AvailableDays { get; set; }

    public BookingDetails(Room selectedRoom)
	{
		InitializeComponent();

        _selectedRoom = selectedRoom;
        RoomNameLabel.Text = $"Room: {_selectedRoom.RoomName}";

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
    private void OnBookButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var selectedTimeSlot = (RoomSlot)button.BindingContext;

        // Logic to create the booking
        
    }
}