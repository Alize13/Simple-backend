using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Dapper;
using WebApi.Data;
using static WebApi.Data.TypeEnum;

namespace MvcApplication1.Models
{

    public class LoginUser
    {
        #region 公開屬性
        /// <summary>
        /// 登入帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 登入密碼
        /// </summary>
        public string Password { get; set; }

        #endregion

    }

    public class MemberSearchParam
    {
        /// <summary>
        /// 搜尋帳號
        /// </summary>
        public string MemberAccount { get; set; }

        /// <summary>
        /// 搜尋狀態
        /// </summary>
        public bool MemberStatus { get; set; }

        /// <summary>
        /// 建立時間(起)
        /// </summary>
        public string startDate { get; set; }

        /// <summary>
        /// 建立時間(訖)
        /// </summary>
        public string endDate { get; set; }

        /// <summary>
        /// 篩選薪水運算子 EX:大於等於('>=')，小於等於('<=')
        /// </summary>
        public string trace_op
        {
            get; set;
        }

        /// <summary>
        /// 篩選薪水值
        /// </summary>
        public int trace_value
        {
            get; set;
        }

        /// <summary>
        /// 模糊搜尋興趣
        /// </summary>
        public bool fuzzyQuery { get; set; }

        /// <summary>
        /// 搜尋會員興趣
        /// </summary>
        public string MemberInterests { get; set; }

        /// <summary>
        /// 排序欄位(預設:1   1.CreateDateTime,2.MemberLevel,3.MemberAccount,4.MemberName,5.CreateUser )	
        /// </summary>
        public int sort_column
        {
            get; set;
        }

        /// <summary>
        /// 升冪排序為 1(Asc) 降冪為0(DESC)
        /// </summary>
        public int asc_order
        {
            get; set;
        }

        /// <summary>
        /// 起始頁
        /// </summary>
        public int row_start
        {
            get; set;
        }

        /// <summary>
        /// 當頁呈現筆數
        /// </summary>
        public int row_number
        {
            get; set;
        }
    }

    public class DB_Member
    {
        #region 公開屬性
        public int MemberID { get; set; }
        public string MemberAccount { get; set; }
        public string MemberPassword { get; set; }
        public bool MemberStatus { get; set; }
        public string MemberName { get; set; }
        public string MemberEmail { get; set; }
        public int MemberLevel { get; set; }
        public string MemberLevel_Str { get; set; }
        public decimal MemberSalaly { get; set; }
        public string MemberInterests { get; set; }
        public string MemberInterests_Str { get; set; }
        public int CreateUser { get; set; }
        public string CreateDateTime { get; set; }

        #endregion

        #region 方法

        #region 登入驗證
        /// <summary>
        /// 驗證密碼並取得會員資料
        /// </summary>
        /// <param name="_model"></param>
        /// <returns></returns>
        public object LoginCheck(LoginUser _model)
        {
            object retObj = new object();
            try
            {
                DB_Member _results = new DB_Member();

                if (_model.Account=="Admin" && _model.Password=="1234") { 
                    _results.MemberAccount = _model.Account;
                    _results.MemberPassword = _model.Password;
                    _results.MemberLevel = 10;
                    _results.MemberLevel_Str = "最高管理員";
                }
                string ConnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;
                string pwd = Security.EncodePassword(_model.Password, SecurityType.UnIntact);
                using (var cn = new SqlConnection(ConnStr))
                {
                    _results = cn.Query<DB_Member>(
                     @"SELECT * FROM Member 
                        WHERE MemberAccount=@Account AND MemberPassword=@Password AND MemberStatus = 1",
                     new { Account = _model.Account, Password = pwd }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(_results.MemberAccount))
                    {
                        retObj = _results;
                    }
                }

            }
            catch (Exception ex)
            {
                //jsonData = "Error";
                throw new Exception(ex.Message);
            }
            return retObj;
        }
        #endregion

        #region 取得會員列表-分頁
        /// <summary>
        /// 取得列表-分頁
        /// </summary>
        /// <param name="_model"></param>
        /// <returns></returns>
        public  object GetListPaging(MemberSearchParam _model)
        {
            object jsonData = new object();
            string cnMainStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;
            //int recordCount = 0;    //筆數
            try
            {
                List<DB_Member> _results_list = new List<DB_Member>();


                _model.row_number = _model.row_number > 0 ? _model.row_number : 50;
                _model.row_start = _model.row_start > 0 ? _model.row_start : 1;

                using (var cn = new SqlConnection(cnMainStr))
                {
                    var param = new DynamicParameters();
                    param.Add("@MemberAccount", dbType: DbType.String, value: _model.MemberAccount, direction: ParameterDirection.Input);//會員帳號
                    param.Add("@MemberStatus", dbType: DbType.Boolean, value: _model.MemberStatus, direction: ParameterDirection.Input);  //帳號狀態(1啟用 0停用)
                    param.Add("@trace_op", dbType: DbType.String, value: _model.trace_op, direction: ParameterDirection.Input);         //篩選薪水運算子 大於等於('>=')，小於等於('<=')
                    param.Add("@trace_value", dbType: DbType.Int32, value: _model.trace_value, direction: ParameterDirection.Input);    //篩選薪水值
                    param.Add("@startDate", dbType: DbType.String, value: _model.startDate, direction: ParameterDirection.Input);       //起始日
                    param.Add("@endDate", dbType: DbType.String, value: _model.endDate, direction: ParameterDirection.Input);           //結束日
                    param.Add("@fuzzyQuery", dbType: DbType.Boolean, value: _model.fuzzyQuery, direction: ParameterDirection.Input);    //興趣是否為模糊搜尋(1模糊 0精確)
                    param.Add("@MemberInterests", dbType: DbType.String, value: _model.MemberInterests, direction: ParameterDirection.Input); //興趣
                    param.Add("@sort_column", dbType: DbType.Int32, value: _model.sort_column, direction: ParameterDirection.Input);    //排序欄位(預設:0代理   1.註冊人數 2.首存人數 3.活躍會員數 4.存款金額 5.取款金額 6.有效投注 7.公司輸贏 8.活動優惠 9.返水金額 10.收益差 11.轉換率 12.調整金額 13.彩金)	
                    param.Add("@asc_order", dbType: DbType.Int32, value: _model.asc_order, direction: ParameterDirection.Input);        //排序方式(預設:DESC True:ASC False:DESC)
                    param.Add("@row_start", dbType: DbType.Int32, value: _model.row_start, direction: ParameterDirection.Input);        //起始頁
                    param.Add("@row_number", dbType: DbType.Int32, value: _model.row_number, direction: ParameterDirection.Input);      //當頁呈現筆數

                    using (var dr = cn.QueryMultiple("TSP_MemberSearch_GetList", param, commandType: CommandType.StoredProcedure))
                    {

                        dynamic _result_sum = new { };


                        //列表
                        if (dr.IsConsumed == false)
                        {
                            _results_list = dr.Read().Select(item => new DB_Member()
                            {
                                MemberID = item.MemberID,
                                MemberAccount = item.MemberAccount,
                                MemberName = item.MemberName,
                                MemberEmail = item.MemberEmail,
                                MemberLevel = item.MemberLevel,
                                MemberInterests = item.MemberInterests,
                                MemberStatus = item.MemberStatus,
                                CreateUser = item.CreateUser,
                                CreateDateTime = item.CreateDateTime
                            }
                            ).ToList();
                        }

                        //加總
                        if (dr.IsConsumed == false)
                        {
                            string ShowTitle = string.Empty;


                            _result_sum = dr.Read().Select(item => new
                            {
                                RecordCount = item.RecordCount,
                                SalaryTotal = item.SalaryTotal
                            }).SingleOrDefault();
                        }

                        if (_results_list.Count > 0)
                        {
                            List<DB_Interest> InterestList = new List<DB_Interest>();
                            DB_Interest.GetList(ref InterestList);

                            List<DB_MemberLevel> LevelList = new List<DB_MemberLevel>();
                            DB_MemberLevel.GetList(ref LevelList);


                            foreach (DB_Member item in _results_list)
                            {
                                if (LevelList.Exists(x => x.MemberLevel == item.MemberLevel))
                                {
                                    item.MemberLevel_Str = LevelList.Find(x => x.MemberLevel == item.MemberLevel).LevelName;
                                }
                                if (!string.IsNullOrEmpty(item.MemberInterests))
                                {
                                    string[] Interests = item.MemberInterests.Split(';');
                                    foreach (var itrs in Interests)
                                    {
                                        if (InterestList.Exists(x => x.InterestID == Convert.ToInt32(itrs)))
                                        {
                                            if (item.MemberInterests_Str != "") { item.MemberInterests_Str += "、"; }
                                            item.MemberLevel_Str += InterestList.Find(x => x.InterestID == Convert.ToInt32(itrs)).Interest;
                                        }
                                    }
                                }
                            }

                        }

                        int totalRecords = _result_sum.RecordCount;
                        int totalPages = (int)Math.Ceiling((float)totalRecords / (float)_model.row_number);
                        jsonData = new
                        {
                            total = totalPages,
                            page = _model.row_start,
                            records = totalRecords,
                            rows = _results_list,
                            info = _result_sum
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                //jsonData = "Error";
                throw new Exception(ex.Message);
            }
            return jsonData;
        }
        #endregion


        #region public object Create(CP_LoginSysInfo LoginSysInfo)
        /// <summary>
        /// 新增會員資料
        /// </summary>
        /// <returns>0成功; string失敗</returns>
        public object Create()
        {
            object retObj = new object();
            try
            {
                string cnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;
                using (var cn = new SqlConnection(cnStr))
                {
                    dynamic result = cn.Query(@"INSERT INTO Member(MemberAccount, MemberName, MemberEmail, MemberLevel, MemberSalaly, CreateUser) 
                            VALUES(@MemberAccount, @MemberName, @MemberEmail, @MemberLevel, @MemberSalaly, @CreateUser)",
                        new
                        {
                            MemberAccount = this.MemberAccount
                            ,
                            MemberName = this.MemberName
                            ,
                            MemberEmail = this.MemberEmail
                            ,
                            MemberLevel = this.MemberLevel
                            ,
                            MemberSalaly = this.MemberSalaly
                            ,
                            CreateUser = this.CreateUser
                        });
                    retObj = result > 0 ? "0" : "新增失敗";
                }
            }
            catch (Exception ex)
            {
                retObj = ex.ToString();
            }
            return retObj;
        }
        #endregion

        #region public object Update(CP_LoginSysInfo LoginSysInfo)
        /// <summary>
        /// 更新會員資料
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
                    if (string.IsNullOrEmpty(this.MemberPassword))
                    {
                        result = cn.Query(@"UPDATE Member SET MemberStatus = @MemberStatus, MemberName = @MemberName, MemberEmail = @MemberEmail
                                    , MemberLevel = @MemberLevel, MemberSalaly = @MemberSalaly, MemberInterests = @MemberInterests WHERE MemberID=@MemberID"
                            , new
                            {
                                MemberID = this.MemberID,
                                MemberStatus = this.MemberStatus,
                                MemberName = this.MemberName,
                                MemberEmail = this.MemberEmail,
                                MemberLevel = this.MemberLevel,
                                MemberSalaly = this.MemberSalaly,
                                MemberInterests = this.MemberInterests
                            });
                    }
                    else
                    {
                        result = cn.Query(@"UPDATE Member SET MemberPassword = @MemberPassword, MemberStatus = @MemberStatus, MemberName = @MemberName
                                    , MemberEmail = @MemberEmail, MemberLevel = @MemberLevel, MemberSalaly = @MemberSalaly, MemberInterests = @MemberInterests 
                                    WHERE MemberID=@MemberID"
                            , new
                            {
                                MemberID = this.MemberID,
                                MemberPassword = this.MemberPassword,
                                MemberStatus = this.MemberStatus,
                                MemberName = this.MemberName,
                                MemberEmail = this.MemberEmail,
                                MemberLevel = this.MemberLevel,
                                MemberSalaly = this.MemberSalaly,
                                MemberInterests = this.MemberInterests
                            });

                    }
                    retObj = result > 0 ? "0" : "修改失敗";
                }
            }
            catch (Exception ex)
            {
                retObj = ex.ToString();
            }
            return retObj;
        }
        #endregion

        #region public object GetOne(string BankId)
        /// <summary>
        /// 取得一筆會員資料
        /// </summary>    
        /// <param name="key">會員ID</param>
        /// <returns>0成功; string失敗</returns>
        public object GetOne(int key)
        {
            object retObj = 0;
            try
            {
                DB_Member _results = new DB_Member();
                string ConnStr = WebConfigurationManager.ConnectionStrings["DBTESTConnStr"].ConnectionString;

                using (var cn = new SqlConnection(ConnStr))
                {
                    _results = cn.QuerySingleOrDefault<DB_Member>(@"SELECT * FROM Interest");
                }

                if (!string.IsNullOrEmpty(_results.MemberAccount))
                {
                    retObj = _results;
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
        /// 刪除會員資料
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
                    cn.Query("DELETE FROM Member WHERE MemberID=@id", new { id = key });
                }
            }
            catch (Exception ex)
            {
                retObj = ex.ToString();
            }
            return 0;
        }
        #endregion

        #endregion
    }
}