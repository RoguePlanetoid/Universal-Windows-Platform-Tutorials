using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string app_title = "Rich Editor";
    private const string file_extension = ".rtf";

    public async Task<bool> ConfirmAsync(string content, string title, string ok, string cancel)
    {
        bool result = false;
        MessageDialog dialog = new MessageDialog(content, title);
        dialog.Commands.Add(new UICommand(ok, new UICommandInvokedHandler((cmd) => result = true)));
        dialog.Commands.Add(new UICommand(cancel, new UICommandInvokedHandler((cmd) => result = false)));
        await dialog.ShowAsync();
        return result;
    }

    private void Focus(ref RichEditBox display)
    {
        display.Focus(FocusState.Keyboard);
    }

    private void Set(ref RichEditBox display, string value)
    {
        display.Document.SetText(TextSetOptions.FormatRtf, value);
        Focus(ref display);
    }

    public string Get(ref RichEditBox display)
    {
        string value = string.Empty;
        display.Document.GetText(TextGetOptions.FormatRtf, out value);
        return value;
    }

    public bool Bold(ref RichEditBox display)
    {
        display.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        Focus(ref display);
        return display.Document.Selection.CharacterFormat.Bold.Equals(FormatEffect.On);
    }

    public bool Italic(ref RichEditBox display)
    {
        display.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        Focus(ref display);
        return display.Document.Selection.CharacterFormat.Italic.Equals(FormatEffect.On);
    }

    public bool Underline(ref RichEditBox display)
    {
        display.Document.Selection.CharacterFormat.Underline =
            display.Document.Selection.CharacterFormat.Underline.Equals(UnderlineType.Single) ?
            UnderlineType.None : UnderlineType.Single;
        display.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        Focus(ref display);
        return display.Document.Selection.CharacterFormat.Underline.Equals(UnderlineType.Single);
    }

    public bool Align(ref RichEditBox display, ParagraphAlignment alignment)
    {
        display.Document.Selection.ParagraphFormat.Alignment = alignment;
        Focus(ref display);
        return display.Document.Selection.ParagraphFormat.Alignment.Equals(alignment);
    }

    public void Size(ref RichEditBox display, ref ComboBox value)
    {
        if (display != null && value != null)
        {
            string selected = ((ComboBoxItem)value.SelectedItem).Tag.ToString();
            display.Document.Selection.CharacterFormat.Size = float.Parse(selected);
            Focus(ref display);
        }
    }

    public void Colour(ref RichEditBox display, ref ComboBox value)
    {
        if (display != null && value != null)
        {
            string selected = ((ComboBoxItem)value.SelectedItem).Tag.ToString();
            display.Document.Selection.CharacterFormat.ForegroundColor = Color.FromArgb(
                Byte.Parse(selected.Substring(0, 2), NumberStyles.HexNumber),
                Byte.Parse(selected.Substring(2, 2), NumberStyles.HexNumber),
                Byte.Parse(selected.Substring(4, 2), NumberStyles.HexNumber),
                Byte.Parse(selected.Substring(6, 2), NumberStyles.HexNumber));
            Focus(ref display);
        }
    }

    public async void NewAsync(RichEditBox display)
    {
        if (await ConfirmAsync("Create New Document?", app_title, "Yes", "No"))
        {
            Set(ref display, string.Empty);
        }
    }

    public async void OpenAsync(RichEditBox display)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(file_extension);
            StorageFile file = await picker.PickSingleFileAsync();
            Set(ref display, await FileIO.ReadTextAsync(file));
        }
        catch
        {

        }
    }

    public async void SaveAsync(RichEditBox display)
    {
        try
        {
            FileSavePicker picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = file_extension,
                SuggestedFileName = "Document"
            };
            picker.FileTypeChoices.Add("Rich Text", new List<string>() { file_extension });
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, Get(ref display));
            }
        }
        catch
        {

        }
    }
}