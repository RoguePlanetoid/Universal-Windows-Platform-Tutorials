using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Linq;

namespace PomodoroApp
{
    public enum PomodoroType
    {
        [Description("\U0001F345")]
        TaskTimer = 25,
        [Description("\U00002615")]
        ShortBreak = 1,
        [Description("\U0001F34F")]
        LongBreak = 15
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

    public class PomodoroItem : BindableBase
    {
        private PomodoroType _type;
        private Color _light;
        private Color _dark;

        private string GetGlyph(PomodoroType type)
        {
            FieldInfo info = type.GetType().GetField(type.ToString());
            DescriptionAttribute[] attr =
            (DescriptionAttribute[])info.GetCustomAttributes
            (typeof(DescriptionAttribute), false);
            return attr[0].Description;
        }

        private string GetId(PomodoroType type)
        {
            return Enum.GetName(typeof(PomodoroType), type);
        }

        private IEnumerable<string> GetName(PomodoroType type)
        {
            string text = GetId(type);
            Regex regex = new Regex(@"\p{Lu}\p{Ll}*");
            foreach (Match match in regex.Matches(text))
            {
                yield return match.Value;
            }
        }

        public PomodoroType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public Color Dark
        {
            get { return _dark; }
            set { SetProperty(ref _dark, value); }
        }

        public Color Light
        {
            get { return _light; }
            set { SetProperty(ref _light, value); }
        }

        public string Id => GetId(_type);

        public string Glyph => GetGlyph(_type);

        public string Name => string.Join(" ", GetName(_type));

        public TimeSpan TimeSpan => TimeSpan.FromMinutes((double)_type);

        public PomodoroItem(PomodoroType type, Color dark, Color light)
        {
            Type = type;
            Dark = dark;
            Light = light;
        }
    }

    public class PomodoroSetup : BindableBase
    {
        private bool _started;
        private string _display;
        private DateTime _start;
        private DateTime _finish;
        private TimeSpan _current;
        private PomodoroItem _item;
        private List<PomodoroItem> _items;
        private DispatcherTimer _timer;
        private ToastNotifier _notifier = ToastNotificationManager.CreateToastNotifier();

        private ScheduledToastNotification GetToast()
        {
            return _notifier.GetScheduledToastNotifications().FirstOrDefault();
        }

        private void ClearToast()
        {
            foreach (ScheduledToastNotification toast in _notifier.GetScheduledToastNotifications())
            {
                _notifier.RemoveFromSchedule(toast);
            }
        }

        private void AddToast(string id, string name, string glyph, DateTime start, DateTime finish)
        {
            XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            xml.GetElementsByTagName("text")[0].InnerText = $"{glyph} {name}";
            xml.GetElementsByTagName("text")[1].InnerText = $"Start {start:HH:mm} Finish {finish:HH:mm}";
            ScheduledToastNotification toast = new ScheduledToastNotification(xml, finish) { Id = id };
            _notifier.AddToSchedule(toast);
        }

        private string GetDisplay(TimeSpan timeSpan) => timeSpan.ToString(@"mm\:ss");

        private void Start()
        {
            _started = true;
            if (_timer == null)
            {
                _timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(250)
                };
                _timer.Tick += (object s, object args) =>
                {
                    TimeSpan difference = _start - DateTime.UtcNow;
                    string display = GetDisplay(_current + difference);
                    if (_started && display != GetDisplay(TimeSpan.Zero))
                    {
                        Display = display;
                    }
                    else
                    {
                        _current = TimeSpan.Zero;
                        Display = GetDisplay(_current);
                        _timer.Stop();
                        _started = false;
                    }
                };
            }
            _timer.Start();
        }

        private void Stop()
        {
            if (_timer != null) _timer.Stop();
            _started = false;
            Select(_item);
        }

        public PomodoroItem Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value); }
        }

        public List<PomodoroItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public string Display
        {
            get { return _display; }
            set { SetProperty(ref _display, value); }
        }

        public bool Started
        {
            get { return _started; }
            set { SetProperty(ref _started, value); }
        }

        public void Select(PomodoroItem item)
        {
            Item = item;
            _current = Item.TimeSpan;
            Display = GetDisplay(_current);
        }

        public void Init()
        {
            if (Items == null)
            {
                Items = new List<PomodoroItem>
                {
                    new PomodoroItem(PomodoroType.TaskTimer,
                    Color.FromArgb(255, 240, 58, 23),
                    Color.FromArgb(255, 239, 105, 80)),
                    new PomodoroItem(PomodoroType.ShortBreak,
                    Color.FromArgb(255, 131, 190, 236),
                    Color.FromArgb(255, 179, 219, 212)),
                    new PomodoroItem(PomodoroType.LongBreak,
                    Color.FromArgb(255, 186, 216, 10),
                    Color.FromArgb(255, 228, 245, 119)),
                };
            }
            ScheduledToastNotification toast = GetToast();
            PomodoroItem item = Items[0];
            if (toast != null)
            {
                item = Items.FirstOrDefault(f => f.Id == toast.Id);
                _start = toast.DeliveryTime.UtcDateTime - item.TimeSpan;
                _finish = toast.DeliveryTime.UtcDateTime;
                Start();
            }
            Select(item);
        }

        public void Toggle()
        {
            _start = DateTime.UtcNow;
            _finish = _start.Add(TimeSpan.FromMinutes((double)Item.Type));
            if (_started)
            {
                ClearToast();
                Stop();
            }
            else
            {
                AddToast(_item.Id, _item.Name, _item.Glyph, _start, _finish);
                _current = Item.TimeSpan;
                Start();
            }
        }
    }

    public class Library
    {
        private const string app_title = "Pomodoro App";

        private IAsyncOperation<IUICommand> _dialogCommand;
        private PomodoroSetup _setup = new PomodoroSetup();

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton button = (AppBarButton)sender;
            if (_setup.Started)
            {
                await ShowDialogAsync($"You will need to Stop {_setup.Item.Name} Timer to Switch");
            }
            else
            {
                _setup.Select((PomodoroItem)button.Tag);
            }
        }

        public void Init(ref CommandBar command, ref Grid display)
        {
            _setup.Init();
            if (command.PrimaryCommands.Count == 1)
            {
                foreach (PomodoroItem item in _setup.Items)
                {
                    AppBarButton button = new AppBarButton()
                    {
                        Tag = item,
                        Content = item.Name,
                        Icon = new FontIcon()
                        {
                            Glyph = item.Glyph,
                            Margin = new Thickness(0, -5, 0, 0),
                            FontFamily = new FontFamily("Segoe UI Emoji")
                        }
                    };
                    button.Click += Button_Click;
                    command.PrimaryCommands.Add(button);
                }
            }
            display.DataContext = _setup;
        }

        public void Toggle()
        {
            _setup.Toggle();
        }
    }
}