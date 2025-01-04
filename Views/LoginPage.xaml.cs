using BookingApp.Models;
using Microsoft.Maui.Controls;
using static BookingApp.Models.User;

namespace BookingApp.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UsernameEntry?.Text) || string.IsNullOrWhiteSpace(PasswordEntry?.Text))
        {
            ShowNotification("Please enter both username and password.");
            return;
        }

        var username = UsernameEntry.Text.Trim();
        var password = PasswordEntry.Text.Trim();

        // Check if the user is the default admin
        if (username == "admin" && password == "admin123")
        {
            CurrentUser.LoggedInUser = new User
            {
                Username = "admin",
                Password = "admin123",
            };

            Application.Current.MainPage = new NavigationPage(new AdminOverviewPage());
            return;
        }

        var user = App.DatabaseService.GetUserByUsername(username);
        if (user != null && user.Password == password) // Password check
        {
            CurrentUser.LoggedInUser = user;

            Application.Current.MainPage = new NavigationPage(new OpeningPage());
        }
        else
        {
            ShowNotification("Invalid username and password.");
        }
    }


    private async void OnSignUpButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new SignUpPage());
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