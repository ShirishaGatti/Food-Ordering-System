using ItemListModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListModel
{
    public class Item
    {

        public int ItemId { get; set; }

        //public int I

        public decimal price { get; set; }
        public FoodCategory FoodCategoryId { get; set; }
        public string description { get; set; }
        public string ItemName { get; set; }
        public Restaurant RestaurantId { get; set; }
        public DbAdmin DbAdmin { get; set; }
    }
}
