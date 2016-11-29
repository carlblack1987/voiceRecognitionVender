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
    /// FrmVmDiagnose.xaml 的交互逻辑
    /// </summary>
    public partial class FrmVmDiagnose : Window
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

        public FrmVmDiagnose()
        {
            InitializeComponent();
            bool result = InitForm();
            if (result)
            {
                LoadData();
            }
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

        private bool InitForm()
        {
            bool result = false;
            try
            {
                tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose");
                tbBaseInfo.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_BaseInfo");
                tbDoorTmp.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_DoorTmp");
                tbNetStatus.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_NetStatus");
                tbAsile.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_Asile");
                tbPayMent.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_Payment");
                tbOtherDevice.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_OtherDevice");

                btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
                btnRefresh.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Refresh");

                string strMore = PubHelper.p_LangOper.GetStringBundle("Pub_More");
                btnPayment_More.Content = strMore;
                btnAsile_More.Content = strMore;
                btnTmp_More.Content = strMore;

                #region 初始化基本信息

                tbVmId_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_VmId");
                tbVmId_Value.Text = PubHelper.p_BusinOper.ConfigInfo.VmId;
                tbSoftVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_SoftVer");
                tbSoftVer_Value.Text = PubHelper.p_BusinOper.IVendSoftVer;// "V" + App.ResourceAssembly.GetName(false).Version.ToString();
                tbVmCode_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_VmCode");
                tbVmCode_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("VmCode");
                tbKmbVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_KsmbVer");
                tbKmbVer_Value.Text = PubHelper.p_BusinOper.KmbVer;
                tbArmVer_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_ArmVer");
                tbArmVer_Value.Text = PubHelper.p_BusinOper.DeviceInfo.KmbSoftVer;

                #endregion

                #region 初始化门及温度标签

                tbDoor_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Door");
                tbVendBox.Text = PubHelper.p_LangOper.GetStringBundle("Pub_VendBox_Main");
                tbTmpType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_TmpType");
                tbNowTmp_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Tmp");
                tbTarTmp_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_TargetTmp");

                #endregion

                #region 初始化网络标签

                tbNetKind_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_NetStatus");
                tbNetNum_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_NetNum");
                tbNetSwitch_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg_NetStitch");
                tbNetType_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetType");
                tbNetSoftName_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_SoftName");

                #endregion

                #region 其它设备标签

                tbRef_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Ref");
                tbLight_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Jacklight");
                tbAdvertLamp_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Advlight");

                #endregion

                #region 货道状态

                tbAsile.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_Asile");
                tbAsileCode_Text_1.Visibility = tbAsileCode_Text_2.Visibility = tbAsileCode_Text_3.Visibility =
                    tbAsileCode_Value_1.Visibility = tbAsileCode_Value_2.Visibility = tbAsileCode_Value_3.Visibility =
                     System.Windows.Visibility.Hidden;
                btnAsile_More.Visibility = System.Windows.Visibility.Hidden;

                #endregion

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_OtherErr").Replace("{N}", ex.Message), PubHelper.MsgType.Ok);
            }

            return result;
        }

        private void LoadData()
        {
            try
            {
                AfreshMonOutTime();

                string strRunTitle = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
                string strStopTitle = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");

                #region 正常及故障的颜色定义

                SolidColorBrush brush_Ok = new SolidColorBrush();
                brush_Ok.Color = Colors.White;
                SolidColorBrush brush_Err = new SolidColorBrush();
                brush_Err.Color = Colors.Red;

                #endregion

                #region 门及温度数据

                tbDoor_Value.Text = DictionaryHelper.Dictionary_Door(0);
                string strDoorStatus = PubHelper.p_BusinOper.AsileOper.VendBoxList[0].DoorStatus;
                if ((strDoorStatus != "00") && (strDoorStatus != "01"))
                {
                    tbDoor_Value.Foreground = brush_Err;// Brushes.Red;
                }
                else
                {
                    tbDoor_Value.Foreground = brush_Ok;
                }

                tbTmpType_Value.Text = DictionaryHelper.Dictionary_TmpType(0);
                tbNowTmp_Value.Text = DictionaryHelper.Dictionary_NowTmp(0,true);
                if (PubHelper.p_BusinOper.AsileOper.VendBoxList[0].TmpStatus == "00")
                {
                    tbNowTmp_Value.Foreground = brush_Ok;
                }
                else
                {
                    tbNowTmp_Value.Foreground = brush_Err;
                }

                tbTarTmp_Value.Text = PubHelper.p_BusinOper.AsileOper.VendBoxList[0].TargetTmp + PubHelper.TMP_UNIT;//
                if (PubHelper.p_BusinOper.AsileOper.VendBoxList.Count > 1)
                {
                    btnTmp_More.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    btnTmp_More.Visibility = System.Windows.Visibility.Hidden;
                }
                //PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("TargetTmp1") + PubHelper.TMP_UNIT;

                #endregion

                #region 网络

                PubHelper.p_BusinOper.GetNetStatus();
                tbNetKind_Value.Text = DictionaryHelper.Dictionary_NetStatus();
                if (PubHelper.p_BusinOper.DeviceInfo.NetStatus == Business.Enum.BusinessEnum.NetStatus.OnLine)
                {
                    // 联机
                    tbNetKind_Value.Foreground = brush_Ok;
                }
                else
                {
                    // 离线
                    tbNetKind_Value.Foreground = brush_Err;
                }
                tbNetNum_Value.Text = PubHelper.p_BusinOper.GetWaitNetDataNum().ToString();
                switch (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetSwitch"))
                {
                    case "0":// 关闭
                        tbNetSwitch_Value.Text = strStopTitle;
                        break;
                    case "1":// 开启
                        tbNetSwitch_Value.Text = strRunTitle;
                        break;
                }
                string strNetType = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("NetType");// 网络通信类型
                switch (strNetType)
                {
                    case "0":// DTU方式
                        tbNetType_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetType_DTU");
                        break;
                    default:
                        tbNetType_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Net_NetType_Other");
                        break;
                }
                tbNetSoftName_Value.Text = PubHelper.p_BusinOper.DeviceInfo.NetSoftVer;

                #endregion

                #region 其它设备状态

                tbRef_Value.Text = DictionaryHelper.Dictionary_DeviceStatus(PubHelper.p_BusinOper.AsileOper.VendBoxList[0].RefControl.ControlStatus.ToString());
                tbLight_Value.Text = DictionaryHelper.Dictionary_DeviceStatus(PubHelper.p_BusinOper.AsileOper.VendBoxList[0].LightControl.ControlStatus.ToString());
                tbAdvertLamp_Value.Text = DictionaryHelper.Dictionary_DeviceStatus(PubHelper.p_BusinOper.AsileOper.VendBoxList[0].AdvertLightControl.ControlStatus.ToString());

                #endregion

                #region 货道状态

                int intAsileCount = PubHelper.p_BusinOper.AsileOper.AsileList.Count;
                int intErrAsile = 0;
                string strAsileCode = string.Empty;
                string strAsileStatus = string.Empty;
                string strAsileInfo = string.Empty;
                for (int i = 0; i < intAsileCount; i++)
                {
                    if (PubHelper.p_BusinOper.AsileOper.AsileList[i].PaStatus != "02")
                    {
                        // 货道状态异常
                        intErrAsile++;
                        strAsileCode = PubHelper.p_BusinOper.AsileOper.AsileList[i].PaCode;
                        strAsileStatus = PubHelper.p_BusinOper.AsileOper.AsileList[i].PaStatus;
                        strAsileInfo = DictionaryHelper.Dictionary_AsileStatus(strAsileStatus);
                        if (intErrAsile == 1)
                        {
                            tbAsileCode_Text_1.Visibility = tbAsileCode_Value_1.Visibility = System.Windows.Visibility.Visible;
                            tbAsileCode_Text_1.Text = strAsileCode;
                            tbAsileCode_Value_1.Text = strAsileStatus + "  " + strAsileInfo;
                            tbAsileCode_Text_1.Foreground = tbAsileCode_Value_1.Foreground = brush_Err;
                        }
                        if (intErrAsile == 2)
                        {
                            tbAsileCode_Text_2.Visibility = tbAsileCode_Value_2.Visibility = System.Windows.Visibility.Visible;
                            tbAsileCode_Text_2.Text = strAsileCode;
                            tbAsileCode_Value_2.Text = strAsileStatus + "  " + strAsileInfo;
                            tbAsileCode_Text_2.Foreground = tbAsileCode_Value_2.Foreground = brush_Err;
                        }
                        if (intErrAsile == 3)
                        {
                            tbAsileCode_Text_3.Visibility = tbAsileCode_Value_3.Visibility = System.Windows.Visibility.Visible;
                            tbAsileCode_Text_3.Text = strAsileCode;
                            tbAsileCode_Value_3.Text = strAsileStatus + "  " + strAsileInfo;
                            tbAsileCode_Text_3.Foreground = tbAsileCode_Value_3.Foreground = brush_Err;
                        }
                        if (intErrAsile == 3)
                        {
                            // 超过4个，显示更多
                            btnAsile_More.Visibility = System.Windows.Visibility.Visible;
                            break;
                        }
                    }
                }
                if (intErrAsile == 0)
                {
                    // 货道正常
                    tbAsileCode_Text_1.Visibility = System.Windows.Visibility.Visible;
                    tbAsileCode_Text_1.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
                    tbAsileCode_Text_1.Foreground = brush_Ok;
                }

                #endregion

                #region 支付方式

                tbCashControl_Text.Text = tbCash_Text.Text = tbCoin_Text.Text =
                            tbCashControl_Value.Text = tbCash_Value.Text = tbCoin_Value.Text = string.Empty;

                // 获取当前支付方式数量
                int intEnablePayNum = PubHelper.p_BusinOper.PaymentOper.GetOpenPayNum();
                switch (intEnablePayNum)
                {
                    case 0:// 没有支付方式
                        btnPayment_More.Visibility = System.Windows.Visibility.Hidden;
                        tbCashControl_Text.Visibility = tbCash_Text.Visibility = tbCoin_Text.Visibility =
                            tbCashControl_Value.Visibility = tbCash_Value.Visibility = tbCoin_Value.Visibility =
                             System.Windows.Visibility.Hidden;
                        break;
                    case 1:// 只有一种支付方式
                        btnPayment_More.Visibility = System.Windows.Visibility.Hidden;// 隐藏更多按钮
                        break;
                    default:// 多种支付方式
                        btnPayment_More.Visibility = System.Windows.Visibility.Visible;// 显示更多按钮
                        break;
                }
                if (intEnablePayNum == 0)
                {
                    return;
                }

                #region 现金支付
                if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.Cash))
                {
                    // 如果现金支付方式开启，则界面上默认显示现金支付方式的相关信息
                    tbCashControl_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Cash_Title");
                    tbCash_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Bill");
                    tbCoin_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Coin");

                    switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.Cash.ControlSwitch)
                    {
                        case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                            tbCashControl_Value.Text = strStopTitle;
                            break;
                        case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                            tbCashControl_Value.Text = strRunTitle;
                            break;
                    }
                    tbCash_Value.Text = DictionaryHelper.Dictionary_CashStatus();
                    tbCoin_Value.Text = DictionaryHelper.Dictionary_CoinStatus();

                    return;
                }
                #endregion
                #region 储值卡支付
                if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.IcCard))
                {
                    // 如果储值卡支付方式开启，则界面上默认显示储值卡支付方式的相关信息
                    tbCashControl_Text.Text = PubHelper.ConvertIcCardPayName();// PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_Title");
                    tbCash_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Card_Soft");// 组件版本
                    tbCoin_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Ic");// 设备状态

                    switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.IC.ControlSwitch)
                    {
                        case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                            tbCashControl_Value.Text = strStopTitle;
                            break;
                        case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                            tbCashControl_Value.Text = strRunTitle;
                            break;
                    }
                    tbCash_Value.Text = PubHelper.p_BusinOper.DeviceInfo.ICSoftVer;// 组件版本值
                    tbCoin_Value.Text = DictionaryHelper.Dictionary_ICStatus();// 设备状态值

                    return;
                }
                #endregion
                #region 非储值卡（磁条卡支付）
                if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.NoFeeCard))
                {
                    // 如果支付方式开启，则界面上默认显示支付方式的相关信息
                    tbCashControl_Text.Text = PubHelper.ConvertNoFeeCardPayName();// PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_Title");
                    tbCash_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_NoFeeCard_Soft");// 组件版本
                    tbCoin_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_NoFeeCard");// 设备状态

                    switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.NoFeeCard.ControlSwitch)
                    {
                        case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                            tbCashControl_Value.Text = strStopTitle;
                            break;
                        case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                            tbCashControl_Value.Text = strRunTitle;
                            break;
                    }
                    tbCash_Value.Text = PubHelper.p_BusinOper.DeviceInfo.NoFeeCardSoftVer;// 组件版本值
                    tbCoin_Value.Text = DictionaryHelper.Dictionary_NoFeeCardStatus();// 设备状态值

                    return;
                }
                #endregion
                #region 银联支付
                if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.QuickPass))
                {
                    // 如果支付方式开启，则界面上默认显示支付方式的相关信息
                    tbCashControl_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UnionPay_Title");
                    tbCash_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UnionPay_Soft");// 组件版本
                    tbCoin_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_UnionPay");// 设备状态

                    switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.UnionPay.ControlSwitch)
                    {
                        case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                            tbCashControl_Value.Text = strStopTitle;
                            break;
                        case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                            tbCashControl_Value.Text = strRunTitle;
                            break;
                    }
                    tbCash_Value.Text = PubHelper.p_BusinOper.DeviceInfo.UnionPaySoftVer;// 组件版本值
                    tbCoin_Value.Text = DictionaryHelper.Dictionary_UnionPayStatus();// 设备状态值

                    return;
                }
                #endregion
                #region 二维码支付
                if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.QRCodeCard))
                {
                    // 如果支付方式开启，则界面上默认显示支付方式的相关信息
                    tbCashControl_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_QrCode_Title");
                    tbCash_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_QrCode_Soft");// 组件版本
                    tbCoin_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_QRCode");// 设备状态

                    switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.QRCodeCard.ControlSwitch)
                    {
                        case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                            tbCashControl_Value.Text = strStopTitle;
                            break;
                        case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                            tbCashControl_Value.Text = strRunTitle;
                            break;
                    }
                    tbCash_Value.Text = PubHelper.p_BusinOper.DeviceInfo.QrSoftVer;// 组件版本值
                    tbCoin_Value.Text = DictionaryHelper.Dictionary_QRCodeStatus();// 设备状态值

                    return;
                }
                #endregion
                #region 微信扫码支付
                if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.WeChatCode))
                {
                    // 如果支付方式开启，则界面上默认显示支付方式的相关信息
                    tbCashControl_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_WeChatCode_Title");
                    tbCash_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_WeChatCode_Soft");// 组件版本
                    tbCoin_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_QRCode");// 设备状态

                    switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.WeChatCode.ControlSwitch)
                    {
                        case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                            tbCashControl_Value.Text = strStopTitle;
                            break;
                        case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                            tbCashControl_Value.Text = strRunTitle;
                            break;
                    }
                    tbCash_Value.Text = PubHelper.p_BusinOper.DeviceInfo.AliPayWaveSoftVer;// 组件版本值
                    tbCoin_Text.Visibility = tbCoin_Value.Visibility = System.Windows.Visibility.Hidden;
                    ////tbCoin_Value.Text = DictionaryHelper.Dictionary_AliPayWaveStatus();// 设备状态值

                    return;
                }
                #endregion

                #region 支付宝扫码支付
                if (PubHelper.p_BusinOper.PaymentOper.CheckPaymentIsExist(Business.Enum.BusinessEnum.PayMent.AliPay_Code))
                {
                    // 如果支付方式开启，则界面上默认显示支付方式的相关信息
                    tbCashControl_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_AliPayCode_Title");
                    tbCash_Text.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_AliPayCode_Soft");// 组件版本
                    tbCoin_Text.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Device_QRCode");// 设备状态

                    switch (PubHelper.p_BusinOper.PaymentOper.PaymentList.AliPay_Code.ControlSwitch)
                    {
                        case Business.Enum.BusinessEnum.ControlSwitch.Stop:// 关闭
                            tbCashControl_Value.Text = strStopTitle;
                            break;
                        case Business.Enum.BusinessEnum.ControlSwitch.Run:// 开启
                            tbCashControl_Value.Text = strRunTitle;
                            break;
                    }
                    tbCash_Value.Text = PubHelper.p_BusinOper.DeviceInfo.AliPayWaveSoftVer;// 组件版本值
                    ////tbCoin_Value.Text = DictionaryHelper.Dictionary_AliPayWaveStatus();// 设备状态值
                    tbCoin_Text.Visibility = tbCoin_Value.Visibility = System.Windows.Visibility.Hidden;

                    return;
                }
                #endregion

                #endregion
            }
            catch(Exception ex)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_OtherErr").Replace("{N}", ex.Message), PubHelper.MsgType.Ok);
            }
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
            m_MonOutTime = 10;
            string strOutTime = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("TermStatusShowTime");
            if (!string.IsNullOrEmpty(strOutTime))
            {
                m_MonOutTime = Convert.ToInt32(strOutTime);
            }

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
            StopMonOutTime();
            FrmVmDiagnose_Payment frmPayment = new FrmVmDiagnose_Payment();
            frmPayment.ShowDialog();

            AfreshMonOutTime();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 更多故障的货道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAsile_More_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmVmDiagnose_Asile frmAsile = new FrmVmDiagnose_Asile();
            frmAsile.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            AfreshMonOutTime();
            m_IsMonTime = true;
        }

        private void btnTmp_More_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();

            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmVmDiagnose_Tmp frmVmDiagnose_Tmp = new FrmVmDiagnose_Tmp();
            frmVmDiagnose_Tmp.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            AfreshMonOutTime();
            m_IsMonTime = true;
        }
    }
}
