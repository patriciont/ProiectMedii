namespace BookingApp.Views;
using static BookingApp.Models.User;

public partial class ManageBookings : ContentPage
{
	public ManageBookings()
	{
		InitializeComponent();
        LoadUserProfile();

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

    private void OnViewBookingsButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new ViewBookings());
    }

    private void OnEditProfileButtonClicked(object sender, EventArgs e)
    {
        //Application.Current.MainPage.Navigation.PushAsync(new EditProfilePage());
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