using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using TimelineLib;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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

        public float EventFromLineHeight { get; set; }
        public uint EventTextCharacterSize { get; set; }
        public int LineThickness { get; set; }

        public float LineY { get; set; }

        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float PanSpeed { get; set; }
        public Font font { get; set; }
        public Color EventBackgroundColor { get; set; }

        public List<TimelineEvent> ListOfEvents { get; set; }

        public TimelineModel()
        {
            WindowTitle = "Timeline Project";
            WindowWidth = 1200;
            WindowHeight = 900;

            OffsetX = 0;
            OffsetY = 0;
            PanSpeed = 0.5f;

            font = new Font("D:\\dev\\repository\\Timeline\\TimelineLib\\Fonts\\Hack-Regular.ttf");
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

        public void DrawNumber(int value, string message, RenderWindow renderWindow)
        {
            Text text = new Text(message + value.ToString(), font);
            text.Position = new Vector2f(renderWindow.Size.X - text.GetLocalBounds().Width - 40, 140);
            text.FillColor = Color.Black;
            renderWindow.Draw(text);
        }

        public void DrawMarkers(RenderWindow window)
        {
            int nMarkers = 0;

            //Always draw the origin marker (Year 0) at the X Offset
            DrawMarker(window, OffsetX, "0");

            nMarkers++;


            //Go Right from the Origin Marker
            int i = 1;
            while (OffsetX + i * MarkerInterval < window.Size.X)
            {
                if (OffsetX + i * MarkerInterval > 0)
                {
                    DrawMarker(window, OffsetX + i * MarkerInterval, i.ToString());
                    nMarkers++;
                }

                i++;
            }

            //Go Left from the Origin Marker
            i = -1;
            while (OffsetX + i * MarkerInterval > 0)
            {
                if (OffsetX + i * MarkerInterval < window.Size.X)
                {
                    DrawMarker(window, OffsetX + i * MarkerInterval, i.ToString());
                    nMarkers++;
                }

                i--;
            }

            //Display num of markers and OffsetX at top of screen for testing
            Text text = new Text("Markers: " + nMarkers.ToString(), font);
            text.Position = new Vector2f(window.Size.X - text.GetLocalBounds().Width - 20, 20);
            text.FillColor = Color.Black;
            window.Draw(text);
            
            Text text2 = new Text("X Offset: " + OffsetX.ToString(), font);
            text2.Position = new Vector2f(window.Size.X - text.GetLocalBounds().Width - 40, 80);
            text2.FillColor = Color.Black;
            window.Draw(text2);
        }

        private void DrawMarker(RenderWindow window, float x, string label = "N/A")
        {
            RectangleShape marker = new RectangleShape();
            marker.Position = new Vector2f(x - LineThickness/2, LineY - MarkerHeight / 2);
            marker.Size = new Vector2f(LineThickness, MarkerHeight);
            marker.FillColor = Color.Black;
            window.Draw(marker);

            Text t = new Text(label, font);
            t.CharacterSize = 14;
            t.Position = new Vector2f(marker.Position.X - t.GetLocalBounds().Width / 2, marker.Position.Y + 25);
            t.FillColor = Color.Black;
            window.Draw(t);
        }

        public void DrawEvents(RenderWindow window)
        {
            foreach(TimelineEvent e in ListOfEvents)
            {
                float x = OffsetX + MarkerInterval * e.StartYear;

                Text text = new Text(e.Name, font);
                text.CharacterSize = EventTextCharacterSize;
                text.Position = new Vector2f(x - text.GetLocalBounds().Width / 2, LineY - EventFromLineHeight);
                text.FillColor = Color.Black;

                RectangleShape textBg = new RectangleShape();
                textBg.FillColor = EventBackgroundColor;
                textBg.Position = new Vector2f(text.Position.X - 5, text.Position.Y - 5);
                textBg.Size = new Vector2f(text.GetLocalBounds().Width + 10, text.GetLocalBounds().Height + 20);

                VertexArray triangle = new VertexArray(PrimitiveType.Triangles, 3);
                triangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, textBg.Position.Y + textBg.Size.Y), EventBackgroundColor);
                triangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, textBg.Position.Y + textBg.Size.Y), EventBackgroundColor);
                triangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y + 8), EventBackgroundColor);
                window.Draw(triangle);
                


                window.Draw(textBg);
                window.Draw(text);
            }
        }
    }
}
