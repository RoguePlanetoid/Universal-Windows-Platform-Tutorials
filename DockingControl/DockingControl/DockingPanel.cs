using System;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DockingControl
{
    public class DockingPanel : Panel
    {
        private static bool _ignorePropertyChange;

        public enum Dock
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        private static void OnLastChildFillPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is DockingPanel panel)
                panel.InvalidateArrange();
        }

        private static void OnDockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_ignorePropertyChange)
            {
                _ignorePropertyChange = false;
                return;
            }
            UIElement element = (UIElement)d;
            Dock value = (Dock)e.NewValue;
            if ((value != Dock.Left) && (value != Dock.Top) &&
                (value != Dock.Right) && (value != Dock.Bottom))
            {
                _ignorePropertyChange = true;
                element.SetValue(DockProperty, (Dock)e.OldValue);
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "InvalidValue", value), "value");
            }
            if (VisualTreeHelper.GetParent(element) is DockingPanel panel)
                panel.InvalidateMeasure();
        }

        public static readonly DependencyProperty LastChildFillProperty =
        DependencyProperty.Register("LastChildFill", typeof(bool),
        typeof(DockingPanel), new PropertyMetadata(true, OnLastChildFillPropertyChanged));

        public static readonly DependencyProperty DockProperty =
        DependencyProperty.RegisterAttached("Dock", typeof(Dock),
        typeof(DockingPanel), new PropertyMetadata(Dock.Left, OnDockPropertyChanged));

        public bool LastChildFill
        {
            get { return (bool)GetValue(LastChildFillProperty); }
            set { SetValue(LastChildFillProperty, value); }
        }

        public static Dock GetDock(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return (Dock)element.GetValue(DockProperty);
        }

        public static void SetDock(UIElement element, Dock dock)
        {
            if (element == null) throw new ArgumentNullException("element");
            element.SetValue(DockProperty, dock);
        }

        protected override Size ArrangeOverride(Size size)
        {
            double left = 0.0;
            double top = 0.0;
            double right = 0.0;
            double bottom = 0.0;
            UIElementCollection children = Children;
            int count = children.Count - (LastChildFill ? 1 : 0);
            int index = 0;
            foreach (UIElement element in children)
            {
                Rect rect = new Rect(left, top,
                  Math.Max(0.0, size.Width - left - right),
                  Math.Max(0.0, size.Height - top - bottom));
                if (index < count)
                {
                    Size desiredSize = element.DesiredSize;
                    switch (GetDock(element))
                    {
                        case Dock.Left:
                            left += desiredSize.Width;
                            rect.Width = desiredSize.Width;
                            break;
                        case Dock.Top:
                            top += desiredSize.Height;
                            rect.Height = desiredSize.Height;
                            break;
                        case Dock.Right:
                            right += desiredSize.Width;
                            rect.X = Math.Max(0.0, size.Width - right);
                            rect.Width = desiredSize.Width;
                            break;
                        case Dock.Bottom:
                            bottom += desiredSize.Height;
                            rect.Y = Math.Max(0.0, size.Height - bottom);
                            rect.Height = desiredSize.Height;
                            break;
                    }
                }
                element.Arrange(rect);
                index++;
            }
            return size;
        }

        protected override Size MeasureOverride(Size size)
        {
            double width = 0.0;
            double height = 0.0;
            double maxWidth = 0.0;
            double maxHeight = 0.0;
            foreach (UIElement element in Children)
            {
                Size remainingSize = new Size(
                    Math.Max(0.0, size.Width - width),
                    Math.Max(0.0, size.Height - height));
                element.Measure(remainingSize);
                Size desired = element.DesiredSize;
                switch (GetDock(element))
                {
                    case Dock.Left:
                    case Dock.Right:
                        maxHeight = Math.Max(maxHeight, height + desired.Height);
                        width += desired.Width;
                        break;
                    case Dock.Top:
                    case Dock.Bottom:
                        maxWidth = Math.Max(maxWidth, width + desired.Width);
                        height += desired.Height;
                        break;
                }
            }
            maxWidth = Math.Max(maxWidth, width);
            maxHeight = Math.Max(maxHeight, height);
            return new Size(maxWidth, maxHeight);
        }
    }
}