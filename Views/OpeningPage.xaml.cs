namespace BookingApp.Views;

public partial class OpeningPage : ContentPage
{
	public OpeningPage()
	{
		InitializeComponent();
	}

	private void OnBookButtonClicked(object sender, EventArgs e)
	{
        Application.Current.MainPage = new BookingPage();
    }

	private void OnManageButtonClicked(object sender, EventArgs e)
	{
		Application.Current.MainPage = new ManageBookings();
	}
}