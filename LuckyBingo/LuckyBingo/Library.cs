using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private int _index = 0;
    private List<int> _numbers = null;
    private ObservableCollection<Grid> _list = new ObservableCollection<Grid>();
    private Random _random = new Random((int)DateTime.Now.Ticks);

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

    private void Pick(ref GridView grid)
    {
        if (_index < _numbers.Count)
        {
            int number = _numbers[_index];
            TextBlock text = new TextBlock()
            {
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 24,
                Text = number.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid container = new Grid()
            {
                Margin = new Thickness(2),
                Width = 48,
                Height = 48
            };
            Color fill = Colors.Transparent;
            if (number >= 1 && number <= 10)
            {
                fill = Colors.Magenta;
            }
            else if (number >= 11 && number <= 20)
            {
                fill = Colors.Blue;
            }
            else if (number >= 21 && number <= 30)
            {
                fill = Colors.DarkGreen;
            }
            else if (number >= 31 && number <= 40)
            {
                fill = Colors.YellowGreen;
            }
            else if (number >= 41 && number <= 50)
            {
                fill = Colors.Orange;
            }
            else if (number >= 51 && number <= 60)
            {
                fill = Colors.DarkBlue;
            }
            else if (number >= 61 && number <= 70)
            {
                fill = Colors.Red;
            }
            else if (number >= 71 && number <= 80)
            {
                fill = Colors.DarkCyan;
            }
            else if (number >= 81 && number <= 90)
            {
                fill = Colors.Purple;
            }
            Ellipse ball = new Ellipse
            {
                Width = container.Width,
                Height = container.Height,
                Fill = new SolidColorBrush(fill)
            };
            container.Children.Add(ball);
            container.Children.Add(text);
            _list.Add(container);
            _index++;
            grid.ItemsSource = _list;
        }
    }

    private void Layout(ref GridView grid)
    {
        grid.ItemsSource = null;
        _list = new ObservableCollection<Grid>();
        _index = 0;
        _numbers = Select(1, 90, 90);
    }

    public void New(GridView grid)
    {
        Layout(ref grid);
    }

    public void Pick(GridView grid)
    {
        if (_numbers == null) Layout(ref grid);
        Pick(ref grid);
    }
}