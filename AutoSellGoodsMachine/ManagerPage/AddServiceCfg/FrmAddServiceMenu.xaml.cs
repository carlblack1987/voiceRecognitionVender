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
    /// FrmAddServiceMenu.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAddServiceMenu : Window
    {
        public FrmAddServiceMenu()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            tbMenuTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Title");
            AddService_Menu_IDCardTake.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Title_IDCardTake");
            AddService_Menu_BarCodeTake.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Title_BarCodeTake");
            AddService_Menu_WxTake.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Title_WxTake");
            AddService_Menu_Cancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
        }

        /// <summary>
        /// 菜单—返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 菜单—身份证免费领
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddService_Menu_IDCardTake_Click(object sender, RoutedEventArgs e)
        {
            if (PubHelper.p_BusinOper.ConfigInfo.IDCardFreeTake_Switch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            {
                // 没有开通该服务
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoServer"), PubHelper.MsgType.Ok);
                return;
            }

            PubHelper.p_IsRefreshSerBtnName = false;
            FrmIDCardTakeSerCfg frmIDCardTakeSerCfg = new FrmIDCardTakeSerCfg();
            frmIDCardTakeSerCfg.ShowDialog();
        }

        /// <summary>
        /// 菜单—线下订单取货
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddService_Menu_BarCodeTake_Click(object sender, RoutedEventArgs e)
        {
            ////if (PubHelper.p_BusinOper.ConfigInfo.O2OTake_Switch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            ////{
            ////    // 没有开通该服务
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoServer"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            PubHelper.p_IsRefreshSerBtnName = false;
            FrmBarCodeTakeSerCfg frmBarCodeTakeSerCfg = new FrmBarCodeTakeSerCfg();
            frmBarCodeTakeSerCfg.ShowDialog();
        }

        /// <summary>
        /// 菜单—微信关注有礼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddService_Menu_WxTake_Click(object sender, RoutedEventArgs e)
        {
            ////if (PubHelper.p_BusinOper.ConfigInfo.WxTake_Switch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            ////{
            ////    // 没有开通该服务
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoServer"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            PubHelper.p_IsRefreshSerBtnName = false;
            FrmWxTakeSerCfg frmWxTakeSerCfg = new FrmWxTakeSerCfg();
            frmWxTakeSerCfg.ShowDialog();
        }
    }
}
