using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListModel.ViewModel
{
    public class EditOrderViewModel
    {
        public OrderViewModel Order { get; set; }
        public List<OrderItemViewModel> AllItems { get; set; }

        public EditOrderViewModel()
        {
            Order = new OrderViewModel();
            AllItems = new List<OrderItemViewModel>();
        }
    }
}
