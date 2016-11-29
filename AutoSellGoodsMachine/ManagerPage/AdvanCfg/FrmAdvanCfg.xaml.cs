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
    /// FrmAdvanCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg : Window
    {
        public FrmAdvanCfg()
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
            tbMenuTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg");

            AdvanCfg_Menu_Base.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_Title");
            AdvanCfg_Menu_Cancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            AdvanCfg_Menu_Payment.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Payment_Title");
            AdvanCfg_Menu_Net.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_Title");
            AdvanCfg_Menu_Skin.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Skin_Title");
            AdvanCfg_Menu_ResetData.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_ResetData_Title");
            AdvanCfg_Menu_ResetCfg.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_ResetCfg_Title");
            AdvanCfg_Menu_OutTime.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_Title");
            AdvanCfg_Menu_SellGoods.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_Title");

            AdvanCfg_Menu_ShZyZCfg.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_ShZyZ_Title");
            AdvanCfg_Menu_AddService.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Title");

            AdvanCfg_Menu_Log.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_Title");

            AdvanCfg_Menu_CfgUpload.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_CfgUpload_Title");

            if (PubHelper.p_BusinOper.ConfigInfo.MainLgsTop_Type == Business.Enum.BusinessEnum.Main_Lgs_TopType.ShangHai_ZhiYuanZhe)
            {
                AdvanCfg_Menu_ShZyZCfg.IsEnabled = true;
            }
            else
            {
                AdvanCfg_Menu_ShZyZCfg.IsEnabled = false;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.AddedServiceSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                AdvanCfg_Menu_AddService.IsEnabled = true;
            }
            else
            {
                AdvanCfg_Menu_AddService.IsEnabled = false;
            }
        }

        /// <summary>
        /// 菜单—基础设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_Base_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_Base frmAdvanCfg_Base = new FrmAdvanCfg_Base();
            frmAdvanCfg_Base.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—通信设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_Net_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_Net frmAdvanCfg_Net = new FrmAdvanCfg_Net();
            frmAdvanCfg_Net.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—出货设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_SellGoods_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_SellGoods frmAdvacCfg_SellGoods = new FrmAdvanCfg_SellGoods();
            frmAdvacCfg_SellGoods.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—超时设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_OutTime_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_OutTime frmAdvanCfg_OutTime = new FrmAdvanCfg_OutTime();
            frmAdvanCfg_OutTime.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—支付设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_Payment_Click(object sender, RoutedEventArgs e)
        {
            FrmAdvanCfg_PaymentMenu frmAdvanCfg_PaymentMenu = new FrmAdvanCfg_PaymentMenu();
            frmAdvanCfg_PaymentMenu.ShowDialog();
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
        /// 菜单—日志设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_Log_Click(object sender, RoutedEventArgs e)
        {
            FrmAdvanCfg_Log frmAdvanCfg_Log = new FrmAdvanCfg_Log();
            this.Opacity = PubHelper.OPACITY_GRAY;
            frmAdvanCfg_Log.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—界面样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_Skin_Click(object sender, RoutedEventArgs e)
        {
            string strOldSkinStyle = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("SkinStyle");

            FrmAdvanCfg_Skin frmAdvanCfg_Skin = new FrmAdvanCfg_Skin();
            frmAdvanCfg_Skin.ShowDialog();
            if (strOldSkinStyle != PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("SkinStyle"))
            {
                // 界面皮肤需要刷新
                PubHelper.p_IsRefreshSkin = true;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("HideCuror") == "1")
            {
                // 如果需要隐藏鼠标，则隐藏
                Mouse.OverrideCursor = Cursors.None;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// 菜单—参数更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_CfgUpload_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_CfgUpload frmAdvanCfg_CfgUpload = new FrmAdvanCfg_CfgUpload();
            frmAdvanCfg_CfgUpload.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—升降设备设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_UpDownCfg_Click(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// 菜单—恢复出厂数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_ResetData_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_ResetType = "0";
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_Reset frmAdvanCfg_Reset = new FrmAdvanCfg_Reset();
            frmAdvanCfg_Reset.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }

        /// <summary>
        /// 菜单—恢复出厂参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_ResetCfg_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_ResetType = "1";
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmAdvanCfg_Reset frmAdvanCfg_Reset = new FrmAdvanCfg_Reset();
            frmAdvanCfg_Reset.ShowDialog();
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

        /// <summary>
        /// 菜单—增值服务设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_AddService_Click(object sender, RoutedEventArgs e)
        {
            FrmAddServiceMenu frmAddServiceMenu = new FrmAddServiceMenu();
            frmAddServiceMenu.ShowDialog();
        }

        /// <summary>
        /// 菜单—上海志愿设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdvanCfg_Menu_ShZyZCfg_Click(object sender, RoutedEventArgs e)
        {
            FrmAdvanCfg_ShZyZCfg frmAdvanCfg_ShZyZCfg = new FrmAdvanCfg_ShZyZCfg();
            frmAdvanCfg_ShZyZCfg.ShowDialog();
        }
    }
}
