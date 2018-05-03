using System;
using System.Collections.Generic;
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
    private const string app_title = "Hit or Miss";

    private const int score = 18;
    private const int size = 6;
    private const int hit = 1;
    private const int miss = 0;
    private readonly List<string> values = new List<string> { "Miss", "Hit" };

    private int _moves = 0;
    private int _hits = 0;
    private int _misses = 0;
    private bool _won = false;
    private int[,] _board = new int[size, size];
    private Random _random = new Random((int)DateTime.Now.Ticks);

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
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

    private Path GetPiece(string value)
    {
        Path path = new Path
        {
            Stretch = Stretch.Uniform,
            StrokeThickness = 5,
            Margin = new Thickness(5),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        if ((value == "Miss")) // Draw Miss
        {
            LineGeometry line1 = new LineGeometry
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(30, 30)
            };
            LineGeometry line2 = new LineGeometry
            {
                StartPoint = new Point(30, 0),
                EndPoint = new Point(0, 30)
            };
            GeometryGroup linegroup = new GeometryGroup();
            linegroup.Children.Add(line1);
            linegroup.Children.Add(line2);
            path.Data = linegroup;
            path.Stroke = new SolidColorBrush(Colors.Red);
        }
        else // Draw Hit
        {
            EllipseGeometry ellipse = new EllipseGeometry
            {
                Center = new Point(15, 15),
                RadiusX = 15,
                RadiusY = 15
            };
            path.Data = ellipse;
            path.Stroke = new SolidColorBrush(Colors.Blue);
        }
        return path;
    }

    private void Add(ref Grid grid, int row, int column)
    {
        Grid element = new Grid
        {
            Height = 50,
            Width = 50,
            Margin = new Thickness(5),
            Background = new SolidColorBrush(Colors.WhiteSmoke)
        };
        element.Tapped += (object sender, TappedRoutedEventArgs e) =>
        {
            if (!_won)
            {
                int selected;
                element = ((Grid)(sender));
                selected = _board[(int)element.GetValue(Grid.RowProperty),
                    (int)element.GetValue(Grid.ColumnProperty)];
                if (element.Children.Count <= 0)
                {
                    element.Children.Clear();
                    element.Children.Add(GetPiece(values[selected]));
                    if (selected == hit)
                    {
                        _hits++;
                    }
                    else if (selected == miss)
                    {
                        _misses++;
                    }
                    _moves++;
                }
                if (_moves < (size * size) && _misses < score)
                {
                    if (_hits == score)
                    {
                        Show($"Well Done! You scored {_hits} hits and {_misses} misses!", app_title);
                        _won = true;
                    }
                }
                else
                {
                    Show($"Game Over! You scored {_hits} hits and {_misses} misses!", app_title);
                    _won = true;
                }
            }
        };
        element.SetValue(Grid.ColumnProperty, column);
        element.SetValue(Grid.RowProperty, row);
        grid.Children.Add(element);
    }

    private void Layout(ref Grid grid)
    {
        _moves = 0;
        _hits = 0;
        _misses = 0;
        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for (int row = 0; (row < size); row++)
        {
            for (int column = 0; (column < size); column++)
            {
                Add(ref grid, row, column);
            }
        }
    }

    public void New(ref Grid grid)
    {
        Layout(ref grid);
        List<int> values = new List<int>();
        List<int> indices = new List<int>();
        _won = false;
        int counter = 0;
        while (values.Count < (size * size))
        {
            values.Add(hit);
            values.Add(miss);
        }
        indices = Select(1, (size * size), (size * size));
        // Setup Board
        for (int column = 0; (column < size); column++)
        {
            for (int row = 0; (row < size); row++)
            {
                _board[column, row] = values[indices[counter] - 1];
                counter++;
            }
        }
    }
}