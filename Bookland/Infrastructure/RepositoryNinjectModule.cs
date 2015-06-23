using Bookland.DAL;
using Bookland.DAL.Abstract;
using Bookland.DAL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookland.Infrastructure
{
    public class RepositoryNinjectModule : Ninject.Modules.NinjectModule
    {
        private BookshopContext context;

        public RepositoryNinjectModule(BookshopContext context)
        {
            this.context = context;
        }

        public override void Load()
        {
            Bind<IProductRepository>().To<EfProductRepository>()
                .WithConstructorArgument("context", context);

            Bind<ICategoryRepository>().To<EfCategoryRepository>()
                .WithConstructorArgument("context", context);

            Bind<IUserProfileRepository>().To<EfUserProfileRepository>()
                .WithConstructorArgument("context", context);

            Bind<ICartRepository>().To<EfCartRepository>()
                .WithConstructorArgument("context", context);

            Bind<IPurchaseRepository>().To<EfPurchaseRepository>()
                .WithConstructorArgument("context", context);

            Bind<IProductStatusRepository>().To<EfProductStatusRepository>()
                .WithConstructorArgument("context", context);
        }
    }
}