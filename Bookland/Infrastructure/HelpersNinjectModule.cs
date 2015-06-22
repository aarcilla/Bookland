using Bookland.Helpers;
using Bookland.Helpers.Abstract;
using System.Configuration;

namespace Bookland.Infrastructure
{
    public class HelpersNinjectModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            bool emailEnabled = bool.Parse(ConfigurationManager.AppSettings["emailEnabled"]);

            // If email functionality is disabled in configuration settings, bind email helper interface 
            // to mock implementation that just returns expected successful values
            if (emailEnabled)
                Bind<IMailHelpers>().To<MailHelpers>();
            else
                Bind<IMailHelpers>().To<MockEmailHelpers>();

            Bind<IMvcHelpers>().To<MvcHelpers>();

            Bind<IProductHelpers>().To<ProductHelpers>();

            Bind<IAccountHelpers>().To<AccountHelpers>();

            Bind<ICategoryHelpers>().To<CategoryHelpers>();

            Bind<ISearchHelpers>().To<SearchHelpers>();
        }
    }
}