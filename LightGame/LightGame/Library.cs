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
    private const string app_title = "Light Game";
    private const int size = 7;
    private const int on = 1;
    private const int off = 0;
    private readonly Color lightOn = Colors.White;
    private readonly Color lightOff = Colors.Black;

    private int _moves = 0;
    private bool _won = false;
    private int[,] _board = new int[size, size];

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    private bool Winner()
    {
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                if (_board[column, row] == on)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void Toggle(Grid grid, int row, int column)
    {
        _board[row, column] = (_board[row, column] == on ? off : on);
        Grid element = (Grid)grid.Children.Single(
                    w => Grid.GetRow((Grid)w) == row
                    && Grid.GetColumn((Grid)w) == column);
        element.Background = _board[row, column] == on ?
            new SolidColorBrush(lightOn) : new SolidColorBrush(lightOff);
    }

    private void Add(Grid grid, int row, int column)
    {
        Grid element = new Grid
        {
            Height = 40,
            Width = 40,
            Margin = new Thickness(5),
            Background = new SolidColorBrush(lightOn)
        };
        element.Tapped += (object sender, TappedRoutedEventArgs e) =>
        {
            if (!_won)
            {
                element = ((Grid)(sender));
                row = (int)element.GetValue(Grid.RowProperty);
                column = (int)element.GetValue(Grid.ColumnProperty);
                Toggle(grid, row, column);
                if (row > 0)
                {
                    Toggle(grid, row - 1, column); // Toggle Left
                }
                if (row < (size - 1))
                {
                    Toggle(grid, row + 1, column); // Toggle Right
                }
                if (column > 0)
                {
                    Toggle(grid, row, column - 1); // Toggle Above
                }
                if (column < (size - 1))
                {
                    Toggle(grid, row, column + 1); // Toggle Below
                }
                _moves++;
                if (Winner())
                {
                    Show($"Well Done! You won in {_moves} moves!", app_title);
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
        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
        // Setup Grid
        grid.Background = new SolidColorBrush(Colors.DarkGray);
        for (int index = 0; (index < size); index++)
        {
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for (int row = 0; (row < size); row++)
        {
            for (int column = 0; (column < size); column++)
            {
                Add(grid, row, column);
            }
        }
    }

    public void New(ref Grid grid)
    {
        Layout(ref grid);
        _won = false;
        // Setup Board
        for (int column = 0; (column < size); column++)
        {
            for (int row = 0; (row < size); row++)
            {
                _board[column, row] = on;
            }
        }
    }
}