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
    private ToastNotifier _notifier = ToastNotificationManager.CreateToastNotifier();
    private Random _random = new Random((int)DateTime.Now.Ticks);

    public void Init(ListBox display)
    {
        display.Items.Clear();
        IReadOnlyList<ScheduledToastNotification> list = _notifier.GetScheduledToastNotifications();
        foreach (ScheduledToastNotification item in list)
        {
            display.Items.Add(new Item
            {
                Id = item.Id,
                Time = item.Content.GetElementsByTagName("text")[0].InnerText,
                Content = item.Content.GetElementsByTagName("text")[1].InnerText,            
            });
        }
    }

    public void Add(ref ListBox display, string value, TimeSpan occurs)
    {
        DateTime when = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
            occurs.Hours, occurs.Minutes, occurs.Seconds);
        if (when > DateTime.Now)
        {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            xml.GetElementsByTagName("text")[0].InnerText = when.ToLocalTime().ToString();
            xml.GetElementsByTagName("text")[1].InnerText = value;
            ScheduledToastNotification toast = new ScheduledToastNotification(xml, when)
            {
                Id = _random.Next(1, 100000000).ToString()
            };
            _notifier.AddToSchedule(toast);
            display.Items.Add(new Item { Id = toast.Id, Content = value, Time = when.ToString() });
        }
    }

    public void Remove(ListBox display)
    {
        if (display.SelectedIndex > -1)
        {
            _notifier = ToastNotificationManager.CreateToastNotifier();
            try
            {
                _notifier.RemoveFromSchedule(_notifier.GetScheduledToastNotifications().Where(
                    p => p.Id.Equals(((Item)display.SelectedItem).Id)).SingleOrDefault());
            }
            catch(Exception)
            {

            }
            display.Items.RemoveAt(display.SelectedIndex);
        }
    }
}