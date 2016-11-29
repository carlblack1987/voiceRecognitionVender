#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：机器诊断
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
using System.Threading;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmVmDiagnose_Payment.xaml 的交互逻辑
    /// </summary>
    public partial class FrmVmDiagnose_Payment : Window
    {
        #region 变量声明

        /// <summary>
        /// 是否关闭窗体 False：不关闭 True：关闭
        /// </summary>
        private bool m_CloseForm = false;

        /// <summary>
        /// 是否启动操作时间超时监控
        /// </summary>
        private bool m_IsMonTime = false;

        /// <summary>
        /// 监控操作超时参数
        /// </summary>
        private int m_OutNum = 0;
        private int m_OperNum = 0;

        /// <summary>
        /// 监控操作的超时时间，以秒为单位
        /// </summary>
        private int m_MonOutTime = 0;

        #endregion

        public FrmVmDiagnose_Payment()
        {
            InitializeComponent();
            InitForm();
            LoadData();
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 启动超时控制工作线程

            Thread TrdMonOutTime = new Thread(new ThreadStart(MonOutTimeTrd));
            TrdMonOutTime.IsBackground = true;
            TrdMonOutTime.Start();

            m_IsMonTime = true;

            #endregion
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_IsMonTime = false;

            m_CloseForm = true;
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_Payment");

            tbCashTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Cash_Title");
            tbPosTitle.Text = PubHelper.ConvertIcCardPayName();// PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_Title");
            tbNoFeeCardTitle.Text = PubHelper.ConvertNoFeeCardPayName();// PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_Title");
            tbWeChatCodeTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_WeChatCode_Title");
            tbAliPayCodeTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_AliPayCode_Title");
            tbBestPayCodeTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_BestPayCode_Title");
            tbUnionPayTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UnionPay_Title");
            tbQRTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_QrCode_Title");

            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
        }

        private void LoadData()
        {
            AfreshMonOutTime();

            string strRunTitle = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            string strStopTitle = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            string strSwitchTitle = PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Control");
            string strNoPayment = PubHelper.p_LangOper.GetStringBundle("Pub_NoExist");

            #region 现金支付
            if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.Cash))
            {
                tbCashSwitch_Text.Text = strSwitchTitle;
                tbBill_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Bill");
                tbCoin_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Coin");

                switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch)
                {
                    case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                        tbCashSwitch_Value.Text = strStopTitle;
                        break;
                    case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                        tbCashSwitch_Value.Text = strRunTitle;
                        break;
                }
                tbBill_Value.Text = DictionaryHelper.Dictionary_CashStatus();
                tbCoin_Value.Text = DictionaryHelper.Dictionary_CoinStatus();
            }
            else
            {
                tbCashSwitch_Text.Text = strNoPayment;
                tbCashSwitch_Value.Visibility = tbBill_Text.Visibility = tbBill_Value.Visibility =
                    tbCoin_Text.Visibility = tbCoin_Value.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
            #region 储值卡支付
            if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.IcCard))
            {
                tbPosSwitch_Text.Text = strSwitchTitle;

                tbPosVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_Soft");// 组件版本
                tbPosStatus_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Ic");// 设备状态

                switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.IC.ControlSwitch)
                {
                    case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                        tbPosSwitch_Value.Text = strStopTitle;
                        break;
                    case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                        tbPosSwitch_Value.Text = strRunTitle;
                        break;
                }
                tbPosVer_Value.Text = PubHelper.p_BusinOper.DeviceInfo.ICSoftVer;// 组件版本值
                tbPosStatus_Value.Text = DictionaryHelper.Dictionary_ICStatus();// 设备状态值
            }
            else
            {
                tbPosSwitch_Text.Text = strNoPayment;
                tbPosSwitch_Value.Visibility = tbPosVer_Text.Visibility = tbPosStatus_Text.Visibility =
                    tbPosVer_Value.Visibility = tbPosStatus_Value.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
            #region 非储值卡（磁条卡支付）
            if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.NoFeeCard))
            {
                tbNoFeeCardSwitch_Text.Text = strSwitchTitle;

                tbNoFeeCardVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_Soft");// 组件版本
                tbNoFeeCardStatus_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_NoFeeCard");// 设备状态

                switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.NoFeeCard.ControlSwitch)
                {
                    case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                        tbNoFeeCardSwitch_Value.Text = strStopTitle;
                        break;
                    case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                        tbNoFeeCardSwitch_Value.Text = strRunTitle;
                        break;
                }
                tbNoFeeCardVer_Value.Text = PubHelper.p_BusinOper.DeviceInfo.NoFeeCardSoftVer;// 组件版本值
                tbNoFeeCardStatus_Value.Text = DictionaryHelper.Dictionary_NoFeeCardStatus();// 设备状态值
            }
            else
            {
                tbNoFeeCardSwitch_Text.Text = strNoPayment;
                tbNoFeeCardSwitch_Value.Visibility = tbNoFeeCardVer_Text.Visibility = tbNoFeeCardStatus_Text.Visibility =
                    tbNoFeeCardVer_Value.Visibility = tbNoFeeCardStatus_Value.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
            #region 银联支付
            if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.QuickPass))
            {
                tbUnipnPaySwitch_Text.Text = strSwitchTitle;

                tbUnionPayVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UnionPay_Soft");// 组件版本

                switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.UnionPay.ControlSwitch)
                {
                    case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                        tbUnipnPaySwitch_Value.Text = strStopTitle;
                        break;
                    case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                        tbUnipnPaySwitch_Value.Text = strRunTitle;
                        break;
                }
                tbUnionPayVer_Value.Text = PubHelper.p_BusinOper.DeviceInfo.UnionPaySoftVer;// 组件版本值
            }
            else
            {
                tbUnipnPaySwitch_Text.Text = strNoPayment;
                tbUnipnPaySwitch_Value.Visibility = tbUnionPayVer_Text.Visibility =
                    tbUnionPayVer_Value.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
            #region 二维码支付
            if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.QRCodeCard))
            {
                tbQRSwitch_Text.Text = strSwitchTitle;

                tbQRVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_QrCode_Soft");// 组件版本
                tbQRStatus_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_QRCode");// 设备状态

                switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.QRCodeCard.ControlSwitch)
                {
                    case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                        tbQRSwitch_Value.Text = strStopTitle;
                        break;
                    case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                        tbQRSwitch_Value.Text = strRunTitle;
                        break;
                }
                tbQRVer_Value.Text = PubHelper.p_BusinOper.DeviceInfo.QrSoftVer;// 组件版本值
                tbQRStatus_Value.Text = DictionaryHelper.Dictionary_QRCodeStatus();// 设备状态值
            }
            else
            {
                tbQRSwitch_Text.Text = strNoPayment;
                tbQRSwitch_Value.Visibility = tbQRVer_Text.Visibility = tbQRStatus_Text.Visibility =
                    tbQRVer_Value.Visibility = tbQRStatus_Value.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
            #region 微信扫码支付
            if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.WeChatCode))
            {
                tbWeChatCodeSwitch_Text.Text = strSwitchTitle;

                ////tbWeChatCodeVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_WeChatCode_Soft");// 组件版本

                switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.WeChatCode.ControlSwitch)
                {
                    case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                        tbWeChatCodeSwitch_Value.Text = strStopTitle;
                        break;
                    case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                        tbWeChatCodeSwitch_Value.Text = strRunTitle;
                        break;
                }
                ////tbWeChatCodeVer_Value.Text = PubHelper.p_BusinOper.DeviceInfo.AliPayWaveSoftVer;// 组件版本值
            }
            else
            {
                tbWeChatCodeSwitch_Text.Text = strNoPayment;
                tbWeChatCodeSwitch_Value.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
            #region 支付宝扫码支付
            if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.AliPay_Code))
            {
                tbAliPayCodeSwitch_Text.Text = strSwitchTitle;

                ////tbAliPayCodeVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_AliPayCode_Soft");// 组件版本

                switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.AliPay_Code.ControlSwitch)
                {
                    case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                        tbAliPayCodeSwitch_Value.Text = strStopTitle;
                        break;
                    case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                        tbAliPayCodeSwitch_Value.Text = strRunTitle;
                        break;
                }
                ////tbAliPayCodeVer_Value.Text = PubHelper.p_BusinOper.DeviceInfo.AliPayCodeSoftVer;// 组件版本值
            }
            else
            {
                tbAliPayCodeSwitch_Text.Text = strNoPayment;
                tbAliPayCodeSwitch_Value.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
            #region 翼支付付款码支付
            if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.BestPay_Code))
            {
                tbBestPayCodeSwitch_Text.Text = strSwitchTitle;

                ////tbAliPayCodeVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_AliPayCode_Soft");// 组件版本

                switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.BestPay_Code.ControlSwitch)
                {
                    case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                        tbBestPayCodeSwitch_Value.Text = strStopTitle;
                        break;
                    case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                        tbBestPayCodeSwitch_Value.Text = strRunTitle;
                        break;
                }
                ////tbAliPayCodeVer_Value.Text = PubHelper.p_BusinOper.DeviceInfo.AliPayCodeSoftVer;// 组件版本值
            }
            else
            {
                tbBestPayCodeSwitch_Text.Text = strNoPayment;
                tbBestPayCodeSwitch_Value.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        #region 超时监控业务控制

        /// <summary>
        /// 超时监控主业务流程
        /// </summary>
        private void MonOutTimeTrd()
        {
            // 获取超时时间
            m_MonOutTime = Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("TermStatusShowTime"));

            while (!m_CloseForm)
            {
                Thread.Sleep(20);

                if (!m_IsMonTime)
                {
                    // 重新开始超时监控
                    AfreshMonOutTime();
                }
                else
                {
                    m_OutNum++;
                    if (m_OutNum >= 50)
                    {
                        m_OutNum = 0;
                        m_OperNum++;

                        try
                        {
                            this.tbTitle.Dispatcher.Invoke(new Action(() =>
                            {
                                if (!m_CloseForm)
                                {
                                    if (m_OperNum > m_MonOutTime)
                                    {
                                        // 超时，自动返回
                                        // 重新开始超时监控
                                        AfreshMonOutTime();
                                        m_IsMonTime = true;
                                        //OperStepKind();

                                        this.Close();
                                        //Application.DoEvents();
                                    }
                                    else
                                    {
                                        ////if (tbOutTime.Visibility == System.Windows.Visibility.Hidden)
                                        ////{
                                        ////    tbOutTime.Visibility = System.Windows.Visibility.Visible;
                                        ////}
                                        ////// 显示剩余时间提示
                                        ////tbOutTime.Text = (m_MonOutTime - m_OperNum + 1).ToString();

                                        DispatcherHelper.DoEvents();
                                    }
                                }
                            }));
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 重新开始超时监控
        /// </summary>
        private void AfreshMonOutTime()
        {
            m_OutNum = 0;
            m_OperNum = 0;
        }

        /// <summary>
        /// 停止超时监控
        /// </summary>
        private void StopMonOutTime()
        {
            m_IsMonTime = false;
        }

        #endregion

        /// <summary>
        /// 按钮—更多—支付方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPayment_More_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
