using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace SegmentControl
{
    public class Segment : StackPanel
    {
        private readonly byte[][] table =
        {
            // a, b, c, d, e, f, g
            new byte[] { 1, 1, 1, 1, 1, 1, 0 }, // 0
            new byte[] { 0, 1, 1, 0, 0, 0, 0 }, // 1
            new byte[] { 1, 1, 0, 1, 1, 0, 1 }, // 2
            new byte[] { 1, 1, 1, 1, 0, 0, 1 }, // 3
            new byte[] { 0, 1, 1, 0, 0, 1, 1 }, // 4
            new byte[] { 1, 0, 1, 1, 0, 1, 1 }, // 5
            new byte[] { 1, 0, 1, 1, 1, 1, 1 }, // 6
            new byte[] { 1, 1, 1, 0, 0, 0, 0 }, // 7
            new byte[] { 1, 1, 1, 1, 1, 1, 1 }, // 8
            new byte[] { 1, 1, 1, 0, 0, 1, 1 }, // 9
            new byte[] { 0, 0, 0, 0, 0, 0, 0 }, // None
            new byte[] { 0, 0, 0, 0, 0, 0, 0 }, // Colon
        };
        private const int width = 5;
        private const int height = 25;

        private string _value;
        private int _count;
        private DispatcherTimer _timer;

        public enum Sources
        {
            Value = 0,
            Time = 1
        }

        public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(Sources),
        typeof(Segment), new PropertyMetadata(Sources.Time));

        public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Brush),
        typeof(Segment), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Sources Source
        {
            get { return (Sources)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        private Rectangle AddElement(string name, int left, int top, int width, int height)
        {
            Rectangle rect = new Rectangle()
            {
                Tag = name,
                Width = width,
                Height = height,
                RadiusX = 2,
                RadiusY = 2,
                Fill = Foreground
            };
            Canvas.SetLeft(rect, left);
            Canvas.SetTop(rect, top);
            return rect;
        }

        private void AddSegment(string name)
        {
            Canvas segment = new Canvas()
            {
                Margin = new Thickness(2),
                Tag = name,
                Height = 50,
                Width = 25
            };
            segment.Children.Add(AddElement($"{name}.a", width, 0, height, width));
            segment.Children.Add(AddElement($"{name}.h", width + width + width, width + width + width, width, width));
            segment.Children.Add(AddElement($"{name}.f", 0, width, width, height));
            segment.Children.Add(AddElement($"{name}.b", height + width, width, width, height));
            segment.Children.Add(AddElement($"{name}.g", width, height + width, height, width));
            segment.Children.Add(AddElement($"{name}.e", 0, height + width + width, width, height));
            segment.Children.Add(AddElement($"{name}.c", height + width, height + width + width, width, height));
            segment.Children.Add(AddElement($"{name}.i", width + width + width, height + width + width + width + width, width, width));
            segment.Children.Add(AddElement($"{name}.d", width, height + height + width + width, height, width));
            this.Children.Add(segment);
        }

        private Canvas SetLayout(string name)
        {
            return this.Children.Cast<Canvas>().FirstOrDefault(f => (string)f.Tag == name);
        }

        private Rectangle SetElement(Canvas layout, string name)
        {
            return layout.Children.Cast<Rectangle>().FirstOrDefault(f => (string)f.Tag == name);
        }

        private void SetSegment(string name, int digit)
        {
            Canvas layout = SetLayout(name);
            byte[] values = table[digit];
            SetElement(layout, $"{name}.a").Opacity = values[0];
            SetElement(layout, $"{name}.b").Opacity = values[1];
            SetElement(layout, $"{name}.c").Opacity = values[2];
            SetElement(layout, $"{name}.d").Opacity = values[3];
            SetElement(layout, $"{name}.e").Opacity = values[4];
            SetElement(layout, $"{name}.f").Opacity = values[5];
            SetElement(layout, $"{name}.g").Opacity = values[6];
            SetElement(layout, $"{name}.h").Opacity = digit > 10 ? 1 : 0;
            SetElement(layout, $"{name}.i").Opacity = digit > 10 ? 1 : 0;
        }

        private void GetLayout()
        {
            char[] array = _value.ToCharArray();
            int length = array.Length;
            List<int> list = Enumerable.Range(0, length).ToList();
            if (_count != length)
            {
                this.Children.Clear();
                foreach (int item in list)
                {
                    AddSegment(item.ToString());
                }
                _count = length;
            }
            foreach (int item in list)
            {
                string val = array[item].ToString();
                int digit = val == ":" ? 11 : int.Parse(val);
                SetSegment(item.ToString(), digit);
            }
        }

        public Segment()
        {
            this.Spacing = 10;
            this.Orientation = Orientation.Horizontal;
            if (Source == Sources.Time)
            {
                _timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(250)
                };
                _timer.Tick += (object sender, object e) =>
                {
                    string time = DateTime.Now.ToString("HH:mm:ss");
                    this.Value = time;
                };
                _timer.Start();
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                GetLayout();
            }
        }
    }
}