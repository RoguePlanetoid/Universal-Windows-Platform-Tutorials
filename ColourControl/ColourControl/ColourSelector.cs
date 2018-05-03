using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ColourControl
{
    public class ColourSelector : ComboBox
    {
        public class Colour
        {
            public string Name { get; set; }
            public Color Value { get; set; }
        }

        private static IEnumerable<string> SplitCapital(string text)
        {
            Regex regex = new Regex(@"\p{Lu}\p{Ll}*");
            foreach (Match match in regex.Matches(text))
            {
                yield return match.Value;
            }
        }

        private List<Colour> _colours = typeof(Colors)
        .GetRuntimeProperties()
        .Select(c => new Colour
        {
            Value = (Color)c.GetValue(null),
            Name = string.Join(" ", SplitCapital(c.Name))
        }).ToList();

        public ColourSelector()
        {
            ItemTemplate = (DataTemplate)XamlReader.Load(
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            "<StackPanel Orientation=\"Horizontal\">" +
            "<Rectangle Width=\"20\" Height=\"15\" Margin=\"5,0\"><Rectangle.Fill>" +
            "<SolidColorBrush Color=\"{Binding Value}\"/></Rectangle.Fill></Rectangle>" +
            "<TextBlock VerticalAlignment=\"Center\" Text=\"{Binding Name}\"/>" +
            "</StackPanel></DataTemplate>");
            SelectedValuePath = "Value";
            ItemsSource = _colours;
        }

        public Color Selected
        {
            get { return ((Colour)SelectedItem).Value; }
            set { SelectedItem = (_colours.Single(w => w.Value == value)); }
        }
    }
}