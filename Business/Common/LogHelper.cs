#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：日志处理类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Business
{
    public class LogHelper
    {
        #region 日志处理

        #region 属性

        private static string m_LogName = "Log_Business";

        public static bool IsLogToFile = true;

        #endregion

        #region 变量

        /// <summary>
        /// 日志文件夹路径名称
        /// </summary>
        private static string m_LogDire = string.Empty;

        #endregion

        public static void AddBusLog_Code(string logType, string code, string data)
        {
            AddBusLog(logType + "  " + "Code:" + code + "  " + "Data:" + data);
        }

        public static void AddErrLog(string logType, string code, string errInfo)
        {
            AddBusLog(logType + "  " + "Code:" + code + "  " + "Error:" + errInfo);
        }

        /// <summary>
        /// 记录组件日志
        /// </summary>
        /// <param name="logInfo">日志内容</param>
        public static void AddBusLog(string logInfo)
        {
            try
            {
                if (!IsLogToFile)
                {
                    // 不记录日志
                    return;
                }

                m_LogDire = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Log\\";
                if (!Directory.Exists(m_LogDire))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(m_LogDire);
                }
                m_LogDire += m_LogName + "\\";
                if (!Directory.Exists(m_LogDire))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(m_LogDire);
                }

                // 获取当前日志的文件夹
                string strLogFile = m_LogDire + "\\" + m_LogName + "_" + DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(strLogFile))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(strLogFile);
                }

                File.AppendAllText(strLogFile +
                        "\\" + m_LogName + "_" + DateTime.Now.ToString("yyyyMMdd_HH") + ".log",
                        System.DateTime.Now.ToString("HH:mm:ss:fff") + "  " + logInfo + "\r\n",
                        Encoding.Default);
            }
            catch
            {
            }
        }

        #endregion
    }
}
