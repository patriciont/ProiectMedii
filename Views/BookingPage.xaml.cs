using BookingApp.Models;
using BookingApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Views
{
    public partial class BookingPage : ContentPage
    {
        private string _userFieldOfStudy;

        public BookingPage(string userFieldOfStudy)
        {
            InitializeComponent();
            _userFieldOfStudy = userFieldOfStudy;
            LoadRooms();
        }

        private void LoadRooms()
        {
            IEnumerable<Room> rooms;

            if (User.CurrentUser.LoggedInUser.PermissionsLevel == 1 || User.CurrentUser.LoggedInUser.PermissionsLevel == 2) // Admin
            {
                rooms = App.DatabaseService.GetAllRooms();
            }
            else
            {
                rooms = App.DatabaseService.GetRoomsByFieldOfStudy(_userFieldOfStudy);
            }

            RoomsCollectionView.ItemsSource = rooms;
        }

        public void OnSelectRoomClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedRoom = (Room)button.BindingContext;

            var bookingDetailsPage = new BookingDetails(selectedRoom);
            Application.Current.MainPage = new NavigationPage(bookingDetailsPage);
        }

        public async void OnBookRoomClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedRoom = (Room)button.BindingContext;

            TimeSpan defaultStart = new TimeSpan(9, 0, 0); // 9:00 AM
            TimeSpan defaultEnd = new TimeSpan(12, 0, 0);  // 12:00 PM
            DateTime bookingDate = DateTime.Today;

            var booking = new Booking
            {
                UserId = User.CurrentUser.LoggedInUser.Id,
                RoomId = selectedRoom.Id,
                BookingDate = bookingDate,
                BookingStart = defaultStart,
                BookingEnd = defaultEnd,
                RoomName = selectedRoom.RoomName
            };

            App.DatabaseService.AddBooking(booking);

            await DisplayAlert("Booking Confirmed",
                $"You have successfully booked {selectedRoom.RoomName} on {bookingDate:MM/dd/yyyy} from {defaultStart} to {defaultEnd}.",
                "OK");

            await Navigation.PushAsync(new ViewBookings());
        }


        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new OpeningPage();
        }
    }
}
