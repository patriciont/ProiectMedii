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


        // Selected Room

        private Room _selectedRoom;
        public Room SelectedRoom { get => _selectedRoom; set { _selectedRoom = value; OnPropertyChanged(nameof(SelectedRoom)); }}

        

        // Dropdown menus
        private List<string> _roomFieldsOfStudy = new List<string>
        {
            "Computer Science", "Library", "Physics", "Engineering", "Biology", "Chemistry", "Arts", "Commons"
        };

        // Commands variables
        public Command<Room> DeleteRoomCommand { get; private set; }


        // INIT
        public MainPage()
        {
            InitializeComponent();

            _roomService = new RoomService(App.DatabaseService);

            // Dropdown Menus
            RoomFieldOfStudyPicker.ItemsSource = _roomFieldsOfStudy;

            // Define Commands
            DeleteRoomCommand = new Command<Room>(OnDeleteRoom);

            // Set the BindingContext to this page
            BindingContext = this;

            // Get all Rooms
            LoadRooms();
            UpdateSavedDays();
            SavedDaysCollectionView.ItemsSource = _savedDays;
            ClearRoomFields();
        }

        // ROOM

        // SAVE ROOM
        private void OnAddRoomClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RoomNameEntry.Text) || string.IsNullOrWhiteSpace(RoomNumberEntry.Text) ||
                RoomFieldOfStudyPicker.SelectedItem == null )
            {
                ShowNotification("Error. Room Name, Room Number, and Field Of Study cannot be empty");
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
                ShowNotification("Error. Please select a room first.");
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

        // D ROOM
        private async void OnDeleteRoom(Room room)
        {
            bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to delete room {room.RoomName}?", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteRoom(room.Id);
                LoadRooms();
                ShowNotification($"Room '{room.RoomName}' deleted.");
            }
        }


        // D ALL ROOMS
        private async void OnDeleteAllRoomsClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm", "Are you sure you want to delete all rooms? This action cannot be undone.", "Yes", "No");
            if (confirm)
            {
                App.DatabaseService.DeleteAllRooms();
                LoadRooms();
                ShowNotification("All data deleted.");
            }
        }


        // NAVIGATION

        private void OnGoToSiteClicked(object sender, EventArgs e)
        {
            string userFieldOfStudy = User.CurrentUser.LoggedInUser?.FieldOfStudy ?? "Commons";
            var databaseService = App.DatabaseService;

            var bookingPage = new BookingPage(userFieldOfStudy);
            var loginPage = new LoginPage();
            var landingPage = new OpeningPage();
            var profilePage = new ManageBookings();
            Application.Current.MainPage = new NavigationPage(profilePage);
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
            ShowNotification($"You selected: {selectedRoom.RoomName}, {selectedRoom.RoomNumber}.");
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

        // NOTIFICATION BAR

        private async void ShowNotification(string message)
        {
            NotificationLabel.Text = message;
            NotificationFrame.IsVisible = true;

            await Task.Delay(5000);

            NotificationFrame.IsVisible = false;
        }

        private void CloseNotificationClicked(object sender, EventArgs e)
        {
            NotificationFrame.IsVisible = false;
        }

    }

}
