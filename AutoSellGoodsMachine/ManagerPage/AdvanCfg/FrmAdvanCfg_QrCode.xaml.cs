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
    /// FrmAdvanCfg_QrCode.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_QrCode : Window
    {
        public FrmAdvanCfg_QrCode()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_QrCode_Title");
            tbSwitch.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Control");
            tbPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_QrCode_Port");

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            rdbSwitch_Run.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            rdbSwitch_Stop.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");

            for (int i = 1; i < 11; i++)
            {
                cmbPort.Items.Add("COM" + i.ToString());
            }

            #endregion

            #region 加载数据

            string strSwitch = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("QRCodeCardSwitch");
            if (strSwitch == "0")
            {
                rdbSwitch_Stop.IsChecked = true;
            }
            else
            {
                rdbSwitch_Run.IsChecked = true;
            }

            cmbPort.Text = "COM" + PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("QrCodePort");

            #endregion
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            string strSwitch = "0";
            if (rdbSwitch_Run.IsChecked == true)
            {
                strSwitch = "1";
            }

            string strPort = cmbPort.Text.Replace("COM", "");

            // 保存参数
            PubHelper.p_BusinOper.UpdateSysCfgValue("QRCodeCardSwitch", strSwitch);
            PubHelper.p_BusinOper.UpdateSysCfgValue("QrCodePort", strPort);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void rdbSwitch_Run_Checked(object sender, RoutedEventArgs e)
        {
            cmbPort.IsEnabled = true;
        }

        private void rdbSwitch_Stop_Checked(object sender, RoutedEventArgs e)
        {
            cmbPort.IsEnabled = false;
        }
    }
}
