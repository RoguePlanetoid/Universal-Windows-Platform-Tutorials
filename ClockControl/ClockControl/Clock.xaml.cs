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

namespace ClockControl
{
    public sealed partial class Clock : UserControl
    {
        public Clock()
        {
            this.InitializeComponent();
        }

        private DispatcherTimer _timer = new DispatcherTimer();
        private Canvas _markers = new Canvas();
        private Canvas _face = new Canvas();
        private Windows.UI.Xaml.Shapes.Rectangle _secondsHand;
        private Windows.UI.Xaml.Shapes.Rectangle _minutesHand;
        private Windows.UI.Xaml.Shapes.Rectangle _hoursHand;

        private static Windows.UI.ViewManagement.UISettings _uiSettings = new Windows.UI.ViewManagement.UISettings();
        private Brush _foreground = new SolidColorBrush(_uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background));
        private Brush _background = new SolidColorBrush(_uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent));

        private int _secondsWidth = 1;
        private int _secondsHeight;
        private int _minutesWidth = 5;
        private int _minutesHeight;
        private int _hoursWidth = 8;
        private int _hoursHeight;
        private double _diameter;

        public bool IsRealTime { get; set; } = true;
        public bool ShowSeconds { get; set; } = true;
        public bool ShowMinutes { get; set; } = true;
        public bool ShowHours { get; set; } = true;
        public DateTime Time { get; set; } = DateTime.Now;

        private Windows.UI.Xaml.Shapes.Rectangle Hand(double width, double height, double radiusX, double radiusY, double thickness)
        {
            Windows.UI.Xaml.Shapes.Rectangle hand = new Windows.UI.Xaml.Shapes.Rectangle
            {
                Width = width,
                Height = height,
                Fill = _background,
                StrokeThickness = thickness,
                RadiusX = radiusX,
                RadiusY = radiusY
            };
            return hand;
        }

        private void RemoveHand(ref Windows.UI.Xaml.Shapes.Rectangle hand)
        {
            if (hand != null && _face.Children.Contains(hand))
            {
                _face.Children.Remove(hand);
            }
        }

        private void AddHand(ref Windows.UI.Xaml.Shapes.Rectangle hand)
        {
            if (!_face.Children.Contains(hand))
            {
                _face.Children.Add(hand);
            }
        }

        private TransformGroup TransformGroup(double angle, double x, double y)
        {
            TransformGroup transformGroup = new TransformGroup();
            TranslateTransform firstTranslate = new TranslateTransform
            {
                X = x,
                Y = y
            };
            transformGroup.Children.Add(firstTranslate);
            RotateTransform rotateTransform = new RotateTransform
            {
                Angle = angle
            };
            transformGroup.Children.Add(rotateTransform);
            TranslateTransform secondTranslate = new TranslateTransform
            {
                X = _diameter / 2,
                Y = _diameter / 2
            };
            transformGroup.Children.Add(secondTranslate);
            return transformGroup;
        }

        private void SecondHand(int seconds)
        {
            RemoveHand(ref _secondsHand);
            if (ShowSeconds)
            {
                _secondsHand = Hand(_secondsWidth, _secondsHeight, 0, 0, 0);
                _secondsHand.RenderTransform = TransformGroup(seconds * 6,
                -_secondsWidth / 2, -_secondsHeight + 4.25);
                AddHand(ref _secondsHand);
            }
        }

        private void MinuteHand(int minutes, int seconds)
        {
            RemoveHand(ref _minutesHand);
            if (ShowMinutes)
            {
                _minutesHand = Hand(_minutesWidth, _minutesHeight, 2, 2, 0.6);
                _minutesHand.RenderTransform = TransformGroup(6 * minutes + seconds / 10,
                -_minutesWidth / 2, -_minutesHeight + 4.25);
                AddHand(ref _minutesHand);
            }
        }

        private void HourHand(int hours, int minutes, int seconds)
        {
            RemoveHand(ref _hoursHand);
            if (ShowHours)
            {
                _hoursHand = Hand(_hoursWidth, _hoursHeight, 3, 3, 0.6);
                _hoursHand.RenderTransform = TransformGroup(30 * hours + minutes / 2 + seconds / 120,
                -_hoursWidth / 2, -_hoursHeight + 4.25);
                AddHand(ref _hoursHand);
            }
        }

        private void Layout(ref Canvas canvas)
        {
            canvas.Children.Clear();
            _diameter = canvas.Width;
            double inner = _diameter - 15;
            Windows.UI.Xaml.Shapes.Ellipse rim = new Windows.UI.Xaml.Shapes.Ellipse
            {
                Height = _diameter,
                Width = _diameter,
                Stroke = RimBackground,
                StrokeThickness = 20
            };
            canvas.Children.Add(rim);
            _markers.Children.Clear();
            _markers.Width = inner;
            _markers.Height = inner;
            for (int i = 0; i < 60; i++)
            {
                Windows.UI.Xaml.Shapes.Rectangle marker = 
                    new Windows.UI.Xaml.Shapes.Rectangle
                {
                    Fill = RimForeground
                };
                if ((i % 5) == 0)
                {
                    marker.Width = 3;
                    marker.Height = 8;
                    marker.RenderTransform = TransformGroup(i * 6, -(marker.Width / 2),
                    -(marker.Height * 2 + 4.5 - rim.StrokeThickness / 2 - inner / 2 - 6));
                }
                else
                {
                    marker.Width = 1;
                    marker.Height = 4;
                    marker.RenderTransform = TransformGroup(i * 6, -(marker.Width / 2),
                    -(marker.Height * 2 + 12.75 - rim.StrokeThickness / 2 - inner / 2 - 8));
                }
                _markers.Children.Add(marker);
            }
            canvas.Children.Add(_markers);
            _face.Width = _diameter;
            _face.Height = _diameter;
            canvas.Children.Add(_face);
            _secondsHeight = (int)_diameter / 2 - 20;
            _minutesHeight = (int)_diameter / 2 - 40;
            _hoursHeight = (int)_diameter / 2 - 60;
        }

        public Brush RimForeground
        {
            get { return _foreground; }
            set
            {
                _foreground = value;
                Layout(ref Display);
            }
        }

        public Brush RimBackground
        {
            get { return _background; }
            set
            {
                _background = value;
                Layout(ref Display);
            }
        }

        public bool Enabled
        {
            get { return _timer.IsEnabled; }
            set
            {
                if (_timer.IsEnabled)
                {
                    _timer.Stop();
                }
                else
                {
                    _timer.Start();
                }
            }
        }

        private void Display_Loaded(object sender, RoutedEventArgs e)
        {
            Layout(ref Display);
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (object s, object obj) =>
            {
                if (IsRealTime) Time = DateTime.Now;
                SecondHand(Time.Second);
                MinuteHand(Time.Minute, Time.Second);
                HourHand(Time.Hour, Time.Minute, Time.Second);
            };
            _timer.Start();
        }
    }
}
