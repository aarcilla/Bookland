using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Bookland.Helpers
{
    /// <summary>
    /// A collection of helper methods that caters to displaying Categories.
    /// </summary>
    public static class CategoryHelpers
    {
        /// <summary>
        /// Return list of categories as HTML, ordered and displayed in a tree-like format. The tags'
        /// CSS classes and structure cater to styling and DOM navigation & manipulation purposes.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="categoryNode">The root category tree node.</param>
        /// /// <param name="admin">A Boolean specifying whether the tree is for admin purposes (e.g. editing).</param>
        /// <returns>HTML list of categories in a tree-like format.</returns>
        public static MvcHtmlString DisplayCategoryTree(this HtmlHelper htmlHelper, TreeNode<Category> rootCategoryNode, bool admin = false)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            // Generate a script reference tag to JavaScript that contains click event handlers; the source script is based on the tree's intended purpose: viewing or editing/admin
            TagBuilder scriptRef = new TagBuilder("script");
            string scriptSource = admin ? "/Scripts/Bookland/treeEdit.js" : "/Scripts/Bookland/treeView.js";
            scriptRef.MergeAttribute("src", scriptSource);

            string categoryTreeHtml = scriptRef.ToString() + GenerateCategoryTreeHtml(urlHelper, rootCategoryNode, admin);
            
            return MvcHtmlString.Create(categoryTreeHtml);
        }

        /// <summary>
        /// Generate HTML for category tree, by recursively traversing through a Category-based tree data structure.
        /// </summary>
        /// <param name="urlHelper">A UrlHelper instance, used to generate action anchors.</param>
        /// <param name="categoryNode">The root category tree node.</param>
        /// <param name="admin">A Boolean specifying whether the tree is for admin purposes (e.g. editing).</param>
        /// <returns>A string containing the HTML of the category tree.</returns>
        private static string GenerateCategoryTreeHtml(UrlHelper urlHelper, TreeNode<Category> categoryNode, bool admin)
        {
            if (categoryNode == null)
            {
                throw new ArgumentNullException("categoryNode", "categoryNode cannot be null.");
            }

            Category category = categoryNode.Data;

            // Generate an anchor tag intended to toggle between hiding and showing the category's children
            TagBuilder controlAnchorTag = new TagBuilder("a");
            controlAnchorTag.MergeAttribute("href", "#");
            controlAnchorTag.AddCssClass("category-link");
            controlAnchorTag.InnerHtml = "[-]";

            List<TreeNode<Category>> childrenNodes = categoryNode.Children;
            string children = "";
            if (childrenNodes != null && childrenNodes.Count > 0)
            {
                // Retrieve tags for all descendants (children, and the children's children, and so forth)
                // through a depth-first pre-order recursive traversal (for desired order) of the category tree.
                StringBuilder childrenHtml = new StringBuilder();
                childrenNodes.ForEach(childNode => childrenHtml.Append(GenerateCategoryTreeHtml(urlHelper, childNode, admin)));

                // Surround children data within a children-denoting div tag (intended for toggling display visibility)
                TagBuilder childrenTag = new TagBuilder("div");
                childrenTag.AddCssClass("children");
                childrenTag.InnerHtml = childrenHtml.ToString();
                children = childrenTag.ToString();
            }
            else
            {
                // CSS class for hidden visibility styling of control anchor 
                // (so that they still appear on the same level as ones with children)
                controlAnchorTag.AddCssClass("no-children");
            }

            // Surround control anchor HTML and HTML of category with its action links within a parent-denoting div tag,
            // including a hover-over dialog (a.k.a. tooltip) with extra Category information
            TagBuilder parentTag = new TagBuilder("div");
            parentTag.AddCssClass("parent");
            parentTag.MergeAttribute("title", String.Format("ID: {0}\u000aDESCRIPTION: {1}\u000aLEVEL: {2}", 
                category.CategoryID, category.CategoryDescription, category.CategoryLevel));

            // Prevent root node from getting update or delete action anchors (i.e. prevent root node editing);
            // Also prevent generating action anchors if this tree isn't for admin/editing purposes
            if (admin && category.CategoryID != 1)
            {
                // Generate category-specific URLs for 'Create', 'Update' and 'Delete' action methods
                string createRoute = urlHelper.Action("Create", "Category", new { parentCategoryID = category.CategoryID });
                string updateRoute = urlHelper.Action("Update", "Category", new { categoryID = category.CategoryID });
                string deleteRoute = urlHelper.Action("Delete", "Category", new { categoryID = category.CategoryID });

                // Generate anchor tags based on the aforementioned action method URLs
                TagBuilder createAnchorTag = new TagBuilder("a");
                createAnchorTag.MergeAttribute("href", createRoute);
                createAnchorTag.AddCssClass("actions-link create");
                createAnchorTag.InnerHtml = "Add Child";

                TagBuilder updateAnchorTag = new TagBuilder("a");
                updateAnchorTag.MergeAttribute("href", updateRoute);
                updateAnchorTag.AddCssClass("actions-link update");
                updateAnchorTag.InnerHtml = "Update";

                TagBuilder deleteAnchorTag = new TagBuilder("a");
                deleteAnchorTag.MergeAttribute("href", deleteRoute);
                deleteAnchorTag.AddCssClass("actions-link delete");
                deleteAnchorTag.InnerHtml = "Delete";

                StringBuilder categoryWithActions = new StringBuilder(category.CategoryName);
                categoryWithActions.Append(createAnchorTag.ToString());
                categoryWithActions.Append(updateAnchorTag.ToString());
                categoryWithActions.Append(deleteAnchorTag.ToString());

                parentTag.InnerHtml = controlAnchorTag.ToString() + categoryWithActions;
            }
            else
            {
                parentTag.InnerHtml = controlAnchorTag.ToString() + category.CategoryName;
            }

            // Surround the current node's (parent and children) HTML within a div tag
            TagBuilder categoryTag = new TagBuilder("div");
            categoryTag.InnerHtml = parentTag.ToString() + children;

            return categoryTag.ToString();
        }

        /// <summary>
        /// Generate a list of all stored Categories, to be used to select a Category's parent (through a drop-down list).
        /// </summary>
        /// <param name="categoryRepo">An existing instance of the Category repository.</param>
        /// <param name="selectedCategoryID">The ID of the Category that is to be pre-selected for the drop-down list.</param>
        /// <param name="excludedCategory">If applicable, the category that is being updated, to ensure that it's excluded from the list.</param>
        /// <returns>A SelectListItem list of all stored categories, with tree-like formatting.</returns>
        public static List<SelectListItem> ParentCategoryOptions(TreeNode<Category> categoryTree, int selectedCategoryID, params Category[] excludedCategories)
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

        /// <summary>
        /// Return a menu of categories in HTML, formatted with unordered lists,
        /// with child categories as child lists to denote hierarchical structure.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="rootCategoryNode">The root category tree node.</param>
        /// <returns>Menu of categories in a hierarchical structure.</returns>
        public static MvcHtmlString DisplayCategoryMenu(this HtmlHelper htmlHelper, TreeNode<Category> rootCategoryNode)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string categoryMenu = GenerateCategoryListHtml(urlHelper, rootCategoryNode.Children);

            return MvcHtmlString.Create(categoryMenu);
        }

        /// <summary>
        /// Generate HTML of categories as unordered lists ("ul") in a hierarchical structure.
        /// </summary>
        /// <param name="urlHelper">A UrlHelper instance, used to generate action anchors.</param>
        /// <param name="categories">List of category nodes (including descendants) to be included in the list.</param>
        /// <returns>Unordered lists of categories in a hierarchical structure.</returns>
        private static string GenerateCategoryListHtml(UrlHelper urlHelper, List<TreeNode<Category>> categories)
        {
            StringBuilder listItemHtml = new StringBuilder();
            foreach (TreeNode<Category> catNode in categories)
            {
                Category currentCategory = catNode.Data;

                TagBuilder categoryAnchor = new TagBuilder("a");
                categoryAnchor.MergeAttribute("href", urlHelper.Action("Index", "Home", new { category = currentCategory.CategoryID }));
                categoryAnchor.InnerHtml = currentCategory.CategoryName;

                TagBuilder listItem = new TagBuilder("li");
                listItem.InnerHtml = categoryAnchor.ToString();

                List<TreeNode<Category>> childNodes = catNode.Children;
                if (childNodes.Count > 0)
                {
                    string childList = GenerateCategoryListHtml(urlHelper, childNodes);
                    listItem.InnerHtml += childList;
                }

                listItemHtml.Append(listItem.ToString());
            }

            TagBuilder unorderedList = new TagBuilder("ul");
            unorderedList.InnerHtml = listItemHtml.ToString();

            return unorderedList.ToString();
        }
    }
}