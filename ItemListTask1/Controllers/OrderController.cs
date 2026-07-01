//using ItemListDal.dal;
//using ItemListModel.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.Web.Mvc;

//namespace ItemListTask1.Controllers
//{
//    public class OrderController : Controller
//    {
//        private readonly OrderRepository _repo = new OrderRepository();       
//        public ActionResult OrderList(OrderViewModel orderViewModel)
//        {
//            try
//            {
//                var orders = _repo.GetOrderList(orderViewModel);

//                if (Request.IsAjaxRequest())
//                    return PartialView("_OrderCardsContainer", orders);

//                var vm = new OrderViewModel
//                {
//                    Orders = orders,
//                    Status = orderViewModel.Status,
//                    PriceRange = orderViewModel.PriceRange,
//                    Price = orderViewModel.Price,
//                    DateFrom = orderViewModel.DateFrom,
//                    DateTo = orderViewModel.DateTo
//                };
//                return View(vm);
//            }
//            catch (Exception ex)
//            {
//                return Content("REAL ERROR: " + ex.Message + " | INNER: " + ex.InnerException?.Message);
//            }
//        }
//        // ── GET: Load edit modal (mirrors SaveItem GET) ───────────────────
//        [HttpGet]
//        public ActionResult SaveOrder(int? id)
//        {
//            OrderViewModel vm = id.HasValue
//                ? _repo.LoadOrder(id.Value)
//                : new OrderViewModel { AllItems = _repo.GetAllItems() };

//            return PartialView("_SaveOrder", vm);
//        }

//        // ── POST: Save (mirrors SaveItem POST) ────────────────────────────
//        [HttpPost]
//        public ActionResult SaveOrder(OrderViewModel model)
//        {
//            _repo.SaveOrder(model);
//            return Json(new { success = true });
//        }

//        // ── Get item price for dropdown change ────────────────────────────
//        public JsonResult GetItemPrice(int itemId)
//        {
//            decimal price = _repo.GetAllItems()
//                .Find(x => x.ItemId == itemId)?.Price ?? 0;

//            return Json(price, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public ActionResult DeleteOrder(int orderId)
//        {
//            _repo.OrderDelete(orderId);

//            return Json(new
//            {
//                success = true
//            });
//        }
//    }
//}

// FILE: ItemListTask1\Controllers\OrderController.cs

using ItemListDal.Services;
using ItemListModel.ViewModel;
using System.Web.Mvc;

namespace ItemListTask1.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderService _service = new OrderService();

        public ActionResult OrderList(OrderViewModel orderViewModel)
        {
            var vm = _service.GetOrderList(orderViewModel);

            if (Request.IsAjaxRequest())
                return PartialView("_OrderCardsContainer", vm);

            return View(vm);
        }

        [HttpGet]
        public ActionResult SaveOrder(int? id)
        {
            OrderViewModel vm = id.HasValue
                ? _service.LoadOrder(id.Value)
                : new OrderViewModel { AllItems = _service.GetAllItems() };

            return PartialView("_SaveOrder", vm);
        }

        [HttpPost]
        public ActionResult SaveOrder(OrderViewModel model)
        {
            _service.SaveOrder(model);
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult DeleteOrder(int orderId)
        {
            _service.DeleteOrder(orderId);
            return Json(new { success = true });
        }
    }
}