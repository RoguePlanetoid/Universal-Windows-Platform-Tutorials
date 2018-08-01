using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace DonutControl
{
    public class Donut : Grid
    {
        private const double circle = 360;

        private List<double> _items = new List<double>();
        private double _angle = 0;

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

        public Point Compute(double angle, double hole)
        {
            double radians = (Math.PI / 180) * (angle - 90);
            double x = hole * Math.Cos(radians);
            double y = hole * Math.Sin(radians);
            return new Point(x += Radius, y += Radius);
        }

        private Path GetSector(Color fill, double sweep, double hole)
        {
            bool isLargeArc = sweep > 180.0;
            Point start = new Point(Radius, Radius);
            Point innerArcStart = Compute(_angle, hole);
            Point innerArcEnd = Compute(_angle + sweep, hole);
            Point outerArcStart = Compute(_angle, Radius);
            Point outerArcEnd = Compute(_angle + sweep, Radius);
            Size outerArcSize = new Size(Radius, Radius);
            Size innerArcSize = new Size(hole, hole);

            LineSegment lineOne = new LineSegment()
            {
                Point = outerArcStart
            };
            ArcSegment arcOne = new ArcSegment()
            {
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = isLargeArc,
                Point = outerArcEnd,
                Size = outerArcSize
            };
            LineSegment lineTwo = new LineSegment()
            {
                Point = innerArcEnd
            };
            ArcSegment arcTwo = new ArcSegment()
            {
                SweepDirection = SweepDirection.Counterclockwise,
                IsLargeArc = isLargeArc,
                Point = innerArcStart,
                Size = innerArcSize
            };
            PathFigure figure = new PathFigure()
            {
                StartPoint = innerArcStart,
                IsClosed = true,
                IsFilled = true
            };
            figure.Segments.Add(lineOne);
            figure.Segments.Add(arcOne);
            figure.Segments.Add(lineTwo);
            figure.Segments.Add(arcTwo);
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(figure);
            Path path = new Path
            {
                Fill = new SolidColorBrush(fill),
                Data = pathGeometry
            };
            _angle += sweep;
            return path;
        }

        private void Layout()
        {
            _angle = 0;
            double total = 100;
            double sweep = 0;
            double value = (circle / total);
            List<double> percentages = Percentages();
            Canvas canvas = new Canvas()
            {
                Width = Radius * 2,
                Height = Radius * 2
            };
            this.Children.Clear();
            for (int index = 0; index < percentages.Count(); index++)
            {
                double percentage = percentages[index];
                Color colour = (index < Palette.Count()) ? Palette[index] : Colors.Black;
                sweep = value * percentage;
                Path sector = GetSector(colour, sweep, Hole);
                canvas.Children.Add(sector);
            }
            Viewbox viewbox = new Viewbox()
            {
                Child = canvas
            };
            this.Children.Add(viewbox);
        }

        public List<Color> Palette { get; set; } = new List<Color>();

        public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register("Radius", typeof(int),
        typeof(Donut), new PropertyMetadata(100));

        public static readonly DependencyProperty HoleProperty =
        DependencyProperty.Register("Hole", typeof(UIElement),
        typeof(Donut), new PropertyMetadata(50.0));

        public int Radius
        {
            get { return (int)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); Layout(); }
        }

        public double Hole
        {
            get { return (double)GetValue(HoleProperty); }
            set { SetValue(HoleProperty, value); Layout(); }
        }

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
