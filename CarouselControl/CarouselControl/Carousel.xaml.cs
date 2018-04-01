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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CarouselControl
{
    public sealed partial class Carousel : UserControl
    {
        public Carousel()
        {
            this.InitializeComponent();
        }

        private Windows.UI.Xaml.Media.Animation.Storyboard _animation =
            new Windows.UI.Xaml.Media.Animation.Storyboard();
        private List<Windows.UI.Xaml.Media.Imaging.BitmapImage> _list =
            new List<Windows.UI.Xaml.Media.Imaging.BitmapImage>();

        private Point _point;
        private Point _radius = new Point { X = -20, Y = 200 };
        private double _speed = 0.0125;
        private double _perspective = 55;
        private double _distance;

        private void Layout(ref Canvas display)
        {
            display.Children.Clear();
            for (int index = 0; index < _list.Count(); index++)
            {
                _distance = 1 / (1 - (_point.X / _perspective));
                Image item = new Image
                {
                    Width = 150,
                    Source = _list[index],
                    Tag = index * ((Math.PI * 2) / _list.Count),
                    RenderTransform = new ScaleTransform()
                };
                _point.X = Math.Cos((double)item.Tag) * _radius.X;
                _point.Y = Math.Sin((double)item.Tag) * _radius.Y;
                Canvas.SetLeft(item, _point.X - (item.Width - _perspective));
                Canvas.SetTop(item, _point.Y);
                item.Opacity = ((ScaleTransform)item.RenderTransform).ScaleX =
                    ((ScaleTransform)item.RenderTransform).ScaleY = _distance;
                display.Children.Add(item);
            }
        }

        private void Rotate()
        {
            foreach (Image item in Display.Children)
            {
                double angle = (double)item.Tag;
                angle -= _speed;
                item.Tag = angle;
                _point.X = Math.Cos(angle) * _radius.X;
                _point.Y = Math.Sin(angle) * _radius.Y;
                Canvas.SetLeft(item, _point.X - (item.Width - _perspective));
                Canvas.SetTop(item, _point.Y);
                if (_radius.X >= 0)
                {
                    _distance = 1 * (1 - (_point.X / _perspective));
                    Canvas.SetZIndex(item, -(int)(_point.X));
                }
                else
                {
                    _distance = 1 / (1 - (_point.X / _perspective));
                    Canvas.SetZIndex(item, (int)(_point.X));
                }
                item.Opacity = ((ScaleTransform)item.RenderTransform).ScaleX =
                    ((ScaleTransform)item.RenderTransform).ScaleY = _distance;
            }
            _animation.Begin();
        }

        public void Add(Windows.UI.Xaml.Media.Imaging.BitmapImage image)
        {
            _list.Add(image);
            Layout(ref Display);
        }

        public void RemoveLast()
        {
            if (_list.Any())
            {
                _list.Remove(_list.Last());
                Layout(ref Display);
            }
        }

        public void New()
        {
            _list.Clear();
            Layout(ref Display);
        }

        private void Display_Loaded(object sender, RoutedEventArgs e)
        {
            _animation.Completed += (object s, object obj) =>
            {
                Rotate();
            };
            _animation.Begin();
        }
    }
}
