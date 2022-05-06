using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Dapper;

namespace MvcApplication1.Models
{
    public class DB_Interest
    {
        #region 公開屬性
        /// <summary>
        /// 流水號
        /// </summary>
        public int InterestID { get; set; }

        [Required]
        [Display(Name = "興趣")]
        /// <summary>
        /// 興趣
        /// </summary>
        public string Interest { get; set; }
        #endregion

        #region 方法

        #region 取得興趣列表
        /// <summary>
        /// 取得興趣列表
        /// </summary>
        /// <returns></returns>
        public static object GetList(ref List<DB_Interest> _results)
        {
            object reObj = 0;
            _results = new List<DB_Interest>();
            try
            {
                string ConnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;

                using (var cn = new SqlConnection(ConnStr))
                {
                    _results = cn.Query<DB_Interest>(@"SELECT * FROM Interests").ToList();
                }
            }
            catch (Exception ex)
            {
                //reObj = ex.ToString();
                throw new Exception(ex.Message);
            }
            return reObj;
        }
        #endregion


        #region public object Create()
        /// <summary>
        /// 新增興趣資料
        /// </summary>
        /// <returns>0成功; string失敗</returns>
        public object Create()
        {
            object retObj = 0;
            try
            {
                string cnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;
                using (var cn = new SqlConnection(cnStr))
                {
                    dynamic result = cn.Query(@"INSERT INTO Interests(Interest) VALUES(@Interest)", new { Interest = this.Interest });
                }
            }
            catch (Exception ex)
            {
                retObj = ex.ToString();
            }
            return retObj;
        }
        #endregion

        #region public object Update()
        /// <summary>
        /// 更新興趣資料
        /// </summary>
        /// <returns>0成功; string失敗</returns>
        public object Update()
        {
            object retObj = 0;
            try
            {
                string cnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;
                using (var cn = new SqlConnection(cnStr))
                {
                    dynamic result;

                    result = cn.Query(@"UPDATE Interests SET Interest = @Interest WHERE InterestID=@InterestID" , new{ InterestID = this.InterestID, Interest = this.Interest });
                }
            }
            catch (Exception ex)
            {
                retObj = ex.ToString();
            }
            return retObj;
        }
        #endregion

        #region public object GetOne(int key)
        /// <summary>
        /// 取得一筆興趣資料
        /// </summary>    
        /// <param name="key">會員ID</param>
        /// <returns>0成功; string失敗</returns>
        public object GetOne(int key)
        {
            object retObj = 0;
            try
            {
                DB_Interest _results = new DB_Interest();
                string ConnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;

                using (var cn = new SqlConnection(ConnStr))
                {
                    _results = cn.QuerySingleOrDefault<DB_Interest>(@"SELECT * FROM Interests WHERE InterestID = @id", new { id = key });
                }
                this.InterestID = _results.InterestID;
                this.Interest = _results.Interest;
                if (string.IsNullOrEmpty(_results.Interest))
                {
                    retObj = "資料不存在";
                }
            }
            catch (Exception ex)
            {
                retObj = ex.ToString();
            }
            return retObj;
        }
        #endregion

        #region  public object Delete(int key)
        /// <summary>
        /// 刪除興趣資料
        /// </summary>
        /// <param name="key">會員ID</param>
        /// <returns></returns>
        public object Delete(int key)
        {
            object retObj = 0;
            try
            {
                string cnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;
                using (var cn = new SqlConnection(cnStr))
                {
                    cn.Query("DELETE FROM Interests WHERE InterestID=@id", new { id = key });
                }
            }
            catch (Exception ex)
            {
                retObj = ex.ToString();
            }
            return retObj;
        }
        #endregion
        #endregion
    }
}