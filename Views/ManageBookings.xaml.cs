namespace BookingApp.Views;
using static BookingApp.Models.User;

public partial class ManageBookings : ContentPage
{
	public ManageBookings()
	{
		InitializeComponent();
        LoadUserProfile();
        CheckUserPermissions();
    }

    private void LoadUserProfile()
    {
        // Load the user's profile information
        var user = CurrentUser.LoggedInUser;

        if (user != null)
        {
            UserNameLabel.Text = user.Username;
            UserEmailLabel.Text = user.Email;
            UserFieldOfStudyLabel.Text = user.FieldOfStudy;
        }
    }

    private void CheckUserPermissions()
    {
        var currentUser = CurrentUser.LoggedInUser;

        if (currentUser != null && currentUser.PermissionsLevel == 1)
        {
            RoomButton.IsVisible = true;
            UserButton.IsVisible = true;
            UserBookingsButton.IsVisible = true;
        }
    }

    private async void OnRoomButtonClicked(object sender, EventArgs e)
    {
        // Navigate to the manage rooms Page
        await Navigation.PushAsync(new MainPage()); 
    }

    private async void OnUserButtonClicked(object sender, EventArgs e)
    {
        // Navigate to the manage users Page
        await Navigation.PushAsync(new ManageUsers());
    }

    private async void OnUserBookingsButtonClicked(object sender, EventArgs e)
    {
        // Navigate to the User bookings Page
        await Navigation.PushAsync(new AdminOverviewPage());
    }

    private void OnLogOutButtonClicked(object sender, EventArgs e)
    {
        // Handle user log out and navigate to the login page
        CurrentUser.LoggedInUser = null;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new OpeningPage();
    }
}