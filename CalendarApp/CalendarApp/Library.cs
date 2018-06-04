using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public class Item
{
    public string Id { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public string When { get { return StartTime.ToString("dd MMMM yyyy HH:mm:ss"); } }
    public string Subject { get; set; }
    public string Details { get; set; }
}

public class Library
{
    private const string app_title = "Calendar App";

    private async Task<AppointmentStore> Store()
    {
        return await AppointmentManager.RequestStoreAsync(AppointmentStoreAccessType.AppCalendarsReadWrite);
    }

    private DateTime GetDateTime(DateTimeOffset date, TimeSpan time)
    {
        return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
    }

    private async Task<Appointment> Dialog(Appointment appointment, ResourceDictionary resources)
    {
        Thickness margin = new Thickness(5);
        DatePicker date = new DatePicker()
        {
            Date = appointment.StartTime,
            Margin = margin,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        TimePicker time = new TimePicker()
        {
            Time = appointment.StartTime.TimeOfDay,
            Margin = margin,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        TextBox subject = new TextBox()
        {
            Text = appointment.Subject,
            Margin = margin,
            PlaceholderText = "Subject"
        };
        TextBox details = new TextBox()
        {
            Text = appointment.Details,
            Margin = margin,
            PlaceholderText = "Details",
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            Height = (double)resources["SearchBoxSuggestionPopupThemeMaxHeight"]
        };
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Vertical
        };
        panel.Children.Add(date);
        panel.Children.Add(time);
        panel.Children.Add(subject);
        panel.Children.Add(details);
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Appointment",
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            Content = panel
        };
        ContentDialogResult result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            appointment.StartTime = GetDateTime(date.Date, time.Time);
            appointment.Subject = subject.Text;
            appointment.Details = details.Text;
            return appointment;
        }
        return null;
    }

    private async Task<AppointmentCalendar> GetAsync()
    {
        AppointmentCalendar result = null;
        AppointmentStore store = await Store();
        if (store != null)
        {
            IReadOnlyList<AppointmentCalendar> list = await store.FindAppointmentCalendarsAsync();
            if (list.Count == 0)
            {
                result = await store.CreateAppointmentCalendarAsync(app_title);
            }
            else
            {
                result = list.FirstOrDefault(s => s.DisplayName == app_title);
            }
        }
        return result;
    }

    private async Task<IReadOnlyList<Appointment>> ListAppointmentsAsync(DateTimeOffset start, TimeSpan range)
    {
        AppointmentStore store = await Store();
        if (store != null)
        {
            AppointmentCalendar calendar = await GetAsync();
            if (calendar != null)
            {
                return await calendar.FindAppointmentsAsync(start, range);
            }
        }
        return null;
    }

    private async Task<Appointment> GetAppointmentAsync(string id)
    {
        AppointmentCalendar calendar = await GetAsync();
        if (calendar != null)
        {
            return await calendar.GetAppointmentAsync(id);
        }
        return null;
    }

    public void Start(ref CalendarDatePicker picker)
    {
        DateTime now = DateTime.Now;
        if (picker.Date == null)
        {
            picker.Date = GetDateTime(DateTime.Now, new TimeSpan(0, 0, 0));
        }
    }

    public async Task<List<Item>> ListAsync(DateTimeOffset start)
    {
        start = start.AddDays(-1);
        DateTimeOffset finish = start.AddMonths(1);
        TimeSpan range = finish - start;
        List<Item> results = new List<Item>();
        IReadOnlyList<Appointment> appointments = await ListAppointmentsAsync(start, range);
        foreach (Appointment appointment in appointments)
        {
            results.Add(new Item()
            {
                Id = appointment.LocalId,
                StartTime = appointment.StartTime,
                Subject = appointment.Subject,
                Details = appointment.Details
            });
        }
        return results;
    }

    public async Task<bool> AddAsync(AppBarButton button, ResourceDictionary resources)
    {
        Appointment appointment = await Dialog(new Appointment(), resources);
        if (appointment != null)
        {
            AppointmentCalendar calendar = await GetAsync();
            if (calendar != null)
            {
                await calendar.SaveAppointmentAsync(appointment);
                return true;
            }
        }
        return false;
    }

    public async Task<bool> EditAsync(AppBarButton button, ResourceDictionary resources)
    {
        Item item = (Item)button.Tag;
        Appointment appointment = await GetAppointmentAsync(item.Id);
        if (appointment != null)
        {
            appointment = await Dialog(appointment, resources);
            if (appointment != null)
            {
                AppointmentCalendar calendar = await GetAsync();
                if (calendar != null)
                {
                    await calendar.SaveAppointmentAsync(appointment);
                    return true;
                }
            }
        }
        return false;
    }

    public async Task<bool> DeleteAsync(AppBarButton button)
    {
        Item item = (Item)button.Tag;
        Appointment appointment = await GetAppointmentAsync(item.Id);
        if (appointment != null)
        {
            AppointmentCalendar calendar = await GetAsync();
            if (calendar != null)
            {
                await calendar.DeleteAppointmentAsync(item.Id);
                return true;
            }
        }
        return false;
    }
}