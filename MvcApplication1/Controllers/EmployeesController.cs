using MvcApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;


namespace MvcApplication1.Controllers
{
    public class EmployeesController : BaseController
    {
        public ActionResult Index()
        {            
            return RedirectToAction("List");
        }

        [Route("Operations")]
        public ActionResult List()
        {
            List<Employees> Emp = new List<Employees>();
            Emp = Employees.GetSession();

            ViewBag.Title = "員工資料 > 清單";

            return View(Emp);
        }

        public ActionResult Insert()
        {
            ViewBag.Title = "員工資料 > 新增";
            
            return View();
        }

        [HttpPost]
        public ActionResult Insert(Employees NewEmp)
        {          
            if (!string.IsNullOrEmpty(NewEmp.Number))
            {
                int MaxIndex;
                List<Employees> Emp = new List<Employees>();
                Emp = Employees.GetSession();
                if (Emp.Count > 0)
                {
                    MaxIndex = Emp.Max(x => x.ID);
                    NewEmp.ID = MaxIndex + 1;
                }
                else { NewEmp.ID = 1; }
                Emp.Add(NewEmp);
                System.Web.HttpContext.Current.Session["EmpTable"] = Emp;
            }
            return RedirectToAction("List");
        }

        public ActionResult Edit(string id)
        {
            ViewBag.Title = "員工資料 > 編輯";

            List<Employees> Emp = new List<Employees>();
            Employees EmpItem = new Employees();
            Emp = Employees.GetSession();
            try
            {
                EmpItem = Emp.Find(x => x.ID == Int32.Parse(id));
            }
            catch
            {
                return RedirectToAction("List");
            }

            return View(EmpItem);
        }

        [HttpPost] 
        public ActionResult Edit(Employees NewEmp)
        {
            ViewBag.Title = "員工資料 > 編輯";
            if (!string.IsNullOrEmpty(NewEmp.Number))
            {
                List<Employees> Emp = new List<Employees>();
                Employees OldEmp = new Employees();
                Emp = Employees.GetSession();
                OldEmp = Emp.Find(x => x.ID == NewEmp.ID);
                Emp.Remove(OldEmp);
                Emp.Add(NewEmp);
                Emp = Emp.OrderBy(x => x.ID).ToList();
                System.Web.HttpContext.Current.Session["EmpTable"] = Emp;
            }

            return RedirectToAction("List");
        }

        public ActionResult Detail(string id)
        {

           ViewBag.Title = "員工資料 > 明細";
            
           List<Employees> Emp = new List<Employees>();
           Employees EmpItem = new Employees();
           Emp = Employees.GetSession();
           try
           {
               EmpItem = Emp.Find(x => x.ID == Int32.Parse(id));              
           }
           catch
           {
               return RedirectToAction("List");
           }

           return View(EmpItem);           
            
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            List<Employees> Emp = new List<Employees>();
            Employees EmpItem = new Employees();
            Emp = Employees.GetSession();
            try
            {
                EmpItem = Emp.Find(x => x.ID == Int32.Parse(id));
                Emp.Remove(EmpItem);
            }
            catch
            {
                return RedirectToAction("List");
            }
            return RedirectToAction("List");
        }
    }
}
