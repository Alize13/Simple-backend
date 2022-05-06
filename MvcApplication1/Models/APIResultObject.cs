using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static WebApi.Data.TypeEnum;

namespace MvcApplication1.Models
{
    public class APIResultObject
    {
        #region Private property

        private StatusCode _statusCode;  // 狀態參數
        private string _message = string.Empty; // 信息
        private object _retObj = null; // 回傳資料集合
        #endregion

        #region Public  property

        /// <summary>
        /// 狀態參數
        /// </summary>
        public StatusCode Code
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        /// <summary>
        /// 信息
        /// </summary>
        public string ErrMsg
        {
            get { return _message; }
            set { _message = value; }
        }
        /// <summary>
        /// 狀態參數
        /// </summary>
        public object RetObj
        {
            get { return _retObj; }
            set { _retObj = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// 構造函數
        /// </summary>
        public APIResultObject()
        {
        }

        /// <summary>
        /// 構造函數
        /// </summary>
        /// <param name="statusCode">狀態代碼</param>
        /// <param name="retObj">結果對象</param>
        /// <param name="retObj">消息</param>
        public APIResultObject(StatusCode statusCode, object retObj, string message = "")
        {
            _statusCode = statusCode;
            _retObj = retObj;
            if (!message.Equals(""))
            {
                _message = message;
            }
        }


        #region public void Parse(string inputJson)

        /// <summary>
        /// 解析Json格式
        /// </summary>
        /// <param name="inputJson">Json字符串</param>
        public void Parse(string inputJson)
        {
            if (string.IsNullOrEmpty(inputJson)) return;
            Dictionary<string, object> propList = JsonConvert.DeserializeObject<Dictionary<string, object>>(inputJson);

            Code = propList.ContainsKey("Code") ? (StatusCode)Int32.Parse(propList["Code"].ToString()) : StatusCode.ApiMsgError;
            ErrMsg = propList.ContainsKey("ErrMsg") ? propList["ErrMsg"].ToString() : string.Empty;
            RetObj = propList.ContainsKey("RetObj") ? JsonConvert.SerializeObject(propList["RetObj"]) : string.Empty;
            
        }

        #endregion
        #endregion
    }
}