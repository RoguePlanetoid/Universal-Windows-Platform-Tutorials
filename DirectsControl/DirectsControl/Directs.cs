using System;
using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace DirectsControl
{
    public class Directs : Grid
    {
        private const int size = 3;
        private const string path_up = "M 0,0 40,0 40,60 20,80 0,60 0,0 z";
        private const string path_down = "M 0,20 20,0 40,20 40,80 0,80 z";
        private const string path_left = "M 0,0 60,0 80,20 60,40 0,40 z";
        private const string path_right = "M 0,20 20,0 80,0 80,40 20,40 z";

        public enum Directions
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3
        }

        public delegate void DirectionEvent(object sender, Directions direction);
        public event DirectionEvent Direction;

        public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Brush),
        typeof(Directs), null);

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        private Path StringToPath(string value)
        {
            return (Path)XamlReader.Load(
                $"<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><Path.Data>{value}</Path.Data></Path>"
            );
        }

        private void Path_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(this);
            bool fire = (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse) ?
                point.Properties.IsLeftButtonPressed : point.IsInContact;
            if (fire)
            {
                Path path = ((Path)sender);
                if (Direction != null)
                {
                    this.Direction(path, (Directions)Enum.Parse(typeof(Directions), path.Name));
                }
            }
        }

        private void Add(ref Grid grid,
            string name, string value,
            int row, int column,
            int? rowspan, int? columnspan,
            VerticalAlignment? vertical = null,
            HorizontalAlignment? horizontal = null)
        {
            Path path = StringToPath(value);
            path.Name = name;
            path.Margin = new Thickness(5);
            if (vertical != null) path.VerticalAlignment = vertical.Value;
            if (horizontal != null) path.HorizontalAlignment = horizontal.Value;
            path.SetBinding(Path.FillProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Foreground"),
                Mode = BindingMode.TwoWay
            });
            path.PointerMoved += Path_PointerMoved;
            path.SetValue(Grid.RowProperty, row);
            path.SetValue(Grid.ColumnProperty, column);
            if (rowspan != null) path.SetValue(Grid.RowSpanProperty, rowspan);
            if (columnspan != null) path.SetValue(Grid.ColumnSpanProperty, columnspan);
            grid.Children.Add(path);
        }

        public Directs()
        {
            Grid grid = new Grid()
            {
                Height = 180,
                Width = 180
            };
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            for (int index = 0; (index < size); index++)
            {
                grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = (index == 1) ? GridLength.Auto : new GridLength(100, GridUnitType.Star)
                });
                grid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = (index == 1) ? GridLength.Auto : new GridLength(100, GridUnitType.Star)
                });
            }
            Add(ref grid, "Up", path_up, 0, 1, 2, null, VerticalAlignment.Top, null);
            Add(ref grid, "Down", path_down, 1, 1, 2, null, VerticalAlignment.Bottom, null);
            Add(ref grid, "Left", path_left, 1, 0, null, 2, null, HorizontalAlignment.Left);
            Add(ref grid, "Right", path_right, 1, 1, null, 2, null, HorizontalAlignment.Right);
            Viewbox box = new Viewbox()
            {
                Child = grid
            };
            this.Children.Add(box);
        }
    }
}