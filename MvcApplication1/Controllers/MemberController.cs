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
    public class MemberController : BaseController
    {
        //
        // GET: /Member/

        public ActionResult List()
        {
            ViewBag.Title = "會員列表";
            List<DB_Interest> List = new List<DB_Interest>();
            DB_Interest.GetList(ref List);
            ViewBag.IntertestList = List;

            List<DB_Member> ML = new List<DB_Member>();
            return View(ML);
        }


        #region 會員資料-列表
        /// <summary>
        /// 會員等級設定-列表
        /// </summary>
        public JsonResult List_Load(MemberSearchParam param)
        {
            //發送API，取得會員列表

            object jsonData = new object();
            Uri address = new Uri(ApiDomain, "/api/Member/Get");
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
                            jsonData = Catch.RetObj;
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
                jsonData = new object();
            }
            JsonResult jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;            
        }

        #endregion

        #region MemberAdd 會員資料新增與修改
        [HttpGet]
        public ActionResult MemberAdd()
        {
            try
            {
                DB_Member ItemAdd = new DB_Member();

                int _key = Convert.ToInt32(Request.QueryString["key"]);
                if (_key > 0)
                {
                    ItemAdd.GetOne(_key);
                }
                if (ItemAdd.MemberLevel == 0)
                {
                    ItemAdd.MemberLevel = 1;
                }
                object retobj = 0;

                return PartialView("~/Views/Modal/#Member.cshtml", ItemAdd);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return RedirectToAction("Logout", "Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MemberAdd(DB_Member ItemAdd)
        {
            object retobj = new object();

            try
            {
                if (ModelState.IsValid)
                {
                    if (ItemAdd.MemberID > 0)
                    {
                        retobj = ItemAdd.Update();
                    }
                    else
                    {
                        retobj = ItemAdd.Create();

                    }
                }
                else
                {
                    //取得第一條錯誤訊息
                    foreach (ModelState error in ModelState.Values)
                    {
                        if (error.Errors.Count > 0)
                        {
                            retobj = error.Errors[0].ErrorMessage;
                            break;
                        }
                    }
                }

                return Json(retobj);
            }
            catch (Exception ex)
            {
                ex.ToString();
                retobj = new object();
                return Json(retobj);
            }
        }



        #endregion

        #region MemberDelete 會員資料刪除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MemberDelete(int Key)
        {
            object retObj = new object();
            try
            {
                DB_Member item = new DB_Member();
                retObj = item.Delete(Key);

                return Json(retObj);
            }
            catch (Exception ex)
            {
                ex.ToString();
                retObj = new object();
                return Json(retObj);
            }
        }
        #endregion


        public ActionResult MyAccount()
        {

            ViewBag.Title = "個人資料";

            DB_Member User = new DB_Member();
            try
            {
                User= (DB_Member)Session["Member"];
            }
            catch
            {
                return RedirectToAction("List");
            }

            return View(User);
        }


    }
}
