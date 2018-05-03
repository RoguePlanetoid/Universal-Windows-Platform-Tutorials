using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Compression;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string app_title = "Compression App";
    private const string text_file_extension = ".txt";
    private const string compressed_file_extension = ".compressed";
    private readonly CompressAlgorithm compression_algorithm = CompressAlgorithm.Lzms;

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    public async Task<bool> ConfirmAsync(string content, string title, string ok, string cancel)
    {
        bool result = false;
        MessageDialog dialog = new MessageDialog(content, title);
        dialog.Commands.Add(new UICommand(ok, new UICommandInvokedHandler((cmd) => result = true)));
        dialog.Commands.Add(new UICommand(cancel, new UICommandInvokedHandler((cmd) => result = false)));
        await dialog.ShowAsync();
        return result;
    }

    public async void NewAsync(TextBox display)
    {
        if (await ConfirmAsync("Create New?", app_title, "Yes", "No"))
        {
            display.Text = string.Empty;
        }
    }

    public async void OpenAsync(TextBox display)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(text_file_extension);
            picker.FileTypeFilter.Add(compressed_file_extension);
            StorageFile file = await picker.PickSingleFileAsync();
            switch (file.FileType)
            {
                case text_file_extension:
                    display.Text = await FileIO.ReadTextAsync(file);
                    break;
                case compressed_file_extension:
                    using (MemoryStream stream = new MemoryStream())
                    using (IInputStream input = await file.OpenSequentialReadAsync())
                    using (Decompressor decompressor = new Decompressor(input))
                    using (IRandomAccessStream output = stream.AsRandomAccessStream())
                    {
                        long inputSize = input.AsStreamForRead().Length;
                        ulong outputSize = await RandomAccessStream.CopyAsync(decompressor, output);
                        output.Seek(0);
                        display.Text = await new StreamReader(output.AsStream()).ReadToEndAsync();
                        Show($"Decompressed {inputSize} bytes to {outputSize} bytes", app_title);
                    }
                    break;
                default:
                    break;
            }
        }
        catch
        {

        }
    }

    public async void SaveAsync(TextBox display)
    {
        try
        {
            FileSavePicker picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeChoices.Add("Text File", new List<string>() { text_file_extension });
            picker.FileTypeChoices.Add("Compressed File", new List<string>() { compressed_file_extension });
            picker.DefaultFileExtension = text_file_extension;
            StorageFile file = await picker.PickSaveFileAsync();
            switch (file.FileType)
            {
                case text_file_extension:
                    await FileIO.WriteTextAsync(file, display.Text);
                    break;
                case compressed_file_extension:
                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(display.Text)))
                    using (IRandomAccessStream input = stream.AsRandomAccessStream())
                    using (IRandomAccessStream output = await file.OpenAsync(FileAccessMode.ReadWrite))
                    using (Compressor compressor = new Compressor(output.GetOutputStreamAt(0), compression_algorithm, 0))
                    {
                        ulong inputSize = await RandomAccessStream.CopyAsync(input, compressor);
                        bool finished = await compressor.FinishAsync();
                        ulong outputSize = output.Size;
                        Show($"Compressed {inputSize} bytes to {outputSize} bytes", app_title);
                    }
                    break;
                default:
                    break;
            }
        }
        catch
        {

        }
    }

    public void Sample(ref TextBox display)
    {
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            text.AppendLine("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris non massa diam. " +
            "Nunc luctus non lorem id imperdiet. Nunc quis mi nec enim malesuada commodo mollis eget nisl. " +
            "Sed vulputate in purus eu vulputate. Quisque commodo eu odio et malesuada. Duis porttitor, " +
            "lectus ut egestas placerat, purus nisi elementum diam, congue lacinia erat lectus sit amet felis. " +
            "Proin suscipit lobortis bibendum. Aliquam erat volutpat. Nunc vitae nulla nunc.\n");
        }
        display.Text = text.ToString();
    }
}