using Microsoft.Maui.Controls;
using BookingApp.Models;
using System;

namespace BookingApp.Views;

public partial class BookingPage : ContentPage
{
	public BookingPage()
	{
		InitializeComponent();
	}

    private void OnAddAdminClicked(object sender, EventArgs e)
    {
        var admin = new Admin
        {
            Name = "Professor John",
            Email = "john@example.com",
            PhoneNumber = "123456789"
        };

        App.DatabaseService.SaveAdmin(admin);
        StatusLabel.Text = "Admin added.";
    }

    private void OnAddRoomClicked(object sender, EventArgs e)
    {
        var admin = App.DatabaseService.GetAdmin(1); // Assuming an Admin with ID 1 exists

        if (admin != null)
        {
            var room = new Room
            {
                FieldOfStudy = "Computer Science",
                Description = "Room 101",
                AdminId = admin.Id,
                Capacity = 10
            };

            App.DatabaseService.SaveRoom(room);
            StatusLabel.Text = "Room added.";
        }
        else
        {
            StatusLabel.Text = "Admin not found.";
        }
    }

    private void OnAddSlotClicked(object sender, EventArgs e)
    {
        var room = App.DatabaseService.GetRoom(1); // Assuming a Room with ID 1 exists

        if (room != null)
        {
            var slot = new RoomSlot
            {
                RoomId = room.Id,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };

            App.DatabaseService.SaveRoomSlot(slot);
            StatusLabel.Text = "Slot added.";
        }
        else
        {
            StatusLabel.Text = "Room not found.";
        }
    }

    private void OnListRoomsClicked(object sender, EventArgs e)
    {
        var rooms = App.DatabaseService.GetAllRooms();
        StatusLabel.Text = $"Rooms count: {rooms.Count}";

        foreach (var room in rooms)
        {
            StatusLabel.Text += $"\nRoom: {room.Description}, Field: {room.FieldOfStudy}";
        }
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new OpeningPage();
    }
}