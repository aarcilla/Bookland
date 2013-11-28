using Bookland.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Bookland.DAL
{
    public class BookshopContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Configure 1-to-1 relationship between UserProfile and Address
            modelBuilder.Entity<Address>()
                .HasRequired(a => a.UserProfile)
                .WithOptional(u => u.Address)
                .Map(m => m.MapKey("UserID"));

            // Configure 1-to-1 relationship between UserProfile and Cart
            modelBuilder.Entity<Cart>()
                .HasRequired(c => c.UserProfile)
                .WithOptional(u => u.Cart)
                .Map(m => m.MapKey("UserID"));

            // Configure many-to-many relationship between parent and child Categories
            modelBuilder.Entity<Category>().HasMany(c => c.ChildCategories).WithMany()
                .Map(t => t.MapLeftKey("ParentID").MapRightKey("ChildID")
                .ToTable("CategoryRelationship"));
        }
    }
}