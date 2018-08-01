using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace EmojiGame
{
    public enum EmojiType
    {
        [Description("\U0001F600")]
        Grinning,
        [Description("\U0001F601")]
        Beaming,
        [Description("\U0001F602")]
        TearsOfJoy,
        [Description("\U0001F606")]
        Squinting,
        [Description("\U0001F609")]
        Winking,
        [Description("\U0001F60B")]
        Savouring,
        [Description("\U0000263A")]
        Smiling,
        [Description("\U0001F917")]
        Hugging,
        [Description("\U0001F914")]
        Thinking,
        [Description("\U0001F928")]
        RaisedEyebrow,
        [Description("\U0001F610")]
        Neutral,
        [Description("\U0001F611")]
        Expressionless,
        [Description("\U0001F644")]
        RollingEyes,
        [Description("\U0001F623")]
        Persevering,
        [Description("\U0001F62E")]
        OpenMouth,
        [Description("\U0001F62F")]
        Hushed,
        [Description("\U0001F62A")]
        Sleepy,
        [Description("\U0001F62B")]
        Tired,
        [Description("\U0001F634")]
        Sleeping,
        [Description("\U0001F60C")]
        Relieved,
        [Description("\U0001F612")]
        Unamused,
        [Description("\U0001F614")]
        Pensive,
        [Description("\U0001F615")]
        Confused,
        [Description("\U0001F632")]
        Astonished,
        [Description("\U00002639")]
        Frowning,
        [Description("\U0001F616")]
        Confounded,
        [Description("\U0001F61E")]
        Disappointed,
        [Description("\U0001F61F")]
        Worried,
        [Description("\U0001F624")]
        Triumph,
        [Description("\U0001F627")]
        Anguished,
        [Description("\U0001F628")]
        Fearful,
        [Description("\U0001F633")]
        Flushed,
        [Description("\U0001F92A")]
        Zany,
        [Description("\U0001F635")]
        Dizzy,
        [Description("\U0001F620")]
        Angry,
        [Description("\U0001F913")]
        Nerdy
    }

    public enum EmojiState
    {
        None, Correct, Incorrect
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

    public class EmojiItem : BindableBase
    {
        private EmojiType _type;
        private EmojiState _state;
        private bool _correct;

        public EmojiType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public EmojiState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public bool Correct
        {
            get { return _correct; }
            set { SetProperty(ref _correct, value); }
        }

        public EmojiItem(EmojiType type, bool correct)
        {
            Type = type;
            Correct = correct;
            State = EmojiState.None;
        }
    }

    public class EmojiBoard : BindableBase
    {
        public const int Rounds = 12;
        public const int Columns = 3;

        private readonly List<EmojiType> types =
        Enum.GetValues(typeof(EmojiType)).Cast<EmojiType>().ToList();
        private readonly Random random = new Random((int)DateTime.Now.Ticks);

        private List<EmojiType> _selected = new List<EmojiType>();
        private List<EmojiType> _options = new List<EmojiType>();
        private int _current = 0;

        private List<EmojiType> ChooseTypes()
        {
            EmojiType selected;
            List<EmojiType> types = new List<EmojiType>();
            while ((types.Count < Rounds))
            {
                selected = (EmojiType)random.Next(0, this.types.Count - 1);
                if ((!types.Contains(selected)) || (types.Count < 1))
                {
                    types.Add(selected);
                }
            }
            return types;
        }

        private List<int> ChooseOptions(List<EmojiType> source)
        {
            int selected;
            List<int> target = new List<int>();
            while ((target.Count < Columns))
            {
                selected = random.Next(0, source.Count - 1);
                if ((!target.Contains(selected)) || (target.Count < 1))
                {
                    target.Add(selected);
                }
            }
            return target;
        }

        private void Shuffle(List<EmojiItem> list)
        {
            int count = list.Count;
            while (count > 1)
            {
                count--;
                int index = random.Next(count + 1);
                EmojiItem value = list[index];
                list[index] = list[count];
                list[count] = value;
            }
        }

        private IEnumerable<string> SplitCapital(string text)
        {
            Regex regex = new Regex(@"\p{Lu}\p{Ll}*");
            foreach (Match match in regex.Matches(text))
            {
                yield return match.Value;
            }
        }

        private string GetText(EmojiType type)
        {
            return string.Join(" ", SplitCapital(Enum.GetName(typeof(EmojiType), type)));
        }

        private string _text;
        private ObservableCollection<EmojiItem> _items = new ObservableCollection<EmojiItem>();

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public ObservableCollection<EmojiItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public EmojiBoard()
        {
            _selected = ChooseTypes();
            _options = types.Where(types => !_selected.Any(selected => selected == types)).ToList();
        }

        public bool Next()
        {
            if (_current < Rounds)
            {
                Items.Clear();
                EmojiType answer = _selected[_current];
                List<EmojiItem> selections = new List<EmojiItem>
                {
                    new EmojiItem(answer, true)
                };
                Text = GetText(answer);
                List<int> indexes = ChooseOptions(_options);
                int indexOne = indexes[0];
                int indexTwo = indexes[1];
                EmojiType one = _options[indexOne];
                EmojiType two = _options[indexTwo];
                _options.RemoveAt(indexOne);
                _options.RemoveAt(indexTwo);
                selections.Add(new EmojiItem(one, false));
                selections.Add(new EmojiItem(two, false));
                Shuffle(selections);
                foreach (EmojiItem item in selections)
                {
                    Items.Add(item);
                }
                _current++;
                return true;
            }
            return false;
        }

        public void SetState(EmojiItem selected)
        {
            foreach (EmojiItem item in Items)
            {
                if (selected.Type == item.Type)
                {
                    item.Correct = selected.Correct;
                    item.State = item.Correct ?
                    EmojiState.Correct : EmojiState.Incorrect;
                }
            }
        }
    }

    public class EmojiTypeToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is EmojiType type)
            {
                FieldInfo info = type.GetType().GetField(type.ToString());
                DescriptionAttribute[] attr =
                    (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attr[0].Description;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class EmojiStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is EmojiState state)
            {
                Color[] colors = new Color[] { Colors.Transparent, Colors.ForestGreen, Colors.IndianRed };
                return new SolidColorBrush(colors[(int)state]);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class Library
    {
        private const string app_title = "Emoji Game";

        private bool _gameOver = false;
        private EmojiItem _selected = null;
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

        public EmojiBoard Board { get; set; } = new EmojiBoard();

        public void Init(ref ItemsControl display, ref TextBlock text)
        {
            text.SetBinding(TextBlock.TextProperty, new Binding()
            {
                Source = Board,
                Path = new PropertyPath("Text"),
                Mode = BindingMode.OneWay
            });
            _gameOver = false;
            Board.Next();
            display.ItemsSource = Board.Items;
        }

        public async void Tapped(Button button)
        {
            if (!_gameOver)
            {
                _selected = (EmojiItem)button.Tag;
                Board.SetState(_selected);
                if (_selected.Correct)
                {
                    if (!Board.Next())
                    {
                        await ShowDialogAsync("Game Over, You Won!");
                        _gameOver = true;
                    }
                }
                else
                {
                    await ShowDialogAsync("Incorrect, You Lost!");
                    _gameOver = true;
                }
            }
            else
            {
                await ShowDialogAsync("Game Over!");
            }
        }

        public void New(ref ItemsControl display, ref TextBlock text)
        {
            Board = new EmojiBoard();
            Init(ref display, ref text);
            Board.Next();
        }
    }
}