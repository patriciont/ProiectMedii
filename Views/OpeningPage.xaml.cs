using static BookingApp.Models.User;
namespace BookingApp.Views;

public partial class OpeningPage : ContentPage
{
	public OpeningPage()
	{
		InitializeComponent();
	}

    private void OnBookButtonClicked(object sender, EventArgs e)
    {
        // Retrieve the field of study from the current user
        string userFieldOfStudy = CurrentUser.LoggedInUser?.FieldOfStudy ?? "DefaultFieldOfStudy";

        // Get the database service instance
        var databaseService = App.DatabaseService;

        // Create and navigate to the BookingPage
        var bookingPage = new BookingPage(userFieldOfStudy);
        Application.Current.MainPage = new NavigationPage(bookingPage);
    }
    private void OnManageButtonClicked(object sender, EventArgs e)
	{
		Application.Current.MainPage = new ManageBookings();
	}
}