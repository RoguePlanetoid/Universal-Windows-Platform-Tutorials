using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public class Library
{
    private readonly List<string> glyphs = new List<string>
    {
        "number", "activity", "alarm", "attention", "available", "away",
        "busy", "error", "newMessage", "paused", "playing", "unavailable"
    };

    private async Task<string> DialogAsync()
    {
        ComboBox glyph = new ComboBox()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(5),
            ItemsSource = glyphs
        };
        glyph.SelectedIndex = 0;
        TextBox number = new TextBox()
        {
            PlaceholderText = "Number",
            Margin = new Thickness(5)
        };
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Vertical
        };
        panel.Children.Add(glyph);
        panel.Children.Add(number);
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Badges",
            PrimaryButtonText = "Update",
            CloseButtonText = "Cancel",
            Content = panel
        };
        ContentDialogResult result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            string selected = (string)glyph.SelectedItem;
            return (selected == "number") ? number.Text : selected;
        }
        return null;
    }

    private void UpdateBadge(string value)
    {
        bool isNumber = int.TryParse(value, out int number);
        XmlDocument badge = BadgeUpdateManager.GetTemplateContent(isNumber ?
            BadgeTemplateType.BadgeNumber : BadgeTemplateType.BadgeGlyph);
        XmlNodeList attributes = badge.GetElementsByTagName("badge");
        IXmlNode node = attributes[0].Attributes.GetNamedItem("value");
        node.NodeValue = value;
        BadgeNotification notification = new BadgeNotification(badge);
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(notification);
    }

    public async Task<bool> PinAsync()
    {
        bool isPinned = false;
        AppListEntry entry = (await Package.Current.GetAppListEntriesAsync()).FirstOrDefault();
        if (entry != null)
        {
            isPinned = await StartScreenManager.GetDefault().ContainsAppListEntryAsync(entry);
        }
        if (!isPinned)
        {
            isPinned = await StartScreenManager.GetDefault().RequestAddAppListEntryAsync(entry);
        }
        return isPinned;
    }

    public async void Badge()
    {
        string result = await DialogAsync();
        if (result != null)
        {
            UpdateBadge(result);
        }
    }
}