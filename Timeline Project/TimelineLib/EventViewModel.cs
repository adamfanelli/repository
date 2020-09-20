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

    public class EventViewModel
    {
        public string Name { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }
        public int StartHour { get; set; }

        public Vector2f ScreenPos { get; set; }
        public Vector2f Size { get; set; }

        public EventViewModel(string name = "", int startYear = 0, int startMonth = 0, int startHour = 0)
        {
            Name = name;
            StartYear = startYear;
            StartMonth = startMonth;
            StartHour = startHour;
        }

        public void SetViewModel(EventModel e)
        {
            this.Name = e.Name;
            this.StartYear = e.StartYear;
            this.StartMonth = e.StartMonth;
            this.StartDay = e.StartDay;
        }

        public EventModel ConvertToSaveModel()
        {
            EventModel e = new EventModel();

            e.Name = this.Name;
            e.StartYear = this.StartYear;
            e.StartMonth = this.StartMonth;
            e.StartDay = this.StartDay;

            return e;
        }

        public bool IsMouseOver(RenderWindow window)
        {
            return Mouse.GetPosition().X - window.Position.X > ScreenPos.X &&
                   Mouse.GetPosition().X - window.Position.X < ScreenPos.X + Size.X &&
                   Mouse.GetPosition().Y - window.Position.Y > ScreenPos.Y &&
                   Mouse.GetPosition().Y - window.Position.Y < ScreenPos.Y + Size.Y;
        }
    }
}
