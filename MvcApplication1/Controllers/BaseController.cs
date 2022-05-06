using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class BaseController : Controller
    {
        //API網域
        public static readonly Uri ApiDomain = new Uri("http://localhost:1822/");

        public BaseController()
        {
            ViewBag.ApiDomain = ApiDomain;
            DB_Member UserData = new DB_Member();
            UserData = (DB_Member)System.Web.HttpContext.Current.Session["Member"];
            if (UserData != null)
            {
                ViewBag.LoginAcc = UserData.MemberAccount;
                ViewBag.LoginName = string.IsNullOrEmpty(UserData.MemberName) ? UserData.MemberAccount : UserData.MemberName;
            }
        }

    }

}
