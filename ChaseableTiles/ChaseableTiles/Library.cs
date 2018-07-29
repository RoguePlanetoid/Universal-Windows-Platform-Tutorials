using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Controls;

public class ChaseableItem
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

    public ChaseableItem() { }

    public ChaseableItem(string value)
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

public class Tile
{
    private readonly Random random = new Random((int)DateTime.Now.Ticks);

    private TileNotification GetNotification(ChaseableItem item)
    {
        TileContent content = new TileContent()
        {
            Visual = new TileVisual()
            {
                Arguments = item.Create(),
                Branding = TileBranding.NameAndLogo,
                TileMedium = new TileBinding()
                {
                    Content = new TileBindingContentAdaptive()
                    {
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
            }
        };
        return new TileNotification(content.GetXml());
    }

    public ChaseableItem Update(string title, string body)
    {
        string id = random.Next(1, 100000000).ToString();
        ChaseableItem item = new ChaseableItem() { Id = id, Title = title, Body = body };
        TileNotification notification = GetNotification(item);
        TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        return item;
    }
}

public static class Library
{
    private const string app_title = "Chaseable Tiles";
    private static readonly Tile tile = new Tile();

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

    public static async void Launched(LaunchActivatedEventArgs args)
    {
        if (args.TileActivatedInfo != null)
        {
            string argument = args.TileActivatedInfo.RecentlyShownNotifications
            .Select(s => s.Arguments).FirstOrDefault();
            await ShowDialogAsync($"Selected - {new ChaseableItem(argument)}");
        }
    }

    public static async void Add(TextBox title, TextBox desc)
    {
        await ShowDialogAsync($"Added - {tile.Update(title.Text, desc.Text)}");
    }

    public static async Task<bool> PinAsync()
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
}