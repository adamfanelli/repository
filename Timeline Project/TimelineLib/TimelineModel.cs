using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using TimelineLib;
using System.Collections.Generic;

namespace TimelineLib
{
    public class TimelineModel
    {
        public uint WindowWidth { get; set; }
        public uint WindowHeight { get; set; }
        public string WindowTitle { get; set; }

        public RectangleShape Line { get; set; }

        public const float BASE_INTERVAL = 60;
        public float MarkerInterval { get; set; }
        public float MarkerHeight { get; set; }


        public float Zoom { get; set; }
        public float ZoomSpeed { get; set; }

        public float EventFromLineHeight { get; set; }
        public uint EventTextCharacterSize { get; set; }
        public int LineThickness { get; set; }

        public float LineY { get; set; }

        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float PanSpeed { get; set; }
        public Font font { get; set; }
        public Color EventBackgroundColor { get; set; }
        public Color BackgroundColor { get; set; }

        public List<TimelineEvent> ListOfEvents { get; set; }

        public TimelineModel()
        {
            WindowTitle = "Timeline Project";
            WindowWidth = 1200;
            WindowHeight = 900;

            OffsetX = 0;
            OffsetY = 0;

            Zoom = 1.0f;
            ZoomSpeed = 10.0f;

            string dir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            font = new Font(dir + "\\Fonts\\Hack-Regular.ttf");

            EventTextCharacterSize = 20;
            EventBackgroundColor = new Color(230, 230, 230);

            ListOfEvents = new List<TimelineEvent>();

            Line = new RectangleShape();
            Line.FillColor = Color.Black;
            LineThickness = 4;

            EventFromLineHeight = 50;

            MarkerInterval = 60;
            MarkerHeight = 14;
        }

        public void AddEvent(TimelineEvent e)
        {
            ListOfEvents.Add(e);
        }

        public void DrawLine(RenderWindow window)
        {
            LineY = window.Size.Y / 2 + 50;
            Line.Position = new Vector2f(0, LineY - LineThickness / 2);
            Line.Size = new Vector2f(window.Size.X, LineThickness);
            window.Draw(Line);
        }

        List<int> Intervals = new List<int>() { 100, 50, 20, 10, 5, 2, 1 };

        public void DrawMarkers(RenderWindow window)
        {
            // Draw the origin marker (Year 0) at the X Offset
            if(OffsetX > 0 && OffsetX < window.Size.X) DrawMarker(window, OffsetX, "0");


            double minMarkerInterval = 60.0;
            int Interval = -1;

            int _base = 100;

            while(Interval == -1)
            {
                foreach (int interval in Intervals)
                {
                    if (_base / interval * BASE_INTERVAL * Zoom > minMarkerInterval)
                    {
                        Interval = _base / interval;
                        break;
                    }
                }

                _base *= 100;
            }
            

            int nMarkers = 0;
            int i = 0;
            float x = 0;


            while (OffsetX + x < window.Size.X)
            {
                if(OffsetX + x > 0)
                {
                    DrawMarker(window, OffsetX + x, i.ToString());
                    nMarkers++;
                }
                
                i += Interval;
                x += Interval * BASE_INTERVAL * Zoom;
            }

            i = -Interval;
            x = -(Interval * BASE_INTERVAL * Zoom);

            while (OffsetX + x > 0)
            {
                if(OffsetX + x < window.Size.X)
                {
                    DrawMarker(window, OffsetX + x, i.ToString());
                    nMarkers++;
                }

                i -= Interval;
                x -= Interval * BASE_INTERVAL * Zoom;
            }

            DrawDebugNumber("Markers: ", nMarkers, window, 20);
            DrawDebugNumber("Offset X: ", OffsetX, window, 80);
            DrawDebugNumber("Interval: ", Interval, window, 140);
        }

        private void DrawMarker(RenderWindow window, float x, string label = "N/A")
        {
            RectangleShape marker = new RectangleShape();
            marker.Position = new Vector2f(x - LineThickness / 2, LineY - MarkerHeight / 2);
            marker.Size = new Vector2f(LineThickness, MarkerHeight);
            marker.FillColor = Color.Black;
            window.Draw(marker);

            Text t = new Text(label, font);
            t.CharacterSize = 14;
            t.Position = new Vector2f(marker.Position.X - t.GetLocalBounds().Width / 2, marker.Position.Y + 25);
            t.FillColor = Color.Black;
            window.Draw(t);
        }


        public void DrawDebugNumber(string message, float value, RenderWindow renderWindow, float Y)
        {
            Text text = new Text(message + value.ToString(), font);
            text.Position = new Vector2f(renderWindow.Size.X - text.GetLocalBounds().Width - 40, Y);
            text.FillColor = Color.Black;
            renderWindow.Draw(text);
        }

        public void DrawEvents(RenderWindow window)
        {
            foreach (TimelineEvent e in ListOfEvents)
            {
                float x = OffsetX + (BASE_INTERVAL * Zoom) * e.StartYear;

                //Text
                Text text = new Text(e.Name, font);
                text.CharacterSize = EventTextCharacterSize;
                text.Position = new Vector2f(x - text.GetLocalBounds().Width / 2, LineY - EventFromLineHeight);
                text.FillColor = Color.Black;

                //Background Rectangle
                RectangleShape textBg = new RectangleShape();
                textBg.FillColor = EventBackgroundColor;
                textBg.Position = new Vector2f(text.Position.X - 5, text.Position.Y - 5);
                textBg.Size = new Vector2f(text.GetLocalBounds().Width + 10, text.GetLocalBounds().Height + 20);

                //Triangle
                VertexArray triangle = new VertexArray(PrimitiveType.Triangles, 3);
                triangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, textBg.Position.Y + textBg.Size.Y), EventBackgroundColor);
                triangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, textBg.Position.Y + textBg.Size.Y), EventBackgroundColor);
                triangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y + 8), EventBackgroundColor);

                window.Draw(textBg);
                window.Draw(triangle);
                window.Draw(text);
            }
        }
    }
}
