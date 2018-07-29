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

namespace AdaptiveCard
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

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            library.View(ref Title, ref Body, ref Input, ref Display);
        }

        private void Json_Click(object sender, RoutedEventArgs e)
        {
            library.View(ref Input, ref Display);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            library.Open(Input, Display);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            library.Save(ref Input, ref Display);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            library.Add(ref Input, ref Display);
        }
    }
}
