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
    private string[] tags = { "a", "b", "c", "d", "e", "f", "g", "h", "i" };
    private string[] tiles =
    {
        "0,0",
        "0,1", "1,1",
        "0,2", "1,2", "2,2",
        "0,3", "1,3", "2,3", "3,3",
        "0,4", "1,4", "2,4", "3,4", "4,4",
        "0,5", "1,5", "2,5", "3,5", "4,5", "5,5",
        "0,6", "1,6", "2,6", "3,6", "4,6", "5,6", "6,6"
    };
    private int[][] table =
    {
                 // a, b, c, d, e, f, g, h, i
        new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, // 0
        new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0 }, // 1
        new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 1 }, // 2
        new int[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 }, // 3
        new int[] { 1, 0, 1, 0, 0, 0, 1, 0, 1 }, // 4
        new int[] { 1, 0, 1, 0, 1, 0, 1, 0, 1 }, // 5
        new int[] { 1, 0, 1, 1, 0, 1, 1, 0, 1 }, // 6
    };

    private Random _random = new Random((int)DateTime.Now.Ticks);
    private List<int> _one = new List<int>();
    private List<int> _two = new List<int>();
    private int _turns = 0;

    private List<int> Shuffle(int total)
    {
        return Enumerable.Range(0, total).OrderBy(r => _random.Next(0, total)).ToList();
    }

    private void Add(ref Grid grid, int row, int column, string name)
    {
        Ellipse ellipse = new Ellipse()
        {
            Name = name,
            Fill = new SolidColorBrush(Colors.White),
            Margin = new Thickness(5),
            Opacity = 0
        };
        ellipse.SetValue(Grid.ColumnProperty, column);
        ellipse.SetValue(Grid.RowProperty, row);
        grid.Children.Add(ellipse);
    }

    private Grid Portion(string name)
    {
        int size = 3;
        Grid grid = new Grid()
        {
            Name = name,
            Width = 100,
            Height = 100,
            Padding = new Thickness(5),
            Background = new LinearGradientBrush(new GradientStopCollection()
            {
                new GradientStop() { Color = Colors.Black, Offset = 0.0 },
                new GradientStop() { Color = Colors.Gray, Offset = 1.0 }
            }, 90)
        };
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        int count = 0;
        for (int row = 0; (row < size); row++)
        {
            for (int column = 0; (column < size); column++)
            {
                Add(ref grid, row, column, $"{name}.{tags[count]}");
                count++;
            }
        }
        return grid;
    }

    private StackPanel Domino(string name)
    {
        Grid upper = Portion($"{name}.upper");
        Grid lower = Portion($"{name}.lower");
        StackPanel panel = new StackPanel()
        {
            Name = name,
            Orientation = Orientation.Vertical,
            Margin = new Thickness(25),
        };
        panel.Children.Add(upper);
        panel.Children.Add(lower);
        return panel;
    }

    private void Layout(ref Grid grid)
    {
        StackPanel one = Domino("one");
        StackPanel two = Domino("two");
        StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
        panel.Children.Add(one);
        panel.Children.Add(two);
        grid.Children.Add(panel);
    }

    private Ellipse GetPip(ref Grid portion, string name)
    {
        return (Ellipse)portion.FindName(name);
    }

    private void SetPortion(ref StackPanel domino, string name, int value)
    {
        Grid portion = (Grid)domino.FindName(name);
        int[] values = table[value];
        GetPip(ref portion, $"{name}.a").Opacity = values[0];
        GetPip(ref portion, $"{name}.b").Opacity = values[1];
        GetPip(ref portion, $"{name}.c").Opacity = values[2];
        GetPip(ref portion, $"{name}.d").Opacity = values[3];
        GetPip(ref portion, $"{name}.e").Opacity = values[4];
        GetPip(ref portion, $"{name}.f").Opacity = values[5];
        GetPip(ref portion, $"{name}.g").Opacity = values[6];
        GetPip(ref portion, $"{name}.h").Opacity = values[7];
        GetPip(ref portion, $"{name}.i").Opacity = values[8];
    }

    private void SetDomino(ref Grid grid, string name, string tile)
    {
        StackPanel domino = (StackPanel)grid.FindName(name);
        SetPortion(ref domino, $"{name}.upper", 0);
        SetPortion(ref domino, $"{name}.lower", 0);
        string[] values = tile.Split(',');
        int upper = int.Parse(values[0]);
        int lower = int.Parse(values[1]);
        SetPortion(ref domino, $"{name}.upper", upper);
        SetPortion(ref domino, $"{name}.lower", lower);
    }

    public void New(ref Grid grid)
    {
        _one = Shuffle(tiles.Count());
        _two = Shuffle(tiles.Count());
        Layout(ref grid);
        _turns = tiles.Count() - 1;
    }

    public void Choose(ref Grid grid)
    {
        if (_turns > 0)
        {
            SetDomino(ref grid, "one", tiles[_one[_turns]]);
            SetDomino(ref grid, "two", tiles[_two[_turns]]);
            _turns--;
        }
        else
        {
            New(ref grid);
        }
    }
}