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

namespace Mahjong
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
            library.Init(ref Display);
        }

        private void Display_Tapped(object sender, TappedRoutedEventArgs e)
        {
            library.Tapped(sender as ItemsControl, e.OriginalSource as ContentPresenter);
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            library.New(ref Display);
        }

        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            library.Hint();
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            library.Show();
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            library.Shuffle();
        }
    }
}
