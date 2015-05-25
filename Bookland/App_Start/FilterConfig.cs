using Bookland.DAL;
using Bookland.DAL.Abstract;
using Bookland.Infrastructure;
using Ninject;
using System.Web.Mvc;

namespace Bookland
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, BookshopContext context)
        {
            filters.Add(new HandleErrorAttribute());

            IKernel ninjectKernel = new StandardKernel(new MergeCartsNinjectModule(context));
            var cartRepo = ninjectKernel.Get<ICartRepository>();
            filters.Add(new MergeSessionAndDbCartsAttribute(cartRepo));
        }
    }
}