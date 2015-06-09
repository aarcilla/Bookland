using Bookland.Data_Structures;
using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Helpers.Abstract
{
    public interface ICategoryHelpers
    {
        /// <summary>
        /// Generate a list of all stored Categories, to be used to select a Category's parent (through a drop-down list).
        /// </summary>
        /// <param name="categoryRepo">An existing instance of the Category repository.</param>
        /// <param name="selectedCategoryID">The ID of the Category that is to be pre-selected for the drop-down list.</param>
        /// <param name="excludedCategory">If applicable, the category that is being updated, to ensure that it's excluded from the list.</param>
        /// <returns>A SelectListItem list of all stored categories, with tree-like formatting.</returns>
        List<SelectListItem> ParentCategoryOptions(TreeNode<Category> categoryTree, int selectedCategoryID, params Category[] excludedCategories);
    }
}
