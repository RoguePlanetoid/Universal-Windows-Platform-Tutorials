using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private const string app_title = "Fruit Game";
    private const int size = 3;
    private readonly string[] values = { "Apple", "Lemon", "Orange", "Strawberry", "Blackberry", "Cherry" };

    private int _spins = 0;
    private int[] _board = new int[size];
    private Random _random = new Random((int)DateTime.Now.Ticks);

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    private Path Ellipse(int x, int y, Color fill)
    {
        return new Path()
        {
            Data = new EllipseGeometry()
            {
                Center = new Point(20, 20),
                RadiusX = x,
                RadiusY = y,
            },
            StrokeThickness = 5,
            Margin = new Thickness(10),
            Fill = new SolidColorBrush(fill)
        };
    }

    private UIElement Fruit(int type)
    {
        switch (type)
        {
            case 0: // Apple
                return Ellipse(20, 18, Colors.LawnGreen);
            case 1: // Lemon
                return Ellipse(16, 20, Colors.Yellow);
            case 2: // Orange
                return Ellipse(20, 20, Colors.Orange);
            case 3: // Strawberry
                return new Polygon()
                {
                    Points = new PointCollection
                    {
                        new Point(0, 0),
                        new Point(15, 30),
                        new Point(30, 0)
                    },
                    Stretch = Stretch.Uniform,
                    StrokeLineJoin = PenLineJoin.Round,
                    Height = 40,
                    Width = 40,
                    Fill = new SolidColorBrush(Colors.IndianRed),
                    Stroke = new SolidColorBrush(Colors.IndianRed),
                    StrokeThickness = 5,
                    Margin = new Thickness(10)
                };
            case 4: // Blackberry
                return new Path()
                {
                    Data = new GeometryGroup()
                    {
                        Children = new GeometryCollection()
                        {
                            new EllipseGeometry()
                            {
                                Center = new Point(10, 10),
                                RadiusX = 8, RadiusY = 8
                            },
                            new EllipseGeometry()
                            {
                                Center = new Point(30, 10),
                                RadiusX = 8, RadiusY = 8
                            },
                            new EllipseGeometry()
                            {
                                Center = new Point(20, 30),
                                RadiusX = 8, RadiusY = 8
                            }
                        }
                    },
                    Fill = new SolidColorBrush(Colors.MediumPurple),
                    StrokeThickness = 5,
                    Margin = new Thickness(10)
                };
            case 5: // Cherry
                return new Path()
                {
                    Data = new GeometryGroup()
                    {
                        Children = new GeometryCollection()
                        {
                            new EllipseGeometry()
                            {
                                Center = new Point(10, 25),
                                RadiusX = 8, RadiusY = 8
                            },
                            new EllipseGeometry()
                            {
                                Center = new Point(30, 25),
                                RadiusX = 8, RadiusY = 8
                            }
                        }
                    },
                    Fill = new SolidColorBrush(Colors.MediumVioletRed),
                    StrokeThickness = 5,
                    Margin = new Thickness(10)
                };
            default:
                return null;
        }
    }

    private void Add(ref Grid grid, int column, int type)
    {
        Viewbox viewbox = new Viewbox()
        {
            Height = 100,
            Width = 100,
            Stretch = Stretch.UniformToFill,
            Child = Fruit(type),
        };
        viewbox.SetValue(Grid.ColumnProperty, column);
        grid.Children.Add(viewbox);
    }

    private void Layout(ref Grid grid)
    {
        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        // Setup Grid
        for (int column = 0; (column < size); column++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            _board[column] = _random.Next(0, 6);
            Add(ref grid, column, _board[column]);
        }
    }

    public void New(ref Grid grid)
    {
        Layout(ref grid);
        _spins++;
        // Check Winner        
        if (_board.All(item => item == _board.First()))
        {
            Show($"Spin {_spins} matched {values[_board.First()]}", app_title);
            _spins = 0;
        }
    }
}