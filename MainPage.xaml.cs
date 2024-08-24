using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Models;
using BookingApp.Services;

namespace BookingApp
{
    public partial class MainPage : ContentPage
    {
        private List<string> _roomFieldsOfStudy = new List<string>
        {
            "Computer Science",
            "Library",
            "Physics",
            "Engineering",
            "Biology",
            "Chemistry",
            "Arts",
            "Open"
        };

        private List<string> _userFieldsOfStudy = new List<string>
        {
            "Computer Science",
            "Business",
            "Physics",
            "Engineering",
            "Biology",
            "Chemistry",
            "Arts"
        };

        public MainPage()
        {
            InitializeComponent();

            RoomFieldOfStudyPicker.ItemsSource = _roomFieldsOfStudy;
            UserFieldOfStudyPicker.ItemsSource = _userFieldsOfStudy;

            
            LoadUsers();
            LoadRooms();
        }

        private void OnAddUserClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserNameEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                StatusLabel.Text = "Please enter a valid name and password.";
                return;
            }

            var user = new User
            {
                Username = UserNameEntry.Text.Trim(),
                Password = PasswordEntry.Text.Trim(),
                FieldOfStudy = UserFieldOfStudyPicker.SelectedItem?.ToString() ?? "Unspecified",
                PermissionsLevel = 4
            };

            App.DatabaseService.SaveUser(user);
            LoadUsers(); 
            StatusLabel.Text = $"User '{user.Username}' added successfully.";
        }

        private void OnAddRoomClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RoomName.Text) || string.IsNullOrWhiteSpace(RoomNumber.Text))
            {
                StatusLabel.Text = "Please enter a valid room name and number.";
                return;
            }

            var room = new Room
            {
                RoomName = RoomName.Text.Trim(),
                RoomNumber = RoomNumber.Text.Trim(),
                Description = RoomDescriptionEntry.Text?.Trim(),
                FieldOfStudy = RoomFieldOfStudyPicker.SelectedItem?.ToString() ?? "Unspecified",
                Capacity = int.TryParse(RoomCapacityEntry.Text, out var capacity) ? capacity : 0
            };

            App.DatabaseService.SaveRoom(room);
            LoadRooms(); // Reload the room list
            StatusLabel.Text = $"Room '{room.RoomName}' added successfully.";
        }

        private async void OnDeleteUserClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var user = (User)button.BindingContext;  

            bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete user {user.Username}?", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteUser(user.Id);
                LoadUsers(); 
                StatusLabel.Text = $"User '{user.Username}' deleted.";
            }
        }

        private async void OnDeleteRoomClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var room = (Room)button.BindingContext;

            bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete room {room.RoomName}?", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteRoom(room.Id);
                LoadRooms(); 
                StatusLabel.Text = $"Room '{room.RoomName}' deleted.";
            }
        }

        private async void OnDeleteAllUsersClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm", "Are you sure you want to delete all users? This action cannot be undone.", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteAllUsers();
                LoadUsers();  // Refresh the user list
                LoadRooms();  // Refresh the room list
                StatusLabel.Text = "All data deleted.";
            }
        }

        private async void OnDeleteAllRoomsClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm", "Are you sure you want to delete all rooms? This action cannot be undone.", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteAllRooms();
                LoadUsers();  // Refresh the user list
                LoadRooms();  // Refresh the room list
                StatusLabel.Text = "All data deleted.";
            }
        }

        private void LoadUsers()
        {
            var users = App.DatabaseService.GetAllUsers();
            UsersListView.ItemsSource = users;
        }

        private void LoadRooms()
        {
            var rooms = App.DatabaseService.GetAllRooms();
            RoomsListView.ItemsSource = rooms;
        }

    }

}
