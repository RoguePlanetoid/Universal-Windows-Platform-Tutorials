using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public class Library
{
    private const string app_title = "Playing Cards";
    private const string clubs = "♣";
    private const string diamonds = "♦";
    private const string hearts = "♥";
    private const string spades = "♠";
    private const string ace = "A";
    private const string jack = "J";
    private const string queen = "Q";
    private const string king = "K";
    private const int rows = 5;
    private const int columns = 3;

    private readonly string[] card_pips = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o" };
    private readonly string[] card_values = { "K", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q" };
    private readonly List<string> face_value = new List<string> { "A", "J", "Q", "K" };

    private readonly byte[][] table =
    {
                  // a, b, c, d, e, f, g, h, i, j, k, l, m, n, o
        new byte[] { 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1 }, // K
        new byte[] { 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1 }, // A
        new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 }, // 2
        new byte[] { 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0 }, // 3
        new byte[] { 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1 }, // 4
        new byte[] { 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1 }, // 5
        new byte[] { 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1 }, // 6
        new byte[] { 1, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 1 }, // 7
        new byte[] { 1, 0, 1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1 }, // 8
        new byte[] { 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1 }, // 9
        new byte[] { 1, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 1 }, // 10
        new byte[] { 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1 }, // J
        new byte[] { 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 1 }, // Q
        new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 0
    };

    private List<int> _deckOne = new List<int>();
    private List<int> _deckTwo = new List<int>();
    private int _cardOne, _cardTwo;
    private int _first, _second;
    private int _score, _counter;
    private Random _random = new Random((int)DateTime.Now.Ticks);

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    private List<int> Select()
    {
        int number;
        List<int> numbers = new List<int>();
        while ((numbers.Count < 53)) // Select 52 Numbers
        {
            number = _random.Next(1, 54); // Seeded Random Number
            if ((!numbers.Contains(number)) || (numbers.Count < 1))
            {
                numbers.Add(number); // Add if number Chosen or None
            }
        }
        return numbers;
    }

    private void Add(ref Grid grid, int row, int column, string content, byte opacity, string name)
    {
        TextBlock text = new TextBlock()
        {
            Name = name,
            FontSize = 25,
            FontFamily = new FontFamily("Arial"),
            Foreground = new SolidColorBrush(Colors.Black),
            Text = content,
            Opacity = opacity,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        text.SetValue(Grid.ColumnProperty, column);
        text.SetValue(Grid.RowProperty, row);
        grid.Children.Add(text);
    }

    private void Card(ref Grid grid, string name, int value, Color back)
    {
        string suit = clubs;
        Grid card = new Grid()
        {
            Name = name,
            CornerRadius = new CornerRadius(5),
            BorderThickness = new Thickness(2),
            BorderBrush = new SolidColorBrush(Colors.Black),
            Background = new SolidColorBrush(back),
        };
        for (int column = 0; column < columns; column++)
        {
            card.ColumnDefinitions.Add(new ColumnDefinition() { });
        }
        for (int row = 0; row < rows; row++)
        {
            card.RowDefinitions.Add(new RowDefinition() { });
        }
        int count = 0;
        for (int row = 0; (row < rows); row++)
        {
            for (int column = 0; (column < columns); column++)
            {
                Add(ref card, row, column, suit,
                    table[value][count], $"{name}.{card_pips[count]}");
                count++;
            }
        }
        grid.Children.Clear();
        grid.Children.Add(card);
    }

    private void SetPip(ref Grid grid, string suit, Color color, string name, int opacity)
    {
        TextBlock item = (TextBlock)grid.FindName(name);
        item.Text = suit;
        item.Foreground = new SolidColorBrush(color);
        item.Opacity = opacity;
    }

    private int SetCard(ref Grid grid, string name, int number)
    {
        int index = (number % 13);
        string suit = clubs;
        string face = clubs;
        switch (number)
        {
            case int c when (number >= 1 && number <= 13):
                suit = clubs;
                break;
            case int d when (number >= 14 && number <= 26):
                suit = diamonds;
                break;
            case int h when (number >= 27 && number <= 39):
                suit = hearts;
                break;
            case int s when (number >= 40 && number <= 52):
                suit = spades;
                break;
        }
        Color color = (suit == hearts || suit == diamonds) ? Colors.Red : Colors.Black;
        string value = card_values[index];
        face = face_value.Contains(value) ? value : suit;
        Grid card = (Grid)grid.FindName(name);
        card.Background = new SolidColorBrush(Colors.White);
        byte[] values = table[index];
        SetPip(ref card, suit, color, $"{name}.a", values[0]);
        SetPip(ref card, suit, color, $"{name}.b", values[1]);
        SetPip(ref card, suit, color, $"{name}.c", values[2]);
        SetPip(ref card, suit, color, $"{name}.d", values[3]);
        SetPip(ref card, suit, color, $"{name}.e", values[4]);
        SetPip(ref card, suit, color, $"{name}.f", values[5]);
        SetPip(ref card, suit, color, $"{name}.g", values[6]);
        SetPip(ref card, face, color, $"{name}.h", values[7]);
        SetPip(ref card, suit, color, $"{name}.i", values[8]);
        SetPip(ref card, suit, color, $"{name}.j", values[9]);
        SetPip(ref card, suit, color, $"{name}.k", values[10]);
        SetPip(ref card, suit, color, $"{name}.l", values[11]);
        SetPip(ref card, suit, color, $"{name}.m", values[12]);
        SetPip(ref card, suit, color, $"{name}.n", values[13]);
        SetPip(ref card, suit, color, $"{name}.o", values[14]);
        return index;
    }

    public void New(ref Grid deckOne, ref Grid deckTwo)
    {
        _score = 0;
        _counter = 0;
        _cardOne = 1;
        _cardTwo = 1;
        _deckOne = Select();
        _deckTwo = Select();
        Card(ref deckOne, "One", 13, Colors.Red);
        Card(ref deckTwo, "Two", 13, Colors.Blue);
    }

    public void Match(ref Grid deckOne, ref Grid deckTwo)
    {
        if (_deckOne != null && _deckTwo != null)
        {
            if ((_cardOne <= 52) && (_cardTwo <= 52))
            {
                _first = _deckOne[_cardOne];
                SetCard(ref deckOne, "One", _first);
                _cardOne++;
                _second = _deckTwo[_cardTwo];
                SetCard(ref deckTwo, "Two", _second);
                _cardTwo++;
                if ((_first % 13) == (_second % 13)) // Ignore Suite for Match
                {
                    _score++;
                    Show("Match!", app_title);
                }
                _counter++;
            }
            else
            {
                Show($"Game Over! Matched {_score} out of {_counter} cards!", app_title);
            }
        }
    }
}