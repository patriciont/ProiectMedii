using BookingApp.Models;
using BookingApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingApp.Views;

public partial class BookingPage : ContentPage
{
    private string _userFieldOfStudy;
    private Room _selectedRoom;

    public BookingPage(string userFieldOfStudy)
    {
        InitializeComponent();
        _userFieldOfStudy = userFieldOfStudy;
        LoadRooms();
    }

    private void LoadRooms()
    {
        var rooms = App.DatabaseService.GetRoomsByFieldOfStudy(_userFieldOfStudy);
        RoomsCollectionView.ItemsSource = rooms;
    }

    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new OpeningPage();
    }
}