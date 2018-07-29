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

namespace DictatesApp
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
            library.Init(Dictate, Language, Display);
            Language.ItemsSource = library.Languages();
            Language.SelectedIndex = 0;
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            library.Language(Language.SelectedValue);
        }

        private void Dictate_Click(object sender, RoutedEventArgs e)
        {
            library.Dictate(Dictate, Language, Display);
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            library.New(Display);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            library.Open(Display);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            library.Save(ref Display);
        }
    }
}
