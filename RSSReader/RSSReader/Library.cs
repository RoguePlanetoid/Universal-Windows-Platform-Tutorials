using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Web.Syndication;

public class Library
{
    SyndicationClient _client = new SyndicationClient();
    SyndicationFeed _feed = new SyndicationFeed();

    private async void Load(ItemsControl list, Uri uri)
    {
        _client = new SyndicationClient();
        _feed = await _client.RetrieveFeedAsync(uri);
        list.ItemsSource = _feed.Items;
    }

    public void Go(ref ItemsControl list, string value, KeyRoutedEventArgs args)
    {
        if (args.Key == Windows.System.VirtualKey.Enter)
        {
            try
            {
                Load(list, new Uri(value));
                list.Focus(FocusState.Keyboard);
            }
            catch
            {

            }
        }
    }
}