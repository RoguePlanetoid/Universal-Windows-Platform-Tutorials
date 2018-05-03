using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

public class Library
{
    private const string app_title = "Order Game";
    private const int size = 6;
    private const int total = size * size;

    private DateTime _timer;
    private ObservableCollection<int> _list = new ObservableCollection<int>();
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

    private bool Winner()
    {
        return _list.OrderBy(o => o).ToList().SequenceEqual(_list.ToList());
    }

    private void Layout(ref GridView grid)
    {
        _timer = DateTime.UtcNow;
        grid.IsEnabled = true;
        grid.ItemsSource = null;
        _list = new ObservableCollection<int>();
        List<int> numbers = Select(1, total, total);
        int index = 0;
        while (index < numbers.Count)
        {
            _list.Add(numbers[index]);
            index++;
        }
        grid.ItemsSource = _list;
    }

    public void New(ref GridView grid)
    {
        Layout(ref grid);
    }

    public void Order(ref GridView grid)
    {
        if (Winner())
        {
            TimeSpan duration = (DateTime.UtcNow - _timer).Duration();
            Show($"Well Done! Completed in {duration.Hours} Hours, {duration.Minutes} Minutes and {duration.Seconds} Seconds!", app_title);
            grid.IsEnabled = false;
        }
    }
}