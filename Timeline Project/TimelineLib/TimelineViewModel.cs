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
        public float eventBgMargin { get; set; }
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

        private int newEventMonth;
        public int NewEventMonth
        {
            get { return newEventMonth; }
            set { newEventMonth = value; NotifyPropertyChanged(); }
        }

        private int newEventDay;
        public int NewEventDay
        {
            get { return newEventDay; }
            set { newEventDay = value; NotifyPropertyChanged(); }
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
            ZoomMinCap = 40;
            ZoomMaxCap = -132;
            ScrollSpeed = 5f;

            EventTextCharacterSize = 24;
            MarkerCharacterSize = 15;
            MarkerHighlightedCharacterSize = 18;
            IntervalLengthPx = 60;
            IntervalThresholdPx = 60;
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
        private List<int> MonthIntervals = new List<int>() { 1, 2, 3, 5, 11 };
        private List<string> Months = new List<string>() { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };

        public void DrawMarkers(RenderWindow window)
        {
            // Draw the origin (year 0) marker
            if (OffsetX > 0 && OffsetX < window.Size.X) DrawMarker(window, OffsetX, "0", 2, true, true);

            int Interval = -1;
            int _base = 100;
            bool done = false;

            // (1) Cycle through each interval and check if that interval divided by 100 and multiplied by _base, multiplied by IntervalLengthPx and Zoom, is greater
            // than the min pixel threshold. If it is, set Interval to that interval. If it isn't, multiply _base by 100 and loop again.
            // ------------------------------------------------------------------------------------------------
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
            // ---------------------------------------------------------------


            // (2) Determine on what interval markers should be highlighted based on the current Interval value.
            // ------------------------------------------------------------------------------------------------
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
            // ---------------------------------------------------------------



            // (3) Draw the markers from 0 rightward, then from 0 leftward. Highlight appropriate markers. Draw month markers if needed.
            // ------------------------------------------------------------------------------------------------
            int i = Interval;
            double intervalLength = Interval * IntervalLengthPx * Zoom;
            double x = intervalLength;

            if (intervalLength > 240) highlightInterval = 1;

            // Month Markers
            int monthLabelBuffer = 9;
            int noMonthMarkers = 0;
            foreach(int interval in MonthIntervals)
                if ((int)((intervalLength / Interval) / IntervalLengthPx) - 1 > interval)
                    noMonthMarkers = interval;

            
            // Set x to the left side of the screen, then draw markers from left to right
            while (OffsetX + x > 0)
            {
                i -= Interval;
                x -= intervalLength;
            }

            while (OffsetX + x - intervalLength < window.Size.X)
            {
                DrawMarker(window, OffsetX + x, i.ToString(), i % highlightInterval == 0 ? 2 : 1, true, intervalLength > 300);

                if(noMonthMarkers > 0 && noMonthMarkers < 11)
                {
                    //Always draw Jan
                    Text t = new Text(Months[0], PrimaryFont);
                    t.FillColor = Theme.LineColor;
                    t.CharacterSize = MarkerCharacterSize;
                    t.Position = new Vector2f((float)(OffsetX + x + monthLabelBuffer), LineTopY + 6);
                    window.Draw(t);

                    int monthInterval = 12 / (noMonthMarkers + 1);

                    for (int idx = monthInterval; idx < 12; idx += monthInterval)
                    {
                        Text _t = new Text(Months[idx], PrimaryFont);
                        _t.FillColor = Theme.LineColor;
                        _t.CharacterSize = MarkerCharacterSize;
                        _t.Position = new Vector2f((float)(OffsetX + x + ((intervalLength / 12) * idx) + monthLabelBuffer), LineTopY + 6);
                        window.Draw(_t);
                    }
                }


                for (int idx = 1; idx <= noMonthMarkers; idx++)
                {
                    float mx = (float)(OffsetX + x + idx * (intervalLength / (noMonthMarkers + 1)));

                    DrawMarker(window, mx, "", 3, noMonthMarkers == 11);

                    if(noMonthMarkers == 11)
                    {
                        Text t = new Text(Months[12 / (noMonthMarkers + 1) * idx - 1], PrimaryFont);
                        t.FillColor = Theme.LineColor;
                        t.CharacterSize = MarkerCharacterSize;
                        t.Position = new Vector2f((float)(mx - intervalLength / 24 - t.GetLocalBounds().Width / 2), LineTopY + 10);
                        window.Draw(t);

                        // Draw Twelfth Year
                        if (idx == 11)
                        {
                            Text _t = new Text(Months[11], PrimaryFont);
                            _t.FillColor = Theme.LineColor;
                            _t.CharacterSize = MarkerCharacterSize;
                            _t.Position = new Vector2f((float)(mx + intervalLength / 24 - _t.GetLocalBounds().Width / 2), LineTopY + 10);
                            window.Draw(_t);
                        }
                    }
                }

                i += Interval;
                x += intervalLength;
            }
        }

        private void DrawMarker(RenderWindow window, double x, string label = "N/A", int type = 1, bool drawLine = true, bool highlightLine = false)
        {
            if(drawLine)
            {
                RectangleShape markerLine = new RectangleShape();
                markerLine.Position = new Vector2f((float)x - LineThickness / 4, 0);
                markerLine.Size = new Vector2f(LineThickness / 2, window.Size.Y);
                markerLine.FillColor = Theme.TextColor - new Color(0, 0, 0, (byte)(highlightLine ? 180 : 240));
                window.Draw(markerLine);
            }

            // Types: 1 for normal, 2 for highlighted, 3 for bump
            if (type == 1 || type == 2)
            {

                RectangleShape marker = new RectangleShape();
                marker.Position = new Vector2f((float)x - LineThickness / 2, (LineTopY + LineThickness / 2) - MarkerHeight / 2);
                marker.Size = new Vector2f(LineThickness, MarkerHeight);
                marker.FillColor = Theme.LineColor;
                window.Draw(marker); 
                
                Text t = new Text(label, PrimaryFont);
                t.FillColor = Theme.TextColor;
                if (type == 2)
                {
                    t.CharacterSize = MarkerHighlightedCharacterSize;
                    t.Style = Text.Styles.Bold;
                }
                else
                    t.CharacterSize = MarkerCharacterSize;
                t.Position = new Vector2f(marker.Position.X - t.GetLocalBounds().Width / 2, marker.Position.Y + 25);
                window.Draw(t);
            }
            else if (type == 3)
            {
                int radius = LineThickness;
                CircleShape markerBump = new CircleShape();
                markerBump.Position = new Vector2f((float)x - radius, (LineTopY + LineThickness / 2) - radius);
                markerBump.Radius = radius;
                markerBump.FillColor = Theme.LineColor;
                window.Draw(markerBump);
            }
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
            List<float> PrevRightSideX = new List<float>() { 0 };

            // Items in these lists will be drawn at the end of each loop
            List<Shape> ShapesToDraw = new List<Shape>();
            List<Text> TextToDraw = new List<Text>();
            List<VertexArray> VertexArraysToDraw = new List<VertexArray>();

            foreach (EventViewModel e in ListOfEvents)
            {
                // Set X (horizontal center of event)
                double x = OffsetX + 
                    Zoom * ( 
                    IntervalLengthPx * e.StartYear                  // Year
                    + IntervalLengthPx/12 * (e.StartMonth - 1)      // Month
                    + IntervalLengthPx/365 * e.StartDay             // Day
                    );

                // Create Text
                Text text = new Text(e.Name, PrimaryFont);
                text.CharacterSize = EventTextCharacterSize;
                text.FillColor = Theme.TextColor;

                // Initialize some variables
                float eventHeight = 40;
                float eventBgMargin = 5;
                float EventFromLineHeight = 60;
                float MinEventWidth = 60;
                float circleBorderRadius = eventHeight / 2;

                float eventLeftPos = (float)(x - text.GetLocalBounds().Width / 2 - eventBgMargin - circleBorderRadius);
                float eventRightPos = (float)(x + text.GetLocalBounds().Width / 2 + eventBgMargin + circleBorderRadius);

                e.Size = new Vector2f(text.GetLocalBounds().Width + eventBgMargin * 2 + circleBorderRadius * 2, eventHeight);

                // Set Level
                int level = 1;

                if (e != ListOfEvents.First())
                {
                    while(eventLeftPos < PrevRightSideX[level])
                    {
                        level++;

                        if (level >= PrevRightSideX.Count) break;
                    }
                }

                // Set ScreenPos of Event (this includes borders)
                e.ScreenPos = new Vector2f(eventLeftPos, LineTopY - (level * EventFromLineHeight) - eventHeight);

                // Set Text Position
                text.Position = new Vector2f((float)x - text.GetLocalBounds().Width / 2, e.ScreenPos.Y + eventBgMargin);

                // Change color of background on hover or while editing
                Color bgColor = (e.IsMouseOver(window) || e == EditingEvent) ? Theme.EventHoverColor : Theme.EventBackgroundColor;

                // Background Rectangle
                RectangleShape textBg = new RectangleShape() {
                    Position = new Vector2f(text.Position.X - eventBgMargin, e.ScreenPos.Y),
                    Size = new Vector2f(text.GetLocalBounds().Width + eventBgMargin * 2, eventHeight),
                    FillColor = bgColor
                };
                ShapesToDraw.Add(textBg);

                // Background Circular Borders
                CircleShape lCircle = new CircleShape() {
                    Radius = circleBorderRadius,
                    Position = new Vector2f(eventLeftPos, e.ScreenPos.Y),
                    FillColor = bgColor
                };
                ShapesToDraw.Add(lCircle);

                CircleShape rCircle = new CircleShape() {
                    Radius = circleBorderRadius,
                    Position = new Vector2f(eventRightPos - circleBorderRadius * 2, e.ScreenPos.Y),
                    FillColor = bgColor
                };
                ShapesToDraw.Add(rCircle);

                // Set PrevX
                float prevx = e.ScreenPos.X + e.Size.X;

                if (level >= PrevRightSideX.Count)
                    PrevRightSideX.Add(prevx);
                else
                    PrevRightSideX[level] = prevx;


                // Background Triangle
                VertexArray bgTriangle = new VertexArray(PrimitiveType.Triangles, 3);
                bgTriangle[0] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 - 8, textBg.Position.Y + textBg.Size.Y), bgColor);
                bgTriangle[1] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2 + 8, textBg.Position.Y + textBg.Size.Y), bgColor);
                bgTriangle[2] = new Vertex(new Vector2f(text.Position.X + text.GetLocalBounds().Width / 2, textBg.Position.Y + textBg.Size.Y + 8), bgColor);
                VertexArraysToDraw.Add(bgTriangle);
                
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
