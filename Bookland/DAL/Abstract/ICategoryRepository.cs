using Bookland.Data_Structures;
using Bookland.Models;
using System;
using System.Collections.Generic;

namespace Bookland.DAL.Abstract
{
    public interface ICategoryRepository : IDisposable
    {
        /// <summary>
        /// Retrieve all stored categories.
        /// </summary>
        /// <returns>An enumeration of stored categories.</returns>
        IEnumerable<Category> GetCategories();
        
        /// <summary>
        /// Retrieve a specific category, based on ID.
        /// </summary>
        /// <param name="categoryID">The ID of the requested category.</param>
        /// <returns>A Category object of the requested category.</returns>
        Category GetCategory(int categoryID);
        
        /// <summary>
        /// Retrieve a specific category, based on its name. This may not retrieve the intended category
        /// if there exists multiple categories with the same name.
        /// </summary>
        /// <param name="categoryName">The name of the requested category.</param>
        /// <returns>A Category object of the requested category.</returns>
        Category GetCategory(string categoryName);
        
        /// <summary>
        /// Retrieve the parent category of a specific category.
        /// </summary>
        /// <param name="categoryID">The name of the requested category.</param>
        /// <returns>A Category object of the parent of the requested category.</returns>
        Category GetParentCategory(int categoryID);

        /// <summary>
        /// Retrieve the child categories of a specific category.
        /// </summary>
        /// <param name="categoryID">The name of the specified category.</param>
        /// <returns>An enumeration of the children of the specified category.</returns>
        IEnumerable<Category> GetChildCategories(int categoryID);

        /// <summary>
        /// Retrieve stored categories in a tree structure, representing the categories' hierarchy.
        /// </summary>
        /// <param name="categoryID">The category ID of the desired highest-ranking category.</param>
        /// <returns>Stored categories in a tree structure, starting with the desired root node.</returns>
        TreeNode<Category> GetCategoryTree(int categoryID = 1);

        /// <summary>
        /// Add a category to the DB, and add the category to the specified parent's list of child categories.
        /// </summary>
        /// <param name="category">The category to add.</param>
        /// <param name="parentCategoryID">Intended parent category's ID.</param>
        void CreateCategory(Category category, int parentCategoryID);
        
        /// <summary>
        /// Update a specific category's values to the DB.
        /// </summary>
        /// <param name="category">The category to update, with updated values.</param>
        /// <param name="parentCategoryID">Updated parent category's ID.</param>
        void UpdateCategory(Category category, int parentCategoryID);

        /// <summary>
        /// Delete the specified category, as well as its child categories, from the DB. 
        /// Also remove the specified category from the children list/collection of its parent (i.e. many-to-many parent-child relationship).
        /// </summary>
        /// <param name="categoryID">The ID of the category to be deleted.</param>
        void DeleteCategory(int categoryID);

        /// <summary>
        /// Check if no categories exist in the repository.
        /// </summary>
        /// <returns>A Boolean condition specifying whether any categories exist or not.</returns>
        bool NoCategoriesExist();
        
        /// <summary>
        /// Commit any changes to the DB.
        /// </summary>
        void Commit();
    }
}