using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public class Library
{
    private const string app_title = "Four In Row";
    private const int size = 7;

    private bool _won = false;
    private int[,] _board = new int[size, size];
    private int _player = 0;

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    public async Task<bool> ConfirmAsync(string content, string title, string ok, string cancel)
    {
        bool result = false;
        MessageDialog dialog = new MessageDialog(content, title);
        dialog.Commands.Add(new UICommand(ok, new UICommandInvokedHandler((cmd) => result = true)));
        dialog.Commands.Add(new UICommand(cancel, new UICommandInvokedHandler((cmd) => result = false)));
        await dialog.ShowAsync();
        return result;
    }

    public bool Winner(int column, int row)
    {
        int total = 3; // Total Excluding Current
        int value = 0; // Value in Line
        int amend = 0; // Add or Remove
        // Check Vertical
        do
        {
            value++;
        }
        while (row + value < size &&
        _board[column, row + value] == _player);
        if (value > total)
        {
            return true;
        }
        value = 0;
        amend = 0;
        // Check Horizontal - From Left
        do
        {
            value++;
        }
        while (column - value >= 0 &&
        _board[column - value, row] == _player);
        if (value > total)
        {
            return true;
        }
        value -= 1; // Deduct Middle - Prevent double count
        // Then Right
        do
        {
            value++;
            amend++;
        }
        while (column + amend < size &&
        _board[column + amend, row] == _player);
        if (value > total)
        {
            return true;
        }
        value = 0;
        amend = 0;
        // Diagonal - Left Top
        do
        {
            value++;
        }
        while (column - value >= 0 && row - value >= 0 &&
        _board[column - value, row - value] == _player);
        if (value > total)
        {
            return true;
        }
        value -= 1; // Deduct Middle - Prevent double count
        // To Right Bottom
        do
        {
            value++;
            amend++;
        }
        while (column + amend < size && row + amend < size &&
        _board[column + amend, row + amend] == _player);
        if (value > total)
        {
            return true;
        }
        value = 0;
        amend = 0;
        // Diagonal - From Right Top
        do
        {
            value++;
        }
        while (column + value < size && row - value >= 0 &&
        _board[column + value, row - value] == _player);
        if (value > total)
        {
            return true;
        }
        value -= 1; // Deduct Middle - Prevent double count
        // To Left Bottom
        do
        {
            value++;
            amend++;
        }
        while (column - amend >= 0 &&
        row + amend < size &&
        _board[column - amend, row + amend] == _player);
        if (value > total)
        {
            return true;
        }
        return false;
    }

    private bool Full()
    {
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                if (_board[column, row] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private Path GetPiece(int player)
    {
        Path path = new Path
        {
            Stretch = Stretch.Uniform,
            StrokeThickness = 5,
            Margin = new Thickness(5),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        if ((player == 1))
        {
            LineGeometry line1 = new LineGeometry
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(30, 30)
            };
            LineGeometry line2 = new LineGeometry
            {
                StartPoint = new Point(30, 0),
                EndPoint = new Point(0, 30)
            };
            GeometryGroup linegroup = new GeometryGroup();
            linegroup.Children.Add(line1);
            linegroup.Children.Add(line2);
            path.Data = linegroup;
            path.Stroke = new SolidColorBrush(Colors.Red);
        }
        else
        {
            EllipseGeometry ellipse = new EllipseGeometry
            {
                Center = new Point(15, 15),
                RadiusX = 15,
                RadiusY = 15
            };
            path.Data = ellipse;
            path.Stroke = new SolidColorBrush(Colors.Blue);
        }
        return path;
    }

    private void Place(Grid grid, int column, int row)
    {
        for (int i = size - 1; i > -1; i--)
        {
            if (_board[column, i] == 0)
            {
                _board[column, i] = _player;
                Grid element = (Grid)grid.Children.Single(
                    w => Grid.GetRow((Grid)w) == i
                    && Grid.GetColumn((Grid)w) == column);
                element.Children.Add(GetPiece(_player));
                row = i;
                break;
            }
        }
        if (Winner(column, row))
        {
            _won = true;
            Show($"Player {_player} has won!", app_title);
        }
        else if (Full())
        {
            Show("Board Full!", app_title);
        }
        _player = _player == 1 ? 2 : 1; // Set Player
    }

    private void Add(Grid grid, int row, int column)
    {
        Grid element = new Grid
        {
            Height = 40,
            Width = 40,
            Margin = new Thickness(5),
            Background = new SolidColorBrush(Colors.WhiteSmoke),
        };
        element.Tapped += (object sender, TappedRoutedEventArgs e) =>
        {
            if (!_won)
            {
                element = ((Grid)(sender));
                row = (int)element.GetValue(Grid.RowProperty);
                column = (int)element.GetValue(Grid.ColumnProperty);
                if (_board[column, 0] == 0) // Check Free Row
                {
                    Place(grid, column, row);
                }
            }
            else
            {
                Show("Game Over!", app_title);
            }
        };
        element.SetValue(Grid.ColumnProperty, column);
        element.SetValue(Grid.RowProperty, row);
        grid.Children.Add(element);
    }

    private void Layout(ref Grid Grid)
    {
        _player = 1;
        Grid.Children.Clear();
        Grid.ColumnDefinitions.Clear();
        Grid.RowDefinitions.Clear();
        // Setup Grid
        for (int index = 0; (index < size); index++)
        {
            Grid.RowDefinitions.Add(new RowDefinition());
            Grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        // Setup Board
        for (int column = 0; (column < size); column++)
        {
            for (int row = 0; (row < size); row++)
            {
                Add(Grid, row, column);
                _board[row, column] = 0;
            }
        }
    }

    public async void NewAsync(Grid grid)
    {
        Layout(ref grid);
        _won = false;
        _player = await ConfirmAsync("Who goes First?", app_title, "X", "O") ? 1 : 2;
    }
}