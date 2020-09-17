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


        public double Zoom { get; set; }
        public float ZoomSpeed { get; set; }
        public int ScrollCount { get; set; }
        public int ZoomMinCap { get; set; }
        public int ZoomMaxCap { get; set; }

        public float ScrollSpeed { get; set; }

        public float EventFromLineHeight { get; set; }
        public float MinEventWidth { get; set; }
        public float MarkerHeight { get; set; }
        public float EventBgMargin { get; set; }
        public uint EventTextCharacterSize { get; set; }
        public uint MarkerCharacterSize { get; set; }
        public uint MarkerHighlightedCharacterSize { get; set; }

        public EventViewModel EditingEvent { get; set; }

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

        private string sideColumnHeader;
        public string SideColumnHeader
        {
            get { return sideColumnHeader; }
            set { sideColumnHeader = value; NotifyPropertyChanged(); }
        }
        
        public bool showDeleteButton;
        public bool ShowDeleteButton
        {
            get { return showDeleteButton; }
            set { showDeleteButton = value; NotifyPropertyChanged(); }
        }

        public int YearAtMouse { get; set; }


        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float LineTopY { get; set; }
        public Font PrimaryFont { get; set; }
        public Font SecondaryFont { get; set; }

        public List<EventViewModel> ListOfEvents { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TimelineViewModel()
        {
            Line = new RectangleShape();
            ListOfEvents = new List<EventViewModel>();

            LineThickness = 4;
            Zoom = 1.0f;
            ZoomSpeed = 10.0f;
            ZoomMinCap = 12;
            ZoomMaxCap = -132;
            ScrollSpeed = 5f;

            EventTextCharacterSize = 24;
            MarkerCharacterSize = 15;
            MarkerHighlightedCharacterSize = 18;
            EventFromLineHeight = 60;
            EventBgMargin = 5;
            MinEventWidth = 40;
            IntervalThresholdPx = 100;
            IntervalLengthPx = 60;
            MarkerHeight = 14;

            PrimaryFont = new Font(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
            "\\Fonts\\GeosansLight.ttf");

            SecondaryFont = new Font(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
            "\\Fonts\\OptimusPrinceps.ttf");
        }

        public void SetViewModel(TimelineModel t)
        {
            if (t.EventModels != null)
                foreach (EventModel e in t.EventModels)
                {
                    EventViewModel vm = new EventViewModel();
                    vm.SetViewModel(e);
                    this.ListOfEvents.Add(vm);
                }

            this.Theme = Theme.GetThemeByID(t.ThemeID);
            this.Title = t.Title;
        }

        public TimelineModel ConvertToSaveModel()
        {
            TimelineModel t = new TimelineModel();
            t.EventModels = new List<EventModel>();

            foreach(EventViewModel e in ListOfEvents)
            {
                t.EventModels.Add(e.ConvertToSaveModel());
            }

            t.ThemeID = this.Theme.ID;
            t.Title = this.Title;

            return t;
        }



        public void AddEvent(EventViewModel e)
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
            double x = (Interval * IntervalLengthPx * Zoom);


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

        private void DrawMarker(RenderWindow window, double x, string label = "N/A", bool highlighted = false)
        {
            RectangleShape markerLine = new RectangleShape();
            markerLine.Position = new Vector2f((float)x - LineThickness / 4, 0);
            markerLine.Size = new Vector2f(LineThickness / 2, window.Size.Y);
            markerLine.FillColor = Theme.TextColor - new Color(0, 0, 0, 240);
            window.Draw(markerLine);

            RectangleShape marker = new RectangleShape();
            marker.Position = new Vector2f((float)x - LineThickness / 2, (LineTopY + LineThickness/2) - MarkerHeight / 2);
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

        public void DrawTitle(RenderWindow window)
        {
            Text text = new Text(Title, SecondaryFont);
            text.Position = new Vector2f(20, 20);
            text.FillColor = Theme.TitleColor;
            window.Draw(text);
        }

        public void DrawEvents(RenderWindow window)
        {
            // An array of the Previous X for each level
            List<float> PrevX = new List<float>() { 0 };

            List<Shape> ShapesToDraw = new List<Shape>();
            List<Text> TextToDraw = new List<Text>();
            List<VertexArray> VertexArraysToDraw = new List<VertexArray>();

            foreach (EventViewModel e in ListOfEvents)
            {

                // Create Text
                Text text = new Text(e.Name, PrimaryFont);
                text.CharacterSize = EventTextCharacterSize;
                text.FillColor = Theme.TextColor;

                // Set Level
                int level = 1;

                // Set X (center of event)
                double x = OffsetX + (IntervalLengthPx * Zoom) * e.StartYear;

                if (e != ListOfEvents.First())
                {
                    while(x - text.GetLocalBounds().Width / 2 - EventBgMargin - text.GetLocalBounds().Height / 2 < PrevX[level])
                    {
                        level++;

                        if (level >= PrevX.Count) break;
                    }
                }

                // Set Y (top of event)
                float y = LineTopY - (level * EventFromLineHeight) - text.GetLocalBounds().Height;

                // Set Text Position
                text.Position = new Vector2f((float)x - text.GetLocalBounds().Width/2, y + EventBgMargin);

                // Background Rectangle
                RectangleShape textBg = new RectangleShape();
                textBg.Position = new Vector2f(text.Position.X - EventBgMargin, text.Position.Y - 5);
                textBg.Size = new Vector2f(text.GetLocalBounds().Width + EventBgMargin * 2, text.GetLocalBounds().Height + 20);

                // Background Circular Borders
                CircleShape lCircle = new CircleShape();
                lCircle.Radius = textBg.Size.Y / 2;
                lCircle.Position = new Vector2f(textBg.Position.X - lCircle.Radius, text.Position.Y - 5);

                CircleShape rCircle = new CircleShape();
                rCircle.Radius = textBg.Size.Y / 2;
                rCircle.Position = new Vector2f(textBg.Position.X + textBg.Size.X - rCircle.Radius, text.Position.Y - 5);

                // Set ScreenPos of EventModel
                e.ScreenPos = new Vector2f((float)x - textBg.Size.X / 2 - lCircle.Radius, y);
                e.Size = new Vector2f(lCircle.Radius + textBg.Size.X + rCircle.Radius, textBg.Size.Y);

                // Change background color on hover
                Color bgColor = (e.IsMouseOver(window) || e == EditingEvent) ? Theme.EventHoverColor : Theme.EventBackgroundColor;

                textBg.FillColor = bgColor;
                lCircle.FillColor = bgColor;
                rCircle.FillColor = bgColor;

                // Background Triangle
                VertexArray bgTriangle = new VertexArray(PrimitiveType.Triangles, 3);
                bgTriangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, textBg.Position.Y + textBg.Size.Y), bgColor);
                bgTriangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, textBg.Position.Y + textBg.Size.Y), bgColor);
                bgTriangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y + 8), bgColor);

                VertexArraysToDraw.Add(bgTriangle);

                ShapesToDraw.Add(textBg);
                ShapesToDraw.Add(lCircle);
                ShapesToDraw.Add(rCircle);


                // Set PrevX
                float prevx = e.ScreenPos.X + e.Size.X;

                if (level >= PrevX.Count)
                    PrevX.Add(prevx);
                else
                    PrevX[level] = prevx;
                
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
                VertexArraysToDraw.Add(connectorTriangle);
                TextToDraw.Add(text);
            }

            foreach (Shape shape in ShapesToDraw)
                window.Draw(shape);
            
            foreach(VertexArray vertexArray in VertexArraysToDraw)
                window.Draw(vertexArray);
            
            foreach(Text text in TextToDraw)
                window.Draw(text);

        }
    }
}
