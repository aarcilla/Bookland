﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bookland.Helpers;

namespace Bookland.Infrastructure
{
    public class HelpersNinjectModule : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<IMailHelpers>().To<MailHelpers>();

            Bind<IMvcHelpers>().To<MvcHelpers>();
        }
    }
}