#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：制冷设置
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

using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmRefriCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmRefriCfg : Window
    {
        /// <summary>
        /// 当前的货柜编号
        /// </summary>
        private string m_VendBoxCode = "1";

        /// <summary>
        /// 当前的货柜索引
        /// </summary>
        private int m_VendBoxIndex = 0;

        private string m_TargetTmp = "3";
        private string m_WarnTmp = "30";
        private bool m_IsInit = true;

        public FrmRefriCfg()
        {
            InitializeComponent();
            InitForm();
        }

        /// <summary>
        /// 加载界面及数据
        /// </summary>
        private void InitForm()
        {
            #region 加载时间段下拉列表 

            string strHour = string.Empty;
            for (int i = 0; i < 24; i++)
            {
                strHour = i.ToString().PadLeft(2, '0');
                cmbTime1_Begin_Hour.Items.Add(strHour);
                cmbTime1_End_Hour.Items.Add(strHour);
                cmbTime2_Begin_Hour.Items.Add(strHour);
                cmbTime2_End_Hour.Items.Add(strHour);
            }

            string strMin = string.Empty;
            for (int i = 0; i < 60; i++)
            {
                strMin = i.ToString().PadLeft(2, '0');
                cmbTime1_Begin_Min.Items.Add(strMin);
                cmbTime1_End_Min.Items.Add(strMin);
                cmbTime2_Begin_Min.Items.Add(strMin);
                cmbTime2_End_Min.Items.Add(strMin);
            }

            #endregion

            #region 加载界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_Ref"); ;

            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");

            tbTmpControlModel.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_TmpType");
            tbTmpControl.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_TmpControl");
            tbTargetTmp.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_TargetTmp");
            tbWarnTmp.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_WarnTmp");
            tbOutTmp.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_OutTmpModel");

            rdbOutTmp_Run.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            rdbOutTmp_Stop.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");

            tbTempModel.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_TmpModel");
            rdbTmpModel_Time.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_0");
            rdbTmpModel_Run.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_1");
            rdbTmpModel_Stop.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_2");
            tbTime1.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_0_Time1");
            tbTime2.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_WorkModel_0_Time2");

            tbOutTmp.Visibility = rdbOutTmp_Run.Visibility = rdbOutTmp_Stop.Visibility = System.Windows.Visibility.Hidden;

            #endregion

            m_VendBoxCode = "1";// 默认第一个柜子
            CreateBox();//
        }

        /// <summary>
        /// 创建货柜
        /// </summary>
        private void CreateBox()
        {
            var map = new Dictionary<int, string>();
            for (int i = 1; i < 10; i++)
            {
                map.Add(i, DictionaryHelper.Dictionary_VendBoxName(i.ToString()));
            }

            int intBoxCount = PubHelper.p_BusinOper.AsileOper.VendBoxList.Count;
            #region 有多个柜子
            for (int i = 1; i <= intBoxCount; i++)
            {
                panelBox.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                var palletButton = new RadioButton()
                {
                    Focusable = false,

                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,

                    VerticalAlignment = System.Windows.VerticalAlignment.Center,

                    Style = App.Current.Resources["VendBoxButtonStyle"] as Style,

                    Foreground = new SolidColorBrush(Colors.White),

                    Padding = new Thickness(20, 4, 20, 8),

                    FontSize = 22,

                    Tag = i
                };

                Grid.SetColumn(palletButton, i - 1);

                palletButton.Checked += (VendBoxButtonChecked);

                palletButton.Content = map[i];

                panelBox.Children.Add(palletButton);

                if (i == 1)
                {
                    palletButton.IsChecked = true;
                }
            }
            #endregion
        }

        /// <summary>
        /// 货柜选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VendBoxButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                int currentTrayIndex = Convert.ToInt32((sender as RadioButton).Tag);
                m_VendBoxCode = currentTrayIndex.ToString();
                LoadVendBoxTmpData();
            }
        }

        private void LoadVendBoxTmpData()
        {
            // 获取当前货柜的索引
            m_VendBoxIndex = PubHelper.p_BusinOper.AsileOper.GetVendBoxIndex(m_VendBoxCode);

            #region 加载货柜温控相关数据

            tbTmpControlModel_Value.Text = DictionaryHelper.Dictionary_TmpType(m_VendBoxIndex);

            m_TargetTmp = PubHelper.p_BusinOper.AsileOper.VendBoxList[m_VendBoxIndex].TargetTmp;
            if (string.IsNullOrEmpty(m_TargetTmp))
            {
                m_TargetTmp = "3";
            }
            tbTargetTmp_Value.Text = m_TargetTmp + PubHelper.TMP_UNIT;

            m_WarnTmp = PubHelper.p_BusinOper.AsileOper.VendBoxList[m_VendBoxIndex].OutTmpWarnValue;
            if (string.IsNullOrEmpty(m_WarnTmp))
            {
                m_WarnTmp = "30";
            }
            tbWarnTmp_Value.Text = m_WarnTmp + PubHelper.TMP_UNIT;

            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("OutTmpWarnModel") == "1")
            {
                rdbOutTmp_Run.IsChecked = true;
            }
            else
            {
                rdbOutTmp_Run.IsChecked = false;
            }

            string strTmpRunModel = PubHelper.p_BusinOper.AsileOper.VendBoxList[m_VendBoxIndex].RefControl.ControlModel;
            bool enable = false;
            switch (strTmpRunModel)
            {
                case "0":// 定时开启
                    enable = true;
                    rdbTmpModel_Time.IsChecked = true;
                    string strBeginTime1 = PubHelper.p_BusinOper.AsileOper.VendBoxList[m_VendBoxIndex].RefControl.BeginTime1;
                    string strEndTime1 = PubHelper.p_BusinOper.AsileOper.VendBoxList[m_VendBoxIndex].RefControl.EndTime1;
                    string strBeginTime2 = PubHelper.p_BusinOper.AsileOper.VendBoxList[m_VendBoxIndex].RefControl.BeginTime2;
                    string strEndTime2 = PubHelper.p_BusinOper.AsileOper.VendBoxList[m_VendBoxIndex].RefControl.EndTime2;

                    if (strBeginTime1.Length == 4)
                    {
                        cmbTime1_Begin_Hour.Text = strBeginTime1.Substring(0, 2);
                        cmbTime1_Begin_Min.Text = strBeginTime1.Substring(2);
                    }
                    if (strEndTime1.Length == 4)
                    {
                        cmbTime1_End_Hour.Text = strEndTime1.Substring(0, 2);
                        cmbTime1_End_Min.Text = strEndTime1.Substring(2);
                    }
                    if (strBeginTime2.Length == 4)
                    {
                        cmbTime2_Begin_Hour.Text = strBeginTime2.Substring(0, 2);
                        cmbTime2_Begin_Min.Text = strBeginTime2.Substring(2);
                    }
                    if (strEndTime2.Length == 4)
                    {
                        cmbTime2_End_Hour.Text = strEndTime2.Substring(0, 2);
                        cmbTime2_End_Min.Text = strEndTime2.Substring(2);
                    }
                    break;
                case "1":// 全时段开启
                    rdbTmpModel_Run.IsChecked = true;
                    break;
                case "2":// 全时段关闭
                    rdbTmpModel_Stop.IsChecked = true;
                    break;
            }
            ControlCmb(enable);

            m_IsInit = false;

            #endregion
        }

        /// <summary>
        /// 按钮—目标温度减少
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTargetTmpDecuse_Click(object sender, RoutedEventArgs e)
        {
            int intTargetTmp = Convert.ToInt32(m_TargetTmp);
            if (intTargetTmp <= 3)
            {
                return;
            }
            intTargetTmp--;
            m_TargetTmp = intTargetTmp.ToString();
            tbTargetTmp_Value.Text = m_TargetTmp + PubHelper.TMP_UNIT;
        }

        /// <summary>
        /// 按钮—目标温度增加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTargetTmpAdd_Click(object sender, RoutedEventArgs e)
        {
            int intTargetTmp = Convert.ToInt32(m_TargetTmp);
            if (intTargetTmp >= 80)
            {
                return;
            }
            intTargetTmp++;
            m_TargetTmp = intTargetTmp.ToString();
            tbTargetTmp_Value.Text = m_TargetTmp + PubHelper.TMP_UNIT;
        }

        /// <summary>
        /// 按钮—预警温度减少
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWarnTmpDecuse_Click(object sender, RoutedEventArgs e)
        {
            int intWarnTmp = Convert.ToInt32(m_WarnTmp);
            if (intWarnTmp <= 10)
            {
                return;
            }
            intWarnTmp--;
            m_WarnTmp = intWarnTmp.ToString();
            tbWarnTmp_Value.Text = m_WarnTmp + PubHelper.TMP_UNIT;
        }

        /// <summary>
        /// 按钮—预警温度增加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWanrTmpAdd_Click(object sender, RoutedEventArgs e)
        {
            int intWarnTmp = Convert.ToInt32(m_WarnTmp);
            if (intWarnTmp >= 80)
            {
                return;
            }
            intWarnTmp++;
            m_WarnTmp = intWarnTmp.ToString();
            tbWarnTmp_Value.Text = m_WarnTmp + PubHelper.TMP_UNIT;
        }

        /// <summary>
        /// 按钮—保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ControlButton(false);
            DispatcherHelper.DoEvents();

            #region 保存制冷模式

            string strTmpRunModel = string.Empty;
            if (rdbTmpModel_Time.IsChecked == true)
            {
                strTmpRunModel = "0";
            }
            if (rdbTmpModel_Run.IsChecked == true)
            {
                strTmpRunModel = "1";
            }
            if (rdbTmpModel_Stop.IsChecked == true)
            {
                strTmpRunModel = "2";
            }

            string strBeginTime1 = cmbTime1_Begin_Hour.Text + cmbTime1_Begin_Min.Text;
            string strEndTime1 = cmbTime1_End_Hour.Text + cmbTime1_End_Min.Text;
            string strBeginTime2 = cmbTime2_Begin_Hour.Text + cmbTime2_Begin_Min.Text;
            string strEndTime2 = cmbTime2_End_Hour.Text + cmbTime2_End_Min.Text;

            if (strTmpRunModel == "0")
            {
                // 检测时间段是否有效
                bool result = false;
                string strErrInfo = string.Empty;
                
                result = PubHelper.CheckModelTime(strBeginTime1,strEndTime1,strBeginTime2,strEndTime2, out strErrInfo);
                if (!result)
                {
                    PubHelper.ShowMsgInfo(strErrInfo, PubHelper.MsgType.Ok);
                    ControlButton(true);
                    return;
                }
            }

            #endregion

            PubHelper.p_BusinOper.AsileOper.UpdateVendBoxTmpCfg(m_VendBoxCode, m_TargetTmp, m_WarnTmp, strTmpRunModel,
                strBeginTime1, strEndTime1, strBeginTime2, strEndTime2);

            string strOutTmpModel = "0";
            if (rdbOutTmp_Run.IsChecked == true)
            {
                strOutTmpModel = "1";
            }
            PubHelper.p_BusinOper.UpdateSysCfgValue("OutTmpWarnModel", strOutTmpModel);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            ControlButton(true);
        }

        /// <summary>
        /// 按钮—取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ControlButton(bool enable)
        {
            btnSave.IsEnabled = enable;
            btnCancel.IsEnabled = enable;
            DispatcherHelper.SleepControl();
        }

        private void ControlCmb(bool enable)
        {
            cmbTime1_Begin_Hour.IsEnabled = cmbTime1_Begin_Min.IsEnabled = cmbTime1_End_Hour.IsEnabled =
                cmbTime1_End_Min.IsEnabled = cmbTime2_Begin_Hour.IsEnabled = cmbTime2_Begin_Min.IsEnabled =
                cmbTime2_End_Hour.IsEnabled = cmbTime2_End_Min.IsEnabled = enable;
        }

        private void rdbTmpModel_Time_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbTmpModel_Time.IsChecked;
                ControlCmb(blnChecked);
            }
        }

        private void rdbTmpModel_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbTmpModel_Run.IsChecked;
                ControlCmb(!blnChecked);
            }
        }

        private void rdbTmpModel_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                bool blnChecked = (bool)rdbTmpModel_Stop.IsChecked;
                ControlCmb(!blnChecked);
            }
        }
    }
}
