using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace FontControl
{
    public class FontSelector : ComboBox
    {
        public FontSelector()
        {
            ItemTemplate = (DataTemplate)XamlReader.Load(
            "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
            "<TextBlock Text=\"{Binding}\" FontFamily=\"{Binding}\"/></DataTemplate>");
            ItemsSource = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();
        }

        public FontFamily Selected
        {
            get { return new FontFamily((string)SelectedItem); }
            set { SelectedValue = value.Source; }
        }
    }
}