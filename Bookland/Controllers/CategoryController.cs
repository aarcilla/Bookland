using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Models;
using System.Web.Mvc;

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
