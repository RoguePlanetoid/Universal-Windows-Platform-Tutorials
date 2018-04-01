using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private Random _random = new Random((int)DateTime.Now.Ticks);

    private List<int> Choose()
    {
        int number;
        List<int> numbers = new List<int>();
        while ((numbers.Count < 6)) // Select 6 Numbers
        {
            number = _random.Next(1, 60);
            if ((!numbers.Contains(number)) || (numbers.Count < 1))
            {
                numbers.Add(number); // Add if not Chosen or None
            }
        }
        numbers.Sort();
        return numbers;
    }

    public void New(StackPanel stack)
    {
        stack.Children.Clear();
        List<int> numbers = Choose();
        foreach (int number in numbers)
        {
            Grid container = new Grid()
            {
                Height = 60,
                Width = 60,
                Background = new SolidColorBrush(Colors.WhiteSmoke)
            };
            Ellipse ball = new Ellipse()
            {
                Height = 50,
                Width = 50,
                StrokeThickness = 5,
                Margin = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            if (number >= 1 && number <= 9)
            {
                ball.Fill = new SolidColorBrush(Colors.White);
            }
            else if (number >= 10 && number <= 19)
            {
                ball.Fill = new SolidColorBrush(Colors.Cyan);
            }
            else if (number >= 20 && number <= 29)
            {
                ball.Fill = new SolidColorBrush(Colors.Magenta);
            }
            else if (number >= 30 && number <= 39)
            {
                ball.Fill = new SolidColorBrush(Colors.LawnGreen);
            }
            else if (number >= 40 && number <= 49)
            {
                ball.Fill = new SolidColorBrush(Colors.Yellow);
            }
            else if (number >= 50 && number <= 59)
            {
                ball.Fill = new SolidColorBrush(Colors.Purple);
            }
            container.Children.Add(ball);
            TextBlock label = new TextBlock()
            {
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = number.ToString()
            };
            container.Children.Add(label);
            stack.Children.Add(container);
        }
    }
}