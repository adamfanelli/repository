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

        public Vector2f ScreenPos { get; set; }
        public Vector2f Size { get; set; }

        public EventViewModel(string name = "", int startYear = 0)
        {
            Name = name;
            StartYear = startYear;
        }

        public void SetViewModel(EventModel e)
        {
            this.Name = e.Name;
            this.StartYear = e.StartYear;
        }

        public EventModel ConvertToSaveModel()
        {
            EventModel e = new EventModel();

            e.Name = this.Name;
            e.StartYear = this.StartYear;

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
