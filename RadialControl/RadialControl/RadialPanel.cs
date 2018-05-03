using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RadialControl
{
    public partial class RadialPanel : Panel
    {
        private bool _ignorePropertyChange;

        private static void OnIsOrientedPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            RadialPanel source = (RadialPanel)d;
            bool value = (bool)e.NewValue;
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }
            source.InvalidateMeasure();
        }

        private static void OnItemHeightOrWidthPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            RadialPanel source = (RadialPanel)d;
            double value = (double)e.NewValue;
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }
            if (!double.IsNaN(value) && ((value <= 0.0) || double.IsPositiveInfinity(value)))
            {
                source._ignorePropertyChange = true;
                source.SetValue(e.Property, (double)e.OldValue);
                throw new ArgumentException("OnItemHeightOrWidthPropertyChanged InvalidValue", "value");
            }
            source.InvalidateMeasure();
        }

        public static readonly DependencyProperty ItemHeightProperty =
        DependencyProperty.Register("ItemHeight", typeof(double), typeof(RadialPanel),
        new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));

        public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register("ItemWidth", typeof(double), typeof(RadialPanel),
        new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));

        public static readonly DependencyProperty IsOrientedProperty =
        DependencyProperty.Register("IsOriented", typeof(bool), typeof(RadialPanel),
        new PropertyMetadata(false, OnIsOrientedPropertyChanged));

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public bool IsOriented
        {
            get { return (bool)GetValue(IsOrientedProperty); }
            set { SetValue(IsOrientedProperty, value); }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            Size itemSize = new Size(
                hasFixedWidth ? itemWidth : constraint.Width,
                hasFixedHeight ? itemHeight : constraint.Height);
            foreach (UIElement element in Children)
            {
                element.Measure(itemSize);
            }
            return itemSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            double radiusX = finalSize.Width * 0.5;
            double radiusY = finalSize.Height * 0.5;
            double count = Children.Count;
            double deltaAngle = 2 * Math.PI / count;
            Point centre = new Point(finalSize.Width / 2,
                finalSize.Height / 2);
            for (int i = 0; i < count; i++)
            {
                UIElement element = Children[i];
                Size elementSize = new Size(
                hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                hasFixedHeight ? itemHeight : element.DesiredSize.Height);
                double angle = i * deltaAngle;
                double x = centre.X + radiusX * Math.Cos(angle)
                    - elementSize.Width / 2;
                double y = centre.Y + radiusY * Math.Sin(angle)
                    - elementSize.Height / 2;
                if (IsOriented)
                {
                    element.RenderTransform = null;
                }
                else
                {
                    element.RenderTransformOrigin = new Point(0.5, 0.5);
                    element.RenderTransform = new RotateTransform()
                    {
                        Angle = angle * 180 / Math.PI
                    };
                }
                element.Arrange(new Rect(x, y,
                    elementSize.Width, elementSize.Height));
            }
            return finalSize;
        }
    }
}