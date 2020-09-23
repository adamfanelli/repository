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
        public string Note { get; set; }
    }
}
