#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：条形码扫描处理类
// 创建标识：2015-03-16		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BarCodeScan;

namespace Business.Common
{
    public class BarCodeHelper
    {
        #region 变量声明

        private ScanOper m_ScanOper = new ScanOper();

        #endregion

        #region 属性

        /// <summary>
        /// 是否记录通信日志到文件，False：不记录 True：记录
        /// </summary>
        public bool IsLogToFile
        {
            get { return m_ScanOper.IsLogToFile; }
            set { m_ScanOper.IsLogToFile = value; }
        }

        /// <summary>
        /// 是否记录通信日志到队列，False：不记录 True：记录
        /// </summary>
        public bool IsLogToQueue
        {
            get { return m_ScanOper.IsLogToQueue; }
            set { m_ScanOper.IsLogToQueue = value; }
        }

        /// <summary>
        /// 串口号
        /// </summary>
        public int ComPort
        {
            get { return m_ScanOper.ComPort; }
            set { m_ScanOper.ComPort = value; }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate
        {
            get { return m_ScanOper.BaudRate; }
        }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version
        {
            get { return m_ScanOper.Version; }
        }
        /// <summary>
        /// 组件名称
        /// </summary>
        public string SoftName
        {
            get { return m_ScanOper.SoftName; }
        }

        private string m_BarCodeDeviceStatus = "01";
        /// <summary>
        /// 条形码设备状态
        /// </summary>
        public string BarCodeDeviceStatus
        {
            get { return m_BarCodeDeviceStatus; }
        }

        #endregion

        #region 公共接口函数

        /// <summary>
        /// 初始化条形码扫描设备
        /// </summary>
        /// <returns></returns>
        public int InitBarCodeScan()
        {
            int intErrCode = 0;

            string strLogType = "InitBarCodeScan";

            LogHelper.AddBusLog(strLogType + "  " + "Port:" + ComPort);

            intErrCode = Initialize();
            if (intErrCode == 0)
            {
                // 初始化成功，设置条形码设备参数
                intErrCode = m_ScanOper.SetScanPara();
            }

            string strStatus = string.Empty;
            if (intErrCode != 0)
            {
                strStatus = "01";
            }
            else
            {
                strStatus = "02";
            }
            m_BarCodeDeviceStatus = strStatus;

            LogHelper.AddBusLog_Code(strLogType, intErrCode.ToString(), strStatus);

            return intErrCode;
        }

        /// <summary>
        /// 初始化扫描设备
        /// </summary>
        /// <returns></returns>
        public int Initialize()
        {
            m_ScanOper.Displose();
            return m_ScanOper.Initialize();
        }

        /// <summary>
        /// 设置扫描参数
        /// </summary>
        /// <returns></returns>
        public int SetScanPara()
        {
            return m_ScanOper.SetScanPara();
        }

        /// <summary>
        /// 检测扫描设备状态
        /// </summary>
        /// <returns></returns>
        public int CheckDeviceStatus()
        {
            string strLogType = "QueryBarCodeScanStatus";

            // 如果开启条形码扫描终端
            int intErrCode = m_ScanOper.CheckDeviceStatus();

            if (intErrCode == 0)
            {
                m_BarCodeDeviceStatus = "02";
            }
            else
            {
                m_BarCodeDeviceStatus = "01";
            }

            LogHelper.AddBusLog_Code(strLogType, intErrCode.ToString(), m_BarCodeDeviceStatus);

            return intErrCode;
        }

        /// <summary>
        /// 查询扫描结果
        /// </summary>
        /// <param name="_cardData"></param>
        /// <param name="_errICCode"></param>
        /// <returns></returns>
        public int QueryBarCodeNum(out string _cardData, out string _errICCode)
        {
            _cardData = string.Empty;
            _errICCode = string.Empty;

            return m_ScanOper.QueryBarCodeNum(out _cardData, out _errICCode);
        }

        /// <summary>
        /// 清除扫描内容
        /// </summary>
        /// <returns></returns>
        public int ClearBarCodeNum()
        {
            return m_ScanOper.ClearBarCodeNum();
        }

        /// <summary>
        /// 开始扫描
        /// </summary>
        /// <returns></returns>
        public int BeginScan()
        {
            string strLogType = "BeginBarCodeScan";

            int intErrCode = m_ScanOper.BeginScan();

            LogHelper.AddBusLog_Code(strLogType, intErrCode.ToString(), "");

            return intErrCode;
        }

        /// <summary>
        /// 停止扫描
        /// </summary>
        /// <returns></returns>
        public int StopScan()
        {
            string strLogType = "StopBarCodeScan";

            int intErrCode = m_ScanOper.StopScan();

            LogHelper.AddBusLog_Code(strLogType, intErrCode.ToString(), "");

            return intErrCode;
        }

        #endregion
    }
}
