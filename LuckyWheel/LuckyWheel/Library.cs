using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace LuckyWheel
{
    public class WheelWedge
    {
        public string Value { get; set; }
        public Color Colour { get; set; }
        public double Sector { get; set; }

        public WheelWedge(string value, Color colour)
        {
            Value = value;
            Colour = colour;
        }
    }

    public class WheelBoard
    {
        private const double axis = 180.0;
        private const double circle = 360;
        private const double radius = 200;
        private const double diameter = 400;
        private readonly List<WheelWedge> wedges = new List<WheelWedge>()
        {
            new WheelWedge("1000", Colors.WhiteSmoke),
            new WheelWedge("600", Colors.LightGreen),
            new WheelWedge("500", Colors.Yellow),
            new WheelWedge("300", Colors.Red),
            new WheelWedge("500", Colors.Azure),
            new WheelWedge("800", Colors.Orange),
            new WheelWedge("550", Colors.Violet),
            new WheelWedge("400", Colors.Yellow),
            new WheelWedge("300", Colors.Pink),
            new WheelWedge("900", Colors.Red),
            new WheelWedge("500", Colors.Azure),
            new WheelWedge("300", Colors.LightGreen),
            new WheelWedge("900", Colors.Pink),
            new WheelWedge("LOSE", Colors.Black),
            new WheelWedge("600", Colors.Violet),
            new WheelWedge("400", Colors.Yellow),
            new WheelWedge("300", Colors.Azure),
            new WheelWedge("LOSE", Colors.Black),
            new WheelWedge("800", Colors.Red),
            new WheelWedge("350", Colors.Violet),
            new WheelWedge("450", Colors.Pink),
            new WheelWedge("700", Colors.LightGreen),
            new WheelWedge("300", Colors.Orange),
            new WheelWedge("600", Colors.Violet)
        };
        private readonly Random random = new Random((int)DateTime.Now.Ticks);

        private Storyboard _storyboard;
        private double _selected;
        private double _sector;
        private double _position;
        private Canvas _wheel;
        private int _total;
        private bool _lost;
        private bool _spin;

        public delegate void DialogHandler(string content);
        public event DialogHandler Dialog;

        private int Choose()
        {
            return random.Next(1, (int)circle);
        }

        private Path GetWedgePath(Color fill, ref double angle, double sector)
        {
            double DegreeToRadian(double degree)
            {
                return Math.PI * degree / axis;
            }
            Path path = new Path()
            {
                Fill = new SolidColorBrush(fill),
            };
            PathGeometry geometry = new PathGeometry();
            double x = Math.Cos(angle) * radius + radius;
            double y = Math.Sin(angle) * radius + radius;
            LineSegment lineOne = new LineSegment()
            {
                Point = new Point(x, y)
            };
            angle += DegreeToRadian(sector);
            x = Math.Cos(angle) * radius + radius;
            y = Math.Sin(angle) * radius + radius;
            ArcSegment arc = new ArcSegment()
            {
                RotationAngle = sector,
                Point = new Point(x, y),
                IsLargeArc = sector >= axis,
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise,
            };
            LineSegment lineTwo = new LineSegment()
            {
                Point = new Point(radius, radius)
            };
            PathFigure figure = new PathFigure()
            {
                StartPoint = new Point(radius, radius)
            };
            figure.Segments.Add(lineOne);
            figure.Segments.Add(arc);
            figure.Segments.Add(lineTwo);
            geometry.Figures.Add(figure);
            path.Data = geometry;
            return path;
        }

        private Grid GetWedgeLabel(Color foreground, string value,
        double x, double y, ref double rotate, double sector)
        {
            Grid grid = new Grid()
            {
                Height = radius
            };
            TextBlock textblock = new TextBlock()
            {
                Text = value,
                FontSize = 20,
                Margin = new Thickness(2),
                FontWeight = FontWeights.SemiBold,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(foreground),
            };
            rotate += sector;
            textblock.Inlines.Clear();
            for (int i = 0; i < value.Length; i++)
            {
                textblock.Inlines.Add(new Run() { Text = value[i] + Environment.NewLine });
            }
            textblock.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double width = textblock.DesiredSize.Width;
            double left = y - (width / 2);
            grid.RenderTransform = new RotateTransform()
            {
                Angle = rotate,
                CenterY = radius,
                CenterX = width / 2
            };
            grid.Children.Add(textblock);
            Canvas.SetZIndex(grid, 99);
            Canvas.SetLeft(grid, left);
            Canvas.SetTop(grid, x);
            return grid;
        }

        private Ellipse GetCircle(Color fill, Color stroke,
            double x, double y, double dimension)
        {
            Ellipse ellipse = new Ellipse()
            {
                Width = dimension,
                Height = dimension,
                StrokeThickness = 5,
                Fill = new SolidColorBrush(fill),
                Stroke = new SolidColorBrush(stroke)
            };
            Canvas.SetLeft(ellipse, x - (dimension / 2));
            Canvas.SetTop(ellipse, y - (dimension / 2));
            return ellipse;
        }

        private Polygon GetMarker(Color fill, Color stroke, double dimension)
        {
            Polygon polygon = new Polygon
            {
                Width = dimension,
                Height = dimension,
                StrokeThickness = 5,
                Stretch = Stretch.Uniform,
                Fill = new SolidColorBrush(fill),
                StrokeLineJoin = PenLineJoin.Round,
                Stroke = new SolidColorBrush(stroke),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Points = { new Point(0, 0), new Point(100, 0), new Point(50, 50) }
            };
            return polygon;
        }

        private WheelWedge GetWedge(double selected)
        {
            double value = circle - selected;
            return wedges.First(w => value >= w.Sector && value < (w.Sector + _sector));
        }

        private void Rotation_Completed(object sender, object e)
        {
            _spin = false;
            WheelWedge wedge = GetWedge(_selected);
            if (int.TryParse(wedge.Value, out int result) && !_lost)
            {
                Dialog?.Invoke($"You Won {result}, Total is {_total}");
                _total += result;
            }
            else
            {
                Dialog?.Invoke($"You Lose, Total was {_total}!");
                _lost = true;
            }
        }

        private void Rotation(ref Canvas wheel, double angle)
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = _position,
                To = (circle * 2) + angle,
                EasingFunction = new QuadraticEase(),
                RepeatBehavior = new RepeatBehavior(1),
                Duration = new Duration(TimeSpan.FromSeconds(5)),
            };
            Storyboard.SetTargetProperty(animation, "(Canvas.RenderTransform).(RotateTransform.Angle)");
            Storyboard.SetTarget(animation, wheel);
            _storyboard = new Storyboard();
            _storyboard.Completed += Rotation_Completed;
            _storyboard.Children.Add(animation);
            _storyboard.Begin();
        }

        private void Reset()
        {
            _total = 0;
            _spin = false;
            _lost = false;
            _selected = 0;
        }

        private void Wheel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (!_spin)
            {
                _spin = true;
                if (_lost)
                {
                    Dialog?.Invoke($"You Lost, Total was {_total}, Starting New Game");
                    Reset();
                }
                if (!_lost)
                {
                    _position = _selected;
                    _selected = Choose();
                    Rotation(ref _wheel, _selected);
                }
            }
        }

        private Canvas GetWheel()
        {
            double current = 0;
            double total = wedges.Count;
            double angle = -(Math.PI / 2);
            _sector = (360 / total);
            _position = -(_sector / 2);
            double rotate = -(_sector / 2);
            Canvas canvas = new Canvas()
            {
                Width = diameter,
                Height = diameter,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                RenderTransform = new RotateTransform()
                {
                    Angle = rotate,
                    CenterX = radius,
                    CenterY = radius
                }
            };
            canvas.Children.Add(GetCircle(Colors.Transparent, Colors.Gold, radius, radius, diameter + 10));
            foreach (WheelWedge wedge in wedges)
            {
                wedge.Sector = current;
                Path path = GetWedgePath(wedge.Colour, ref angle, _sector);
                Grid label = GetWedgeLabel(wedge.Colour == Colors.Black ? Colors.White : Colors.Black,
                wedge.Value, 0, radius, ref rotate, _sector);
                canvas.Children.Add(path);
                canvas.Children.Add(label);
                current += _sector;
            }
            canvas.Children.Add(GetCircle(Colors.Green, Colors.Gold, radius, radius, 120));
            canvas.Tapped += Wheel_Tapped;
            return canvas;
        }

        public void Layout(ref Grid grid)
        {
            grid.Children.Clear();
            _wheel = GetWheel();
            StackPanel panel = new StackPanel();
            panel.Children.Add(GetMarker(Colors.WhiteSmoke, Colors.Black, 30));
            panel.Children.Add(_wheel);
            grid.Children.Add(panel);
        }

        public void New()
        {
            Reset();
        }
    }

    public class Library
    {
        private const string app_title = "Lucky Wheel";

        private IAsyncOperation<IUICommand> _dialogCommand = null;
        private WheelBoard _board = new WheelBoard();

        private async Task<bool> ShowDialogAsync(string content, string title = app_title)
        {
            try
            {
                if (_dialogCommand != null)
                {
                    _dialogCommand.Cancel();
                    _dialogCommand = null;
                }
                _dialogCommand = new MessageDialog(content, title).ShowAsync();
                await _dialogCommand;
                return true;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        private async void Board_Dialog(string content)
        {
            await ShowDialogAsync(content);
        }

        public void Init(ref Grid grid)
        {
            _board.Layout(ref grid);
            _board.Dialog += Board_Dialog;
        }

        public void New(ref Grid grid)
        {
            _board.Layout(ref grid);
            _board.New();
        }
    }
}