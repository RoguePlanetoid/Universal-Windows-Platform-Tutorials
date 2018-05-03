using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

public static class Library
{
    private static IEnumerable<string> SplitCapital(string text)
    {
        Regex regex = new Regex(@"\p{Lu}\p{Ll}*");
        foreach (Match match in regex.Matches(text))
        {
            yield return match.Value;
        }
    }

    private static Dictionary<string, Color> _colours = typeof(Colors)
    .GetRuntimeProperties()
    .Select(c => new
    {
        Color = (Color)c.GetValue(null),
        Name = string.Join(" ", SplitCapital(c.Name))
    }).ToDictionary(x => x.Name, x => x.Color);

    public static string Command { get; set; }

    public static async void Parse(TextBlock title, Rectangle display)
    {
        try
        {
            StorageFile file = await Package.Current.InstalledLocation.GetFileAsync(@"VCD.xml");
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
        }
        catch
        {

        }
        if (!string.IsNullOrEmpty(Command))
        {
            string titleCase = Regex.Replace(Command.ToLower(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            if (_colours.Any(a => a.Key == titleCase))
            {
                KeyValuePair<string, Color> value = _colours.Where(w => w.Key == titleCase).FirstOrDefault();
                title.Text = value.Key;
                display.Fill = new SolidColorBrush(value.Value);
            }
        }
    }
}