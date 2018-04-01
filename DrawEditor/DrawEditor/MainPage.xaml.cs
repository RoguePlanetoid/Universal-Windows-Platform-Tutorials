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

namespace DrawEditor
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
            library.Init(ref Display, ref Size, ref Colour);
        }

        private void Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            library.Size(ref Display, ref Size);
        }

        private void Colour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            library.Colour(ref Display, ref Colour);
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            library.New(Display);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            library.OpenAsync(Display);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            library.SaveAsync(Display);
        }
    }
}
