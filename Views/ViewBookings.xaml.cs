using System.Collections.ObjectModel;
using BookingApp.Models;
using static BookingApp.Models.User;

namespace BookingApp.Views;

public partial class ViewBookings : ContentPage
{
    public ObservableCollection<Booking> UserBookings { get; set; }

    public ViewBookings()
	{
		InitializeComponent();

        UserBookings = new ObservableCollection<Booking>();

        LoadUserBookings();
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
    }
}