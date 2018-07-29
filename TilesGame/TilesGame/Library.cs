using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace TilesGame
{
    public enum TilesType
    {
        White, Black, Start, Finish, Correct, Incorrect
    }

    public enum TilesState
    {
        Ready, Started, Lost, Complete
    }

    public class TilesItem
    {
        public int Row { get; set; }
        public TilesType Type { get; set; }

        public TilesItem(int row, TilesType type)
        {
            Row = row;
            Type = type;
        }
    }

    public class TilesBoard
    {
        private const int total = 32;
        private const int offset = 3;
        private const int columns = 4;
        private const int view_height = 400;
        private const int view_width = 200;
        private const int tile_height = view_height / 4;
        private const int tile_width = view_width / 4;
        private readonly Random random = new Random((int)DateTime.Now.Ticks);
        private readonly Color[] colours =
        {
            Colors.White, Colors.Black, Colors.Yellow,
            Colors.ForestGreen, Colors.Gray, Colors.IndianRed
        };

        private List<int> _selected = new List<int>();
        private DateTime _start;
        private double _height;
        private int _current;
        private Grid _layout;

        public delegate void StateChangedEvent(TilesState state);
        public event StateChangedEvent StateChanged;

        public TimeSpan Current { get { return DateTime.Now - _start; } }

        public TimeSpan Best { get; private set; }

        public TimeSpan Time { get; private set; }

        public TilesState State { get; private set; }

        private List<int> Choose()
        {
            List<int> list = new List<int>();
            while ((list.Count < total))
            {
                list.Add(random.Next(0, columns));
            }
            return list;
        }

        private SolidColorBrush GetBrush(TilesType type)
        {
            return new SolidColorBrush(colours[(int)type]);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (State != TilesState.Lost)
            {
                Grid grid = (Grid)sender;
                TilesItem item = (TilesItem)grid.Tag;
                if (item.Type == TilesType.Black)
                {
                    _current--;
                    if (_current == item.Row)
                    {
                        if (State != TilesState.Started)
                        {
                            _start = DateTime.Now;
                            State = TilesState.Started;
                        }
                        grid.Background = GetBrush(TilesType.Correct);
                        _height = _height + tile_height;
                        Canvas.SetTop(_layout, _height);
                        if (_current == offset + 1)
                        {
                            _height = _height + tile_height;
                            Canvas.SetTop(_layout, _height);
                            Time = DateTime.Now - _start;
                            if (Best == TimeSpan.Zero || Time < Best)
                            {
                                Best = Time;
                            }
                            State = TilesState.Complete;
                        }
                        else
                        {
                            State = TilesState.Started;
                        }
                    }
                    else
                    {
                        grid.Background = GetBrush(TilesType.Incorrect);
                        State = TilesState.Lost;
                    }
                }
                else if (item.Type == TilesType.White)
                {
                    grid.Background = GetBrush(TilesType.Incorrect);
                    State = TilesState.Lost;
                }
                StateChanged?.Invoke(State);
            }
        }

        private Grid Add(int row, int column)
        {
            TilesType type = TilesType.White;
            if (row <= offset)
            {
                type = TilesType.Finish;
            }
            else if (row >= total)
            {
                type = TilesType.Start;
            }
            else
            {
                type = (_selected[row] == column) ?
                TilesType.Black : TilesType.White;
            }
            Grid grid = new Grid()
            {
                Background = GetBrush(type),
                Tag = new TilesItem(row, type),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Colors.WhiteSmoke)
            };
            Grid.SetRow(grid, row);
            Grid.SetColumn(grid, column);
            grid.Tapped += Grid_Tapped;
            return grid;
        }

        private void Layout(ref Canvas canvas)
        {
            canvas.Children.Clear();
            int rows = total + offset;
            _layout = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            _layout.Children.Clear();
            _layout.ColumnDefinitions.Clear();
            _layout.RowDefinitions.Clear();
            for (int row = 0; (row < rows); row++)
            {
                _layout.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(tile_height)
                });
            }
            for (int column = 0; (column < columns); column++)
            {
                _layout.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(tile_width)
                });
            }
            for (int row = 0; (row < total + offset); row++)
            {
                for (int column = 0; (column < columns); column++)
                {
                    _layout.Children.Add(Add(row, column));
                }
            }
            _height = -tile_height * total + (tile_height * offset);
            Canvas.SetTop(_layout, _height);
            canvas.Children.Add(_layout);
        }

        public void Init(ref Canvas canvas)
        {
            _current = total;
            Time = TimeSpan.Zero;
            State = TilesState.Ready;
            _selected = Choose();
            Layout(ref canvas);
        }
    }

    public class Library
    {
        private const string app_title = "Tiles Game";

        private TilesBoard _board = new TilesBoard();
        private IAsyncOperation<IUICommand> _dialogCommand;
        private DispatcherTimer _timer;

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

        private void SetTextBlock(TimeSpan value, TextBlock time, TextBlock best)
        {
            time.Text = $@"Time:{Environment.NewLine}{value:ss\.fff}";
            best.Text = $@"Best:{Environment.NewLine}{_board.Best:ss\.fff}";
        }

        private async void Board_StateChanged(TilesState state)
        {
            switch (state)
            {
                case TilesState.Lost:
                    await ShowDialogAsync($@"Game Over, You Lost! Best Time: {_board.Best:ss\.fff}");
                    break;
                case TilesState.Complete:
                    await ShowDialogAsync($@"Completion Time: {_board.Time:ss\.fff}, Best Time: {_board.Best:ss\.fff}");
                    break;
            }
        }

        public void Timer(TextBlock time, TextBlock best)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += (object sender, object e) =>
            {
                if (_board.State == TilesState.Ready)
                {
                    SetTextBlock(_board.Time, time, best);
                }
                else if (_board.State == TilesState.Started)
                {
                    SetTextBlock(_board.Current, time, best);
                }
                else if (_board.State == TilesState.Complete)
                {
                    SetTextBlock(_board.Time, time, best);
                    _timer.Stop();
                }
                else
                {
                    _timer.Stop();
                }
            };
            _timer.Start();
        }

        public void Init(ref Canvas display, TextBlock time, TextBlock best)
        {
            _board.StateChanged += Board_StateChanged;
            _board.Init(ref display);
            Timer(time, best);
        }

        public void New(ref Canvas display, TextBlock time, TextBlock best)
        {
            _board.Init(ref display);
            Timer(time, best);
        }
    }
}