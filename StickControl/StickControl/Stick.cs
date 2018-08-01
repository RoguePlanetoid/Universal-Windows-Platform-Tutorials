using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace StickControl
{
    public class Stick : Grid
    {
        private bool _capture;
        private Ellipse _knob;
        private Ellipse _face;
        private double x = 0;
        private double y = 0;
        private double _m = 0;
        private double _res = 0;
        private double _width = 0;
        private double _height = 0;
        private double _alpha = 0;
        private double _alphaM = 0;
        private double _centreX = 0;
        private double _centreY = 0;
        private double _distance = 0;
        private double _oldAlphaM = -999.0;
        private double _oldDistance = -999.0;

        public delegate void ValueChangedEventHandler(object sender, double angle, double ratio);
        public event ValueChangedEventHandler ValueChanged;

        private void Middle()
        {
            _capture = false;
            Canvas.SetLeft(_knob, (this.Width - _width) / 2);
            Canvas.SetTop(_knob, (this.Height - _height) / 2);
            _centreX = this.Width / 2;
            _centreY = this.Height / 2;
        }

        private void Move(PointerRoutedEventArgs e)
        {
            double ToRadians(double angle)
            {
                return Math.PI * angle / 180.0;
            }

            double ToDegrees(double angle)
            {
                return angle * (180.0 / Math.PI);
            }

            x = e.GetCurrentPoint(this).Position.X;
            y = e.GetCurrentPoint(this).Position.Y;
            _res = Math.Sqrt((x - _centreX) *
            (x - _centreX) + (y - _centreY) * (y - _centreY));
            _m = (y - _centreY) / (x - _centreX);
            _alpha = ToDegrees(Math.Atan(_m) + Math.PI / 2);
            if (x < _centreX)
            {
                _alpha += 180.0;
            }
            else if (x == _centreX && y <= _centreY)
            {
                _alpha = 0.0;
            }
            else if (x == _centreX)
            {
                _alpha = 180.0;
            }
            if (_res > Radius)
            {
                x = _centreX + Math.Cos(ToRadians(_alpha) - Math.PI / 2) * Radius;
                y = _centreY + Math.Sin(ToRadians(_alpha) - Math.PI / 2) * Radius
                * ((_alpha % 180.0 == 0.0) ? -1.0 : 1.0);
                _res = Radius;
            }
            if (Math.Abs(_alpha - _alphaM) >= Sensitivity ||
                Math.Abs(_distance * Radius - _res) >= Sensitivity)
            {
                _alphaM = _alpha;
                _distance = _res / Radius;
            }
            if (_oldAlphaM != _alphaM ||
                _oldDistance != _distance)
            {
                Angle = _alphaM;
                Ratio = _distance;
                _oldAlphaM = _alphaM;
                _oldDistance = _distance;
                ValueChanged?.Invoke(this, Angle, Ratio);
            }
            Canvas.SetLeft(_knob, x - _width / 2);
            Canvas.SetTop(_knob, y - _height / 2);
        }

        private Ellipse GetCircle(double dimension, string path)
        {
            Ellipse circle = new Ellipse()
            {
                Height = dimension,
                Width = dimension
            };
            circle.SetBinding(Shape.FillProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath(path),
                Mode = BindingMode.TwoWay
            });
            return circle;
        }

        private void Init()
        {
            _knob = GetCircle(Radius / 2, "Knob");
            _face = GetCircle(Radius * 2, "Face");
            _height = _knob.ActualHeight;
            _width = _knob.ActualWidth;
            this.Width = _width + Radius * 2;
            this.Height = _height + Radius * 2;
            Middle();
            this.PointerExited -= null;
            this.PointerExited += (object sender, PointerRoutedEventArgs e) =>
            {
                Middle();
            };
            _knob.PointerReleased += (object sender, PointerRoutedEventArgs e) =>
            {
                Middle();
            };
            _knob.PointerPressed += (object sender, PointerRoutedEventArgs e) =>
            {
                _capture = true;
            };
            _knob.PointerMoved += (object sender, PointerRoutedEventArgs e) =>
            {
                if (_capture) Move(e);
            };
            _knob.PointerExited += (object sender, PointerRoutedEventArgs e) =>
            {
                Middle();
            };
        }

        private void Layout()
        {
            Init();
            this.Children.Clear();
            this.Children.Add(_face);
            Canvas canvas = new Canvas()
            {
                Width = this.Width,
                Height = this.Height
            };
            canvas.Children.Add(_knob);
            this.Children.Add(canvas);
        }

        public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register("Radius", typeof(int),
        typeof(Stick), new PropertyMetadata(100));

        public static readonly DependencyProperty KnobProperty =
        DependencyProperty.Register("Knob", typeof(Brush),
        typeof(Stick), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public static readonly DependencyProperty FaceProperty =
        DependencyProperty.Register("Face", typeof(Brush),
        typeof(Stick), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register("Angle", typeof(double),
        typeof(Stick), null);

        public static readonly DependencyProperty RatioProperty =
        DependencyProperty.Register("Ratio", typeof(double),
        typeof(Stick), null);

        public static readonly DependencyProperty SensitivityProperty =
        DependencyProperty.Register("Sensitivity", typeof(double),
        typeof(Stick), null);

        public int Radius
        {
            get { return (int)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); Layout(); }
        }

        public Brush Knob
        {
            get { return (Brush)GetValue(KnobProperty); }
            set { SetValue(KnobProperty, value); }
        }

        public Brush Face
        {
            get { return (Brush)GetValue(FaceProperty); }
            set { SetValue(FaceProperty, value); }
        }

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public double Ratio
        {
            get { return (double)GetValue(RatioProperty); }
            set { SetValue(RatioProperty, value); }
        }

        public double Sensitivity
        {
            get { return (double)GetValue(SensitivityProperty); }
            set { SetValue(SensitivityProperty, value); }
        }

        public Stick() => Layout();
    }
}