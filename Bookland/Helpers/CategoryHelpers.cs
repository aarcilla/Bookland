using Bookland.Data_Structures;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Helpers
{
    public class CategoryHelpers : Abstract.ICategoryHelpers
    {
        /// <summary>
        /// Generate a list of all stored Categories, to be used to select a Category's parent (through a drop-down list).
        /// </summary>
        /// <param name="categoryRepo">An existing instance of the Category repository.</param>
        /// <param name="selectedCategoryID">The ID of the Category that is to be pre-selected for the drop-down list.</param>
        /// <param name="excludedCategory">If applicable, the category that is being updated, to ensure that it's excluded from the list.</param>
        /// <returns>A SelectListItem list of all stored categories, with tree-like formatting.</returns>
        public List<SelectListItem> ParentCategoryOptions(TreeNode<Category> categoryTree, int selectedCategoryID, params Category[] excludedCategories)
        {
            if (categoryTree == null)
            {
                throw new ArgumentNullException("categoryTree", "categoryTree cannot be null.");
            }

            // If there are categories to be updated (using the list this method returns), exclude those categories and its descendant categories
            // from being parent selections, in order to prevent invalid circular relationships
            // (e.g. Films -> Action | Update Film so that its parent is Action | Invalid circular relationship outside the scope of the main tree)
            foreach (Category cat in excludedCategories)
            {
                categoryTree.DeleteNode(cat);
            }

            List<SelectListItem> categories = new List<SelectListItem>();

            List<Category> categoriesOrdered = categoryTree.ToList();
            foreach (Category cat in categoriesOrdered)
            {
                // Prepare whitespace (unicode) in front of category names for display in drop-down list, to denote parent-child relationships.
                // Amount of whitespace is based on category's tree level.
                string whiteSpace = "";
                for (int i = 0; i < cat.CategoryLevel; i++)
                {
                    whiteSpace += "\u00a0\u00a0\u00a0";
                }

                categories.Add(new SelectListItem
                {
                    Text = whiteSpace + cat.CategoryName,
                    Value = cat.CategoryID.ToString(),
                    Selected = (cat.CategoryID == selectedCategoryID)
                });
            }

            return categories;
        }
    }
}