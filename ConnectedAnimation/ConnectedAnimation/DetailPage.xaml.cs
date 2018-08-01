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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ConnectedAnimation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Target.Fill = (SolidColorBrush)e.Parameter;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back) Library.From(ref Target);
            base.OnNavigatingFrom(e);
        }

        private void Target_Loaded(object sender, RoutedEventArgs e)
        {
            Library.Loaded(ref Target);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
