using Bookland.DAL.Abstract;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    public class SearchController : Controller
    {
        IProductRepository productRepo;

        public SearchController(IProductRepository productRepo)
        {
            this.productRepo = productRepo;
        }

        public ActionResult Index(string searchQuery)
        {
            

            return View();
        }
    }
}
