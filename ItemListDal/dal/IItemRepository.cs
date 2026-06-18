using ItemListModel;
using ItemListModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListDal.dal
{
    interface IItemRepository
    {
    //    List<Item> GetAllItems();
        ItemViewModel GetList(ItemViewModel itemViewModel);
         bool Load();
        bool Save();
        bool SaveItem(ItemViewModel itemViewModel);
      //  bool ItemsUpdate(ItemViewModel itemViewModel);
        ItemViewModel LoadItem(int ItemId);
        bool ItemsDelete(int ItemId);


    }
}
