using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private RenderWindow _renderWindow;
        private readonly CircleShape _circle;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            this._circle = new CircleShape(20) { FillColor = SFML.Graphics.Color.Magenta };
            this.CreateRenderWindow();

            var refreshRate = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            this._timer = new DispatcherTimer { Interval = refreshRate };
            this._timer.Tick += Timer_Tick;
            this._timer.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            var color = new SFML.Graphics.Color((byte)rand.Next(), (byte)rand.Next(), (byte)rand.Next());
            this._circle.FillColor = color;
        }

        private void CreateRenderWindow()
        {
            if (this._renderWindow != null)
            {
                this._renderWindow.SetActive(false);
                this._renderWindow.Dispose();
            }

            var context = new ContextSettings { DepthBits = 24 };
            this._renderWindow = new RenderWindow(DrawSurface.Handle, context);
            this._renderWindow.MouseButtonPressed += RenderWindow_MouseButtonPressed;
            this._renderWindow.SetActive(true);
        }

        private void DrawSurface_SizeChanged(object sender, EventArgs e)
        {
            this.CreateRenderWindow();
        }

        private void RenderWindow_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            this._circle.Position = new Vector2f(e.X, e.Y);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this._renderWindow.DispatchEvents();

            this._renderWindow.Clear(SFML.Graphics.Color.Black);
            this._renderWindow.Draw(this._circle);
            this._renderWindow.Display();
        }
    }
}
