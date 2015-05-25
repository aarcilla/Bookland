using Bookland.Binders;
using Bookland.DAL;
using Bookland.Infrastructure;
using Bookland.Models;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Bookland
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Instantiate a single DB context, for all repositories to utilise.
            // This avoids possible failed transactions involving multiple entities 
            // if/when each of their repositories instantiate separate DB contexts.
            BookshopContext context = new BookshopContext();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters, context);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory(context));
            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());
        }
    }
}