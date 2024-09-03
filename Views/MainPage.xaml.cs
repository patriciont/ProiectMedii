using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingApp.Models;
using BookingApp.Services;
using BookingApp.Views;
using Plugin.Maui.Calendar.Models;
using Plugin.Maui.Calendar.Controls;
using Plugin.Maui.Calendar.Shared.Controls.SelectionEngines;
using System.Collections.ObjectModel;

namespace BookingApp
{

    // ADMIN PAGE
    public partial class MainPage : ContentPage
    {

        // Calender 
        public EventCollection Events { get; set; }
        private readonly MultiSelectionEngine _selectionEngine = new MultiSelectionEngine();

        // Room service (database connection class)
        private readonly RoomService _roomService;

        // Lists
        private List<RoomSlot> _currentSlotTemplate = new List<RoomSlot>();
        private List<AvailableDay> _AvailableDays = new List<AvailableDay>();
        private List<AvailableDay> _savedDays = new List<AvailableDay>();
        public ObservableCollection<User> Users { get; set; }


        // Selected Room

        private Room _selectedRoom;
        public Room SelectedRoom { get => _selectedRoom; set { _selectedRoom = value; OnPropertyChanged(nameof(SelectedRoom)); }}

        // Selected User
        private User _selectedUser;
        public User SelectedUser
        { get => _selectedUser; set { _selectedUser = value; OnPropertyChanged(nameof(SelectedUser));}}

        // Dropdown menus
        private List<string> _roomFieldsOfStudy = new List<string>
        {
            "Computer Science", "Library", "Physics", "Engineering", "Biology", "Chemistry", "Arts", "Commons"
        };

        private List<string> _userFieldsOfStudy = new List<string>
        {
            "Computer Science", "Business", "Physics", "Engineering", "Biology", "Chemistry", "Arts"
        };

        // Commands variables
        public Command<User> DeleteUserCommand { get; private set; }
        public Command<Room> DeleteRoomCommand { get; private set; }


        // INIT
        public MainPage()
        {
            InitializeComponent();

            _roomService = new RoomService(App.DatabaseService);

            // Dropdown Menus
            RoomFieldOfStudyPicker.ItemsSource = _roomFieldsOfStudy;
            UserFieldOfStudyPicker.ItemsSource = _userFieldsOfStudy;

            // Define Commands
            DeleteUserCommand = new Command<User>(OnDeleteUser);
            DeleteRoomCommand = new Command<Room>(OnDeleteRoom);

            // User list
            Users = new ObservableCollection<User>();

            // Set the BindingContext to this page
            BindingContext = this;

            // Get all Users and Rooms
            LoadUsers();
            LoadRooms();
            UpdateSavedDays();
            SavedDaysCollectionView.ItemsSource = _savedDays;
            ClearRoomFields();
        }

        // USER 

        // Add user button
        private void OnAddUserClicked(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(UserNameEntry.Text) ||
                string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
                string.IsNullOrWhiteSpace(EmailEntry.Text) ||
                UserFieldOfStudyPicker.SelectedItem == null)
            {
                UserStatusLabel.Text = "Please fill in all required fields.";
                UserStatusLabel.TextColor = Colors.Red;
                return;
            }

            // Create User object
            var user = new User
            {
                Username = UserNameEntry.Text.Trim(),
                Password = PasswordEntry.Text.Trim(),
                Email = EmailEntry.Text.Trim(),
                FieldOfStudy = UserFieldOfStudyPicker.SelectedItem.ToString(),
                PermissionsLevel = 4 
            };

            // Save user to database
            App.DatabaseService.SaveUser(user);
            UserStatusLabel.Text = $"User '{user.Username}' added successfully.";
            UserStatusLabel.TextColor = Colors.Green;

            ClearUserInputs();
        }

        

        // ROOM

        // SAVE ROOM
        private void OnAddRoomClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RoomNameEntry.Text) || string.IsNullOrWhiteSpace(RoomNumberEntry.Text) ||
                RoomFieldOfStudyPicker.SelectedItem == null )
            {
                DisplayAlert("Error", "Room Name, Room Number, Field Of Study cannot be empty.", "Okay");
                return;
            }

            var room = new Room
            {
                RoomName = RoomNameEntry.Text,
                RoomNumber = RoomNumberEntry.Text,
                Description = RoomDescriptionEntry.Text,
                FieldOfStudy = RoomFieldOfStudyPicker.SelectedItem.ToString(),
                Capacity = int.TryParse(RoomCapacityEntry.Text, out var capacity) ? capacity : 0,
            };

            // Save room to database
            App.DatabaseService.SaveRoom(room);

            SelectedRoom = room;
            UpdateSelectedRoomLabels(room);
            LoadRooms();
            ClearRoomFields();
        }

        // CALENDER AND SLOTS

        // SAVE DAYS 
        private void SaveSelectedDates(object sender, EventArgs e)
        {
            var selectedDates = PlugCal.SelectedDates;

            if (selectedDates == null)
            {
                return;
            }

            foreach (var date in selectedDates)
            {
                if (!_AvailableDays.Any(d => d.Date == date))
                {
                    var newDay = new AvailableDay
                    {
                        Date = date,
                        Slots = new List<RoomSlot>(_currentSlotTemplate),
                        RoomId = _selectedRoom.Id,
                    };

                    _AvailableDays.Add(newDay);
                    App.DatabaseService.SaveAvailableDay(newDay);
                }
            }
        }

        // SAVE SLOTS
        private async void SaveSlots(object sender, EventArgs e)
        {
            // Create slot list from roomslot pickers
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
                        EndTime = endTimePicker.Time,
                    };

                    newSlots.Add(roomSlot);
                }
            }

            // Assign slots to each available day
            foreach (var day in _AvailableDays)
            {
                day.Slots = newSlots.Select(slot => new RoomSlot
                {
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    AvailableDayId = day.Id,
                    RoomId = _selectedRoom.Id,
                    Capacity = _selectedRoom.Capacity,  
                    BookedCount = 0  
                }).ToList();

                foreach (var slot in day.Slots)
                {
                    await App.DatabaseService.SaveRoomSlot(slot);
                }
            }

            ClearSlotTemplate();
            _AvailableDays.Clear();
        }

        // Save slots and days button
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            SaveSelectedDates(sender, e);

            SaveSlots(sender, e);
        }


        // SLOTS

        // Add slot button
        private void OnAddSlotClicked(object sender, EventArgs e)
        {
            if (SelectedRoom == null)
            {
                DisplayAlert("Error", "Please select a room first.", "Okay");
                return;
            }

            var slotLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 10 };

            var startTimePicker = new TimePicker();
            var endTimePicker = new TimePicker();

            // Button style
            var removeButton = new Button
            {
                Text = "Remove",
                BackgroundColor = Colors.Red, 
                TextColor = Colors.White,
                WidthRequest = 80, 
                HeightRequest = 30, 
                Padding = new Thickness(5) 
            };

            // Set default times or increment time
            if (SlotTemplateContainer.Children.Count == 0)
            {
                StartTimePicker.Time = new TimeSpan(9, 0, 0);
                EndTimePicker.Time = new TimeSpan(10, 0, 0);
            }
            else
            {
                StartTimePicker.Time = StartTimePicker.Time.Add(new TimeSpan(1, 0, 0));
                EndTimePicker.Time = EndTimePicker.Time.Add(new TimeSpan(1, 0, 0));
            }

            startTimePicker.Time = StartTimePicker.Time;
            endTimePicker.Time = EndTimePicker.Time;

            // Remove button function
            removeButton.Clicked += (s, args) => SlotTemplateContainer.Children.Remove(slotLayout);

            // Create new time picker / slot
            slotLayout.Children.Add(new Label { Text = "Start:" });
            slotLayout.Children.Add(startTimePicker);
            slotLayout.Children.Add(new Label { Text = "End:" });
            slotLayout.Children.Add(endTimePicker);
            slotLayout.Children.Add(removeButton);

            SlotTemplateContainer.Children.Add(slotLayout);
        }

        // Remove slot button
        private void OnRemoveSlotClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var slotLayout = (StackLayout)button.Parent;
            SlotTemplateContainer.Children.Remove(slotLayout);
        }






            // DELETE, UPDATE, LOAD, CLEAR FIELDS //
        // ------------------------------------------- //

        // DELETE IN DATABASE

        // D USER

        private async void OnDeleteUser(User user)
        {
            if (user == null)
                return;

            bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete user {user.Username}?", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteUser(user.Id);
                LoadUsers();
                StatusLabel.Text = $"User '{user.Username}' deleted.";
            }
        }

        // D ROOM
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

        // D ALL USERS
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

        // D ALL ROOMS
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


        // NAVIGATION

        private void OnGoToSiteClicked(object sender, EventArgs e)
        {
            string userFieldOfStudy = User.CurrentUser.LoggedInUser?.FieldOfStudy ?? "Commons";
            var databaseService = App.DatabaseService;

            var bookingPage = new BookingPage(userFieldOfStudy);
            var loginPage = new LoginPage();
            Application.Current.MainPage = new NavigationPage(loginPage);
        }


        // CLEAR INPUTS

        // C ROOM FIELD
        private void ClearRoomFields()
        {
            RoomNameEntry.Text = string.Empty;
            RoomNumberEntry.Text = string.Empty;
            RoomDescriptionEntry.Text = string.Empty;
            RoomFieldOfStudyPicker.SelectedIndex = -1;
            RoomCapacityEntry.Text = string.Empty;
            //AdminPicker.SelectedIndex = -1;
            SlotTemplateContainer.Children.Clear();
            _currentSlotTemplate.Clear();
            _AvailableDays.Clear();
            _savedDays.Clear();
            UpdateSavedDays();
        }

        // C SLOT FIELD
        private void ClearSlotTemplate()
        {
            SlotTemplateContainer.Children.Clear();
        }

        // C USER FIELD
        private void ClearUserInputs()
        {
            UserNameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            EmailEntry.Text = string.Empty;
            UserFieldOfStudyPicker.SelectedItem = null;

            LoadUsers();
        }


        // LOAD AND UPDATE

        // L USERS

        private async void LoadUsers()
        {
            // Fetch users from the database
            var usersFromDb = App.DatabaseService.GetAllUsers();

            Users.Clear();
            foreach (var user in usersFromDb)
            {
                Users.Add(user);
            }
        }

        // L ROOMS
        private void LoadRooms()
        {
            var rooms = _roomService.GetAllRooms();
            RoomsCollectionView.ItemsSource = rooms;
        }

        private void OnSelectRoomClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedRoom = (Room)button.CommandParameter;

            _selectedRoom = selectedRoom;
            UpdateSelectedRoomLabels(selectedRoom);
            DisplayAlert("Room Selected", $"You selected: {selectedRoom.RoomName}", "OK");
        }

        // U SAVED DAYS
        private void UpdateSavedDays()
        {
            SavedDaysCollectionView.ItemsSource = null; 
            SavedDaysCollectionView.ItemsSource = _savedDays;
        }

        private void UpdateSelectedRoomLabels(Room room)
        {
            SelectedRoomLabel.Text = $"Adding Days To: {room.RoomName}";
        }

    }

}
