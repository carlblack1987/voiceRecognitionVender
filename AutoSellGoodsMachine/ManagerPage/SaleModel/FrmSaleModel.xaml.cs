#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：销售模式
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

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
    /// FrmSaleModel.xaml 的交互逻辑
    /// </summary>
    public partial class FrmSaleModel : Window
    {
        public FrmSaleModel()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            #region 加载标签

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_SaleModel");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");

            tbSaleModel.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_SaleModel_SaleModel");
            tbChangeModel.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_SaleModel_ChangeModel");
            tbDropModel.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_SaleModel_DropModel");
            tbFreeSale.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_SaleModel_FreeSale");

            rdbSaleModel_Run.Content = PubHelper.p_LangOper.GetStringBundle("Pub_SaleModel_Circle");
            rdbSaleModel_Stop.Content = PubHelper.p_LangOper.GetStringBundle("Pub_SaleModel_Single");

            string strRun = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            string strStop = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            rdbChangeModel_Run.Content = rdbDropModel_Run.Content = rdbFreeSale_Run.Content = strRun;
            rdbChangeModel_Stop.Content = rdbDropModel_Stop.Content = rdbFreeSale_Stop.Content = strStop;

            tbTipInfo_SaleModel.Text = tbTipInfo_ChangeModel.Text = PubHelper.p_LangOper.GetStringBundle("Tip_OnlyCashValid");
            tbTipInfo_DropModel.Text = PubHelper.p_LangOper.GetStringBundle("Tip_OnlySpringValid");

            #endregion

            #region 加载数据

            if (PubHelper.p_BusinOper.ConfigInfo.SaleModel == "0")
            {
                // 连续
                rdbSaleModel_Run.IsChecked = true;
            }
            else
            {
                rdbSaleModel_Run.IsChecked = false;
            }

            if (PubHelper.p_BusinOper.ConfigInfo.ChangeModel == "0")
            {
                // 兑零功能（关闭）
                rdbChangeModel_Run.IsChecked = false;
            }
            else
            {
                rdbChangeModel_Run.IsChecked = true;
            }

            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Stop)
            {
                // 如果现金支付方式被关闭，则销售模式和找零模式不可用
                rdbSaleModel_Run.IsEnabled = rdbSaleModel_Stop.IsEnabled = rdbChangeModel_Run.IsEnabled =
                    rdbChangeModel_Stop.IsEnabled = false;
            }

            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("DropModel") == "0")
            {
                // 掉货检测（关闭）
                rdbDropModel_Run.IsChecked = false;
            }
            else
            {
                rdbDropModel_Run.IsChecked = true;
            }

            for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.VendBoxList.Count; i++)
            {
                if (PubHelper.p_BusinOper.AsileOper.VendBoxList[i].SellGoodsType == Business.Enum.BusinessEnum.SellGoodsType.Spring)
                {

                }
            }

            if (!PubHelper.p_BusinOper.AsileOper.UpDown_CheckIsExistSpring())
            {
                // 如果出货方式不属于弹簧出货，则掉货检测不可用
                rdbDropModel_Run.IsEnabled = rdbDropModel_Stop.IsEnabled = false;
            }

            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("FreeSale") == "0")
            {
                // 免费销售（关闭）
                rdbFreeSale_Run.IsChecked = false;
            }
            else
            {
                rdbFreeSale_Run.IsChecked = true;
            }

            #endregion
        }

        private void ControlButton(bool enable)
        {
            btnSave.IsEnabled = enable;
            btnCancel.IsEnabled = enable;
            DispatcherHelper.SleepControl();
        }

        /// <summary>
        /// 按钮—取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 按钮—保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strSaleModel = "0";
            string strChangeModel = "0";
            string strDropModel = "0";
            string strFreeSale = "0";

            ControlButton(false);
            DispatcherHelper.DoEvents();

            if (rdbSaleModel_Run.IsChecked == false)
            {
                strSaleModel = "1";
            }
            if (rdbChangeModel_Run.IsChecked == true)
            {
                strChangeModel = "1";
            }
            if (rdbDropModel_Run.IsChecked == true)
            {
                strDropModel = "1";
            }
            if (rdbFreeSale_Run.IsChecked == true)
            {
                strFreeSale = "1";
            }

            // 保存数据
            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                PubHelper.p_BusinOper.UpdateSysCfgValue("SaleModel", strSaleModel);
                PubHelper.p_BusinOper.UpdateSysCfgValue("ChangeModel", strChangeModel);
            }
            ////if (PubHelper.p_BusinOper.DeviceInfo.SellGoodsType == Business.Enum.BusinessEnum.SellGoodsType.Spring)
            ////{
            ////    PubHelper.p_BusinOper.UpdateSysCfgValue("DropModel", strDropModel);
            ////}
            PubHelper.p_BusinOper.UpdateSysCfgValue("DropModel", strDropModel);

            PubHelper.p_BusinOper.UpdateSysCfgValue("FreeSale", strFreeSale);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            this.Close();
        }
    }
}
