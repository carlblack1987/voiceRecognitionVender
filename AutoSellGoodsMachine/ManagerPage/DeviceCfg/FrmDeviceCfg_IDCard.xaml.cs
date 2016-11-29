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
    /// FrmDeviceCfg_IDCard.xaml 的交互逻辑
    /// </summary>
    public partial class FrmDeviceCfg_IDCard : Window
    {
        public FrmDeviceCfg_IDCard()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_IDCard_Title");
            tbCfgTitle.Text = PubHelper.p_LangOper.GetStringBundle("Pub_DeviceCfg");

            tbWebUrl.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_IDCard_WebUrl");

            tbSerPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_IDCard_SerPort");
            cmbPort.Items.Add("USB");
            for (int i = 1; i < 11; i++)
            {
                cmbPort.Items.Add("COM" + i.ToString());
            }
            string strPort = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IDCardPort");
            if (strPort == "0")
            {
                cmbPort.Text = "USB";
            }
            else
            {
                cmbPort.Text = "COM" + strPort;
            }

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            tbDeviceTest.Text = PubHelper.p_LangOper.GetStringBundle("Pub_DeviceTest");
            tbErrCodeTitle.Text = PubHelper.p_LangOper.GetStringBundle("Pub_ResultCode");

            btnOpen.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_IDCard_Open");
            btnClose.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_IDCard_Close");
            btnReadInfo.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_IDCard_Read");
            btnClearInfo.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_IDCard_Clear");

            tbErrCode.Text = DictionaryHelper.Dictionary_Lang_IDCardErrCode();

            tbIDCardInfo.Text = string.Empty;

            #endregion

            #region 加载参数

            tbWebUrl_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IDCardWebUrl");

            if (PubHelper.p_BusinOper.UserType != Business.Enum.BusinessEnum.UserType.SystemUser)
            {
                // 不是厂商管理员
                cmbPort.IsEnabled = btnSave.IsEnabled = false;
            }

            btnOpen.IsEnabled = true;
            btnClose.IsEnabled = btnReadInfo.IsEnabled = false;

            #endregion
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strPort = cmbPort.Text;
            if (strPort == "USB")
            {
                strPort = "0";
            }
            else
            {
                strPort = strPort.Replace("COM", "");
            }

            if (strPort == PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IDCardPort"))
            {
                return;
            }

            bool result = PubHelper.p_BusinOper.UpdateSysCfgValue("IDCardPort", strPort);

            if (result)
            {
                // 保存成功
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            int intErrCode = PubHelper.p_BusinOper.IDCardOper.OpenIDCard();
            tbErrCodeValue.Text = intErrCode.ToString();
            if (intErrCode == 0)
            {
                btnOpen.IsEnabled = false;
                btnClose.IsEnabled = btnReadInfo.IsEnabled = true;
            }
        }

        private void btnReadInfo_Click(object sender, RoutedEventArgs e)
        {
            string strCardNum = string.Empty;
            int intErrCode = PubHelper.p_BusinOper.IDCardOper.ReadIDCardInfo(out strCardNum);
            tbErrCodeValue.Text = intErrCode.ToString();
            if (intErrCode == 0)
            {
                StringBuilder sbCardInfo = new StringBuilder();
                sbCardInfo.Append("身份证号码：" + PubHelper.p_BusinOper.IDCardOper.IDCard_UserInfo.IDC);
                sbCardInfo.Append("姓名：" + PubHelper.p_BusinOper.IDCardOper.IDCard_UserInfo.Name);
                sbCardInfo.Append("性别：" + PubHelper.p_BusinOper.IDCardOper.IDCard_UserInfo.Sex_CName);
                sbCardInfo.Append("民族：" + PubHelper.p_BusinOper.IDCardOper.IDCard_UserInfo.NATION_Code);
                sbCardInfo.Append("出生日期：" + PubHelper.p_BusinOper.IDCardOper.IDCard_UserInfo.BIRTH);
                sbCardInfo.Append("地址：" + PubHelper.p_BusinOper.IDCardOper.IDCard_UserInfo.ADDRESS);

                tbIDCardInfo.Text = sbCardInfo.ToString();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            int intErrCode = PubHelper.p_BusinOper.IDCardOper.CloseIDCard();
            tbErrCodeValue.Text = intErrCode.ToString();
            if (intErrCode == 0)
            {
                btnOpen.IsEnabled = true;
                btnClose.IsEnabled = btnReadInfo.IsEnabled = false;
            }
        }

        private void btnClearInfo_Click(object sender, RoutedEventArgs e)
        {
            tbIDCardInfo.Text = string.Empty;
        }
    }
}
