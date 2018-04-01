using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string app_title = "Draw Editor";
    private const string file_extension = ".drw";

    private string ToString(Color value)
    {
        return $"{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
    }

    private Color FromString(string value)
    {
        return Color.FromArgb(
        Byte.Parse(value.Substring(0, 2), NumberStyles.HexNumber),
        Byte.Parse(value.Substring(2, 2), NumberStyles.HexNumber),
        Byte.Parse(value.Substring(4, 2), NumberStyles.HexNumber),
        Byte.Parse(value.Substring(6, 2), NumberStyles.HexNumber));
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

    public void Init(ref InkCanvas display, ref ComboBox size, ref ComboBox colour)
    {
        string selectedSize = ((ComboBoxItem)size.SelectedItem).Tag.ToString();
        string selectedColour = ((ComboBoxItem)colour.SelectedItem).Tag.ToString();
        InkDrawingAttributes attributes = new InkDrawingAttributes
        {
            Color = FromString(selectedColour),
            Size = new Size(int.Parse(selectedSize), int.Parse(selectedSize)),
            IgnorePressure = false,
            FitToCurve = true
        };
        display.InkPresenter.UpdateDefaultDrawingAttributes(attributes);
        display.InkPresenter.InputDeviceTypes =
           CoreInputDeviceTypes.Mouse |
           CoreInputDeviceTypes.Pen |
           CoreInputDeviceTypes.Touch;
    }

    public void Colour(ref InkCanvas display, ref ComboBox colour)
    {
        if (display != null)
        {
            string selectedColour = ((ComboBoxItem)colour.SelectedItem).Tag.ToString();
            InkDrawingAttributes attributes = display.InkPresenter.CopyDefaultDrawingAttributes();
            attributes.Color = FromString(selectedColour);
            display.InkPresenter.UpdateDefaultDrawingAttributes(attributes);
        }
    }

    public void Size(ref InkCanvas display, ref ComboBox size)
    {
        if (display != null)
        {
            string selectedSize = ((ComboBoxItem)size.SelectedItem).Tag.ToString();
            InkDrawingAttributes attributes = display.InkPresenter.CopyDefaultDrawingAttributes();
            attributes.Size = new Size(int.Parse(selectedSize), int.Parse(selectedSize));
            display.InkPresenter.UpdateDefaultDrawingAttributes(attributes);
        }
    }

    public async void New(InkCanvas display)
    {
        if (await ConfirmAsync("Create New Drawing?", app_title, "Yes", "No"))
        {
            display.InkPresenter.StrokeContainer.Clear();
        }
    }

    public async void OpenAsync(InkCanvas display)
    {
        try
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(file_extension);
            StorageFile file = await picker.PickSingleFileAsync();
            using (IInputStream stream = await file.OpenSequentialReadAsync())
            {
                await display.InkPresenter.StrokeContainer.LoadAsync(stream);
            }
        }
        catch
        {

        }
    }

    public async void SaveAsync(InkCanvas display)
    {
        try
        {
            FileSavePicker picker = new FileSavePicker
            {
                DefaultFileExtension = file_extension,
                SuggestedFileName = "Drawing",
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeChoices.Add("Drawing", new List<string>() { file_extension });
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await display.InkPresenter.StrokeContainer.SaveAsync(stream);
                }
            }
        }
        catch
        {

        }
    }
}