using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace BookingApp.Styles
{
    // LABEL STYLES

    // Room Label
    public class RoomLabel : Label
    {
        public RoomLabel()
        {
            TextColor = Colors.Black;
            FontSize = 15;
        }
    }

    // Expander Label
    public class ExpanderLabel : Label
    {
        public ExpanderLabel()
        {
            FontSize = 15;
            WidthRequest = 250;
            FontAttributes = FontAttributes.Bold;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            BackgroundColor = Colors.Green;
            TextColor = Colors.White;
            Padding = new Thickness(10);
            Margin = new Thickness(5);
            HorizontalTextAlignment = TextAlignment.Center;
            VerticalTextAlignment = TextAlignment.Center;
        }
    }

    // BUTTON STYLES

    // Main Button
    public class MainButton : Button
    {
        public MainButton()
        {
            BackgroundColor = Colors.Green;
            TextColor = Colors.White;
            WidthRequest = 110;
            HeightRequest = 35;
            Padding = new Thickness(10);
        }

    }

    public class RoomSelectButton : Button
    {
        public RoomSelectButton()
        {
            BackgroundColor = Colors.Green;
            TextColor = Colors.White;
            WidthRequest = 110;
            HeightRequest = 35;
            Padding = new Thickness(10);
        }

    }


}
