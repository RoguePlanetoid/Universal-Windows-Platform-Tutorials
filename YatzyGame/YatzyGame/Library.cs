using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace YatzyGame
{
    public enum YatzyScoreType
    {
        AcesScore, TwosScore, ThreesScore, FoursScore, FivesScore, SixesScore,
        UpperTotalScore, UpperTotalBonusScore, ThreeOfAKindScore, FourOfAKindScore,
        FullHouseScore, SmallStraightScore, LargeStraightScore, YahtzeeScore,
        ChanceScore, YahtzeeBonusScore, LowerTotalScore, TotalScore
    }

    public class CommandHandler : ICommand
    {
        public event EventHandler CanExecuteChanged = null;
        private readonly Action<object> _action;

        public CommandHandler(Action<object> action) { _action = action; }

        public bool CanExecute(object parameter) { return true; }

        public void Execute(object parameter) { _action(parameter); }
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

    public class YatzyDice : BindableBase
    {
        private ICommand _command;
        private Random _random;
        private int _index;
        private int _value;
        private bool _hold;

        public ICommand Command
        {
            get { return _command; }
            set { SetProperty(ref _command, value); }
        }

        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }

        public int Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public bool Hold
        {
            get { return _hold; }
            set { SetProperty(ref _hold, value); }
        }

        public YatzyDice(Random random)
        {
            _random = random;
        }

        public void Roll()
        {
            if (!Hold) Value = _random.Next(1, 7);
        }
    }

    public class YatzyCalculate : BindableBase
    {
        private const int diceTotal = 5;

        private int _total;
        private int _upperTotal;
        private int _lowerTotal;
        private bool _upperBonus;

        public void ResetScores()
        {
            _total = 0;
            _upperTotal = 0;
            _lowerTotal = 0;
            _upperBonus = false;
        }

        public void UpdateTotals(int score, bool upperScore)
        {
            if (upperScore)
            {
                _upperTotal += score;
                if (_upperTotal >= 63) _upperBonus = true;
            }
            else
            {
                _lowerTotal += score;
            }
            _total = 0;
            _total += _upperTotal;
            if (_upperBonus == true) _total += 35;
            _total += _lowerTotal;
        }

        public int GetAddUp(ref YatzyDice[] dice, int value)
        {
            int sum = 0;
            for (int i = 0; i < dice.Length; i++)
            {
                if (dice[i].Value == value)
                {
                    sum += value;
                }
            }
            return sum;
        }

        public int GetOfAKind(ref YatzyDice[] dice, int value)
        {
            int sum = 0;
            bool result = false;
            for (int i = 1; i <= 6; i++)
            {
                int count = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (dice[j].Value == i) count++;
                    if (count > value) result = true;
                }
            }
            if (result)
            {
                for (int i = 0; i < dice.Length; i++)
                {
                    sum += dice[i].Value;
                }
            }
            return sum;
        }

        public int GetFullHouse(ref YatzyDice[] dice)
        {
            int sum = 0;
            int[] item = dice.Select(s => s.Value).ToArray();
            Array.Sort(item);
            if ((((item[0] == item[1]) && (item[1] == item[2])) && // Three of a Kind
               (item[3] == item[4]) && // Two of a Kind
               (item[2] != item[3])) ||
               ((item[0] == item[1]) && // Two of a Kind
               ((item[2] == item[3]) && (item[3] == item[4])) && // Three of a Kind
               (item[1] != item[2])))
            {
                sum = 25;
            }
            return sum;
        }

        public int GetSmallStraight(ref YatzyDice[] dice)
        {
            int sort = 0;
            int[] item = dice.Select(s => s.Value).ToArray();
            Array.Sort(item);
            for (int j = 0; j < 4; j++)
            {
                int value = 0;
                if (item[j] == item[j + 1])
                {
                    value = item[j];
                    for (int k = j; k < 4; k++)
                    {
                        item[k] = item[k + 1];
                    }
                    item[4] = value;
                }
            }
            if (((item[0] == 1) && (item[1] == 2) && (item[2] == 3) && (item[3] == 4)) ||
                ((item[0] == 2) && (item[1] == 3) && (item[2] == 4) && (item[3] == 5)) ||
                ((item[0] == 3) && (item[1] == 4) && (item[2] == 5) && (item[3] == 6)) ||
                ((item[1] == 1) && (item[2] == 2) && (item[3] == 3) && (item[4] == 4)) ||
                ((item[1] == 2) && (item[2] == 3) && (item[3] == 4) && (item[4] == 5)) ||
                ((item[1] == 3) && (item[2] == 4) && (item[3] == 5) && (item[4] == 6)))
            {
                sort = 30;
            }
            return sort;
        }

        public int GetLargeStraight(ref YatzyDice[] dice)
        {
            int sum = 0;
            int[] i = dice.Select(s => s.Value).ToArray();
            Array.Sort(i);
            if (((i[0] == 1) && (i[1] == 2) && (i[2] == 3) && (i[3] == 4) && (i[4] == 5)) ||
                ((i[0] == 2) && (i[1] == 3) && (i[2] == 4) && (i[3] == 5) && (i[4] == 6)))
            {
                sum = 40;
            }
            return sum;
        }

        public int GetYahtzee(ref YatzyDice[] dice)
        {
            int sum = 0;
            for (int i = 1; i <= 6; i++)
            {
                int Count = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (dice[j].Value == i) Count++;
                    if (Count > 4) sum = 50;
                }
            }
            return sum;
        }

        public int GetChance(ref YatzyDice[] dice)
        {
            int sum = 0;
            for (int i = 0; i < 5; i++)
            {
                sum += dice[i].Value;
            }
            return sum;
        }

        public int TotalScore
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }

        public int UpperTotalScore
        {
            get { return _upperTotal; }
            set { SetProperty(ref _upperTotal, value); }
        }

        public int LowerTotalScore
        {
            get { return _lowerTotal; }
            set { SetProperty(ref _lowerTotal, value); }
        }

        public bool UpperBonus
        {
            get { return _upperBonus; }
            set { SetProperty(ref _upperBonus, value); }
        }
    }

    public class IntegerToGlyphConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int face)
            {
                string[] faces = { null, "\u2680", "\u2681", "\u2682", "\u2683", "\u2684", "\u2685" };
                return faces[face];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class YatzyItem : BindableBase
    {
        private int _score;
        private ICommand _command = null;
        private YatzyScoreType _type;

        public int Score
        {
            get { return _score; }
            set { SetProperty(ref _score, value); }
        }

        public ICommand Command
        {
            get { return _command; }
            set { SetProperty(ref _command, value); }
        }

        public YatzyScoreType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        public string Content
        {
            get
            {
                IEnumerable<string> GetContent(YatzyScoreType type)
                {
                    string text = Enum.GetName(typeof(YatzyScoreType), type);
                    Regex regex = new Regex(@"\p{Lu}\p{Ll}*");
                    foreach (Match match in regex.Matches(text))
                    {
                        yield return match.Value;
                    }
                }
                return string.Join(" ", GetContent(_type));
            }
        }
    }

    public class YatzyBoard : BindableBase
    {
        private const int total_dice = 5;
        private const string accept = "Do you wish to Accept?";
        private readonly Random random = new Random((int)DateTime.Now.Ticks);

        private YatzyDice[] _dice = new YatzyDice[total_dice]
        {
            null, null, null, null, null
        };
        private YatzyCalculate _calculate = new YatzyCalculate();
        private List<YatzyItem> _items = new List<YatzyItem>();
        private Func<string, Task<bool>> _confirm = null;
        private int _rollCount = 0;
        private int _scoreCount = 0;

        private YatzyItem GetItemByType(YatzyScoreType type)
        {
            return _items.FirstOrDefault(t => t.Type == type);
        }

        private void SetItemScoreByType(YatzyScoreType type, int score)
        {
            YatzyItem item = GetItemByType(type);
            if (item != null)
            {
                item.Score = score;
            }
        }

        private int GetItemScoreByType(YatzyScoreType type)
        {
            YatzyItem item = GetItemByType(type);
            if (item != null)
            {
                return item.Score;
            }
            return 0;
        }

        private async void AddUpDice(YatzyScoreType type, int value)
        {
            int score = GetItemScoreByType(type);
            if (_rollCount > 0 && score == 0)
            {
                int total = _calculate.GetAddUp(ref _dice, value);
                bool result = await _confirm($"Total is {total}. {accept}");
                if (result)
                {
                    SetItemScoreByType(type, total);
                    _calculate.UpdateTotals(total, true);
                    Reset();
                }
            }
        }

        private async void ValueOfAKind(YatzyScoreType type, int value, string name)
        {
            int score = GetItemScoreByType(type);
            if (_rollCount > 0 && score == 0)
            {
                int total = _calculate.GetOfAKind(ref _dice, value - 1);
                if (total != 0)
                {
                    bool result = await _confirm($"Total is {total}. {accept}");
                    if (result)
                    {
                        SetItemScoreByType(type, total);
                        _calculate.UpdateTotals(total, false);
                        Reset();
                    }
                }
                else
                {
                    bool result = await _confirm($"No {name} of a Kind. {accept}");
                    if (result)
                    {
                        SetItemScoreByType(type, 0);
                        _calculate.UpdateTotals(total, false);
                        Reset();
                    }
                }
            }
        }

        private async void ItemScore(YatzyScoreType type, int value, string name)
        {
            int score = GetItemScoreByType(type);
            if ((_rollCount > 0) && (score == 0))
            {
                int total = 0;
                if (type == YatzyScoreType.FullHouseScore)
                {
                    total = _calculate.GetFullHouse(ref _dice);
                }
                else if(type == YatzyScoreType.SmallStraightScore)
                {
                    total = _calculate.GetSmallStraight(ref _dice);
                }
                else if (type == YatzyScoreType.LargeStraightScore)
                {
                    total = _calculate.GetLargeStraight(ref _dice);
                }
                if (total == value)
                {
                    SetItemScoreByType(type, total);
                    _calculate.UpdateTotals(total, false);
                    Reset();
                }
                else
                {
                    bool result = await _confirm($"No {name}. {accept}");
                    if (result)
                    {
                        SetItemScoreByType(type, 0);
                        _calculate.UpdateTotals(total, false);
                        Reset();
                    }
                }
            }
        }

        private async void Yahtzee()
        {
            int score = GetItemScoreByType(YatzyScoreType.YahtzeeScore);
            if ((_rollCount > 0) && (score == 0))
            {
                int total = _calculate.GetYahtzee(ref _dice);
                if (total == 50)
                {
                    SetItemScoreByType(YatzyScoreType.YahtzeeScore, total);
                    _calculate.UpdateTotals(total, false);
                    Reset();
                }
                else
                {
                    bool result = await _confirm($"No Yahtzee. {accept}");
                    if (result)
                    {
                        SetItemScoreByType(YatzyScoreType.YahtzeeScore, 0);
                        SetItemScoreByType(YatzyScoreType.YahtzeeBonusScore, 0);
                        _scoreCount++;
                        _calculate.UpdateTotals(total, true);
                        Reset();
                    }
                }
            }
        }

        private async void Chance()
        {
            int score = GetItemScoreByType(YatzyScoreType.ChanceScore);
            if ((_rollCount > 0) && (score == 0))
            {
                int total = _calculate.GetChance(ref _dice);
                bool result = await _confirm($"Total is {total}. {accept}");
                if (result)
                {
                    SetItemScoreByType(YatzyScoreType.ChanceScore, total);
                    _calculate.UpdateTotals(total, false);
                    Reset();
                }
            }
        }

        private void YahtzeeBonus()
        {
            int yahtzeeScore = GetItemScoreByType(YatzyScoreType.YahtzeeScore);
            int yahtzeeBonusScore = GetItemScoreByType(YatzyScoreType.YahtzeeBonusScore);
            if ((_rollCount > 0) && (yahtzeeScore == 0) && (yahtzeeBonusScore != 0))
            {
                int total = _calculate.GetYahtzee(ref _dice);
                if (total == 50)
                {
                    SetItemScoreByType(YatzyScoreType.YahtzeeBonusScore, 100);
                    _calculate.UpdateTotals(100, false);
                    Reset();
                }
                else
                {
                    SetItemScoreByType(YatzyScoreType.YahtzeeBonusScore, 0);
                    _calculate.UpdateTotals(0, true);
                    Reset();
                }
            }
        }

        private void Hold(int index)
        {
            if (_rollCount != 0)
            {
                _dice[index].Hold = !_dice[index].Hold;
            }
        }

        public YatzyBoard()
        {
            for (int i = 0; i < total_dice; i++)
            {
                _dice[i] = new YatzyDice(random)
                {
                    Index = i,
                    Command = new CommandHandler((param) => Hold((int)param))
                };
            }
            _items.Clear();
            foreach (YatzyScoreType type in Enum.GetValues(typeof(YatzyScoreType)))
            {
                YatzyItem item = new YatzyItem() { Type = type };
                switch (item.Type)
                {
                    case YatzyScoreType.AcesScore:
                        item.Command = new CommandHandler((p) => AddUpDice(item.Type, 1));
                        break;
                    case YatzyScoreType.TwosScore:
                        item.Command = new CommandHandler((p) => AddUpDice(item.Type, 2));
                        break;
                    case YatzyScoreType.ThreesScore:
                        item.Command = new CommandHandler((p) => AddUpDice(item.Type, 3));
                        break;
                    case YatzyScoreType.FoursScore:
                        item.Command = new CommandHandler((p) => AddUpDice(item.Type, 4));
                        break;
                    case YatzyScoreType.FivesScore:
                        item.Command = new CommandHandler((p) => AddUpDice(item.Type, 5));
                        break;
                    case YatzyScoreType.SixesScore:
                        item.Command = new CommandHandler((p) => AddUpDice(item.Type, 6));
                        break;
                    case YatzyScoreType.ThreeOfAKindScore:
                        item.Command = new CommandHandler((p) => ValueOfAKind(item.Type, 3, "Three"));
                        break;
                    case YatzyScoreType.FourOfAKindScore:
                        item.Command = new CommandHandler((p) => ValueOfAKind(item.Type, 4, "Four"));
                        break;
                    case YatzyScoreType.FullHouseScore:
                        item.Command = new CommandHandler((p) => ItemScore(item.Type, 25, "Full House"));
                        break;
                    case YatzyScoreType.SmallStraightScore:
                        item.Command = new CommandHandler((p) => ItemScore(item.Type, 30, "Small Straight"));
                        break;
                    case YatzyScoreType.LargeStraightScore:
                        item.Command = new CommandHandler((p) => ItemScore(item.Type, 40, "Large Straight"));
                        break;
                    case YatzyScoreType.YahtzeeScore:
                        item.Command = new CommandHandler((p) => Yahtzee());
                        break;
                    case YatzyScoreType.ChanceScore:
                        item.Command = new CommandHandler((p) => Chance());
                        break;
                    case YatzyScoreType.YahtzeeBonusScore:
                        item.Command = new CommandHandler((p) => YahtzeeBonus());
                        break;
                }
                _items.Add(item);
            }
        }

        public YatzyBoard(Func<string, Task<bool>> dialog) : this()
        {
            _confirm = dialog;
        }

        public YatzyDice[] Dice
        {
            get { return _dice; }
            set { SetProperty(ref _dice, value); }
        }

        public int RollCount
        {
            get { return _rollCount; }
            set { SetProperty(ref _rollCount, value); }
        }

        public int ScoreCount
        {
            get { return _scoreCount; }
            set { SetProperty(ref _scoreCount, value); }
        }

        public List<YatzyItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public void Clear()
        {
            _calculate.ResetScores();
            _rollCount = 0;
            _scoreCount = 0;
            SetItemScoreByType(YatzyScoreType.UpperTotalScore, _calculate.UpperTotalScore);
            SetItemScoreByType(YatzyScoreType.UpperTotalBonusScore, 0);
            SetItemScoreByType(YatzyScoreType.LowerTotalScore, _calculate.LowerTotalScore);
            SetItemScoreByType(YatzyScoreType.TotalScore, _calculate.TotalScore);
            SetItemScoreByType(YatzyScoreType.AcesScore, 0);
            SetItemScoreByType(YatzyScoreType.TwosScore, 0);
            SetItemScoreByType(YatzyScoreType.ThreesScore, 0);
            SetItemScoreByType(YatzyScoreType.FoursScore, 0);
            SetItemScoreByType(YatzyScoreType.FivesScore, 0);
            SetItemScoreByType(YatzyScoreType.SixesScore, 0);
            SetItemScoreByType(YatzyScoreType.ThreeOfAKindScore, 0);
            SetItemScoreByType(YatzyScoreType.FourOfAKindScore, 0);
            SetItemScoreByType(YatzyScoreType.FullHouseScore, 0);
            SetItemScoreByType(YatzyScoreType.SmallStraightScore, 0);
            SetItemScoreByType(YatzyScoreType.LargeStraightScore, 0);
            SetItemScoreByType(YatzyScoreType.YahtzeeScore, 0);
            SetItemScoreByType(YatzyScoreType.ChanceScore, 0);
            SetItemScoreByType(YatzyScoreType.YahtzeeScore, 0);
            int value = 1;
            foreach (YatzyDice die in _dice)
            {
                die.Hold = false;
                die.Value = value++;
            }
        }

        public async void Reset()
        {
            _rollCount = 0;
            _scoreCount++;
            foreach (YatzyDice die in _dice)
            {
                die.Hold = false;
            }
            SetItemScoreByType(YatzyScoreType.UpperTotalScore, _calculate.UpperTotalScore);
            if (_calculate.UpperBonus)
            {
                SetItemScoreByType(YatzyScoreType.UpperTotalBonusScore, 35);
            }
            else
            {
                SetItemScoreByType(YatzyScoreType.UpperTotalBonusScore, 0);
            }
            SetItemScoreByType(YatzyScoreType.LowerTotalScore, _calculate.LowerTotalScore);
            SetItemScoreByType(YatzyScoreType.TotalScore, _calculate.TotalScore);
            if (_scoreCount == 14)
            {
                int total = GetItemScoreByType(YatzyScoreType.TotalScore);
                bool result = await _confirm($"Game Over, Score {total}. Play again?");
                if (result)
                {
                    Clear();
                }
            }
        }

        public void Roll()
        {
            if (_rollCount < 3)
            {
                if (_rollCount == 0)
                {
                    foreach (YatzyDice die in _dice)
                    {
                        die.Hold = false;
                    }
                }
                foreach (YatzyDice die in _dice)
                {
                    die.Roll();
                }
                _rollCount++;
                if (_rollCount == 3)
                {
                    foreach (YatzyDice die in _dice)
                    {
                        die.Hold = true;
                    }
                }
            }
        }
    }

    public class YatzyItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ScoreItem { get; set; }
        public DataTemplate TotalItem { get; set; }

        protected override DataTemplate SelectTemplateCore(object value, DependencyObject container)
        {
            if (value is YatzyItem item)
            {
                return item.Command != null ? ScoreItem : TotalItem;
            }
            return null;
        }
    }

    public class Library
    {
        private const string app_title = "Yatzy Game";

        private YatzyBoard _board = null;
        private IAsyncOperation<ContentDialogResult> _dialogResult = null;

        private async Task<bool> ShowDialogAsync(string content)
        {
            return await ShowDialogAsync(content, "Yes", "No");
        }

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

        public void Init(ref ItemsControl dice, ref ItemsControl scores)
        {
            _board = new YatzyBoard(ShowDialogAsync);
            dice.ItemsSource = _board.Dice;
            scores.ItemsSource = _board.Items;
            _board.Clear();
        }

        public void Roll()
        {
            _board.Roll();
        }

        public async void New()
        {
            bool result = await ShowDialogAsync("Start a New Game?");
            if (result)
            {
                _board.Clear();
            }
        }
    }
}