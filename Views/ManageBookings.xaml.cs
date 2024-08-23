namespace BookingApp.Views;

public partial class ManageBookings : ContentPage
{
	public ManageBookings()
	{
		InitializeComponent();
	}

	private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new OpeningPage();
    }
}