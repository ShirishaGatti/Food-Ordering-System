using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemListModel.ViewModel
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.Today;
    
    public int UserId { get; set; } = 1;
        public List<OrderItemViewModel> Items { get; set; }
        public List<OrderItemViewModel> AllItems { get; set; } // for dropdown

        public decimal TotalAmount
        {
            get { return Items?.Sum(x => x.Price * x.Quantity) ?? 0; }
        }

        public OrderViewModel()
        {
            Items = new List<OrderItemViewModel>();
            AllItems = new List<OrderItemViewModel>();
        }
    }
}