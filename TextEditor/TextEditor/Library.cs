using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string app_title = "Text Editor";
    private const string file_extension = ".txt";

    private async Task<bool> ConfirmAsync(string content, string title, string ok, string cancel)
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
        if (await ConfirmAsync("Create New Document?", app_title, "Yes", "No"))
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
            picker.FileTypeFilter.Add(file_extension);
            StorageFile file = await picker.PickSingleFileAsync();
            display.Text = await FileIO.ReadTextAsync(file);
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
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = file_extension,
                SuggestedFileName = "Document"
            };
            picker.FileTypeChoices.Add("Plain Text", new List<string>() { file_extension });
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, display.Text);
            }
        }
        catch
        {

        }
    }
}