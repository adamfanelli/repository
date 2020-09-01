using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
using TimelineLib;

using Color = SFML.Graphics.Color;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = SFML.Window.KeyEventArgs;
using Window = SFML.Window.Window;

namespace Timeline_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private RenderWindow _renderWindow;
        private TimelineModel model;
        private System.Timers.Timer _timer;
        Context context;

        public bool KeyPressed_W;
        public bool KeyPressed_A;
        public bool KeyPressed_S;
        public bool KeyPressed_D;

        public int c = 8;
        public int tCount = 0;

        public const float FPS = 60.0f;

        public MainWindow()
        {
            InitializeComponent();

            this.CreateRenderWindow();

            model = new TimelineModel()
            {
                OffsetX = this._renderWindow.Size.X / 2,
                BackgroundColor = new Color(255, 253, 244),
                PanSpeed = 10f
            };

            model.AddEvent(new TimelineEvent("Test event", 2));
            model.AddEvent(new TimelineEvent("Test event 2", -5.25f));
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
            this._renderWindow.SetActive(true);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            var color = new Color((byte)rand.Next(), (byte)rand.Next(), (byte)rand.Next());
            model.BackgroundColor = color;
        }

        private void OnClose(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        private void DrawSurface_SizeChanged(object sender, EventArgs e)
        {
            this.CreateRenderWindow();
        }

        private void RenderWindow_MouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            
        }

        private void myKeyHandler(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.W:
                    if (!KeyPressed_W) KeyPressed_W = true;
                    break;

                case Keyboard.Key.A:
                    if (!KeyPressed_A) KeyPressed_A = true;
                    break;

                case Keyboard.Key.S:
                    if (!KeyPressed_S) KeyPressed_S = true;
                    break;

                case Keyboard.Key.D:
                    if (!KeyPressed_D) KeyPressed_D = true;
                    break;
            }
        }
        private void myKeyReleasedHandler(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.W:
                    KeyPressed_W = false;
                    break;

                case Keyboard.Key.A:
                    KeyPressed_A = false;
                    break;

                case Keyboard.Key.S:
                    KeyPressed_S = false;
                    break;

                case Keyboard.Key.D:
                    KeyPressed_D = false;
                    break;
            }
        }

        private void Button_Click_New_Event(object sender, RoutedEventArgs e)
        {
            model.AddEvent(new TimelineEvent("New Event", c));
            c += 4;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //using(_timer = new System.Timers.Timer(1000 / FPS))
            //{
            //    context = new Context();
            //    //_timer.Elapsed += DrawScreen; //OnTimedEvent;
            //    _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);                      //async (_sender, _e) => await OnTimedEvent(this, new ElapsedEventArgs());
            //    _timer.AutoReset = true;
            //    _timer.Enabled = true;
            //}

            this._renderWindow.MouseButtonPressed += RenderWindow_MouseButtonPressed;
            this._renderWindow.KeyPressed += new EventHandler<KeyEventArgs>(myKeyHandler);
            this._renderWindow.KeyReleased += new EventHandler<KeyEventArgs>(myKeyReleasedHandler);
            this._renderWindow.Closed += OnClose;
            this._renderWindow.Resized += DrawSurface_SizeChanged;

            _renderWindow.SetFramerateLimit(30);

            while(_renderWindow.IsOpen)
            {
                UpdateWindow();
            }
        }

        //public void timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    //DrawScreen();

        //    tCount++;
        //    if(tCount % 60 == 0) Debug.Print(tCount.ToString());
        //}


        private void DrawScreen()
        {
            //PAN SCREEN
            if (KeyPressed_W) model.OffsetY += model.PanSpeed;
            if (KeyPressed_A) model.OffsetX += model.PanSpeed;
            if (KeyPressed_S) model.OffsetY -= model.PanSpeed;
            if (KeyPressed_D) model.OffsetX -= model.PanSpeed;

            //DRAW LINE
            model.DrawLine(this._renderWindow);

            //DRAW MARKERS
            model.DrawMarkers(this._renderWindow);

            //DRAW EVENTS
            model.DrawEvents(this._renderWindow);

            //DRAW TICKS for testing
            model.DrawNumber(tCount++, "Ticks: ", this._renderWindow);
        }

        private void UpdateWindow()
        {
            //Process Events
            this._renderWindow.DispatchEvents();

            //Clear Screen
            this._renderWindow.Clear(model.BackgroundColor);

            //Draw Screen
            this.DrawScreen();

            //Display Window
            this._renderWindow.Display();
        }
    }
}
