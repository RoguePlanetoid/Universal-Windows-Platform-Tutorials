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
using Windows.UI.Xaml.Shapes;

public class Piece : Grid
{
    public Piece(int index)
    {
        this.Background = new SolidColorBrush(Colors.Black);
        Rectangle rect = new Rectangle()
        {
            Stroke = new SolidColorBrush(Colors.Gray),
        };
        TextBlock text = new TextBlock()
        {
            FontSize = 30,
            Text = index.ToString(),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Colors.White)
        };
        this.Children.Add(rect);
        this.Children.Add(text);
    }

    public int Row { get; set; }
    public int Column { get; set; }
}

public class Library
{
    private const string app_title = "Slide Game";
    private const int size = 4;

    private int _moves = 0;
    private int[,] _board = new int[size, size];
    private List<int> _values;
    private Random _random = new Random((int)DateTime.Now.Ticks);

    private List<int> Shuffle(int start, int total)
    {
        return Enumerable.Range(start, total).OrderBy(r => _random.Next(start, total)).ToList();
    }

    public void Show(string content, string title)
    {
        IAsyncOperation<IUICommand> command = new MessageDialog(content, title).ShowAsync();
    }

    private bool Valid(int row, int column)
    {
        if (row < 0 || column < 0 || row > 3 || column > 3)
        {
            return false;
        }
        return (_board[row, column] == 0);
    }

    private bool Check()
    {
        int previous = _board[0, 0];
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                if (_board[row, column] < previous)
                {
                    return false;
                }
                previous = _board[row, column];
            }
        }
        return true;
    }

    private void Move(Canvas canvas, Piece piece, int row, int column)
    {
        _moves++;
        _board[row, column] = _board[piece.Row, piece.Column];
        _board[piece.Row, piece.Column] = 0;
        piece.Row = row;
        piece.Column = column;
        Layout(canvas);
        if (Check())
        {
            Show($"Correct In {_moves} Moves", app_title);
        }
    }

    private void Layout(Canvas canvas)
    {
        canvas.Children.Clear();
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                if (_board[row, column] > 0)
                {
                    int index = _board[row, column];
                    Piece piece = new Piece(index)
                    {
                        Width = canvas.Width / size,
                        Height = canvas.Height / size,
                        Row = row,
                        Column = column
                    };
                    Canvas.SetTop(piece, row * (canvas.Width / size));
                    Canvas.SetLeft(piece, column * (canvas.Width / size));
                    piece.PointerReleased += (object sender, PointerRoutedEventArgs e) =>
                    {
                        piece = (Piece)sender;
                        if (Valid(piece.Row - 1, piece.Column))
                        {
                            Move(canvas, piece, piece.Row - 1, piece.Column);
                        }
                        else if (Valid(piece.Row, piece.Column + 1))
                        {
                            Move(canvas, piece, piece.Row, piece.Column + 1);
                        }
                        else if (Valid(piece.Row + 1, piece.Column))
                        {
                            Move(canvas, piece, piece.Row + 1, piece.Column);
                        }
                        else if (Valid(piece.Row, piece.Column - 1))
                        {
                            Move(canvas, piece, piece.Row, piece.Column - 1);
                        }
                    };
                    canvas.Children.Add(piece);
                }
            }
        }
    }

    public void New(ref Canvas canvas)
    {
        int index = 1;
        _values = Shuffle(1, _board.Length);
        _values.Insert(0, 0);
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                _board[row, column] = _values[index++];
                if (index == size * size) index = 0;
            }
        }
        Layout(canvas);
    }
}