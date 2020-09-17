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
        public Color EventHoverColor { get; set; }
        public Color LineColor { get; set; }
        public Color TextColor { get; set; }
        public Color TitleColor { get; set; }
        public int ID { get; set; }

        public static Theme GetThemeByID(int ID)
        {
            switch(ID)
            {
                case 0: return new ThemeBeige();
                case 1: return new ThemeBlue();
                case 2: return new ThemeGreen();
                case 3: return new ThemeDark();

                default: return new ThemeBeige();
            }
        }

        public static Theme GetThemeByName(string name)
        {
            switch (name)
            {
                case "Beige": return new ThemeBeige();
                case "Blue": return new ThemeBlue();
                case "Green": return new ThemeGreen();
                case "Dark": return new ThemeDark();

                default: return new ThemeBeige();
            }
        }
    }

    //Beige Theme
    public class ThemeBeige : Theme
    {
        public ThemeBeige()
        {
            BackgroundColor = new Color(255, 253, 244);
            EventBackgroundColor = new Color(214, 213, 205);
            EventHoverColor = EventBackgroundColor + new Color(40, 40, 40);
            LineColor = new Color(26, 26, 30);
            TextColor = new Color(26, 26, 30);
            TitleColor = TextColor;
            ID = 0;
        }
    }

    //Blue Theme
    public class ThemeBlue : Theme
    {
        public ThemeBlue()
        {
            BackgroundColor = new Color(66, 136, 183);
            EventBackgroundColor = new Color(79, 160, 214);
            EventHoverColor = EventBackgroundColor + new Color(40, 40, 40);
            LineColor = Color.White;
            TextColor = Color.White;
            TitleColor = TextColor;
            ID = 1;
        }
    }
    
    //Green Theme
    public class ThemeGreen : Theme
    {
        public ThemeGreen()
        {
            BackgroundColor = new Color(88, 124, 92);
            EventBackgroundColor = new Color(105, 168, 92);
            EventHoverColor = EventBackgroundColor + new Color(40, 40, 40);
            LineColor = new Color(214, 150, 94);
            TextColor = new Color(232, 220, 211);
            TitleColor = TextColor;
            ID = 2;
        }
    }
    
    //Dark Theme
    public class ThemeDark : Theme
    {
        public ThemeDark()
        {
            BackgroundColor = new Color(47, 47, 60);
            EventBackgroundColor = new Color(75, 75, 90);
            EventHoverColor = EventBackgroundColor + new Color(40, 40, 40);
            LineColor = new Color(201, 201, 201);
            TextColor = new Color(244, 236, 200); 
            TitleColor = TextColor;
            ID = 3;
        }
    }
}
