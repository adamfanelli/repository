using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace TimelineLib
{
    public class Category
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Color BackgroundColor { get; set; }
        public Color BackgroundColorHover { get; set; }
        public Color TextColor { get; set; }
        public int PriorityLevel { get; set; }
    }
}
