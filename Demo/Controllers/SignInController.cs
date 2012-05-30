using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.Security;

namespace Demo.Controllers
{
    public class SignInController : Controller
    {
      public ActionResult Index()
      {
        return View();
      }
      
      public ActionResult LiveId()
      {
        OAuthWebSecurity.RequestAuthentication("WindowsLive", "~/signin/callback");
        return new EmptyResult();
      }


      public ActionResult Google()
      {
        OAuthWebSecurity.RequestAuthentication("google_oauth", "~/signin/callback");
        return new EmptyResult();
      }  

      public ActionResult Callback()
      {
        AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication();
        if (!result.IsSuccessful)
        {
          return RedirectToAction("Index");
        }
        DateTime now = DateTime.Now;
        FormsAuthentication.SetAuthCookie(result.UserName, false);
        Session["id"] = result.ProviderUserId;
        Session["provider"] = result.Provider;
        Session["email"] = result.ExtraData["email"];
        return RedirectToAction("Index", "Home");
      }
        

      public ActionResult SignOut()
      {
        FormsAuthentication.SignOut();
        Session.Clear();
        return RedirectToAction("Index", "Home");
      }        
        
        
    }
}
