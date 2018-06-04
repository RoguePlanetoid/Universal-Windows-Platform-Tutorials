using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private const string app_title = "High or Low";
    private const string club = "M 19.5155,51.3 C 17.1155,50.1 15.9155,43.7 " +
        "21.5155,43.7 C 27.1155,43.7 25.9155,50.1 23.5155,51.3 C 25.1155,50.1 " +
        "30.3155,48.5 30.3155,54.1 C 30.3155,59.7 23.1155,59.3 22.7155,54.9 L " +
        "21.9155,54.9 C 21.9155,54.9 22.3155,59.3 23.5155,61.3 L 19.5155,61.3 C " +
        "20.7155,59.3 21.1155,54.9 21.1155,54.9 L 20.3155,54.9 C 19.9155,59.3 " +
        "12.3155,59.7 12.3155,54.1 C 12.3155,49.3 17.9155,50.1 19.5155,51.3 z";
    private const string diamond = "M 170.1155,199.8 L 177.3155,191 L 184.5155,199.4 " +
        "L 177.3155,209 L 170.1155,199.8 z";
    private const string heart = "M 99.5,99.75 C 99.5,93.75 89.5,81.75 79.5,81.75 C " +
        "69.5,81.75 59.5,89.75 59.5,103.75 C 59.5,125.75 91.5,161.75 99.5,171.75 C " +
        "107.5,161.75 139.5,125.75 139.5,103.75 C 139.5,89.75 129.5,81.75 119.5,81.75 C " +
        "109.45012,81.75 99.5,93.75 99.5,99.75 z";
    private const string spade = "M 21.1155,43.3 C 17.9155,48.1 13.7155,50.9 13.7155,54.9 " +
        "C 13.7155,58.5 15.7155,59.3 16.9155,59.3 C 18.5155,59.3 19.7155,58.5 19.7155,55.7 " +
        "C 19.7155,54.9 20.5155,54.9 20.5155,55.7 C 20.5155,59.7 19.7155,59.7 18.9155,61.7 " +
        "L 23.3155,61.7 C 22.5155,59.7 21.7155,59.7 21.7155,55.7 C 21.7155,54.9 22.5155,54.9 " +
        "22.5155,55.7 C 22.5155,58.9 23.7155,59.3 25.3155,59.3 C 26.5155,59.3 28.9155,58.5 " +
        "28.9155,54.9 C 28.9155,50.84784 24.3155,48.1 21.1155,43.3 z";

    private string[] card_pips = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r" };
    private string[] card_values = { "K", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q" };

    private int[][] table =
    {
        //          a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 0
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 1
        new int[] { 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 2
        new int[] { 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 }, // 3
        new int[] { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 }, // 4
        new int[] { 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0 }, // 5
        new int[] { 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0 }, // 6
        new int[] { 1, 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0 }, // 7
        new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0 }, // 8
        new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0 }, // 9
        new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1, 1, 0 }, // 10
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 11
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 12
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 13
    };

    private List<int> _deck = new List<int>();
    private Random _random = new Random((int)DateTime.Now.Ticks);
    private int _turn;
    private int _value;
    private bool _lost;

    private List<int> Shuffle(int total)
    {
        return Enumerable.Range(0, total).OrderBy(r => _random.Next(0, total)).ToList();
    }

    public void ShowAsync(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    private Geometry GetGeometry(string data)
    {
        return (Geometry)XamlReader.Load(
            $"<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{data}</Geometry>"
        );
    }

    private Path AddPip(string symbol, Color color,
        double height, int margin, string name)
    {
        return new Path()
        {
            Name = name,
            Data = GetGeometry(symbol),
            Fill = new SolidColorBrush(color),
            Height = height,
            Stretch = Stretch.Uniform,
            Margin = new Thickness(margin),
            Opacity = 0
        };
    }

    private void Add(ref Grid grid,
        int row, int rowspan, int column,
        string symbol, Color color, string name)
    {
        bool flip = row > 2;
        Path pip = AddPip(symbol, color, 100, 5, name);
        if (flip)
        {
            pip.RenderTransform = new ScaleTransform() { ScaleY = -1, CenterY = 50 };
        }
        pip.SetValue(Grid.RowProperty, row);
        pip.SetValue(Grid.RowSpanProperty, rowspan);
        pip.SetValue(Grid.ColumnProperty, column);
        grid.Children.Add(pip);
    }

    private void AddItem(ref Grid grid,
        int row, int column, string symbol,
        Color color, string value, bool flip, string name)
    {
        StackPanel item = new StackPanel();
        Path pip = AddPip(symbol, color, 40, 0, $"{name}.pip");
        if (flip)
        {
            pip.RenderTransform = new ScaleTransform() { ScaleY = -1, CenterY = 20 };
        }
        item.Children.Add(pip);
        item.Children.Add(new TextBlock()
        {
            Name = $"{name}.num",
            HorizontalAlignment = HorizontalAlignment.Center,
            Text = value,
            Foreground = new SolidColorBrush(color),
            FontSize = 40,
            Opacity = 0
        });
        item.SetValue(Grid.ColumnProperty, column);
        item.SetValue(Grid.RowProperty, row);
        grid.Children.Add(item);
    }

    private void AddFace(ref Grid grid,
        int row, int rowspan,
        int column, int colspan,
        Color color, string value, string name)
    {
        TextBlock face = new TextBlock()
        {
            Name = name,
            HorizontalAlignment = HorizontalAlignment.Center,
            Text = value,
            Foreground = new SolidColorBrush(color),
            FontSize = 300
        };
        face.SetValue(Grid.RowProperty, row);
        face.SetValue(Grid.RowSpanProperty, rowspan);
        face.SetValue(Grid.ColumnProperty, column);
        face.SetValue(Grid.ColumnSpanProperty, colspan);
        face.Opacity = 0;
        grid.Children.Add(face);
    }

    private Viewbox Card(string name, Color back)
    {
        string suit = club;
        Grid card = new Grid()
        {
            Name = name,
            Margin = new Thickness(10),
            Padding = new Thickness(10, 20, 10, 10),
            CornerRadius = new CornerRadius(10),
            BorderThickness = new Thickness(5),
            BorderBrush = new SolidColorBrush(Colors.Black),
            Background = new SolidColorBrush(back),
        };
        for (int c = 0; c < 5; c++)
        {
            card.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(80) });
        }
        for (int r = 0; r < 6; r++)
        {
            card.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
        }
        AddItem(ref card, 0, 0, suit, Colors.Black, "1", false, $"{name}.topleft");
        AddItem(ref card, 6, 0, suit, Colors.Black, "1", true, $"{name}.bottomleft");
        AddItem(ref card, 6, 4, suit, Colors.Black, "1", true, $"{name}.bottomright");
        AddItem(ref card, 0, 4, suit, Colors.Black, "1", false, $"{name}.topright");
        int count = 0;
        for (int i = 1; i < 5; i++)
        {
            Add(ref card, i, 1, 1, suit, Colors.Black, $"{name}.{card_pips[count]}");
            count++;
        }
        Add(ref card, 2, 2, 1, suit, Colors.Black, $"{name}.{card_pips[count]}");
        count++;
        for (int i = 1; i < 5; i++)
        {
            Add(ref card, i, 1, 2, suit, Colors.Black, $"{name}.{card_pips[count]}");
            count++;
        }
        for (int i = 1; i < 4; i++)
        {
            Add(ref card, i, 2, 2, suit, Colors.Black, $"{name}.{card_pips[count]}");
            count++;
        }
        Add(ref card, 2, 2, 3, suit, Colors.Black, $"{name}.{card_pips[count]}");
        count++;
        for (int i = 1; i < 5; i++)
        {
            Add(ref card, i, 1, 3, suit, Colors.Black, $"{name}.{card_pips[count]}");
            count++;
        }
        AddFace(ref card, 1, 4, 1, 3, Colors.Black, "A", $"{name}.{card_pips[count]}");
        Viewbox box = new Viewbox()
        {
            Child = card
        };
        return box;
    }

    private void SetPip(ref Grid grid, string symbol,
        Color color, string name, int opacity)
    {
        Path pip = (Path)grid.FindName(name);
        pip.Data = GetGeometry(symbol);
        pip.Fill = new SolidColorBrush(color);
        pip.Opacity = opacity;
    }

    private void SetItem(ref Grid grid, string symbol,
        Color color, string value, string name)
    {
        Path pip = (Path)grid.FindName($"{name}.pip");
        pip.Data = GetGeometry(symbol);
        pip.Fill = new SolidColorBrush(color);
        pip.Opacity = 1;
        TextBlock num = (TextBlock)grid.FindName($"{name}.num");
        num.Foreground = new SolidColorBrush(color);
        num.Text = value.ToString();
        num.Opacity = 1;
    }

    private void SetFace(ref Grid grid, Color color,
        string value, string name, int opacity)
    {
        TextBlock face = (TextBlock)grid.FindName(name);
        face.Text = value;
        face.Foreground = new SolidColorBrush(color);
        face.Opacity = opacity;
    }

    private int SetCard(ref Grid grid, string name, int number)
    {
        int index = (number % 13);
        string suit = club;
        switch (number)
        {
            case int c when (number > 1 && number <= 13):
                suit = club;
                break;
            case int d when (number > 14 && number <= 26):
                suit = diamond;
                break;
            case int h when (number >= 27 && number <= 39):
                suit = heart;
                break;
            case int s when (number >= 40 && number <= 52):
                suit = spade;
                break;
        }
        Color color = (suit == heart || suit == diamond) ? Colors.Red : Colors.Black;
        string value = card_values[index];
        Grid card = (Grid)grid.FindName(name);
        card.Background = new SolidColorBrush(Colors.White);
        SetItem(ref card, suit, color, value, $"{name}.topleft");
        SetItem(ref card, suit, color, value, $"{name}.bottomleft");
        SetItem(ref card, suit, color, value, $"{name}.bottomright");
        SetItem(ref card, suit, color, value, $"{name}.topright");
        int[] values = table[index];
        SetPip(ref card, suit, color, $"{name}.a", values[0]);
        SetPip(ref card, suit, color, $"{name}.b", values[1]);
        SetPip(ref card, suit, color, $"{name}.c", values[2]);
        SetPip(ref card, suit, color, $"{name}.d", values[3]);
        SetPip(ref card, suit, color, $"{name}.e", values[4]);
        SetPip(ref card, suit, color, $"{name}.f", values[5]);
        SetPip(ref card, suit, color, $"{name}.g", values[6]);
        SetPip(ref card, suit, color, $"{name}.h", values[7]);
        SetPip(ref card, suit, color, $"{name}.i", values[8]);
        SetPip(ref card, suit, color, $"{name}.j", values[9]);
        SetPip(ref card, suit, color, $"{name}.k", values[10]);
        SetPip(ref card, suit, color, $"{name}.l", values[11]);
        SetPip(ref card, suit, color, $"{name}.m", values[12]);
        SetPip(ref card, suit, color, $"{name}.n", values[13]);
        SetPip(ref card, suit, color, $"{name}.o", values[14]);
        SetPip(ref card, suit, color, $"{name}.p", values[15]);
        SetPip(ref card, suit, color, $"{name}.q", values[16]);
        SetFace(ref card, color, value, $"{name}.r", values[17]);
        return index;
    }

    private void Layout(ref Grid grid)
    {
        Grid container = new Grid()
        {
            Width = 150
        };
        container.Children.Add(Card("card", Colors.Red));
        grid.Children.Add(container);
    }

    public void New(ref Grid grid)
    {
        _turn = 0;
        _lost = false;
        _deck = Shuffle(52);
        Layout(ref grid);
        _value = SetCard(ref grid, "card", _deck[_turn]);
    }

    public void Higher(ref Grid grid)
    {
        int higher = 0;
        if (_turn < _deck.Count && !_lost)
        {
            _turn++;
            higher = SetCard(ref grid, "card", _deck[_turn]);
            _lost = (higher < _value);
        }
        if (_lost)
        {
            ShowAsync($"Game Over : Turn {_turn} - Card Was Lower!", app_title);
        }
    }

    public void Lower(ref Grid grid)
    {
        int lower = 0;
        if (_turn < _deck.Count && !_lost)
        {
            _turn++;
            lower = SetCard(ref grid, "card", _deck[_turn]);
            _lost = (lower > _value);
        }
        if (_lost)
        {
            ShowAsync($"Game Over : Turn {_turn} - Card Was Higher!", app_title);
        }
    }
}