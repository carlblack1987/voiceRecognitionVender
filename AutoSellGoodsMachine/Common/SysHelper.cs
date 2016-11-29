#region [ KIMMA Co.,Ltd. Copyright (C) 2013 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件公共处理类
// 业务功能：iVend终端软件公共处理类
// 创建标识：2013-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Text;
using System.Threading;

namespace AutoSellGoodsMachine
{
    public class SysHelper
    {
        #region 公共函数

        /// <summary>
        /// 根据网络通信类型获取通信类型名称
        /// </summary>
        /// <param name="netType">通信类型编码</param>
        /// <returns>通信类型名称</returns>
        public static string GetNetTypeName(string netType)
        {
            string strNetTypeName = string.Empty;

            switch (netType)
            {
                case "0":// DTU
                    strNetTypeName = "DTU";
                    break;

                case "1":// 3G
                    strNetTypeName = "3G";
                    break;

                case "2":// WIFI
                    strNetTypeName = "WIFI";
                    break;

                case "3":// 有线
                    strNetTypeName = "Internet";
                    break;
            }

            return strNetTypeName;
        }

        /// <summary>
        /// 根据机器型号编码来获取机器型号名称
        /// </summary>
        /// <param name="vmType">机器型号编码</param>
        /// <returns>机器型号名称</returns>
        public static string GetVmTypeName(string vmType)
        {
            string strVmTypeName = string.Empty;

            switch (vmType)
            {
                case "10":// G636
                    strVmTypeName = "G636";
                    break;

                case "11":// G654
                    strVmTypeName = "G654";
                    break;

                case "21":// G654M
                    strVmTypeName = "G654M";
                    break;

                default:
                    break;
            }

            return strVmTypeName;
        }

        /// <summary>
        /// 把字符型的转换为*
        /// </summary>
        /// <param name="pwdValue">要转换的字符串</param>
        /// <returns>转换后的*串</returns>
        public static string ChangePwdToMi(string pwdValue)
        {
            int intLen = pwdValue.Length;
            string strValue = string.Empty;
            for (int i = 0; i < intLen; i++)
            {
                strValue += "*";
            }
            return strValue;
        }

        #endregion
    }
}
