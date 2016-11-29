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
    /// FrmAdvanCfg_NoFeeCard.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_NoFeeCard : Window
    {
        private bool m_IsInit = true;

        public FrmAdvanCfg_NoFeeCard()
        {
            InitializeComponent();

            m_IsInit = true;
            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.ConvertNoFeeCardPayName();// PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_Title");
            tbSwitch.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Control");
            tbPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_Port");
            tbPayShow.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_PayShow");
            tbHideCardNum.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_HideCardNum");
            tbIsRebate.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_IsRebate");
            
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            string strRun = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            string strStop = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            rdbSwitch_Run.Content = rdbPayShow_Run.Content = rdbHideCardNum_Run.Content = 
                rdbIsRebate_Run.Content = strRun;
            rdbSwitch_Stop.Content = rdbPayShow_Stop.Content = rdbHideCardNum_Stop.Content = 
                rdbIsRebate_Stop.Content = strStop;

            for (int i = 1; i < 11; i++)
            {
                cmbPort.Items.Add("COM" + i.ToString());
            }

            #endregion

            #region 加载数据

            string strSwitch = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NoFeeCardSwitch");
            if (strSwitch == "0")
            {
                rdbSwitch_Stop.IsChecked = true;
                cmbPort.IsEnabled = rdbPayShow_Run.IsEnabled = rdbPayShow_Stop.IsEnabled = 
                    rdbHideCardNum_Run.IsEnabled = rdbHideCardNum_Stop.IsEnabled = false;
            }
            else
            {
                rdbSwitch_Run.IsChecked = true;
                cmbPort.IsEnabled = rdbPayShow_Run.IsEnabled = rdbPayShow_Stop.IsEnabled =
                    rdbHideCardNum_Run.IsEnabled = rdbHideCardNum_Stop.IsEnabled = true;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.NoFeeCardPayShow == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            {
                rdbPayShow_Stop.IsChecked = true;
            }
            else
            {
                rdbPayShow_Run.IsChecked = true;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.NoFeeCardNumHide == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            {
                rdbHideCardNum_Stop.IsChecked = true;
            }
            else
            {
                rdbHideCardNum_Run.IsChecked = true;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.NoFeeCardIsRebate == "0")
            {
                rdbIsRebate_Stop.IsChecked = true;
            }
            else
            {
                rdbIsRebate_Run.IsChecked = true;
            }

            cmbPort.Text = "COM" + PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NoFeeCardPort");

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

            string strPayShow = "0";
            if (rdbPayShow_Run.IsChecked == true)
            {
                strPayShow = "1";
            }

            string strHideCardNum = "0";
            if (rdbHideCardNum_Run.IsChecked == true)
            {
                strHideCardNum = "1";
            }

            string strIsRebate = "0";
            if (rdbIsRebate_Run.IsChecked == true)
            {
                strIsRebate = "1";
            }

            string strPort = cmbPort.Text.Replace("COM", "");

            // 保存参数
            PubHelper.p_BusinOper.UpdateSysCfgValue("NoFeeCardSwitch", strSwitch);
            PubHelper.p_BusinOper.UpdateSysCfgValue("NoFeeCardPort", strPort);
            PubHelper.p_BusinOper.UpdateSysCfgValue("NoFeeCardPayShow", strPayShow);
            PubHelper.p_BusinOper.UpdateSysCfgValue("NoFeeCardNumHide", strHideCardNum);
            PubHelper.p_BusinOper.UpdateSysCfgValue("NoFeeCardIsRebate", strIsRebate);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void rdbSwitch_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                cmbPort.IsEnabled = rdbPayShow_Run.IsEnabled = rdbPayShow_Stop.IsEnabled =
                    rdbHideCardNum_Run.IsEnabled = rdbHideCardNum_Stop.IsEnabled = 
                    rdbIsRebate_Run.IsEnabled = rdbIsRebate_Stop.IsEnabled = true;
            }
        }

        private void rdbSwitch_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                cmbPort.IsEnabled = rdbPayShow_Run.IsEnabled = rdbPayShow_Stop.IsEnabled =
                    rdbHideCardNum_Run.IsEnabled = rdbHideCardNum_Stop.IsEnabled =
                    rdbIsRebate_Run.IsEnabled = rdbIsRebate_Stop.IsEnabled = false;
            }
        }
    }
}
