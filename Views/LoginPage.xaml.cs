using Microsoft.Maui.Controls;

namespace BookingApp.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            LoginStatusLabel.Text = "Username and password cannot be empty.";
            LoginStatusLabel.IsVisible = true;
            return;
        }

        if (username == "Student" && password == "Password") 
        {
            LoginStatusLabel.IsVisible = false;
            Application.Current.MainPage = new OpeningPage(); 
        }
        else
        {
            LoginStatusLabel.Text = "Invalid login, please try again.";
            LoginStatusLabel.IsVisible = true;
        }
    }
}