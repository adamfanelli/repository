using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using TimelineLib;
using TimelineLib.Themes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Diagnostics;

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
        public int ScrollCount { get; set; }

        public float ScrollSpeed { get; set; }

        public float EventFromLineHeight { get; set; }
        public float MinEventWidth { get; set; }
        public float MarkerHeight { get; set; }
        public float EventBgMargin { get; set; }
        public uint EventTextCharacterSize { get; set; }
        public uint MarkerCharacterSize { get; set; }
        public uint MarkerHighlightedCharacterSize { get; set; }



        private bool isSideColumnVisible;
        public bool IsSideColumnVisible
        {
            get { return isSideColumnVisible; }
            set { isSideColumnVisible = value; NotifyPropertyChanged(); }
        }

        private string newEventName;
        public string NewEventName
        {
            get { return newEventName; }
            set { newEventName = value; NotifyPropertyChanged(); }
        }

        private int newEventYear;
        public int NewEventYear
        {
            get { return newEventYear; }
            set { newEventYear = value; NotifyPropertyChanged(); }
        }


        public int YearAtMouse { get; set; }


        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float LineTopY { get; set; }
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
            ListOfEvents = ListOfEvents.OrderBy(x => x.StartYear).ToList();
        }

        public void DrawLine(RenderWindow window)
        {
            LineTopY = window.Size.Y / 2 + 50 + OffsetY;
            Line.FillColor = Theme.LineColor;
            Line.Position = new Vector2f(0, LineTopY);
            Line.Size = new Vector2f(window.Size.X, LineThickness);
            window.Draw(Line);
        }

        private List<int> Intervals = new List<int>() { 1, 2, 5, 10, 20, 25, 50, 100 };

        public void DrawMarkers(RenderWindow window)
        {
            // Draw the origin (year 0) marker
            if (OffsetX > 0 && OffsetX < window.Size.X)
                DrawMarker(window, OffsetX, "0", true);

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
                case 1: highlightInterval = 5; break;
                case 2: highlightInterval = 10; break;
                case 5: highlightInterval = 50; break;
                case 10: highlightInterval = 50; break;
                case 20: highlightInterval = 100; break;
                case 25: highlightInterval = 100; break;
                case 50: highlightInterval = 100; break;
                case 100: highlightInterval = 500; break;
            }

            highlightInterval *= _base / 100;


            int i = Interval;
            float x = (Interval * IntervalLengthPx * Zoom);


            while (OffsetX + x < window.Size.X)
            {
                if (OffsetX + x > 0)
                    DrawMarker(window, OffsetX + x, i.ToString(), i % highlightInterval == 0);

                i += Interval;
                x += Interval * IntervalLengthPx * Zoom;
            }

            i = -Interval;
            x = -(Interval * IntervalLengthPx * Zoom);

            while (OffsetX + x > 0)
            {
                if (OffsetX + x < window.Size.X)
                    DrawMarker(window, OffsetX + x, i.ToString(), i % highlightInterval == 0);

                i -= Interval;
                x -= Interval * IntervalLengthPx * Zoom;
            }
        }

        private void DrawMarker(RenderWindow window, float x, string label = "N/A", bool highlighted = false)
        {
            RectangleShape markerLine = new RectangleShape();
            markerLine.Position = new Vector2f(x - LineThickness / 4, 0);
            markerLine.Size = new Vector2f(LineThickness / 2, window.Size.Y);
            markerLine.FillColor = Theme.TextColor - new Color(0, 0, 0, 240);
            window.Draw(markerLine);

            RectangleShape marker = new RectangleShape();
            marker.Position = new Vector2f(x - LineThickness / 2, (LineTopY + LineThickness/2) - MarkerHeight / 2);
            marker.Size = new Vector2f(LineThickness, MarkerHeight);
            marker.FillColor = Theme.LineColor;
            window.Draw(marker);

            Text t = new Text(label, PrimaryFont);
            t.FillColor = Theme.TextColor;
            if (highlighted)
            {
                t.CharacterSize = MarkerHighlightedCharacterSize;
                t.Style = Text.Styles.Bold;
            }
            else
            {
                t.CharacterSize = MarkerCharacterSize;
            }
            t.Position = new Vector2f(marker.Position.X - t.GetLocalBounds().Width / 2, marker.Position.Y + 25);

            window.Draw(t);
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
            int level = 1;
            float prevX = 0;

            List<Shape> ShapesToDraw = new List<Shape>();
            List<Text> TextToDraw = new List<Text>();
            List<VertexArray> VertexArraysToDraw = new List<VertexArray>();

            foreach (EventModel e in ListOfEvents)
            {
                // Set X
                float x = OffsetX + (IntervalLengthPx * Zoom) * e.StartYear;

                // Create Text
                Text text = new Text(e.Name, PrimaryFont);
                text.CharacterSize = EventTextCharacterSize;
                text.FillColor = Theme.TextColor;

                // Set Level
                if(e != ListOfEvents.First())
                {
                    if (x - text.GetLocalBounds().Width / 2 - EventBgMargin < prevX)
                    {
                        level++;
                    }
                    else
                    {
                        level = 1;
                    }
                }
                

                // Set position of text
                text.Position = new Vector2f(x - text.GetLocalBounds().Width/2, LineTopY - (level * EventFromLineHeight) - text.GetLocalBounds().Height + EventBgMargin);

                // Background Rectangle
                RectangleShape textBg = new RectangleShape();
                textBg.FillColor = Theme.EventBackgroundColor;
                textBg.Position = new Vector2f(text.Position.X - EventBgMargin, text.Position.Y - 5);
                textBg.Size = new Vector2f(text.GetLocalBounds().Width + EventBgMargin * 2, text.GetLocalBounds().Height + 20);

                // Set PrevX
                prevX = textBg.Position.X + textBg.Size.X;

                // Triangle
                VertexArray triangle = new VertexArray(PrimitiveType.Triangles, 3);
                triangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, textBg.Position.Y + textBg.Size.Y), Theme.EventBackgroundColor);
                triangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, textBg.Position.Y + textBg.Size.Y), Theme.EventBackgroundColor);
                triangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y + 8), Theme.EventBackgroundColor);
                
                // Connector Triangle
                VertexArray connectorTriangle = new VertexArray(PrimitiveType.Triangles, 3);
                connectorTriangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, LineTopY - 8), Theme.TextColor);
                connectorTriangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, LineTopY - 8), Theme.TextColor);
                connectorTriangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, LineTopY), Theme.TextColor);

                // Connector Dashed Line    (draw noDashes dashes per level)
                int noDashes = 4;
                int dashThickness = 2;

                for(int i = 0; i < noDashes * level; i++)
                {
                    RectangleShape dash = new RectangleShape();
                    dash.FillColor = Theme.TextColor;

                    float bottomOfTextBox = LineTopY - EventFromLineHeight * level;

                    dash.Position = new Vector2f(
                        text.Position.X + text.GetLocalBounds().Width / 2 - dashThickness/2,

                        bottomOfTextBox + i * ((LineTopY - bottomOfTextBox) / (noDashes * level))                                          // i * ((EventFromLineHeight * level) / (noDashes * level))

                        );
                    dash.Size = new Vector2f(
                        dashThickness,
                        (LineTopY - bottomOfTextBox) / (noDashes * level * 2)
                        );

                    window.Draw(dash);
                }

                //window.Draw(textBg);
                //window.Draw(triangle);
                //window.Draw(text);
                //window.Draw(connectorTriangle);

                ShapesToDraw.Add(textBg);
                VertexArraysToDraw.Add(triangle);
                VertexArraysToDraw.Add(connectorTriangle);
                TextToDraw.Add(text);
            }


            foreach(Shape shape in ShapesToDraw)
                window.Draw(shape);
            
            foreach(VertexArray vertexArray in VertexArraysToDraw)
                window.Draw(vertexArray);
            
            foreach(Text text in TextToDraw)
                window.Draw(text);

        }
    }
}
