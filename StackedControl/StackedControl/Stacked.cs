using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace StackedControl
{
    public class Stacked : Grid
    {
        private List<double> _items = new List<double>();

        private List<double> Percentages()
        {
            List<double> results = new List<double>();
            double total = _items.Sum();
            foreach (double item in _items)
            {
                results.Add((item / total) * 100);
            }
            return results.OrderBy(o => o).ToList();
        }

        private Rectangle GetRectangle(Color colour, int column)
        {
            Rectangle rect = new Rectangle()
            {
                Fill = new SolidColorBrush(colour)
            };
            rect.SetValue(Grid.ColumnProperty, column);
            return rect;
        }

        private void Layout()
        {
            List<double> percentages = Percentages();
            this.ColumnDefinitions.Clear();
            for (int index = 0; index < percentages.Count(); index++)
            {
                double percentage = percentages[index];
                ColumnDefinition column = new ColumnDefinition()
                {
                    Width = new GridLength(percentage, GridUnitType.Star)
                };
                this.ColumnDefinitions.Add(column);
                Color colour = (index < Palette.Count()) ? Palette[index] : Colors.Black;
                this.Children.Add(GetRectangle(colour, index));
            }
        }

        public List<Color> Palette { get; set; } = new List<Color>();

        public List<double> Items
        {
            get { return _items; }
            set { _items = value; Layout(); }
        }

        public void Fibonacci(params Color[] colours)
        {
            Palette = colours.ToList();
            int fibonacci(int value) => value > 1 ?
            fibonacci(value - 1) + fibonacci(value - 2) : value;
            Items = Enumerable.Range(0, Palette.Count())
            .Select(fibonacci).Select(s => (double)s).ToList();
        }
    }
}