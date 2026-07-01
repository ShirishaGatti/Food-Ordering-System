using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemListModel.ViewModel
{
    public class OrderViewModel
    {
             public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
            public int Status { get; set; } = 10;
        public string PriceRange { get; set; } = ">=";
        public decimal? Price { get; set; } = 0;
            public DateTime? DateFrom { get; set; }= DateTime.Now.AddMonths(-1);
        public DateTime? DateTo { get; set; }= DateTime.Now;
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.Today;    
        public int UserId { get; set; } = 1;
        public List<OrderItemViewModel> Items { get; set; }
        public List<OrderItemViewModel> AllItems { get; set; } // for dropdown
        public bool? IsActive { get; set; }
        //  public int status { get; set; } = 10;   // 10 = Both (default)
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 2;
        public int ItemTotalCount { get; set; }
        public int ItemTotalPages { get; set; }
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