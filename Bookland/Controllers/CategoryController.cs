using Bookland.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bookland.Helpers;
using Bookland.Data_Structures;
using Bookland.Models;

namespace Bookland.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryRepository categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }

        public PartialViewResult Menu()
        {
            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();

            return PartialView(categoryTree);
        }

    }
}
