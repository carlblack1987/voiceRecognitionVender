#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：基础设置
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
    /// FrmBaseCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmBaseCfg : Window
    {
        public FrmBaseCfg()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            #region 加载界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");

            tbBaseTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_Base");
            tbVmCode.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_VmCode");// 机器自编号
            tbLanguage.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_Language");// 语言

            tbNetTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_Net");
            tbNetSwitch.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_NetStitch");// 通信功能
            tbPhoneNum.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_NetPhoneNum");// 通信号码
            tbNetPwd.Text = PubHelper.p_LangOper.GetStringBundle("Pub_NetPwd");// 通信密码

            tbOtherTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_Other");
            tbStock.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_Stock");

            string strRun = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            string strStop = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            rdbNetSwitch_Run.Content = rdbStock_Run.Content = strRun;
            rdbNetSwitch_Stop.Content = rdbStock_Stop.Content = strStop;

            tbKeyBoard.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_KeyBoard");
            rdbKeyBoard_Small.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_KeyBoard_Small");
            rdbKeyBoard_Big.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_KeyBoard_Big");

            #endregion

            #region 加载数据

            tbVmCode_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("VmCode");
            switch (PubHelper.p_BusinOper.ConfigInfo.Language)
            {
                case Business.Enum.BusinessEnum.Language.English:// 英文
                    rdbLanguage_English.IsChecked = true;
                    break;
                case Business.Enum.BusinessEnum.Language.Zh_CN:// 中文
                    rdbLanguage_Chinese.IsChecked = true;
                    break;
                case Business.Enum.BusinessEnum.Language.French:// 法文
                    rdbLanguage_French.IsChecked = true;
                    break;
            }

            switch (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetSwitch"))
            {
                case "0":// 关闭
                    rdbNetSwitch_Stop.IsChecked = true;
                    break;
                case "1":// 开启
                    rdbNetSwitch_Run.IsChecked = true;
                    break;
            }
            tbPhoneNum_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetPhone");
            tbNetPwd_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetPwd");

            switch (PubHelper.p_BusinOper.ConfigInfo.IsRunStock)
            {
                case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                    rdbStock_Stop.IsChecked = true;
                    break;
                case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                    rdbStock_Run.IsChecked = true;
                    break;
            }

            switch (PubHelper.p_BusinOper.ConfigInfo.KeyBoardType)
            {
                case "0":// 小键盘
                    rdbKeyBoard_Small.IsChecked = true;
                    break;
                case "1":// 大键盘
                    rdbKeyBoard_Big.IsChecked = true;
                    break;
            }

            #endregion
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void rdbTmpModel_Stop1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            string strLanguage = "0";
            if (rdbLanguage_Chinese.IsChecked == true)
            {
                strLanguage = "1";
            }
            if (rdbLanguage_French.IsChecked == true)
            {
                strLanguage = "3";
            }
            string strVmCode = tbVmCode_Value.Text;

            string strNetSwitch = "0";
            if (rdbNetSwitch_Stop.IsChecked == false)
            {
                strNetSwitch = "1";
            }
            string strNetPhoneNum = tbPhoneNum_Value.Text;
            string strNetPwd = tbNetPwd_Value.Text;

            string strStockSwitch = "0";
            if (rdbStock_Stop.IsChecked == false)
            {
                strStockSwitch = "1";
            }

            string strKeyboard = "0";
            if (rdbKeyBoard_Small.IsChecked == false)
            {
                strKeyboard = "1";
            }

            // 保存数据
            PubHelper.p_BusinOper.UpdateSysCfgValue("VmCode", strVmCode);
            string strOldLanguage = "0";
            if (PubHelper.p_BusinOper.ConfigInfo.Language == Business.Enum.BusinessEnum.Language.Zh_CN)
            {
                strOldLanguage = "1";
            }

            if (strLanguage != strOldLanguage)
            {
                PubHelper.p_BusinOper.UpdateSysCfgValue("Language", strLanguage);
                bool blnResult = PubHelper.RefreshLanguage();
                PubHelper.p_IsRefreshLanguage = true;
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("NetSwitch", strNetSwitch);
            PubHelper.p_BusinOper.UpdateSysCfgValue("NetPhone", strNetPhoneNum);
            PubHelper.p_BusinOper.UpdateSysCfgValue("NetPwd", strNetPwd);

            PubHelper.p_BusinOper.UpdateSysCfgValue("IsRunStock", strStockSwitch);

            if (strKeyboard != PubHelper.p_BusinOper.ConfigInfo.KeyBoardType)
            {
                PubHelper.p_BusinOper.UpdateSysCfgValue("KeyBoardType", strKeyboard);
            }

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void btnTargetTmpAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearVmCode_Click(object sender, RoutedEventArgs e)
        {
            tbVmCode_Value.Text = string.Empty;
        }

        private void btnClearPhoneNum_Click(object sender, RoutedEventArgs e)
        {
            tbPhoneNum_Value.Text = string.Empty;
        }

        private void btnClearNetPwd_Click(object sender, RoutedEventArgs e)
        {
            tbNetPwd_Value.Text = string.Empty;
        }

        private void tbVmCode_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbVmCode_Value.Text);
            tbVmCode_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbPhoneNum_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbPhoneNum_Value.Text);
            tbPhoneNum_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbNetPwd_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbNetPwd_Value.Text);
            tbNetPwd_Value.Text = PubHelper.p_Keyboard_Input;
        }
    }
}
