using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Linq;

namespace TimelineProject.Data.Models
{

    public class Article
    {
        public Article()
        {
            string rootFolder = @"D:\dev\repository\ASP.NET MVC 5 Learning\ASP.NET MVC 5 Learning";
            this.Description = File.ReadLines(rootFolder + @"\TimelineProject.Data\Models\lorem_ipsum.txt").First();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }

        public string Type { get; set; }
    }
}
