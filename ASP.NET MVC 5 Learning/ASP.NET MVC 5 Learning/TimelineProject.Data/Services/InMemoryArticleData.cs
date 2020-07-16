using System;
using System.Collections.Generic;
using System.Text;
using TimelineProject.Data.Models;

namespace TimelineProject.Data.Services
{
    public class InMemoryArticleData
    {
        private List<Article> articles;

        public InMemoryArticleData()
        {
            articles = new List<Article>()
            {
                new Article { Type = "Character", Title = "Character Article"},
                new Article { Type = "World", Title = "World Article"},
                new Article { Type = "Other", Title = "Other Article"},
                new Article { Type = "Character", Title = "Character Article"},
                new Article { Type = "World", Title = "World Article"},
                new Article { Type = "Other", Title = "Other Article"},
                new Article { Type = "Character", Title = "Character Article"},
                new Article { Type = "World", Title = "World Article"},
                new Article { Type = "Other", Title = "Other Article"}
            };
        }

        public IEnumerable<Article> GetArticles()
        {
            return articles;
        }
    }
}
