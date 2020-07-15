using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimelineProject.Web.Controllers
{
    public class TimelineController : Controller
    {
        // GET: Timeline
        public ActionResult Index()
        {
            return View();
        }
    }
}