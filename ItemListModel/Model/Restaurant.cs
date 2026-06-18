using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListModel.Model
{
    public class Restaurant
    {

        public int RestaurantId { get; set; }

        public string RestaurantName { get; set; }

        public string contact { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public int? TablesAvailable { get; set; }

        public string address { get; set; }
    }
}
