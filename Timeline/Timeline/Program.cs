using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using TimelineLib;

namespace Timeline
{
    class Program
    {
        public static RenderWindow window;
        public static TimelineModel model;

        public static bool KeyPressed_W;
        public static bool KeyPressed_A;
        public static bool KeyPressed_S;
        public static bool KeyPressed_D;

        static void Main(string[] args)
        {
            model = new TimelineModel();
            window = new RenderWindow(new VideoMode(model.WindowWidth, model.WindowHeight), model.WindowTitle);

            //Controls


            while (window.IsOpen)
            {
                window.DispatchEvents();

                window.Closed += new EventHandler(OnClose);
                window.KeyPressed += new EventHandler<KeyEventArgs>(myKeyHandler);
                window.KeyReleased += new EventHandler<KeyEventArgs>(myKeyReleasedHandler);
                window.Resized += new EventHandler<SizeEventArgs>(myResizeHandler);

                window.Clear(Color.White);

                //DRAW LINE
                model.DrawLine(window);

                //DRAW MARKERS
                model.DrawMarkers(window);


                window.Display();
            }
        }


        private static void myResizeHandler(object sender, SizeEventArgs e)
        {
            FloatRect visibleArea = new FloatRect(new Vector2f(0, 0), new Vector2f(e.Width, e.Height));
            window.SetView(new View(visibleArea));
        }

        private static void myKeyHandler(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.W:
                    if (!KeyPressed_W)
                    {
                        model.OffsetY -= model.PanSpeed;
                        KeyPressed_W = true;
                    }
                    break;

                case Keyboard.Key.A:
                    if (!KeyPressed_A)
                    {
                        model.OffsetX -= model.PanSpeed;
                        KeyPressed_A = true;
                    }
                    break;

                case Keyboard.Key.S:
                    if (!KeyPressed_S)
                    {
                        model.OffsetY += model.PanSpeed;
                        KeyPressed_S = true;
                    }
                    break;

                case Keyboard.Key.D:
                    if (!KeyPressed_D)
                    {
                        model.OffsetX += model.PanSpeed;
                        KeyPressed_D = true;
                    }
                    break;
            }
        }
        private static void myKeyReleasedHandler(object sender, KeyEventArgs e)
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

        private static void OnClose(object sender, EventArgs e)
        {
            window.Close();
        }
    }
}
