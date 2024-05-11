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
        public Rectangle leaf;
        bool leafCarried;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var dxdy = GetVector();
            var pos = Mouse.GetPosition(AMap);
            
            if(leafCarried && dxdy.X == 0 &&  dxdy.Y == 0 )
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
                }
                else
                {
                    CurrentState.Text = leafCarried ? "Нести лист домой" : "Искать лист";
                    deltaX = 1 * dxdy.X;
                    deltaY = 1 * dxdy.Y;
                }

                antX += deltaX;
                Canvas.SetLeft(ant.Rect, antX);
                antY += deltaY;
                Canvas.SetTop(ant.Rect, antY);
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
            return (a - b) < 0? -1 : 1;
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
            
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            AMap.Children.Clear();
            CompositionTarget.Rendering -= Timer_Tick;
            Reset.IsEnabled = false;
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
            double dx = (leafCarried? homeX : leafX) - antX;
            double dy = (leafCarried? homeY : leafY) - antY;

            double len = Math.Sqrt(dx * dx + dy * dy);
            if(len > 10) 
            {
                dx /= len;
                dy /= len;
            }
            else
            {
                leafCarried = true;
                AMap.Children.Remove(leaf);
            }

            return new Point(dx,dy);
        }
    }
}