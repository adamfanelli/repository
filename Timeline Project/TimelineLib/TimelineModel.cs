using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using System;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using TimelineLib;
using TimelineLib.Themes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimelineLib
{
    public class TimelineModel
    {
        public Theme theme;
        public uint WindowWidth { get; set; }
        public uint WindowHeight { get; set; }
        public string WindowTitle { get; set; }

        public RectangleShape Line { get; set; }

        // The number of pixels between 0 and 1 when Zoom is at 1.0
        public float BASE_INTERVAL = 60;

        public double minMarkerInterval;
        public float MarkerInterval { get; set; }
        public float MarkerHeight { get; set; }

        public string TimelineTitle { get; set; }
        public float Zoom { get; set; }
        public float ZoomSpeed { get; set; }

        public float EventFromLineHeight { get; set; }
        public uint EventTextCharacterSize { get; set; }
        public int LineThickness { get; set; }

        public float LineY { get; set; }

        public int YearAtMouse { get; set; }


        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float ScrollSpeed { get; set; }
        public Font primaryFont { get; set; }
        public Font secondaryFont { get; set; }

        public List<EventModel> ListOfEvents { get; set; }

        public TimelineModel()
        {
            ListOfEvents = new List<EventModel>();

            Line = new RectangleShape();
            LineThickness = 4;

            //Set the theme
            theme = new ThemeBlue();
        }



        public void AddEvent(EventModel e)
        {
            ListOfEvents.Add(e);
        }

        public void DrawLine(RenderWindow window)
        {
            LineY = window.Size.Y / 2 + 50 + OffsetY;
            Line.FillColor = theme.LineColor;
            Line.Position = new Vector2f(0, LineY - LineThickness / 2);
            Line.Size = new Vector2f(window.Size.X, LineThickness);
            window.Draw(Line);
        }

        private List<int> Intervals = new List<int>() { 1, 2, 5, 10, 20, 25, 50, 100 };

        public void DrawMarkers(RenderWindow window)
        {
            int nMarkers = 0;

            // Draw the origin (year 0) marker
            if (OffsetX > 0 && OffsetX < window.Size.X)
            {
                DrawMarker(window, OffsetX, "0", true);
                nMarkers++;
            }
                

            int Interval = -1;
            int _base = 100;
            bool done = false;

            while (!done)
            {

                foreach (int interval in Intervals)
                {
                    if (_base / (100 / interval) * BASE_INTERVAL * Zoom > minMarkerInterval)
                    {
                        Interval = _base / 100 * interval;
                        done = true;
                        break;
                    }
                }

                if (!done) _base *= 100;
            }

            int highlightInterval = 2;
            switch (Interval / (_base / 100))
            {
                case 1:     highlightInterval = 5;       break;
                case 2:     highlightInterval = 10;      break;
                case 5:     highlightInterval = 50;      break;
                case 10:    highlightInterval = 50;      break;
                case 20:    highlightInterval = 100;     break;
                case 25:    highlightInterval = 100;     break;
                case 50:    highlightInterval = 100;     break;
                case 100:   highlightInterval = 500;     break;
            }

            highlightInterval *= _base / 100;


            int i = Interval;
            float x = (Interval * BASE_INTERVAL * Zoom);


            while (OffsetX + x < window.Size.X)
            {
                if(OffsetX + x > 0)
                {
                    DrawMarker(window, OffsetX + x, i.ToString(), i % highlightInterval == 0);
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
                    DrawMarker(window, OffsetX + x, i.ToString(), i % highlightInterval == 0);
                    nMarkers++;
                }

                i -= Interval;
                x -= Interval * BASE_INTERVAL * Zoom;
            }

            float MouseOffsetDelta = (Mouse.GetPosition().X - window.Position.X) - OffsetX;

            //DrawDebugNumber("Markers: ", nMarkers, window, 20);
            //DrawDebugNumber("Offset X: ", OffsetX, window, 20);
            //DrawDebugNumber("Interval: ", Interval, window, 120);
            //DrawDebugNumber("Base: ", _base, window, 200);
            //DrawDebugNumber("Mouse X: ", Mouse.GetPosition().X - window.Position.X, window, 70);
            //DrawDebugNumber("Delta between Mouse and OffsetX: ", MouseOffsetDelta, window, 120);

            DrawDebugNumber(
                "Year at Mouse: ",
                YearAtMouse, 
                window, 
                20
            );
        }

        

        private void DrawMarker(RenderWindow window, float x, string label = "N/A", bool highlighted = false)
        {
            RectangleShape markerLine = new RectangleShape();
            markerLine.Position = new Vector2f(x - LineThickness / 4, 0);
            markerLine.Size = new Vector2f(LineThickness / 2, window.Size.Y);
            markerLine.FillColor = theme.TextColor - new Color(0, 0, 0, 240);
            window.Draw(markerLine);

            RectangleShape marker = new RectangleShape();
            marker.Position = new Vector2f(x - LineThickness / 2, LineY - MarkerHeight / 2);
            marker.Size = new Vector2f(LineThickness, MarkerHeight);
            marker.FillColor = theme.LineColor;
            window.Draw(marker);

            Text t = new Text(label, primaryFont);
            t.FillColor = theme.TextColor;
            if (highlighted)
            {
                t.CharacterSize = 18;
                t.Style = Text.Styles.Bold;
            }
            else
            {
                t.CharacterSize = 14;
            }
            t.Position = new Vector2f(marker.Position.X - t.GetLocalBounds().Width / 2, marker.Position.Y + 25);

            window.Draw(t);
        }


        public void DrawDebugNumber(string message, float value, RenderWindow renderWindow, float Y)
        {
            Text text = new Text(message + value.ToString(), secondaryFont);
            text.Position = new Vector2f(renderWindow.Size.X - text.GetLocalBounds().Width - 40, Y);
            text.FillColor = theme.TextColor;
            renderWindow.Draw(text);
        }

        public void DrawTitle(RenderWindow renderWindow)
        {
            Text text = new Text(TimelineTitle, secondaryFont);
            text.Position = new Vector2f(20, 20);
            text.FillColor = theme.TextColor;
            renderWindow.Draw(text);
        }

        public void DrawEvents(RenderWindow window)
        {
            foreach (EventModel e in ListOfEvents)
            {
                float x = OffsetX + (BASE_INTERVAL * Zoom) * e.StartYear;
                float y = LineY - EventFromLineHeight;

                // Text
                Text text = new Text(e.Name, primaryFont);
                text.CharacterSize = EventTextCharacterSize;
                text.Position = new Vector2f((int)(x - text.GetLocalBounds().Width / 2), y);
                text.FillColor = theme.TextColor;

                // Background Rectangle
                RectangleShape textBg = new RectangleShape();
                textBg.FillColor = theme.EventBackgroundColor;
                textBg.Position = new Vector2f(text.Position.X - 5, text.Position.Y - 5);
                textBg.Size = new Vector2f(text.GetLocalBounds().Width + 13, text.GetLocalBounds().Height + 20);

                // Triangle
                VertexArray triangle = new VertexArray(PrimitiveType.Triangles, 3);
                triangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, textBg.Position.Y + textBg.Size.Y), theme.EventBackgroundColor);
                triangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, textBg.Position.Y + textBg.Size.Y), theme.EventBackgroundColor);
                triangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y + 8), theme.EventBackgroundColor);
                
                // Connector Triangle
                VertexArray connectorTriangle = new VertexArray(PrimitiveType.Triangles, 3);
                connectorTriangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, (LineY - LineThickness/2) - 8), theme.TextColor);
                connectorTriangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, (LineY - LineThickness / 2) - 8), theme.TextColor);
                connectorTriangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, (LineY - LineThickness / 2)), theme.TextColor);

                // Connector Line
                //RectangleShape connectorLine = new RectangleShape();
                //connectorLine.FillColor = Color.Red; // theme.EventBackgroundColor;
                //connectorLine.Position = new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y);
                //connectorLine.Size = new Vector2f(2, (LineY - 10) - (text.Position.Y + text.GetLocalBounds().Height));

                // Connector Dot
                //CircleShape connectorDot = new CircleShape();
                //connectorDot.FillColor = theme.EventBackgroundColor;
                //connectorDot.Radius = 5;
                //connectorDot.Position = new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - connectorDot.Radius / 2, LineY - 12);

                window.Draw(textBg);
                window.Draw(triangle);
                window.Draw(text);
                //window.Draw(connectorLine);
                window.Draw(connectorTriangle);
            }
        }
    }
}
