using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace MvcApplication1.Models
{
    public class Employees
    {        
        [Required]
        [Display(Name = "流水號")]
        public int ID { get; set; }

        [Required]
        [RegularExpression(@"[N]" + @"\d{5}", ErrorMessage = "格式錯誤，格式為N+5 位數字，例如：N22168。")]
        [Display(Name = "員工編號")]
        public string Number { get; set; }

        [Required]
        [Display(Name = "姓名")]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "年齡")]
        [RegularExpression(@"^\+?[1-9][0-9]*$", ErrorMessage = "格式錯誤!請填寫正確年齡!")]
        [Range(18,55)]
        public string Age { get; set; }

        [StringLength(100)]
        [Display(Name = "地址")]
        public string Address { get; set; }


        public static List<Employees> GetSession()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session["EmpTable"] == null)
                {
                    List<Employees> Emp = new List<Employees>();
                    HttpContext.Current.Session["EmpTable"] = Emp;
                }
                return (List<Employees>)HttpContext.Current.Session["EmpTable"];
            }
            else
            {
                throw new InvalidOperationException("Session發生錯誤");
            }
        }
                
    }
}