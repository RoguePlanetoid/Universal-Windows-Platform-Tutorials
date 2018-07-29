using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace LuckyChess
{
    public enum ChessBackground
    {
        Light, Dark
    }

    public enum PieceSet
    {
        White, Black
    }

    public enum PieceType
    {
        Pawn, Knight, Bishop, Rook, Queen, King
    }

    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T field, T value,
        [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ChessCoordinate
    {
        private static string[] chess_ranks = { "8", "7", "6", "5", "4", "3", "2", "1" };
        private static string[] chess_files = { "A", "B", "C", "D", "E", "F", "G", "H" };

        public int Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public ChessBackground Background { get; set; }
        public string Notation { get; set; }

        public ChessCoordinate(int id)
        {
            Id = id;
            Row = Id / 8;
            Column = Id % 8;
            Background = (Row + Column) % 2 == 0 ? ChessBackground.Light : ChessBackground.Dark;
            Notation = chess_files[Column] + chess_ranks[Row];
        }
    }

    public class ChessPiece : BindableBase
    {
        private PieceType _type;
        private PieceSet _set;

        public ChessPiece(PieceType type, PieceSet set)
        {
            _type = type;
            _set = set;
        }

        public PieceType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public PieceSet Set
        {
            get { return _set; }
            set { SetProperty(ref _set, value); }
        }
    }

    public class ChessSquare : BindableBase
    {
        private int _id;
        private ChessPiece _piece;
        private ChessCoordinate _coordinate;
        private bool _isSelected;

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public ChessPiece Piece
        {
            get { return _piece; }
            set { SetProperty(ref _piece, value); }
        }

        public ChessCoordinate Coordinate
        {
            get { return _coordinate; }
            set { SetProperty(ref _coordinate, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
    }

    public class ChessPosition : List<ChessPiece>
    {
        public ChessPosition() : base(new ChessPiece[64]) { }

        public ChessPosition(string position) : this()
        {
            int i = 0;
            foreach (char item in position)
            {
                switch (item)
                {
                    case 'p': this[i++] = new ChessPiece(PieceType.Pawn, PieceSet.Black); break;
                    case 'n': this[i++] = new ChessPiece(PieceType.Knight, PieceSet.Black); break;
                    case 'b': this[i++] = new ChessPiece(PieceType.Bishop, PieceSet.Black); break;
                    case 'r': this[i++] = new ChessPiece(PieceType.Rook, PieceSet.Black); break;
                    case 'q': this[i++] = new ChessPiece(PieceType.Queen, PieceSet.Black); break;
                    case 'k': this[i++] = new ChessPiece(PieceType.King, PieceSet.Black); break;
                    case 'P': this[i++] = new ChessPiece(PieceType.Pawn, PieceSet.White); break;
                    case 'N': this[i++] = new ChessPiece(PieceType.Knight, PieceSet.White); break;
                    case 'B': this[i++] = new ChessPiece(PieceType.Bishop, PieceSet.White); break;
                    case 'R': this[i++] = new ChessPiece(PieceType.Rook, PieceSet.White); break;
                    case 'Q': this[i++] = new ChessPiece(PieceType.Queen, PieceSet.White); break;
                    case 'K': this[i++] = new ChessPiece(PieceType.King, PieceSet.White); break;
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8': i += int.Parse(item.ToString()); break;
                    case '/': if (i % 8 != 0) throw new ArgumentException("Invalid FEN"); break;
                    default: throw new ArgumentException($"Invalid FEN Character: '{item}'");
                }
            }
        }
    }

    public class ChessBoard
    {
        public ChessSquare[] ChessSquares { get; set; } = new ChessSquare[64];

        public ChessBoard(string fen)
        {
            ChessPosition position = new ChessPosition(fen);
            for (int i = 0; i < position.Count; i++)
            {
                ChessSquares[i] = new ChessSquare
                {
                    Id = i,
                    Piece = position[i],
                    Coordinate = new ChessCoordinate(i)
                };
            }
        }
    }

    public class ChessSquareStyleSelector : StyleSelector
    {
        public Style Light { get; set; }
        public Style Dark { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            if (item is ChessSquare square)
            {
                int row = square.Id / 8;
                int col = square.Id % 8;
                return (row + col) % 2 == 0 ? Light : Dark;
            }
            return base.SelectStyleCore(item, container);
        }
    }

    public class ChessPieceToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ChessPiece piece)
            {
                if (piece.Set == PieceSet.White)
                {
                    switch (piece.Type)
                    {
                        case PieceType.King: return "\u2654";
                        case PieceType.Queen: return "\u2655";
                        case PieceType.Rook: return "\u2656";
                        case PieceType.Bishop: return "\u2657";
                        case PieceType.Knight: return "\u2658";
                        case PieceType.Pawn: return "\u2659";
                    }
                }
                else
                {
                    switch (piece.Type)
                    {
                        case PieceType.King: return "\u265A";
                        case PieceType.Queen: return "\u265B";
                        case PieceType.Rook: return "\u265C";
                        case PieceType.Bishop: return "\u265D";
                        case PieceType.Knight: return "\u265E";
                        case PieceType.Pawn: return "\u265F";
                    }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isVisible = (bool)value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class Binder
    {
        public static readonly DependencyProperty GridColumnBindingPathProperty =
        DependencyProperty.RegisterAttached("GridColumnBindingPath", typeof(string), typeof(Binder),
        new PropertyMetadata(null, GridBindingPathPropertyChanged));

        public static readonly DependencyProperty GridRowBindingPathProperty =
        DependencyProperty.RegisterAttached("GridRowBindingPath", typeof(string), typeof(Binder),
        new PropertyMetadata(null, GridBindingPathPropertyChanged));

        public static string GetGridColumnBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(GridColumnBindingPathProperty);
        }

        public static void SetGridColumnBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(GridColumnBindingPathProperty, value);
        }

        public static string GetGridRowBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(GridRowBindingPathProperty);
        }

        public static void SetGridRowBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(GridRowBindingPathProperty, value);
        }

        private static void GridBindingPathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string path)
            {
                DependencyProperty property = null;
                if (e.Property == GridColumnBindingPathProperty)
                    property = Grid.ColumnProperty;
                else if (e.Property == GridRowBindingPathProperty)
                    property = Grid.RowProperty;

                BindingOperations.SetBinding(obj, property,
                new Binding { Path = new PropertyPath(path) });
            }
        }
    }

    public class Library
    {
        private const string fen_start = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

        ChessSquare _square = null;

        public ChessBoard Board { get; set; } = new ChessBoard(fen_start);

        private ItemsPanelTemplate Layout()
        {
            StringBuilder columns = new StringBuilder();
            StringBuilder rows = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                columns.Append("<ColumnDefinition Width=\"*\"/>");
                rows.Append("<RowDefinition Height=\"*\"/>");
            }
            return (ItemsPanelTemplate)
            XamlReader.Load($@"<ItemsPanelTemplate 
            xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                <Grid>
                    <Grid.ColumnDefinitions>{columns}</Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>{rows}</Grid.RowDefinitions>
                </Grid>
            </ItemsPanelTemplate>");
        }

        public void Init(ref ItemsControl display)
        {
            display.ItemsPanel = Layout();
            display.ItemsSource = Board.ChessSquares;
        }

        public void Tapped(ItemsControl display, ContentPresenter container)
        {
            ChessSquare square = (ChessSquare)display.ItemFromContainer(container);
            if (_square == null &&
                square.Piece != null)
            {
                square.IsSelected = true;
                _square = square;
            }
            else if (square == _square)
            {
                square.IsSelected = false;
                _square = null;
            }
            else if (_square != null &&
            _square.Piece != null &&
            _square.Piece.Set != square?.Piece?.Set)
            {
                square.Piece = _square.Piece;
                _square.IsSelected = false;
                _square.Piece = null;
                _square = null;
            }
        }

        public void New(ref ItemsControl display)
        {
            Board = new ChessBoard(fen_start);
            display.ItemsSource = Board.ChessSquares;
        }
    }
}