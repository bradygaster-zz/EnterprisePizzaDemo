using EnterprisePizza.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EnterprisePizza.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        ServiceHost _inventoryService;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapHubs();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            _inventoryService = new ServiceHost(typeof(WebSiteInventoryService));
            _inventoryService.Open();
        }

        protected void Application_End()
        {
            _inventoryService.Close();
        }
    }
}