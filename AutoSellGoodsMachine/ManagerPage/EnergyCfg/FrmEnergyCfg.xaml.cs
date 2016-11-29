#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：节能设置
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmEnergyCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmEnergyCfg : Window
    {
        private bool m_IsInit = true;

        public FrmEnergyCfg()
        {
            InitializeComponent();
            InitForm();
            LoadData();
        }

        private void InitForm()
        {
            #region 加载时间段下拉列表

            string strHour = string.Empty;
            for (int i = 0; i < 24; i++)
            {
                strHour = i.ToString().PadLeft(2, '0');

                cmbLight_BeginTime1_Hour.Items.Add(strHour);
                cmbLight_EndTime1_Hour.Items.Add(strHour);
                cmbLight_BeginTime2_Hour.Items.Add(strHour);
                cmbLight_EndTime2_Hour.Items.Add(strHour);

                cmbAdvert_BeginTime1_Hour.Items.Add(strHour);
                cmbAdvert_EndTime1_Hour.Items.Add(strHour);
                cmbAdvert_BeginTime2_Hour.Items.Add(strHour);
                cmbAdvert_EndTime2_Hour.Items.Add(strHour);

                cmbChuWu_BeginTime1_Hour.Items.Add(strHour);
                cmbChuWu_EndTime1_Hour.Items.Add(strHour);
                cmbChuWu_BeginTime2_Hour.Items.Add(strHour);
                cmbChuWu_EndTime2_Hour.Items.Add(strHour);

                cmbScreen_BeginTime1_Hour.Items.Add(strHour);
                cmbScreen_EndTime1_Hour.Items.Add(strHour);
                cmbScreen_BeginTime2_Hour.Items.Add(strHour);
                cmbScreen_EndTime2_Hour.Items.Add(strHour);
            }

            string strMin = string.Empty;
            for (int i = 0; i < 60; i++)
            {
                strMin = i.ToString().PadLeft(2, '0');

                cmbLight_BeginTime1_Min.Items.Add(strMin);
                cmbLight_EndTime1_Min.Items.Add(strMin);
                cmbLight_BeginTime2_Min.Items.Add(strMin);
                cmbLight_EndTime2_Min.Items.Add(strMin);

                cmbAdvert_BeginTime1_Min.Items.Add(strMin);
                cmbAdvert_EndTime1_Min.Items.Add(strMin);
                cmbAdvert_BeginTime2_Min.Items.Add(strMin);
                cmbAdvert_EndTime2_Min.Items.Add(strMin);

                cmbChuWu_BeginTime1_Min.Items.Add(strMin);
                cmbChuWu_EndTime1_Min.Items.Add(strMin);
                cmbChuWu_BeginTime2_Min.Items.Add(strMin);
                cmbChuWu_EndTime2_Min.Items.Add(strMin);

                cmbScreen_BeginTime1_Min.Items.Add(strMin);
                cmbScreen_EndTime1_Min.Items.Add(strMin);
                cmbScreen_BeginTime2_Min.Items.Add(strMin);
                cmbScreen_EndTime2_Min.Items.Add(strMin);
            }

            #endregion

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Energy");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");

            string strModel_Time = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_0");
            string strModel_Run = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_1");
            string strModel_Stop = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_2");
            string strTime1 = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_0_Time1");
            string strTime2 = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_0_Time2");

            #region 初始化照明标签

            tbLight_Title.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Jacklight");
            rdbLightModel_Time.Content = strModel_Time;
            rdbLightModel_Run.Content = strModel_Run;
            rdbLightModel_Stop.Content = strModel_Stop;
            tbLight_Time1.Text = strTime1;
            tbLight_Time2.Text = strTime2;

            #endregion

            #region 初始化广告灯标签

            tbAdvert_Title.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Advlight");
            rdbAdvertModel_Time.Content = strModel_Time;
            rdbAdvertModel_Run.Content = strModel_Run;
            rdbAdvertModel_Stop.Content = strModel_Stop;
            tbAdvert_Time1.Text = strTime1;
            tbAdvert_Time2.Text = strTime2;

            #endregion

            #region 初始化除雾设备标签

            tbChuWu_Title.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_ChuWuModel");
            rdbChuWuModel_Time.Content = strModel_Time;
            rdbChuWuModel_Run.Content = strModel_Run;
            rdbChuWuModel_Stop.Content = strModel_Stop;
            tbChuWu_Time1.Text = strTime1;
            tbChuWu_Time2.Text = strTime2;

            #endregion

            #region 初始化屏幕标签

            tbScreen_Title.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Screen");
            rdbScreenModel_Time.Content = strModel_Time;
            rdbScreenModel_Run.Content = strModel_Run;
            rdbScreenModel_Stop.Content = strModel_Stop;
            tbScreen_Time1.Text = strTime1;
            tbScreen_Time2.Text = strTime2;

            #endregion
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadData()
        {
            bool enable = false;
            string strModel = string.Empty;
            string strBeginTime1 = string.Empty;
            string strEndTime1 = string.Empty;
            string strBeginTime2 = string.Empty;
            string strEndTime2 = string.Empty;

            #region 加载照明灯数据

            enable = false;
            strModel = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("LightModel");
            switch (strModel)
            {
                case "0":// 定时开启
                    enable = true;
                    rdbLightModel_Time.IsChecked = true;
                    strBeginTime1 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("LightBeginTime1");
                    strEndTime1 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("LightEndTime1");
                    strBeginTime2 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("LightBeginTime2");
                    strEndTime2 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("LightEndTime2");

                    if (strBeginTime1.Length == 4)
                    {
                        cmbLight_BeginTime1_Hour.Text = strBeginTime1.Substring(0, 2);
                        cmbLight_BeginTime1_Min.Text = strBeginTime1.Substring(2);
                    }
                    if (strEndTime1.Length == 4)
                    {
                        cmbLight_EndTime1_Hour.Text = strEndTime1.Substring(0, 2);
                        cmbLight_EndTime1_Min.Text = strEndTime1.Substring(2);
                    }
                    if (strBeginTime2.Length == 4)
                    {
                        cmbLight_BeginTime2_Hour.Text = strBeginTime2.Substring(0, 2);
                        cmbLight_BeginTime2_Min.Text = strBeginTime2.Substring(2);
                    }
                    if (strEndTime2.Length == 4)
                    {
                        cmbLight_EndTime2_Hour.Text = strEndTime2.Substring(0, 2);
                        cmbLight_EndTime2_Min.Text = strEndTime2.Substring(2);
                    }
                    break;
                case "1":// 全时段开启
                    rdbLightModel_Run.IsChecked = true;
                    break;
                case "2":// 全时段关闭
                    rdbLightModel_Stop.IsChecked = true;
                    break;
            }
            ControlCmbLight(enable);

            #endregion

            #region 加载广告灯数据

            enable = false;
            strModel = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdLampModel");
            switch (strModel)
            {
                case "0":// 定时开启
                    enable = true;
                    rdbAdvertModel_Time.IsChecked = true;
                    strBeginTime1 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdLampBeginTime1");
                    strEndTime1 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdLampEndTime1");
                    strBeginTime2 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdLampBeginTime2");
                    strEndTime2 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdLampEndTime2");

                    if (strBeginTime1.Length == 4)
                    {
                        cmbAdvert_BeginTime1_Hour.Text = strBeginTime1.Substring(0, 2);
                        cmbAdvert_BeginTime1_Min.Text = strBeginTime1.Substring(2);
                    }
                    if (strEndTime1.Length == 4)
                    {
                        cmbAdvert_EndTime1_Hour.Text = strEndTime1.Substring(0, 2);
                        cmbAdvert_EndTime1_Min.Text = strEndTime1.Substring(2);
                    }
                    if (strBeginTime2.Length == 4)
                    {
                        cmbAdvert_BeginTime2_Hour.Text = strBeginTime2.Substring(0, 2);
                        cmbAdvert_BeginTime2_Min.Text = strBeginTime2.Substring(2);
                    }
                    if (strEndTime2.Length == 4)
                    {
                        cmbAdvert_EndTime2_Hour.Text = strEndTime2.Substring(0, 2);
                        cmbAdvert_EndTime2_Min.Text = strEndTime2.Substring(2);
                    }
                    break;
                case "1":// 全时段开启
                    rdbAdvertModel_Run.IsChecked = true;
                    break;
                case "2":// 全时段关闭
                    rdbAdvertModel_Stop.IsChecked = true;
                    break;
            }
            ControlCmbAdvert(enable);

            #endregion

            #region 加载除雾设备数据

            enable = false;
            strModel = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("FogModel");
            switch (strModel)
            {
                case "0":// 定时开启
                    enable = true;
                    rdbChuWuModel_Time.IsChecked = true;
                    strBeginTime1 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("FogBeginTime1");
                    strEndTime1 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("FogEndTime1");
                    strBeginTime2 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("FogBeginTime2");
                    strEndTime2 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("FogEndTime2");

                    if (strBeginTime1.Length == 4)
                    {
                        cmbChuWu_BeginTime1_Hour.Text = strBeginTime1.Substring(0, 2);
                        cmbChuWu_BeginTime1_Min.Text = strBeginTime1.Substring(2);
                    }
                    if (strEndTime1.Length == 4)
                    {
                        cmbChuWu_EndTime1_Hour.Text = strEndTime1.Substring(0, 2);
                        cmbChuWu_EndTime1_Min.Text = strEndTime1.Substring(2);
                    }
                    if (strBeginTime2.Length == 4)
                    {
                        cmbChuWu_BeginTime2_Hour.Text = strBeginTime2.Substring(0, 2);
                        cmbChuWu_BeginTime2_Min.Text = strBeginTime2.Substring(2);
                    }
                    if (strEndTime2.Length == 4)
                    {
                        cmbChuWu_EndTime2_Hour.Text = strEndTime2.Substring(0, 2);
                        cmbChuWu_EndTime2_Min.Text = strEndTime2.Substring(2);
                    }
                    break;
                case "1":// 全时段开启
                    rdbChuWuModel_Run.IsChecked = true;
                    break;
                case "2":// 全时段关闭
                    rdbChuWuModel_Stop.IsChecked = true;
                    break;
            }
            ControlCmbChuWu(enable);

            #endregion

            #region 加载显示屏数据

            enable = false;
            strModel = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("PowerModel");
            switch (strModel)
            {
                case "0":// 定时开启
                    enable = true;
                    rdbScreenModel_Time.IsChecked = true;
                    strBeginTime1 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("PowerBeginTime1");
                    strEndTime1 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("PowerEndTime1");
                    strBeginTime2 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("PowerBeginTime2");
                    strEndTime2 = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("PowerEndTime2");

                    if (strBeginTime1.Length == 4)
                    {
                        cmbScreen_BeginTime1_Hour.Text = strBeginTime1.Substring(0, 2);
                        cmbScreen_BeginTime1_Min.Text = strBeginTime1.Substring(2);
                    }
                    if (strEndTime1.Length == 4)
                    {
                        cmbScreen_EndTime1_Hour.Text = strEndTime1.Substring(0, 2);
                        cmbScreen_EndTime1_Min.Text = strEndTime1.Substring(2);
                    }
                    if (strBeginTime2.Length == 4)
                    {
                        cmbScreen_BeginTime2_Hour.Text = strBeginTime2.Substring(0, 2);
                        cmbScreen_BeginTime2_Min.Text = strBeginTime2.Substring(2);
                    }
                    if (strEndTime2.Length == 4)
                    {
                        cmbScreen_EndTime2_Hour.Text = strEndTime2.Substring(0, 2);
                        cmbScreen_EndTime2_Min.Text = strEndTime2.Substring(2);
                    }
                    break;
                case "1":// 全时段开启
                    rdbScreenModel_Run.IsChecked = true;
                    break;
                case "2":// 全时段关闭
                    rdbScreenModel_Stop.IsChecked = true;
                    break;
            }
            ControlCmbScreen(enable);

            #endregion

            m_IsInit = false;
        }

        private void ControlCmbLight(bool enable)
        {
            cmbLight_BeginTime1_Hour.IsEnabled = cmbLight_BeginTime1_Min.IsEnabled = cmbLight_EndTime1_Hour.IsEnabled =
                cmbLight_EndTime1_Min.IsEnabled = cmbLight_BeginTime2_Hour.IsEnabled = cmbLight_BeginTime2_Min.IsEnabled =
                cmbLight_EndTime2_Hour.IsEnabled = cmbLight_EndTime2_Min.IsEnabled = enable;
        }

        private void ControlCmbAdvert(bool enable)
        {
            cmbAdvert_BeginTime1_Hour.IsEnabled = cmbAdvert_BeginTime1_Min.IsEnabled = cmbAdvert_EndTime1_Hour.IsEnabled =
                cmbAdvert_EndTime1_Min.IsEnabled = cmbAdvert_BeginTime2_Hour.IsEnabled = cmbAdvert_BeginTime2_Min.IsEnabled =
                cmbAdvert_EndTime2_Hour.IsEnabled = cmbAdvert_EndTime2_Min.IsEnabled = enable;
        }

        private void ControlCmbChuWu(bool enable)
        {
            cmbChuWu_BeginTime1_Hour.IsEnabled = cmbChuWu_BeginTime1_Min.IsEnabled = cmbChuWu_EndTime1_Hour.IsEnabled =
                cmbChuWu_EndTime1_Min.IsEnabled = cmbChuWu_BeginTime2_Hour.IsEnabled = cmbChuWu_BeginTime2_Min.IsEnabled =
                cmbChuWu_EndTime2_Hour.IsEnabled = cmbChuWu_EndTime2_Min.IsEnabled = enable;
        }

        private void ControlCmbScreen(bool enable)
        {
            cmbScreen_BeginTime1_Hour.IsEnabled = cmbScreen_BeginTime1_Min.IsEnabled = cmbScreen_EndTime1_Hour.IsEnabled =
                cmbScreen_EndTime1_Min.IsEnabled = cmbScreen_BeginTime2_Hour.IsEnabled = cmbScreen_BeginTime2_Min.IsEnabled =
                cmbScreen_EndTime2_Hour.IsEnabled = cmbScreen_EndTime2_Min.IsEnabled = enable;
        }

        private void ControlButton(bool enable)
        {
            btnSave.IsEnabled = enable;
            btnCancel.IsEnabled = enable;
            DispatcherHelper.SleepControl();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strModel = string.Empty;
            bool result = false;
            string strErrInfo = string.Empty;
            string strBeginTime1 = string.Empty;
            string strEndTime1 = string.Empty;
            string strBeginTime2 = string.Empty;
            string strEndTime2 = string.Empty;

            ControlButton(false);
            DispatcherHelper.DoEvents();

            #region 保存照明灯模式
                        
            if (rdbLightModel_Time.IsChecked == true)
            {
                strModel = "0";
            }
            if (rdbLightModel_Run.IsChecked == true)
            {
                strModel = "1";
            }
            if (rdbLightModel_Stop.IsChecked == true)
            {
                strModel = "2";
            }

            if (strModel == "0")
            {
                // 检测时间段是否有效
                result = false;
                strBeginTime1 = cmbLight_BeginTime1_Hour.Text + cmbLight_BeginTime1_Min.Text;
                strEndTime1 = cmbLight_EndTime1_Hour.Text + cmbLight_EndTime1_Min.Text;
                strBeginTime2 = cmbLight_BeginTime2_Hour.Text + cmbLight_BeginTime2_Min.Text;
                strEndTime2 = cmbLight_EndTime2_Hour.Text + cmbLight_EndTime2_Min.Text;
                result = PubHelper.CheckModelTime(strBeginTime1, strEndTime1, strBeginTime2, strEndTime2, out strErrInfo);
                if (!result)
                {
                    PubHelper.ShowMsgInfo(strErrInfo, PubHelper.MsgType.Ok);
                    ControlButton(true);
                    return;
                }
                PubHelper.p_BusinOper.UpdateSysCfgValue("LightBeginTime1", strBeginTime1);
                PubHelper.p_BusinOper.UpdateSysCfgValue("LightEndTime1", strEndTime1);
                PubHelper.p_BusinOper.UpdateSysCfgValue("LightBeginTime2", strBeginTime2);
                PubHelper.p_BusinOper.UpdateSysCfgValue("LightEndTime2", strEndTime2);
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("LightModel", strModel);

            #endregion

            #region 保存广告灯灯模式

            if (rdbAdvertModel_Time.IsChecked == true)
            {
                strModel = "0";
            }
            if (rdbAdvertModel_Run.IsChecked == true)
            {
                strModel = "1";
            }
            if (rdbAdvertModel_Stop.IsChecked == true)
            {
                strModel = "2";
            }

            if (strModel == "0")
            {
                // 检测时间段是否有效
                result = false;
                strBeginTime1 = cmbAdvert_BeginTime1_Hour.Text + cmbAdvert_BeginTime1_Min.Text;
                strEndTime1 = cmbAdvert_EndTime1_Hour.Text + cmbAdvert_EndTime1_Min.Text;
                strBeginTime2 = cmbAdvert_BeginTime2_Hour.Text + cmbAdvert_BeginTime2_Min.Text;
                strEndTime2 = cmbAdvert_EndTime2_Hour.Text + cmbAdvert_EndTime2_Min.Text;
                result = PubHelper.CheckModelTime(strBeginTime1, strEndTime1, strBeginTime2, strEndTime2, out strErrInfo);
                if (!result)
                {
                    PubHelper.ShowMsgInfo(strErrInfo, PubHelper.MsgType.Ok);
                    ControlButton(true);
                    return;
                }
                PubHelper.p_BusinOper.UpdateSysCfgValue("AdLampBeginTime1", strBeginTime1);
                PubHelper.p_BusinOper.UpdateSysCfgValue("AdLampEndTime1", strEndTime1);
                PubHelper.p_BusinOper.UpdateSysCfgValue("AdLampBeginTime2", strBeginTime2);
                PubHelper.p_BusinOper.UpdateSysCfgValue("AdLampEndTime2", strEndTime2);
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("AdLampModel", strModel);

            #endregion

            #region 保存除雾设备模式

            if (rdbChuWuModel_Time.IsChecked == true)
            {
                strModel = "0";
            }
            if (rdbChuWuModel_Run.IsChecked == true)
            {
                strModel = "1";
            }
            if (rdbChuWuModel_Stop.IsChecked == true)
            {
                strModel = "2";
            }

            if (strModel == "0")
            {
                // 检测时间段是否有效
                result = false;
                strBeginTime1 = cmbChuWu_BeginTime1_Hour.Text + cmbChuWu_BeginTime1_Min.Text;
                strEndTime1 = cmbChuWu_EndTime1_Hour.Text + cmbChuWu_EndTime1_Min.Text;
                strBeginTime2 = cmbChuWu_BeginTime2_Hour.Text + cmbChuWu_BeginTime2_Min.Text;
                strEndTime2 = cmbChuWu_EndTime2_Hour.Text + cmbChuWu_EndTime2_Min.Text;
                result = PubHelper.CheckModelTime(strBeginTime1, strEndTime1, strBeginTime2, strEndTime2, out strErrInfo);
                if (!result)
                {
                    PubHelper.ShowMsgInfo(strErrInfo, PubHelper.MsgType.Ok);
                    ControlButton(true);
                    return;
                }
                PubHelper.p_BusinOper.UpdateSysCfgValue("FogBeginTime1", strBeginTime1);
                PubHelper.p_BusinOper.UpdateSysCfgValue("FogEndTime1", strEndTime1);
                PubHelper.p_BusinOper.UpdateSysCfgValue("FogBeginTime2", strBeginTime2);
                PubHelper.p_BusinOper.UpdateSysCfgValue("FogEndTime2", strEndTime2);
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("FogModel", strModel);

            #endregion

            #region 保存显示屏模式

            if (rdbScreenModel_Time.IsChecked == true)
            {
                strModel = "0";
            }
            if (rdbScreenModel_Run.IsChecked == true)
            {
                strModel = "1";
            }
            if (rdbScreenModel_Stop.IsChecked == true)
            {
                strModel = "2";
            }

            if (strModel == "0")
            {
                // 检测时间段是否有效
                result = false;
                strBeginTime1 = cmbScreen_BeginTime1_Hour.Text + cmbScreen_BeginTime1_Min.Text;
                strEndTime1 = cmbScreen_EndTime1_Hour.Text + cmbScreen_EndTime1_Min.Text;
                strBeginTime2 = cmbScreen_BeginTime2_Hour.Text + cmbScreen_BeginTime2_Min.Text;
                strEndTime2 = cmbScreen_EndTime2_Hour.Text + cmbScreen_EndTime2_Min.Text;
                result = PubHelper.CheckModelTime(strBeginTime1, strEndTime1, strBeginTime2, strEndTime2, out strErrInfo);
                if (!result)
                {
                    PubHelper.ShowMsgInfo(strErrInfo, PubHelper.MsgType.Ok);
                    ControlButton(true);
                    return;
                }
                PubHelper.p_BusinOper.UpdateSysCfgValue("PowerBeginTime1", strBeginTime1);
                PubHelper.p_BusinOper.UpdateSysCfgValue("PowerEndTime1", strEndTime1);
                PubHelper.p_BusinOper.UpdateSysCfgValue("PowerBeginTime2", strBeginTime2);
                PubHelper.p_BusinOper.UpdateSysCfgValue("PowerEndTime2", strEndTime2);
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("PowerModel", strModel);

            #endregion

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void rdbLightModel_Time_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbLightModel_Time.IsChecked;
                ControlCmbLight(blnChecked);
            }
        }

        private void rdbLightModel_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbLightModel_Run.IsChecked;
                ControlCmbLight(!blnChecked);
            }
        }

        private void rdbLightModel_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbLightModel_Stop.IsChecked;
                ControlCmbLight(!blnChecked);
            }
        }

        private void rdbAdvertModel_Time_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbAdvertModel_Time.IsChecked;
                ControlCmbAdvert(blnChecked);
            }
        }

        private void rdbAdvertModel_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbAdvertModel_Run.IsChecked;
                ControlCmbAdvert(!blnChecked);
            }
        }

        private void rdbAdvertModel_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbAdvertModel_Stop.IsChecked;
                ControlCmbAdvert(!blnChecked);
            }
        }

        private void rdbChuWuModel_Time_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbChuWuModel_Time.IsChecked;
                ControlCmbChuWu(blnChecked);
            }
        }

        private void rdbChuWuModel_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbChuWuModel_Run.IsChecked;
                ControlCmbChuWu(!blnChecked);
            }
        }

        private void rdbChuWuModel_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbChuWuModel_Stop.IsChecked;
                ControlCmbChuWu(!blnChecked);
            }
        }

        private void rdbScreenModel_Time_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbScreenModel_Time.IsChecked;
                ControlCmbScreen(blnChecked);
            }
        }

        private void rdbScreenModel_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbScreenModel_Run.IsChecked;
                ControlCmbScreen(!blnChecked);
            }
        }

        private void rdbScreenModel_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbScreenModel_Stop.IsChecked;
                ControlCmbScreen(!blnChecked);
            }
        }
    }
}
