namespace BookingApp.Views;

public partial class PrivacyStatementPage : ContentPage
{
	public PrivacyStatementPage()
	{
		InitializeComponent();
	}

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}