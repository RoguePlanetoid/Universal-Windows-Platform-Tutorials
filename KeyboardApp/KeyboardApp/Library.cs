using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

public enum Modes
{
    Character, Backspace, Tab, Enter, Shift, Space
}

public class Item
{
    public Modes Mode { get; set; }
    public string Content { get; set; }
    public string Normal { get; set; }
    public string Shift { get; set; }
    public string Value { get; set; }
}

public class Library
{
    private readonly string[][][] keys =
    {
        new string[][]
        {
            new string[] { "`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" },
            new string[] { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]" },
            new string[] { "a", "s", "d", "f", "g", "h", "j", "k", "l", ";", "'", "#" },
            new string[] { "\\", "z", "x", "c", "v", "b", "n", "m", ",", ".", "/" }
        },
        new string[][]
        {
            new string[] { "¬", "!", "\"", "£", "$", "%", "^", "&", "*", "(", ")", "_", "+" },
            new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "{", "}" },
            new string[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", ":", "@", "~" },
            new string[] { "|", "Z", "X", "C", "V", "B", "N", "M", "<", ">", "?" }
        },
    };

    private enum Chords { normal, shift };
    private enum Rows { top, upper, middle, lower, bottom };

    private Chords _chord = Chords.normal;

    public delegate void PressedEvent(Item item);
    public event PressedEvent Pressed;

    public static IEnumerable<T> GetChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }
                foreach (T childOfChild in GetChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }

    private Button Add(Item item, int width, int height)
    {
        Button button = new Button()
        {
            Height = height,
            Width = width,
            FontSize = 10,
            Padding = new Thickness(0),
            Margin = new Thickness(0.5),
            Content = item.Content,
            Tag = item,
        };
        button.Click += (object sender, RoutedEventArgs e) =>
        {
            button = (Button)sender;
            Pressed?.Invoke((Item)button.Tag);
        };
        return button;
    }

    private void Row(StackPanel panel, string[] normal, string[] shift)
    {
        for (int i = 0; i < normal.Length; i++)
        {
            Item item = new Item()
            {
                Mode = Modes.Character,
                Normal = normal[i],
                Shift = shift[i],
                Value = (_chord == Chords.shift) ? shift[i] : normal[i],
                Content = (_chord == Chords.shift) ? shift[i] : normal[i],
            };
            panel.Children.Add(Add(item, 24, 24));
        }
    }

    private void Layout(ref StackPanel panel)
    {
        // Top
        StackPanel top = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            BorderThickness = new Thickness(1, 0, 1, 0)
        };
        Row(top, keys[(int)Chords.normal][(int)Rows.top],
            keys[(int)Chords.shift][(int)Rows.top]);
        top.Children.Add(Add(new Item()
        {
            Mode = Modes.Backspace,
            Content = "BkSp",
        }, 42, 24));
        panel.Children.Add(top);
        // Upper
        StackPanel upper = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            BorderThickness = new Thickness(1, 0, 1, 0)
        };
        upper.Children.Add(Add(new Item()
        {
            Mode = Modes.Tab,
            Content = "↹",
        }, 34, 24));
        Row(upper, keys[(int)Chords.normal][(int)Rows.upper],
            keys[(int)Chords.shift][(int)Rows.upper]);
        upper.Children.Add(Add(new Item()
        {
            Mode = Modes.Enter,
            Content = "↵",
        }, 32, 24));
        panel.Children.Add(upper);
        // Middle
        StackPanel middle = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            BorderThickness = new Thickness(1, 0, 1, 0)
        };
        middle.Children.Add(new Canvas()
        {
            Width = 42,
            Height = 24
        });
        Row(middle, keys[(int)Chords.normal][(int)Rows.middle],
                    keys[(int)Chords.shift][(int)Rows.middle]);
        panel.Children.Add(middle);
        // Lower
        StackPanel lower = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            BorderThickness = new Thickness(1, 0, 1, 0)
        };
        lower.Children.Add(new Canvas()
        {
            Width = 50,
            Height = 24
        });
        Row(lower, keys[(int)Chords.normal][(int)Rows.lower],
                    keys[(int)Chords.shift][(int)Rows.lower]);
        panel.Children.Add(lower);
        // Bottom
        StackPanel bottom = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            BorderThickness = new Thickness(1, 0, 1, 0)
        };
        bottom.Children.Add(new Canvas()
        {
            Width = 60,
            Height = 24
        });
        bottom.Children.Add(Add(new Item()
        {
            Mode = Modes.Shift,
            Content = "Shift",
        }, 40, 24));
        bottom.Children.Add(Add(new Item()
        {
            Mode = Modes.Space,
            Content = "Space",
        }, 170, 24));
        bottom.Children.Add(Add(new Item()
        {
            Mode = Modes.Shift,
            Content = "Shift",
        }, 40, 24));
        panel.Children.Add(bottom);
    }

    private void Update(StackPanel input)
    {
        IEnumerable<Button> buttons = GetChildren<Button>(input);
        foreach (Button button in buttons)
        {
            if (button.Tag != null &&
                button.Tag.GetType() == typeof(Item))
            {
                Item item = (Item)button.Tag;
                if (item.Mode == Modes.Character)
                {
                    item.Value = (_chord == Chords.shift) ? item.Shift : item.Normal;
                    item.Content = item.Value;
                    button.Content = item.Content;
                    button.Tag = item;
                }
            }
        }
    }

    public void Init(ref StackPanel panel)
    {
        Layout(ref panel);
    }

    public void Output(TextBox display, StackPanel input, Item item)
    {
        string value = string.Empty;
        switch (item.Mode)
        {
            case Modes.Backspace:
                int start = display.SelectionStart == 0 ?
                    display.Text.Length + 1 : display.SelectionStart;
                display.Select(start - 1, 1);
                display.SelectedText = string.Empty;
                break;
            case Modes.Character:
                value = (_chord == Chords.shift) ? item.Shift : item.Normal;
                break;
            case Modes.Enter:
                value = "\n";
                break;
            case Modes.Shift:
                _chord = (_chord == Chords.shift) ? Chords.normal : Chords.shift;
                Update(input);
                break;
            case Modes.Space:
                value = " ";
                break;
            case Modes.Tab:
                value = "\t";
                break;
        }
        display.Text += value;
    }
}