using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Mahjong
{
    public enum MahjongSelect
    {
        None, Selected, Disabled, Incorrect, Hint
    }

    public enum MahjongState
    {
        DifferentType, NotMove, ValidMove, InvalidMove, NoMoves, Won
    }

    public enum MahjongType
    {
        Joker, RedDragon, GreenDragon, WhiteDragon, EastWind, SouthWind, WestWind, NorthWind,
        Spring, Summer, Autumn, Winter, Plum, Orchid, Chrysanthemum, Bamboo,
        OneOfCircles, TwoOfCircles, ThreeOfCircles, FourOfCircles, FiveOfCircles,
        SixOfCircles, SevenOfCircles, EightOfCircles, NineOfCircles,
        OneOfBamboos, TwoOfBamboos, ThreeOfBamboos, FourOfBamboos, FiveOfBamboos,
        SixOfBamboos, SevenOfBamboos, EightOfBamboos, NineOfBamboos,
        OneOfCharacters, TwoOfCharacters, ThreeOfCharacters, FourOfCharacters, FiveOfCharacters,
        SixOfCharacters, SevenOfCharacters, EightOfCharacters, NineOfCharacters
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

    public class MahjongPosition : BindableBase
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Index { get; set; }

        public MahjongPosition(int column, int row)
        {
            Row = row;
            Column = column;
        }

        public MahjongPosition(int column, int row, int index)
        {
            Row = row;
            Column = column;
            Index = index;
        }
    }

    public class MahjongTile : BindableBase
    {
        private MahjongType? _type;
        private MahjongSelect _select;
        private MahjongPosition _position;

        public MahjongType? Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public MahjongSelect Select
        {
            get { return _select; }
            set { SetProperty(ref _select, value); }
        }

        public MahjongPosition Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public MahjongTile(MahjongType? type, int column, int row, int index)
        {
            Type = type;
            Position = new MahjongPosition(column, row, index);
        }
    }

    public class MahjongPair : BindableBase
    {
        private static readonly Random random = new Random((int)DateTime.Now.Ticks);
        private MahjongTile _tileOne;
        private MahjongTile _tileTwo;

        public MahjongTile TileOne
        {
            get { return _tileOne; }
            set { SetProperty(ref _tileOne, value); }
        }

        public MahjongTile TileTwo
        {
            get { return _tileTwo; }
            set { SetProperty(ref _tileTwo, value); }
        }

        public MahjongPair(MahjongTile tileOne, MahjongTile tileTwo)
        {
            _tileOne = tileOne;
            _tileTwo = tileTwo;
        }

        public static MahjongPair GetPair(List<MahjongTile> tiles)
        {
            if (tiles.Count < 2) throw new InvalidOperationException();
            int index = random.Next() % tiles.Count;
            MahjongTile tileOne = tiles[index];
            tiles.RemoveAt(index);
            index = random.Next() % tiles.Count;
            MahjongTile tileTwo = tiles[index];
            tiles.RemoveAt(index);
            return new MahjongPair(tileOne, tileTwo);
        }
    }

    public class MahjongBoard : BindableBase
    {
        public const int Rows = 8;
        public const int Columns = 10;
        public const int Indexes = 5;

        private readonly byte[] layout =
        {
            0, 1, 1, 1, 1, 1, 1, 1, 1, 0,
            1, 1, 2, 2, 2, 2, 2, 2, 1, 1,
            1, 1, 2, 3, 4, 4, 3, 2, 1, 1,
            1, 1, 2, 4, 5, 5, 4, 2, 1, 1,
            1, 1, 2, 4, 5, 5, 4, 2, 1, 1,
            1, 1, 2, 3, 4, 4, 3, 2, 1, 1,
            1, 1, 2, 2, 2, 2, 2, 2, 1, 1,
            0, 1, 1, 1, 1, 1, 1, 1, 1, 0
        };
        private readonly List<MahjongType> types =
        Enum.GetValues(typeof(MahjongType)).Cast<MahjongType>().ToList();
        private readonly Random random = new Random((int)DateTime.Now.Ticks);

        private bool _started;
        private MahjongState _state;
        private ObservableCollection<MahjongTile> _tiles;

        private MahjongTile GetTile(int column, int row, int index)
        {
            return _tiles.FirstOrDefault(w =>
            w.Position.Row == row &&
            w.Position.Column == column &&
            w.Position.Index == index);
        }

        private MahjongPosition[] GetPositions(int column, int row)
        {
            MahjongPosition[] positions = new MahjongPosition[Columns * Rows];
            int p = 0;
            for (int c = 0; c < Columns; c++)
            {
                for (int r = 0; r < Rows; r++)
                {
                    positions[p++] = new MahjongPosition(column + c, row + r);
                }
            }
            return positions;
        }

        private bool CanMove(MahjongTile tile, int column, int row, int index)
        {
            MahjongPosition[] positions = GetPositions(tile.Position.Column, tile.Position.Row);
            int i = tile.Position.Index + index;
            for (int p = 0; p < positions.Length; p++)
            {
                int c = positions[p].Column + column;
                int r = positions[p].Row + row;
                MahjongTile found = GetTile(c, r, i);
                if (found != null && tile != found) return false;
            }
            return true;
        }

        private bool CanMoveUp(MahjongTile tile) => CanMove(tile, 0, 0, 1);

        private bool CanMoveRight(MahjongTile tile) => CanMove(tile, 1, 0, 0);

        private bool CanMoveLeft(MahjongTile tile) => CanMove(tile, -1, 0, 0);

        public bool CanMove(MahjongTile tile)
        {
            bool up = CanMoveUp(tile);
            bool leftUp = up && CanMoveLeft(tile);
            bool rightUp = up && CanMoveRight(tile);
            return leftUp || rightUp;
        }

        private bool NextMovePossible()
        {
            List<MahjongTile> removable = new List<MahjongTile>();
            foreach (MahjongTile tile in _tiles)
            {
                if (CanMove(tile)) removable.Add(tile);
            }
            for (int i = 0; i < removable.Count; i++)
            {
                for (int j = 0; j < removable.Count; j++)
                {
                    if (j != i && removable[i].Type == removable[j].Type)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private MahjongTile AddTile(MahjongType? type, int column, int row, int index)
        {
            byte current = layout[row * Columns + column];
            return (current > 0 && index < current) ?
            new MahjongTile(type, column, row, index) : null;
        }

        private void Add(MahjongTile tile) => _tiles.Add(tile);

        private void Remove(MahjongTile tile) => _tiles.Remove(tile);

        private List<MahjongTile> ExtractRemovableTiles(MahjongBoard board)
        {
            List<MahjongTile> removable = new List<MahjongTile>();
            MahjongTile[] tiles = new MahjongTile[board.Tiles.Count];
            board.Tiles.CopyTo(tiles, 0);
            foreach (MahjongTile tile in tiles)
            {
                if (board.CanMove(tile)) removable.Add(tile);
            }
            foreach (MahjongTile tile in removable)
            {
                board.Remove(tile);
            }
            return removable;
        }

        private void Scramble(MahjongBoard board)
        {
            List<MahjongPair> reversed = new List<MahjongPair>();
            while (board.Tiles.Count > 0)
            {
                List<MahjongTile> removable = new List<MahjongTile>();
                removable.AddRange(ExtractRemovableTiles(board));
                while (removable.Count > 1)
                {
                    reversed.Add(MahjongPair.GetPair(removable));
                }
                foreach (MahjongTile tile in removable)
                {
                    board.Add(tile);
                }
            }
            for (int i = reversed.Count - 1; i >= 0; i--)
            {
                int typeIndex = random.Next() % types.Count;
                reversed[i].TileOne.Type = board.types[typeIndex];
                reversed[i].TileTwo.Type = board.types[typeIndex];
                board.Add(reversed[i].TileOne);
                board.Add(reversed[i].TileTwo);
            }
        }

        private void Structure(MahjongBoard board)
        {
            for (int index = 0; index < Indexes; index++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    for (int row = 0; row < Rows; row++)
                    {
                        MahjongTile tile = AddTile(null, column, row, index);
                        if (tile != null) board.Tiles.Add(tile);
                    }
                }
            }
        }

        private void Generate(MahjongBoard board)
        {
            board.Tiles = new ObservableCollection<MahjongTile>();
            Structure(board);
            Scramble(board);
        }

        private MahjongPair GetHint()
        {
            List<MahjongTile> tiles = new List<MahjongTile>();
            foreach (MahjongTile tile in _tiles)
            {
                if (CanMove(tile)) tiles.Add(tile);
            }
            for (int i = 0; i < tiles.Count; i++)
            {
                for (int j = 0; j < tiles.Count; j++)
                {
                    if (i == j) continue;
                    if (tiles[i].Type == tiles[j].Type)
                    {
                        return new MahjongPair(tiles[i], tiles[j]);
                    }
                }
            }
            return null;
        }

        public MahjongState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public ObservableCollection<MahjongTile> Tiles
        {
            get { return _tiles; }
            set { SetProperty(ref _tiles, value); }
        }

        public void Start() => Generate(this);

        public void Shuffle() => Scramble(this);

        public MahjongState Play(MahjongTile tileOne, MahjongTile tileTwo)
        {
            if (!_started) _started = true;
            if (tileOne == tileTwo) return MahjongState.InvalidMove;
            if (tileOne.Type != tileTwo.Type) return MahjongState.DifferentType;
            if (!CanMove(tileOne) || !CanMove(tileTwo)) return MahjongState.NotMove;
            Remove(tileOne);
            Remove(tileTwo);
            if (_tiles.Count == 0) return MahjongState.Won;
            MahjongState result = MahjongState.ValidMove;
            if (!NextMovePossible()) result |= MahjongState.NoMoves;
            return result;
        }

        public void SetHint()
        {
            if (Tiles.Count > 0)
            {
                SetNone();
                MahjongPair pair = GetHint();
                pair.TileOne.Select = MahjongSelect.Hint;
                pair.TileTwo.Select = MahjongSelect.Hint;
            }
        }

        public void SetNone()
        {
            if (Tiles.Count > 0)
            {
                foreach (MahjongTile tile in Tiles)
                {
                    tile.Select = MahjongSelect.None;
                }
            }
        }

        public void SetDisabled()
        {
            if (Tiles.Count > 0)
            {
                foreach (MahjongTile tile in Tiles)
                {
                    tile.Select = CanMove(tile) ?
                    MahjongSelect.None : MahjongSelect.Disabled;
                }
            }
        }
    }

    public class MahjongSelectToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MahjongSelect select)
            {
                switch (select)
                {
                    case MahjongSelect.None:
                        return new SolidColorBrush(Colors.Transparent);
                    case MahjongSelect.Selected:
                        return new SolidColorBrush(Colors.ForestGreen);
                    case MahjongSelect.Disabled:
                        return new SolidColorBrush(Colors.DarkSlateGray);
                    case MahjongSelect.Incorrect:
                        return new SolidColorBrush(Colors.IndianRed);
                    case MahjongSelect.Hint:
                        return new SolidColorBrush(Colors.CornflowerBlue);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MahjongTypeToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MahjongType type)
            {
                switch (type)
                {
                    case MahjongType.Joker:
                        return "\U0001F02A";
                    case MahjongType.RedDragon:
                        return "\U0001F004";
                    case MahjongType.GreenDragon:
                        return "\U0001F005";
                    case MahjongType.WhiteDragon:
                        return "\U0001F006";
                    case MahjongType.EastWind:
                        return "\U0001F000";
                    case MahjongType.SouthWind:
                        return "\U0001F001";
                    case MahjongType.WestWind:
                        return "\U0001F002";
                    case MahjongType.NorthWind:
                        return "\U0001F003";
                    case MahjongType.Spring:
                        return "\U0001F026";
                    case MahjongType.Summer:
                        return "\U0001F027";
                    case MahjongType.Autumn:
                        return "\U0001F028";
                    case MahjongType.Winter:
                        return "\U0001F029";
                    case MahjongType.Plum:
                        return "\U0001F022";
                    case MahjongType.Orchid:
                        return "\U0001F023";
                    case MahjongType.Chrysanthemum:
                        return "\U0001F025";
                    case MahjongType.Bamboo:
                        return "\U0001F024";
                    case MahjongType.OneOfCircles:
                        return "\U0001F019";
                    case MahjongType.TwoOfCircles:
                        return "\U0001F01A";
                    case MahjongType.ThreeOfCircles:
                        return "\U0001F01B";
                    case MahjongType.FourOfCircles:
                        return "\U0001F01C";
                    case MahjongType.FiveOfCircles:
                        return "\U0001F01D";
                    case MahjongType.SixOfCircles:
                        return "\U0001F01E";
                    case MahjongType.SevenOfCircles:
                        return "\U0001F01F";
                    case MahjongType.EightOfCircles:
                        return "\U0001F020";
                    case MahjongType.NineOfCircles:
                        return "\U0001F021";
                    case MahjongType.OneOfBamboos:
                        return "\U0001F010";
                    case MahjongType.TwoOfBamboos:
                        return "\U0001F011";
                    case MahjongType.ThreeOfBamboos:
                        return "\U0001F012";
                    case MahjongType.FourOfBamboos:
                        return "\U0001F013";
                    case MahjongType.FiveOfBamboos:
                        return "\U0001F014";
                    case MahjongType.SixOfBamboos:
                        return "\U0001F015";
                    case MahjongType.SevenOfBamboos:
                        return "\U0001F016";
                    case MahjongType.EightOfBamboos:
                        return "\U0001F017";
                    case MahjongType.NineOfBamboos:
                        return "\U0001F018";
                    case MahjongType.OneOfCharacters:
                        return "\U0001F007";
                    case MahjongType.TwoOfCharacters:
                        return "\U0001F008";
                    case MahjongType.ThreeOfCharacters:
                        return "\U0001F009";
                    case MahjongType.FourOfCharacters:
                        return "\U0001F00A";
                    case MahjongType.FiveOfCharacters:
                        return "\U0001F00B";
                    case MahjongType.SixOfCharacters:
                        return "\U0001F00C";
                    case MahjongType.SevenOfCharacters:
                        return "\U0001F00D";
                    case MahjongType.EightOfCharacters:
                        return "\U0001F00E";
                    case MahjongType.NineOfCharacters:
                        return "\U0001F00F";
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int index) return new Thickness(0, 0, 0, index * 3);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MahjongTileStyle : StyleSelector
    {
        public Style Tile { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            if (item is MahjongTile tile) return Tile;
            return base.SelectStyleCore(item, container);
        }
    }

    public class Binder
    {
        public static readonly DependencyProperty GridColumnBindingPathProperty =
        DependencyProperty.RegisterAttached("GridColumnBindingPath", typeof(string), typeof(Binder),
        new PropertyMetadata(null, BindingPathPropertyChanged));

        public static readonly DependencyProperty GridRowBindingPathProperty =
        DependencyProperty.RegisterAttached("GridRowBindingPath", typeof(string), typeof(Binder),
        new PropertyMetadata(null, BindingPathPropertyChanged));

        public static readonly DependencyProperty CanvasZIndexBindingPathProperty =
        DependencyProperty.RegisterAttached("CanvasZIndexBindingPath", typeof(string), typeof(Binder),
        new PropertyMetadata(null, BindingPathPropertyChanged));

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

        public static string GetCanvasZIndexBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(CanvasZIndexBindingPathProperty);
        }

        public static void SetCanvasZIndexBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(CanvasZIndexBindingPathProperty, value);
        }

        private static void BindingPathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string path)
            {
                DependencyProperty property = null;
                if (e.Property == GridColumnBindingPathProperty)
                    property = Grid.ColumnProperty;
                else if (e.Property == GridRowBindingPathProperty)
                    property = Grid.RowProperty;
                else if (e.Property == CanvasZIndexBindingPathProperty)
                    property = Canvas.ZIndexProperty;

                BindingOperations.SetBinding(obj, property,
                new Binding { Path = new PropertyPath(path) });
            }
        }
    }

    public class Library
    {
        private const string app_title = "Mahjong";

        private bool _gameOver = false;
        private MahjongTile _selected = null;
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
            StringBuilder rows = new StringBuilder();
            StringBuilder columns = new StringBuilder();
            for (int r = 0; r < MahjongBoard.Rows; r++)
            {
                rows.Append($"<RowDefinition Height=\"Auto\"/>");
            }
            for (int c = 0; c < MahjongBoard.Columns; c++)
            {
                columns.Append($"<ColumnDefinition Width=\"Auto\"/>");
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

        public MahjongBoard Board { get; set; } = new MahjongBoard();

        public void Init(ref ItemsControl display)
        {
            _gameOver = false;
            Board.Start();
            display.ItemsPanel = Layout();
            display.ItemsSource = Board.Tiles;
        }

        public async void Tapped(ItemsControl display, ContentPresenter container)
        {
            if (!_gameOver)
            {
                MahjongTile tile = (MahjongTile)display.ItemFromContainer(container);
                if (_selected == null || _selected == tile)
                {
                    Board.SetNone();
                    if (_selected == tile)
                    {
                        tile.Select = MahjongSelect.None;
                        _selected = null;
                    }
                    else
                    {
                        tile.Select = MahjongSelect.Selected;
                        _selected = tile;
                    }
                }
                else
                {
                    Board.State = Board.Play(_selected, tile);
                    switch (Board.State)
                    {
                        case MahjongState.DifferentType:
                            tile.Select = MahjongSelect.Incorrect;
                            break;
                        case MahjongState.NotMove:
                            tile.Select = MahjongSelect.Incorrect;
                            break;
                        case MahjongState.ValidMove:
                            tile.Select = MahjongSelect.None;
                            break;
                        case MahjongState.InvalidMove:
                            tile.Select = MahjongSelect.Incorrect;
                            break;
                        case MahjongState.NoMoves:
                            Board.Shuffle();
                            break;
                        case MahjongState.Won:
                            await ShowDialogAsync("You Won, Game Over!");
                            _gameOver = true;
                            break;
                    }
                    _selected = null;
                }
            }
            else
            {
                await ShowDialogAsync("You Won, Game Over!");
            }
        }

        public void New(ref ItemsControl display)
        {
            _gameOver = false;
            Board.Start();
            display.ItemsSource = Board.Tiles;
        }

        public void Hint() => Board.SetHint();

        public void Show() => Board.SetDisabled();

        public void Shuffle() => Board.Shuffle();
    }
}