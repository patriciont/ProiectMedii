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

        // Admin list
        private List<Admin> _adminList = new List<Admin>
        {
            // Update later, connect to database (getalladmins()?)
            new Admin { Id = 1, Name = "John Doe" },
            new Admin { Id = 2, Name = "Jane Smith" }
        };

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
            "Commons"
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

            AdminPicker.ItemsSource = _adminList;

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
            // Validate inputs
            if (string.IsNullOrWhiteSpace(UserNameEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                UserFieldOfStudyPicker.SelectedItem == null)
            {
                UserStatusLabel.Text = "Please fill in all required fields.";
                UserStatusLabel.TextColor = Colors.Red;
                return;
            }

            // Create new User object
            var user = new User
            {
                Username = UserNameEntry.Text.Trim(),
                Password = PasswordEntry.Text.Trim(),
                Email = EmailEntry.Text.Trim(),
                FieldOfStudy = UserFieldOfStudyPicker.SelectedItem.ToString(),
                PermissionsLevel = 4 // Default permissions level
            };

            // Save user to database
            App.DatabaseService.SaveUser(user);

            // Clear input fields
            UserNameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            UserFieldOfStudyPicker.SelectedItem = null;

            // Update status label and reload users
            UserStatusLabel.Text = $"User '{user.Username}' added successfully.";
            UserStatusLabel.TextColor = Colors.Green;
            LoadUsers();
        }

        // Add room button
        private void OnAddRoomClicked(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(RoomNameEntry.Text) || string.IsNullOrWhiteSpace(RoomNumberEntry.Text) || RoomFieldOfStudyPicker.SelectedItem == null || AdminPicker.SelectedItem == null)
            {
                RoomStatusLabel.Text = "Please fill in all required fields.";
                RoomStatusLabel.TextColor = Colors.Red;
                return;
            }

            // Create new Room object
            var room = new Room
            {
                RoomName = RoomNameEntry.Text.Trim(),
                RoomNumber = RoomNumberEntry.Text.Trim(),
                Description = RoomDescriptionEntry.Text?.Trim(),
                FieldOfStudy = RoomFieldOfStudyPicker.SelectedItem.ToString(),
                Capacity = int.TryParse(RoomCapacityEntry.Text, out var capacity) ? capacity : 0,
                AdminId = ((Admin)AdminPicker.SelectedItem).Id,
                AvailabilitySlots = new List<RoomSlot>()
            };

            // Add room slots
            foreach (var slotLayout in SlotContainer.Children)
            {
                var startTimePicker = (slotLayout as StackLayout).Children[1] as TimePicker;
                var endTimePicker = (slotLayout as StackLayout).Children[3] as TimePicker;

                var slot = new RoomSlot
                {
                    StartTime = DateTime.Today.Add(startTimePicker.Time),
                    EndTime = DateTime.Today.Add(endTimePicker.Time)
                };

                room.AvailabilitySlots.Add(slot);
            }

            // Save room to database
            App.DatabaseService.SaveRoom(room);

            // Clear fields
            RoomNameEntry.Text = string.Empty;
            RoomNumberEntry.Text = string.Empty;
            RoomDescriptionEntry.Text = string.Empty;
            RoomFieldOfStudyPicker.SelectedIndex = -1;
            RoomCapacityEntry.Text = string.Empty;
            AdminPicker.SelectedIndex = -1;
            SlotContainer.Children.Clear();

            // Update status label
            RoomStatusLabel.Text = $"Room '{room.RoomName}' added successfully.";
            RoomStatusLabel.TextColor = Colors.Green;

            // Reload rooms if needed
            LoadRooms();
        }

        // Add slot button
        private void OnAddSlotClicked(object sender, EventArgs e)
        {
            var slotLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 10 };

            var startTimePicker = new TimePicker();
            var endTimePicker = new TimePicker();
            var removeButton = new Button { Text = "Remove", BackgroundColor = Colors.Red, TextColor = Colors.White };

            removeButton.Clicked += (s, args) => SlotContainer.Children.Remove(slotLayout);

            slotLayout.Children.Add(new Label { Text = "Start:" });
            slotLayout.Children.Add(startTimePicker);
            slotLayout.Children.Add(new Label { Text = "End:" });
            slotLayout.Children.Add(endTimePicker);
            slotLayout.Children.Add(removeButton);

            SlotContainer.Children.Add(slotLayout);
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
