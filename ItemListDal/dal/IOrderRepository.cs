using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemListModel.ViewModel;

namespace ItemListDal.dal
{
    interface IOrderRepository
    {
        List<OrderViewModel> GetOrderList();
    //    bool Load();
   //     bool Save();
        bool SaveOrder(OrderViewModel orderViewModel);
        //  bool ItemsUpdate(ItemViewModel itemViewModel);
        OrderViewModel LoadOrder(int ItemId);
        bool OrderDelete(int ItemId);
    }
}
