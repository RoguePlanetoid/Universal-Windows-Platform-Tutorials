using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace Reversi
{
    public enum ReversiState
    {
        Empty, Black, White
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

    public class ReversiSquare : BindableBase
    {
        private ReversiState _state;
        private int _row;
        private int _column;

        public ReversiSquare(int row, int column)
        {
            _row = row;
            _column = column;
        }

        public ReversiState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public int Row
        {
            get { return _row; }
            set { SetProperty(ref _row, value); }
        }

        public int Column
        {
            get { return _column; }
            set { SetProperty(ref _column, value); }
        }
    }

    public class ReversiBoard : BindableBase
    {
        private const int total = 8;

        delegate void Navigation(ref int row, ref int column);

        private static List<Navigation> _functions = new List<Navigation>()
        {
            new Navigation(delegate (ref int row, ref int column) { row++; }),
            new Navigation(delegate (ref int row, ref int column) { row++; }),
            new Navigation(delegate (ref int row, ref int column) { row--; }),
            new Navigation(delegate (ref int row, ref int column) { row++; column--; }),
            new Navigation(delegate (ref int row, ref int column) { row++; column++; }),
            new Navigation(delegate (ref int row, ref int column) { row--; column--; }),
            new Navigation(delegate (ref int row, ref int column) { row--; column++; }),
            new Navigation(delegate (ref int row, ref int column) { column++; }),
            new Navigation(delegate (ref int row, ref int column) { column--; }),
        };
        private ReversiState _nextMove = ReversiState.White;
        private int _whiteScore = 0;
        private int _blackScore = 0;
        private bool _gameOver = false;

        private ReversiSquare GetSquare(int row, int column)
        {
            return Squares.Single(s => s.Column == column && s.Row == row);
        }

        private IEnumerable<ReversiSquare> NavigateBoard(int row, int column,
            Navigation navigation)
        {
            navigation(ref column, ref row);
            while (row >= 0 && row < total && column >= 0 && column < total)
            {
                yield return GetSquare(row, column);
                navigation(ref column, ref row);
            }
        }

        private ReversiState ToggleState(ReversiState state)
        {
            return state == ReversiState.Black ?
            ReversiState.White : ReversiState.Black;
        }

        private bool IsMoveSurroundsCounters(int row, int column,
            Navigation navigation, ReversiState state)
        {
            int index = 1;
            var squares = NavigateBoard(row, column, navigation);
            foreach (var square in squares)
            {
                ReversiState currentState = square.State;
                if (index == 1)
                {
                    if (currentState != ToggleState(state)) return false;
                }
                else
                {
                    if (currentState == state) return true;
                    if (currentState == ReversiState.Empty) return false;
                }
                index++;
            }
            return false;
        }

        private void FlipOpponentCounters(int row, int column,
            ReversiState state)
        {
            foreach (Navigation navigation in _functions)
            {
                if (!IsMoveSurroundsCounters(row, column, navigation, state)) continue;
                ReversiState opponentState = ToggleState(state);
                IEnumerable<ReversiSquare> squares = NavigateBoard(row, column, navigation);
                foreach (var square in squares)
                {
                    if (square.State == state) break;
                    square.State = state;
                }
            }
        }

        private bool IsValidMove(int row, int col, ReversiState state)
        {
            if (GetSquare(row, col).State != ReversiState.Empty) return false;
            return _functions.Any(nav => IsMoveSurroundsCounters(row, col, nav, state));
        }

        private bool IsValidMove(int row, int col)
        {
            return IsValidMove(row, col, NextMove);
        }

        private bool HasPlayerMove(ReversiState state)
        {
            for (int row = 0; row < total; row++)
            {
                for (int column = 0; column < total; column++)
                {
                    if (IsValidMove(row, column, state)) return true;
                }
            }
            return false;
        }

        private bool IsGameOver()
        {
            return !HasPlayerMove(ReversiState.Black) &&
            !HasPlayerMove(ReversiState.White);
        }

        private void Setup()
        {
            foreach (ReversiSquare square in Squares)
            {
                square.State = ReversiState.Empty;
            }
            GetSquare(3, 4).State = ReversiState.Black;
            GetSquare(4, 3).State = ReversiState.Black;
            GetSquare(4, 4).State = ReversiState.White;
            GetSquare(3, 3).State = ReversiState.White;
            NextMove = ReversiState.Black;
            WhiteScore = 0;
            BlackScore = 0;
        }

        public ReversiBoard()
        {
            Squares = new List<ReversiSquare>();
            for (int row = 0; (row < total); row++)
            {
                for (int column = 0; (column < total); column++)
                {
                    Squares.Add(new ReversiSquare(row, column));
                }
            }
            Setup();
        }

        public int WhiteScore
        {
            get { return _whiteScore; }
            set { SetProperty(ref _whiteScore, value); }
        }

        public int BlackScore
        {
            get { return _blackScore; }
            set { SetProperty(ref _blackScore, value); }
        }

        public bool GameOver
        {
            get { return _gameOver; }
            set { SetProperty(ref _gameOver, value); }
        }

        public ReversiState NextMove
        {
            get { return _nextMove; }
            set { SetProperty(ref _nextMove, value); }
        }

        public List<ReversiSquare> Squares { get; }

        public void MakeMove(int row, int col)
        {
            if (!IsValidMove(row, col, NextMove)) return;
            GetSquare(row, col).State = NextMove;
            FlipOpponentCounters(row, col, NextMove);
            NextMove = ToggleState(NextMove);
            if (!HasPlayerMove(NextMove)) NextMove = ToggleState(NextMove);
            GameOver = IsGameOver();
            BlackScore = Squares.Count(s => s.State == ReversiState.Black);
            WhiteScore = Squares.Count(s => s.State == ReversiState.White);
        }

        public void Init()
        {
            Setup();
        }
    }

    public class ReversiStateToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ReversiState state)
            {
                switch (state)
                {
                    case ReversiState.Black: return "\u26AB";
                    case ReversiState.White: return "\u26AA";
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ReversiSquareStyle : StyleSelector
    {
        public Style Tile { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            if (item is ReversiSquare square) return Tile;
            return base.SelectStyleCore(item, container);
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
        private const string app_title = "Reversi";

        private IAsyncOperation<IUICommand> _dialogCommand;

        private async Task<bool> ShowDialogAsync(string content, string title = app_title)
        {
            try
            {
                if (_dialogCommand != null)
                {
                    _dialogCommand.Cancel();
                    _dialogCommand = null;
                }
                _dialogCommand = new MessageDialog(content, title).ShowAsync();
                await _dialogCommand;
                return true;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

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

        public ReversiBoard Board { get; set; } = new ReversiBoard();

        public void Init(ref ItemsControl display, ref TextBlock scores)
        {
            display.ItemsPanel = Layout();
            display.ItemsSource = Board.Squares;
            scores.DataContext = Board;
        }

        public async void Tapped(ItemsControl display, ContentPresenter container)
        {
            if (!Board.GameOver)
            {
                ReversiSquare square = (ReversiSquare)display.ItemFromContainer(container);
                Board.MakeMove(square.Row, square.Column);
            }
            else
            {
                await ShowDialogAsync($"Game Over! White: {Board.WhiteScore}, Black: {Board.BlackScore}");
            }
        }

        public void New(ref ItemsControl display, ref TextBlock scores)
        {
            Board = new ReversiBoard();
            scores.DataContext = Board;
            display.ItemsSource = Board.Squares;
        }
    }
}