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
    /// FrmAdvanCfg_Net.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_Net : Window
    {
        public FrmAdvanCfg_Net()
        {
            InitializeComponent();

            InitForm();
        }

        /// <summary>
        /// 初始化窗体
        /// </summary>
        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_Title");
            tbNetType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetType");
            tbNetEquKind.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetEquKind");
            tbDtuPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_DtuPort");
            tbNetIp.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetIp");
            tbNetPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetPort");

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            for (int i = 1; i < 11; i++)
            {
                cmbDtuPort.Items.Add("COM" + i.ToString());
            }

            #endregion

            #region 加载参数数据

            string strNetType = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetType");// 网络通信类型
            switch (strNetType)
            {
                case "0":// DTU方式
                    tbNetType_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetType_DTU");
                    break;
                default:
                    tbNetType_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetType_Other");
                    break;
            }

            string strNetEquKind = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetEquKind");// 网络设备连接方式
            if (strNetEquKind == "0")
            {
                // 上位机连接网络设备
                tbNetEquKind_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetEquKind_PC");
            }
            else
            {
                // 控制主板连接网络设备
                tbNetEquKind_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetEquKind_ARM");
            }

            tbNetIp_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetIp");
            tbNetPort_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetPort");
            cmbDtuPort.Text = "COM" + PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("DtuPort");

            if ((strNetType == "0") && (strNetEquKind == "0"))
            {
                cmbDtuPort.IsEnabled = true;
            }
            else
            {
                cmbDtuPort.IsEnabled = false;
            }

            if ((strNetType != "0") && (strNetEquKind == "0"))
            {
                tbNetIp_Value.IsEnabled = tbNetPort_Value.IsEnabled = btnClearNetIp.IsEnabled = btnClearNetPort.IsEnabled = true;
            }
            else
            {
                tbNetIp_Value.IsEnabled = tbNetPort_Value.IsEnabled = btnClearNetIp.IsEnabled = btnClearNetPort.IsEnabled = false;
            }

            #endregion
        }

        private void btnClearNetIp_Click(object sender, RoutedEventArgs e)
        {
            tbNetIp_Value.Text = string.Empty;
        }

        private void btnClearNetPort_Click(object sender, RoutedEventArgs e)
        {
            tbNetPort_Value.Text = string.Empty;
        }

        private void tbNetPort_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbNetPort_Value.Text);
            tbNetPort_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDtuPort.IsEnabled == true)
            {
                // 只检查DTU串口
                string strDtuPort = cmbDtuPort.Text.Replace("COM", "");
                PubHelper.p_BusinOper.UpdateSysCfgValue("DtuPort", strDtuPort);
            }
            else
            {
                // 只检查通信网关IP、端口
                string strNetIp = tbNetIp_Value.Text;
                string strNetPort = tbNetPort_Value.Text;

                // 检查数据有效性
                if (string.IsNullOrEmpty(strNetIp))
                {
                    // 通信网关IP无效
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_Input_InvalidNetIP"), PubHelper.MsgType.Ok);
                    return;
                }

                if (!(PubHelper.p_BusinOper.CheckDataOper.CheckIsPosInt(strNetPort)) ||
                    (strNetPort.Length != 4))
                {
                    // 通信网关端口无效
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_Input_InvalidNetPort"), PubHelper.MsgType.Ok);
                    return;
                }

                btnSave.IsEnabled = btnCancel.IsEnabled = false;
                DispatcherHelper.DoEvents();

                PubHelper.p_BusinOper.UpdateSysCfgValue("NetIp", strNetIp);
                PubHelper.p_BusinOper.UpdateSysCfgValue("NetPort", strNetPort);
            }

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc_Restart"), PubHelper.MsgType.Ok);
            this.Close();
        }
    }
}
