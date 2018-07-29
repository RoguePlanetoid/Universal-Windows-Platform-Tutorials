using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

public class TimelineItem
{
    public string Id { get; set; }
    public Color Colour { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}

public class Library
{
    private const string app_title = "Timeline";
    private const string primary_text = "Create";
    private const string secondary_text = "Cancel";
    private const string timeline_uri = "https://comentsys.wordpress.com/uwp-timeline";
    private readonly Random random = new Random((int)DateTime.Now.Ticks);

    private ColorPicker _picker;
    private IAsyncOperation<ContentDialogResult> _dialogResult;
    private UserActivityChannel _channel = UserActivityChannel.GetDefault();
    private ObservableCollection<TimelineItem> _list = new ObservableCollection<TimelineItem>();

    private async Task<ContentDialogResult> ShowDialogAsync(object content, string title = app_title)
    {
        try
        {
            if (_dialogResult != null)
            {
                _dialogResult.Cancel();
                _dialogResult = null;
            }
            _dialogResult = new ContentDialog()
            {
                Title = app_title,
                Content = content,
                PrimaryButtonText = primary_text,
                SecondaryButtonText = secondary_text
            }.ShowAsync();
            return await _dialogResult;
        }
        catch (TaskCanceledException)
        {
            return ContentDialogResult.None;
        }
    }

    private async Task<Color?> ShowColour()
    {
        _picker = new ColorPicker()
        {
            IsColorChannelTextInputVisible = true,
            IsAlphaSliderVisible = false,
            IsColorSliderVisible = false,
            IsHexInputVisible = true,
            IsAlphaEnabled = false,
        };
        if (await ShowDialogAsync(_picker) == ContentDialogResult.Primary)
        {
            return _picker.Color;
        }
        return null;
    }

    private async Task<string> Create(string title, string body, Color background)
    {
        string id = random.Next(1, 100000000).ToString();
        UserActivity activity = await _channel.GetOrCreateUserActivityAsync(id);
        activity.VisualElements.DisplayText = title;
        activity.ActivationUri = new Uri(timeline_uri);
        activity.FallbackUri = new Uri(timeline_uri);
        activity.VisualElements.BackgroundColor = background;
        activity.VisualElements.Description = body;
        await activity.SaveAsync();
        UserActivitySession session = activity.CreateSession();
        session?.Dispose();
        return id;
    }

    public async void Init(ListBox display)
    {
        _list.Clear();
        IList<UserActivitySessionHistoryItem> list = await
            _channel.GetRecentUserActivitiesAsync(maxUniqueActivities: 25);
        foreach (UserActivitySessionHistoryItem item in list)
        {
            _list.Add(new TimelineItem()
            {
                Id = item.UserActivity.ActivityId,
                Title = item.UserActivity.VisualElements.DisplayText,
                Body = item.UserActivity.VisualElements.Description,
                Colour = item.UserActivity.VisualElements.BackgroundColor
            });
        }
        display.ItemsSource = _list;
    }

    public async void Add(ListBox display, TextBox title, TextBox body)
    {
        Color? result = await ShowColour();
        if (result != null)
        {
            string id = await Create(title.Text, body.Text, result.Value);
            _list.Add(new TimelineItem
            {
                Id = id,
                Title = title.Text,
                Body = body.Text,
                Colour = result.Value
            });
        }
    }

    public async void Remove(ListBox display, AppBarButton button)
    {
        TimelineItem item = (TimelineItem)button.Tag;
        if (_channel.GetOrCreateUserActivityAsync(item.Id) != null)
        {
            await _channel.DeleteActivityAsync(item.Id);
        }
        _list.Remove(item);
    }
}