using Bookland.DAL;
using Bookland.DAL.Abstract;
using Bookland.DAL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bookland.Infrastructure
{
    public class MergeCartsNinjectModule : Ninject.Modules.NinjectModule
    {
        private BookshopContext context;

        public MergeCartsNinjectModule(BookshopContext context)
        {
            this.context = context;
        }

        public override void Load()
        {
            Bind<ICartRepository>().To<EfCartRepository>()
                .WithConstructorArgument("context", context);
        }
    }
}