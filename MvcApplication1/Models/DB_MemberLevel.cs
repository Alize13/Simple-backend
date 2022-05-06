using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Dapper;

namespace MvcApplication1.Models
{
    public class DB_MemberLevel
    {
        #region 公開屬性
        /// <summary>
        /// 流水號
        /// </summary>
        public int LevelID { get; set; }

        /// <summary>
        /// 會員等級
        /// </summary>
        [Required]
        [Display(Name = "會員等級")]
        public int MemberLevel { get; set; }

        /// <summary>
        /// 會員等級名稱
        /// </summary>
        [Required]
        [Display(Name = "會員等級名稱")]
        public string LevelName { get; set; }

        /// <summary>
        /// 會員數
        /// </summary>
        public int MemberCount { get; set; }
        #endregion

        #region 方法

        #region 取得會員等級列表
        /// <summary>
        /// 取得會員等級列表
        /// </summary>
        /// <returns></returns>
        public static object GetList(ref List<DB_MemberLevel> _results)
        {
            object reObj = 0;
            _results = new List<DB_MemberLevel>();
            try
            {
                string ConnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;

                using (var cn = new SqlConnection(ConnStr))
                {
                    _results = cn.Query<DB_MemberLevel>(@"SELECT LV.*,Isnull(MemberCount,0) as MemberCount  FROM MemberLevels LV
                        left join (select MemberLevel,count(1) MemberCount from Member group by MemberLevel) M
						ON  M.MemberLevel= LV.MemberLevel").ToList();
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
        /// 新增會員等級資料
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
                    dynamic result = cn.Query(@"INSERT INTO MemberLevels(MemberLevel, LevelName)  VALUES(@MemberLevel, @LevelName)",
                        new
                        {
                            MemberLevel = this.MemberLevel,
                            LevelName = this.LevelName
                        });
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
        /// 更新會員等級資料
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
                    result = cn.Query(@"UPDATE MemberLevels SET MemberLevel = @MemberLevel, LevelName = @LevelName WHERE LevelID=@LevelID"
                            , new
                            {
                                LevelID = this.LevelID,
                                MemberLevel = this.MemberLevel,
                                LevelName = this.LevelName
                            });
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
        /// 取得一筆會員等級資料
        /// </summary>    
        /// <param name="key">會員ID</param>
        /// <returns>0成功; string失敗</returns>
        public object GetOne(int key)
        {
            object retObj = 0;
            try
            {
                DB_MemberLevel _results = new DB_MemberLevel();
                string ConnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;

                using (var cn = new SqlConnection(ConnStr))
                {
                    _results = cn.QuerySingleOrDefault<DB_MemberLevel>(@"SELECT * FROM MemberLevels WHERE LevelID=@id", new { id = key });
                }

                this.LevelID = _results.LevelID;
                this.MemberLevel = _results.MemberLevel;
                this.LevelName = _results.LevelName;

                if (string.IsNullOrEmpty(_results.LevelName))
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
        /// 刪除會員等級資料
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
                    var param = new DynamicParameters();
                    param.Add("@key", dbType: DbType.Int32, value: key, direction: ParameterDirection.Input);

                    retObj = cn.Query("TSP_MemberSearch_GetList", param, commandType: CommandType.StoredProcedure);
                    if (Convert.ToInt32(retObj) == 99)
                    {
                        retObj = "【刪除失敗】查無資料";
                    }
                    else if (Convert.ToInt32(retObj) == 98)
                    {
                        retObj = "【刪除失敗】尚有會員為此等級，請先將此等級的會員調整成其他等級"; //"There are already members in this VIP level."
                    }
                    else if (Convert.ToInt32(retObj) == 97)
                    {
                        retObj = "【刪除失敗】當前等級數值最大的資料才可被刪除";
                    }
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