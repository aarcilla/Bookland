using Bookland.Areas.Admin.Models;
using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Helpers;
using Bookland.Models;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Bookland.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator, Support, Staff")]
    public class CategoryController : Controller
    {
        private ICategoryRepository categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }

        public ViewResult Index()
        {
            if (!categoryRepo.NoCategoriesExist())
            {
                TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();

                return View(categoryTree);
            }

            return View();
        }

        public JsonResult GetChildCategoriesJson(int categoryID)
        {
            var data = categoryRepo.GetChildCategories(categoryID)
                .Select(c => new
                {
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription,
                    CategoryLevel = c.CategoryLevel
                });

            return Json(new { ParentCategory = categoryID, ChildCategories = data }, 
                JsonRequestBehavior.AllowGet);
        }


        public ViewResult Create(int parentCategoryID = 1)
        {
            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();

            return View("Editor", new CategoryEditorViewModel
            {
                Category = null,
                Action = "Create",
                ParentCategoryOptions = CategoryHelpers.ParentCategoryOptions(categoryTree, parentCategoryID)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryName, CategoryDescription")]Category category, int parentCategoryID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    categoryRepo.CreateCategory(category, parentCategoryID);
                    categoryRepo.Commit();
                    TempData["message"] = String.Format("'{0}' category successfully added to the database.", category.CategoryName);

                    return RedirectToAction("Index", "Category");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("DbError", "Unable to save changes. Please contact your system admin if problems persist.");
            }

            TreeNode<Category> categoryTree = categoryRepo.GetCategoryTree();
            return View("Editor", new CategoryEditorViewModel
            {
                Category = category,
                Action = "Create",
                ParentCategoryOptions = CategoryHelpers.ParentCategoryOptions(categoryTree, parentCategoryID)
            });
        }


        public ActionResult Update(int categoryID)
        {
            Category category = categoryRepo.GetCategory(categoryID);

            if (category != null)
            {
                TreeNode<Category> categoryTreeNode = categoryRepo.GetCategoryTree();
                Category parentCategory = categoryRepo.GetParentCategory(categoryID);

                return View("Editor", new CategoryEditorViewModel
                {
                    Category = category,
                    Action = "Update",
                    ParentCategoryOptions = CategoryHelpers.ParentCategoryOptions(categoryTreeNode, parentCategory.CategoryID, category)
                });
            }
            else
            {
                string message404 = String.Format("No category exists under the ID of {0}.", categoryID);
                return HttpNotFound(message404);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "CategoryID, CategoryName, CategoryDescription")]Category category, int parentCategoryID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    categoryRepo.UpdateCategory(category, parentCategoryID);
                    categoryRepo.Commit();
                    TempData["message"] = String.Format("'{0}' category successfully updated.", category.CategoryName);

                    return RedirectToAction("Index", "Category");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("DbError", "Unable to save changes. Please contact your system admin if problems persist.");
            }

            TreeNode<Category> categoryTreeNode = categoryRepo.GetCategoryTree();
            return View("Editor", new CategoryEditorViewModel
            {
                Category = category,
                Action = "Update",
                ParentCategoryOptions = CategoryHelpers.ParentCategoryOptions(categoryTreeNode, parentCategoryID)
            });
        }


        public ActionResult Delete(int categoryID)
        {
            TreeNode<Category> category = categoryRepo.GetCategoryTree(categoryID);
            if (category != null)
            {
                return View(category);  // Confirmation
            }
            else
            {
                string message404 = String.Format("No category exists under the ID of {0}.", categoryID);
                return HttpNotFound(message404);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult Delete([Bind(Prefix = "Data", Include = "CategoryID, CategoryName")]Category category)
        {
            try
            {
                categoryRepo.DeleteCategory(category.CategoryID);
                categoryRepo.Commit();
                TempData["message"] = String.Format("'{0}' category and its descendant categories successfully deleted from the database.", category.CategoryName);

                return RedirectToAction("Index", "Category");
            }
            catch (DataException)
            {
                TempData["message"] = "Deletion was not successful. Please contact your system admin if problems persist.";
            }

            return RedirectToAction("Delete", category.CategoryID);
        }
    }
}
