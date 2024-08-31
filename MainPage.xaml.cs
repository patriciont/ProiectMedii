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


        // INIT
        public MainPage()
        {
            InitializeComponent();

            App.PrintDatabasePath();

            _roomService = new RoomService(App.DatabaseService);

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

            
            UpdateSavedDays();
            
            SavedDaysCollectionView.ItemsSource = _savedDays;
            ClearRoomFields();
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

        // ADD ROOM
        private void OnAddRoomClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RoomNameEntry.Text) || string.IsNullOrWhiteSpace(RoomNumberEntry.Text) ||
                RoomFieldOfStudyPicker.SelectedItem == null || !_savedDays.Any())
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
                AvailableDays = _savedDays
            };

            // Save room to database
            App.DatabaseService.SaveRoom(room);

            // Save available days and slots
            foreach (var day in _savedDays)
            {
                day.RoomId = room.Id; // Set foreign key
                var dayId = App.DatabaseService.SaveAvailableDay(day); // Save day and get ID

                foreach (var slot in day.Slots)
                {
                    slot.AvailableDayId = dayId; 
                    slot.RoomId = room.Id; 
                    App.DatabaseService.SaveRoomSlot(slot); 

                }
            }

            LoadRooms();
            ClearRoomFields();
        }


        // ADD SLOT
        private void OnAddSlotClicked(object sender, EventArgs e)
        {
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

        // REMOVE SLOT
        private void OnRemoveSlotClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var slotLayout = (StackLayout)button.Parent;
            SlotTemplateContainer.Children.Remove(slotLayout);
        }

        // ADD DAY
        private void OnAddDayClicked(object sender, EventArgs e)
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
                        Slots = new List<RoomSlot>(_currentSlotTemplate)
                    };

                    _AvailableDays.Add(newDay);
                }
            }
        }

        // SAVE TEMPLATE
        private void OnSaveTemplateClicked(object sender, EventArgs e)
        {

            // Add days first
            OnAddDayClicked(sender, e);

            // Create a new list of slots to use for each day
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

                        //room id here
                    };

                    newSlots.Add(roomSlot);
                }
            }

            // Assign slots to each available day
            foreach (var day in _AvailableDays)
            {
                // Make a deep copy of slots to ensure each day gets its own instances
                day.Slots = newSlots.Select(slot => new RoomSlot
                {
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    AvailableDayId = day.Id
                    //roomid here
                }).ToList();
            }

            _savedDays.AddRange(_AvailableDays);
            UpdateSavedDays();
            ClearSlotTemplate();
            _AvailableDays.Clear();
        }


        // DELETE IN DATABASE

        // D USER

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


        // LOAD AND UPDATE

        // L USERS

        private void LoadUsers()
        {
            var users = App.DatabaseService.GetAllUsers();
            UsersCollectionView.ItemsSource = users;
        }

        // L ROOMS
        private void LoadRooms()
        {
            var rooms = _roomService.GetAllRooms();
            RoomsCollectionView.ItemsSource = rooms;
        }

        // U SAVED DAYS
        private void UpdateSavedDays()
        {
            SavedDaysCollectionView.ItemsSource = null; 
            SavedDaysCollectionView.ItemsSource = _savedDays;
        }

    }

}
