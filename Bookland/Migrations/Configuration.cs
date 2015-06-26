namespace Bookland.Migrations
{
    using Bookland.Constants;
    using Bookland.DAL;
    using Bookland.Helpers;
    using Bookland.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<Bookland.DAL.BookshopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Bookland.DAL.BookshopContext context)
        {
            string constraintCountQueryFormat = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = @p0";
            IEnumerable<int> ucCartExists = context.Database.SqlQuery<int>(constraintCountQueryFormat, "uc_Cart");
            IEnumerable<int> ucUserExists = context.Database.SqlQuery<int>(constraintCountQueryFormat, "uc_User");

            if (ucCartExists.First() < 1)
                context.Database.ExecuteSqlCommand("ALTER TABLE Cart ADD CONSTRAINT uc_Cart UNIQUE(UserID)");

            if (ucUserExists.First() < 1)
                context.Database.ExecuteSqlCommand("ALTER TABLE Address ADD CONSTRAINT uc_User UNIQUE(UserID)");

            Category bookCategory = new Category { CategoryName = "Books", CategoryDescription = "Physical books.", CategoryLevel = 1 };
            context.Categories.AddOrUpdate(c => c.CategoryName, bookCategory);

            Category rootCategory = new Category { CategoryName = "ROOT", CategoryDescription = "Root category.", CategoryLevel = 0 };
            if (rootCategory.ChildCategories == null)
                rootCategory.ChildCategories = new List<Category>();
            rootCategory.ChildCategories.Add(bookCategory);
            context.Categories.AddOrUpdate(c => c.CategoryName, rootCategory);

            
            ProductStatus statusOnSale = new ProductStatus { ProductStatusName = ProductStatusOptions.OnSale };
            var productStatuses = new List<ProductStatus> 
            {
                new ProductStatus { ProductStatusName = ProductStatusOptions.PreOrder },
                statusOnSale,
                new ProductStatus { ProductStatusName = ProductStatusOptions.Discontinued },
                new ProductStatus { ProductStatusName = ProductStatusOptions.NotVisible }
            };
            productStatuses.ForEach(s => context.ProductStatuses.AddOrUpdate(pS => pS.ProductStatusName, s));
            context.SaveChanges();

            var products = new List<Product>
            {
                new Product { Name = "Pro ASP.NET MVC 4", Description = "AUTHOR(S): Freeman, Adam", Category = bookCategory, Price = 25.99M, Year = 2012, DateAdded = DateTime.Now, ProductStatus = statusOnSale },
                new Product { Name = "Beginning jQuery", Description = "AUTHOR(S): Franklin, Jack", Category = bookCategory, Price = 24.95M, Year = 2013, DateAdded = DateTime.Now, ProductStatus = statusOnSale }
            };
            products.ForEach(s => context.Products.AddOrUpdate(p => p.Name, s));
            context.SaveChanges();


            // Initialise SimpleMembership for database - borrowed from InitializeSimpleMembershipAttribute
            Database.SetInitializer<BookshopContext>(null);
            try
            {
                WebSecurity.InitializeDatabaseConnection("BookshopContext", "UserProfile", "UserID", "UserName", autoCreateTables: true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
            }

            // Also create other roles
            if (!Roles.RoleExists("Administrator")) Roles.CreateRole("Administrator");
            if (!Roles.RoleExists("Support")) Roles.CreateRole("Support");
            if (!Roles.RoleExists("Staff")) Roles.CreateRole("Staff");
            if (!Roles.RoleExists("Customer")) Roles.CreateRole("Customer");

            // Skip admin account creation if it's already in the database
            if (context.UserProfiles.Any(c => c.UserName == "admin0"))
                return;

            // Create admin account
            WebSecurity.CreateUserAndAccount("admin0", "overlord", new
            {
                FirstName = "Admin",
                LastName = "Zero",
                Email = ConfigurationManager.AppSettings["adminEmail"]
            });

            // Assign the 'Administrator' role
            Roles.AddUserToRole("admin0", "Administrator");
            UserProfile adminUserProfile = context.UserProfiles.FirstOrDefault(u => u.UserName == "admin0");

            // Add associated Address record for admin account
            context.Addresses.Add(new Address
            {
                StreetLine1 = "0 Zero Street",
                StreetLine2 = "",
                City = "Bookland",
                State = "New South Wales",
                Country = "Australia",
                Postcode = 2000,
                UserProfile = adminUserProfile
            });

            context.Carts.Add(new Cart { UserProfile = adminUserProfile });

            context.SaveChanges();
        }
    }
}
