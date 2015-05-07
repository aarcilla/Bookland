using Bookland.DAL.Abstract;
using Bookland.Data_Structures;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookland.DAL.Concrete
{
    public class EfCategoryRepository : ICategoryRepository, IDisposable
    {
        private BookshopContext context;

        public EfCategoryRepository(BookshopContext context)
        {
            this.context = context;
        }

        public IEnumerable<Category> GetCategories()
        {
            return context.Categories.OrderBy(c => c.CategoryName).ToList();
        }

        public Category GetCategory(int categoryID)
        {
            return context.Categories.FirstOrDefault(c => c.CategoryID == categoryID);
        }

        public Category GetCategory(string categoryName)
        {
            return context.Categories.FirstOrDefault(c => c.CategoryName.ToUpper() == categoryName.ToUpper());
        }

        public Category GetParentCategory(int categoryID)
        {
            return context.Categories
                .FirstOrDefault(c => c.ChildCategories.Any(ch => ch.CategoryID == categoryID));
        }

        public IEnumerable<Category> GetChildCategories(int categoryID)
        {
            return GetCategory(categoryID).ChildCategories;
        }

        public TreeNode<Category> GetCategoryTree(int categoryID = 1)
        {
            Category category = GetCategory(categoryID);

            if (category != null)
            {
                TreeNode<Category> currentParentNode = new TreeNode<Category>(category);

                // If children exist for the current category node, append their trees to the current tree
                var children = category.ChildCategories;
                if (children != null)
                {
                    foreach (Category cat in children.OrderBy(c => c.CategoryName))
                    {
                        // Retrieve child category trees through recursion
                        TreeNode<Category> childTree = GetCategoryTree(cat.CategoryID);
                        currentParentNode.AddChild(childTree);
                    }
                }

                return currentParentNode;
            }

            return null;
        }
        
        public void CreateCategory(Category category, int parentCategoryID)
        {
            context.Categories.Add(category);

            DeclareParentChildRelationship(category, parentCategoryID);
        }

        public void UpdateCategory(Category category, int parentCategoryID)
        {
            // Update relevant Category properties/columns
            Category dbCategory = GetCategory(category.CategoryID);
            dbCategory.CategoryName = category.CategoryName;
            dbCategory.CategoryDescription = category.CategoryDescription;

            // Update parent-child relationship (if it changed)
            Category currentParent = GetParentCategory(category.CategoryID);
            if (parentCategoryID != currentParent.CategoryID)
            {
                // Retrieve the category's category level before updating, to aid in updating descendant category level values
                int oldCategoryLevel = dbCategory.CategoryLevel;

                // Remove the specified category's child listing from its old parent, 
                // and declare new parent-child relationship with the new one
                currentParent.ChildCategories.Remove(dbCategory);
                DeclareParentChildRelationship(dbCategory, parentCategoryID);

                // Retrieve the difference between the old and updated category levels
                // (N.B. Category level was updated in the DeclareParentChildRelationship helper method)
                int categoryLevelDiff = dbCategory.CategoryLevel - oldCategoryLevel;

                // Generate a new list of child categories for current category, if it hasn't been instantiated yet
                if (dbCategory.ChildCategories == null)
                {
                    dbCategory.ChildCategories = new List<Category>();
                }

                // Update the category's descendants' level values
                List<Category> descendants = GetCategoryTree(dbCategory.CategoryID).ToList();
                foreach (Category cat in descendants)
                {
                    if (cat.CategoryID != dbCategory.CategoryID)
                    {
                        // Add the parent's level difference for its descendants; differences should be identical
                        // (e.g. OLD: 5{6{7}, 6{7, 7}} | NEW: 3{4{5}, 4{5, 5}} | DIFF: -2)
                        cat.CategoryLevel += categoryLevelDiff;
                    }
                }
            }
        }

        /// <summary>
        /// Declares a parent-child relationship between two categories in the DB.
        /// </summary>
        /// <param name="childCategory">The child category.</param>
        /// <param name="parentCategoryID">The ID of the parent category.</param>
        private void DeclareParentChildRelationship(Category childCategory, int parentCategoryID)
        {
            Category parent = GetCategory(parentCategoryID);
            if (parent != null)
            {
                // Set the child category's hierarchy level, based on the parent's level
                childCategory.CategoryLevel = parent.CategoryLevel + 1;

                // Generate a new list of child categories for parent, if it hasn't been instantiated yet
                // (N.B. Errors pertaining to null child category lists occur when the parent & child were created in the same session)
                if (parent.ChildCategories == null)
                {
                    parent.ChildCategories = new List<Category>();
                }

                // Add child category to parent's list of child categories
                parent.ChildCategories.Add(childCategory);
            }
        }

        public void DeleteCategory(int categoryID)
        {
            Category category = GetCategory(categoryID);
            Category parent = GetParentCategory(category.CategoryID);

            // Remove the the specified category's child listing from its parent
            parent.ChildCategories.Remove(category);

            // Clear the child categories list for the specified category, then delete all of them, as well as disassociate any associated Products
            if (category.ChildCategories != null)
            {
                List<Category> childCategories = GetCategoryTree(category.CategoryID).ToList();

                category.ChildCategories.Clear();
                foreach (Category cat in childCategories)
                {
                    // For each Product associated with this to-be-removed child Category, set its Category to the root node (i.e. unspecified)
                    List<Product> childCategoryProducts = context.Products.Where(p => p.Category.CategoryID == cat.CategoryID).ToList();
                    childCategoryProducts.ForEach(p => p.Category = GetCategory(1));

                    context.Categories.Remove(cat);
                }
            }

            // For each Product associated with this to-be-removed Category, set its Category to the root node (i.e. unspecified)
            List<Product> categoryProducts = context.Products.Where(p => p.Category.CategoryID == category.CategoryID).ToList();
            categoryProducts.ForEach(p => p.Category = GetCategory(1));

            // Delete the specified category
            context.Categories.Remove(category);
        }

        public bool NoCategoriesExist()
        {
            var categories = context.Categories;

            return (categories == null || categories.Count() <= 0);
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Dispose of the database context to ensure DB connection is properly closed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}