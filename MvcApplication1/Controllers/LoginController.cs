using MvcApplication1.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class LoginController : BaseController
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginUser User)
        {
            //發送API，檢查是否帳號、密碼正確
            Uri address = new Uri(ApiDomain, "/api/Login/Post");
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string data = JsonConvert.SerializeObject(User);
                    var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = httpClient.PostAsync(address, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string Result = response.Content.ReadAsStringAsync().Result;
                        APIResultObject Catch = new APIResultObject();
                        Catch.Parse(Result);
                        if (Catch.Code == WebApi.Data.TypeEnum.StatusCode.OK && Catch.RetObj != null)
                        {
                            Session["Member"] = JsonConvert.DeserializeObject<DB_Member>(Catch.RetObj.ToString());
                            return Json("登入成功");
                        }
                        else
                        {
                            return Json(Catch.ErrMsg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return Json("登入失敗");

        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

    }
}
