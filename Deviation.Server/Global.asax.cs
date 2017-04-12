using Deviation.Server.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Routing;

namespace Deviation.Server
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
			var container = IocContainer.Setup();
			GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
				new WindsorCompositionRoot(container));
		}
	}
}
