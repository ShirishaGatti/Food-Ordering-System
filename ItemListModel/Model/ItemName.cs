using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListModel.Model
{
    public class ItemName
    {
        public int ItemNameId { get; set; }
        public string Item_Name { get; set; }
        public FoodCategory FoodCategoryId { get; set; }

    }
}
