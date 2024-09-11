using BookingApp.Models;
using BookingApp.Services;
using BookingApp.Views;
using System.Collections.ObjectModel;

namespace BookingApp.Views;

public partial class ManageUsers : ContentPage
{
    public ObservableCollection<User> Users { get; set; }
    public ObservableCollection<User> FilteredUsers { get; set; }

    // Selected User
    private User _selectedUser;
    public User SelectedUser
    { get => _selectedUser; set { _selectedUser = value; OnPropertyChanged(nameof(SelectedUser)); } }

    // Dropdown menus
    private List<string> _userFieldsOfStudy = new List<string>
        {
            "Computer Science", "Business", "Physics", "Engineering", "Biology", "Chemistry", "Arts"
        };

    private List<string> _permissionsLevels = new List<string>
        {
            "1 - Super User",
            "2 - Low Admin",
            "4 - Client User"
        };

    // Commands variables
    public Command<User> DeleteUserCommand { get; private set; }
    public Command<Room> DeleteRoomCommand { get; private set; }

    public ManageUsers()
	{
		InitializeComponent();

        UserFieldOfStudyPicker.ItemsSource = _userFieldsOfStudy;
        UserPermissionsPicker.ItemsSource = _permissionsLevels;

        FilteredUsers = new ObservableCollection<User>();
        UserPicker.ItemsSource = FilteredUsers;

        // Define Commands
        DeleteUserCommand = new Command<User>(OnDeleteUser);

        // User list
        Users = new ObservableCollection<User>();

        // Set the BindingContext to this page
        BindingContext = this;

        LoadUsers();
    }

    // USER 

    // Add user button
    private void OnAddUserClicked(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(UserNameEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            UserFieldOfStudyPicker.SelectedItem == null ||
            UserPermissionsPicker.SelectedItem == null)
        {
            ShowNotification("Please fill in all required fields.");
            return;
        }

        // Get selected permissions level
        int permissionsLevel = UserPermissionsPicker.SelectedItem.ToString() switch
        {
            "1 - Super User" => 1,
            "2 - Low Admin" => 2,
            "4 - Client User" => 4,
            _ => 4 // Default to Client User
        };

        // Create User object
        var user = new User
        {
            Username = UserNameEntry.Text.Trim(),
            Password = PasswordEntry.Text.Trim(),
            Email = EmailEntry.Text.Trim(),
            FieldOfStudy = UserFieldOfStudyPicker.SelectedItem.ToString(),
            PermissionsLevel = permissionsLevel
        };

        // Save user to database
        App.DatabaseService.SaveUser(user);
        ShowNotification($"User '{user.Username}' added successfully.");
        ClearUserInputs();
    }

    // Filter user list
    private void OnUserSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            FilteredUsers.Clear();
            foreach (var user in Users)
            {
                FilteredUsers.Add(user);
            }
        }
        else
        {
            FilteredUsers.Clear();
            foreach (var user in Users.Where(u => u.Username.ToLower().Contains(searchText)))
            {
                FilteredUsers.Add(user);
            }
        }
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
            ShowNotification($"User '{user.Username}' deleted.");
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
            ShowNotification("All data deleted.");
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

    // C USER FIELD
    private void ClearUserInputs()
    {
        UserNameEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        UserFieldOfStudyPicker.SelectedItem = null;
        UserPermissionsPicker.SelectedItem = null;

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

        OnUserSearchBarTextChanged(null, new TextChangedEventArgs(string.Empty, string.Empty));
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