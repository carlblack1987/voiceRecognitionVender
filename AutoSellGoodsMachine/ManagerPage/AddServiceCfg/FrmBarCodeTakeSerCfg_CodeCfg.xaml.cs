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
    /// FrmBarCodeTakeSerCfg_CodeCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmBarCodeTakeSerCfg_CodeCfg : Window
    {
        public FrmBarCodeTakeSerCfg_CodeCfg()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            #region 初始化系统资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_CodeConfig");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            tbCodeName.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_CodeName");
            tbCodeLen.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_CodeLen");
            tbTipInfo_CodeLen.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_CodeLen_Tip");

            for (int i = 0; i < 22; i++)
            {
                cmbCodeLen.Items.Add(i.ToString());
            }

            #endregion

            #region 加载系统参数

            switch (PubHelper.p_Now_Cfg_AddService)
            {
                case Business.Enum.BusinessEnum.AddServiceType.BarCode_Take:// 线下扫码取货
                    tbCodeName_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("O2OTake_Name_CodeNum");
                    cmbCodeLen.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("O2OTake_CodeLen");
                    break;
                case Business.Enum.BusinessEnum.AddServiceType.WxTakeCode_Take:// 微信取货码取货
                    tbCodeName_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_Name_CodeNum");
                    cmbCodeLen.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_CodeLen");
                    break;
            }

            #endregion
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strCodeName = tbCodeName_Value.Text;
            string strCodeLen = cmbCodeLen.Text;

            btnSave.IsEnabled = btnCancel.IsEnabled = false;

            bool result = false;
            switch (PubHelper.p_Now_Cfg_AddService)
            {
                case Business.Enum.BusinessEnum.AddServiceType.BarCode_Take:// 线下扫码取货
                    result = PubHelper.p_BusinOper.UpdateSysCfgValue("O2OTake_Name_CodeNum", strCodeName);
                    result = PubHelper.p_BusinOper.UpdateSysCfgValue("O2OTake_CodeLen", strCodeLen);
                    break;
                case Business.Enum.BusinessEnum.AddServiceType.WxTakeCode_Take:// 微信取货码取货
                    result = PubHelper.p_BusinOper.UpdateSysCfgValue("WxTake_Name_CodeNum", strCodeName);
                    result = PubHelper.p_BusinOper.UpdateSysCfgValue("WxTake_CodeLen", strCodeLen);
                    break;
            }

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            btnSave.IsEnabled = btnCancel.IsEnabled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
