using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ZuneViewApp
{
    public enum ZuneViewType
    {
        Keel, Draco, Scorpius, Pavo
    }

    public class ZuneViewStyle
    {
        public string Name { get; set; }
        public Color Colour { get; set; }
        public ZuneViewType[] Types { get; set; }

        public ZuneViewStyle(string name, Color colour, ZuneViewType[] types)
        {
            Name = name;
            Types = types;
            Colour = colour;
        }
    }

    public class ZuneViewDevice
    {
        public string Name { get; set; }
        public ZuneViewType Type { get; set; }
        public Brush Fill { get; set; } // Change to Style

        public ZuneViewDevice(ZuneViewType type, Brush fill)
        {
            Type = type;
            Fill = fill;
        }

        public ZuneViewDevice(string name, ZuneViewType type, Brush fill)
        {
            Name = name;
            Type = type;
            Fill = fill;
        }
    }

    public class ZuneViewDeviceTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Keel { get; set; }
        public DataTemplate Draco { get; set; }
        public DataTemplate Scorpius { get; set; }
        public DataTemplate Pavo { get; set; }

        protected override DataTemplate SelectTemplateCore(object value, DependencyObject container)
        {
            if (value is ZuneViewDevice item)
            {
                switch (item.Type)
                {
                    case ZuneViewType.Keel:
                        return Keel;
                    case ZuneViewType.Draco:
                        return Draco;
                    case ZuneViewType.Scorpius:
                        return Scorpius;
                    case ZuneViewType.Pavo:
                        return Pavo;
                }
            }
            return null;
        }
    }

    public class Library
    {
        private const int scale_size = 1000;
        private const double image_dpi = 96.0;
        private const string file_extension = ".png";
        private readonly Color brown = Color.FromArgb(255, 68, 48, 22);
        private readonly Color black = Color.FromArgb(255, 28, 28, 28);
        private readonly Color red = Color.FromArgb(255, 132, 20, 44);
        private readonly Color platinum = Color.FromArgb(255, 220, 220, 220);

        private List<ZuneViewStyle> _styles = new List<ZuneViewStyle>();
        private List<ZuneViewDevice> _devices = new List<ZuneViewDevice>();

        private void AddStyle(string name, Color colour, params ZuneViewType[] types)
        {
            _styles.Add(new ZuneViewStyle(name, colour, types));
        }

        private void SetStyles()
        {
            AddStyle("Brown", brown, ZuneViewType.Keel);
            AddStyle("Halo", Color.FromArgb(255, 70, 90, 90), ZuneViewType.Keel);
            AddStyle("Red", Color.FromArgb(255, 234, 48, 104), ZuneViewType.Keel);
            AddStyle("Pink", Color.FromArgb(255, 228, 182, 194), ZuneViewType.Keel);
            AddStyle("Magenta", Color.FromArgb(255, 230, 62, 100), ZuneViewType.Keel);
            AddStyle("Blue", Color.FromArgb(255, 14, 84, 150), ZuneViewType.Keel);
            AddStyle("Orange", Color.FromArgb(255, 246, 116, 0), ZuneViewType.Keel);
            AddStyle("Black", Color.FromArgb(255, 16, 24, 22), ZuneViewType.Keel);
            AddStyle("White", Color.FromArgb(255, 235, 235, 235),
            ZuneViewType.Keel, ZuneViewType.Draco, ZuneViewType.Scorpius);
            AddStyle("Black", black, ZuneViewType.Draco, ZuneViewType.Scorpius);
            AddStyle("Red", red, ZuneViewType.Draco, ZuneViewType.Scorpius);
            AddStyle("Blue", Color.FromArgb(255, 2, 70, 130),
            ZuneViewType.Draco, ZuneViewType.Scorpius);
            AddStyle("Green", Color.FromArgb(255, 140, 132, 66), ZuneViewType.Scorpius);
            AddStyle("Pink", Color.FromArgb(255, 246, 164, 196), ZuneViewType.Scorpius);
            AddStyle("Citron", Color.FromArgb(255, 210, 198, 6), ZuneViewType.Scorpius);
            AddStyle("Platinum", platinum, ZuneViewType.Pavo);
            AddStyle("Onyx", Color.FromArgb(255, 24, 42, 44), ZuneViewType.Pavo);
            AddStyle("Red", Color.FromArgb(255, 124, 30, 54), ZuneViewType.Pavo);
            AddStyle("Blue", Color.FromArgb(255, 30, 104, 146), ZuneViewType.Pavo);
            AddStyle("Green", Color.FromArgb(255, 166, 172, 92), ZuneViewType.Pavo);
            AddStyle("Pink", Color.FromArgb(255, 192, 162, 182), ZuneViewType.Pavo);
            AddStyle("Magenta", Color.FromArgb(255, 140, 88, 140), ZuneViewType.Pavo);
            AddStyle("Purple", Color.FromArgb(255, 94, 90, 120), ZuneViewType.Pavo);
            AddStyle("Atomic", Color.FromArgb(255, 186, 24, 0), ZuneViewType.Pavo);
        }

        private void AddDevice(string name, ZuneViewType type, Color colour)
        {
            _devices.Add(new ZuneViewDevice(name, type, new SolidColorBrush(colour)));
        }

        private void SetDevices()
        {
            AddDevice("Zune 30", ZuneViewType.Keel, brown);
            AddDevice("Zune 80 120", ZuneViewType.Draco, black);
            AddDevice("Zune 4 8 16", ZuneViewType.Scorpius, red);
            AddDevice("Zune HD", ZuneViewType.Pavo, platinum);
        }

        private void SetItem(ref ItemsControl display, ref ComboBox devices, ref ComboBox styles)
        {
            ZuneViewDevice device = (ZuneViewDevice)devices.SelectedItem ?? _devices.First();
            ZuneViewStyle style = (ZuneViewStyle)styles.SelectedItem ?? _styles.First();
            display.ItemsSource = new List<ZuneViewDevice>()
            {
                new ZuneViewDevice(device.Type, new SolidColorBrush(style.Colour))
            };
        }

        private async void Render(UIElement element, StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                RenderTargetBitmap target = new RenderTargetBitmap();
                await target.RenderAsync(element, scale_size, 0);
                IBuffer buffer = await target.GetPixelsAsync();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied,
                (uint)target.PixelWidth, (uint)target.PixelHeight, image_dpi, image_dpi, buffer.ToArray());
                await encoder.FlushAsync();
                target = null;
                buffer = null;
                encoder = null;
            }
        }

        public Library()
        {
            SetDevices();
            SetStyles();
        }

        public void Init(ref ItemsControl display, ref ComboBox devices, ref ComboBox styles)
        {
            devices.ItemsSource = _devices;
            styles.ItemsSource = _styles;
            devices.SelectedIndex = 0;
            styles.SelectedIndex = 0;
            SetItem(ref display, ref devices, ref styles);
        }

        public void Style(ref ItemsControl display, ref ComboBox devices, ref ComboBox styles)
        {
            SetItem(ref display, ref devices, ref styles);
        }

        public void Device(ref ItemsControl display, ref ComboBox devices, ref ComboBox styles)
        {
            ZuneViewDevice selected = (ZuneViewDevice)devices.SelectedItem;
            IEnumerable<ZuneViewStyle> list = _styles.Where(w => w.Types.Contains(selected.Type));
            styles.ItemsSource = list;
            styles.SelectedIndex = 0;
            SetItem(ref display, ref devices, ref styles);
        }

        public async void Save(UIElement element, ComboBox devices)
        {
            try
            {
                ZuneViewDevice selected = (ZuneViewDevice)devices.SelectedItem;
                FileSavePicker picker = new FileSavePicker
                {
                    DefaultFileExtension = file_extension,
                    SuggestedFileName = selected.Name,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                picker.FileTypeChoices.Add("Image", new List<string>() { file_extension });
                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    Render(element, file);
                }
            }
            finally
            {
                // Ignore Exceptions
            }
        }
    }
}