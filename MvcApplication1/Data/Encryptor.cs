using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static WebApi.Data.TypeEnum;

namespace WebApi.Data
{
    public class Security
    {
        #region Global Variables

        /// <summary>
        /// 產生驗證碼的集合
        /// </summary>
        private static char[] CodeArray = new char[35] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'W', 'X', 'Y', 'Z' };

        #endregion

        #region public static string EncodePassword(string input, SecurityType securityType)

        /// <summary>
        /// 加密密碼信息
        /// </summary>
        /// <param name="input">需加密的密碼</param>
        /// <param name="securityType">加密類型</param>
        /// <returns>已經加密的密碼</returns>
        public static string EncodePassword(string input, SecurityType securityType)
        {
            byte[] b = System.Text.Encoding.Default.GetBytes(input);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');

            switch (securityType)
            {
                case SecurityType.Intact: return ret.ToLower();
                case SecurityType.UnIntact: return ret.ToLower().Substring(8, 16);
            }

            return string.Empty;
        }

        #endregion

        #region public static string ConvertTo64Code(string input)

        /// <summary>
        /// 轉化為Base64Code
        /// </summary>
        /// <param name="input">需轉換的字符</param>
        /// <returns>返回Base64Code</returns>
        public static string ConvertTo64Code(string input)
        {
            byte[] bytes = ASCIIEncoding.UTF8.GetBytes(input);

            return Convert.ToBase64String(bytes);
        }

        #endregion

        #region public static string ConvertFrom64Code(string input)

        /// <summary>
        /// 反轉Base64Code
        /// </summary>
        /// <param name="input">需反轉的字符串</param>
        /// <returns>返回反轉後的字符串</returns>
        public static string ConvertFrom64Code(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);

            return ASCIIEncoding.UTF8.GetString(bytes);
        }

        #endregion

        #region public static string GetRandomCode(int codeCount,int maxValue)

        /// <summary>
        /// 產生隨機的亂數
        /// </summary>
        /// <param name="codeCount">需產生隨機亂數的個數</param>
        /// <param name="maxValue">隨機函數的最大值</param>
        /// <returns>返回隨機亂數</returns>
        public static string GetRandomCode(int codeCount, int maxValue)
        {
            StringBuilder validateCode = new StringBuilder(codeCount);

            Random rd = new Random(unchecked((int)DateTime.Now.Ticks));

            // 亂數產生驗證文字
            for (int i = 0; i < codeCount; i++)
            {
                validateCode.Append(CodeArray[rd.Next(maxValue)]);
            }

            return validateCode.ToString();
        }

        #endregion
    }
    public class Encryptor
    {
        private static byte[] _keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private static string _decryptedResult = string.Empty;
        private static string _encryptedResult = string.Empty;

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="encryptString">要加密的資料</param>
        /// <param name="encryptKey">用來加密的 Key</param>
        /// <returns></returns>
        public static string Encrypt(string encryptString, string encryptKey)
        {
            if (encryptKey.Trim().Length != 8 | string.IsNullOrEmpty(encryptString))
            {
                return string.Empty;
            }

            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = _keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);

            try
            {
                cStream.FlushFinalBlock();
                _encryptedResult = Convert.ToBase64String(mStream.ToArray());
            }
            catch (System.Exception)
            {
                _encryptedResult = string.Empty;
            }
            return _encryptedResult;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="decryptString">要解密的資料</param>
        /// <param name="decryptKey">用來解密的 Key</param>
        /// <returns></returns>
        public static string Decrypt(string decryptString, string decryptKey)
        {
            if (decryptKey.Trim().Length != 8 | string.IsNullOrEmpty(decryptString))
            {
                return string.Empty;
            }

            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
            byte[] rgbIV = _keys;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);

            try
            {
                cStream.FlushFinalBlock();
                _decryptedResult = Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                _decryptedResult = string.Empty;
            }
            catch (System.Exception)
            {
                _decryptedResult = string.Empty;
            }
            return _decryptedResult;
        }
    }
}