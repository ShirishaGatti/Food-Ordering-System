using ItemListModel.Model;
using System.Collections.Generic;

namespace ItemListModel.ViewModel
{
    public class ItemViewModel
    {
        public int ItemId { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 20;
        public string ItemName { get; set; }

        // Filter category (used on list page filter)
        public int? SelectedCategoryId { get; set; }
        //multiple selection 
        public List<int> SelectedCategoryIds { get; set; }
        public string SelectedCategoryName { get; set; }

        // Item's own category (used on insert/update form)
        public int? ItemCategoryId { get; set; }

        public string RestaurantName { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public int status { get; set; } = 1;
        public List<FoodCategory> FoodCategory { get; set; }
        public List<Restaurant> Restaurant { get; set; }
        public int RestaurantId { get; set; }
        public List<Item> Items { get; set; }
        public int ItemTotalCount { get; set; }
        public int ItemTotalPages { get; set; }
        //sorting
        public string SortField { get; set; } = "ItemId";

        public string SortDirection { get; set; } = "ASC";
    }
}