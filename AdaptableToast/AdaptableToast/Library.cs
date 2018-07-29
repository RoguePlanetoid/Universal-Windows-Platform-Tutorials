using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

public class AdaptableItem
{
    private const string key_id = "id";
    private const string key_title = "title";
    private const string key_body = "body";

    private string CreateQueryString(Dictionary<string, string> source)
    {
        string[] array = source.Select(kv =>
        $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}").ToArray();
        return $"?{string.Join("&", array)}";
    }

    private Dictionary<string, string> ParseQueryString(string query)
    {
        NameValueCollection value = HttpUtility.ParseQueryString(query);
        return value.AllKeys.ToDictionary(x => HttpUtility.UrlDecode(x),
        x => HttpUtility.UrlDecode(value[x]));
    }

    public string Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }

    public AdaptableItem() { }

    public AdaptableItem(string value)
    {
        Dictionary<string, string> dict = ParseQueryString(value);
        Id = dict[key_id];
        Title = dict[key_title];
        Body = dict[key_body];
    }

    public string Create()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>()
        {
            { key_id, this.Id },
            { key_title, this.Title },
            { key_body, this.Body }
        };
        return CreateQueryString(dict);
    }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, Body: {Body}";
    }
}

public class Toast
{
    private readonly Random random = new Random((int)DateTime.Now.Ticks);

    private ToastNotification GetNotification(AdaptableItem item)
    {
        ToastContent content = new ToastContent()
        {
            Launch = item.Create(),
            Visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = "ms-appx:///Assets/StoreLogo.png"
                    },
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = item.Title,
                            HintStyle = AdaptiveTextStyle.Body
                        },
                        new AdaptiveText()
                        {
                            Text = item.Body,
                            HintWrap = true,
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        }
                    }
                }
            }
        };
        return new ToastNotification(content.GetXml());
    }

    public AdaptableItem Show(string title, string body)
    {
        string id = random.Next(1, 100000000).ToString();
        AdaptableItem item = new AdaptableItem() { Id = id, Title = title, Body = body };
        ToastNotification notification = GetNotification(item);
        ToastNotificationManager.CreateToastNotifier().Show(notification);
        return item;
    }
}

public static class Library
{
    private const string app_title = "Adaptable Toast";
    private static readonly Toast toast = new Toast();

    private static IAsyncOperation<IUICommand> _dialogCommand;

    private static async Task<bool> ShowDialogAsync(string content, string title = app_title)
    {
        try
        {
            if (_dialogCommand != null)
            {
                _dialogCommand.Cancel();
                _dialogCommand = null;
            }
            _dialogCommand = new MessageDialog(content, title).ShowAsync();
            await _dialogCommand;
            return true;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    public static async void Activated(ToastNotificationActivatedEventArgs args)
    {
        if (args != null)
        {
            string argument = args.Argument;
            await ShowDialogAsync($"Selected - {new AdaptableItem(argument)}");
        }
    }

    public static void Add(TextBox title, TextBox desc)
    {
        toast.Show(title.Text, desc.Text);
    }
}