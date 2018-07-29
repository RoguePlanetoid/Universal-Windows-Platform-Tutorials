using AdaptiveCards;
using AdaptiveCards.Rendering.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Cards = AdaptiveCards;
using RenderCards = AdaptiveCards.Rendering.Uwp;

public class AdaptiveItem
{
    public string Title { get; set; }
    public string Body { get; set; }
}

public class Card
{
    private const string auto = "Auto";
    private const string stretch = "Stretch";

    private readonly Random random = new Random((int)DateTime.Now.Ticks);
    private readonly AdaptiveCardRenderer renderer = new AdaptiveCardRenderer();
    private readonly Uri adaptive_card_image =
    new Uri("http://adaptivecards.io/content/adaptive-card-50.png");

    private Cards.AdaptiveCard GetCard(AdaptiveItem item)
    {
        Cards.AdaptiveCard card = new Cards.AdaptiveCard()
        {
            Id = random.Next(1, 100000000).ToString(),
            Body = new List<AdaptiveElement>()
            {
              new Cards.AdaptiveColumnSet()
              {
                    Columns = new List<Cards.AdaptiveColumn>()
                    {
                        new Cards.AdaptiveColumn()
                        {
                            Width = auto,
                            Items = new List<AdaptiveElement>()
                            {
                                new Cards.AdaptiveImage()
                                {
                                    Url = adaptive_card_image
                                }
                            }
                        },
                        new Cards.AdaptiveColumn()
                        {
                            Width = stretch,
                            Items = new List<AdaptiveElement>()
                            {
                                new Cards.AdaptiveTextBlock()
                                {
                                    Text = item.Title,
                                    Size = AdaptiveTextSize.ExtraLarge,
                                    Weight = AdaptiveTextWeight.Bolder,
                                },
                                new Cards.AdaptiveTextBlock()
                                {
                                    Text = item.Body,
                                    Size = AdaptiveTextSize.Medium,
                                    Weight = AdaptiveTextWeight.Lighter
                                }
                            },
                        }
                    }
                }
            }
        };
        return card;
    }

    private RenderCards.AdaptiveCard Convert(Cards.AdaptiveCard source, out string json)
    {
        try
        {
            json = source.ToJson();
            RenderCards.AdaptiveCardParseResult result = RenderCards.AdaptiveCard.FromJsonString(json);
            return result.AdaptiveCard;
        }
        catch (Exception)
        {
            json = null;
            return null;
        }
    }

    private Cards.AdaptiveCard Parse(string json)
    {
        try
        {
            Cards.AdaptiveCardParseResult result = Cards.AdaptiveCard.FromJson(json);
            return result.Card;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private FrameworkElement Render(Cards.AdaptiveCard card, out string json)
    {
        try
        {
            RenderedAdaptiveCard rendered = renderer.RenderAdaptiveCard(Convert(card, out json));
            return rendered.FrameworkElement;
        }
        catch (Exception)
        {
            json = null;
            return null;
        }
    }

    public FrameworkElement Render(AdaptiveItem item, out string json)
    {
        Cards.AdaptiveCard card = GetCard(item);
        return Render(card, out json);
    }

    public FrameworkElement Render(string json)
    {
        Cards.AdaptiveCard card = Parse(json);
        return Render(card, out json);
    }
}

public class Timeline
{
    private const string uri = "https://comentsys.wordpress.com/uwp-adaptive-card";
    private readonly Random random = new Random((int)DateTime.Now.Ticks);
    private readonly UserActivityChannel channel = UserActivityChannel.GetDefault();

    public async void Create(string json, string text)
    {
        string id = random.Next(1, 100000000).ToString();
        UserActivity activity = await channel.GetOrCreateUserActivityAsync(id);
        activity.VisualElements.DisplayText = text;
        activity.VisualElements.Content =
        AdaptiveCardBuilder.CreateAdaptiveCardFromJson(json);
        activity.ActivationUri = new Uri(uri);
        activity.FallbackUri = new Uri(uri);
        await activity.SaveAsync();
        UserActivitySession session = activity.CreateSession();
        session?.Dispose();
    }
}

public class Library
{
    private const string app_title = "Adaptive Card";
    private const string extension_json = ".json";
    private const string extension_png = ".png";

    private static readonly Card card = new Card();
    private static readonly Timeline timeline = new Timeline();

    private async Task<string> OpenAsync()
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add(extension_json);
            StorageFile open = await picker.PickSingleFileAsync();
            if (open != null)
            {
                return await FileIO.ReadTextAsync(open);
            }
        }
        finally
        {
        }
        return null;
    }

    private async void Render(FrameworkElement element, StorageFile file)
    {
        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(
            BitmapEncoder.PngEncoderId, stream);
            RenderTargetBitmap target = new RenderTargetBitmap();
            await target.RenderAsync(element, 0, 0);
            IBuffer buffer = await target.GetPixelsAsync();
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied,
            (uint)target.PixelWidth, (uint)target.PixelHeight, 96.0, 96.0, buffer.ToArray());
            await encoder.FlushAsync();
            target = null;
            buffer = null;
            encoder = null;
        }
    }

    private async void SaveAsync(FrameworkElement element, string json)
    {
        try
        {
            FileSavePicker picker = new FileSavePicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = extension_json,
                SuggestedFileName = "Template"
            };
            picker.FileTypeChoices.Add("Json File", new List<string>() { extension_json });
            picker.FileTypeChoices.Add("Image File", new List<string>() { extension_png });
            StorageFile save = await picker.PickSaveFileAsync();
            if (save != null)
            {
                if (save.FileType == extension_json)
                {
                    await FileIO.WriteTextAsync(save, json);
                }
                else if (save.FileType == extension_png)
                {
                    Render(element, save);
                }
            }
        }
        finally
        {
        }
    }

    public void View(ref TextBox title, ref TextBox body, ref TextBox input, ref Canvas display)
    {
        if (!string.IsNullOrEmpty(title.Text) && !string.IsNullOrEmpty(body.Text))
        {
            display.Children.Clear();
            AdaptiveItem item = new AdaptiveItem() { Title = title.Text, Body = body.Text };
            FrameworkElement element = card.Render(item, out string json);
            if (element != null && json != null)
            {
                input.Text = json;
                display.Children.Add(element);
            }
        }
    }

    public void View(ref TextBox input, ref Canvas display)
    {
        if (!string.IsNullOrEmpty(input.Text))
        {
            string json = input.Text;
            display.Children.Clear();
            FrameworkElement element = card.Render(json);
            if (element != null && json != null)
            {
                input.Text = json;
                display.Children.Add(element);
            }
        }
    }

    public async void Open(TextBox input, Canvas display)
    {
        string json = await OpenAsync();
        if (json != null)
        {
            FrameworkElement element = card.Render(json);
            if (element != null && json != null)
            {
                input.Text = json;
                display.Children.Add(element);
            }
        }
    }

    public void Save(ref TextBox input, ref Canvas display)
    {
        if (!string.IsNullOrEmpty(input.Text) && display.Children.Any())
        {
            string json = input.Text;
            FrameworkElement element = display.Children.FirstOrDefault() as FrameworkElement;
            SaveAsync(element, json);
        }
    }

    public void Add(ref TextBox input, ref Canvas display)
    {
        if (!string.IsNullOrEmpty(input.Text) && display.Children.Any())
        {
            string json = input.Text;
            timeline.Create(json, app_title);
        }
    }
}