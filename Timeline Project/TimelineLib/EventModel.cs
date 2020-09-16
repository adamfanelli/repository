using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace TimelineLib
{
    //public enum EventType
    //{
    //    Default,

    //}

    public class EventModel
    {
        public string Name { get; set; }
        public float StartYear { get; set; }

        public EventModel(string name, float startYear)
        {
            Name = name;
            StartYear = startYear;
        }
    }
}
