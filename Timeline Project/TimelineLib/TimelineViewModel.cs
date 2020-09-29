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
        // DEBUG
        public int Debug_NoRefreshes { get; set; }

        // GENERAL DATA
        public string Title { get; set; }
        public Theme Theme { get; set; }

        public RectangleShape Line { get; set; }
        public int LineThickness { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float LineTopY { get; set; }
        public Font PrimaryFont { get; set; }
        public Font SecondaryFont { get; set; }

        public double Zoom { get; set; }
        public float ZoomSpeed { get; set; }
        public int ScrollCount { get; set; }
        public int ZoomMinCap { get; set; }
        public int ZoomMaxCap { get; set; }
        public float ScrollSpeed { get; set; }
        public int YearAtMouse { get; set; }

        public float IntervalLengthPx { get; set; }
        public float IntervalThresholdPx { get; set; }
        public float EventFromLineHeight { get; set; }
        public float MinEventWidth { get; set; }
        public float MarkerHeight { get; set; }
        public float EventBgMargin { get; set; }
        public uint EventTextCharacterSize { get; set; }
        public uint MarkerCharacterSize { get; set; }
        public uint MarkerHighlightedCharacterSize { get; set; }

        public EventViewModel EventToDrawNote { get; set; }
        public EventViewModel EditingEvent { get; set; }

        public float DisplayNoteDelayInSeconds { get; set; }

        // DATA CONTEXT VARIABLES
        private bool isSideColumnVisible;
        public bool IsSideColumnVisible
        {
            get { return isSideColumnVisible; }
            set { isSideColumnVisible = value; NotifyPropertyChanged(); }
        }

        private string editingEventName;
        public string EditingEventName
        {
            get { return editingEventName; }
            set { editingEventName = value; NotifyPropertyChanged(); }
        }

        private int editingEventStartYear;
        public int EditingEventStartYear
        {
            get { return editingEventStartYear; }
            set { editingEventStartYear = value; NotifyPropertyChanged(); }
        }

        private int editingEventStartMonth;
        public int EditingEventStartMonth
        {
            get { return editingEventStartMonth; }
            set { editingEventStartMonth = value; NotifyPropertyChanged(); }
        }

        private int editingEventStartDay;
        public int EditingEventStartDay
        {
            get { return editingEventStartDay; }
            set { editingEventStartDay = value; NotifyPropertyChanged(); }
        }
        
        private int? editingEventEndYear;
        public int? EditingEventEndYear
        {
            get { return editingEventEndYear; }
            set { editingEventEndYear = value; NotifyPropertyChanged(); }
        }

        private int? editingEventEndMonth;
        public int? EditingEventEndMonth
        {
            get { return editingEventEndMonth; }
            set { editingEventEndMonth = value; NotifyPropertyChanged(); }
        }

        private int? editingEventEndDay;
        public int? EditingEventEndDay
        {
            get { return editingEventEndDay; }
            set { editingEventEndDay = value; NotifyPropertyChanged(); }
        }

        private string editingEventNote;
        public string EditingEventNote
        {
            get { return editingEventNote; }
            set { editingEventNote = value; NotifyPropertyChanged(); }
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

        public List<EventViewModel> ListOfEvents { get; set; }
        public List<Category> ListOfCategories { get; set; }

        // Property Changed Event Handler
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TimelineViewModel()
        {
            Line = new RectangleShape();
            ListOfEvents = new List<EventViewModel>();
            ListOfCategories = new List<Category>();
            EditingEvent = null;
            EventToDrawNote = null;

            // Default Values
            LineThickness = 4;
            Zoom = 1.0f;
            ZoomSpeed = 10.0f;
            ZoomMinCap = 40;
            ZoomMaxCap = -132;
            ScrollSpeed = 5f;
            EventTextCharacterSize = 24;
            EventBgMargin = 5;
            MarkerCharacterSize = 15;
            MarkerHighlightedCharacterSize = 18;
            IntervalLengthPx = 60;
            IntervalThresholdPx = 60;
            MarkerHeight = 14;
            DisplayNoteDelayInSeconds = 0.75f;

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

            foreach(EventViewModel e in this.ListOfEvents)
            {
                t.EventModels.Add(e.ConvertToSaveModel());
            }

            t.ThemeID = this.Theme.ID;
            t.Title = this.Title;

            return t;
        }



        public void AddEvent(EventViewModel e)
        {
            e.Category = ListOfCategories.Find(x => x.ID == e.CategoryID);

            ListOfEvents.Add(e);
            ListOfEvents = ListOfEvents.OrderBy(x => x.StartYear).ToList();
        }

        public void AddCategory(Category c)
        {
            ListOfCategories.Add(c);
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
        private List<int> MonthIntervals = new List<int>() { 1, 3, 5, 11 };
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

            Text NoRefreshesText = new Text("Refresh Count: " + Debug_NoRefreshes.ToString(), SecondaryFont);
            NoRefreshesText.Position = new Vector2f(20, 80);
            NoRefreshesText.FillColor = Theme.TitleColor;
            window.Draw(NoRefreshesText);
        }

        public void DrawEvents(RenderWindow window)
        {
            // An array of the Previous X for each level
            List<float> PrevRightPosTop = new List<float>() { 0 };
            List<float> PrevRightPosBottom = new List<float>() { 0 };

            // Items in these lists will be drawn at the end of each loop
            List<Shape> ShapesToDraw = new List<Shape>();
            List<Text> TextToDraw = new List<Text>();
            List<VertexArray> VertexArraysToDraw = new List<VertexArray>();



            // Initialize some variables
            float eventHeight = 40;
            float EventFromLineHeight = 60;
            float MinEventWidth = 60;
            float circleBorderRadius = eventHeight / 2;

            foreach (EventViewModel e in ListOfEvents)
            {

                // If the event has an EndYear, draw a timespan. Otherwise, draw an event.
                if (e.EndYear != null)
                {
                    double leftX = OffsetX +
                        Zoom * (
                        IntervalLengthPx * e.StartYear              // Year
                        + IntervalLengthPx / 12 * e.StartMonth        // Month
                        + IntervalLengthPx / 365 * e.StartDay         // Day
                        );
                    
                    double rightX = OffsetX +
                        Zoom * (
                        IntervalLengthPx * e.EndYear ?? 0            // Year
                        + IntervalLengthPx / 12 * e.EndMonth ?? 0        // Month
                        + IntervalLengthPx / 365 * e.EndDay ?? 0        // Day
                        );

                    // Set Level
                    int level = 1;

                    if (PrevRightPosBottom.Count > 1)
                    {
                        while (leftX < PrevRightPosBottom[level])
                        {
                            level++;

                            if (level >= PrevRightPosBottom.Count) break;
                        }
                    }

                    // Set Screen Position and Size
                    e.ScreenPos = new Vector2f((float)leftX, LineTopY + LineThickness + (level * EventFromLineHeight));
                    e.Size = new Vector2f ((float)(rightX - leftX), eventHeight / 2);

                    // Change color of background on hover or while editing
                    Color bgColor =
                        (e.IsMouseOver(window) || e == EditingEvent)
                        ? e.Category == null ? Theme.EventHoverColor : e.Category.BackgroundColorHover
                        : e.Category == null ? Theme.EventBackgroundColor : e.Category.BackgroundColor;

                    // Background Rectangle
                    RectangleShape textBg = new RectangleShape()
                    {
                        Position = e.ScreenPos,
                        Size = e.Size,
                        FillColor = bgColor
                    };
                    ShapesToDraw.Add(textBg);

                    // Create Text
                    Text text = new Text(e.Name, PrimaryFont);
                    text.CharacterSize = EventTextCharacterSize - 5;
                    text.FillColor = Theme.TextColor;
                    text.Position = new Vector2f(e.ScreenPos.X + 10, e.ScreenPos.Y - 20);
                    TextToDraw.Add(text);

                    // Set PrevX
                    float prevx = e.ScreenPos.X + e.Size.X;

                    if (level >= PrevRightPosBottom.Count)
                        PrevRightPosBottom.Add(prevx);
                    else
                        PrevRightPosBottom[level] = prevx;
                }
                else
                {
                    // Set X (horizontal center of event)
                    e.X = OffsetX +
                        Zoom * (
                        IntervalLengthPx * e.StartYear              // Year
                        + IntervalLengthPx / 12 * e.StartMonth        // Month
                        + IntervalLengthPx / 365 * e.StartDay         // Day
                        );

                    // Create Text
                    Text text = new Text(e.Name, PrimaryFont);
                    text.CharacterSize = EventTextCharacterSize;
                    text.FillColor = Theme.TextColor;

                    float eventLeftPos = (float)(e.X - text.GetLocalBounds().Width / 2 - EventBgMargin - circleBorderRadius);
                    float eventRightPos = (float)(e.X + text.GetLocalBounds().Width / 2 + EventBgMargin + circleBorderRadius);

                    e.Size = new Vector2f(text.GetLocalBounds().Width + EventBgMargin * 2 + circleBorderRadius * 2, eventHeight);

                    // Set Level
                    int level = 1;

                    if (PrevRightPosTop.Count > 1)
                    {
                        while (eventLeftPos < PrevRightPosTop[level])
                        {
                            level++;

                            if (level >= PrevRightPosTop.Count) break;
                        }
                    }

                    // Set ScreenPos of Event (this includes borders)
                    e.ScreenPos = new Vector2f(eventLeftPos, LineTopY - (level * EventFromLineHeight) - eventHeight);

                    // Set Text Position
                    text.Position = new Vector2f((float)e.X - text.GetLocalBounds().Width / 2, e.ScreenPos.Y + EventBgMargin);

                    // Change color of background on hover or while editing
                    Color bgColor =
                        (e.IsMouseOver(window) || e == EditingEvent)
                        ? e.Category == null ? Theme.EventHoverColor : e.Category.BackgroundColorHover
                        : e.Category == null ? Theme.EventBackgroundColor : e.Category.BackgroundColor;

                    // Background Rectangle
                    RectangleShape textBg = new RectangleShape()
                    {
                        Position = new Vector2f(text.Position.X - EventBgMargin, e.ScreenPos.Y),
                        Size = new Vector2f(text.GetLocalBounds().Width + EventBgMargin * 2, eventHeight),
                        FillColor = bgColor
                    };
                    ShapesToDraw.Add(textBg);

                    // Background Circular Borders
                    CircleShape lCircle = new CircleShape()
                    {
                        Radius = circleBorderRadius,
                        Position = new Vector2f(eventLeftPos, e.ScreenPos.Y),
                        FillColor = bgColor
                    };
                    ShapesToDraw.Add(lCircle);

                    CircleShape rCircle = new CircleShape()
                    {
                        Radius = circleBorderRadius,
                        Position = new Vector2f(eventRightPos - circleBorderRadius * 2, e.ScreenPos.Y),
                        FillColor = bgColor
                    };
                    ShapesToDraw.Add(rCircle);

                    // Set PrevX
                    float prevx = e.ScreenPos.X + e.Size.X;

                    if (level >= PrevRightPosTop.Count)
                        PrevRightPosTop.Add(prevx);
                    else
                        PrevRightPosTop[level] = prevx;


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
                    for (int i = 0; i < noDashes * level; i++)
                    {
                        RectangleShape dash = new RectangleShape();
                        dash.FillColor = Theme.TextColor;

                        float bottomOfTextBox = LineTopY - EventFromLineHeight * level;

                        dash.Position = new Vector2f(
                            text.Position.X + text.GetLocalBounds().Width / 2 - dashThickness / 2,

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
            }

            foreach (Shape shape in ShapesToDraw)
                window.Draw(shape);
            
            foreach(VertexArray vertexArray in VertexArraysToDraw)
                window.Draw(vertexArray);
            
            foreach(Text text in TextToDraw)
                window.Draw(text);

            // Draw Note
            if(EventToDrawNote != null)
            {
                Text noteText = new Text(EventToDrawNote.Note, PrimaryFont);
                noteText.CharacterSize = EventTextCharacterSize;
                noteText.FillColor = Theme.TextColor;
                noteText.Position = new Vector2f((float)EventToDrawNote.X - noteText.GetLocalBounds().Width / 2, EventToDrawNote.ScreenPos.Y - 60);

                RectangleShape noteBg = new RectangleShape();
                noteBg.Position = noteText.Position - new Vector2f(EventBgMargin, EventBgMargin);
                noteBg.Size = new Vector2f(noteText.GetLocalBounds().Width + EventBgMargin * 2, noteText.GetLocalBounds().Height + EventBgMargin * 4);
                noteBg.FillColor = Theme.EventNoteBackgroundColor;

                CircleShape lCircle = new CircleShape()
                {
                    Radius = noteBg.Size.Y / 2,
                    Position = noteBg.Position - new Vector2f(noteBg.Size.Y / 2, 0),
                    FillColor = Theme.EventNoteBackgroundColor
                };

                CircleShape rCircle = new CircleShape()
                {
                    Radius = noteBg.Size.Y / 2,
                    Position = lCircle.Position + new Vector2f(noteBg.Size.X, 0),
                    FillColor = Theme.EventNoteBackgroundColor
                };

                window.Draw(noteBg);
                window.Draw(lCircle);
                window.Draw(rCircle);
                window.Draw(noteText);

            }
        }
    }
}
