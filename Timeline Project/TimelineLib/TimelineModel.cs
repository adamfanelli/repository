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
        public List<EventModel> EventModels { get; set; }
        public int ThemeID { get; set; }
        public string Title { get; set; }
    }
}
