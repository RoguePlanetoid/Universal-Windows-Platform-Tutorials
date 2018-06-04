using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace LightsControl
{
    public class Light : Grid, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Brush),
        typeof(Light), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register("Size", typeof(double),
        typeof(Light), new PropertyMetadata((double)100));

        public static readonly DependencyProperty OffProperty =
        DependencyProperty.Register("Off", typeof(Visibility),
        typeof(Light), new PropertyMetadata(Visibility.Collapsed));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set
            {
                SetValue(ForegroundProperty, value);
                NotifyPropertyChanged("Foreground");
            }
        }

        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set
            {
                SetValue(SizeProperty, value);
                NotifyPropertyChanged("Size");
            }
        }

        public Visibility Off
        {
            get { return (Visibility)GetValue(OffProperty); }
            set
            {
                SetValue(OffProperty, value);
                NotifyPropertyChanged("Off");
            }
        }

        public Light()
        {
            this.Margin = new Thickness(5);
            Ellipse element = new Ellipse()
            {
                Stretch = Stretch.Fill,
                Height = Size,
                Width = Size
            };
            element.SetBinding(Ellipse.FillProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Foreground"),
                Mode = BindingMode.TwoWay
            });
            Ellipse overlay = new Ellipse()
            {
                Fill = new SolidColorBrush(Colors.Black),
                Stretch = Stretch.Fill,
                Opacity = 0.75,
                Height = Size,
                Width = Size
            };
            overlay.SetBinding(Ellipse.VisibilityProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Off"),
                Mode = BindingMode.TwoWay
            });
            this.Children.Add(element);
            this.Children.Add(overlay);
        }

        public bool IsOn
        {
            get { return Off == Visibility.Collapsed; }
            set
            {
                Off = value ? Visibility.Collapsed : Visibility.Visible;
                NotifyPropertyChanged("IsOn");
                NotifyPropertyChanged("Off");
            }
        }
    }

    public class Lights : StackPanel
    {
        private Light _red = new Light { Foreground = new SolidColorBrush(Colors.Red) };
        private Light _orange = new Light { Foreground = new SolidColorBrush(Colors.Orange) };
        private Light _green = new Light { Foreground = new SolidColorBrush(Colors.Green) };

        private async Task<bool> Delay(int seconds = 2)
        {
            await Task.Delay(seconds * 1000);
            return true;
        }

        public Lights()
        {
            this.Orientation = Orientation.Vertical;
            this.Children.Add(_red);
            this.Children.Add(_orange);
            this.Children.Add(_green);
        }

        public async void Traffic()
        {
            _red.IsOn = false;
            _orange.IsOn = false;
            _green.IsOn = true;
            await Delay();
            _green.IsOn = false;
            await Delay();
            _orange.IsOn = true;
            await Delay();
            _orange.IsOn = false;
            await Delay();
            _red.IsOn = true;
            await Delay();
            _red.IsOn = true;
            await Delay();
            _orange.IsOn = true;
            await Delay();
            _red.IsOn = false;
            _orange.IsOn = false;
            _green.IsOn = true;
            await Delay();
        }
    }
}