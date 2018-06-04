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

namespace PdfViewApp
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

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            uint pages = await library.OpenAsync();
            if (pages > 0)
            {
                Page.ItemsSource = library.Numbers(Convert.ToInt32(pages));
                Page.SelectedIndex = 0;
            }
        }

        private async void Page_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uint.TryParse(Page.SelectedItem.ToString(), out uint page);
            Display.Source = await library.ViewAsync(page);
        }
    }
}
