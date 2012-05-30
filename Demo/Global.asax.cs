using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Demo.Models;

namespace Demo
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }

    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      WebMatrix.Security.OAuthWebSecurity.RegisterClient(new GoogleClient("478295084580.apps.googleusercontent.com", "4MMOjFpHQdFOHFUkxlA0BZzH"));
      WebMatrix.Security.OAuthWebSecurity.RegisterClient(new ExtendedWindowsLiveClient("000000004006F97D", "OdQmjvitSJpdut55VeqwrU14C4ON6eli"));      
                  

      // Use LocalDB for Entity Framework by default
      Database.DefaultConnectionFactory = new SqlConnectionFactory("Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);

      BundleTable.Bundles.RegisterTemplateBundles();
    }
  }
}