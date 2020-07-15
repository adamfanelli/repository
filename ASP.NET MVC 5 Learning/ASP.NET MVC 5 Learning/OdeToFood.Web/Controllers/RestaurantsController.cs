using OdeToFood.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OdeToFood.Web.Controllers
{
    public class RestaurantsController : Controller
    {
        IRestaurantData db;

        public RestaurantsController()
        {
            this.db = new InMemoryRestaurantData();
        }

        // GET: Restaurants
        public ActionResult Index()
        {
            var model = db.GetAll();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = db.GetAll();
            return View(model);
        }
    }
}