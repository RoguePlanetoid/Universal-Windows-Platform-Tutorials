using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

public class Library
{
    private const string app_title = "PdfView App";

    private PdfDocument _document = null;

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    public async Task<uint> OpenAsync()
    {
        uint pages = 0;
        try
        {
            _document = null;
            FileOpenPicker picker = new FileOpenPicker()
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".pdf");
            StorageFile open = await picker.PickSingleFileAsync();
            if (open != null)
            {
                _document = await PdfDocument.LoadFromFileAsync(open);
            }
            if (_document != null)
            {
                if (_document.IsPasswordProtected)
                {
                    Show("Password Protected PDF Document", app_title);
                }
                pages = _document.PageCount;
            }
        }
        catch (Exception ex)
        {
            if (ex.HResult == unchecked((int)0x80004005))
            {
                Show("Invalid PDF Document", app_title);
            }
            else
            {
                Show(ex.Message, app_title);
            }
        }
        return pages;
    }

    public async Task<BitmapImage> ViewAsync(uint number)
    {
        BitmapImage source = new BitmapImage();
        if (!(number < 1 || number > _document.PageCount))
        {
            uint index = number - 1;
            using (PdfPage page = _document.GetPage(index))
            {
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream);
                    await source.SetSourceAsync(stream);
                }
            }
        }
        return source;
    }

    public List<int> Numbers(int total)
    {
        return Enumerable.Range(1, total).ToList();
    }
}