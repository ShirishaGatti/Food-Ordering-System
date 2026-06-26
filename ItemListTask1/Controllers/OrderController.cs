//using ItemListDal.dal;
//using ItemListModel.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ItemListTask1.Controllers
//{
//    public class OrderController : Controller
//    {
//        public ActionResult OrderList()
//        {
//            OrderRepository dal = new OrderRepository();

//            List<OrderViewModel> model = dal.GetOrderList();

//            return View(model);
//        }
//    }
////}

using ItemListDal.dal;
using ItemListModel.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ItemListTask1.Controllers
{
    public class OrderController : Controller
    {
        private readonly OrderRepository _repo = new OrderRepository();

        // ── Main list page ────────────────────────────────────────────────
        public ActionResult OrderList()
        {
            List<OrderViewModel> model = _repo.GetOrderList();
            return View(model);
        }

        // ── GET: Load edit modal (mirrors SaveItem GET) ───────────────────
        [HttpGet]
        public ActionResult SaveOrder(int? id)
        {
            OrderViewModel vm = id.HasValue
                ? _repo.LoadOrder(id.Value)
                : new OrderViewModel { AllItems = _repo.GetAllItems() };

            return PartialView("_SaveOrder", vm);
        }

        // ── POST: Save (mirrors SaveItem POST) ────────────────────────────
        [HttpPost]
        public ActionResult SaveOrder(OrderViewModel model)
        {
            _repo.SaveOrder(model);
            return Json(new { success = true });
        }

        // ── Get item price for dropdown change ────────────────────────────
        public JsonResult GetItemPrice(int itemId)
        {
            decimal price = _repo.GetAllItems()
                .Find(x => x.ItemId == itemId)?.Price ?? 0;

            return Json(price, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteOrder(int orderId)
        {
            _repo.OrderDelete(orderId);

            return Json(new
            {
                success = true
            });
        }
    }
}