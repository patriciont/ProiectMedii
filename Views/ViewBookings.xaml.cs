using System.Collections.ObjectModel;
using BookingApp.Models;
using Plugin.Maui.Calendar.Models;
using static BookingApp.Models.User;

namespace BookingApp.Views;

public partial class ViewBookings : ContentPage
{
    public EventCollection Events { get; set; }

    public ObservableCollection<Booking> UserBookings { get; set; }
    public ObservableCollection<Booking> SelectedDateBookings { get; set; }

    private DateTime _selectedDate;
    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate != value)
            {
                _selectedDate = value;
                OnSelectedDateChanged();
            }
        }
    }

    public ViewBookings()
	{
		InitializeComponent();

        UserBookings = new ObservableCollection<Booking>();
        SelectedDateBookings = new ObservableCollection<Booking>();

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

    private void OnSelectedDateChanged()
    {
        // Clear the previous date's bookings
        SelectedDateBookings.Clear();

        // Find all bookings that match the selected date
        var matchingBookings = UserBookings.Where(b => b.BookingDate.Date == SelectedDate).ToList();

        // Add the bookings to the observable collection for the selected date
        foreach (var booking in matchingBookings)
        {
            var room = App.DatabaseService.GetRoom(booking.RoomId);
            booking.RoomName = room.RoomName; 

            SelectedDateBookings.Add(booking);
        }

        // Show the booking details frame if there are matching bookings
        BookingDetails.IsVisible = SelectedDateBookings.Any();
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

            // Remove event for the canceled booking
            var eventDate = bookingToRemove.BookingDate.Date;
            if (Events.ContainsKey(eventDate))
            {
                var eventsOnDate = (List<CalendarEvent>)Events[eventDate];
                var eventToRemove = eventsOnDate.FirstOrDefault(e => e.StartTime == bookingToRemove.BookingDate.Add(bookingToRemove.BookingStart));
                if (eventToRemove != null)
                {
                    eventsOnDate.Remove(eventToRemove);
                    if (eventsOnDate.Count == 0)
                    {
                        Events.Remove(eventDate);
                    }
                }
            }
        }

        BookingDetails.IsVisible = false;
        LoadEvents();  

        DisplayAlert("Booking Canceled", "The booking has been successfully canceled.", "OK");
    }

    private void UnloadedHandler(object sender, EventArgs e)
    {
        ProfCal.Dispose();
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new OpeningPage();
    }
}