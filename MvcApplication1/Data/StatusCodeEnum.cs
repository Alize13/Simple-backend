using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Data
{
    public class TypeEnum
    {
        #region public enum SecurityType
        /// <summary>
        /// 密碼類別類型
        /// </summary>
        public enum SecurityType
        {
            /// <summary>
            /// MD5取32的長度
            /// </summary>
            Intact = 0,
            /// <summary>
            /// MD5取16的長度
            /// </summary>
            UnIntact = 1
        }
        #endregion



        #region public enum StatusCode

        /// <summary>
        /// 狀態代碼
        /// </summary>
        public enum StatusCode
        {
            /// <summary>
            /// 執行成功
            /// </summary>
            OK = 200,

            /// <summary>
            /// 登入錯誤
            /// </summary>
            MemberLoginError = 201,

            /// <summary>
            /// 無法識別的請求
            /// </summary>
            BadRequest = 400,

            /// <summary>
            /// 服務器拒絕請求
            /// </summary>
            Forbidden = 403,

            /// <summary>
            /// 請求的資源不在服務器上
            /// </summary>
            NotFound = 404,

            /// <summary>
            /// 內部錯誤
            /// </summary>
            InternalServerError = 500,

            /// <summary>
            /// Session錯誤
            /// </summary>
            SessionError = 501,

            /// <summary>
            ///ApiToekn錯誤
            /// </summary>
            ApiToeknError = 502,
            /// <summary>
            ///ApiReferrer錯誤
            /// </summary>
            ApiReferrerError = 503,

            /// <summary>
            /// 回應顯示的錯誤訊息
            /// </summary>
            ApiMsgError = 599,

            /// <summary>
            /// 參數有誤
            /// </summary>
            ParameterIncorrect = 615,


        }

        #endregion

    }
}