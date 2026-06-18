using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ItemListDal;
using ItemListModel;
using ItemListModel.ViewModel;

namespace ItemListTask1.Controllers
{
    public class HomeController : Controller
    {
        ItemRepository itemRepository;
          
        public HomeController()
        {
            itemRepository = new ItemRepository(); 
        }
        public ActionResult Index()
        {
             return View(); 
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}