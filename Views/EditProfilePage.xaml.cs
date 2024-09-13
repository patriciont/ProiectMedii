using static BookingApp.Models.User;

namespace BookingApp.Views;

public partial class EditProfilePage : ContentPage
{
	public EditProfilePage()
	{
		InitializeComponent();
        LoadUserProfile();
    }

    private void LoadUserProfile()
    {
        // Load user data here, for example:
        var user = App.DatabaseService.GetUser(CurrentUser.LoggedInUser.Id);

        UsernameLabel.Text = user.Username;
        UniqueIdLabel.Text = user.Id.ToString();
        FieldOfStudyLabel.Text = user.FieldOfStudy;

        // Make sure the email entry is populated and passwords are cleared
        EmailEntry.Text = user.Email;
        OldPasswordEntry.Text = string.Empty;
        NewPasswordEntry.Text = string.Empty;
    }

    private async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        // Validate input and save changes
        var email = EmailEntry.Text;
        var oldPassword = OldPasswordEntry.Text;
        var newPassword = NewPasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
        {
            await DisplayAlert("Error", "Please fill in all required fields.", "OK");
            return;
        }

        // Update user data here, e.g., check old password and save new password
        var user = App.DatabaseService.GetUser(CurrentUser.LoggedInUser.Id);
        if (user.Password != oldPassword)
        {
            await DisplayAlert("Error", "Old password is incorrect.", "OK");
            return;
        }

        user.Email = email;
        user.Password = newPassword;  // Update password after validation

        App.DatabaseService.UpdateUser(user);

        await DisplayAlert("Success", "Profile updated successfully.", "OK");

        // Navigate back or refresh profile
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}