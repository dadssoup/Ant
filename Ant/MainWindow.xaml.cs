using System.Security.Cryptography.Xml;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AntClass ant;
        double antX;
        double antY;
        double leafX;
        double leafY;
        double homeX;
        double homeY;
        double HomeEllipseX;
        double HomeEllipseY;
        public Rectangle leaf;
        public Ellipse Home;
        public Ellipse Leafsit;
        public Ellipse Scary;
        public Ellipse DeadEnd;
        bool leafCarried;

        public MainWindow()
        {

            InitializeComponent();
            var states = new[]
            {
                new { Name = "A1", X = 150, Y = 80 },
                new { Name = "A2", X = 150, Y = 100 },
                new { Name = "A3", X = 100, Y = 130 },
                new { Name = "A4", X = 120, Y = 130 },
                new { Name = "A5", X = 135, Y = 120 },
                new { Name = "B1", X = 245, Y = 80 },
                new { Name = "B2", X = 245, Y = 100 },
                new { Name = "B3", X = 285, Y = 130 },
                new { Name = "C1", X = 100, Y = 250 },
                new { Name = "C2", X = 120, Y = 250 },
                new { Name = "C3", X = 150, Y = 290 },
                new { Name = "C4", X = 110, Y = 330 },
                new { Name = "D1", X = 255, Y = 260 },
                new { Name = "D2", X = 285, Y = 250 },
                new { Name = "D3", X = 245, Y = 290 }

            };
            var transitions = new[]
            {
                new { From = "A1", To = "B1", Label = "Листик найден",YOffSet = -20,XOffSet = 0 },
                new { From = "B2", To = "A2", Label = "Донес листик до дома",YOffSet = 5,XOffSet = 0 },
                new { From = "A3", To = "C1", Label = "Курсор рядом",YOffSet = 10,XOffSet = -80},
                new { From = "C2", To = "A4", Label = "Курсор далеко", YOffSet = 30,XOffSet = 5},
                new { From = "A5", To = "D1", Label = "Нажат сброс", YOffSet = 0,XOffSet = 70},
                new { From = "B3", To = "D2", Label = "Нажат сброс" ,YOffSet = 0,XOffSet = 5},
                new { From = "C3", To = "D3", Label = "Нажат сброс" ,YOffSet = 5,XOffSet = 0},
                new { From = "C4", To = "C4", Label = "Курсор рядом",YOffSet = 0 ,XOffSet = 25},
            };
            var StateNames = new[]
            {
                new { Name = "Искать листик", X = 70, Y = 50 },
                new { Name = "Нести листик домой", X = 245, Y = 50 },
                new { Name = "Убегать от курсора", X = 70, Y = 250 },
                new { Name = "Конец", X = 245, Y = 250 },
            };
            Home = new Ellipse();
            Home.Stroke = Brushes.Black;
            Home.StrokeDashCap = PenLineCap.Flat;
            Home.StrokeDashOffset = 1;
            Home.Width = 80;
            Home.Height = 80;
            HomeEllipseY = 50;
            HomeEllipseX = 70;
            Home.StrokeDashArray = new(new double[] { 5 });
            Home.StrokeDashOffset = 1;
            Situation.Children.Add(Home);
            Canvas.SetTop(Home, HomeEllipseY);
            Canvas.SetLeft(Home, HomeEllipseX);
            Leafsit = new Ellipse();
            Leafsit.Stroke = Brushes.Black;
            Leafsit.Width = 80;
            Leafsit.Height = 80;
            Situation.Children.Add(Leafsit);
            Canvas.SetTop(Leafsit, HomeEllipseY);
            Canvas.SetLeft(Leafsit, HomeEllipseX + 175);
            Scary = new Ellipse();
            Scary.Stroke = Brushes.Black;
            Scary.Width = 80;
            Scary.Height = 80;
            Situation.Children.Add(Scary);
            Canvas.SetTop(Scary, HomeEllipseY + 200);
            Canvas.SetLeft(Scary, HomeEllipseX);
            DeadEnd = new Ellipse();
            DeadEnd.Stroke = Brushes.Black;
            DeadEnd.Width = 80;
            DeadEnd.Height = 80;
            Situation.Children.Add(DeadEnd);
            Canvas.SetTop(DeadEnd, HomeEllipseY + 200);
            Canvas.SetLeft(DeadEnd, HomeEllipseX + 175);

            foreach (var state in StateNames)
            {

                TextBlock textBlock = new TextBlock
                {
                    Text = state.Name,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Black
                };

                // Установка текста по центру эллипса
                textBlock.Loaded += (s, e) =>
                {
                    Canvas.SetLeft(textBlock, state.X - textBlock.ActualWidth / 2 + 40);
                    Canvas.SetTop(textBlock, state.Y - textBlock.ActualHeight / 2 + 40);
                };
                Situation.Children.Add(textBlock);
            }
            foreach (var transition in transitions)
            {
                var fromState = states.First(s => s.Name == transition.From);
                var toState = states.First(s => s.Name == transition.To);
                if (fromState == toState)
                {
                    // Рисование петли
                    DrawSelfLoop(fromState.X, fromState.Y);
                }
                else
                {
                    // Рисование линии
                    Line line = new Line
                    {
                        X1 = fromState.X,
                        Y1 = fromState.Y,
                        X2 = toState.X,
                        Y2 = toState.Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };

                    Situation.Children.Add(line);

                    // Рисование наконечника стрелки
                    DrawArrowHead(fromState.X, fromState.Y, toState.X, toState.Y);
                }

                // Добавление метки перехода
                TextBlock label = new TextBlock
                {
                    Text = transition.Label,
                    FontSize = 12,
                    Foreground = Brushes.DarkSlateGray,
                };

                double midX = (fromState.X + toState.X) / 2;
                double midY = (fromState.Y + toState.Y) / 2;
                Canvas.SetLeft(label, Math.Min(fromState.X, toState.X) + transition.XOffSet);
                Canvas.SetTop(label, midY + transition.YOffSet);
                Situation.Children.Add(label);


            }

        }
        private void DrawArrowHead(double x1, double y1, double x2, double y2)
        {
            double arrowLength = 10;
            double arrowWidth = 5;

            // Вычисляем угол линии
            double angle = Math.Atan2(y2 - y1, x2 - x1);

            // Вычисляем координаты точек треугольника
            Point p1 = new Point(x2, y2);
            Point p2 = new Point(x2 - arrowLength * Math.Cos(angle - Math.PI / 6),
                                 y2 - arrowLength * Math.Sin(angle - Math.PI / 6));
            Point p3 = new Point(x2 - arrowLength * Math.Cos(angle + Math.PI / 6),
                                 y2 - arrowLength * Math.Sin(angle + Math.PI / 6));

            // Создание треугольника для наконечника стрелки
            Polygon arrowHead = new Polygon
            {
                Points = new PointCollection(new[] { p1, p2, p3 }),
                Fill = Brushes.Black
            };

            Situation.Children.Add(arrowHead);
        }
        private void DrawSelfLoop(double x, double y)
        {
            double arrowSize = 10;
            // Создание петли
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(x + 15, y); // Начальная точка на правом крае состояния

            ArcSegment arcSegment = new ArcSegment
            {
                Point = new Point(x - 15, y),
                Size = new Size(25, 25),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = true
            };

            pathFigure.Segments.Add(arcSegment);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            Path path = new Path
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Data = pathGeometry
            };

            Situation.Children.Add(path);

            // Добавление наконечника стрелки на петлю
            Point endPoint = arcSegment.Point;
            double angle = Math.Atan2(endPoint.X - x, endPoint.Y - y) + Math.PI / 4;

            Point arrowPoint1 = new Point(
                endPoint.X - arrowSize * Math.Cos(angle - Math.PI / 6),
                endPoint.Y - arrowSize * Math.Sin(angle - Math.PI / 6)
            );

            Point arrowPoint2 = new Point(
                endPoint.X - arrowSize * Math.Cos(angle + Math.PI / 6),
                endPoint.Y - arrowSize * Math.Sin(angle + Math.PI / 6)
            );

            Polygon arrowHead = new Polygon
            {
                Points = new PointCollection(new[] { arrowPoint2, arrowPoint1, endPoint }),
                Fill = Brushes.Black
            };

            Situation.Children.Add(arrowHead);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var dxdy = GetVector();
            var pos = Mouse.GetPosition(AMap);

            if (leafCarried && dxdy.X == 0 && dxdy.Y == 0)
            {
                leafCarried = false;
                InitLeaf();
                CurrentState.Text = "Гоооооол";
            }
            else
            {
                double deltaX; double deltaY;
                if (!leafCarried && IsScary())
                {
                    deltaX = 2 * SignOf(antX, pos.X);
                    deltaY = 2 * SignOf(antY, pos.Y);
                    CurrentState.Text = "Улепётывать";
                    GetSituation(Prikol.Scary);
                }
                else
                {
                    CurrentState.Text = leafCarried ? "Нести лист домой" : "Искать лист";
                    deltaX = 1 * dxdy.X;
                    deltaY = 1 * dxdy.Y;
                    GetSituation(leafCarried? Prikol.GoLeaf : Prikol.GoHome );
                }

                antX += deltaX;
                Canvas.SetLeft(ant.Rect, antX);
                antY += deltaY;
                Canvas.SetTop(ant.Rect, antY);
                ant.Rect.RenderTransform = new RotateTransform(Math.Atan2(deltaY, deltaX) * 180 / Math.PI, ant.Rect.Width / 2, ant.Rect.Height / 2);
            }
            
        }

        public bool IsScary()
        {
            var pos = Mouse.GetPosition(AMap);
            double dx = antX - pos.X;
            double dy = antY - pos.Y;
            double len = Math.Sqrt(dx * dx + dy * dy);
            return len < 80;
        }

        public int SignOf(double a, double b)
        {
            return (a - b) < 0 ? -1 : 1;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Reset.IsEnabled = true;

            leafCarried = false;

            ant = new AntClass(new System.Drawing.Point(50, 50));

            AMap.Children.Add(ant.Rect);
            antX = 50;
            antY = 50;
            Canvas.SetTop(ant.Rect, antY);
            Canvas.SetLeft(ant.Rect, antX);

            Rectangle home = new();
            home.Width = 20;
            home.Height = 20;
            home.Fill = Brushes.Black;
            AMap.Children.Add(home);
            homeX = antX;
            homeY = antY;
            Canvas.SetTop(home, homeY);
            Canvas.SetLeft(home, homeX);

            InitLeaf();

            CompositionTarget.Rendering += Timer_Tick;
            Start.IsEnabled = false;

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            AMap.Children.Clear();
            CompositionTarget.Rendering -= Timer_Tick;
            Reset.IsEnabled = false;
            Start.IsEnabled = true;
            GetSituation(Prikol.DeadEnd);
        }

        public void InitLeaf()
        {
            Random rand = new();
            leaf = new Rectangle();
            leaf.Fill = Brushes.Green;
            leaf.Width = 30;
            leaf.Height = 30;
            AMap.Children.Add(leaf);
            rand.Next();
            leafX = rand.Next((int)AMap.ActualWidth - (int)leaf.Width);
            if (leafX > 400)
            {
                leafY = rand.Next((int)AMap.ActualHeight - (int)leaf.Height);
            }
            else
            {
                leafY = 200 + rand.Next(((int)AMap.ActualHeight - (int)leaf.Height) - 200);
            }

            Canvas.SetTop(leaf, leafY);
            Canvas.SetLeft(leaf, leafX);
        }
        public Point GetVector()
        {
            double dx = (leafCarried ? homeX : leafX) - antX;
            double dy = (leafCarried ? homeY : leafY) - antY;

            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len > 10)
            {
                dx /= len;
                dy /= len;
            }
            else
            {
                leafCarried = true;
                AMap.Children.Remove(leaf);
            }

            return new Point(dx, dy);
        }


        public void GetSituation(Prikol state)
        {
            
            foreach (var child in Situation.Children)
            {
                if(child.ToString() == "System.Windows.Shapes.Ellipse")
                {
                    ((Ellipse)child).Stroke = Brushes.Black;
                }
            }
            switch
                (state)
            {
                case Prikol.GoHome: Home.Stroke = Brushes.Red; break;
                case Prikol.GoLeaf: Leafsit.Stroke = Brushes.Red; break;
                case Prikol.Scary: Scary.Stroke = Brushes.Red; break;
                case Prikol.DeadEnd: DeadEnd.Stroke = Brushes.Red; break;
                default: break;
            }


        }

    }
    public enum Prikol
    {
        GoHome, GoLeaf, Scary, DeadEnd
    }

}