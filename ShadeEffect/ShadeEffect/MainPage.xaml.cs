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

namespace ShadeEffect
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

        private Windows.UI.Composition.SpriteVisual _visual;

        private Windows.UI.Composition.Compositor Compositor
        {
            get
            {
                return Windows.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(Logo).Compositor;
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            _visual = Compositor.CreateSpriteVisual();
            _visual.Size = new System.Numerics.Vector2((float)Logo.ActualWidth, (float)Logo.ActualHeight);
            Windows.UI.Composition.DropShadow shadow = Compositor.CreateDropShadow();
            shadow.Color = Windows.UI.Colors.Black;
            shadow.Offset = new System.Numerics.Vector3(10, 10, 0);
            shadow.Mask = Logo.GetAlphaMask();
            _visual.Shadow = shadow;
            Windows.UI.Xaml.Hosting.ElementCompositionPreview.SetElementChildVisual(ShadowElement, _visual);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _visual.Shadow = null;
        }
    }
}
