using Canteen_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Canteen_Management_System.Controllers
{
    public class HomeController : Controller
    {
        Canteen_ManagementEntities db = new Canteen_ManagementEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PublicQuery(Public_Query qry)
        {
            return RedirectToAction("Index");
        }
        public JsonResult GetExtraPrice(int extraId)
        {
            var extraItem = db.Extra_Item.FirstOrDefault(e => e.Ex_Id == extraId);
            if (extraItem != null)
            {
                return Json(new { price = extraItem.Price }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { price = 0 }, JsonRequestBehavior.AllowGet); // Return 0 if not found
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName)
            {
                Expires = DateTime.Now.AddDays(-1),  // Set expiry date to the past to delete the cookie
                HttpOnly = true,                     // Ensure the cookie can't be accessed by client-side scripts
                Secure = Request.IsSecureConnection  // Only send cookie over HTTPS if the connection is secure
            };
            Response.Cookies.Add(authCookie);
            return RedirectToAction("Index", "Home");
        }
    }
}