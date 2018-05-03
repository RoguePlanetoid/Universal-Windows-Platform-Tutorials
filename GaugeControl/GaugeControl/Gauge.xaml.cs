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

namespace GaugeControl
{
    public sealed partial class Gauge : UserControl
    {
        public Gauge()
        {
            this.InitializeComponent();
        }

        private Windows.UI.Xaml.Shapes.Rectangle _needle;
        private int _needleWidth = 2;
        private int _needleLength = 0;
        private double _diameter = 0;
        private bool _initialised = false;

        private TransformGroup TransformGroup(double angle, double x, double y)
        {
            TransformGroup transformGroup = new TransformGroup();
            TranslateTransform firstTranslate = new TranslateTransform()
            {
                X = x,
                Y = y
            };
            transformGroup.Children.Add(firstTranslate);
            RotateTransform rotateTransform = new RotateTransform()
            {
                Angle = angle
            };
            transformGroup.Children.Add(rotateTransform);
            TranslateTransform secondTranslate = new TranslateTransform()
            {
                X = _diameter / 2,
                Y = _diameter / 2
            };
            transformGroup.Children.Add(secondTranslate);
            return transformGroup;
        }

        private void Indicator(int value)
        {
            Init(ref Display);
            double percentage = (((double)value / (double)Maximum) * 100);
            double position = (percentage / 2) + 5;
            _needle.RenderTransform = TransformGroup(position * 6,
            -_needleWidth / 2, -_needleLength + 4.25);
        }

        private void Init(ref Canvas canvas)
        {
            canvas.Children.Clear();
            _diameter = canvas.Width;
            double inner = _diameter;
            Windows.UI.Xaml.Shapes.Ellipse face = new Windows.UI.Xaml.Shapes.Ellipse()
            {
                Height = _diameter,
                Width = _diameter,
                Fill = Fill
            };
            canvas.Children.Add(face);
            Canvas markers = new Canvas()
            {
                Width = inner,
                Height = inner
            };
            for (int i = 0; i < 51; i++)
            {
                Windows.UI.Xaml.Shapes.Rectangle marker = new Windows.UI.Xaml.Shapes.Rectangle()
                {
                    Fill = Foreground
                };
                if ((i % 5) == 0)
                {
                    marker.Width = 4;
                    marker.Height = 16;
                    marker.RenderTransform = TransformGroup(i * 6, -(marker.Width / 2),
                    -(marker.Height * 2 + 4.5 - face.StrokeThickness / 2 - inner / 2 - 16));
                }
                else
                {
                    marker.Width = 2;
                    marker.Height = 8;
                    marker.RenderTransform = TransformGroup(i * 6, -(marker.Width / 2),
                    -(marker.Height * 2 + 12.75 - face.StrokeThickness / 2 - inner / 2 - 16));
                }
                markers.Children.Add(marker);
            }
            markers.RenderTransform = new RotateTransform()
            {
                Angle = 30,
                CenterX = _diameter / 2,
                CenterY = _diameter / 2
            };
            canvas.Children.Add(markers);
            _needle = new Windows.UI.Xaml.Shapes.Rectangle()
            {
                Width = _needleWidth,
                Height = (int)_diameter / 2 - 30,
                Fill = Foreground
            };
            canvas.Children.Add(_needle);
            Windows.UI.Xaml.Shapes.Ellipse middle = new Windows.UI.Xaml.Shapes.Ellipse()
            {
                Height = _diameter / 10,
                Width = _diameter / 10,
                Fill = Foreground
            };
            Canvas.SetLeft(middle, (_diameter - middle.ActualWidth) / 2);
            Canvas.SetTop(middle, (_diameter - middle.ActualHeight) / 2);
            canvas.Children.Add(middle);
        }

        public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register("Fill", typeof(Brush),
        typeof(Gauge), null);

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(Gauge),
        new PropertyMetadata(25));

        public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register("Minimum", typeof(int), typeof(Gauge),
        new PropertyMetadata(0));

        public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register("Maximum", typeof(int), typeof(Gauge),
        new PropertyMetadata(100));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set
            {
                if (value >= Minimum && value <= Maximum)
                {
                    SetValue(ValueProperty, value);
                    Indicator(value);
                }
            }
        }

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        private void Display_Loaded(object sender, RoutedEventArgs e)
        {
            Indicator(Value);
        }
    }
}
