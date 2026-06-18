/*using ItemListDal;
using ItemListModel.ViewModel;
using System.Web.Mvc;
using Newtonsoft.Json;
 
namespace ItemListTask1.Controllers
{
    public class ItemController : Controller
    {
        private const string FilterStateSessionKey = "ItemFilterState";
 
        public ActionResult ItemList(ItemViewModel vm)
        {
            ItemRepository itemRepository = new ItemRepository();
            vm.FoodCategory = itemRepository.GetFilterValues();
            return View(vm);
        }
 
        public ActionResult GetItems(ItemViewModel itemViewModel)
        {
            ItemRepository itemRepository = new ItemRepository();
            ItemViewModel vm = itemRepository.GetList(itemViewModel);
            return PartialView("_ItemTable", vm);
        }
 
        [HttpPost]
        public ActionResult SaveItem(ItemViewModel itemViewModel)
        {
            ItemRepository itemRepository = new ItemRepository();
            bool result = itemRepository.SaveItem(itemViewModel);
            if (result)
            {
                // var filterState = GetFilterStateFromSession();
 
                Session["pageNumber"] = itemViewModel.pageNumber;
                Session["pageSize"] = itemViewModel.pageSize;
                Session["Satus"] = itemViewModel.status;
                Session["SelectedCategoryId"] = itemViewModel.SelectedCategoryId;
                Session["RestaurantName"] = itemViewModel.RestaurantName;
                Session["FoodCategory"] = itemViewModel.FoodCategory;
                return RedirectToAction("ItemList");
            }
 
            itemViewModel.FoodCategory = itemRepository.GetFilterValues();
            return View("SaveItem", itemViewModel);
        }
 
        [HttpGet]
        public ActionResult SaveItem(int? id)
        {
            ItemRepository itemRepository = new ItemRepository();
 
            ItemViewModel vm = new ItemViewModel();
            if (id.HasValue)
            {
                vm = itemRepository.LoadItem(id.Value);
            }
            Session["RestaurantName"] = itemViewModel.RestaurantName;
            vm.SelectedCategoryId = Session["SelectedCategoryId"];
            vm.RestaurantName = Session["RestaurantName"].ToString();
            vm.pageSize = Session["pageSize"];
            vm.pageNumber = Session["pageNumber"];
            vm.status = Session["Satus"];
            vm.FoodCategory = Session["FoodCategory"];
            return View("SaveItem", vm);
        }
 
        [HttpPost]
        public ActionResult ItemsDelete(int ItemId)
        {
            ItemRepository itemRepository = new ItemRepository();
            bool result = itemRepository.ItemsDelete(ItemId);
            return Json(new { success = result });
        }
    }
}

without exception
 
using ItemListDal;
using ItemListModel.ViewModel;
using System.Web.Mvc;
 
namespace ItemListTask1.Controllers
{
    public class ItemController : Controller
    {
        private const string FilterStateSessionKey = "ItemFilterState";

        public ActionResult ItemList(ItemViewModel vm)
        {
            ItemRepository itemRepository = new ItemRepository();
            //   if (Session["FilterModel"] != null)
           //    {
            //       vm = Session["FilterModel"] as ItemViewModel;
            //   }
            vm.FoodCategory = itemRepository.GetFilterValues();

            return View(vm);
        }

        public ActionResult GetItems(ItemViewModel itemViewModel)
        {
            //  Session["FilterModel"] = itemViewModel;
            ItemRepository itemRepository = new ItemRepository();
            ItemViewModel vm = itemRepository.GetList(itemViewModel);
            return PartialView("_ItemTable", vm);
        }


        [HttpPost]
        public ActionResult SaveItem(ItemViewModel itemViewModel)
        {
            ItemRepository itemRepository = new ItemRepository();

            bool result = itemRepository.SaveItem(itemViewModel);

            if (result)
            {
                // return RedirectToAction("ItemList");
                return Json(new { success = true });

            }

            itemViewModel.FoodCategory =
                itemRepository.GetFilterValues();

            return View("_SaveItem", itemViewModel);
        }

        [HttpGet]
        public ActionResult SaveItem(int? id)
        {
            ItemRepository itemRepository = new ItemRepository();

            ItemViewModel vm = new ItemViewModel();

            if (id.HasValue)
            {
                vm = itemRepository.LoadItem(id.Value);
            }

            vm.FoodCategory = itemRepository.GetFilterValues();

            return PartialView("_SaveItem", vm);
        }
        [HttpPost]
        public ActionResult DeleteItem(int ItemId)
        {
            ItemRepository itemRepository = new ItemRepository();
            bool result = itemRepository.ItemsDelete(ItemId);
            // return RedirectToAction("ItemList");
            return Json(new { success = true });

        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ClearFilters()
        {
            Session.Remove("FilterModel");

            return Json(new { success = true });
        }
    }
}*/

// =============================================
// FILE: Controllers/ItemController.cs  (updated)
// PURPOSE: Clean API calls ONLY.
//          No try/catch — GlobalExceptionHandler handles everything.
//          No business logic — that's ItemService's job.
//          No direct DAL access — always goes through Service layer.
// =============================================
// =============================================
// FILE: ItemListTask1\Controllers\ItemController.cs  (updated)
// PROJECT: FoodOrderingSystem (MVC project)
// =============================================

using ItemListModel.ViewModel;
using ItemListTask1.Service;
using System.Web.Mvc;

namespace ItemListTask1.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;

        public ItemController()
        {
            _itemService = new ItemService();
        }

        // ── No try/catch anywhere. GlobalMvcExceptionFilter handles it all. ──

        public ActionResult ItemList(ItemViewModel vm)
        {
            vm.FoodCategory = _itemService.GetFilterValues();
            return View(vm);
        }

        public ActionResult GetItems(ItemViewModel itemViewModel)
        {
            ItemViewModel vm = _itemService.GetItems(itemViewModel);
            return PartialView("_ItemTable", vm);
        }

        [HttpGet]
        public ActionResult SaveItem(int? id)
        {
            ItemViewModel vm = id.HasValue
                ? _itemService.GetItemById(id.Value)
                : new ItemViewModel();

            vm.FoodCategory = _itemService.GetFilterValues();
            return PartialView("_SaveItem", vm);
        }

        [HttpPost]
        public ActionResult SaveItem(ItemViewModel itemViewModel)
        {
            _itemService.SaveItem(itemViewModel);
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult DeleteItem(int itemId)
        {
            _itemService.DeleteItem(itemId);
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult ClearFilters()
        {
            Session.Remove("FilterModel");
            return Json(new { success = true });
        }
    }
}


// =============================================
// FILE: App_Start\FilterConfig.cs  (updated — add ONE line)
// =============================================
/*
using ItemListTask1.GlobalHandler;
using System.Web.Mvc;

namespace ItemListTask1
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalMvcExceptionFilter());  // ← ADD THIS LINE
        }
    }
}
*/
