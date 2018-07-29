using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace RacerGame
{
    public enum RacerOption
    {
        Top, Middle, Bottom
    }

    public enum RacerState
    {
        Select, Ready, Started, Finished
    }

    public class Racer
    {
        public RacerOption Option { get; set; }
        public TimeSpan Time { get; set; }
    }

    public class Library
    {
        private const string app_title = "Racer Game";
        private const int total = 3;
        private const int width = 200;
        private const int height = 10;
        private readonly Random random = new Random((int)DateTime.Now.Ticks);

        private IAsyncOperation<ContentDialogResult> _dialogResult = null;
        private List<Grid> _items = new List<Grid>();

        private RacerState _state;
        private RacerOption _selected;
        private RacerOption _winner;
        private bool _finished;
        private int _count = 0;

        private async Task<bool> ShowDialogAsync(string content, string primary = "Ok",
            string close = "Close", string title = app_title)
        {
            try
            {
                if (_dialogResult != null)
                {
                    _dialogResult.Cancel();
                    _dialogResult = null;
                }
                _dialogResult = new ContentDialog
                {
                    Title = title,
                    Content = content,
                    CloseButtonText = close,
                    PrimaryButtonText = primary,
                    DefaultButton = ContentDialogButton.Primary,
                }.ShowAsync();
                return await _dialogResult == ContentDialogResult.Primary;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        private List<TimeSpan> Choose(int start, int finish, int total)
        {
            TimeSpan selected;
            List<TimeSpan> selections = new List<TimeSpan>();
            while ((selections.Count < total))
            {
                selected = TimeSpan.FromSeconds(random.Next(start, finish) % finish);
                if ((!selections.Contains(selected)) || (selections.Count < 1))
                {
                    selections.Add(selected);
                }
            }
            return selections;
        }

        private async void Storyboard_Completed(object sender, object e)
        {
            if (_state == RacerState.Started)
            {
                Storyboard storyboard = (Storyboard)sender;
                TimeSpan duration = storyboard.GetCurrentTime();
                Racer racer = (Racer)_items.FirstOrDefault(w => ((Racer)w.Tag).Time == duration).Tag;
                _count++;
                if (_count == 1)
                {
                    _winner = racer.Option;
                    string name = Enum.GetName(typeof(RacerOption), _winner);
                    await ShowDialogAsync($"{name} completed Race in {duration.ToString()}");
                }
                if (_count == total)
                {
                    _state = RacerState.Finished;
                    ShowMessage();
                }
                _finished = true;
            }
        }

        private void Move(Grid grid, double from, double to, TimeSpan duration)
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = duration,
                EasingFunction = new ExponentialEase()
                {
                    EasingMode = EasingMode.EaseIn
                }
            };
            Storyboard storyboard = new Storyboard();
            Storyboard.SetTargetProperty(animation, "(Canvas.Left)");
            Storyboard.SetTarget(animation, grid);
            storyboard.Completed += Storyboard_Completed;
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void Race()
        {
            if (_state != RacerState.Ready)
            {
                ShowMessage();
                return;
            }
            List<TimeSpan> times = Choose(5, 15, total);
            int i = 0;
            foreach (Grid grid in _items)
            {
                Racer racer = (Racer)grid.Tag;
                racer.Time = times[i];
                Move(grid, width, 0, racer.Time);
                i++;
            }
            _state = RacerState.Started;
        }

        private void Start()
        {
            _count = 0;
            _finished = false;
            _state = RacerState.Select;
            ShowMessage();
        }

        private async void ShowMessage()
        {
            switch (_state)
            {
                case RacerState.Select:
                    {
                        await ShowDialogAsync("Select Racer");
                    }
                    break;
                case RacerState.Ready:
                    {
                        string name = Enum.GetName(typeof(RacerOption), _selected);
                        bool result = await ShowDialogAsync($"Selected {name} to Win, select Play to Race", "Play");
                        if (result)
                        {
                            Race();
                        }
                    }
                    break;
                case RacerState.Finished:
                    {
                        string winnerName = Enum.GetName(typeof(RacerOption), _winner);
                        string selectedName = Enum.GetName(typeof(RacerOption), _selected);
                        string content = (_winner == _selected) ?
                        $"Won Race with {winnerName}, select New to Race again!" :
                        $"Racer {selectedName} Lost, {winnerName} Won - select New to try again.";
                        bool result = await ShowDialogAsync(content, "New");
                        if (_finished)
                        {
                            foreach (Grid grid in _items)
                            {
                                Move(grid, 0, width, TimeSpan.FromSeconds(1));
                            }
                            _finished = false;
                        }
                        if (result)
                        {
                            Start();
                        }
                    }
                    break;
            }
        }

        private void Racer_Tapped(object sender, RoutedEventArgs e)
        {
            if (_state == RacerState.Select)
            {
                Grid grid = (Grid)sender;
                Racer racer = (Racer)grid.Tag;
                _selected = racer.Option;
                _state = RacerState.Ready;
                ShowMessage();
            }
        }

        private Grid AddRacer(RacerOption type, int left)
        {
            TextBlock textblock = new TextBlock()
            {
                Text = "\U0001F3CE",
                IsColorFontEnabled = true,
                FontFamily = new FontFamily("Segoe UI Emoji")
            };
            Viewbox viewbox = new Viewbox() { Child = textblock };
            Grid grid = new Grid()
            {
                Tag = new Racer() { Option = type }
            };
            grid.Tapped += Racer_Tapped;
            grid.Children.Add(viewbox);
            Canvas.SetLeft(grid, left);
            return grid;
        }

        private void Add(ref Grid grid, RacerOption type, int row)
        {
            Canvas canvas = new Canvas()
            {
                Width = width,
                Margin = new Thickness(0, 0, 0, 30)
            };
            Grid racer = AddRacer(type, width);
            _items.Add(racer);
            canvas.Children.Add(racer);
            canvas.SetValue(Grid.RowProperty, row);
            grid.Children.Add(canvas);
        }

        private void Layout(ref Grid display)
        {
            _items.Clear();
            display.Children.Clear();
            StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
            Grid track = new Grid();
            for (int row = 0; row < total; row++)
            {
                track.RowDefinitions.Add(new RowDefinition());
                Add(ref track, (RacerOption)row, row);
            }
            Grid finish = new Grid()
            {
                Width = 5,
                Background = new SolidColorBrush(Colors.Black)
            };
            panel.Children.Add(finish);
            panel.Children.Add(track);
            display.Children.Add(panel);
        }

        public void Init(ref Grid display)
        {
            _count = 0;
            _state = RacerState.Select;
            Layout(ref display);
        }

        public void New()
        {
            Start();
        }

        public void Play()
        {
            Race();
        }
    }
}