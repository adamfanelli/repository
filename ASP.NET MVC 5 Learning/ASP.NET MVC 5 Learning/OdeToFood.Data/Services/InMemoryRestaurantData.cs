using System;
using System.Collections.Generic;
using OdeToFood.Data.Models;
using System.Linq;

namespace OdeToFood.Data.Services
{
    public class InMemoryRestaurantData : IRestaurantData
    {
        List<Restaurant> restaurants;
        public int numColumns;

        public InMemoryRestaurantData()
        {
            numColumns = 4;

            restaurants = new List<Restaurant>()
            {
                new Restaurant { Id = 1, Name = "Belagio's" },
                new Restaurant { Id = 2, Name = "Red Robin" },
                new Restaurant { Id = 3, Name = "Buscuit's Cafe" },
                new Restaurant { Id = 4, Name = "Wong's Chinese" },
                new Restaurant { Id = 5, Name = "Panera Bread" },
                new Restaurant { Id = 6, Name = "Oswego Grill" },
                new Restaurant { Id = 7, Name = "Chipotle" },
                new Restaurant { Id = 8, Name = "Chipotle" },
                new Restaurant { Id = 9, Name = "Chipotle" },
                new Restaurant { Id = 10, Name = "Chipotle" }
            };
        }

        public IEnumerable<Restaurant> GetAll()
        {
            return restaurants;
        }
    }
}
