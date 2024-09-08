using BookingApp.Models;
using System.Text.RegularExpressions;


namespace BookingApp.Views;

public partial class SignUpPage : ContentPage
{

    private List<string> _userFieldsOfStudy = new List<string>
    {
        "Computer Science", "Business", "Physics", "Engineering", "Biology", "Chemistry", "Arts"
    };

    public SignUpPage()
	{
		InitializeComponent();

        FieldOfStudyPicker.ItemsSource = _userFieldsOfStudy;

        BindingContext = this;
	}

    private void OnSignupClicked(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(UsernameEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            FieldOfStudyPicker.SelectedItem == null)
        {
            SignupStatusLabel.Text = "Please fill in all required fields.";
            SignupStatusLabel.TextColor = Colors.Red;
            return;
        }

        // Check if username already exists
        var existingUser = App.DatabaseService.GetUserByUsername(UsernameEntry.Text.Trim());
        if (existingUser != null)
        {
            SignupStatusLabel.Text = "Username already exists. Please choose a different one.";
            SignupStatusLabel.TextColor = Colors.Red;
            return;
        }

        // Validate password length and characters
        var password = PasswordEntry.Text.Trim();
        if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"[0-9]"))
        {
            SignupStatusLabel.Text = "Password must be at least 8 characters long and contain both letters and numbers.";
            SignupStatusLabel.TextColor = Colors.Red;
            return;
        }

        // Validate email suffix
        var email = EmailEntry.Text.Trim();
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            SignupStatusLabel.Text = "Please enter a valid email address.";
            SignupStatusLabel.TextColor = Colors.Red;
            return;
        }

        // Create User object
        var user = new User
        {
            Username = UsernameEntry.Text.Trim(),
            Password = PasswordEntry.Text.Trim(),
            Email = EmailEntry.Text.Trim(),
            FieldOfStudy = FieldOfStudyPicker.SelectedItem.ToString(),
            PermissionsLevel = 4 // Adjust based on your requirements
        };

        // Save user to database
        App.DatabaseService.SaveUser(user);
        SignupStatusLabel.Text = $"User '{user.Username}' signed up successfully.";
        SignupStatusLabel.TextColor = Colors.Green;

        ClearSignupInputs();
    }

    private void ClearSignupInputs()
    {
        UsernameEntry.Text = string.Empty;
        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        FieldOfStudyPicker.SelectedItem = null;
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new LoginPage();
    }
}