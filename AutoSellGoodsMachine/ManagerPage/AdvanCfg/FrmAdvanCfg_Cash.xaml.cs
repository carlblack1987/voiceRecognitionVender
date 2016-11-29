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
    /// FrmAdvanCfg_Cash.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_Cash : Window
    {
        private bool m_IsInit = true;

        private string m_Run_Title = string.Empty;
        private string m_Stop_Title = string.Empty;

        private string m_CoinDevice_Normal = string.Empty;
        private string m_CoinDevice_Hook = string.Empty;

        public FrmAdvanCfg_Cash()
        {
            InitializeComponent();

            m_IsInit = true;
            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Cash_Title");
            tbSwitch.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Control");

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            tbCoinDeviceType_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Cash_CoinDevice");
            tbIsReturnBill.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Cash_IsReturnBill");

            m_Run_Title = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            m_Stop_Title = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            rdbSwitch_Run.Content = m_Run_Title;
            rdbSwitch_Stop.Content = m_Stop_Title;

            cmbIsReturnBill.Items.Add(m_Run_Title);
            cmbIsReturnBill.Items.Add(m_Stop_Title);

            m_CoinDevice_Normal = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Cash_CoinDevice_Normal");
            m_CoinDevice_Hook = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Cash_CoinDevice_Hook");
            cmbCoinDeviceType.Items.Add(m_CoinDevice_Normal);
            cmbCoinDeviceType.Items.Add(m_CoinDevice_Hook);

            #endregion

            #region 加载数据

            string strCashSwitch = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("CashControlSwitch");
            if (strCashSwitch == "0")
            {
                rdbSwitch_Stop.IsChecked = true;
                cmbCoinDeviceType.IsEnabled = cmbIsReturnBill.IsEnabled = false;
            }
            else
            {
                rdbSwitch_Run.IsChecked = true;
                cmbCoinDeviceType.IsEnabled = cmbIsReturnBill.IsEnabled = true;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.IsReturnBill == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                cmbIsReturnBill.Text = m_Run_Title;
            }
            else
            {
                cmbIsReturnBill.Text = m_Stop_Title;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.CoinDeviceType == Business.Enum.BusinessEnum.CoinDeviceType.Hook)
            {
                cmbCoinDeviceType.Text = m_CoinDevice_Hook;
            }
            else
            {
                cmbCoinDeviceType.Text = m_CoinDevice_Normal;
            }

            #endregion

            m_IsInit = false;
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

            string strCoinDeviceType = "0";
            if (cmbCoinDeviceType.Text == m_CoinDevice_Hook)
            {
                strCoinDeviceType = "1";
            }

            string strIsReturnBill = "0";
            if (cmbIsReturnBill.Text == m_Run_Title)
            {
                strIsReturnBill = "1";
            }

            // 保存参数
            PubHelper.p_BusinOper.UpdateSysCfgValue("CashControlSwitch", strSwitch);
            PubHelper.p_BusinOper.UpdateSysCfgValue("CoinDeviceType", strCoinDeviceType);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IsReturnBill", strIsReturnBill);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc_Restart"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void rdbSwitch_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                cmbCoinDeviceType.IsEnabled = cmbIsReturnBill.IsEnabled = true;
            }
        }

        private void rdbSwitch_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                cmbCoinDeviceType.IsEnabled = cmbIsReturnBill.IsEnabled = false;
            }
        }
    }
}
