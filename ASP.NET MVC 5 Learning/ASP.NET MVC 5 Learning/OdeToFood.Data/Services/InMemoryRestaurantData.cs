using System;
using System.Collections.Generic;
using OdeToFood.Data.Models;
using System.Linq;

namespace OdeToFood.Data.Services
{
    public class InMemoryRestaurantData : IRestaurantData
    {
        List<Restaurant> restaurants;

        public InMemoryRestaurantData()
        {
            restaurants = new List<Restaurant>()
            {
                new Restaurant { Id = 1, Name = "Belagio's" },
                new Restaurant { Id = 1, Name = "Red Robin" },
                new Restaurant { Id = 2, Name = "Buscuit's Cafe" },
                new Restaurant { Id = 1, Name = "Belagio's" },
                new Restaurant { Id = 1, Name = "Red Robin" },
                new Restaurant { Id = 2, Name = "Buscuit's Cafe" }
            };
        }

        public IEnumerable<Restaurant> GetAll()
        {
            return restaurants.OrderBy(r => r.Name);
        }
    }
}
