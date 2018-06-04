using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private const int size = 4;
    private const int on = 1;
    private const int off = 0;

    private readonly Color light_on = Colors.White;
    private readonly Color light_off = Colors.Black;

    private int[,] _board = new int[size, size];
    private DispatcherTimer _timer = null;
    private Random _random = new Random((int)DateTime.Now.Ticks);
    private List<int> _positions = new List<int>();
    private int _counter = 0;
    private int _turns = 0;
    private int _hits = 0;
    private int _wait = 0;
    private bool _waiting = false;
    private bool _lost = false;

    private List<int> Shuffle(int start, int end, int total)
    {
        return Enumerable.Range(start, total).OrderBy(r => _random.Next(start, end)).ToList();
    }

    private Rectangle Get(Color foreground)
    {
        Rectangle rect = new Rectangle()
        {
            Height = 80,
            Width = 80,
            Fill = new SolidColorBrush(foreground)
        };
        return rect;
    }

    private void Set(ref Grid grid, int row, int column)
    {
        Button button = (Button)grid.Children.Single(
                       w => Grid.GetRow((Button)w) == row
                       && Grid.GetColumn((Button)w) == column);
        button.Content = Get(_board[row, column] == on ? light_on : light_off);
    }

    private void Add(Grid grid, TextBlock text, int row, int column, int index)
    {
        string name = string.Empty;
        Button button = new Button()
        {
            Height = 100,
            Width = 100,
            Content = Get(light_off)
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            if (!_lost)
            {
                button = (Button)sender;
                row = (int)button.GetValue(Grid.RowProperty);
                column = (int)button.GetValue(Grid.ColumnProperty);
                if (_board[row, column] == on)
                {
                    _board[row, column] = off;
                    Set(ref grid, row, column);
                    _hits++;
                }
                else
                {
                    _lost = true;
                }
            }
            else
            {
                text.Text = $"Game Over in {_turns} Turns!";
            }
        };
        button.SetValue(Grid.ColumnProperty, column);
        button.SetValue(Grid.RowProperty, row);
        grid.Children.Add(button);
    }

    private void Layout(ref Grid grid, ref TextBlock text)
    {
        text.Text = string.Empty;
        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
        // Setup Grid
        for (int layout = 0; layout < size; layout++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        // Setup Board
        int index = 0;
        for (int column = 0; column < size; column++)
        {
            for (int row = 0; row < size; row++)
            {
                Add(grid, text, row, column, index);
                index++;
            }
        }
    }

    private void Reset(Grid grid)
    {
        for (int column = 0; column < size; column++)
        {
            for (int row = 0; row < size; row++)
            {
                _board[row, column] = off;
                Set(ref grid, row, column);
            }
        }
        _hits = 0;
        _counter = 0;
        _wait = Shuffle(3, 7, 1).First();
        _waiting = true;
    }

    private void Choose(Grid grid)
    {
        int row = 0;
        int column = 0;
        _positions = Shuffle(0, _board.Length, _board.Length);
        for (int i = 0; i < size; i++)
        {
            column = _positions[i] % size;
            switch (_positions[i])
            {
                case int n when n < 4:
                    row = 0;
                    break;
                case int n when n >= 4 && n < 8:
                    row = 1;
                    break;
                case int n when n >= 8 && n < 12:
                    row = 2;
                    break;
                case int n when n >= 12 && n <= 15:
                    row = 3;
                    break;
            }
            _board[row, column] = on;
            Set(ref grid, row, column);
        }
        _counter = 0;
        _wait = 0;
        _waiting = false;
    }

    private void Timer(Grid grid, TextBlock text)
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer = null;
        }
        _timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (object sender, object e) =>
        {
            if (!_lost)
            {
                int countdown = 0;
                if (_waiting)
                {
                    countdown = (_wait - _counter) + 1;
                    text.Text = $"Waiting {countdown}";
                    if (countdown == 0)
                    {
                        text.Text = string.Empty;
                        Choose(grid);
                    }
                }
                else
                {
                    countdown = (size - _counter) + 1;
                    text.Text = $"Solve In {countdown}";
                    if (countdown == 0)
                    {
                        if (_hits == size)
                        {
                            _turns++;
                            text.Text = string.Empty;
                            Reset(grid);
                        }
                        else
                        {
                            _lost = true;
                            text.Text = $"Game Over in {_turns} Turns!";
                            Reset(grid);
                        }
                    }
                }
                _counter++;
            }
        };
        _timer.Start();
    }

    public void New(ref Grid grid, ref TextBlock text)
    {
        _lost = false;
        _waiting = true;
        _wait = 3;
        _turns = 1;
        Layout(ref grid, ref text);
        Timer(grid, text);
    }
}