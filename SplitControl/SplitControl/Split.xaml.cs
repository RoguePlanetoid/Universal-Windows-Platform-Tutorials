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

namespace SplitControl
{
    public sealed partial class Split : UserControl
    {
        public Split()
        {
            this.InitializeComponent();
        }

        private string _value;
        private string _from;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_from != null)
                {
                    TextBlockBottom.Text = _from;
                    if (_from != value)
                    {
                        TextBlockTop.Text = value;
                        TextBlockFlipBottom.Text = value;
                        FlipAnimation.Begin();
                    }
                }
                TextBlockFlipTop.Text = value;
                _from = value;
            }
        }
    }

    public class Splits : StackPanel
    {
        private const char space = ' ';

        private string _value;
        private int _count;

        public enum Sources
        {
            Value = 0,
            Time = 1,
            Date = 2,
            TimeDate = 3
        }

        public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(Sources),
        typeof(Split), new PropertyMetadata(Sources.Time));

        public Sources Source
        {
            get { return (Sources)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private void Add(string name)
        {
            FrameworkElement element = new Split() { Tag = name };
            if (name == null)
            {
                element = new Canvas { Width = 5 };
            }
            this.Children.Add(element);
        }

        private void SetSplit(string name, char glyph)
        {
            FrameworkElement element = this.Children.Cast<FrameworkElement>()
            .FirstOrDefault(f => (string)f.Tag == name);
            if (element is Split)
            {
                ((Split)element).Value = glyph.ToString();
            }
        }

        private void GetLayout()
        {
            char[] array = _value.ToCharArray();
            int length = array.Length;
            IEnumerable<int> list = Enumerable.Range(0, length);
            if (_count != length)
            {
                this.Children.Clear();
                foreach (int item in list)
                {
                    Add((array[item] == space)
                    ? null : item.ToString());
                }
                _count = length;
            }
            foreach (int item in list)
            {
                SetSplit(item.ToString(), array[item]);
            }
        }

        public Splits()
        {
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
                            format = "HH mm ss";
                            break;
                        case Sources.Date:
                            format = "dd MM yyyy";
                            break;
                        case Sources.TimeDate:
                            format = "HH mm ss  dd MM yyyy";
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
