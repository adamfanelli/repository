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
                new Article { Title = "First Article", Body = "Body of the first article"},
                new Article { Title = "Second Article", Body = "Body of the second article"}
            };
        }

        public IEnumerable<Article> GetArticles()
        {
            return articles;
        }
    }
}
