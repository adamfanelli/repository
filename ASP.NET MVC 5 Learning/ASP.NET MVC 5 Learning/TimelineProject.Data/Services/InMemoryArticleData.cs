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
                new Article { Title = "First Article", Description = "Description of the first article"},
                new Article { Title = "Second Article", Description = "Description of the second article"}
            };
        }

        public IEnumerable<Article> GetArticles()
        {
            return articles;
        }
    }
}
