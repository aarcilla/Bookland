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
            ninjectKernel = new StandardKernel(new RepositoryNinjectModule(context), new HelpersNinjectModule());
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }
    }
}