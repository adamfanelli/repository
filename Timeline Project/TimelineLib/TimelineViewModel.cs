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
    public class TimelineViewModel : INotifyPropertyChanged
    {

        public string Title { get; set; }

        public Theme Theme;

        public RectangleShape Line { get; set; }
        public int LineThickness { get; set; }

        public float IntervalLengthPx { get; set; }
        public float IntervalThresholdPx { get; set; }


        public float Zoom { get; set; }
        public float ZoomSpeed { get; set; }

        public float EventFromLineHeight { get; set; }
        public float MarkerHeight { get; set; }
        public uint EventTextCharacterSize { get; set; }



        private bool isSideColumnVisible;
        public bool IsSideColumnVisible
        {
            get { return isSideColumnVisible; }
            set
            {
                isSideColumnVisible = value;
                NotifyPropertyChanged();
            }
        }

        public int YearAtMouse { get; set; }


        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float LineY { get; set; }
        public float ScrollSpeed { get; set; }
        public Font PrimaryFont { get; set; }
        public Font SecondaryFont { get; set; }

        public List<EventModel> ListOfEvents { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TimelineViewModel()
        {
            Line = new RectangleShape();
        }

        public void SetViewModel(TimelineModel t)
        {
            this.ListOfEvents = t.ListOfEvents;
            this.Theme = t.Theme;
            this.Title = t.Title;
            this.PrimaryFont = t.PrimaryFont;
            this.SecondaryFont = t.SecondaryFont;

            if (t.ListOfEvents == null) 
                this.ListOfEvents = new List<EventModel>();
        }



        public void AddEvent(EventModel e)
        {
            ListOfEvents.Add(e);
        }

        public void DrawLine(RenderWindow window)
        {
            LineY = window.Size.Y / 2 + 50 + OffsetY;
            Line.FillColor = Theme.LineColor;
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
                    if (_base / (100 / interval) * IntervalLengthPx * Zoom > IntervalThresholdPx)
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
            float x = (Interval * IntervalLengthPx * Zoom);


            while (OffsetX + x < window.Size.X)
            {
                if(OffsetX + x > 0)
                {
                    DrawMarker(window, OffsetX + x, i.ToString(), i % highlightInterval == 0);
                    nMarkers++;
                }
                
                i += Interval;
                x += Interval * IntervalLengthPx * Zoom;
            }

            i = -Interval;
            x = -(Interval * IntervalLengthPx * Zoom);

            while (OffsetX + x > 0)
            {
                if(OffsetX + x < window.Size.X)
                {
                    DrawMarker(window, OffsetX + x, i.ToString(), i % highlightInterval == 0);
                    nMarkers++;
                }

                i -= Interval;
                x -= Interval * IntervalLengthPx * Zoom;
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
            markerLine.FillColor = Theme.TextColor - new Color(0, 0, 0, 240);
            window.Draw(markerLine);

            RectangleShape marker = new RectangleShape();
            marker.Position = new Vector2f(x - LineThickness / 2, LineY - MarkerHeight / 2);
            marker.Size = new Vector2f(LineThickness, MarkerHeight);
            marker.FillColor = Theme.LineColor;
            window.Draw(marker);

            Text t = new Text(label, PrimaryFont);
            t.FillColor = Theme.TextColor;
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
            Text text = new Text(message + value.ToString(), SecondaryFont);
            text.Position = new Vector2f(renderWindow.Size.X - text.GetLocalBounds().Width - 40, Y);
            text.FillColor = Theme.TextColor;
            renderWindow.Draw(text);
        }

        public void DrawTitle(RenderWindow renderWindow)
        {
            Text text = new Text(Title, SecondaryFont);
            text.Position = new Vector2f(20, 20);
            text.FillColor = Theme.TextColor;
            renderWindow.Draw(text);
        }

        public void DrawEvents(RenderWindow window)
        {
            foreach (EventModel e in ListOfEvents)
            {
                float x = OffsetX + (IntervalLengthPx * Zoom) * e.StartYear;
                float y = LineY - EventFromLineHeight;

                // Text
                Text text = new Text(e.Name, PrimaryFont);
                text.CharacterSize = EventTextCharacterSize;
                text.Position = new Vector2f((int)(x - text.GetLocalBounds().Width / 2), y);
                text.FillColor = Theme.TextColor;

                // Background Rectangle
                RectangleShape textBg = new RectangleShape();
                textBg.FillColor = Theme.EventBackgroundColor;
                textBg.Position = new Vector2f(text.Position.X - 5, text.Position.Y - 5);
                textBg.Size = new Vector2f(text.GetLocalBounds().Width + 13, text.GetLocalBounds().Height + 20);

                // Triangle
                VertexArray triangle = new VertexArray(PrimitiveType.Triangles, 3);
                triangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, textBg.Position.Y + textBg.Size.Y), Theme.EventBackgroundColor);
                triangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, textBg.Position.Y + textBg.Size.Y), Theme.EventBackgroundColor);
                triangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y + 8), Theme.EventBackgroundColor);
                
                // Connector Triangle
                VertexArray connectorTriangle = new VertexArray(PrimitiveType.Triangles, 3);
                connectorTriangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, (LineY - LineThickness/2) - 8), Theme.TextColor);
                connectorTriangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, (LineY - LineThickness / 2) - 8), Theme.TextColor);
                connectorTriangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, (LineY - LineThickness / 2)), Theme.TextColor);

                // Connector Line
                //RectangleShape connectorLine = new RectangleShape();
                //connectorLine.FillColor = Color.Red; // Theme.EventBackgroundColor;
                //connectorLine.Position = new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y);
                //connectorLine.Size = new Vector2f(2, (LineY - 10) - (text.Position.Y + text.GetLocalBounds().Height));

                // Connector Dot
                //CircleShape connectorDot = new CircleShape();
                //connectorDot.FillColor = Theme.EventBackgroundColor;
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
