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
        public int? CategoryID { get; set; }
        public Category Category { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int StartDay { get; set; }
        public int StartHour { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public int EndDay { get; set; }
        public int EndHour { get; set; }
        public string Note { get; set; }

        public Vector2f ScreenPos { get; set; }
        public Vector2f Size { get; set; }

        public double X { get; set; }

        public EventViewModel()
        {

        }

        public EventViewModel SetViewModel(EventModel e)
        {
            this.Name = e.Name;
            this.StartYear = e.StartYear;
            this.StartMonth = e.StartMonth;
            this.StartDay = e.StartDay;
            this.StartHour = e.StartHour;
            this.EndYear = e.EndYear;
            this.EndMonth = e.EndMonth;
            this.EndDay = e.EndDay;
            this.EndHour = e.EndHour;
            this.CategoryID = e.CategoryID;
            this.Note = e.Note;

            return this;
        }

        public EventModel ConvertToSaveModel()
        {
            EventModel e = new EventModel();

            e.Name = this.Name;
            e.StartYear = this.StartYear;
            e.StartMonth = this.StartMonth;
            e.StartDay = this.StartDay;
            e.StartHour = this.StartHour;
            e.EndYear = this.EndYear;
            e.EndMonth = this.EndMonth;
            e.EndDay = this.EndDay;
            e.EndHour = this.EndHour;
            e.CategoryID = this.Category?.ID;
            e.Note = this.Note;

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
