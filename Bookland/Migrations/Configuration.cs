namespace Bookland.Migrations
{
    using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Bookland.DAL.BookshopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Bookland.DAL.BookshopContext context)
        {
            //context.Database.ExecuteSqlCommand("ALTER TABLE Cart ADD CONSTRAINT uc_Cart UNIQUE(UserID)");
            //context.Database.ExecuteSqlCommand("ALTER TABLE Address ADD CONSTRAINT uc_User UNIQUE(UserID)");

            //var products = new List<Product>
            //{
            //    new Product { Name = "Pro ASP.NET MVC 4", Description = "AUTHOR(S): Freeman, Adam", Price = 25.99M, Year = 2012, DateAdded = DateTime.Now },
            //    new Product { Name = "Beginning jQuery", Description = "AUTHOR(S): Franklin, Jack", Price = 24.95M, Year = 2013, DateAdded = DateTime.Now }
            //};
            //products.ForEach(s => context.Products.AddOrUpdate(p => p.Name, s));
            //context.SaveChanges();

            //var users = new List<UserProfile>
            //{
            //    new UserProfile { UserName = "reznor", FirstName = "Trent", LastName = "Reznor", Email = "treznor@gmail.com" },
            //    new UserProfile { UserName = "homme", FirstName = "Josh", LastName = "Homme", Email = "josh.homme@outlook.com" },
            //    new UserProfile { UserName = "grohl", FirstName = "Dave", LastName = "Grohl", Email = "dgrohl@mail.com" }
            //};
            //users.ForEach(s => context.UserProfiles.AddOrUpdate(p => p.UserName, s));
            //context.SaveChanges();
        }
    }
}
