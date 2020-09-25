using System;
using System.Collections.Generic;
using System.Text;

namespace TimelineLib
{
    public class EventModel
    {
        public string Name { get; set; }
        public int? CategoryID { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }
        public int StartHour { get; set; }
        public int? EndYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndDay { get; set; }
        public int? EndHour { get; set; }
        public string Note { get; set; }
    }
}
