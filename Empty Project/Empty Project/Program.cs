using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace Empty_Project
{
    class Program
    {
        public static RenderWindow window;

        public static int WindowWidth;
        public static int WindowHeight;

        static void Main(string[] args)
        {

            WindowWidth = 1200;
            WindowHeight = 800;
            window = new RenderWindow(new VideoMode((uint)WindowWidth, (uint)WindowHeight), "Timeline Project");

            while (window.IsOpen)
            {
                window.DispatchEvents();

                window.Closed += new EventHandler(OnClose);
                window.KeyPressed += new EventHandler<KeyEventArgs>(myKeyHandler);
                window.Resized += new EventHandler<SizeEventArgs>(myResizeHandler);

                window.Clear(Color.White);

                //DRAW LINE
                RectangleShape Line = new RectangleShape();
                Line.FillColor = Color.Black;
                Line.OutlineThickness = 3;
                Line.Size = new Vector2f(window.Size.X, 6);
                Line.Position = new Vector2f(0, 100);

                

                window.Draw(Line);


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
            //throw new NotImplementedException();
        }

        private static void OnClose(object sender, EventArgs e)
        {
            window.Close();
        }
    }
}
