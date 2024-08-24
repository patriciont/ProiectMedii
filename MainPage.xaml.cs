using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Models;
using BookingApp.Services;

namespace BookingApp
{

    // ADMIN PAGE
    public partial class MainPage : ContentPage
    {

        // Dropdown menus
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

        // Commands variables
        public Command<User> DeleteUserCommand { get; private set; }
        public Command<Room> DeleteRoomCommand { get; private set; }


        public MainPage()
        {
            InitializeComponent();

            // Dropdown Menus
            RoomFieldOfStudyPicker.ItemsSource = _roomFieldsOfStudy;
            UserFieldOfStudyPicker.ItemsSource = _userFieldsOfStudy;

            // Define Commands
            DeleteUserCommand = new Command<User>(OnDeleteUser);
            DeleteRoomCommand = new Command<Room>(OnDeleteRoom);

            // Set the BindingContext to this page
            BindingContext = this;

            // Get all Users and Rooms
            LoadUsers();
            LoadRooms();
        }

        // Add user button
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

            // Clear the input fields
            UserNameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            UserFieldOfStudyPicker.SelectedItem = null;

            StatusLabel.Text = $"User '{user.Username}' added successfully.";
        }

        // Add room button

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
            LoadRooms();

            // Clear the input fields
            RoomName.Text = string.Empty;
            RoomNumber.Text = string.Empty;
            RoomDescriptionEntry.Text = string.Empty;
            RoomFieldOfStudyPicker.SelectedItem = null;
            RoomCapacityEntry.Text = string.Empty;

            StatusLabel.Text = $"Room '{room.RoomName}' added successfully.";
        }

        // Delete user button

        private async void OnDeleteUser(User user)
        {
            bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete user {user.Username}?", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteUser(user.Id);
                LoadUsers();
                StatusLabel.Text = $"User '{user.Username}' deleted.";
            }
        }

        // Delete room button
        private async void OnDeleteRoom(Room room)
        {
            bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete room {room.RoomName}?", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteRoom(room.Id);
                LoadRooms();
                StatusLabel.Text = $"Room '{room.RoomName}' deleted.";
            }
        }

        // Delete all users button
        private async void OnDeleteAllUsersClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm", "Are you sure you want to delete all users? This action cannot be undone.", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteAllUsers();
                LoadUsers();  
                LoadRooms();  
                StatusLabel.Text = "All data deleted.";
            }
        }

        // Delete all rooms button
        private async void OnDeleteAllRoomsClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm", "Are you sure you want to delete all rooms? This action cannot be undone.", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteAllRooms();
                LoadUsers();  
                LoadRooms();  
                StatusLabel.Text = "All data deleted.";
            }
        }

        // Go to site button

        private void OnGoToSiteClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new Views.OpeningPage();
        }

        // Methods to get all users and rooms

        private void LoadUsers()
        {
            var users = App.DatabaseService.GetAllUsers();
            UsersCollectionView.ItemsSource = users;
        }

        private void LoadRooms()
        {
            var rooms = App.DatabaseService.GetAllRooms();
            RoomsCollectionView.ItemsSource = rooms;
        }

    }

}
