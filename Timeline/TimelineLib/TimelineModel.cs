using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using TimelineLib;
using System.ComponentModel.DataAnnotations;

namespace TimelineLib
{
    public class TimelineModel
    {
        public uint WindowWidth { get; set; }
        public uint WindowHeight{ get; set; }
        public string WindowTitle { get; set; }

        public RectangleShape Line { get; set; }
        public float MarkerInterval { get; set; }
        public float MarkerHeight { get; set; }

        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float PanSpeed { get; set; }

        public Font font { get; set; }

        public TimelineModel()
        {
            WindowTitle = "Timeline Project";
            WindowWidth = 1200;
            WindowHeight = 900;

            OffsetX = 0;
            OffsetY = 0;
            PanSpeed = 8f;

            font = new Font("D:\\dev\\repository\\Timeline\\TimelineLib\\Fonts\\JMH Typewriter.ttf");

            Line = new RectangleShape();
            Line.FillColor = Color.Black;
            Line.OutlineThickness =  4;

            MarkerInterval = 60;
            MarkerHeight = 14;
        }

        public void DrawLine(RenderWindow window)
        {
            Line.Position = new Vector2f(0, window.Size.Y / 2);
            Line.Size = new Vector2f(window.Size.X, Line.OutlineThickness);
            window.Draw(Line);
        }

        public void DrawMarkers(RenderWindow window)
        {
            int nMarkers = 0;
            float x = window.Size.X / 2 + OffsetX;

            while (x < window.Size.X)
            {
                DrawMarker(window, x);
                x += MarkerInterval;
                nMarkers++;
            }

            x = window.Size.X / 2 + OffsetX - MarkerInterval;

            while (x > 0)
            {
                DrawMarker(window, x);
                x -= MarkerInterval;
                nMarkers++;
            }

            //Display num of markers and OffsetX at top of screen for testing
            Text text = new Text(nMarkers.ToString(), font);
            text.Position = new Vector2f(window.Size.X - 60, 20);
            text.FillColor = Color.Black;
            window.Draw(text);
            
            Text text2 = new Text(OffsetX.ToString(), font);
            text2.Position = new Vector2f(window.Size.X - 60, 80);
            text2.FillColor = Color.Black;
            window.Draw(text2);
        }

        private void DrawMarker(RenderWindow window, float x)
        {
            RectangleShape marker = new RectangleShape();
            marker.Position = new Vector2f(x, Line.Position.Y + Line.OutlineThickness / 2 - MarkerHeight / 2);
            marker.Size = new Vector2f(Line.OutlineThickness, MarkerHeight);
            marker.FillColor = Color.Black;
            window.Draw(marker);
        }
    }
}
