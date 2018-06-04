using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;

public class Library
{
    public async void Open(BitmapImage display)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".gif");
            StorageFile open = await picker.PickSingleFileAsync();
            if (open != null)
            {
                display.AutoPlay = false;
                await display.SetSourceAsync(await open.OpenReadAsync());
            }
        }
        finally
        {
            // Ignore Exceptions
        }
    }
}