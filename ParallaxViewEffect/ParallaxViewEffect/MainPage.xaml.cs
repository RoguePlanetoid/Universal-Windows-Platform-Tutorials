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

namespace ParallaxViewEffect
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

        private class Item
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Text { get; set; } = string.Empty;
        }

        private void Value_QuerySubmitted(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Display.Items.Add(new Item { Text = Value.Text });
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Item item = (Item)((AppBarButton)sender).Tag;
            Display.Items.Remove(item);
        }
    }
}
