using TNS.Models;
using TNS.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TNS.Controllers
{
    ///<summary>
    ///Login Controller
    ///</summary>
    public class LoginController : Controller
    {

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application != null && application.Context != null)
            {
                application.Context.Response.Headers.Remove("Server");
            }

            Response.AddHeader("x-frame-options", "DENY");
        }

        ///<summary>
        ///Login Controller Index View
        ///</summary>
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Users users, string returnUrl)
        {
            try
            {
                string userkey = ConfigurationManager.AppSettings["userkey"];
                string uid = ConfigurationManager.AppSettings["uid"];

                string ipaddress = Request.UserHostAddress;
                string BrowserUsed = Request.UserAgent;

                TNS.TNS tns = new TNS.TNS();

                var LoginStatus = tns.ADAuth(users.Username, users.Password, BrowserUsed, ipaddress, "", userkey, uid);

                var LoginDetails = LoginStatus.Split('~');

                if (LoginDetails[0] != "01")
                {
                    FormsAuthentication.SetAuthCookie(LoginDetails[1], false);
                    Session["LoginSAPID"] = users.Username;
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Campaign");
                    }
                }
                else
                {
                    TempData["error"] = LoginDetails[1];
                    ViewBag.Error = TempData["error"];
                    return View();
                }
            }
            catch(Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog(users.Username, "2", "3", "4", "Login", "Index", "Login Error", ex.Message.ToString(), 0);
                TempData["error"] = ex.Message.ToString();
                ViewBag.Error = TempData["error"];
                return View();
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        [Authorize]
        public JsonResult GetMenuItems()
        {

            try
            {
                var menuItems = Session["menuView"];
                return Json(new { data = menuItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //LogError logerror = new LogError();
                //logerror.ErrorLog("", LoginUser, "", "Employer/GetCompanyEmployee", "Employer", "GetCompanyEmployee", "GetCompanyEmployee Error", ex.Message.ToString(), 0);
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}