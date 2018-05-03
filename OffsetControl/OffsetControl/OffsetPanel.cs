using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OffsetControl
{
    public class OffsetPanel : Panel
    {
        private bool ignorePropertyChange;

        private static void OnMaximumColumnsPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            OffsetPanel source = (OffsetPanel)d;
            int value = (int)e.NewValue;
            if (source.ignorePropertyChange)
            {
                source.ignorePropertyChange = false;
                return;
            }
            if (value < int.MinValue || value > int.MaxValue)
            {
                source.ignorePropertyChange = true;
                source.SetValue(e.Property, (int)e.OldValue);
                throw new ArgumentException("OnMaximumColumnsPropertyChanged InvalidValue", "value");
            }
            source.InvalidateMeasure();
        }

        private static void OnItemHeightOrWidthPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            OffsetPanel source = (OffsetPanel)d;
            double value = (double)e.NewValue;
            if (source.ignorePropertyChange)
            {
                source.ignorePropertyChange = false;
                return;
            }
            if (!double.IsNaN(value) && ((value <= 0.0) || double.IsPositiveInfinity(value)))
            {
                source.ignorePropertyChange = true;
                source.SetValue(e.Property, (double)e.OldValue);
                throw new ArgumentException("OnItemHeightOrWidthPropertyChanged InvalidValue", "value");
            }
            source.InvalidateMeasure();
        }

        public static readonly DependencyProperty MaximumColumnsProperty =
        DependencyProperty.Register("MaximumColumns", typeof(int), typeof(OffsetPanel),
        new PropertyMetadata(2, OnMaximumColumnsPropertyChanged));

        public static readonly DependencyProperty ColumnOffsetProperty =
        DependencyProperty.Register("ColumnOffset", typeof(double), typeof(OffsetPanel),
        new PropertyMetadata(10.0, OnItemHeightOrWidthPropertyChanged));

        public static readonly DependencyProperty RowOffsetProperty =
        DependencyProperty.Register("RowOffset", typeof(double), typeof(OffsetPanel),
        new PropertyMetadata(5.0, OnItemHeightOrWidthPropertyChanged));

        public static readonly DependencyProperty SpacingYProperty =
        DependencyProperty.Register("SpacingY", typeof(double), typeof(OffsetPanel),
        new PropertyMetadata(10.0, OnItemHeightOrWidthPropertyChanged));

        public static readonly DependencyProperty SpacingXProperty =
        DependencyProperty.Register("SpacingX", typeof(double), typeof(OffsetPanel),
        new PropertyMetadata(10.0, OnItemHeightOrWidthPropertyChanged));

        public int MaximumColumns
        {
            get { return (int)GetValue(MaximumColumnsProperty); }
            set { SetValue(MaximumColumnsProperty, value); }
        }

        public double ColumnOffset
        {
            get { return (double)GetValue(ColumnOffsetProperty); }
            set { SetValue(ColumnOffsetProperty, value); }
        }

        public double RowOffset
        {
            get { return (double)GetValue(RowOffsetProperty); }
            set { SetValue(RowOffsetProperty, value); }
        }

        public double SpacingX
        {
            get { return (double)GetValue(SpacingXProperty); }
            set { SetValue(SpacingXProperty, value); }
        }

        public double SpacingY
        {
            get { return (double)GetValue(SpacingYProperty); }
            set { SetValue(SpacingYProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double itemWidth = 0.0;
            double itemHeight = 0.0;
            double x = 0;
            double y = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement element = Children[i];
                element.Measure(availableSize);
                double width = element.DesiredSize.Width + x;
                double height = element.DesiredSize.Height + y;
                if (width > itemWidth) itemWidth = width;
                if (height > itemHeight) itemHeight = height;
                y += SpacingY;
                if ((i + 1) % MaximumColumns == 0)
                {
                    x -= SpacingX * (MaximumColumns - 1);
                    x += RowOffset;
                    y += ColumnOffset;
                }
                else
                {
                    x += SpacingX;
                }
            }
            return new Size(itemWidth, itemHeight);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;
            double y = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement element = Children[i];
                element.Arrange(new Rect(new Point(x, y),
                    element.DesiredSize));
                y += SpacingY;
                if ((i + 1) % MaximumColumns == 0)
                {
                    x -= SpacingX * (MaximumColumns - 1);
                    x += RowOffset;
                    y += ColumnOffset;
                }
                else
                {
                    x += SpacingX;
                }
            }
            return finalSize;
        }
    }
}