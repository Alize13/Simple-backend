using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class SystemController : BaseController
    {

        #region 會員等級設定
        public ActionResult LevelSetting()
        {
            ViewBag.Title = "會員等級設定";
            List<DB_MemberLevel> ML = new List<DB_MemberLevel>();
            DB_MemberLevel.GetList(ref ML);

            return View(ML);
        }


        #region 會員等級設定-列表
        /// <summary>
        /// 會員等級設定-列表
        /// </summary>
        public JsonResult LevelSetting_Load()
        {
            object jsonData = new object();
            try
            {
                //JQGrid 回傳所需參數　→Core 後台及通路共用
                List<DB_MemberLevel> List = new List<DB_MemberLevel>();
                object result0 = DB_MemberLevel.GetList(ref List);
                jsonData = new
                {
                    //total = totalPages,
                    //page = pageNum,
                    //records = totalRecords,
                    data = List.ToList()
                };
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

        #region LevelSettingAdd 等級設定新增與修改
        [HttpGet]
        public ActionResult LevelSettingAdd()
        {
            try
            {
                DB_MemberLevel ItemAdd = new DB_MemberLevel();

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

                return PartialView("~/Views/Modal/#LevelSetting.cshtml", ItemAdd);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return RedirectToAction("Logout", "Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LevelSettingAdd(DB_MemberLevel ItemAdd)
        {
            object retobj = new object();

            try
            {
                if (ModelState.IsValid)
                {
                    if (ItemAdd.LevelID > 0)
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

        #region LevelSettingDelete 等級設定刪除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LevelSettingDelete(int Key)
        {
            object retObj = new object();
            try
            {
                DB_MemberLevel item = new DB_MemberLevel();
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

        #endregion

        #region 興趣設定
        public ActionResult InterestSetting()
        {
            ViewBag.Title = "興趣設定";
            List<DB_MemberLevel> ML = new List<DB_MemberLevel>();
            DB_MemberLevel.GetList(ref ML);

            return View();
        }


        #region 興趣設定-列表
        /// <summary>
        /// 興趣設定-列表
        /// </summary>
        public JsonResult InterestSetting_Load()
        {
            object jsonData = new object();
            try
            {
                //JQGrid 回傳所需參數　→Core 後台及通路共用
                List<DB_Interest> List = new List<DB_Interest>();
                object result0 = DB_Interest.GetList(ref List);
                jsonData = new
                {
                    data = List.ToList()
                };
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

        #region InterestSettingAdd 興趣設定-新增與修改
        [HttpGet]
        public ActionResult InterestSettingAdd()
        {
            try
            {
                DB_Interest ItemAdd = new DB_Interest();
                object retobj = 0;

                int _key = Convert.ToInt32(Request.QueryString["key"]);
                if (_key > 0)
                {
                    retobj  = ItemAdd.GetOne(_key);
                }

                return PartialView("~/Views/Modal/#InterestSetting.cshtml", ItemAdd);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return RedirectToAction("Logout", "Login", new { lus = "7" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult InterestSettingAdd(DB_Interest ItemAdd)
        {
            object retobj = new object();

            try
            {
                if (ModelState.IsValid)
                {
                    if (ItemAdd.InterestID > 0)
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

        #region InterestSettingDelete 興趣設定-刪除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult InterestSettingDelete(int Key)
        {
            object retObj = new object();
            try
            {
                DB_Interest item = new DB_Interest();
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

        #endregion

    }
}
