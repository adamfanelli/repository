using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimelineProject.Data.Services;

namespace TimelineProject.Web.Controllers
{
    public class ArticlesController : Controller
    {
        InMemoryArticleData data;

        // GET: Articles
        public ActionResult Index()
        {
            var model = new InMemoryArticleData().GetArticles();
            return View(model);
        }
    }
}