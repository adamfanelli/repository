using System;
using System.Collections.Generic;

using TimelineLib.Themes;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace TimelineLib
{
    public class TimelineModel
    {
        public List<EventModel> ListOfEvents { get; set; }

        public Theme Theme { get; set; }
        public Font PrimaryFont { get; set; }
        public Font SecondaryFont { get; set; }

        public string Title { get; set; }
    }
}
