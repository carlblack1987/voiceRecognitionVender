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
    /// FrmAdvanCfg_PaymentMenu.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_PaymentMenu : Window
    {
        public FrmAdvanCfg_PaymentMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbMenuTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Payment_Title");

            AdvanCfg_Menu_Cancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            AdvanCfg_Menu_IcCard.Content = PubHelper.ConvertIcCardPayName();// PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_Title");
            AdvanCfg_Menu_Cash.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Cash_Title");
            AdvanCfg_Menu_NoFeeCard.Content = PubHelper.ConvertNoFeeCardPayName();// PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_Title");
            AdvanCfg_Menu_WeChatCode.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_WeChatCode_Title");
            AdvanCfg_Menu_AliPayCode.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_AliPayCode_Title");
            AdvanCfg_Menu_BestPayCode.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_BestPayCode_Title");
            AdvanCfg_Menu_QrCode.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_QrCode_Title");
            AdvanCfg_Menu_UnionPay.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UnionPay_Title");

            #region 检测各支付方式是否允许

            if (!PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.Cash))
            {
                AdvanCfg_Menu_Cash.IsEnabled = false;
            }

            if (!PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.IcCard))
            {
                AdvanCfg_Menu_IcCard.IsEnabled = false;
            }

            if (!PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.NoFeeCard))
            {
                AdvanCfg_Menu_NoFeeCard.IsEnabled = false;
            }

            if (!PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.QRCodeCard))
            {
                AdvanCfg_Menu_QrCode.IsEnabled = false;
            }

            if (!PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.WeChatCode))
            {
                AdvanCfg_Menu_WeChatCode.IsEnabled = false;
            }

            if (!PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.AliPay_Code))
            {
                AdvanCfg_Menu_AliPayCode.IsEnabled = false;
            }

            if (!PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.BestPay_Code))
            {
                AdvanCfg_Menu_BestPayCode.IsEnabled = false;
            }

            if (!PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.QuickPass))
            {
                AdvanCfg_Menu_UnionPay.IsEnabled = false;
            }

            #endregion
        }

        /// <summary>
        /// 菜单—现金支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_Cash_Click(object sender, RoutedEventArgs e)
        {
            ////// 检查现金支付功能由没有启用
            ////if (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            ////{
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_CashStop"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_Cash frmAdvanCfg_Cash = new FrmAdvanCfg_Cash();
            frmAdvanCfg_Cash.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—储值卡支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_IcCard_Click(object sender, RoutedEventArgs e)
        {
            ////// 检查刷卡功能由没有启用
            ////if (PubHelper.p_BusinOper.PaymentOper.PaymentList.IC.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            ////{
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_ICCardStop"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_ICCard frmAdvanCfg_ICCard = new FrmAdvanCfg_ICCard();
            frmAdvanCfg_ICCard.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—会员卡支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_NoFeeCard_Click(object sender, RoutedEventArgs e)
        {
            ////// 检查会员卡功能由没有启用
            ////if (PubHelper.p_BusinOper.PaymentOper.PaymentList.NoFeeCard.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            ////{
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoFeeCardStop"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_NoFeeCard frmAdvanCfg_NoFeeCard = new FrmAdvanCfg_NoFeeCard();
            frmAdvanCfg_NoFeeCard.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—微信扫码支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_WeChatCode_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_WeChatCode frmAdvanCfg_WeChatCode = new FrmAdvanCfg_WeChatCode();
            frmAdvanCfg_WeChatCode.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—翼支付付款码支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_BestPayCode_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_BestPayCode frmAdvanCfg_BestPayCode = new FrmAdvanCfg_BestPayCode();
            frmAdvanCfg_BestPayCode.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—支付宝扫码支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_AliPayCode_Click(object sender, RoutedEventArgs e)
        {
            ////// 检查支付宝功能由没有启用
            ////if (PubHelper.p_BusinOper.PaymentOper.PaymentList.AliPay.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            ////{
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_AliPayStop"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_AliPayCode frmAdvanCfg_AliPayCode = new FrmAdvanCfg_AliPayCode();
            frmAdvanCfg_AliPayCode.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—二维码支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_QrCode_Click(object sender, RoutedEventArgs e)
        {
            ////// 检查二维码功能由没有启用
            ////if (PubHelper.p_BusinOper.PaymentOper.PaymentList.QRCodeCard.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            ////{
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_QRCodeStop"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_QrCode frmAdvanCfg_QrCode = new FrmAdvanCfg_QrCode();
            frmAdvanCfg_QrCode.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—银联闪付支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_UnionPay_Click(object sender, RoutedEventArgs e)
        {
            ////// 检查银联闪付功能由没有启用
            ////if (PubHelper.p_BusinOper.PaymentOper.PaymentList.UnionPay.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            ////{
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_UnionPayStop"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_UnionPay frmAdvanCfg_UnionPay = new FrmAdvanCfg_UnionPay();
            frmAdvanCfg_UnionPay.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
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
    }
}
