using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Models;
using BookingApp.Services;
using BookingApp.Views;

namespace BookingApp
{

    // ADMIN PAGE
    public partial class MainPage : ContentPage
    {

        private readonly RoomService _roomService;

        private List<RoomSlot> _currentSlotTemplate = new List<RoomSlot>();
        private List<AvailableDay> _AvailableDays = new List<AvailableDay>();
        private List<AvailableDay> _savedDays = new List<AvailableDay>();


        // Admin list
        private List<Admin> _adminList = new List<Admin>
        {
            // Update later, connect to database (getalladmins()?)
            new Admin { Id = 1, Name = "John Doe" }
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

            _roomService = new RoomService(App.DatabaseService);

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

            UpdateSelectedDays();
            UpdateSavedDays();

            AvailableDaysCollectionView.ItemsSource = _AvailableDays;
            SavedDaysCollectionView.ItemsSource = _savedDays;
        }

        private List<AvailableDay> GetAvailableDays()
        {
            // Example data, replace with actual implementation
            return new List<AvailableDay>
            {
                new AvailableDay { Date = DateTime.Now.AddDays(1), IsSelected = false },
                new AvailableDay { Date = DateTime.Now.AddDays(2), IsSelected = false },
                // Add more days as needed
            };
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
            if (string.IsNullOrWhiteSpace(RoomNameEntry.Text) || string.IsNullOrWhiteSpace(RoomNumberEntry.Text) ||
                RoomFieldOfStudyPicker.SelectedItem == null || AdminPicker.SelectedItem == null ||
                !_savedDays.Any())
            {
                // Display error message
                return;
            }

            var room = new Room
            {
                RoomName = RoomNameEntry.Text,
                RoomNumber = RoomNumberEntry.Text,
                Description = RoomDescriptionEntry.Text,
                FieldOfStudy = RoomFieldOfStudyPicker.SelectedItem.ToString(),
                Capacity = int.TryParse(RoomCapacityEntry.Text, out var capacity) ? capacity : 0,
                AdminId = ((Admin)AdminPicker.SelectedItem).Id,
                AvailableDays = _savedDays
            };

            // Save room to database
            App.DatabaseService.SaveRoom(room);

            ClearRoomFields();
        }


        // Add slot button
        private void OnAddSlotClicked(object sender, EventArgs e)
        {
            var slotLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 10 };

            var startTimePicker = new TimePicker();
            var endTimePicker = new TimePicker();
            var removeButton = new Button { Text = "Remove", BackgroundColor = Colors.Red, TextColor = Colors.White };

            removeButton.Clicked += (s, args) => SlotTemplateContainer.Children.Remove(slotLayout);

            slotLayout.Children.Add(new Label { Text = "Start:" });
            slotLayout.Children.Add(startTimePicker);
            slotLayout.Children.Add(new Label { Text = "End:" });
            slotLayout.Children.Add(endTimePicker);
            slotLayout.Children.Add(removeButton);

            SlotTemplateContainer.Children.Add(slotLayout);
        }

        private void OnRemoveSlotClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var slotLayout = (StackLayout)button.Parent;
            SlotTemplateContainer.Children.Remove(slotLayout);
        }

        private void OnSaveTemplateClicked(object sender, EventArgs e)
        {
            var newSlots = new List<RoomSlot>();

            foreach (var child in SlotTemplateContainer.Children)
            {
                if (child is StackLayout slotLayout)
                {
                    var startTimePicker = (TimePicker)slotLayout.Children[1];
                    var endTimePicker = (TimePicker)slotLayout.Children[3];

                    var roomSlot = new RoomSlot
                    {
                        StartTime = startTimePicker.Time,
                        EndTime = endTimePicker.Time
                    };

                    newSlots.Add(roomSlot);
                }
            }

            foreach (var day in _AvailableDays)
            {
                day.Slots = new List<RoomSlot>(newSlots);
            }

            _savedDays.AddRange(_AvailableDays);
            UpdateSavedDays();
            ClearSlotTemplate();
            _AvailableDays.Clear();
            UpdateSelectedDays();
        }

        private void OnAvailableDaysSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = e.CurrentSelection.Cast<AvailableDay>().ToList();
            foreach (var item in selectedItems)
            {
                item.IsSelected = true;
                if (!_AvailableDays.Contains(item))
                {
                    _AvailableDays.Add(item);
                }
            }

            var deselectedItems = e.PreviousSelection.Cast<AvailableDay>().ToList();
            foreach (var item in deselectedItems)
            {
                item.IsSelected = false;
                _AvailableDays.Remove(item);
            }

            UpdateSelectedDays();
        }

        private void ClearSlotTemplate()
        {
            SlotTemplateContainer.Children.Clear();
        }

        private void UpdateSelectedDays()
        {
            AvailableDaysCollectionView.ItemsSource = null; // Reset ItemsSource to update the CollectionView
            AvailableDaysCollectionView.ItemsSource = _AvailableDays;
        }

        private void UpdateSavedDays()
        {
            SavedDaysCollectionView.ItemsSource = null; // Reset ItemsSource to update the CollectionView
            SavedDaysCollectionView.ItemsSource = _savedDays;
        }

        private void OnAddDayClicked(object sender, EventArgs e)
        {
            var selectedDate = DatePicker.Date;

            if (!_AvailableDays.Any(d => d.Date == selectedDate))
            {
                _AvailableDays.Add(new AvailableDay { Date = selectedDate });
                UpdateSelectedDays();
            }
        }

        private void OnRemoveDayClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var day = (AvailableDay)button.BindingContext;
            _AvailableDays.Remove(day);
            UpdateSelectedDays();
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
            // Get the field of study from the current user
            string userFieldOfStudy = User.CurrentUser.LoggedInUser?.FieldOfStudy ?? "DefaultFieldOfStudy";
            var databaseService = App.DatabaseService;

            var bookingPage = new BookingPage(userFieldOfStudy);
            var loginPage = new LoginPage();
            Application.Current.MainPage = new NavigationPage(loginPage);
        }

        // Clear room inputs
        private void ClearRoomFields()
        {
            RoomNameEntry.Text = string.Empty;
            RoomNumberEntry.Text = string.Empty;
            RoomDescriptionEntry.Text = string.Empty;
            RoomFieldOfStudyPicker.SelectedIndex = -1;
            RoomCapacityEntry.Text = string.Empty;
            AdminPicker.SelectedIndex = -1;
            SlotTemplateContainer.Children.Clear();
            _currentSlotTemplate.Clear();
            _AvailableDays.Clear();
            _savedDays.Clear();
            UpdateSelectedDays();
            UpdateSavedDays();
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
