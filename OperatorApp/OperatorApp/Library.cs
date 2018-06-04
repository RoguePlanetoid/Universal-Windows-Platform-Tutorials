using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

public class CommandHandler : ICommand
{
    public event EventHandler CanExecuteChanged = null;
    private Action _action;

    public CommandHandler(Action action)
    {
        _action = action;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        _action();
    }
}

public class Item
{
    public enum ItemMode
    {
        Operand = 1,
        Operator = 2
    }

    public ItemMode Mode { get; set; }
    public Func<Item, int> Action { get; set; }
    public ICommand Command { get { return new CommandHandler(() => this.Action(this)); } }
    public object Value { get; set; }
}

public class Library
{
    private static double first_operand;
    private static double second_operand;
    private static string operation = null;

    private static TextBlock _output;

    private static Func<Item, int> action = (Item item) =>
    {
        switch (item.Mode)
        {
            case Item.ItemMode.Operand:
                _output.Text += (int)item.Value;
                break;
            case Item.ItemMode.Operator:
                string selected = (string)item.Value;
                if (selected == "=")
                {
                    double result = 0.0;
                    if (_output.Text.Length > 0)
                    {
                        second_operand = double.Parse(_output.Text);
                        switch (operation)
                        {
                            case "/":
                                if (second_operand > 0)
                                {
                                    result = first_operand / second_operand;
                                }
                                break;
                            case "*":
                                result = first_operand * second_operand;
                                break;
                            case "-":
                                result = first_operand - second_operand;
                                break;
                            case "+":
                                result = first_operand + second_operand;
                                break;
                        }
                        _output.Text = result.ToString();
                    }
                }
                else if (selected == "<")
                {
                    if (_output.Text.Length > 0)
                    {
                        _output.Text = _output.Text.Substring(0, _output.Text.Length - 1);
                    }
                }
                else
                {
                    if (_output.Text.Length > 0)
                    {
                        first_operand = int.Parse(_output.Text);
                        _output.Text = string.Empty;
                        operation = item.Value.ToString();
                    }
                }
                break;
        }
        return 0;
    };

    private List<Item> list = new List<Item>
    {
        new Item { Mode = Item.ItemMode.Operand, Value = 7, Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 8, Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 9, Action = action },
        new Item { Mode = Item.ItemMode.Operator, Value = "/", Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 4, Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 5, Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 6, Action = action },
        new Item { Mode = Item.ItemMode.Operator, Value = "*", Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 1, Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 2, Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 3, Action = action },
        new Item { Mode = Item.ItemMode.Operator, Value = "-", Action = action },
        new Item { Mode = Item.ItemMode.Operator, Value = "<", Action = action },
        new Item { Mode = Item.ItemMode.Operand, Value = 0, Action = action },
        new Item { Mode = Item.ItemMode.Operator, Value = "=", Action = action },
        new Item { Mode = Item.ItemMode.Operator, Value = "+", Action = action },
    };

    public List<Item> New(ref TextBlock text)
    {
        _output = text;
        return list;
    }
}