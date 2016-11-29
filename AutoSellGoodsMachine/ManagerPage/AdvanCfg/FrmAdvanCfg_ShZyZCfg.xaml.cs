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
    /// FrmAdvanCfg_ShZyZCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_ShZyZCfg : Window
    {
        #region 变量声明

        private string m_Title_Run = string.Empty;
        private string m_Title_Stop = string.Empty;

        // 爱心捐赠状态 0：停止 1：只开启现金捐赠 2：只开启支付宝捐赠 3：现金和支付宝捐赠全开
        private string m_Title_DonPayType_No = "停止";
        private string m_Title_DonPayType_Cash = "只开启现金捐赠";
        private string m_Title_DonPayType_AliPay = "只开启支付宝捐赠";
        private string m_Title_DonPayType_CashAliPay = "现金和支付宝捐赠全开";

        #endregion

        public FrmAdvanCfg_ShZyZCfg()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            #region 加载界面资源

            m_Title_Run = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            m_Title_Stop = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");

            cmbBtnAbout.Items.Add(m_Title_Run);
            cmbBtnAbout.Items.Add(m_Title_Stop);
            cmbBtnQuery.Items.Add(m_Title_Run);
            cmbBtnQuery.Items.Add(m_Title_Stop);
            cmbBtnReg.Items.Add(m_Title_Run);
            cmbBtnReg.Items.Add(m_Title_Stop);
            cmbBtnDuiHuan.Items.Add(m_Title_Run);
            cmbBtnDuiHuan.Items.Add(m_Title_Stop);

            cmbClickXieHui.Items.Add(m_Title_Run);
            cmbClickXieHui.Items.Add(m_Title_Stop);
            cmbClickJiJinHui.Items.Add(m_Title_Run);
            cmbClickJiJinHui.Items.Add(m_Title_Stop);

            // 爱心捐赠状态 0：停止 1：只开启现金捐赠 2：只开启支付宝捐赠 3：现金和支付宝捐赠全开
            cmbDonPayType.Items.Add(m_Title_DonPayType_No);
            cmbDonPayType.Items.Add(m_Title_DonPayType_Cash);
            cmbDonPayType.Items.Add(m_Title_DonPayType_AliPay);
            cmbDonPayType.Items.Add(m_Title_DonPayType_CashAliPay);

            #endregion

            #region 加载参数

            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_Reg_Switch") != "1")
            {
                cmbBtnReg.Text = m_Title_Stop;
            }
            else
            {
                cmbBtnReg.Text = m_Title_Run;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_Query_Switch") != "1")
            {
                cmbBtnQuery.Text = m_Title_Stop;
            }
            else
            {
                cmbBtnQuery.Text = m_Title_Run;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_DuiHuan_Switch") != "1")
            {
                cmbBtnDuiHuan.Text = m_Title_Stop;
            }
            else
            {
                cmbBtnDuiHuan.Text = m_Title_Run;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_About_Switch") != "1")
            {
                cmbBtnAbout.Text = m_Title_Stop;
            }
            else
            {
                cmbBtnAbout.Text = m_Title_Run;
            }

            string strJuanZengSwitch = string.Empty;
            switch (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("JuanZeng_Switch"))
            {
                case "0":// 停止
                    strJuanZengSwitch = m_Title_DonPayType_No;
                    break;
                case "1":// 只接收现金捐赠
                    strJuanZengSwitch = m_Title_DonPayType_Cash;
                    break;
                case "2":// 只接收支付宝捐赠
                    strJuanZengSwitch = m_Title_DonPayType_AliPay;
                    break;
                case "3":// 现金和支付宝捐赠
                    strJuanZengSwitch = m_Title_DonPayType_CashAliPay;
                    break;
                default:
                    strJuanZengSwitch = m_Title_DonPayType_CashAliPay;
                    break;
            }
            cmbDonPayType.Text = strJuanZengSwitch;

            tbDonUploadWebUrl.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("DonUploadWebUrl");

            tbUrl_ZyZXieHui_Index.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Url_ZyZXieHui_Index");
            tbUrl_ZyZJiJinHui_Index.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Url_ZyZJiJinHui_Index");
            tbUrl_ZyZXieHui_Item.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Url_ZyZXieHui_Item");
            tbUrl_ZyZJiJinHui_Item.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Url_ZyZJiJinHui_Item");

            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("VisitWeb_Switch_ZyZXieHui") != "1")
            {
                cmbClickXieHui.Text = m_Title_Stop;
            }
            else
            {
                cmbClickXieHui.Text = m_Title_Run;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("VisitWeb_Switch_ZyZJiJinHui") != "1")
            {
                cmbClickJiJinHui.Text = m_Title_Stop;
            }
            else
            {
                cmbClickJiJinHui.Text = m_Title_Run;
            }

            #endregion
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ControlForm(false);
            string strBtnAbout = "0";
            if (cmbBtnAbout.Text == m_Title_Run)
            {
                strBtnAbout = "1";
            }
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("Btn_About_Switch", strBtnAbout);

            string strBtnReg = "0";
            if (cmbBtnReg.Text == m_Title_Run)
            {
                strBtnReg = "1";
            }
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("Btn_Reg_Switch", strBtnReg);

            string strBtnDuiHuan = "0";
            if (cmbBtnDuiHuan.Text == m_Title_Run)
            {
                strBtnDuiHuan = "1";
            }
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("Btn_DuiHuan_Switch", strBtnDuiHuan);

            string strBtnQuery = "0";
            if (cmbBtnQuery.Text == m_Title_Run)
            {
                strBtnQuery = "1";
            }
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("Btn_Query_Switch", strBtnQuery);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            ControlForm(true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSaveUrl_Click(object sender, RoutedEventArgs e)
        {
            ControlForm(false);
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("Url_ZyZXieHui_Index", tbUrl_ZyZXieHui_Index.Text);
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("Url_ZyZJiJinHui_Index", tbUrl_ZyZJiJinHui_Index.Text);
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("Url_ZyZXieHui_Item", tbUrl_ZyZXieHui_Item.Text);
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("Url_ZyZJiJinHui_Item", tbUrl_ZyZJiJinHui_Item.Text);

            string strClickXieHui = "0";
            if (cmbClickXieHui.Text == m_Title_Run)
            {
                strClickXieHui = "1";
            }
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("VisitWeb_Switch_ZyZXieHui", strClickXieHui);

            string strClickJiJinHui = "0";
            if (cmbClickJiJinHui.Text == m_Title_Run)
            {
                strClickJiJinHui = "1";
            }
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("VisitWeb_Switch_ZyZJiJinHui", strClickJiJinHui);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            ControlForm(true);
        }

        private void btnSaveDon_Click(object sender, RoutedEventArgs e)
        {
            ControlForm(false);
            string strDonPayType = string.Empty;
            if (cmbDonPayType.Text == m_Title_DonPayType_No)
            {
                strDonPayType = "0";
            }
            if (cmbDonPayType.Text == m_Title_DonPayType_Cash)
            {
                strDonPayType = "1";
            }
            if (cmbDonPayType.Text == m_Title_DonPayType_AliPay)
            {
                strDonPayType = "2";
            }
            if (cmbDonPayType.Text == m_Title_DonPayType_CashAliPay)
            {
                strDonPayType = "3";
            }
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("JuanZeng_Switch", strDonPayType);
            PubHelper.p_BusinOper.SysCfgOper.UpdateSysCfg_Third("DonUploadWebUrl", tbDonUploadWebUrl.Text);
            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            ControlForm(true);
        }

        private void ControlForm(bool enable)
        {
            cmbBtnAbout.IsEnabled = cmbBtnDuiHuan.IsEnabled = cmbBtnQuery.IsEnabled = cmbBtnReg.IsEnabled =
                cmbClickJiJinHui.IsEnabled = cmbClickXieHui.IsEnabled = cmbDonPayType.IsEnabled = enable;
            tbUrl_ZyZJiJinHui_Index.IsEnabled = tbUrl_ZyZJiJinHui_Item.IsEnabled = tbUrl_ZyZXieHui_Index.IsEnabled =
                tbUrl_ZyZXieHui_Item.IsEnabled = enable;
            btnCancel.IsEnabled = enable;
            btnSave.IsEnabled = btnSaveDon.IsEnabled = btnSaveUrl.IsEnabled = enable;
        }
    }
}
