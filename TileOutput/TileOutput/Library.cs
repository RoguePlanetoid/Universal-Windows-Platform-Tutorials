using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;

public class Item
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string Time { get; set; }
}

public class Library
{
    private Random _random = new Random((int)DateTime.Now.Ticks);
    private TileUpdater _updater = TileUpdateManager.CreateTileUpdaterForApplication();

    public void Init(ListBox display)
    {
        display.Items.Clear();
        IReadOnlyList<ScheduledTileNotification> list = _updater.GetScheduledTileNotifications();
        foreach (ScheduledTileNotification item in list)
        {
            display.Items.Add(new Item
            {
                Id = item.Id,
                Time = item.Content.GetElementsByTagName("text")[1].InnerText,
                Content = item.Content.GetElementsByTagName("text")[0].InnerText,
            });
        }
    }

    public void Add(ref ListBox display, string value, TimeSpan occurs)
    {
        DateTime when = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            occurs.Hours, occurs.Minutes, occurs.Seconds);
        if (when > DateTime.Now)
        {
            XmlDocument xml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text03);
            xml.GetElementsByTagName("text")[0].InnerText = value;
            xml.GetElementsByTagName("text")[1].InnerText = when.ToString("HH:mm");
            ScheduledTileNotification tile = new ScheduledTileNotification(xml, when)
            {
                Id = _random.Next(1, 100000000).ToString()
            };
            _updater.AddToSchedule(tile);
            display.Items.Add(new Item { Id = tile.Id, Content = value, Time = when.ToString() });
        }
    }

    public void Remove(ListBox display)
    {
        if (display.SelectedIndex > -1)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().GetScheduledTileNotifications().Where(
                p => p.Id.Equals(((Item)display.SelectedItem).Id)).SingleOrDefault();
            display.Items.RemoveAt(display.SelectedIndex);
        }
    }
}