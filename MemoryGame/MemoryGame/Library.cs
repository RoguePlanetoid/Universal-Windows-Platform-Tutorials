using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private const string app_title = "Memory Game";
    private const int size = 4;

    private int _moves = 0;
    private int _firstId = 0;
    private int _secondId = 0;
    private Canvas _first;
    private Canvas _second;
    private int[,] _board = new int[size, size];
    private List<int> _matches = new List<int>();
    private Random _random = new Random((int)DateTime.Now.Ticks);

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    private UIElement Shape(ref PointCollection points, Color fill)
    {
        Polygon polygon = new Polygon
        {
            Height = 40,
            Width = 40,
            Stretch = Stretch.Uniform,
            StrokeLineJoin = PenLineJoin.Round,
            Points = points,
            Fill = new SolidColorBrush(fill),
            Stroke = new SolidColorBrush(fill),
            StrokeThickness = 5,
            Margin = new Thickness(10)
        };
        return polygon;
    }

    private UIElement Card(int type)
    {
        PointCollection points = new PointCollection();
        switch (type)
        {
            case 1: // Circle
                Path circle = new Path();
                EllipseGeometry ellipse = new EllipseGeometry
                {
                    Center = new Point(20, 20),
                    RadiusX = 20,
                    RadiusY = 20
                };
                circle.Data = ellipse;
                circle.Fill = new SolidColorBrush(Colors.Blue);
                circle.Stroke = new SolidColorBrush(Colors.Blue);
                circle.Margin = new Thickness(10);
                return circle;
            case 2: // Cross
                Path cross = new Path();
                LineGeometry line1 = new LineGeometry
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(40, 40)
                };
                LineGeometry line2 = new LineGeometry
                {
                    StartPoint = new Point(40, 0),
                    EndPoint = new Point(0, 40)
                };
                GeometryGroup linegroup = new GeometryGroup();
                linegroup.Children.Add(line1);
                linegroup.Children.Add(line2);
                cross.Data = linegroup;
                cross.Stroke = new SolidColorBrush(Colors.Red);
                cross.StrokeThickness = 5;
                cross.Margin = new Thickness(10);
                return cross;
            case 3: // Triangle
                points.Add(new Point(150, 0));
                points.Add(new Point(0, 250));
                points.Add(new Point(300, 250));
                return Shape(ref points, Colors.Green);
            case 4: // Square
                points.Add(new Point(0, 0));
                points.Add(new Point(0, 100));
                points.Add(new Point(100, 100));
                points.Add(new Point(100, 0));
                return Shape(ref points, Colors.DarkMagenta);
            case 5: // Pentagon
                points.Add(new Point(0, 125));
                points.Add(new Point(150, 0));
                points.Add(new Point(300, 125));
                points.Add(new Point(250, 300));
                points.Add(new Point(50, 300));
                return Shape(ref points, Colors.Crimson);
            case 6: // Hexagon
                points.Add(new Point(75, 0));
                points.Add(new Point(225, 0));
                points.Add(new Point(300, 150));
                points.Add(new Point(225, 300));
                points.Add(new Point(75, 300));
                points.Add(new Point(0, 150));
                return Shape(ref points, Colors.DarkCyan);
            case 7: // Star
                points.Add(new Point(9, 2));
                points.Add(new Point(11, 7));
                points.Add(new Point(17, 7));
                points.Add(new Point(12, 10));
                points.Add(new Point(14, 15));
                points.Add(new Point(9, 12));
                points.Add(new Point(4, 15));
                points.Add(new Point(6, 10));
                points.Add(new Point(1, 7));
                points.Add(new Point(7, 7));
                return Shape(ref points, Colors.Gold);
            case 8: // Rhombus
                points.Add(new Point(50, 0));
                points.Add(new Point(100, 50));
                points.Add(new Point(50, 100));
                points.Add(new Point(0, 50));
                return Shape(ref points, Colors.OrangeRed);
            default:
                return null;
        }
    }

    private void Add(ref Grid grid, int row, int column)
    {
        Canvas canvas = new Canvas
        {
            Height = 60,
            Width = 60,
            Margin = new Thickness(10),
            Background = new SolidColorBrush(Colors.WhiteSmoke)
        };
        canvas.Tapped += async (object sender, TappedRoutedEventArgs e) =>
        {
            int selected;
            canvas = ((Canvas)(sender));
            row = (int)canvas.GetValue(Grid.RowProperty);
            column = (int)canvas.GetValue(Grid.ColumnProperty);
            selected = _board[row, column];
            if ((_matches.IndexOf(selected) < 0))
            {
                if ((_firstId == 0)) // No Match
                {
                    _first = canvas;
                    _firstId = selected;
                    _first.Children.Clear();
                    _first.Children.Add(Card(selected));
                }
                else if ((_secondId == 0))
                {
                    _second = canvas;
                    if (!_first.Equals(_second)) // Different
                    {
                        _secondId = selected;
                        _second.Children.Clear();
                        _second.Children.Add(Card(selected));
                        if ((_firstId == _secondId)) // Is Match
                        {
                            _matches.Add(_firstId);
                            _matches.Add(_secondId);
                            if (!(_first == null))
                            {
                                _first.Background = null;
                                _first = null;
                            }
                            if (!(_second == null))
                            {
                                _second.Background = null;
                                _second = null;
                            }
                            if ((_matches.Count == 16))
                            {
                                Show($"Well Done! You matched them all in {_moves} moves!", app_title);
                            }
                        }
                        else // No Match
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1.5));
                            if (!(_first == null))
                            {
                                _first.Children.Clear();
                                _first = null;
                            }
                            if (!(_second == null))
                            {
                                _second.Children.Clear();
                                _second = null;
                            }
                        }
                        _moves++;
                        _firstId = 0;
                        _secondId = 0;
                    }
                }
            }
        };
        canvas.SetValue(Grid.ColumnProperty, column);
        canvas.SetValue(Grid.RowProperty, row);
        grid.Children.Add(canvas);
    }

    private void Layout(ref Grid grid)
    {
        _moves = 0;
        _matches.Clear();
        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        // Setup Board
        for (int row = 0; (row < size); row++)
        {
            for (int column = 0; (column < size); column++)
            {
                Add(ref grid, row, column);
            }
        }
    }

    private List<int> Select(int start, int finish, int total)
    {
        int number;
        List<int> numbers = new List<int>();
        while ((numbers.Count < total)) // Select Numbers
        {
            // Random Number between Start and Finish
            number = _random.Next(start, finish + 1);
            if ((!numbers.Contains(number)) || (numbers.Count < 1))
            {
                numbers.Add(number); // Add if number Chosen or None
            }
        }
        return numbers;
    }

    public void New(Grid grid)
    {
        Layout(ref grid);
        List<int> values = new List<int>();
        List<int> indices = new List<int>();
        int counter = 0;
        while (values.Count <= size * size)
        {
            List<int> numbers = Select(1, size * 2, size * 2); // Random 1 - 8
            for (int number = 0; number < size * 2; number++)
            {
                values.Add(numbers[number]); // Add to Cards
            }
        }
        indices = Select(1, size * size, size * size); // Random 1 - 16
        for (int column = 0; column < size; column++) // Board Columns
        {
            for (int row = 0; row < size; row++) // Board Rows
            {
                _board[column, row] = values[indices[counter] - 1];
                counter++;
            }
        }
    }
}