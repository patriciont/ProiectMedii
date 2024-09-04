using System.Collections.ObjectModel;
using BookingApp.Models;
using Plugin.Maui.Calendar.Models;
using static BookingApp.Models.User;

namespace BookingApp.Views;

public partial class ViewBookings : ContentPage
{
    public EventCollection Events { get; set; }

    public ObservableCollection<Booking> UserBookings { get; set; }

    public ViewBookings()
	{
		InitializeComponent();

        UserBookings = new ObservableCollection<Booking>();

        LoadUserBookings();
        LoadEvents();
        BindingContext = this;
    }

    private void LoadUserBookings()
    {
        var bookings = App.DatabaseService.GetBookingsForUser(CurrentUser.LoggedInUser.Id);
        foreach (var booking in bookings)
        {
            UserBookings.Add(booking);
        }
    }

    private void LoadEvents()
    {
        Events = new EventCollection();

        foreach (var booking in UserBookings)
        {
            var eventDate = booking.BookingDate.Date;

            if (!Events.ContainsKey(eventDate))
            {
                Events[eventDate] = new List<CalendarEvent>();
            }

            // Convert the booking to a CalendarEvent and add it to the collection
            ((List<CalendarEvent>)Events[eventDate]).Add(ConvertBookingToEvent(booking));
        }
    }

    private CalendarEvent ConvertBookingToEvent(Booking booking)
    {
        var room = App.DatabaseService.GetRoom(booking.RoomId);

        DateTime bookingDate = booking.BookingDate.Date; // Assuming you have a BookingDate property

        return new CalendarEvent
        {
            StartTime = bookingDate.Add(booking.BookingStart), // Combine DateTime and TimeSpan
            EndTime = bookingDate.Add(booking.BookingEnd),     // Combine DateTime and TimeSpan
            Name = room.RoomName,
            Description = $"Booking in {room.RoomName} from {booking.BookingStart} to {booking.BookingEnd}"
        };
    }

    private void CancelBookingClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var bookingId = (int)button.CommandParameter;

        CancelBooking(bookingId);
    }
    private void CancelBooking(int bookingId)
    {
        App.DatabaseService.DeleteBooking(bookingId);

        // Remove the booking from the collection
        var bookingToRemove = UserBookings.FirstOrDefault(b => b.Id == bookingId);
        if (bookingToRemove != null)
        {
            UserBookings.Remove(bookingToRemove);
        }

        BookingPicker.SelectedItem = null;
        BookingDetails.IsVisible = false;
        //Update the events in calender somehow

        DisplayAlert("Booking Canceled", "The booking has been successfully canceled.", "OK");
    }

    private void BookingPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        if (picker.SelectedItem is Booking selectedBooking)
        {
            RoomNameLabel.Text = App.DatabaseService.GetRoom(selectedBooking.RoomId)?.RoomName;
            BookingDateLabel.Text = selectedBooking.BookingDate.ToString("MM/dd/yyyy");
            StartTimeLabel.Text = selectedBooking.BookingStart.ToString(@"hh\:mm");
            EndTimeLabel.Text = selectedBooking.BookingEnd.ToString(@"hh\:mm");

            BookingDetails.BindingContext = selectedBooking;
            BookingDetails.IsVisible = true;
        }
    }

    private void UnloadedHandler(object sender, EventArgs e)
    {
        ProfCal.Dispose();
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new ManageBookings();
    }
}