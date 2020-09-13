using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;

namespace TimelineLib.Themes
{
    public abstract class Theme
    {
        public Color BackgroundColor { get; set; }
        public Color EventBackgroundColor { get; set; }
        public Color LineColor { get; set; }
        public Color TextColor { get; set; }

    }

    //Beige Theme
    public class ThemeBeige : Theme
    {
        public ThemeBeige()
        {
            BackgroundColor = new Color(255, 253, 244);
            EventBackgroundColor = new Color(230, 230, 230);
            LineColor = Color.Black;
            TextColor = Color.Black;
        }
    }

    //Blue Theme
    public class ThemeBlue : Theme
    {
        public ThemeBlue()
        {
            BackgroundColor = new Color(66, 136, 183);
            EventBackgroundColor = new Color(79, 160, 214);
            LineColor = Color.White;
            TextColor = Color.White;
        }
    }
    
    //Green Theme
    public class ThemeGreen : Theme
    {
        public ThemeGreen()
        {
            BackgroundColor = new Color(88, 124, 92);
            EventBackgroundColor = new Color(105, 168, 92);
            LineColor = new Color(214, 150, 94);
            TextColor = new Color(232, 220, 211);
        }
    }
}
