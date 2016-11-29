#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：支付宝声波支付业务处理类
// 创建标识：2014-09-20		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using AliPayWave;
using JsonPlug;

namespace Business.Common
{
    public class AliPayWaveHelper
    {
        #region 公共变量声明

        /// <summary>
        /// 支付宝声波支付对象
        /// </summary>
        private AliPayWaveOper m_AliPayWave = new AliPayWaveOper();

        #endregion

        #region 私有变量声明

        /// <summary>
        /// 支付宝声波支付验证码
        /// </summary>
        private string m_WaveIdData = string.Empty;

        #endregion

        #region 属性

        private int m_AliPayWaveStatus = 1;
        /// <summary>
        /// 支付宝声波支付获取声波码状态
        /// </summary>
        public int AliPayWaveStatus
        {
            get { return m_AliPayWaveStatus; }
        }

        private string m_VmId = string.Empty;
        /// <summary>
        /// 机器编号
        /// </summary>
        public string VmId
        {
            get { return m_VmId; }
            set { m_VmId = value; }
        }

        private string m_AliPayWebUrl = string.Empty;
        /// <summary>
        /// 支付宝声波支付WebUrl
        /// </summary>
        public string AliPayWebUrl
        {
            get { return m_AliPayWebUrl; }
            set { m_AliPayWebUrl = value; }
        }

        private int m_WaveTimeOut = 100;
        /// <summary>
        /// 支付宝声波支付声波信号监听超时，以秒为单位
        /// </summary>
        public int WaveTimeOut
        {
            get { return m_WaveTimeOut; }
            set { m_WaveTimeOut = value; }
        }

        #endregion

        #region 公有函数

        /// <summary>
        /// 开始声波支付监听
        /// </summary>
        /// <returns>结果代码</returns>
        public int StartAliPayWave()
        {
            int intErrCode = 0;

            m_WaveIdData = string.Empty;
            m_AliPayWaveStatus = 1;

            m_AliPayWave = new AliPayWaveOper();

            m_AliPayWave.WaveTimeOut = m_WaveTimeOut;

            m_AliPayWave.StartReceiveData();

            Thread TrdAliPayWaveMon = new Thread(new ThreadStart(AliPayWaveMonTrd));
            TrdAliPayWaveMon.IsBackground = true;
            TrdAliPayWaveMon.Start();

            return intErrCode;
        }

        /// <summary>
        /// 停止声波支付监听
        /// </summary>
        /// <returns></returns>
        public int StopAliPayWave()
        {
            int intErrCode = 0;

            return intErrCode;
        }

        /// <summary>
        /// 提交支付宝声波支付订单
        /// </summary>
        /// <param name="money">金额</param>
        /// <returns>结果代码 0：支付成功 1：订单请求失败 9：未知错误</returns>
        public int AliPayOrder(string money)
        {
            int intErrCode = 0;

            string strData = string.Empty;
            try
            {
                bool result = m_AliPayWave.AliPay_Order(m_VmId, m_WaveIdData, money, out strData);
                if (result)
                {
                    JsonOper jsonOper = new JsonOper();
                    string strCode = jsonOper.GetJsonKeyValue(strData, "ret");
                    switch (strCode)
                    {
                        case "0":// 支付成功
                            break;

                        default:
                            //lblTip.Text = "支付失败，系统代码：" + strCode + "，支付宝码：" + jsonOper.GetJsonKeyValue(strData, "alicode");
                            //SleepControl();
                            break;
                    }
                }
                else
                {
                    // Http请求失败
                    intErrCode = 1;
                }
            }
            catch
            {
                intErrCode = 9;
            }

            return intErrCode;
        }

        #endregion

        private void AliPayWaveMonTrd()
        {
            bool blnIsExit = false;
            try
            {
                while (true)
                {
                    Thread.Sleep(20);

                    switch (m_AliPayWave.Status)
                    {
                        case 2:// 监听到数据
                            m_WaveIdData = m_AliPayWave.RecData;
                            blnIsExit = true;
                            break;

                        case 3:// 监听超时
                            blnIsExit = true;
                            break;

                        case 4:// 监听错误
                            blnIsExit = true;
                            break;
                    }
                    if (blnIsExit)
                    {
                        break;
                    }
                }

                m_AliPayWave.Destroy();

                m_AliPayWaveStatus = m_AliPayWave.Status;
            }
            catch
            {
            }
        }
    }
}
