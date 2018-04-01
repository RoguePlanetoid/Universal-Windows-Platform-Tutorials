using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string app_title = "Touch Game";
    private const int size = 2;
    private const int speed = 800;
    private const int light = 400;
    private const int click = 200;
    private const int level = 100;

    private readonly Color[] colours =
    {
        Colors.Crimson, Colors.Green,
        Colors.Blue, Colors.Gold
    };
    private readonly Color clicked = Colors.Black;
    private readonly Color lighted = Colors.WhiteSmoke;

    private int _turn = 0;
    private int _count = 0;
    private bool _play = false;
    private bool _isTimer = false;
    private List<int> _items = new List<int>();
    private DispatcherTimer _timer = new DispatcherTimer();
    private Random _random = new Random((int)DateTime.Now.Ticks);

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    private void Highlight(Grid grid, int value, int period, Color background)
    {
        Grid element = (Grid)grid.Children.Single(s =>
            (int)((Grid)s).Tag == value);
        element.Background = new SolidColorBrush(background);
        DispatcherTimer lightup = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(period)
        };
        lightup.Tick += (object sender, object e) =>
        {
            element.Background = new SolidColorBrush(colours[(int)element.Tag]);
            lightup.Stop();
        };
        lightup.Start();
    }

    private List<int> Shuffle(int start, int finish, int total)
    {
        int number;
        List<int> numbers = new List<int>();
        while ((numbers.Count < total)) // Select Numbers
        {
            // Random non-unique Number between Start and Finish
            number = _random.Next(start, finish + 1);
            numbers.Add(number); // Add Number
        }
        return numbers;
    }

    private void Add(Grid grid, int row, int column, int count)
    {
        Grid element = new Grid()
        {
            Margin = new Thickness(10),
            Height = 100,
            Width = 100,
            Tag = count,
            Background = new SolidColorBrush(colours[count])
        };
        element.Tapped += (object sender, TappedRoutedEventArgs e) =>
        {
            if (_play)
            {
                int value = (int)((Grid)sender).Tag;
                Highlight(grid, value, click, clicked);
                if (value == _items[_count])
                {
                    if (_count < _turn)
                    {
                        _count++;
                    }
                    else
                    {
                        _play = false;
                        _turn++;
                        _count = 0;
                        _isTimer = true;
                    }
                }
                else
                {
                    _isTimer = false;
                    Show($"Game Over! You scored {_turn}!", app_title);
                    _play = false;
                    _turn = 0;
                    _count = 0;
                    _timer.Stop();
                }
            }
        };
        element.SetValue(Grid.ColumnProperty, column);
        element.SetValue(Grid.RowProperty, row);
        grid.Children.Add(element);
    }

    private void Layout(ref Grid grid)
    {
        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        int count = 0;
        // Setup Board
        for (int column = 0; (column < size); column++)
        {
            for (int row = 0; (row < size); row++)
            {
                Add(grid, row, column, count);
                count++;
            }
        }
    }

    public void New(Grid grid)
    {
        Layout(ref grid);
        _items = Shuffle(0, 3, level);
        _play = false;
        _turn = 0;
        _count = 0;
        _isTimer = true;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(speed)
        };
        _timer.Tick += (object sender, object e) =>
        {
            if (_isTimer)
            {
                if (_count <= _turn)
                {
                    Highlight(grid, _items[_count], light, lighted);
                    _count++;
                }
                if (_count > _turn)
                {
                    _isTimer = false;
                    _play = true;
                    _count = 0;
                }
            }
        };
        _timer.Start();
    }
}