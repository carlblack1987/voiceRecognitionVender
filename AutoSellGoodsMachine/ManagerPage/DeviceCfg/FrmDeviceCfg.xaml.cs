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

using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmDeviceCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmDeviceCfg : Window
    {
        public FrmDeviceCfg()
        {
            InitializeComponent();

            InitForm();
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void InitForm()
        {
            tbMenuTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg");
            DeviceCfg_Menu_BarCode.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_BarCode_Title");
            DeviceCfg_Menu_Print.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Title");
            DeviceCfg_Menu_IDCard.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_IDCard_Title");
            DeviceCfg_Menu_Coin.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_Title");
            DeviceCfg_Menu_Bill.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Title");
            DeviceCfg_Menu_BillCfg.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_BillCfg_Title");
            DeviceCfg_Menu_Lifter.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Title");
            DeviceCfg_Menu_Cancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
        }

        /// <summary>
        /// 菜单—条形码扫描设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceCfg_Menu_BarCode_Click(object sender, RoutedEventArgs e)
        {
            // 如果需要条形码设备的支付方式或其他全部关闭
            if ((PubHelper.p_BusinOper.PaymentOper.PaymentList.AliPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Stop) &&
                (PubHelper.p_BusinOper.PaymentOper.PaymentList.WeChatCode.ControlSwitch == BusinessEnum.ControlSwitch.Stop) &&
                (PubHelper.p_BusinOper.ConfigInfo.O2OTake_Switch == BusinessEnum.ControlSwitch.Stop) &&
                (PubHelper.p_BusinOper.ConfigInfo.WxTake_Switch == BusinessEnum.ControlSwitch.Stop))
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoPaymentUse"), PubHelper.MsgType.Ok);
                return;
            }
            FrmDeviceCfg_BarCode frmDeviceCfg_BarCode = new FrmDeviceCfg_BarCode();
            frmDeviceCfg_BarCode.ShowDialog();
        }

        /// <summary>
        /// 菜单—打印机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceCfg_Menu_Print_Click(object sender, RoutedEventArgs e)
        {
            FrmDeviceCfg_Print frmDeviceCfg_Print = new FrmDeviceCfg_Print();
            frmDeviceCfg_Print.ShowDialog();
        }

        private void DeviceCfg_Menu_IDCard_Click(object sender, RoutedEventArgs e)
        {
            FrmDeviceCfg_IDCard frmDeviceCfg_IDCard = new FrmDeviceCfg_IDCard();
            frmDeviceCfg_IDCard.ShowDialog();
        }

        private void DeviceCfg_Menu_Lifter_Click(object sender, RoutedEventArgs e)
        {
            if (PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter.Count == 0)
            {
                // 如果出货方式都为弹簧，则不能操作该菜单
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_NoLimit"), PubHelper.MsgType.Ok);
                return;
            }

            FrmAdvanCfg_UpDownCfg frmAdvanCfg_UpDownCfg = new FrmAdvanCfg_UpDownCfg();
            frmAdvanCfg_UpDownCfg.ShowDialog();
        }

        private void DeviceCfg_Menu_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 菜单—硬币库存管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceCfg_Menu_Coin_Click(object sender, RoutedEventArgs e)
        {
            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Stop)
            {
                // 如果现金支付没有启用
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_CashStop"), PubHelper.MsgType.Ok);
                return;
            }
            
            ////////if (PubHelper.p_BusinOper.ConfigInfo.CashManagerModel == BusinessEnum.CashManagerModel.Singal)
            ////////{
            ////////    // 货币高级管理模式没有开通
            ////////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoCashManagerModel"), PubHelper.MsgType.Ok);
            ////////    return;
            ////////}

            this.Opacity = PubHelper.OPACITY_GRAY;
            if (PubHelper.p_BusinOper.ConfigInfo.CoinDeviceType == BusinessEnum.CoinDeviceType.CoinDevice)
            {
                FrmDeviceCfg_CoinStock_Normal frmDeviceCfg_CoinStock = new FrmDeviceCfg_CoinStock_Normal();
                frmDeviceCfg_CoinStock.ShowDialog();
            }
            else
            {
                FrmDeviceCfg_CoinStock_Hopper frmDeviceCfg_CoinStock = new FrmDeviceCfg_CoinStock_Hopper();
                frmDeviceCfg_CoinStock.ShowDialog();
            }
            
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—找零纸币管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceCfg_Menu_Cash_Click(object sender, RoutedEventArgs e)
        {
            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Stop)
            {
                // 如果现金支付没有启用
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_CashStop"), PubHelper.MsgType.Ok);
                return;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.IsReturnBill == BusinessEnum.ControlSwitch.Stop)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Err_No"), PubHelper.MsgType.Ok);
                return;
            }
            if (PubHelper.p_BusinOper.ConfigInfo.CashManagerModel == BusinessEnum.CashManagerModel.Singal)
            {
                // 货币高级管理模式没有开通
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoCashManagerModel"), PubHelper.MsgType.Ok);
                return;
            }
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmDeviceCfg_Bill FrmDeviceCfg_Bill = new FrmDeviceCfg_Bill();
            FrmDeviceCfg_Bill.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 纸币接收设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceCfg_Menu_BillCfg_Click(object sender, RoutedEventArgs e)
        {
            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Stop)
            {
                // 如果现金支付没有启用
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_CashStop"), PubHelper.MsgType.Ok);
                return;
            }
            if (PubHelper.p_BusinOper.ConfigInfo.CashManagerModel ==  BusinessEnum.CashManagerModel.Singal)
            {
                // 货币高级管理模式没有开通
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoCashManagerModel"), PubHelper.MsgType.Ok);
                return;
            }

            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmDeviceCfg_BillCfg FrmDeviceCfg_BillCfg = new FrmDeviceCfg_BillCfg();
            FrmDeviceCfg_BillCfg.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }
    }
}
