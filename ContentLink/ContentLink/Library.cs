using System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

public class Library
{
    public delegate void NavigatedHandler(Uri uri);
    public event NavigatedHandler ContextLinkNavigated;

    private void ContentLinkInvoked(RichEditBox sender, ContentLinkInvokedEventArgs args)
    {
        ContextLinkNavigated?.Invoke(args.ContentLinkInfo.Uri);
        args.Handled = true;
    }

    public void Init(ref RichEditBox input)
    {
        input.ContentLinkInvoked += ContentLinkInvoked;
        input.ContentLinkBackgroundColor = new SolidColorBrush(Colors.Transparent);
        input.ContentLinkForegroundColor = new SolidColorBrush(Colors.Blue);
        input.ContentLinkProviders = new ContentLinkProviderCollection
        {
            new PlaceContentLinkProvider()
        };
    }

    public void New(ref RichEditBox input, ref WebView display)
    {
        input.Document.SetText(TextSetOptions.FormatRtf, string.Empty);
        display.NavigateToString(string.Empty);
    }
}