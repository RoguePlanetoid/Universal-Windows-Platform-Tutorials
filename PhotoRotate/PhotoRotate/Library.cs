using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

public class Library
{
    private int _angle;
    private StorageFile _file;
    private WriteableBitmap _bitmap;

    private readonly Dictionary<int, BitmapRotation> rotation_angles =
        new Dictionary<int, BitmapRotation>()
    {
        { 0, BitmapRotation.None },
        { 90,  BitmapRotation.Clockwise90Degrees },
        { 180,  BitmapRotation.Clockwise180Degrees },
        { 270, BitmapRotation.Clockwise270Degrees },
        { 360, BitmapRotation.None }
    };
    private const string file_extension = ".jpg";

    private async Task<WriteableBitmap> ReadAsync()
    {
        using (IRandomAccessStream stream = await _file.OpenAsync(FileAccessMode.ReadWrite))
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, stream);
            uint width = decoder.PixelWidth;
            uint height = decoder.PixelHeight;
            if (_angle % 180 != 0)
            {
                width = decoder.PixelHeight;
                height = decoder.PixelWidth;
            }
            BitmapTransform transform = new BitmapTransform
            {
                Rotation = rotation_angles[_angle]
            };
            PixelDataProvider data = await decoder.GetPixelDataAsync(
            BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, transform,
            ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
            _bitmap = new WriteableBitmap((int)width, (int)height);
            byte[] buffer = data.DetachPixelData();
            using (Stream pixels = _bitmap.PixelBuffer.AsStream())
            {
                pixels.Write(buffer, 0, (int)pixels.Length);
            }
        }
        return _bitmap;
    }

    private async void WriteAsync()
    {
        using (IRandomAccessStream stream = await _file.OpenAsync(FileAccessMode.ReadWrite))
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
            (uint)_bitmap.PixelWidth, (uint)_bitmap.PixelHeight, 96.0, 96.0, _bitmap.PixelBuffer.ToArray());
            await encoder.FlushAsync();
        }
    }

    public async void OpenAsync(Image display)
    {
        _angle = 0;
        try
        {
            FileOpenPicker picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(file_extension);
            _file = await picker.PickSingleFileAsync();
            if (_file != null)
            {
                display.Source = await ReadAsync();
            }
        }
        catch
        {

        }
    }

    public async void SaveAsync()
    {
        try
        {
            FileSavePicker picker = new FileSavePicker
            {
                DefaultFileExtension = file_extension,
                SuggestedFileName = "Picture",
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeChoices.Add("Picture", new List<string>() { file_extension });
            _file = await picker.PickSaveFileAsync();
            if (_file != null)
            {
                WriteAsync();
            }
        }
        catch
        {

        }
    }

    public async void RotateAsync(Image display)
    {
        if (_angle == 360) _angle = 0;
        _angle += 90;
        display.Source = await ReadAsync();
    }
}