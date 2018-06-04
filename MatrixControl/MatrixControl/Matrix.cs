using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MatrixControl
{
    public class Matrix : StackPanel
    {
        private readonly byte[][] table =
        {
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,1,1,0,0,1,1,0,
            0,1,1,0,0,1,1,0,
            0,1,1,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 0
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,0,0,1,1,0,0,0,
            0,1,1,1,1,0,0,0,
            0,0,0,1,1,0,0,0,
            0,0,0,1,1,0,0,0,
            0,0,0,1,1,0,0,0,
            0,0,0,0,0,0,0,0
            }, // 1 
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,1,1,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 2
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 3
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,0,0,1,1,0,
            0,1,1,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,1,1,0,
            0,0,0,0,0,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 4
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,1,1,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 5
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,1,1,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,1,1,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 6
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,1,1,0,
            0,0,0,0,0,1,1,0,
            0,0,0,0,0,1,1,0,
            0,0,0,0,0,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 7
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,1,1,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,1,1,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 8
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,1,1,1,1,1,1,0,
            0,1,1,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,1,1,0,
            0,1,1,1,1,1,1,0,
            0,0,0,0,0,0,0,0
            }, // 9
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0
            }, // Space
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,0,0,1,1,0,0,0,
            0,0,0,1,1,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,1,1,0,0,0,
            0,0,0,1,1,0,0,0,
            0,0,0,0,0,0,0,0
            }, // Colon
            new byte[] {
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,1,1,0,
            0,0,0,0,1,1,0,0,
            0,0,0,1,1,0,0,0,
            0,0,1,1,0,0,0,0,
            0,1,1,0,0,0,0,0,
            0,0,0,0,0,0,0,0
            } // Slash
        };

        private readonly List<char> glyphs =
            new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ', ':', '/' };
        private const int columns = 8;
        private const int rows = 7;
        private const int padding = 1;

        private string _value;
        private int _count;

        public enum Sources
        {
            Value = 0,
            Time = 1,
            Date = 2,
            TimeDate = 3
        }

        public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Brush),
        typeof(Matrix), null);

        public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(Sources),
        typeof(Matrix), new PropertyMetadata(Sources.Time));

        public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register("Size", typeof(UIElement),
        typeof(Matrix), new PropertyMetadata(4));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public Sources Source
        {
            get { return (Sources)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private Rectangle AddElement(string name, int left, int top)
        {
            Rectangle element = new Rectangle()
            {
                Tag = name,
                Height = Size,
                Width = Size,
                Fill = Foreground,
                RadiusX = 1,
                RadiusY = 1,
                Opacity = 0,
                Margin = new Thickness(2)
            };
            element.SetBinding(Rectangle.FillProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Foreground"),
                Mode = BindingMode.TwoWay
            });
            element.SetValue(Canvas.LeftProperty, left);
            element.SetValue(Canvas.TopProperty, top);
            return element;
        }

        private void AddSection(string name)
        {
            Canvas layout = new Canvas()
            {
                Tag = name,
                Height = rows * Size,
                Width = columns * Size
            };
            int x = 0;
            int y = 0;
            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    layout.Children.Add(AddElement($"{name}.{index}", x, y));
                    x = (x + Size + padding);
                    index++;
                }
                x = 0;
                y = (y + Size + padding);
            }
            this.Children.Add(layout);
        }

        private Rectangle SetElement(Canvas layout, string name)
        {
            return layout.Children.Cast<Rectangle>().FirstOrDefault(f => (string)f.Tag == name);
        }

        private Canvas SetSection(string name)
        {
            return this.Children.Cast<Canvas>().FirstOrDefault(f => (string)f.Tag == name);
        }

        private void SetLayout(string name, char glyph)
        {
            Canvas layout = SetSection(name);
            int pos = glyphs.IndexOf(glyph);
            byte[] values = table[pos];
            for (int index = 0; index < layout.Children.Count; index++)
            {
                SetElement(layout, $"{name}.{index}").Opacity = values[index];
            }
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
                    AddSection(item.ToString());
                }
                _count = length;
            }
            foreach (int item in list)
            {
                SetLayout(item.ToString(), array[item]);
            }
        }

        public Matrix()
        {
            this.Spacing = 0;
            this.Orientation = Orientation.Horizontal;
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            timer.Tick += (object s, object args) =>
            {
                if (Source != Sources.Value)
                {
                    string format = string.Empty;
                    switch (Source)
                    {
                        case Sources.Time:
                            format = "HH:mm:ss";
                            break;
                        case Sources.Date:
                            format = "dd/MM/yyyy";
                            break;
                        case Sources.TimeDate:
                            format = "HH:mm:ss dd/MM/yyyy";
                            break;
                    }
                    Value = DateTime.Now.ToString(format);

                }
            };
            timer.Start();
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