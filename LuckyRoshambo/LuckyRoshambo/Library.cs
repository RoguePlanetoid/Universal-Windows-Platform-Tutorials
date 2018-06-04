using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

public class Library
{
    private const int size = 3;
    private const int win = 1;
    private const int draw = 0;
    private const int lose = -1;
    private int[,] match = new int[size, size]
    {
        { draw, lose, win },
        { win, draw, lose },
        { lose, win, draw }
    };
    private readonly int[] values = new int[] { 0, 1, 2 };
    private readonly string[] options = new string[] { "\uED5B", "\uE130", "\uE16B" };
    private readonly Color[] colours = new Color[] { Colors.DarkRed, Colors.DarkBlue, Colors.DarkGreen };

    private Random random = new Random((int)DateTime.Now.Ticks);

    private async Task<ContentDialogResult> ShowDialogAsync(string title, int option)
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = title,
            Content = GetShape(option, false),
            PrimaryButtonText = "OK"
        };
        return await dialog.ShowAsync();
    }

    private async void Choose(int option)
    {
        int player = values[option];
        int computer = random.Next(0, size - 1);
        int result = match[player, computer];
        string message = string.Empty;
        switch (result)
        {
            case win:
                message = "You Win!";
                break;
            case lose:
                message = "You Lost";
                break;
            case draw:
                message = "You Draw";
                break;
        }
        await ShowDialogAsync($"Computer Picked - {message}", computer);
    }

    private Grid GetShape(int option, bool useEvent)
    {
        Grid grid = new Grid()
        {
            Tag = option,
            Margin = new Thickness(5),
            Height = 80,
            Width = 80,
            Background = new SolidColorBrush(colours[option]),
        };
        TextBlock text = new TextBlock()
        {
            Text = options[option],
            FontSize = 66,
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            Foreground = new SolidColorBrush(Colors.White),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        if (useEvent)
        {
            grid.Tapped += (object sender, TappedRoutedEventArgs e) =>
            {
                Grid selected = (Grid)sender;
                int tag = (int)selected.Tag;
                Choose(tag);
            };
        }
        grid.Children.Add(text);
        return grid;
    }

    private void Layout(ref Grid grid)
    {
        grid.Children.Clear();
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        for (int i = 0; i < size; i++)
        {
            panel.Children.Add(GetShape(i, true));
        }
        grid.Children.Add(panel);
    }

    public void New(ref Grid grid)
    {
        Layout(ref grid);
    }
}