using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace RulerControl
{
    public class Ruler : Canvas
    {
        private const double height = 40.0;

        public enum Units { Cm, Inch };

        public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register("Foreground", typeof(Brush),
        typeof(Ruler), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty LengthProperty =
        DependencyProperty.Register("Length", typeof(double),
        typeof(Ruler), new PropertyMetadata(10.0));

        public static readonly DependencyProperty SegmentProperty =
        DependencyProperty.Register("Segment", typeof(double),
        typeof(Ruler), new PropertyMetadata(20.0));

        public static readonly DependencyProperty UnitProperty =
        DependencyProperty.Register("Unit", typeof(double),
        typeof(Ruler), new PropertyMetadata(Units.Cm));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public double Segment
        {
            get { return (double)GetValue(SegmentProperty); }
            set { SetValue(SegmentProperty, value); }
        }

        public Units Unit
        {
            get { return (Units)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        private double CmToDip(double cm) => (cm * 96.0 / 2.54);

        private double InchToDip(double inch) => (inch * 96.0);

        private Path GetLine(Brush stroke, double thickness, Point start, Point finish)
        {
            return new Path()
            {
                Stroke = stroke,
                StrokeThickness = thickness,
                Data = new LineGeometry() { StartPoint = start, EndPoint = finish }
            };
        }

        private void Layout()
        {
            this.Children.Clear();
            for (double value = 0.0; value <= Length; value++)
            {
                double dip;
                if (Unit == Units.Cm)
                {
                    dip = CmToDip(value);
                    if (value < Length)
                    {
                        for (int i = 1; i <= 9; i++)
                        {
                            if (i != 5)
                            {
                                double mm = CmToDip(value + 0.1 * i);
                                this.Children.Add(GetLine(Foreground, 0.5, new Point(mm, this.Height),
                                new Point(mm, this.Height - Segment / 3.0)));
                            }
                        }
                        double middle = CmToDip(value + 0.5);
                        this.Children.Add(GetLine(Foreground, 1.0, new Point(middle, this.Height),
                        new Point(middle, this.Height - Segment * 2.0 / 3.0)));
                    }
                }
                else
                {
                    dip = InchToDip(value);
                    if (value < Length)
                    {
                        double quarter = InchToDip(value + 0.25);
                        this.Children.Add(GetLine(Foreground, 0.5, new Point(quarter, this.Height),
                        new Point(quarter, this.Height - Segment / 3.0)));
                        double middle = InchToDip(value + 0.5);
                        this.Children.Add(GetLine(Foreground, 1.0, new Point(middle, this.Height),
                        new Point(middle, this.Height - 0.5 * Segment * 2.0 / 3.0)));
                        double division = InchToDip(value + 0.75);
                        this.Children.Add(GetLine(Foreground, 0.5, new Point(division, this.Height),
                        new Point(division, this.Height - 0.25 * Segment / 3.0)));
                    }
                }
                this.Children.Add(GetLine(Foreground, 1.0, new Point(dip, this.Height),
                new Point(dip, this.Height - Segment)));
            }
        }

        public Ruler()
        {
            this.Loaded += (object sender, RoutedEventArgs e) => Layout();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.Height = double.IsNaN(this.Height) ? height : this.Height;
            Size desiredSize = (Unit == Units.Cm) ? new Size(CmToDip(Length),
            this.Height) : new Size(InchToDip(Length), this.Height);
            return desiredSize;
        }
    }
}