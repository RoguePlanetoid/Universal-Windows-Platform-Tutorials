using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace AdaptiveApp
{
    public class Setting
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Value { get; set; }
    }

    public class Library
    {
        public static List<Setting> Settings
        {
            get
            {
                return new List<Setting>()
                {
                    new Setting() { Name = "Display", Icon = "\uE7F8", Value = "display" },
                    new Setting() { Name = "Notifications", Icon = "\uE91C", Value = "notifications" },
                    new Setting() { Name = "Battery", Icon = "\uE8BE", Value = "batterysaver" },
                    new Setting() { Name = "Storage", Icon = "\uE8B7", Value = "storagesense" },
                    new Setting() { Name = "Data", Icon = "\uE774", Value = "datausage" },
                    new Setting() { Name = "Personalisation", Icon = "\uE771", Value = "personalization" },
                    new Setting() { Name = "Privacy", Icon = "\uE1F6", Value = "privacy" },
                    new Setting() { Name = "Developers", Icon = "\uEC7A", Value = "developers" }
                };
            }
        }

        public async void Launch(GridView display)
        {
            string value = ((Setting)(display.SelectedItem)).Value;
            await Launcher.LaunchUriAsync(new Uri($"ms-settings:{value}"));
        }
    }
}