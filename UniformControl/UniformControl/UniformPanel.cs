using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniformControl
{
    public class UniformPanel : Panel
    {
        private int _columns;
        private int _rows;

        public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register("Columns", typeof(int),
        typeof(UniformPanel), new PropertyMetadata(0));

        public static readonly DependencyProperty FirstColumnProperty =
        DependencyProperty.Register("FirstColumn", typeof(int),
        typeof(UniformPanel), new PropertyMetadata(0));

        public static readonly DependencyProperty RowsProperty =
        DependencyProperty.Register("Rows", typeof(int),
        typeof(UniformPanel), new PropertyMetadata(0));

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public int FirstColumn
        {
            get { return (int)GetValue(FirstColumnProperty); }
            set { SetValue(FirstColumnProperty, value); }
        }

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        private void UpdateComputedValues()
        {
            _columns = Columns;
            _rows = Rows;
            if (FirstColumn >= _columns) FirstColumn = 0;
            if ((_rows == 0) || (_columns == 0))
            {
                var row = 0;
                var column = 0;
                int count = Children.Count;
                while (column < count)
                {
                    UIElement element = Children[column];
                    if (element.Visibility != Visibility.Collapsed)
                    {
                        row++;
                    }
                    column++;
                }
                if (row == 0) row = 1;
                if (_rows == 0)
                {
                    if (_columns > 0)
                    {
                        _rows = ((row + FirstColumn) + (_columns - 1)) / _columns;
                    }
                    else
                    {
                        _rows = (int)Math.Sqrt(row);
                        if ((_rows * _rows) < row)
                        {
                            _rows++;
                        }
                        _columns = _rows;
                    }
                }
                else if (_columns == 0)
                {
                    _columns = (row + (_rows - 1)) / _rows;
                }
            }
        }

        protected override Size ArrangeOverride(Size size)
        {
            Rect rectangle = new Rect(0.0, 0.0,
            size.Width / _columns, size.Height / _rows);
            double width = rectangle.Width;
            double value = size.Width - 1.0;
            rectangle.X += rectangle.Width * FirstColumn;
            foreach (UIElement element in Children)
            {
                element.Arrange(rectangle);
                if (element.Visibility != Visibility.Collapsed)
                {
                    rectangle.X += width;
                    if (rectangle.X >= value)
                    {
                        rectangle.Y += rectangle.Height;
                        rectangle.X = 0.0;
                    }
                }
            }
            return size;
        }

        protected override Size MeasureOverride(Size size)
        {
            UpdateComputedValues();
            Size available = new Size(size.Width / (_columns), size.Height / (_rows));
            double width = 0.0;
            double height = 0.0;
            int value = 0;
            int count = Children.Count;
            while (value < count)
            {
                UIElement element = Children[value];
                element.Measure(available);
                Size desired = element.DesiredSize;
                if (width < desired.Width) width = desired.Width;
                if (height < desired.Height) height = desired.Height;
                value++;
            }
            return new Size(width * _columns, height * _rows);
        }
    }
}