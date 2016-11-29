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
    /// FrmAdvanCfg_ICCard.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_ICCard : Window
    {
        private bool m_IsInit = true;

        public FrmAdvanCfg_ICCard()
        {
            InitializeComponent();

            m_IsInit = true;
            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.ConvertIcCardPayName();// PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_Title");
            tbControlSwitch.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Control");
            tbIcQuerySwitch.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_IcQuerySwitch");
            tbIcPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_IcPort");
            tbShowCardInfoTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_ShowCardInfoTime");
            tbPayShow.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_PayShow");
            tbHideCardNum.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_HideCardNum");

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            rdbControlSwitch_Run.Content = rdbIcQuerySwitch_Run.Content = rdbPayShow_Run.Content = rdbHideCardNum_Run.Content =
                PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            rdbControlSwitch_Stop.Content = rdbIcQuerySwitch_Stop.Content = rdbPayShow_Stop.Content = rdbHideCardNum_Stop.Content =
                PubHelper.p_LangOper.GetStringBundle("Pub_Stop");

            tbIcBusiModel.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_IcBusiModel");

            string strSecond = PubHelper.p_LangOper.GetStringBundle("Pub_Second");
            for (int i = 2; i < 16; i++)
            {
                cmbShowCardInfoTime.Items.Add(i.ToString() + strSecond);
            }

            for (int i = 1; i < 21; i++)
            {
                cmbIcPort.Items.Add("COM" + i.ToString());
            }

            string strIcBusiModel_QueryPay = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_IcBusiModel_QueryPay");
            string strIcBusiModel_Pay = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_IcBusiModel_Pay");
            cmbIcBusiModel.Items.Add(strIcBusiModel_QueryPay);
            cmbIcBusiModel.Items.Add(strIcBusiModel_Pay);

            #endregion

            #region 加载数据

            string strIcQuerySwitch = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IcQuerySwitch");
            if (strIcQuerySwitch == "0")
            {
                rdbIcQuerySwitch_Stop.IsChecked = true;
            }
            else
            {
                rdbIcQuerySwitch_Run.IsChecked = true;
            }

            cmbShowCardInfoTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("ShowCardInfoTime") + strSecond;
            cmbIcPort.Text = "COM" + PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IcPort");

            string strControlSwitch = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IcControlSwitch");
            if (strControlSwitch == "0")
            {
                rdbControlSwitch_Stop.IsChecked = true;
                rdbIcQuerySwitch_Run.IsEnabled = rdbIcQuerySwitch_Stop.IsEnabled =
                    cmbIcPort.IsEnabled = cmbShowCardInfoTime.IsEnabled = rdbPayShow_Run.IsEnabled = rdbPayShow_Stop.IsEnabled =
                    cmbIcBusiModel.IsEnabled = rdbHideCardNum_Run.IsEnabled = rdbHideCardNum_Stop.IsEnabled = false;
            }
            else
            {
                rdbControlSwitch_Run.IsChecked = true;
                rdbIcQuerySwitch_Run.IsEnabled = rdbIcQuerySwitch_Stop.IsEnabled =
                    cmbIcPort.IsEnabled = cmbShowCardInfoTime.IsEnabled = rdbPayShow_Run.IsEnabled = rdbPayShow_Stop.IsEnabled =
                    cmbIcBusiModel.IsEnabled = rdbHideCardNum_Run.IsEnabled = rdbHideCardNum_Stop.IsEnabled = true;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.IcPayShowSwitch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            {
                rdbPayShow_Stop.IsChecked = true;
            }
            else
            {
                rdbPayShow_Run.IsChecked = true;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.IcBusiModel == "0")
            {
                cmbIcBusiModel.Text = strIcBusiModel_QueryPay;
            }
            else
            {
                cmbIcBusiModel.Text = strIcBusiModel_Pay;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.IcCardNumHide == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            {
                rdbHideCardNum_Stop.IsChecked = true;
            }
            else
            {
                rdbHideCardNum_Run.IsChecked = true;
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

            string strControlSwitch = "0";
            if (rdbControlSwitch_Run.IsChecked == true)
            {
                strControlSwitch = "1";
            }

            string strIcQuerySwitch = "0";
            if (rdbIcQuerySwitch_Run.IsChecked == true)
            {
                strIcQuerySwitch = "1";
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

            string strIcPort = cmbIcPort.Text.Replace("COM", "");
            string strShowCardInfoTime = cmbShowCardInfoTime.Text.Replace(PubHelper.p_LangOper.GetStringBundle("Pub_Second"), "");

            string strIcBusiModel_Text = cmbIcBusiModel.Text;
            string strIcBusiModel_Value = "1";
            if (strIcBusiModel_Text == PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_IcBusiModel_QueryPay"))
            {
                strIcBusiModel_Value = "0";
            }

            // 保存参数
            PubHelper.p_BusinOper.UpdateSysCfgValue("IcControlSwitch", strControlSwitch);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IcQuerySwitch", strIcQuerySwitch);
            PubHelper.p_BusinOper.UpdateSysCfgValue("ShowCardInfoTime", strShowCardInfoTime);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IcPort", strIcPort);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IcPayShowSwitch", strPayShow);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IcBusiModel", strIcBusiModel_Value);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IcCardNumHide", strHideCardNum);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void rdbControlSwitch_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                rdbIcQuerySwitch_Run.IsEnabled = rdbIcQuerySwitch_Stop.IsEnabled =
                    cmbIcPort.IsEnabled = cmbShowCardInfoTime.IsEnabled =
                    rdbPayShow_Run.IsEnabled = rdbPayShow_Stop.IsEnabled = 
                    cmbIcBusiModel.IsEnabled = rdbHideCardNum_Run.IsEnabled = rdbHideCardNum_Stop.IsEnabled = true;
            }
        }

        private void rdbControlSwitch_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                rdbIcQuerySwitch_Run.IsEnabled = rdbIcQuerySwitch_Stop.IsEnabled =
                    cmbIcPort.IsEnabled = cmbShowCardInfoTime.IsEnabled =
                    rdbPayShow_Run.IsEnabled = rdbPayShow_Stop.IsEnabled =
                    cmbIcBusiModel.IsEnabled = rdbHideCardNum_Run.IsEnabled = rdbHideCardNum_Stop.IsEnabled = false;
            }
        }
    }
}
