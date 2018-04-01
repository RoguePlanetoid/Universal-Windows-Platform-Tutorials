using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SlidePlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        Library library = new Library();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            library.Speed = (int)Speed.Value;
            library.Playing += (Windows.UI.Xaml.Media.Imaging.BitmapImage image, int index) =>
            {
                Display.Source = image;
                Position.Value = index;
            };
            library.Stopped += () =>
            {
                Play.Icon = new SymbolIcon(Symbol.Play);
                Play.Label = "Play";
                Display.Source = null;
                Position.Value = 0;
            };
        }

        private void Go_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            library.Go(ref Display, Value.Text, e);
        }

        private void Position_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            library.Position = (int)Position.Value;
        }

        private void Speed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Speed != null)
            {
                library.Speed = (int)Speed.Value;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Position.Maximum = library.Add(Value.Text);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Position.Maximum = library.Remove((int)Position.Value);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (library.IsPlaying)
            {
                library.Pause();
                Play.Icon = new SymbolIcon(Symbol.Play);
                Play.Label = "Play";
            }
            else
            {
                library.Play();
                Play.Icon = new SymbolIcon(Symbol.Pause);
                Play.Label = "Pause";
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            library.Stop();
        }
    }
}
