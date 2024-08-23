namespace BookingApp.Views;

public partial class BookingPage : ContentPage
{
	public BookingPage()
	{
		InitializeComponent();
	}

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new OpeningPage();
    }
}