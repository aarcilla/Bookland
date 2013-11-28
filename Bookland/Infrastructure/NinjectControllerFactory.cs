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

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            // Instantiate a single DB context, for all repositories to utilise.
            // This avoids possible failed transactions involving multiple entities 
            // if/when each of their repositories instantiate separate DB contexts.
            BookshopContext context = new BookshopContext();
            
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