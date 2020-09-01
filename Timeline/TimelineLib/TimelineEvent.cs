using System;
using System.Collections.Generic;
using System.Text;

namespace TimelineLib
{
    public class TimelineEvent
    {
        public string Name { get; set; }
        public float StartYear { get; set; }

        public TimelineEvent(string name, float startYear)
        {
            Name = name;
            StartYear = startYear;
        }
    }
}
