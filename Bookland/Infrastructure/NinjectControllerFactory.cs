using Bookland.DAL;
using Bookland.DAL.Abstract;
using Bookland.DAL.Concrete;
using Ninject;
using System;
using System.Web.Mvc;

namespace Bookland.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        private BookshopContext context;

        public NinjectControllerFactory(BookshopContext bookshopContext)
        {
            context = bookshopContext;
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {            
            ninjectKernel.Bind<IProductRepository>()
                .To<EfProductRepository>()
                .WithConstructorArgument("context", context);

            ninjectKernel.Bind<ICategoryRepository>()
                .To<EfCategoryRepository>()
                .WithConstructorArgument("context", context);

            ninjectKernel.Bind<IUserProfileRepository>()
                .To<EfUserProfileRepository>()
                .WithConstructorArgument("context", context);

            ninjectKernel.Bind<ICartRepository>()
                .To<EfCartRepository>()
                .WithConstructorArgument("context", context);
        }
    }
}