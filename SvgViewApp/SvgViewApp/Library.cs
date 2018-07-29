using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

public class Library
{
    public async void Open(Image display)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".svg");
            StorageFile open = await picker.PickSingleFileAsync();
            if (open != null)
            {
                SvgImageSource source = new SvgImageSource()
                {
                    RasterizePixelHeight = display.ActualHeight,
                    RasterizePixelWidth = display.ActualWidth
                };
                if (await source.SetSourceAsync(await open.OpenReadAsync())
                    == SvgImageSourceLoadStatus.Success)
                {
                    display.Source = source;
                }
            }
        }
        finally
        {
            // Ignore Exceptions
        }
    }
}