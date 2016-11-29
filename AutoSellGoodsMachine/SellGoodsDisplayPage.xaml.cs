#region [ KIMMA Co.,Ltd. Copyright (C) 2013 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件售货主业务
// 业务功能：iVend终端软件售货主业务
// 创建标识：2013-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoSellGoodsMachine.BulidPage;
using AutoSellGoodsMachine.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using AutoSellGoodsMachineBusiness.StoryBoard;
using System.Threading;
using System.Diagnostics;

//using Business;
using Business.Model;
using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region WPF变量声明

        private Dictionary<AsileModel, ProductItemControl> controlMap = new Dictionary<AsileModel, ProductItemControl>();
        private Dictionary<GoodsModel, GoodsItemControl> controlMap_Goods = new Dictionary<GoodsModel, GoodsItemControl>();
        
        private MusicManager music = null;

        private Storyboard story = new Storyboard();

        private DispatcherTimer timer = new DispatcherTimer();

        #endregion

        #region 变量声明

        /// <summary>
        /// 是否关闭系统 false：不关闭 true：关闭
        /// </summary>
        private bool m_CloseForm = false;

        /// <summary>
        /// 当前选择的货道外部编号或商品编码
        /// </summary>
        private string m_CurrentChoiceCode = string.Empty;

        /// <summary>
        /// 当前点击的商品类型编码
        /// </summary>
        private string m_CurrentGoodsType = string.Empty;

        /// <summary>
        /// 货道价格
        /// </summary>
        private int m_PaPrice = 0;

        /// <summary>
        /// 业务主线程是否结束 False：未结束 True：结束
        /// </summary>
        private bool m_MainMonTrd = false;

        /// <summary>
        /// 超时监控线程是否结束 False：未结束 True：结束
        /// </summary>
        private bool m_OutTimeMonTrd = false;

        #region 语音识别相关变量

        /// <summary>
        /// 语音识别线程是否结束 False：未结束 True：结束
        /// </summary>
        private bool m_VoiceRecognitionTrd = false;

        /// <summary>
        /// 语音识别线程是否暂停 False：未暂停 True：暂停
        /// </summary>
        private bool m_VoiceRecognitionTrdSus = false;

        #region 语音识别串口信息

        private SerialPort vrt_Port = null;
        private String vrt_PortName = "COM7";//串口名称
        private bool vrt_IsConnected = false;//是否已连接
        private int vrt_BaudRate = 9600;//波特率
        private long vrt_Received_Count = 0;//接收计数
        private StringBuilder vrt_Builder = new StringBuilder();
        private String vrt_TestCommandSend = "voice,99,$"; //用来测试语音识别串口的发送命令
        private String vrt_TestCommandRecv = "B4"; //用来测试语音识别串口的接收命令

        //创建一个委托，是为访问TextBox控件服务的。
        public delegate void UpdateProductDetail(string selectCode, bool isInput, bool isGift);
        //定义一个委托变量
        public UpdateProductDetail updateProductDetail;

        #endregion

        #endregion

        /// <summary>
        /// 是否启动操作时间超时监控 False：不启动 True：启动
        /// </summary>
        private bool m_IsMonTime = false;

        /// <summary>
        /// 监控操作超时参数
        /// </summary>
        private int m_OutNum = 0;
        private int m_OperNum = 0;

        /// <summary>
        /// 无投币操作时无操作超时时间，以秒为单位
        /// </summary>
        private int m_SellOperOutTime = 0;

        /// <summary>
        /// 是否查询金额 False：不查询 True：查询
        /// </summary>
        private bool m_BlnQueryMoney = false;

        /// <summary>
        /// 是否需要退币 False：不需要退币 True：需要退币
        /// </summary>
        private bool m_BlnIsReturnMoney = false;

        /// <summary>
        /// 是否需要出货 False：不需要 True：需要出货
        /// </summary>
        private bool m_BlnIsSellGoods = false;

        /// <summary>
        /// 是否需要查询所选货道最新状态 False：不需要 True：需要
        /// </summary>
        private bool  m_BlnQueryPaStatus = false;

        /// <summary>
        /// 检测是否允许购买当前商品的结果码 0：允许购买 其它：不允许购买
        /// </summary>
        private BusinessEnum.ServerReason m_CheckSaleEnvirCode = BusinessEnum.ServerReason.Normal;

        /// <summary>
        /// 查询金额途径 0：商品详细界面查询金额 1：商品主界面查询金额，主要是退币
        /// </summary>
        private string m_QueryCashSource = "0";

        /// <summary>
        /// 是否允许点击商品 False：不允许 True：允许
        /// </summary>
        private bool m_IsClickGoods = true;

        /// <summary>
        /// 点击商品时，是否已经检查完能否购买商品 False：否 True：是
        /// </summary>
        private bool m_IsCheckSaleEnvir = true;

        #region 2015-08-08添加 主界面上各显示区域显示参数

        /// <summary>
        /// 是否显示运营商相关信息区域—底部
        /// </summary>
        private bool m_IsShowLgsArea_Bottom = false;

        /// <summary>
        /// 是否显示运营商相关信息区域—顶部
        /// </summary>
        private bool m_IsShowLgsArea_Top = false;

        /// <summary>
        /// 是否显示取货码等服务显示区域
        /// </summary>
        private bool m_IsShowServerArea = false;

        /// <summary>
        /// 是否显示现金处理区域
        /// </summary>
        private bool m_IsShowMoneyArea = false;

        #endregion

        #endregion

        #region 窗体操作

        /// <summary>
        /// 初始化窗体
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            PubHelper.WriteSystemLog("iVend Soft Begin Run");

            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Loading;

            LoadWholePage("");
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += new EventHandler(Timer1_Tick);
            timer.Interval = TimeSpan.FromSeconds(0.01);

            timer.Start();
        }

        /// <summary>
        /// 窗体键盘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:// F1关闭系统
                    if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main) ||
                        (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.InitErr))
                    {
                        this.Close();
                    }
                    break;

                case Key.F2:// F2后台登陆
                    if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main)
                    {
                        ShowLogin();
                    }
                    break;

                case Key.F3:
                    m_BlnIsSellGoods = true;
                    break;
                case Key.F4:
                    ReturnGoodsTypeList();
                    break;

            }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_CloseForm = true;

            PubHelper.WriteSystemLog("iVend Soft Closeing");

            PubHelper.p_IsStopOper = true;

            if (music != null)
            {
                music.Stop();
            }
            StopMainStory();

            LoadWholePage(PubHelper.p_LangOper.GetStringBundle("Main_Closing"));

            DispatcherHelper.SleepControl();

            PubHelper.p_BusinOper.Displose();

            while (true)
            {
                DispatcherHelper.SleepControl();
                if ((m_MainMonTrd) && (m_OutTimeMonTrd))
                {
                    // 如果线程全部结束
                    break;
                }
            }

            // 控制相关硬件设备状态
            PubHelper.p_BusinOper.InitDevice();

            // 释放语言资源类
            PubHelper.p_LangOper.Dispose();

            // 释放界面资源类
            PubHelper.p_UIOper.Dispose();

            DispatcherHelper.SleepControl(1000);

            PubHelper.WriteSystemLog("iVend Soft Closed");
            PubHelper.WriteSystemLog("*******************");
        }

        private void ErrClose_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.InitErr)
            {
                PubHelper.p_LoginType = "1";// 设备故障，非正常状态下登陆

                PubHelper.p_CheckLoginResult = false;

                FrmLogin frmLogin = new FrmLogin();
                frmLogin.ShowDialog();

                if (PubHelper.p_CheckLoginResult)
                {
                    // 登陆验证成功，退出系统
                    this.Close();
                }
            }
        }

        private void ErrClose_MainInfo_Click(object sender, MouseButtonEventArgs e)
        {
            if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main) && (!PubHelper.p_SoftWorkStatus))
            {
                PubHelper.p_LoginType = "1";// 设备故障，非正常状态下登陆

                PubHelper.p_CheckLoginResult = false;

                FrmLogin frmLogin = new FrmLogin();
                frmLogin.ShowDialog();

                if (PubHelper.p_CheckLoginResult)
                {
                    // 登陆验证成功，退出系统
                    this.Close();
                }
            }
        }

        #endregion

        #region 窗体控件操作

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            lblInitProgress.Content = "Initializing...";
            lblInitProgress.Visibility = System.Windows.Visibility.Visible;
            lblErr_Close.Visibility = System.Windows.Visibility.Visible;
            DispatcherHelper.SleepControl();

            #region 初始化业务组件

            int intErrCode = PubHelper.p_BusinOper.InitPlug();
            if (intErrCode != 0)
            {
                lblInitProgress.Content = "System Error";
                DispatcherHelper.SleepControl();
                return;
            }

            #endregion

            // 判断是否需要隐藏鼠标
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("HideCuror") == "1")
            {
                // 如果需要隐藏鼠标，则隐藏
                Mouse.OverrideCursor = Cursors.None;
            }

            InitControlPostion();

            #region 加载系统皮肤样式

            panelBottom_Area_Money_1.Width = panel_Area_Lgs_Bottom.Width = 
                panel_Area_Lgs_Top.Width = panelBottom_Area_Server.Width =
                panel_Area_Lgs_ShZyZ.Width = this.ActualWidth;
            stpBottom.Width = this.ActualWidth;
            imgChoiceVend_Up_Bottom.Margin = new Thickness(this.ActualWidth / 2 - 60, 0, 0, 0);

            LoadSkin();

            ////ControlMainChoiceUpDown();

            #endregion

            #region 加载系统语言资源

            bool blnResult = PubHelper.RefreshLanguage();
            if (!blnResult)
            {
                // 初始化语言资源类失败
                lblInitProgress.Content = "Load language fail,System will exit,Please check";
                DispatcherHelper.SleepControl();
                return;
            }

            #endregion

            SetServerBtnText();

            //////PubHelper.p_BusinOper.TotalPayMoney = 100000;

            #region 启动工作主线程

            lblInitProgress.Content = PubHelper.p_LangOper.GetStringBundle("Init_Loading_Device");
            DispatcherHelper.SleepControl();

            Thread TrdMoneyBusiness = new Thread(new ThreadStart(MoneyBusinessTrd));
            TrdMoneyBusiness.IsBackground = true;
            TrdMoneyBusiness.Start();

            #endregion

            #region 启动超时监控

            Thread TrdMonOutTime = new Thread(new ThreadStart(MonOutTimeTrd));
            TrdMonOutTime.IsBackground = true;
            TrdMonOutTime.Start();

            #endregion

            #region 启动终端参数更新监控线程

            Thread TrdVmConfigMon = new Thread(new ThreadStart(VmConfigMonTrd));
            TrdVmConfigMon.IsBackground = true;
            TrdVmConfigMon.Start();

            #endregion

            #region 启动语音识别串口监控线程

            Thread TrdVoiceRecognition = new Thread(new ThreadStart(VoiceRecognitionTrd));
            TrdVoiceRecognition.IsBackground = true;
            TrdVoiceRecognition.Start();
            m_VoiceRecognitionTrdSus = false;

            #endregion
        }

        /// <summary>
        /// 初始化主界面各控件显示位置
        /// </summary>
        private void InitControlPostion()
        {
            double dblScreenWidth = this.ActualWidth;
            panelHeader.Width = bgImage.ActualWidth;

            /* 主界面区域划分从下到上如下
             * 1、运营商信息显示区域（只针对50寸大屏）
             * 2、取货码等服务显示区域
             * 3、现金余额显示、退纸、分页显示区域
             * 4、商品列表显示区域
             * 5、状态栏显示区域
            */
            #region 计算运营商信息显示区域（顶部区域和底部区域）

            int intLgsAreaHeight_Bottom = 0;// 运营商信息显示区域底部高度，默认为0
            int intLgsAreaHeight_Top = 0;// 运营商信息显示区域顶部高度，默认为0
            if (PubHelper.p_BusinOper.ConfigInfo.IsShowMainLgsBottom == BusinessEnum.ControlSwitch.Run)
            {
                // 获取显示区域高度
                intLgsAreaHeight_Bottom = Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("MainLgsBottom_Height"));
                ////switch (PubHelper.p_BusinOper.ConfigInfo.ScreenType)
                ////{
                ////    case BusinessEnum.ScreenType.ScreenType50:// 如果为50寸屏，则显示该区域，且该区域高度为400 
                ////        intLgsAreaHeight_Bottom = 400;
                ////        break;
                ////    default:// 如果为其它小尺寸屏，则显示该区域，且该区域高度为200
                ////        intLgsAreaHeight_Bottom = 200;
                ////        break;
                ////}
                m_IsShowLgsArea_Bottom = true;
            }
            if (PubHelper.p_BusinOper.ConfigInfo.IsShowMainLgsTop == BusinessEnum.ControlSwitch.Run)
            {
                // 获取显示区域高度
                intLgsAreaHeight_Top = Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("MainLgsTop_Height"));
                m_IsShowLgsArea_Top = true;

                // 处理顶部显示区域的各控件元素显示
                switch (PubHelper.p_BusinOper.ConfigInfo.MainLgsTop_Type)
                {
                    case BusinessEnum.Main_Lgs_TopType.Normal:// 普通客户样式
                        panel_Area_Lgs_ShZyZ.Visibility = System.Windows.Visibility.Hidden;
                        break;
                    case BusinessEnum.Main_Lgs_TopType.ShangHai_ZhiYuanZhe:// 上海志愿者协会样式
                        panel_Area_Lgs_ShZyZ.Visibility = System.Windows.Visibility.Visible;
                        break;
                }
            }
            
            #endregion

            #region 计算取货码等服务显示区域

            int intServerAreaHeight = 0;// 取货码等服务显示区域高度，默认0
            bool blnIsShowServerArea = false;
            if (PubHelper.p_BusinOper.ConfigInfo.MainLgsTop_Type == BusinessEnum.Main_Lgs_TopType.Normal)
            {
                if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType) ||
                        (PubHelper.p_BusinOper.ConfigInfo.IDCardFreeTake_Switch == BusinessEnum.ControlSwitch.Run) ||
                        (PubHelper.p_BusinOper.ConfigInfo.O2OTake_Switch == BusinessEnum.ControlSwitch.Run) ||
                        (PubHelper.p_BusinOper.ConfigInfo.WxTake_Switch == BusinessEnum.ControlSwitch.Run))
                {
                    // 如果按商品分类显示，或者免费领取、线下取货、微信关注取货等服务开启，则取货码等服务显示区域显示
                    blnIsShowServerArea = true;
                }
            }
            else
            {
                if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IsShowMainServerArea") == "1")
                {
                    blnIsShowServerArea = true;
                }
            }
            if (blnIsShowServerArea)
            {
                m_IsShowServerArea = true;
                intServerAreaHeight = 100;// 26寸屏幕该区域高度为100
                if (PubHelper.p_BusinOper.ConfigInfo.ScreenType == BusinessEnum.ScreenType.ScreenType50)
                {
                    intServerAreaHeight = 200;// 50寸屏幕该区域高度为200
                }
            }

            #endregion

            #region 计算现金处理及分页区域

            m_IsShowMoneyArea = true;
            int intMoneyAreaHeight = 60;// 该区域高度为60

            #endregion

            #region 整体进行计算

            panel_Area_Lgs_Top.Height = intLgsAreaHeight_Top;
            panel_Area_Lgs_Top.Margin = new Thickness(0, 60,0,0);
            panel_Area_Lgs_ShZyZ.Height = intLgsAreaHeight_Top;

            panel_Area_Lgs_Bottom.Height = intLgsAreaHeight_Bottom;
            panel_Area_Lgs_Bottom.Margin = new Thickness(0);

            panelBottom_Area_Server.Height = intServerAreaHeight;
            panelBottom_Area_Server.Margin = new Thickness(0, 0, 0, intLgsAreaHeight_Bottom);

            panelBottom_Area_Money.Height = panelBottom_Area_Money_1.Height = intMoneyAreaHeight;
            panelBottom_Area_Money.Margin = panelBottom_Area_Money_1.Margin = 
                new Thickness(0, 0, 0, intLgsAreaHeight_Bottom + intServerAreaHeight);

            panelProductList.Height = bgImage.ActualHeight - intLgsAreaHeight_Top - intLgsAreaHeight_Bottom - intServerAreaHeight - intMoneyAreaHeight - 60;// 120;
            panelProductList.Margin = new Thickness(0, 70 + intLgsAreaHeight_Top, 0, 70);

            panelGoodsTypeList.Height = panelProductList.Height;
            
            panelGoodsTypeList.Width = bgImage.ActualWidth;
            panelGoodsTypeList.Margin = panelProductList.Margin;

            #endregion

            #region 计算O2O相关服务按钮位置

            int intO2OWidth = 150;// O2O相关服务按钮高度默认100
            int intO2OHeight = 80;
            int intO2OFontSize = 30;
            int intReturnHeight = 80;
            if (PubHelper.p_BusinOper.ConfigInfo.ScreenType == BusinessEnum.ScreenType.ScreenType50)
            {
                intO2OWidth = 200;
                intO2OHeight = 180;
                intReturnHeight = 90;
                intO2OFontSize = 45;
            }
            btnBarCodeTake.Height = btnIDCardTake.Height = btnWxTakeCode.Height = intO2OHeight;
            btnBarCodeTake.Width = btnIDCardTake.Width = btnWxTakeCode.Width = intO2OWidth;

            btnReturn_GoodsType.Height = intServerAreaHeight;
            btnReturn_GoodsType.Width = intO2OWidth;

            #region 计算身份证领、线下取货、微信关注取货等按钮的位置

            btnWxTakeCode.Visibility = btnBarCodeTake.Visibility = btnIDCardTake.Visibility = System.Windows.Visibility.Hidden;
            if (PubHelper.p_BusinOper.ConfigInfo.WxTake_Switch == BusinessEnum.ControlSwitch.Run)
            {
                // 如果微信关注取货开通
                btnWxTakeCode.Margin = new Thickness(0, 10, 10, 10);
                btnWxTakeCode.Visibility = System.Windows.Visibility.Visible;
                if (PubHelper.p_BusinOper.ConfigInfo.O2OTake_Switch == BusinessEnum.ControlSwitch.Run)
                {
                    // 如果线下取货开通
                    btnBarCodeTake.Margin = new Thickness(0, 10, intO2OWidth + 25, 10);
                    btnBarCodeTake.Visibility = System.Windows.Visibility.Visible;
                    if (PubHelper.p_BusinOper.ConfigInfo.IDCardFreeTake_Switch == BusinessEnum.ControlSwitch.Run)
                    {
                        // 如果身份证开通
                        btnIDCardTake.Margin = new Thickness(0, 10, intO2OWidth + intO2OWidth + 40, 10);
                        btnIDCardTake.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    if (PubHelper.p_BusinOper.ConfigInfo.IDCardFreeTake_Switch == BusinessEnum.ControlSwitch.Run)
                    {
                        // 如果身份证开通
                        btnIDCardTake.Margin = new Thickness(0, 10, intO2OWidth + 25, 10);
                        btnIDCardTake.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
            else
            {
                if (PubHelper.p_BusinOper.ConfigInfo.O2OTake_Switch == BusinessEnum.ControlSwitch.Run)
                {
                    // 如果线下取货开通
                    btnBarCodeTake.Margin = new Thickness(0, 10, 10, 10);
                    btnBarCodeTake.Visibility = System.Windows.Visibility.Visible;
                    if (PubHelper.p_BusinOper.ConfigInfo.IDCardFreeTake_Switch == BusinessEnum.ControlSwitch.Run)
                    {
                        // 如果身份证开通
                        btnIDCardTake.Margin = new Thickness(0, 10, intO2OWidth + 25, 10);
                        btnIDCardTake.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    if (PubHelper.p_BusinOper.ConfigInfo.IDCardFreeTake_Switch == BusinessEnum.ControlSwitch.Run)
                    {
                        btnIDCardTake.Margin = new Thickness(0, 10, 10, 10);
                        btnIDCardTake.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }

            #endregion

            ////btnBarCodeTake.Margin = new Thickness(0, 10, 10, 10);
            ////btnIDCardTake.Margin = new Thickness(0, 10, intO2OWidth + 25, 10);
            btnReturn_GoodsType.Margin = new Thickness(0, 0, bgImage.ActualWidth - intO2OWidth + 2, 0);

            btnIDCardTake.FontSize = btnBarCodeTake.FontSize = btnReturn_GoodsType.FontSize =
                btnWxTakeCode.FontSize = intO2OFontSize;

            btnReturn_GoodsType.Visibility = System.Windows.Visibility.Hidden;

            #endregion

            #region 加载相关区域图片

            string strPicPath = string.Empty;
            bool result = PubHelper.GetFormPubPic("Main_Area_Lgs_Bottom.png", out strPicPath);
            if (result)
            {
                imgArea_Lgs_Bottom.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }
            result = PubHelper.GetFormPubPic("Main_Bottom_Area_Server.png", out strPicPath);
            if (result)
            {
                imgBottom_Area_Server.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }
            result = PubHelper.GetFormPubPic("Main_Area_Lgs_Top.png", out strPicPath);
            if (result)
            {
                img_Area_Lgs_Top.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }

            if (m_IsShowLgsArea_Top)
            {
                #region 加载上海志愿者相关图片及相关位置

                if (PubHelper.p_BusinOper.ConfigInfo.MainLgsTop_Type == BusinessEnum.Main_Lgs_TopType.ShangHai_ZhiYuanZhe)
                {
                    result = PubHelper.GetFormPubPic("ShZyZ_logo_shanghaizhiyuanzhe.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Logo_ZhiYuanZhe.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }
                    result = PubHelper.GetFormPubPic("ShZyZ_Qr_ZhiYuanZhe.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Qr_ZhiYuanZhe.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }

                    result = PubHelper.GetFormPubPic("ShZyZ_logo_shanghaizhiyuanzhe_jijinhui.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Logo_JiJinHui.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }
                    result = PubHelper.GetFormPubPic("ShZyZ_Qr_JiJinHui.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Qr_JiJinHui.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }

                    result = PubHelper.GetFormPubPic("ShZyZ_Donate.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Donate.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }
                    result = PubHelper.GetFormPubPic("ShZyZ_Content.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Content.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }
                    result = PubHelper.GetFormPubPic("ShZyZ_Query.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Query.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }
                    result = PubHelper.GetFormPubPic("ShZyZ_Reg.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Reg.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }
                    result = PubHelper.GetFormPubPic("ShZyZ_DuiHuan.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_DuiHuan.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }

                    result = PubHelper.GetFormPubPic("ShZyZ_Donate.png", out strPicPath);
                    if (result)
                    {
                        imgShZy_Donate.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                    }

                    int intLogoPicWidth = 210;

                    imgShZy_Logo_ZhiYuanZhe.Width = imgShZy_Logo_JiJinHui.Width = intLogoPicWidth;
                    imgShZy_Logo_ZhiYuanZhe.Height = imgShZy_Logo_JiJinHui.Height = 169;
                    double dblLogoLeftMin = (dblScreenWidth - intLogoPicWidth * 2) / 2 - 4;// 170;
                    int intLogoTopMin = intLgsAreaHeight_Top / 10;
                    imgShZy_Logo_ZhiYuanZhe.Margin = new Thickness(dblLogoLeftMin, intLogoTopMin, 10, 0);
                    imgShZy_Qr_ZhiYuanZhe.Margin = new Thickness(dblLogoLeftMin + (intLogoPicWidth - 90) / 2, intLogoTopMin + 179, 10, 0);
                    imgShZy_Logo_Line.Margin = new Thickness(dblLogoLeftMin + intLogoPicWidth + 5, intLogoTopMin + 20, 10, 0);
                    imgShZy_Logo_JiJinHui.Margin = new Thickness(dblLogoLeftMin + intLogoPicWidth + 8, intLogoTopMin, 10, 0);
                    imgShZy_Qr_JiJinHui.Margin = new Thickness(dblLogoLeftMin + intLogoPicWidth + 8 + (intLogoPicWidth - 90) / 2, intLogoTopMin + 179, 10, 0);

                    double intre = imgShZy_Logo_ZhiYuanZhe.Width;
                    int fd = 0;
                }

                #endregion
            }

            #endregion
            ////panelProductList.Height = bgImage.ActualHeight - 200 - 100;// 120;
        }

        private void Init(bool isInit)
        {
            music = new MusicManager(this.layout);

            bgDesign.Visibility = System.Windows.Visibility.Collapsed;

            if (isInit)
            {
                ControlMainChoiceUpDown();
                SetMainInfo(true);

                panelBuyProduct.Width = bgImage.Source.Width;
                panelBuyProduct.Height = bgImage.Source.Height;

                CreateGridRow();// 创建货道或者商品显示
            }

            ////////////////BeginMainStory();
        }

        /// <summary>
        /// 设置增值服务按钮名称
        /// </summary>
        private void SetServerBtnText()
        {
            btnIDCardTake.Content = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IDCardFreeTake_Name");// 身份证免费领按钮名称
            btnBarCodeTake.Content = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("O2OTake_Name");// 线下订单取货按钮名称
            btnWxTakeCode.Content = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_Name");// 微信关注有礼按钮名称
        }

        #region 货道或商品或商品类型图形处理

        private void CreateGridRow()
        {
            //MessageBox.Show("Opening port3");
            m_VoiceRecognitionTrdSus = false;
            //if(!vrt_IsConnected) initiateVoiceRecognitionPort();

            int intColumnCount = 0;
            int intRowCount = 0;

            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                #region 如果是商品和货道一一对应

                #region 获取当前货柜的货道相关信息

                PubHelper.p_BusinOper.AsileOper.LoadAsileInfo_VendBox();

                #endregion

                intColumnCount = PubHelper.p_BusinOper.AsileOper.ColumnCount_Current;
                intRowCount = PubHelper.p_BusinOper.AsileOper.RowCount_Current;

                panelProductList.Children.Clear();

                panelProductList.RowDefinitions.Clear();
                panelProductList.ColumnDefinitions.Clear();

                AutoBulidPage.CreateRow(this.panelProductList, intRowCount);
                AutoBulidPage.CreateColumn(this.panelProductList, intColumnCount);

                bgImage.SetValue(Grid.ColumnSpanProperty, intColumnCount + 1);
                bgImage.SetValue(Grid.RowSpanProperty, intRowCount + 1);

                CreateAsile_Asile();

                BeginMainStory();

                #endregion
            }

            if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile) ||
                ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType) &&
                (PubHelper.p_IsClickGoodsType)))
            {
                #region 如果是商品显示模式或者如果是商品类型显示模式，且点击了某个商品类型

                intColumnCount = PubHelper.p_BusinOper.ConfigInfo.EachRowMaxColuNum;
                intRowCount = PubHelper.p_BusinOper.ConfigInfo.EachPageMaxRowNum;

                AutoBulidPage.CreateRow(this.panelProductList, intRowCount - 1);
                AutoBulidPage.CreateColumn(this.panelProductList, intColumnCount - 1);

                bgImage.SetValue(Grid.ColumnSpanProperty, intColumnCount);
                bgImage.SetValue(Grid.RowSpanProperty, intRowCount);

                CreateAsile_Goods();

                BeginMainStory();

                #endregion
            }

            if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType) &&
                (!PubHelper.p_IsClickGoodsType))
            {
                #region 如果是商品类型显示模式且现在没有点击某个商品类型

                CreateAsile_GoodsType();

                IsShowDetailProduct(false);

                #endregion
            }
        }

        /// <summary>
        /// 创建商品类型图形 2015-08-12
        /// </summary>
        private void CreateAsile_GoodsType()
        {
            //MessageBox.Show("Opening Port2");
            m_VoiceRecognitionTrdSus = false;
            //if (!vrt_IsConnected) initiateVoiceRecognitionPort();

            int index = 0;
            panelGoodsTypeList.Children.Clear();
            string strGoodsTypeUnit = PubHelper.p_LangOper.GetStringBundle("SellGoods_GoodsTypeUnit");
            int intGoodsNum = 0;
            string strIsShowGoodsTypeName = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IsShowGoodsTypeName");// 是否显示商品类型名称
            string strGoodsTypeName = string.Empty;

            if (PubHelper.p_BusinOper.GoodsOper.GoodsTypeList.Count > 0)
            {
                #region 创建商品类型图形

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        index++;
                        GoodsTypeControl productControl = new GoodsTypeControl()
                        {
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                            Margin = new Thickness(5)
                        };
                        Grid.SetRow(productControl, i);
                        Grid.SetColumn(productControl, j);
                        productControl.MouseLeftButtonUp += (GoodsTypeClick);
                        productControl.SetSource(PubHelper.p_BusinOper.GoodsOper.GoodsTypeList[index - 1]);

                        intGoodsNum = PubHelper.p_BusinOper.GoodsOper.GoodsTypeList[index - 1].GoodsCount;
                        if (intGoodsNum == 0)
                        {
                            productControl.IsEnabled = false;
                            productControl.SetBgPic_Disable();
                        }
                        else
                        {
                            productControl.IsEnabled = true;
                            productControl.SetBgPic_Normal();
                        }

                        if (strIsShowGoodsTypeName == "0")
                        {
                            strGoodsTypeName = string.Empty;
                        }
                        else
                        {
                            strGoodsTypeName = PubHelper.p_BusinOper.GoodsOper.GoodsTypeList[index - 1].TypeName;
                        }

                        productControl.SetContent_One(strGoodsTypeName,
                            PubHelper.p_BusinOper.ConfigInfo.ScreenType);
                        productControl.SetColor_One(Colors.Red);

                        productControl.SetContent_Second(strGoodsTypeUnit.Replace("{N}", intGoodsNum.ToString()),
                            PubHelper.p_BusinOper.ConfigInfo.ScreenType);

                        panelGoodsTypeList.Children.Add(productControl);
                    }
                }

                #endregion
            }

            if (PubHelper.p_IsClickGoodsType)
            {
                // 如果点击了商品类别，现在在商品列表显示界面
                panelGoodsTypeList.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (panelGoodsTypeList.Visibility == System.Windows.Visibility.Hidden) 
            {
                panelGoodsTypeList.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// 创建货道图形—商品和货道一一对应
        /// </summary>
        private void CreateAsile_Asile()
        {
            //////panelProducts.Children.Clear();
            controlMap.Clear();

            foreach (var productItem in PubHelper.p_BusinOper.AsileOper.AsileList_Current)
            {
                ProductItemControl productControl = new ProductItemControl();

                this.panelProductList.Children.Add(productControl);

                productControl.SetSource(productItem);

                productControl.SetPosition(panelProductList);

                controlMap.Add(productItem, productControl);

                productControl.MouseLeftButtonUp += new MouseButtonEventHandler(ViewProductClick);
            }
        }

        /// <summary>
        /// 创建商品图形—商品不对应货道
        /// </summary>
        private void CreateAsile_Goods()
        {
            //MessageBox.Show("Opening port4");
            //initiateVoiceRecognitionPort();

            int intCurrentPageNo = PubHelper.p_BusinOper.GoodsOper.CurrentPageNo;
            int intNum = PubHelper.p_BusinOper.ConfigInfo.EachPageMaxRowNum * PubHelper.p_BusinOper.ConfigInfo.EachRowMaxColuNum;
            int startIndex = (intCurrentPageNo - 1) * intNum;

            int endIndex = intCurrentPageNo * intNum;

            List<GoodsModel> currentSource = new List<GoodsModel>();

            if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType) &&
                    (PubHelper.p_IsClickGoodsType))
            {
                #region 如果是点击了商品类型

                List<GoodsModel> choiceGoodsList = new List<GoodsModel>();
                for (int index = 0; index < PubHelper.p_BusinOper.GoodsOper.GoodsList_Show.Count; index++)
                {
                    if (PubHelper.p_BusinOper.GoodsOper.GoodsList_Show[index].TypeCode == PubHelper.p_BusinOper.GoodsOper.CurrentGoodsType.TypeCode)
                    {
                        choiceGoodsList.Add(PubHelper.p_BusinOper.GoodsOper.GoodsList_Show[index]);
                    }
                }

                for (int index = 0; index < choiceGoodsList.Count; index++)
                {
                    if (index >= startIndex && index < endIndex)
                    {
                        currentSource.Add(choiceGoodsList[index]);
                    }
                }

                #endregion
            }
            else
            {
                #region 如果是商品图形显示

                for (int index = 0; index < PubHelper.p_BusinOper.GoodsOper.GoodsList_Show.Count; index++)
                {
                    if (index >= startIndex && index < endIndex)
                    {
                        currentSource.Add(PubHelper.p_BusinOper.GoodsOper.GoodsList_Show[index]);
                    }
                }

                #endregion
            }

            PubHelper.p_ShowGoodsCount = currentSource.Count;
            if (currentSource.Count == 0)
            {
                return;
            }

            #region 创建商品图形控件

            panelProductList.Children.Clear();
            controlMap_Goods.Clear();

            int indexProduct = 0;

            GoodsModel productItem = new GoodsModel();
            GoodsItemControl productControl = new GoodsItemControl();
            for (int rowIndex = 0; rowIndex < PubHelper.p_BusinOper.ConfigInfo.EachPageMaxRowNum; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < PubHelper.p_BusinOper.ConfigInfo.EachRowMaxColuNum; columnIndex++)
                {
                    if (indexProduct < currentSource.Count)
                    {
                        productControl = new GoodsItemControl();

                        this.panelProductList.Children.Add(productControl);

                        productItem = currentSource[indexProduct];
                        productItem.RowIndex = rowIndex;
                        productItem.ColumnIndex = columnIndex;

                        productControl.SetPosition(panelProductList);

                        productControl.SetSource(productItem, rowIndex, columnIndex);

                        controlMap_Goods.Add(productItem, productControl);

                        productControl.MouseLeftButtonUp += new MouseButtonEventHandler(ViewProductClick);
                        
                    }
                    else
                    {
                        return;
                    }
                    indexProduct++;
                }
            }

            ////DispatcherHelper.SleepControl();

            #endregion
        }

        /// <summary>
        /// 刷新商品显示列表信息—显示列表
        /// </summary>
        /// <param name="isAll">刷新类型 True：全部刷新 False：不全部刷新，只刷新某个货道或某个商品</param>
        /// <param name="paCode">货道编码或商品编码</param>
        private void RefreshGoodsShowList(bool isAll)
        {
            //MessageBox.Show("Opening Port1");
            //initiateVoiceRecognitionPort(vrt_PortName, vrt_BaudRate);
            m_VoiceRecognitionTrdSus = false;

            string strCode = string.Empty;
            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                #region 如果是商品和货道一一对应显示模式
                for (int i = 0; i < panelProductList.Children.Count; i++)
                {
                    ProductItemControl productControl = (ProductItemControl)panelProductList.Children[i];
                    if ((productControl.CurrentProductItem.PaCode == m_CurrentChoiceCode) && (!isAll))
                    {
                        productControl.RefreshAsileInfo(productControl.CurrentProductItem);
                        break;
                    }
                    else
                    {
                        productControl.RefreshAsileInfo(productControl.CurrentProductItem);
                    }
                }
                #endregion
            }
            if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile) ||
                (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType) &&
                (PubHelper.p_IsClickGoodsType))
            {
                #region 如果是按商品显示模式

                if (!isAll)
                {
                    // 只刷新某个货道的商品
                    for (int i = 0; i < panelProductList.Children.Count; i++)
                    {
                        GoodsItemControl productControl = (GoodsItemControl)panelProductList.Children[i];
                        if (productControl.CurrentGoodsItem.McdCode == m_CurrentChoiceCode)
                        {
                            productControl.RefreshGoodsInfo(productControl.CurrentGoodsItem);
                            break;
                        }
                    }
                }
                else
                {
                    // 全部刷新
                    PubHelper.p_BusinOper.GoodsOper.CurrentPageNo = 1;
                    // 重新获取商品信息
                    bool result = PubHelper.p_BusinOper.GoodsOper.LoadGoodsInfo_Show(PubHelper.p_BusinOper.ConfigInfo.EachPageMaxRowNum,
                        PubHelper.p_BusinOper.ConfigInfo.EachRowMaxColuNum);
                    // 重新创建商品列表
                    CreateAsile_Goods();
                    BeginMainStory();

                    ControlMainChoiceUpDown();
                }
                
                #endregion
            }
        }

        #endregion

        /// <summary>
        /// 加载皮肤样式
        /// </summary>
        private void LoadSkin()
        {
            string strSkinStyle = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("SkinStyle");
            SkinHelper.GetSkinStyle(strSkinStyle);
            bgImage.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "bg_main.png", UriKind.RelativeOrAbsolute));
            //bgImage.Source = new BitmapImage(new Uri("Images/gifTest.gif", UriKind.RelativeOrAbsolute));
            imgQRCode.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/buy-normal.png", UriKind.RelativeOrAbsolute));
            imgMainTop.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/top.png", UriKind.RelativeOrAbsolute));
            btnConfig.Path = SkinHelper.p_SkinName + "ProductPage/ProductList/config.png";
            btnDoor.Path = SkinHelper.p_SkinName + "ProductPage/ProductList/door_open.png";
            btnDevicestatus.Path = SkinHelper.p_SkinName + "ProductPage/ProductList/devicestatus.png";

            imgBottom_Area_Money.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/bottom.png", UriKind.RelativeOrAbsolute));

            ////imgArea_Lgs_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/bottom-area-lgs.png", UriKind.RelativeOrAbsolute));
            ////imgBottom_Area_Server.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/bottom-area-server.png", UriKind.RelativeOrAbsolute));

            imgPrice.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/price.png", UriKind.RelativeOrAbsolute));
            buyBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/buy-normal.png", UriKind.RelativeOrAbsolute));
            exitBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/exit-normal.png", UriKind.RelativeOrAbsolute));
            returnCoinBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/returncoin-normal.png", UriKind.RelativeOrAbsolute));

            imgMsgTip.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/msgtip.png", UriKind.RelativeOrAbsolute));
            imgMainMsgTip.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/msgtip.png", UriKind.RelativeOrAbsolute));
            imgGoodsNameBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/goodsname-bg.png", UriKind.RelativeOrAbsolute));
            imgCardUserBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/cardinfo-bg.png", UriKind.RelativeOrAbsolute));

            if ((PubHelper.p_BusinOper.AsileOper.VendBoxList.Count == 1) &&
                (PubHelper.p_BusinOper.TotalPayMoney > 0))
            {
                ChangeReturnCoinBottomPic(true);
            }

            ChangeKeyBoard_AsileCodeBottomPic(true);

            switch (strSkinStyle)
            {
                case "0":// 蓝天白云样式
                    ptGoodsContentDetailLine.Stroke = new SolidColorBrush(Colors.Red);
                    tbGoodsContentDetail_Title.Foreground = new SolidColorBrush(Colors.Black);
                    tbGoodsContentDetail_Value.Foreground = new SolidColorBrush(Colors.Black);
                    tbDrup_RedOTC.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case "1":// 星空样式
                    ptGoodsContentDetailLine.Stroke = new SolidColorBrush(Colors.White);
                    tbGoodsContentDetail_Title.Foreground = new SolidColorBrush(Colors.Yellow);
                    tbGoodsContentDetail_Value.Foreground = new SolidColorBrush(Colors.White);
                    tbDrup_RedOTC.Foreground = new SolidColorBrush(Colors.White);
                    break;
                default:
                    ptGoodsContentDetailLine.Stroke = new SolidColorBrush(Colors.White);
                    tbGoodsContentDetail_Title.Foreground = new SolidColorBrush(Colors.Yellow);
                    tbGoodsContentDetail_Value.Foreground = new SolidColorBrush(Colors.White);
                    tbDrup_RedOTC.Foreground = new SolidColorBrush(Colors.White);
                    break;
            }
        }

        #region 主界面动画管理

        private void LoadWholePage(string progressTip)
        {
            panelInit.Visibility = System.Windows.Visibility.Visible;
            panelProduct.Visibility = System.Windows.Visibility.Hidden;

            SetBottomAreaShow(false);
            cav.Visibility = System.Windows.Visibility.Hidden;

            DispatcherHelper.SleepControl();
            lblErr_Close.Visibility = System.Windows.Visibility.Hidden;
            lblInitProgress.Visibility = System.Windows.Visibility.Hidden;
            lblInitProgress.Width = SystemParameters.PrimaryScreenWidth;
            lblInitProgress.Margin = new Thickness(0, (SystemParameters.PrimaryScreenHeight / 2) + 100, 0, 0);

            lblInitProgress.Content = progressTip;
            lblInitProgress.Visibility = System.Windows.Visibility.Visible;
            lblErr_Close.Visibility = System.Windows.Visibility.Visible;
            DispatcherHelper.SleepControl();

            tbSetMoneyPanel.Visibility = System.Windows.Visibility.Hidden;

            tbMainInfoPanel.Visibility = System.Windows.Visibility.Hidden;
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        private void BeginMainStory()
        {
            IsShowDetailProduct(false);

            var startTime = 1;

            story = new Storyboard();

            int intColumnCount = 0;

            if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile) ||
                (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType))
            {
                intColumnCount = PubHelper.p_BusinOper.ConfigInfo.EachRowMaxColuNum;
            }
            else
            {
                intColumnCount = PubHelper.p_BusinOper.AsileOper.ColumnCount_Current;
            }

            var width = bgImage.Width / (intColumnCount + 1);

            if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile) ||
                (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType))
            {
                #region 如果是商品显示模式

                int intStorySpace = 50;// 100

                foreach (var key in controlMap_Goods.Keys)
                {
                    var frames = new ThicknessAnimationUsingKeyFrames();

                    Storyboard.SetTargetProperty(frames, new PropertyPath(FrameworkElement.MarginProperty));

                    Storyboard.SetTarget(frames, controlMap_Goods[key]);

                    var span = Convert.ToInt32((key.RowIndex * (intStorySpace + intColumnCount * intStorySpace)) / (intColumnCount / 2));

                    frames.KeyFrames.Add(new EasingThicknessKeyFrame()
                    {
                        KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, startTime, span)),
                        Value = new Thickness(width * ((intColumnCount + 1) - key.ColumnIndex), 0, 0, 0),
                    });

                    frames.KeyFrames.Add(new EasingThicknessKeyFrame()
                    {
                        KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, startTime, 0 + key.ColumnIndex * intStorySpace + span)),
                        Value = new Thickness(0, 0, 0, 0),
                    });

                    frames.Completed += (StoryCompleted);
                    story.Children.Add(frames);
                }

                #endregion
            }
            else
            {
                #region 如果是商品和货道一一对应模式
                foreach (var key in controlMap.Keys)
                {
                    var frames = new ThicknessAnimationUsingKeyFrames();

                    Storyboard.SetTargetProperty(frames, new PropertyPath(FrameworkElement.MarginProperty));

                    Storyboard.SetTarget(frames, controlMap[key]);

                    var span = Convert.ToInt32((key.TrayIndex * (300 + intColumnCount * 200)) / (intColumnCount / 2));

                    frames.KeyFrames.Add(new EasingThicknessKeyFrame()
                    {
                        KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, startTime, span)),
                        Value = new Thickness(width * ((intColumnCount + 1) - key.ColumnIndex), 0, 0, 0),
                    });

                    frames.KeyFrames.Add(new EasingThicknessKeyFrame()
                    {
                        KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, startTime, 300 + key.ColumnIndex * 200 + span)),
                        Value = new Thickness(0, 0, 0, 0),
                    });

                    frames.Completed += (StoryCompleted);
                    story.Children.Add(frames);
                }
                #endregion
            }
            
            story.Begin();

            music.Start(80);// 100
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        private void StopMainStory()
        {
            story.Stop();

            if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile) ||
                (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType))
            {
                foreach (var key in controlMap_Goods.Keys)
                {
                    controlMap_Goods[key].BeginAnimation(FrameworkElement.MarginProperty, null);
                    controlMap_Goods[key].InitMargin();
                }
            }
            else
            {
                foreach (var key in controlMap.Keys)
                {
                    controlMap[key].BeginAnimation(FrameworkElement.MarginProperty, null);
                    controlMap[key].InitMargin();
                }
            }
        }

        void StoryCompleted(object sender, EventArgs e)
        {
            music.Stop();
        }

        #endregion

        #region 商品类型行为管理

        /// <summary>
        /// 商品类型点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoodsTypeClick(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }

            var goodsType = (sender as GoodsTypeControl);

            if (goodsType != null)
            {
                PubHelper.p_BusinOper.GoodsOper.CurrentGoodsType = goodsType.CurrentGoodsItem;
                PubHelper.p_IsClickGoodsType = true;
                PubHelper.p_BusinOper.GoodsOper.CurrentPageNo = 1;
                bool blnIsShowReturn = true;
                if (!PubHelper.p_ClickGoodsType)
                {
                    // 没有点击过商品类型，创建商品列表表格
                    PubHelper.p_ClickGoodsType = true;
                    CreateGridRow();
                }
                else
                {
                    // 点击过商品类型
                    CreateAsile_Goods();

                    if (PubHelper.p_ShowGoodsCount > 0)
                    {
                        BeginMainStory();
                    }
                    else
                    {
                        blnIsShowReturn = false;
                    }
                }

                if (blnIsShowReturn)
                {
                    btnReturn_GoodsType.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 返回商品类型列表页面
        /// </summary>
        private void ReturnGoodsTypeList()
        {
            PubHelper.p_IsClickGoodsType = false;
            btnReturn_GoodsType.Visibility = System.Windows.Visibility.Hidden;
            panelProductList.Visibility = System.Windows.Visibility.Hidden;
            panelGoodsTypeList.Visibility = System.Windows.Visibility.Visible;            
        }

        #endregion

        #region 查看商品行为管理

        /// <summary>
        /// 查询商品明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ViewProductClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Closing Port");
            m_VoiceRecognitionTrdSus = true;
            //closeVoiceRecognitionPort();

            if (!m_IsClickGoods)
            {
                return;
            }
            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                var control = (sender as ProductItemControl);
                PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo = control.CurrentProductItem;
                PubHelper.p_BusinOper.GoodsShowModelType = BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile;
            }
            else
            {
                var control_Goods = (sender as GoodsItemControl);
                PubHelper.p_BusinOper.GoodsOper.CurrentGoods = control_Goods.CurrentGoodsItem;
                PubHelper.p_BusinOper.GoodsShowModelType = BusinessEnum.GoodsShowModelType.GoodsToMultiAsile;
                //MessageBox.Show("product2: " + control_Goods.CurrentGoodsItem.McdCode);
            }
            ViewProductDetail("",false,false);
        }

        /// <summary>
        /// 根据语音显示商品明细内容然后直接出货
        /// </summary>
        /// <param name="selectCode"></param>
        /// <param name="isInput">是否是输入键盘 False：否 True：是</param>
        void ViewProductDetailByVoice(string selectCode, bool isInput, bool isGift)
        {
            ViewProductDetail(selectCode, isInput, isGift);
            BuyProductClickVoice();
        }

        /// <summary>
        /// 显示商品明细内容
        /// </summary>
        /// <param name="selectCode"></param>
        /// <param name="isInput">是否是输入键盘 False：否 True：是</param>
        void ViewProductDetail(string selectCode,bool isInput,bool isGift)
        {
            //关闭语音识别串口
            m_VoiceRecognitionTrdSus = true;
            //closeVoiceRecognitionPort();

            string strMcdCode = string.Empty;// 商品名称
            string strMcdName = string.Empty;// 商品名称
            string strMcdContent = string.Empty;// 商品介绍
            string strMcdContent_Detail = string.Empty;
            string strUnit = string.Empty;// 商品单位
            string strMcdPic = string.Empty;// 商品图片名称
            string strDrupType = string.Empty;// 商品标示
            bool blnIsFree = true;// 是否免费 False：不免费 True：免费

            string strCurrCode = string.Empty;
            string strVendBoxCode = "1";
            //MessageBox.Show("ModelTYpe: " + PubHelper.p_BusinOper.GoodsShowModelType.ToString());
            if ((PubHelper.p_BusinOper.GoodsShowModelType == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile) || (isGift))
            {
                //MessageBox.Show("Asile");
                if (isInput)
                {
                    for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.AsileList.Count; i++)
                    {
                        if (PubHelper.p_BusinOper.AsileOper.AsileList[i].PaCode == selectCode)
                        {
                            PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo = PubHelper.p_BusinOper.AsileOper.AsileList[i];
                            break;
                        }
                    }
                }

                #region 2016-08-29 新增 对上海志愿者赠送商品处理

                if (isGift)
                {
                    // 获取是哪个商品赠送
                    for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.AsileList.Count; i++)
                    {
                        if ((PubHelper.p_BusinOper.AsileOper.AsileList[i].SellModel == BusinessEnum.AsileSellModel.Gift) &&
                            (PubHelper.p_BusinOper.AsileOper.AsileList[i].SurNum > 0))
                        {
                            PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo = PubHelper.p_BusinOper.AsileOper.AsileList[i];
                            break;
                        }
                    }
                }

                #endregion

                if (PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo == null)
                {
                    return;
                }

                m_CurrentChoiceCode = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.PaCode;
                //MessageBox.Show("Pacode: " + m_CurrentChoiceCode + " " + PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdCode);
                strVendBoxCode = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.VendBoxCode;
                m_PaPrice = Convert.ToInt32(PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.SellPrice);
                strMcdName = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdName;
                ////strMcdContent = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdContent;
                strMcdContent_Detail = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.DetailInfo;
                strUnit = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.Unit;
                strMcdPic = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdPicName;
                strDrupType = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.DrugType;
                if (PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.IsFree == "1")
                {
                    // 免费
                    blnIsFree = true;
                }
            }
            else
            {
                //MessageBox.Show("Goods");
                if (PubHelper.p_BusinOper.GoodsOper.CurrentGoods == null)
                {
                    return;
                }

                m_CurrentChoiceCode = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdCode;
                //MessageBox.Show("McdCode: " + PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdCode);
                strCurrCode = strMcdCode;
                m_PaPrice = Convert.ToInt32(PubHelper.p_BusinOper.GoodsOper.CurrentGoods.Price);
                strMcdName = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdName;
                ////strMcdContent = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdContent;
                strMcdContent_Detail = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.DetailInfo;
                strUnit = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.Unit;
                strMcdPic = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.PicName;
                strDrupType = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.DrugType;
                if (PubHelper.p_BusinOper.GoodsOper.CurrentGoods.IsFree == "1")
                {
                    // 免费
                    blnIsFree = true;
                }
            }

            // 检测商品是否能被点击
            bool result = PubHelper.p_BusinOper.CheckIsClickGoods(strVendBoxCode, m_CurrentChoiceCode, m_PaPrice, isGift);
            if (!result)
            {
                return;
            }

            SetBottomAreaShow(false);

            music.Stop();

            #region 2016-06-05修改

            #region 2014-12-03修改
            tbSetMoneyPanel.Visibility = System.Windows.Visibility.Visible;
            tbSetMoney.Foreground = Brushes.White;
            tbSetMoney.Text = PubHelper.p_LangOper.GetStringBundle("Check_Device");
            DispatcherHelper.SleepControl();

            #endregion

            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.ChoiceGoods;

            m_BlnQueryPaStatus = false;

            cardUserArea.Visibility = System.Windows.Visibility.Hidden;

            imgExit.Visibility = System.Windows.Visibility.Hidden;
            tbDownTime.Visibility = System.Windows.Visibility.Hidden;
            tbDownTime.Text = string.Empty;

            // 显示购买二维码
            imgQRCode.Visibility = System.Windows.Visibility.Visible;
            if (imgQRCode.Visibility == System.Windows.Visibility.Hidden)
            {
                MessageBox.Show("Qrcode");
                imgQRCode.Visibility = System.Windows.Visibility.Visible;
            }

            tbBuy.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_Button_Bug");
            tbExit.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_Button_Cancel");
            tbReturnCoin.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_ReturnCoin");

            ////m_BlnQueryPaStatus = true;

            productDetail.Source = new BitmapImage(new Uri(PubHelper.GetMcdPic(strMcdPic)));

            #region 商品标示处理
            string drupPic = string.Empty;
            result = PubHelper.ConvertDrupType(strDrupType, out drupPic);
            if (!result)
            {
                // 不显示商品标示图片
                gridBiaoShi.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                imgGoodsBiaoShi.Source = new BitmapImage(new Uri(drupPic));
                gridBiaoShi.Visibility = System.Windows.Visibility.Visible;
            }
            if (strDrupType == "1")
            {
                // 如果商品标示是红色OTC药品，则显示
                tbDrup_RedOTC.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("RedOTCTipInfo");
                gridDrupOTC.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                gridDrupOTC.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion

            returnCoinBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/returncoin-normal.png"));

            if (!blnIsFree)
            {
                string strGoodsPrice = PubHelper.p_LangOper.GetStringBundle("SellGoods_GoodsPrice");
                tbPrice.Text = strGoodsPrice.Replace("{N}",
                    PubHelper.p_BusinOper.MoneyIntToString(m_PaPrice.ToString()));
            }
            else
            {
                tbPrice.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Price_Free");
            }

            tbUnit.Text = string.Empty;// strUnit;

            tbProductName.Text = PubHelper.GetMcdName(m_CurrentChoiceCode, strMcdName, false);
            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                tbProductContent.Text = PubHelper.ConvertGoodsPropText();
            }

            imgPrice.Visibility = System.Windows.Visibility.Visible;

            tbDownTime.Visibility = System.Windows.Visibility.Visible;

            DispatcherHelper.SleepControl();

            if (PubHelper.p_BusinOper.ConfigInfo.IsShowGoodsDetailContent == BusinessEnum.ControlSwitch.Stop)
            {
                ptGoodsContentDetailLine.Visibility = gdGoodsContentDetailArea.Visibility = System.Windows.Visibility.Hidden;
                tbGoodsContentDetail_Value.Text = tbGoodsContentDetail_Title.Text = string.Empty;
            }
            else
            {
                ptGoodsContentDetailLine.Visibility = gdGoodsContentDetailArea.Visibility = System.Windows.Visibility.Visible;
                // 显示商品的详细介绍
                tbGoodsContentDetail_Title.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_GoodsContent");
                tbGoodsContentDetail_Value.FontSize = PubHelper.p_BusinOper.ConfigInfo.GoodsDetailFontSize;
                tbGoodsContentDetail_Value.Text = strMcdContent_Detail;
            }
            ////DispatcherHelper.SleepControl();

            StopMainStory();
            ////DispatcherHelper.SleepControl();

            IsShowDetailProduct(true);
            ////DispatcherHelper.SleepControl();

            (this.Resources["Storyboard1"] as Storyboard).Begin();

            ////DispatcherHelper.SleepControl();
            music.PlayMusic(Buymp3);
            Thread.Sleep(80);

            imgBuy.Visibility = System.Windows.Visibility.Hidden;
            imgReturnCoin.Visibility = System.Windows.Visibility.Hidden;

            ////DispatcherHelper.SleepControl();

            // 检测当前是否允许购买商品
            m_CheckSaleEnvirCode = BusinessEnum.ServerReason.Err_Other;

            m_BlnQueryPaStatus = true;

            #endregion

        }

        /// <summary>
        /// 切换到显示商品明细界面
        /// </summary>
        /// <param name="show"></param>
        private void IsShowDetailProduct(bool show)
        {
            if (show)
            {
                panel_Area_Lgs_Top.Visibility = System.Windows.Visibility.Hidden;
                panelHeader.Visibility = System.Windows.Visibility.Collapsed;
                panelProductList.Visibility = System.Windows.Visibility.Collapsed;
                panelGoodsTypeList.Visibility = System.Windows.Visibility.Collapsed;
                panelBuyProduct.Visibility = System.Windows.Visibility.Visible;

                var ScaleTransform = (((System.Windows.Media.TransformGroup)(cav.RenderTransform)).Children[0] as ScaleTransform);
                ScaleTransform.ScaleY = bgImage.ActualHeight / bgImage.Source.Height;
                ScaleTransform.ScaleX = bgImage.ActualWidth / bgImage.Source.Width;

                var TranslateTransform = (((System.Windows.Media.TransformGroup)(cav.RenderTransform)).Children[1] as TranslateTransform);
                TranslateTransform.X = (layout.ActualWidth - bgImage.ActualWidth) / 2;
            }
            else
            {
                panelHeader.Visibility = System.Windows.Visibility.Visible;
                SetMainInfo(true);

                if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType) &&
                    (!PubHelper.p_IsClickGoodsType))
                {
                    panelGoodsTypeList.Visibility = System.Windows.Visibility.Visible;
                    panelProductList.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    panelGoodsTypeList.Visibility = System.Windows.Visibility.Hidden;
                    panelProductList.Visibility = System.Windows.Visibility.Visible;
                }
                
                ////panelBottom.Visibility = System.Windows.Visibility.Visible;
                panelBuyProduct.Visibility = System.Windows.Visibility.Collapsed;
                lblInitProgress.Visibility = System.Windows.Visibility.Hidden;
                lblErr_Close.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void ShowProductDetailCompleted(object sender, EventArgs e)
        {
            ProductUpDownMove_Play();
        }

        #endregion

        #region 商品图片上下移动的动画管理

        /// <summary>
        /// 启动商品图片上下移动动画
        /// </summary>
        private void ProductUpDownMove_Play()
        {
            (this.Resources["productUpDownMove"] as Storyboard).Begin();
        }

        /// <summary>
        /// 停止商品图片上下移动动画
        /// </summary>
        private void ProductUpDownMove_Stop()
        {
            (this.Resources["productUpDownMove"] as Storyboard).Stop();

        }

        private void Storyboard_Completed_1(object sender, EventArgs e)
        {
            (this.Resources["productUpDownMove"] as Storyboard).Begin();
        }

        #endregion

        #region 退出行为管理

        /// <summary>
        /// 退出商品改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitProductSellClick(object sender, MouseButtonEventArgs e)
        {
            ExitProductSell();
        }

        private void ExitCompleted(object sender, EventArgs e)
        {
            BeginMainStory();
        }

        private void ExitProductSell()
        {
            PubHelper.p_ChoiceGift = false;
            ProductUpDownMove_Stop();

            CheckIsQueryMoney();

            exitBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/exit-normal.png"));

            if (imgReturnCoin.Visibility == System.Windows.Visibility.Visible)
            {
                imgReturnCoin.Visibility = System.Windows.Visibility.Hidden;
            }

            ptGoodsContentDetailLine.Visibility = gdGoodsContentDetailArea.Visibility = System.Windows.Visibility.Hidden;
            gridBiaoShi.Visibility = gridDrupOTC.Visibility = System.Windows.Visibility.Hidden;

            StopMonOutTime();

            music.PlayMusic(Buymp3);
            Thread.Sleep(80);

            (this.Resources["closeStory"] as Storyboard).Begin();

            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.ReturnMain;
            PubHelper.p_IsTrdOper = false;

            ////SetMainInfo(true);

            while (!PubHelper.p_IsTrdOper)
            {
                DispatcherHelper.SleepControl();
            }
        }

        #endregion

        #region 购买行为管理

        private void BuyProductClick(object sender, MouseButtonEventArgs e)
        {
            ProductUpDownMove_Stop();

            buyBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/buy-normal.png"));

            returnCoinBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/returncoin-normal.png"));

            bool blnIsSell = false;
            if (PubHelper.p_BusinOper.ConfigInfo.IsFreeSellNoPay == BusinessEnum.ControlSwitch.Stop)
            {
                if (Convert.ToDouble(PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.SellPrice) <= PubHelper.p_BusinOper.TotalPayMoney)
                {
                    blnIsSell = true;
                }
            }
            else
            {
                blnIsSell = true;
            }

            if (blnIsSell)
            {
                DropGoods(true);
                //Thread.Sleep(1000);
                //m_VoiceRecognitionTrdSus = false;
            }
            else
            {
                SetOperFailControl(PubHelper.p_LangOper.GetStringBundle("SellGoods_NoEnoughMoney"));

                ProductUpDownMove_Play();

                SetPayMsgInfo();
                ////DropGoods(false);
            }
        }

        private void BuyProductClickVoice()
        {
            ProductUpDownMove_Stop();

            buyBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/buy-normal.png"));

            returnCoinBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/returncoin-normal.png"));

            bool blnIsSell = false;
            if (PubHelper.p_BusinOper.ConfigInfo.IsFreeSellNoPay == BusinessEnum.ControlSwitch.Stop)
            {
                if (Convert.ToDouble(PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.SellPrice) <= PubHelper.p_BusinOper.TotalPayMoney)
                {
                    blnIsSell = true;
                }
            }
            else
            {
                blnIsSell = true;
            }

            if (blnIsSell)
            {
                DropGoods(true);
                //Thread.Sleep(1000);
                //m_VoiceRecognitionTrdSus = false;
            }
            else
            {
                SetOperFailControl(PubHelper.p_LangOper.GetStringBundle("SellGoods_NoEnoughMoney"));

                ProductUpDownMove_Play();

                SetPayMsgInfo();
                ////DropGoods(false);
            }
        }

        private void BuySuccessCompleted(object sender, EventArgs e)
        {
            BeginMainStory();
            DownMp3.Tag = 0;
            DownTomp3.Tag = 0;
        }

        private void BuyStoryCurrentTimeInvalidated(object sender, EventArgs e)
        {
            var timeTotal = ((System.Windows.Media.Animation.Clock)(sender)).CurrentTime.Value.TotalMilliseconds;
            if (timeTotal > 1200 && Convert.ToInt32(DownMp3.Tag) != 1)
            {
                music.PlayMusic(DownMp3);
                DownMp3.Tag = 1;
            }
            if (timeTotal > 3000 && Convert.ToInt32(DownTomp3.Tag) != 1)
            {
                music.PlayMusic(DownTomp3);
                DownTomp3.Tag = 1;
            }
        }

        /// <summary>
        /// 购买出货动画
        /// </summary>
        private void DropGoods(bool blnDrop)
        {
            if (blnDrop)
            {
                imgExit.Visibility = System.Windows.Visibility.Hidden;
                imgBuy.Visibility = System.Windows.Visibility.Hidden;
                imgReturnCoin.Visibility = System.Windows.Visibility.Hidden;
                tbDownTime.Visibility = System.Windows.Visibility.Hidden;
                DispatcherHelper.SleepControl();

                m_BlnQueryMoney = false;
                m_BlnIsSellGoods = true;
            }
            else
            {
                (this.Resources["lessMoneyStory"] as Storyboard).Begin();
            }
        }

        #endregion

        #region 商品点击扭动

        private void productDetailClick(object sender, MouseButtonEventArgs e)
        {
            (this.Resources["rollingStory"] as Storyboard).Begin();
        }

        #endregion

        #region 退币行为管理

        /// <summary>
        /// 退币按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnCoinClick(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }

            ReturnCoinOper();
        }

        private void ReturnCoinAction()
        {
            if (panelBuyProduct.Visibility == System.Windows.Visibility.Hidden)
            {
                panelBuyProduct.Visibility = System.Windows.Visibility.Visible;
            }

            if (imgBuy.Visibility == System.Windows.Visibility.Visible)
            {
                imgBuy.Visibility = System.Windows.Visibility.Hidden;
            }
            if (tbSetMoneyPanel.Visibility == System.Windows.Visibility.Hidden)
            {
                tbSetMoneyPanel.Visibility = System.Windows.Visibility.Visible;
            }

            DispatcherHelper.SleepControl();
        }

        #endregion

        #region 购买 退出、退币按钮 点击时切换到焦点图片

        private void BuyMouseDown(object sender, MouseButtonEventArgs e)
        {
            buyBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/buy-focus.png"));
        }

        private void ExitMouseDown(object sender, MouseButtonEventArgs e)
        {
            exitBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/exit-focus.png"));
        }

        private void ReturnCoinMouseDown(object sender, MouseButtonEventArgs e)
        {
            returnCoinBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/returncoin-focus.png"));
        }

        private void ReturnCoinBottomMouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeReturnCoinBottomPic(false);
        }
        private void ChangeReturnCoinBottomPic(bool isNormal)
        {
            if (isNormal)
            {
                returnCoinBg_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/returncoinbottom-normal.png"));
            }
            else
            {
                returnCoinBg_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/returncoinbottom-focus.png"));
            }
        }
        
        #endregion

        #region 选货键盘

        /// <summary>
        /// 选货键盘点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyBoard_AsileCode_Click(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            ChoiceVend_KeyBoard_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/keyboard-normal.png"));
            PubHelper.p_CheckKeyAsileCodeResult = false;
            PubHelper.p_BusinOper.GoodsShowModelType = BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile;
            PubHelper.p_IsOperManager = true;
            FrmKeyBoard_AsileCode frmKeyBoard_AsileCode = new FrmKeyBoard_AsileCode();
            frmKeyBoard_AsileCode.ShowDialog();
            PubHelper.p_IsOperManager = false;
            if (PubHelper.p_CheckKeyAsileCodeResult)
            {
                ViewProductDetail(PubHelper.p_Keyboard_Input_AsileCode,true,false);
            }
        }

        private void KeyBoard_AsileCode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeKeyBoard_AsileCodeBottomPic(false);
        }

        private void ChangeKeyBoard_AsileCodeBottomPic(bool isNormal)
        {
            if (isNormal)
            {
                ChoiceVend_KeyBoard_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/keyboard-normal.png"));
            }
            else
            {
                ChoiceVend_KeyBoard_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/keyboard-focus.png"));
            }
        }

        #endregion

        #region 状态栏图标点击

        /// <summary>
        /// 后台登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfig_Click(object sender, MouseButtonEventArgs e)
        {
            ShowLogin();
        }

        private void ShowLogin()
        {
            if (!m_IsClickGoods)
            {
                return;
            }

            this.Opacity = PubHelper.OPACITY_GRAY;

            PubHelper.p_IsOperManager = true;// 进入管理设置

            PubHelper.p_LoginType = "0";// 正常途径登陆
            PubHelper.p_IsShutDown = false;
            PubHelper.p_IsRefreshAsile = false;
            PubHelper.p_IsRefreshGoodsType = false;
            PubHelper.p_IsRefreshSkin = false;
            PubHelper.p_IsRefreshSerBtnName = false;
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();

            PubHelper.p_IsOperManager = false;// 退出管理设置

            this.Opacity = PubHelper.OPACITY_NORMAL;

            if (PubHelper.p_IsRefreshLanguage)
            {
                PubHelper.p_IsRefreshLanguage = false;
                SetMainInfo(true);
            }

            if (PubHelper.p_IsShutDown)
            {
                this.Close();
            }
            else
            {

                // 需要刷新货道信息
                if (PubHelper.p_IsRefreshAsile)
                {
                    RefreshGoodsShowList(true); 
                }
                // 需要刷新商品类别信息
                if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType) &&
                    (PubHelper.p_IsRefreshGoodsType))
                {
                    CreateAsile_GoodsType();
                }

                // 需要刷新界面皮肤
                if (PubHelper.p_IsRefreshSkin)
                {
                    LoadSkin();
                }

                // 需要刷新增值服务按钮名称
                if (PubHelper.p_IsRefreshSerBtnName)
                {
                    SetServerBtnText();
                }

                m_SellOperOutTime = PubHelper.p_BusinOper.ConfigInfo.SellOperOutTime;
            }
        }

        /// <summary>
        /// 机器诊断（状态查看）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDevicestatus_Click(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }

            PubHelper.p_IsOperManager = true;// 退出管理设置
            FrmVmDiagnose frmVmDiagnose = new FrmVmDiagnose();
            frmVmDiagnose.ShowDialog();
            PubHelper.p_IsOperManager = false;// 退出管理设置
        }

        #endregion

        #region O2O服务图标点击

        /// <summary>
        /// 图标—商品类型返回点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturn_GoodsType_Click(object sender, RoutedEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            ReturnGoodsTypeList();
        }


        /// <summary>
        /// 图标—身份证免费领点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIDCardTake_Click(object sender, RoutedEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            // 检测当前如果还有现金购买，则暂时不能免费领取
            if (PubHelper.p_BusinOper.TotalPayMoney > 0)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("FreeSell_IDCard_ExistMoney"), PubHelper.MsgType.Ok);
                return;
            }

            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.O2OServer;
            FrmFreeTakeIDCard frmFreeTakeIDCard = new FrmFreeTakeIDCard();
            frmFreeTakeIDCard.ShowDialog();
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
        }

        /// <summary>
        /// 图标—线下取货点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBarCodeTake_Click(object sender, RoutedEventArgs e)
        {
            BarCodeTake_Click();
        }

        /// <summary>
        /// 图标—微信关注取货码点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWxTakeCode_Click(object sender, RoutedEventArgs e)
        {
            WxTakeCode_Click();
        }

        /// <summary>
        /// 线下扫码取货
        /// </summary>
        private void BarCodeTake_Click()
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            // 检测当前如果还有现金购买，则暂时不能提货
            if (PubHelper.p_BusinOper.TotalPayMoney > 0)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("TakeSell_BarCode_ExistMoney"), PubHelper.MsgType.Ok);
                return;
            }
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.O2OServer;
            FrmTakeBarCode frmTakeBarCode = new FrmTakeBarCode();
            frmTakeBarCode.ShowDialog();
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
        }

        /// <summary>
        /// 微信取货码取货
        /// </summary>
        private void WxTakeCode_Click()
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            // 检测当前如果还有现金购买，则暂时不能操作
            if (PubHelper.p_BusinOper.TotalPayMoney > 0)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("WxTakeCode_ExistMoney"), PubHelper.MsgType.Ok);
                return;
            }
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.O2OServer;
            FrmWxTakeCode frmWxTakeCode = new FrmWxTakeCode();
            frmWxTakeCode.ShowDialog();
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
        }

        #endregion

        #region 上海志愿者项目图标点击

        /// <summary>
        /// 上海志愿者LOGO点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShZyz_Logo_Click(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }

            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("VisitWeb_Switch_ZyZXieHui") == "0")
            {
                // 停止服务
                return;
            }

            PubHelper.p_IsOperManager = true;// 退出管理设置
            PubHelper.p_Now_WebUrl = BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_XieHui;
            FrmWebBrowser frmWebBrowser = new FrmWebBrowser();
            frmWebBrowser.ShowDialog();
            PubHelper.p_IsOperManager = false;// 退出管理设置
        }

        /// <summary>
        /// 上海志愿者基金会LOGO点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShZyz_JiJin_Logo_Click(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("VisitWeb_Switch_ZyZJiJinHui") == "0")
            {
                // 停止服务
                return;
            }
            PubHelper.p_IsOperManager = true;// 退出管理设置
            PubHelper.p_Now_WebUrl = BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_JiJinHui;
            FrmWebBrowser frmWebBrowser = new FrmWebBrowser();
            frmWebBrowser.ShowDialog();
            PubHelper.p_IsOperManager = false;// 退出管理设置
        }

        /// <summary>
        /// 上海志愿者—关于我们
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShZyz_Content_Click(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_About_Switch") != "1")
            {
                // 停止服务
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_PauseServer"), PubHelper.MsgType.Ok);
                return;
            }

            PubHelper.p_IsOperManager = true;// 退出管理设置
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmShZyZ_Content frmShZyZ_Content = new FrmShZyZ_Content();
            frmShZyZ_Content.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            PubHelper.p_IsOperManager = false;// 退出管理设置
        }

        /// <summary>
        /// 上海志愿者—查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShZyz_Query_Click(object sender, MouseButtonEventArgs e)
        {
            ////PubHelper.p_IsOperManager = true;// 退出管理设置
            ////this.Opacity = PubHelper.OPACITY_GRAY;
            ////FrmShZyZ_Query_Time frmShZyZ_Query_Time = new FrmShZyZ_Query_Time();
            ////frmShZyZ_Query_Time.ShowDialog();
            ////this.Opacity = PubHelper.OPACITY_NORMAL;
            ////PubHelper.p_IsOperManager = false;// 退出管理设置

            if (!m_IsClickGoods)
            {
                return;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_Query_Switch") != "1")
            {
                // 停止服务
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_PauseServer"), PubHelper.MsgType.Ok);
                return;
            }
            PubHelper.p_IsOperManager = true;// 退出管理设置
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmShZyZ_Query frmShZyZ_Query = new FrmShZyZ_Query();
            frmShZyZ_Query.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            ////PubHelper.p_Now_WebUrl = BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_ItemList;
            ////FrmWebBrowser frmWebBrowser = new FrmWebBrowser();
            ////frmWebBrowser.ShowDialog();
            PubHelper.p_IsOperManager = false;// 退出管理设置
        }

        /// <summary>
        /// 上海志愿者—注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShZyz_Reg_Click(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_Reg_Switch") != "1")
            {
                // 停止服务
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_PauseServer"), PubHelper.MsgType.Ok);
                return;
            }
            PubHelper.p_IsOperManager = true;// 退出管理设置
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmShZyZ_Reg frmShZyZ_Reg = new FrmShZyZ_Reg();
            frmShZyZ_Reg.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            PubHelper.p_IsOperManager = false;// 退出管理设置
        }

        /// <summary>
        /// 上海志愿者—兑换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShZyz_DuiHuan_Click(object sender, MouseButtonEventArgs e)
        {
            ////if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_DuiHuan_Switch") != "1")
            ////{
            ////    // 停止服务
            ////    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_PauseServer"), PubHelper.MsgType.Ok);
            ////    return;
            ////}
            ////BarCodeTake_Click();

            if (!m_IsClickGoods)
            {
                return;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Btn_DuiHuan_Switch") != "1")
            {
                // 停止服务
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_PauseServer"), PubHelper.MsgType.Ok);
                return;
            }
            PubHelper.p_IsOperManager = true;// 退出管理设置
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmShZyZ_DuiHuanTip frmShZyZ_DuiHuanTip = new FrmShZyZ_DuiHuanTip();
            frmShZyZ_DuiHuanTip.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            PubHelper.p_IsOperManager = false;// 退出管理设置
        }

        /// <summary>
        /// 上海志愿者爱心捐赠
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShZyz_Donate_Click(object sender, MouseButtonEventArgs e)
        {
            if (!m_IsClickGoods)
            {
                return;
            }
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("JuanZeng_Switch") == "0")
            {
                // 停止服务
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_PauseServer"), PubHelper.MsgType.Ok);
                return;
            }
            PubHelper.p_IsOperManager = true;// 退出管理设置
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.O2OServer;
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmShZyZ_Donate_ChoicePay frmShZyZ_Donate_ChoicePay = new FrmShZyZ_Donate_ChoicePay();
            frmShZyZ_Donate_ChoicePay.ShowDialog();
            ////FrmShZyZ_Gift FrmShZyZ_Gift = new FrmShZyZ_Gift();
            ////FrmShZyZ_Gift.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            PubHelper.p_IsOperManager = false;// 退出管理设置
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
        }

        #endregion

        #region

        /// <summary>
        /// 操作进行时的控件操作
        /// </summary>
        /// <param name="tipInfo">提示语</param>
        private void SetProgressingControl(string tipInfo)
        {
            imgExit.Visibility = System.Windows.Visibility.Hidden;
            if (imgBuy.Visibility == System.Windows.Visibility.Visible)
            {
                imgBuy.Visibility = System.Windows.Visibility.Hidden;
            }
            if (imgReturnCoin.Visibility == System.Windows.Visibility.Visible)
            {
                imgReturnCoin.Visibility = System.Windows.Visibility.Hidden;
            }

            if (imgReturnCoin.Visibility == System.Windows.Visibility.Visible)
            {
                imgReturnCoin.Visibility = System.Windows.Visibility.Hidden;
            }
            tbDownTime.Visibility = System.Windows.Visibility.Hidden;
            if (tbSetMoneyPanel.Visibility == System.Windows.Visibility.Hidden)
            {
                tbSetMoneyPanel.Visibility = System.Windows.Visibility.Visible;
            }

            ptGoodsContentDetailLine.Visibility = gdGoodsContentDetailArea.Visibility = System.Windows.Visibility.Hidden;

            DispatcherHelper.SleepControl();

            tbSetMoney.Text = tipInfo;

            DispatcherHelper.SleepControl();
        }

        private void SetMainInfo(bool isNormal)
        {
            int intTotalMoney = PubHelper.p_BusinOper.TotalPayMoney;

            if (intTotalMoney > 0)
            {
                string strMoneyTitle = PubHelper.p_LangOper.GetStringBundle("SellGoods_PutMoney_Main");
                string strTotalMoney = PubHelper.p_BusinOper.MoneyIntToString(intTotalMoney.ToString());
                if (PubHelper.p_BusinOper.ConfigInfo.ScreenType == BusinessEnum.ScreenType.ScreenType26)
                {
                    tbMoneyTitle.Text = tbTotalMoney.Text = string.Empty;
                }
                else
                {
                    tbMoneyTitle.Text = strMoneyTitle;
                    tbTotalMoney.Text = strTotalMoney;
                }
                
                tbMainInfo.Text = PubHelper.p_LangOper.GetStringBundle("Main_ChoiceGoods");

                SetBottomInfo();
            }
            else
            {
                tbMoneyTitle.Text = tbTotalMoney.Text = string.Empty;

                SetBottomInfo();

                if (isNormal)
                {
                    tbMainInfo.Text = PubHelper.p_LangOper.GetStringBundle("Main_ChoiceGoods");
                }
                else
                {
                    tbMainInfo.Text = PubHelper.p_LangOper.GetStringBundle("Err_PauseServer_Reason_InitDevice");// 设备故障，暂停服务
                }
            }
        }

        /// <summary>
        /// 设置主界面底部控件
        /// </summary>
        private void SetBottomInfo()
        {
            int intTotalMoney = PubHelper.p_BusinOper.TotalPayMoney;

            if (intTotalMoney > 0)
            {
                string strMoneyTitle = PubHelper.p_LangOper.GetStringBundle("SellGoods_PutMoney_Main");
                string strTotalMoney = PubHelper.p_BusinOper.MoneyIntToString(intTotalMoney.ToString());
                if (PubHelper.p_BusinOper.ConfigInfo.ScreenType == BusinessEnum.ScreenType.ScreenType26)
                {
                    tbMoneyTitle_Bottom.Text = strMoneyTitle;
                    tbTotalMoney_Bottom.Text = strTotalMoney;

                    tbReturnCoin_Bottom.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_ReturnCoin");
                    if (imgReturnCoin_Bottom.Visibility == System.Windows.Visibility.Hidden)
                    {
                        imgReturnCoin_Bottom.Visibility = System.Windows.Visibility.Visible;
                    }
                    ChangeReturnCoinBottomPic(true);
                }
                else
                {
                    tbMoneyTitle_Bottom.Text = tbTotalMoney_Bottom.Text = string.Empty;
                }
            }
            else
            {
                tbMoneyTitle_Bottom.Text = tbTotalMoney_Bottom.Text = string.Empty;

                imgReturnCoin_Bottom.Visibility = System.Windows.Visibility.Hidden;
            }

            SetBottomAreaShow(true);

            tbChoiceVend_KeyBoard_Bottom.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_KeyBoard");  
            if (PubHelper.p_BusinOper.ConfigInfo.IsShowChoiceKeyBoard == BusinessEnum.ControlSwitch.Run)
            {
                imgChoiceVend_KeyBoard_Bottom.Visibility = System.Windows.Visibility.Visible;
                ChangeKeyBoard_AsileCodeBottomPic(true);
            }
            else
            {
                imgChoiceVend_KeyBoard_Bottom.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// 控制底部各区域的显示与否
        /// </summary>
        /// <param name="visible"></param>
        private void SetBottomAreaShow(bool visible)
        {
            if (!visible)
            {
                // 隐藏各区域
                this.Dispatcher.Invoke(new Action(() =>
                {
                    panel_Area_Lgs_Top.Visibility = panel_Area_Lgs_Bottom.Visibility = panelBottom_Area_Money.Visibility = panelBottom_Area_Money_1.Visibility =
                    panelBottom_Area_Server.Visibility = System.Windows.Visibility.Hidden;
                }));
                //panel_Area_Lgs_Top.Visibility = panel_Area_Lgs_Bottom.Visibility = panelBottom_Area_Money.Visibility = panelBottom_Area_Money_1.Visibility =
                //    panelBottom_Area_Server.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                // 显示各区域
                if (m_IsShowLgsArea_Bottom)
                {
                    panel_Area_Lgs_Bottom.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    panel_Area_Lgs_Bottom.Visibility = System.Windows.Visibility.Hidden;
                }
                if (m_IsShowLgsArea_Top)
                {
                    panel_Area_Lgs_Top.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    panel_Area_Lgs_Top.Visibility = System.Windows.Visibility.Hidden;
                }

                if (m_IsShowServerArea)
                {
                    panelBottom_Area_Server.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    panelBottom_Area_Server.Visibility = System.Windows.Visibility.Hidden;
                }
                if (m_IsShowMoneyArea)
                {
                    panelBottom_Area_Money.Visibility = panelBottom_Area_Money_1.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    panelBottom_Area_Money.Visibility = panelBottom_Area_Money_1.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        #region 多货柜上翻、下翻处理

        /// <summary>
        /// 控制多个货柜时主界面的上翻、下翻选择按键
        /// </summary>
        private void ControlMainChoiceUpDown()
        {
            if (PubHelper.p_BusinOper.CheckIsPage())
            {
                // 多个货柜或多个页面
                imgChoiceVend_Up_Bottom.IsEnabled = false;
                //////ChoiceVend_Up_Bottom.IsEnabled = false;
                imgChoiceVend_Up_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_left_gray.png", UriKind.RelativeOrAbsolute));

                imgChoiceVend_Down_Bottom.IsEnabled = true;
                imgChoiceVend_Down_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_right_normal.png", UriKind.RelativeOrAbsolute));
                imgChoiceVend_Up_Bottom.Visibility = imgChoiceVend_Down_Bottom.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                // 1个货柜货1个页面
                imgChoiceVend_Up_Bottom.Visibility = imgChoiceVend_Down_Bottom.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// 货柜选择—上翻点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChoiceVendBoxMain_Up_Click(object sender, MouseButtonEventArgs e)
        {
            ControlUpDownForm(false);
            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                SetVendBoxUpDownControl("0");
            }
            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile)
            {
                SetGoodsPageUpDownControl("0");
            }

            ControlUpDownForm(true);
        }

        private void ChoiceVendBoxMain_Up_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }    

        /// <summary>
        /// 货柜选择—下翻点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChoiceVendBoxMain_Down_Click(object sender, MouseButtonEventArgs e)
        {
            ControlUpDownForm(false);
            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                SetVendBoxUpDownControl("1");
            }
            if (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile)
            {
                SetGoodsPageUpDownControl("1");
            }

            ControlUpDownForm(true);
        }

        private void ChoiceVendBoxMain_Down_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void ControlUpDownForm(bool visible)
        {
            if (visible)
            {
                imgChoiceVend_Up_Bottom.Visibility = imgChoiceVend_Down_Bottom.Visibility = System.Windows.Visibility.Visible;
                if (PubHelper.p_BusinOper.ConfigInfo.IsShowChoiceKeyBoard == BusinessEnum.ControlSwitch.Run)
                {
                    imgChoiceVend_KeyBoard_Bottom.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    imgChoiceVend_KeyBoard_Bottom.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                imgChoiceVend_Up_Bottom.Visibility = imgChoiceVend_Down_Bottom.Visibility =
                    imgChoiceVend_KeyBoard_Bottom.Visibility = System.Windows.Visibility.Hidden;
            }
            DispatcherHelper.SleepControl();
        }

        /// <summary>
        /// 设置货柜上翻、下翻控件
        /// </summary>
        /// <param name="type">0：上翻 1：下翻</param>
        private void SetVendBoxUpDownControl(string type)
        {
            if (!m_IsClickGoods)
            {
                return;
            }

            m_IsClickGoods = false;

            InitAdvertPlayTime();

            int intVendBoxCount = PubHelper.p_BusinOper.AsileOper.VendBoxList.Count;
            int intNowVendBox = Convert.ToInt32(PubHelper.p_BusinOper.AsileOper.CurrentVendBox);
            if (type == "0")
            {
                // 上翻
                if (intNowVendBox > 1)
                {
                    intNowVendBox--;
                }
                else
                {
                    m_IsClickGoods = true;
                    return;
                }
            }
            else
            {
                // 下翻
                if (intNowVendBox < intVendBoxCount)
                {
                    intNowVendBox++;
                }
                else
                {
                    m_IsClickGoods = true;
                    return;
                }
            }

            PubHelper.p_BusinOper.AsileOper.CurrentVendBox = intNowVendBox.ToString();
            if (intNowVendBox > 1)
            {
                if (intNowVendBox == intVendBoxCount)
                {
                    // 如果当前的货柜已经最大
                    // 置灰下翻键，且不可用
                    if (!imgChoiceVend_Up_Bottom.IsEnabled)
                    {
                        imgChoiceVend_Up_Bottom.IsEnabled = true;
                        imgChoiceVend_Up_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_left_normal.png", UriKind.RelativeOrAbsolute));
                    }
                    if (imgChoiceVend_Down_Bottom.IsEnabled)
                    {
                        imgChoiceVend_Down_Bottom.IsEnabled = false;
                        imgChoiceVend_Down_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_right_gray.png", UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                    // 如果当前的货柜在中间
                    if (!imgChoiceVend_Up_Bottom.IsEnabled)
                    {
                        imgChoiceVend_Up_Bottom.IsEnabled = true;
                        imgChoiceVend_Up_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_left_normal.png", UriKind.RelativeOrAbsolute));
                    }
                    if (!imgChoiceVend_Down_Bottom.IsEnabled)
                    {
                        imgChoiceVend_Down_Bottom.IsEnabled = true;
                        imgChoiceVend_Down_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_right_normal.png", UriKind.RelativeOrAbsolute));
                    }
                }
            }
            else
            {
                // 置灰上翻键，且不可用
                if (imgChoiceVend_Up_Bottom.IsEnabled)
                {
                    imgChoiceVend_Up_Bottom.IsEnabled = false;
                    imgChoiceVend_Up_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_left_gray.png", UriKind.RelativeOrAbsolute));
                }
                if (!imgChoiceVend_Down_Bottom.IsEnabled)
                {
                    imgChoiceVend_Down_Bottom.IsEnabled = true;
                    imgChoiceVend_Down_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_right_normal.png", UriKind.RelativeOrAbsolute));
                }
            }

            CreateGridRow();

            m_IsClickGoods = true;
        }

        /// <summary>
        /// 设置商品页面上翻、下翻
        /// </summary>
        private void SetGoodsPageUpDownControl(string type)
        {
            if (!m_IsClickGoods)
            {
                return;
            }

            m_IsClickGoods = false;

            InitAdvertPlayTime();

            if (type == "0")
            {
                // 上翻
                PubHelper.p_BusinOper.GoodsOper.CurrentPageNo--;
            }
            else
            {
                // 下翻
                PubHelper.p_BusinOper.GoodsOper.CurrentPageNo++;
            }

            if (PubHelper.p_BusinOper.GoodsOper.CurrentPageNo < 1)
            {
                PubHelper.p_BusinOper.GoodsOper.CurrentPageNo = 1;
            }
            if (PubHelper.p_BusinOper.GoodsOper.CurrentPageNo >
                PubHelper.p_BusinOper.GoodsOper.GoodsShowPageCount)
            {
                PubHelper.p_BusinOper.GoodsOper.CurrentPageNo = PubHelper.p_BusinOper.GoodsOper.GoodsShowPageCount;
            }

            int intGoodsPageNo = PubHelper.p_BusinOper.GoodsOper.CurrentPageNo;
            if (intGoodsPageNo == 1)
            {
                imgChoiceVend_Up_Bottom.IsEnabled = false;
                imgChoiceVend_Down_Bottom.IsEnabled = true;
            }
            else if (intGoodsPageNo == PubHelper.p_BusinOper.GoodsOper.GoodsShowPageCount)
            {
                imgChoiceVend_Up_Bottom.IsEnabled = true;
                imgChoiceVend_Down_Bottom.IsEnabled = false;
            }
            else
            {
                imgChoiceVend_Up_Bottom.IsEnabled = true;
                imgChoiceVend_Down_Bottom.IsEnabled = true;
            }

            if (imgChoiceVend_Up_Bottom.IsEnabled)
            {
                imgChoiceVend_Up_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_left_normal.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                imgChoiceVend_Up_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_left_gray.png", UriKind.RelativeOrAbsolute));
            }

            if (imgChoiceVend_Down_Bottom.IsEnabled)
            {
                imgChoiceVend_Down_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_right_normal.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                imgChoiceVend_Down_Bottom.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/arrow_right_gray.png", UriKind.RelativeOrAbsolute));
            }

            CreateAsile_Goods();

            BeginMainStory();

            m_IsClickGoods = true;
        }

        #endregion

        /// <summary>
        /// 显示购买失败或其他失败的控件操作
        /// </summary>
        /// <param name="failReason"></param>
        private void SetOperFailControl(string failReason)
        {
            if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main) || (m_QueryCashSource == "1"))
            {
                #region 如果是在商品主界面，则暂时禁止点击其它控件

                m_IsClickGoods = false;

                if (tbMainInfoPanel.Visibility == System.Windows.Visibility.Hidden)
                {
                    tbMainInfoPanel.Visibility = System.Windows.Visibility.Visible;
                }
                if (imgReturnCoin_Bottom.Visibility == System.Windows.Visibility.Visible)
                {
                    imgReturnCoin_Bottom.Visibility = System.Windows.Visibility.Hidden;
                }
                
                tbMainSetInfo.Text = failReason;//
                DispatcherHelper.SleepControl();

                (this.Resources["mainErrStory"] as Storyboard).Begin();

                DispatcherHelper.SleepControl(1000);

                tbMainInfoPanel.Visibility = System.Windows.Visibility.Hidden;
                if (PubHelper.p_BusinOper.TotalPayMoney > 0)
                {
                    ChangeReturnCoinBottomPic(true);
                    imgReturnCoin_Bottom.Visibility = System.Windows.Visibility.Visible;
                }
                m_IsClickGoods = true;

                #endregion
            }

            if ((PubHelper.p_BusinOper.OperStep != BusinessEnum.OperStep.Main) &&
                (PubHelper.p_BusinOper.OperStep != BusinessEnum.OperStep.InitErr) &&
                (PubHelper.p_BusinOper.OperStep != BusinessEnum.OperStep.Loading))
            {
                #region 如果是在商品详细界面

                bool blnBuy = false;
                if (imgBuy.Visibility == System.Windows.Visibility.Visible)
                {
                    blnBuy = true;
                }
                bool blnCancel = false;
                if (imgExit.Visibility == System.Windows.Visibility.Visible)
                {
                    blnCancel = true;
                }
                bool blnReturnCoin = false;
                if (imgReturnCoin.Visibility == System.Windows.Visibility.Visible)
                {
                    blnReturnCoin = true;
                }

                // 2014-12-01 添加
                if (tbSetMoneyPanel.Visibility == System.Windows.Visibility.Hidden)
                {
                    tbSetMoneyPanel.Visibility = System.Windows.Visibility.Visible;
                }

                tbSetMoney.Text = failReason;//
                tbSetMoney.Foreground = Brushes.Yellow;
                imgBuy.Visibility = imgExit.Visibility = imgReturnCoin.Visibility = System.Windows.Visibility.Hidden;
                DispatcherHelper.SleepControl();

                DropGoods(false);

                DispatcherHelper.SleepControl(1000);

                if (cardUserArea.Visibility == System.Windows.Visibility.Visible)
                {
                    cardUserArea.Visibility = System.Windows.Visibility.Hidden;
                }

                tbSetMoney.Foreground = Brushes.White;

                if (blnBuy)
                {
                    imgBuy.Visibility = System.Windows.Visibility.Visible;
                }
                if (blnReturnCoin)
                {
                    returnCoinBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductDetail/returncoin-normal.png"));
                    imgReturnCoin.Visibility = System.Windows.Visibility.Visible;
                }
                imgExit.Visibility = System.Windows.Visibility.Visible;

                #endregion
            }
            
            ////if (blnCancel)
            ////{
            ////    imgExit.Visibility = System.Windows.Visibility.Visible;
            ////}
        }

        #region 支付方式按钮点击事件

        /// <summary>
        /// 显示支付方式提示语
        /// </summary>
        private void SetPayMsgInfo()
        {
            bool blnIsShowBuyReturn = false;

            int intTotalPayMoney = PubHelper.p_BusinOper.TotalPayMoney;
            if (intTotalPayMoney <= 0)
            {
                if (PubHelper.p_BusinOper.ConfigInfo.IsFreeSellNoPay == BusinessEnum.ControlSwitch.Stop)
                {
                    tbSetMoney.Text = PubHelper.GetPayMentMsgTitle();
                }
                else
                {
                    blnIsShowBuyReturn = true;
                    tbSetMoney.Text = "请点击购买出货";
                }
            }
            else
            {
                string strMoney = PubHelper.p_BusinOper.MoneyIntToString(intTotalPayMoney.ToString());
                tbSetMoney.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_PutMoney").Replace("{N}", strMoney);// "已投币" + m_TotalMoney.ToString() + "元";
                if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main)
                {
                    // 主界面上存在金额投入
                    SetMainInfo(true);
                }
                //tbMainInfo.Text = tbSetMoney.Text;
                blnIsShowBuyReturn = true;
            }

            if (blnIsShowBuyReturn)
            {
                if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.ChoiceGoods) ||
                    (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Payment))
                {
                    // 显示购买按钮
                    if (imgBuy.Visibility == System.Windows.Visibility.Hidden)
                    {
                        imgBuy.Visibility = System.Windows.Visibility.Visible;
                    }

                    if (PubHelper.p_BusinOper.ConfigInfo.IsFreeSellNoPay == BusinessEnum.ControlSwitch.Stop)
                    {
                        // 显示退币按钮
                        if (imgReturnCoin.Visibility == System.Windows.Visibility.Hidden)
                        {
                            imgReturnCoin.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                }
            }

            DispatcherHelper.SleepControl();
        }

        private void ControlNoCashPayInfo(bool enable,string tipInfo)
        {
            if (enable)
            {
                BeginMonOutTime();
                imgExit.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                StopMonOutTime();
                // 界面更改
                SetProgressingControl(tipInfo);
            }

            DispatcherHelper.SleepControl();
        }

        #endregion

        #endregion

        #endregion

        #region 工作线程

        #region 相关操作超时监控

        /// <summary>
        /// 时间超时监控工作线程
        /// </summary>
        private void MonOutTimeTrd()
        {
            // 获取允许操作超时时间
            m_SellOperOutTime = PubHelper.p_BusinOper.ConfigInfo.SellOperOutTime;

            m_OutTimeMonTrd = false;

            int intTimeNum = 0;

            TimeSpan tsPlayAdvert;
            bool blnIsPlayAdvertEnd = false;

            int intCheckSaleEnvir = 0;

            try
            {
                while (!m_CloseForm)
                {
                    Thread.Sleep(50);

                    #region 超时处理

                    if (m_IsMonTime)
                    {
                        // 如果启动了超时监控
                        m_OutNum++;
                        if (m_OutNum >= 20)
                        {
                            m_OutNum = 0;
                            m_OperNum++;

                            try
                            {
                                if (!m_CloseForm)
                                {
                                    if (m_OperNum > m_SellOperOutTime - 1)
                                    {
                                        // 超时，自动返回
                                        this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                        {
                                            ExitProductSell();
                                        }));
                                    }
                                    else
                                    {
                                        this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                        {
                                            tbDownTime.Text = (m_SellOperOutTime - m_OperNum).ToString();
                                            if (tbDownTime.Visibility == System.Windows.Visibility.Hidden)
                                            {
                                                tbDownTime.Visibility = System.Windows.Visibility.Visible;
                                            }
                                            if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Payment)
                                            {
                                                if (imgExit.Visibility == System.Windows.Visibility.Hidden)
                                                {
                                                    imgExit.Visibility = System.Windows.Visibility.Visible;
                                                }
                                            }
                                        }));
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    #endregion

                    #region 时间显示

                    // 显示时间
                    if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main)
                    {
                        intTimeNum++;
                        if (intTimeNum >= 20)
                        {
                            intTimeNum = 0;
                            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                            {
                                tbTime.Text = DateTime.Now.ToShortTimeString();
                                if (tbTime.Visibility == System.Windows.Visibility.Hidden)
                                {
                                    tbTime.Visibility = System.Windows.Visibility.Visible;
                                }
                            }));

                            PubHelper.DeleteLogDirectory();
                        }
                    }
                    else
                    {
                        intTimeNum = 0;
                    }

                    #endregion

                    #region 广告播放

                    if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main) &&
                        (!PubHelper.p_IsPlayingAdvert) &&
                        (!PubHelper.p_IsOperManager) && 
                        (PubHelper.p_BusinOper.AdvertOper.AdvertList.Count > 0) &&
                        (PubHelper.p_BusinOper.ConfigInfo.AdvertPlaySwitch == BusinessEnum.ControlSwitch.Run))
                    {
                        if (blnIsPlayAdvertEnd)
                        {
                            Thread.Sleep(2000);
                            m_IsClickGoods = true;
                            blnIsPlayAdvertEnd = false;
                        }

                        blnIsPlayAdvertEnd = false;
                        tsPlayAdvert = DateTime.Now - PubHelper.p_AdvertPlayTime;

                        // 如果是在主界面，且广告允许播放，且当前广告没正在播放
                        if (tsPlayAdvert.TotalSeconds > PubHelper.p_BusinOper.ConfigInfo.AdvertPlayOutTime)
                        {
                            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                            {
                                m_IsClickGoods = false;
                                PubHelper.p_AdvertPlayTime = DateTime.Now;
                                PubHelper.p_IsPlayingAdvert = true;
                                FrmPlayAdvert26 frmAdvertPlay26 = new FrmPlayAdvert26();
                                frmAdvertPlay26.ShowDialog();
                                PubHelper.p_AdvertPlayTime = DateTime.Now;
                                PubHelper.p_IsPlayingAdvert = false;
                                
                                blnIsPlayAdvertEnd = true;
                            }));
                        }
                    }
                    else
                    {
                        InitAdvertPlayTime();
                    }

                    #endregion

                    #region 刷卡查询

                    if (!PubHelper.p_IsPlayingAdvert)
                    {
                        if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main) &&
                                (PubHelper.p_BusinOper.PaymentOper.PaymentList.IC.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                                (PubHelper.p_BusinOper.DeviceInfo.ICStatus.Status == "02") &&
                                (PubHelper.p_BusinOper.ConfigInfo.IcQuerySwitch == BusinessEnum.ControlSwitch.Run))
                        {
                            // 如果界面流程为主界面，且刷卡器启用，且刷卡器状态正常，且允许查询卡信息
                            QueryCard();
                        }
                    }

                    #endregion

                    #region 监控工作主线程是否结束或出现异常

                    if ((m_MainMonTrd) && (!m_CloseForm))
                    {
                        m_MainMonTrd = false;
                        // 如果主线程工作结束且当前还没有正常退出系统，则重新启动电脑
                        this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                        {
                            ProcessStartInfo ps = new ProcessStartInfo(); 
                            ps.FileName = "shutdown.exe";
                            ps.Arguments = "-r -t 1"; 
                            Process.Start(ps);
                        }));
                    }
                    else
                    {
                        // 如果检查商品购买环境时，无法正常走下去
                        if (!m_IsCheckSaleEnvir)
                        {
                            intCheckSaleEnvir++;
                            if (intCheckSaleEnvir > 12000)
                            {
                                intCheckSaleEnvir = 0;
                                m_IsCheckSaleEnvir = true;
                                // 如果10分钟之内还没有进入支付阶段，则返回
                                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                {
                                    ExitProductSell();
                                }));
                            }
                        }
                        else
                        {
                            intCheckSaleEnvir = 0;
                        }
                    }

                    #endregion
                }
            }
            catch(Exception ex)
            {
                PubHelper.WriteSystemLog("Monitor Thread,Error " + ex.ToString());

                // 重新启动监控线程
                Thread TrdMonOutTime = new Thread(new ThreadStart(MonOutTimeTrd));
                TrdMonOutTime.IsBackground = true;
                TrdMonOutTime.Start();
            }

            m_OutTimeMonTrd = true;
        }

        /// <summary>
        /// 重新开始超时监控
        /// </summary>
        private void BeginMonOutTime()
        {
            m_IsMonTime = true;
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

        /// <summary>
        /// 初始化广告播放时间
        /// </summary>
        private void InitAdvertPlayTime()
        {
            PubHelper.p_AdvertPlayTime = DateTime.Now;
            PubHelper.p_IsPlayingAdvert = false;
        }

        #endregion

        #region 购买相关业务

        /// <summary>
        /// 系统工作主线程
        /// </summary>
        private void MoneyBusinessTrd()
        {
            m_MainMonTrd = false;

            int intErrCode = 0;

            int intMoney = 0;

            string strTip = string.Empty;

            bool blnIsReturnMoney = false;

            // 查询金额指令发送次数
            int intQueryMoneyNum = 0;

            string strOutValue = string.Empty;
            // 是否需要刷新零钱不足或充足的状态 False：不刷新 True：刷新
            bool blnIsRefreshEnouthCoin = false;

            int intMonitorDeviceNum = 0;// 设备监控次数
            int intInitErrNum = 0;// 初始化失败次数
            int intNetStatusNum = 0;// 网络连接监控次数
            BusinessEnum.NetStatus strNetHistoryStatus = BusinessEnum.NetStatus.NoNet;// 网络连接历史状态

            string strIsTestVer = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IsTestVer");// 是否属于测试版本 0：否 1：是
            bool blnKmbStatusHistory = true;

            int intTotalMoney = 0;

            bool blnPutMoney = false;

            int intQueryMoneNum = 0;// 查询金额间隔次数
            int intMaxQueryMonNum = 0;// 

            try
            {
                while (!m_CloseForm)
                {
                    Thread.Sleep(50);

                    try
                    {
                        #region 自检

                        if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Loading)
                        {
                            CheckDevice(strIsTestVer);
                        }
                        if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.InitErr)
                        {
                            intInitErrNum++;
                            if (intInitErrNum >= 200)
                            {
                                // 每隔10秒
                                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                {
                                    lblInitProgress.Content = PubHelper.p_LangOper.GetStringBundle("Init_Loading_Device");
                                    DispatcherHelper.SleepControl();
                                }));
                                DispatcherHelper.SleepControl(1000);
                                intInitErrNum = 0;
                                CheckDevice(strIsTestVer);
                            }
                        }

                        #endregion

                        intTotalMoney = PubHelper.p_BusinOper.TotalPayMoney;

                        #region 监控网络连接状态

                        if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main)
                        {
                            intNetStatusNum++;
                            if (intNetStatusNum > 100)
                            {
                                // 5秒监控一次
                                intNetStatusNum = 0;
                                PubHelper.p_BusinOper.GetNetStatus();
                                if (strNetHistoryStatus != PubHelper.p_BusinOper.DeviceInfo.NetStatus)
                                {
                                    strNetHistoryStatus = PubHelper.p_BusinOper.DeviceInfo.NetStatus;

                                    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                    {
                                        if (strNetHistoryStatus == BusinessEnum.NetStatus.OnLine)
                                        {
                                            // 联机
                                            btnNetStatus.Path = SkinHelper.p_SkinName + "ProductPage/ProductList/online.png";
                                        }
                                        else
                                        {
                                            // 离线
                                            btnNetStatus.Path = SkinHelper.p_SkinName + "ProductPage/ProductList/offline.png";
                                        }
                                        if (btnNetStatus.Visibility == System.Windows.Visibility.Hidden)
                                        {
                                            btnNetStatus.Visibility = System.Windows.Visibility.Visible;
                                        }
                                    }));
                                }
                            }
                        }
                        

                        #endregion

                        #region 监控设备工作状态

                        // 界面为主界面，且金额为0时进行设备监控
                        if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main) && (intTotalMoney == 0))
                        {
                            intMonitorDeviceNum++;
                            if (intMonitorDeviceNum > 100)
                            {
                                // 5秒查询一次
                                intMonitorDeviceNum = 0;

                                intErrCode = PubHelper.p_BusinOper.Device_Monitor(true);

                                if (intErrCode == 0)
                                {
                                    #region 监控设备工作状态正常

                                    PubHelper.p_SoftWorkStatus = true;
                                    ////if ((strIsTestVer == "0") &&
                                    ////       (blnKmbStatusHistory != PubHelper.p_BusinOper.DeviceInfo.KmbConnectStatus))
                                    if (strIsTestVer == "0")
                                    {
                                        blnKmbStatusHistory = PubHelper.p_BusinOper.DeviceInfo.KmbConnectStatus;
                                        this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                        {
                                            SetMainInfo(blnKmbStatusHistory);
                                        }));
                                    }
                                    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                    {
                                        #region 控制门的图标显示状态

                                        switch (PubHelper.p_BusinOper.AsileOper.VendBoxList[0].DoorStatus)
                                        {
                                            case "00":// 门关
                                                // 隐藏登陆后台系统的按钮
                                                if (btnConfig.Visibility == System.Windows.Visibility.Visible)
                                                {
                                                    btnConfig.Visibility = System.Windows.Visibility.Hidden;
                                                    btnDoor.Visibility = System.Windows.Visibility.Hidden;
                                                }
                                                if (PubHelper.p_BusinOper.ConfigInfo.IsShowVmDiagnose == BusinessEnum.ControlSwitch.Stop)
                                                {
                                                    // 如果门关后不显示机器诊断图标
                                                    if (btnDevicestatus.Visibility == System.Windows.Visibility.Visible)
                                                    {
                                                        btnDevicestatus.Visibility = System.Windows.Visibility.Hidden;
                                                    }
                                                }
                                                else
                                                {
                                                    if (btnDevicestatus.Visibility == System.Windows.Visibility.Hidden)
                                                    {
                                                        btnDevicestatus.Visibility = System.Windows.Visibility.Visible;
                                                    }
                                                }
                                                break;

                                            case "01":// 门开
                                                // 显示登陆后台系统的按钮
                                                if (btnConfig.Visibility == System.Windows.Visibility.Hidden)
                                                {
                                                    btnConfig.Visibility = System.Windows.Visibility.Visible;
                                                    btnDoor.Visibility = System.Windows.Visibility.Visible;
                                                }
                                                if (btnDevicestatus.Visibility == System.Windows.Visibility.Hidden)
                                                {
                                                    btnDevicestatus.Visibility = System.Windows.Visibility.Visible;
                                                }
                                                break;
                                        }

                                        #endregion

                                        #region 控制制冷或加热启动图标状态及温度状态

                                        if (PubHelper.p_BusinOper.AsileOper.VendBoxList.Count > 0)
                                        {
                                            // 显示第一个货柜的情况
                                            switch (PubHelper.p_BusinOper.AsileOper.VendBoxList[0].RefControl.ControlStatus)
                                            {
                                                case BusinessEnum.DeviceControlStatus.Open:// 制冷或加热启动
                                                    if (btnRefStatus.Visibility == System.Windows.Visibility.Hidden)
                                                    {
                                                        // 根据制冷或加热，显示不同的图标
                                                        if (PubHelper.p_BusinOper.AsileOper.VendBoxList[0].TmpControlModel == BusinessEnum.TmpControlModel.Refrigeration)
                                                        {
                                                            // 制冷
                                                            btnRefStatus.Path = SkinHelper.p_SkinName + "ProductPage/ProductList/snow.png";
                                                        }
                                                        else
                                                        {
                                                            // 加热
                                                            btnRefStatus.Path = SkinHelper.p_SkinName + "ProductPage/ProductList/heat.png";
                                                        }
                                                        btnRefStatus.Visibility = System.Windows.Visibility.Visible;
                                                    }
                                                    break;
                                                default:// 关闭或不存在回路
                                                    if (btnRefStatus.Visibility == System.Windows.Visibility.Visible)
                                                    {
                                                        btnRefStatus.Visibility = System.Windows.Visibility.Hidden;
                                                    }
                                                    break;
                                            }
                                        }

                                        // 如果温度传感器正常，则显示温度
                                        tbTemperature.Text = DictionaryHelper.Dictionary_NowTmp(0,false);

                                        #endregion
                                    }));

                                    #endregion
                                }
                                else
                                {
                                    #region 监控设备工作状态不正常

                                    if (!PubHelper.p_BusinOper.DeviceInfo.KmbConnectStatus)
                                    {
                                        PubHelper.p_SoftWorkStatus = false;
                                        ////if ((strIsTestVer == "0") &&
                                        ////    (blnKmbStatusHistory != PubHelper.p_BusinOper.DeviceInfo.KmbConnectStatus))
                                        if (strIsTestVer == "0")
                                        {
                                            blnKmbStatusHistory = PubHelper.p_BusinOper.DeviceInfo.KmbConnectStatus;
                                            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                            {
                                                SetMainInfo(false);

                                                if (btnConfig.Visibility == System.Windows.Visibility.Visible)
                                                {
                                                    btnConfig.Visibility = System.Windows.Visibility.Hidden;
                                                    btnDoor.Visibility = System.Windows.Visibility.Hidden;
                                                }
                                            }));
                                        }

                                        // 控制主板状态连接不正常，重新连接
                                        PubHelper.p_BusinOper.Device_SelfCheck(false);
                                    }

                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            intMonitorDeviceNum = 0;
                        }

                        #endregion

                        #region 监控远程控制指令

                        if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main) &&
                            (PubHelper.p_BusinOper.ConfigInfo.RemoteControlSwitch == BusinessEnum.ControlSwitch.Run))
                        {
                            PubHelper.p_BusinOper.GetRemoteData();
                        }

                        #endregion

                        #region 选择商品时，检查是否允许购买

                        if ((m_BlnQueryPaStatus) &&
                            (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.ChoiceGoods))
                        {
                            CheckSaleEnvir();
                        }

                        if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.ChoiceGoodsEnd)
                        {
                            ChoiceGoodsEnd();
                        }

                        #endregion

                        #region 出货

                        if ((m_BlnIsSellGoods) &&
                            (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Payment))
                        {
                            SellGoods();
                            intTotalMoney = PubHelper.p_BusinOper.TotalPayMoney;
                        }

                        #endregion

                        #region 领取赠品 2016-08-31 

                        if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Gift)
                        {
                            if (PubHelper.p_QueryGiftNum == 0)
                            {
                                PubHelper.p_QueryGiftNum++;
                                ChoiceGift();
                            }
                        }

                        #endregion

                        #region 退币

                        if ((intTotalMoney > 0) && (m_BlnIsReturnMoney) &&
                            (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.ChangeCoin))
                        {
                            // 需要退币
                            ReturnCoin();
                            intTotalMoney = PubHelper.p_BusinOper.TotalPayMoney;
                        }

                        #endregion

                        #region 吞币

                        if ((PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main) && (intTotalMoney > 0))
                        {
                            CheckIsTunCoin();

                            CheckIsQueryMoney();
                            PubHelper.p_BusinOper.DisableCashPaymnet(false);
                            intTotalMoney = PubHelper.p_BusinOper.TotalPayMoney;
                        }

                        #endregion

                        #region 从商品详细界面返回到主界面过程

                        if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.ReturnMain)
                        {
                            //MessageBox.Show("Opening port5");
                            m_VoiceRecognitionTrdSus = false;
                            //initiateVoiceRecognitionPort();

                            PubHelper.p_IsTrdOper = false;

                            ////PubHelper.p_BusinOper.DisableCashPaymnet(false);
                            PubHelper.p_BusinOper.ControlPayEnviro(BusinessEnum.PayMent.All, false, false);

                            PubHelper.p_IsTrdOper = true;
                            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
                        }

                        #endregion

                        #region 投币

                        blnPutMoney = false;

                        if ((m_BlnQueryMoney) && (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Payment))
                        {
                            if (m_CheckSaleEnvirCode == BusinessEnum.ServerReason.Normal)
                                ////(m_CheckSaleEnvirCode != BusinessEnum.ServerReason.Normal) && (intTotalMoney > 0))
                            {
                                blnPutMoney = true;
                                m_QueryCashSource = "0";
                            }
                        }
                        else if ((intTotalMoney > 0) && (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Main))
                        {
                            // 如果是在主界面，且此时有金额，则主要进行是否退币
                            blnPutMoney = true;
                            m_QueryCashSource = "1";
                        }

                        if (blnPutMoney)
                        {
                            intQueryMoneNum++;
                            intMaxQueryMonNum = GetMaxQueryMoneyNum(intTotalMoney);

                            if (intQueryMoneNum > intMaxQueryMonNum)
                            {
                                intQueryMoneNum = 0;

                                #region 查询金额

                                blnIsRefreshEnouthCoin = false;

                                intErrCode = PubHelper.p_BusinOper.QueryMoney(m_QueryCashSource, out intMoney, out blnIsReturnMoney);

                                if (intErrCode == 0)
                                {
                                    if (blnIsReturnMoney)
                                    {
                                        #region 硬件退币按钮被触发，检测是否需要退币

                                        ReturnCoinOper();

                                        #endregion
                                    }
                                    else
                                    {
                                        if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Payment)
                                        {
                                            intQueryMoneyNum++;
                                            if (intQueryMoneyNum > 8)
                                            {
                                                intQueryMoneyNum = 0;
                                                // 根据零钱是否充足来控制纸币器
                                                PubHelper.p_BusinOper.ControlCashEnoughCoin(false, out blnIsRefreshEnouthCoin, out strOutValue);
                                            }

                                            PubHelper.p_BusinOper.CheckPayEnviro_Cash_1();
                                        }

                                        if (PubHelper.p_BusinOper.TotalPayMoney_NoStack > 0)
                                        {
                                            // 检测零钱是否充足，以决定是否弹出压钞或退钞
                                            long intCoinStock = PubHelper.p_BusinOper.CashInfoOper.GetCashStockMoney_Type("0", "0");
                                            int intGoodsPrice = m_PaPrice;
                                            if (PubHelper.p_BusinOper.TotalPayMoney + PubHelper.p_BusinOper.TotalPayMoney_NoStack - 
                                                intGoodsPrice > intCoinStock)
                                            {
                                                // 停止时间监控
                                                StopMonOutTime();

                                                // 找零零钱不充足，弹出对话框提示
                                                string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SellGoods_NoEnoughChange");
                                                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    PubHelper.p_MsgModel = "1";
                                                    PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
                                                    PubHelper.p_MsgModel = "0";
                                                    if (PubHelper.p_MsgResult)
                                                    {
                                                        // 压钞
                                                        PubHelper.p_BusinOper.StackBillMoney("1");
                                                    }
                                                    else
                                                    {
                                                        // 退钞
                                                        PubHelper.p_BusinOper.StackBillMoney("0");
                                                    }

                                                    // 开启时间监控
                                                    BeginMonOutTime();
                                                }));
                                            }
                                        }
                                        else
                                        {
                                            if (intMoney > 0)
                                            {
                                                #region 存在金额投入

                                                if (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Payment)
                                                {
                                                    BeginMonOutTime();
                                                }

                                                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                                                {
                                                    SetPayMsgInfo();
                                                }));

                                                #endregion
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            intQueryMoneNum = 0;
                        }

                        #endregion

                        #region 其它非现金支付方式购买

                        if ((m_CheckSaleEnvirCode == BusinessEnum.ServerReason.Normal) && 
                            (PubHelper.p_BusinOper.OperStep == BusinessEnum.OperStep.Payment) &&
                                (intTotalMoney == 0) &&
                            (m_IsCheckSaleEnvir))
                        {
                            #region 储值卡购买

                            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_IC == BusinessEnum.PayShowStatus.Normal)
                            {
                                PostCard();
                            }

                            #endregion

                            #region 非储值会员卡购买

                            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_NoFeeCard == BusinessEnum.PayShowStatus.Normal)
                            {
                                PostNoFeeCard();
                            }

                            #endregion

                            #region 虚拟会员卡（二维码）购买

                            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_QRCode == BusinessEnum.PayShowStatus.Normal)
                            {
                                PostQrCodeCard();
                            }

                            #endregion

                            #region 条码类支付方式购买（支付宝付款码、微信刷卡）

                            if ((PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_AliPay_Code == BusinessEnum.PayShowStatus.Normal) ||
                                (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_WeChatCode == BusinessEnum.PayShowStatus.Normal) ||
                                (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_BestPay_Code == BusinessEnum.PayShowStatus.Normal) ||
                                (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_JDPay_Code == BusinessEnum.PayShowStatus.Normal) ||
                                (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_Volunteer_Code == BusinessEnum.PayShowStatus.Normal))
                            {
                                PostBarCode();
                            }

                            #endregion

                            #region 二代身份证

                            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_IDCode == BusinessEnum.PayShowStatus.Normal)
                            {
                                PostIDCard();
                            }

                            #endregion
                        }
                        #endregion
                    }
                    catch(Exception ex)
                    {
                        PubHelper.WriteSystemLog("Main Thread,Error " + ex.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                PubHelper.WriteSystemLog("Main Thread,Error " + ex.ToString());
            }

            // 主线程结束
            m_MainMonTrd = true;
        }

        /// <summary>
        /// 设备自检
        /// </summary>
        private void CheckDevice(string isTestVer)
        {
            PubHelper.p_IsCheckDevice = false;

            bool blnIsMain = false;// 是否能进入主界面
            // 设备自检
            int intErrCode = PubHelper.p_BusinOper.Device_SelfCheck(true);
            if (intErrCode == 0)
            {
                PubHelper.p_IsCheckDevice = true;
                blnIsMain = true;
            }
            else
            {
                // 自检失败
                if (isTestVer == "1")
                {
                    // 如果属于测试版本，也可以进入主界面
                    blnIsMain = true;
                }
            }

            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            {
                if (blnIsMain)
                {
                    lblInitProgress.Visibility = System.Windows.Visibility.Hidden;
                    lblErr_Close.Visibility = System.Windows.Visibility.Hidden;

                    panelInit.Visibility = System.Windows.Visibility.Hidden;

                    panelProduct.Visibility = System.Windows.Visibility.Visible;
                    cav.Visibility = System.Windows.Visibility.Visible;

                    PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;

                    Init(true);
                }
                else
                {
                    lblInitProgress.Content = PubHelper.p_LangOper.GetStringBundle("Err_PauseServer_Reason_InitDevice");
                    DispatcherHelper.SleepControl();
                    PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.InitErr;
                }
            }));
        }

        #region 退币相关业务

        /// <summary>
        /// 退币操作主业务
        /// </summary>
        private void ReturnCoinOper()
        {
            CheckIsReturnCoin();
            if ((PubHelper.p_BusinOper.TotalPayMoney > 0) && (!m_BlnIsReturnMoney))
            {
                ////if (m_ReturnCoinSource == "0")
                ////{
                // 如果是从商品详细界面退币
                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                {
                    SetOperFailControl(PubHelper.p_LangOper.GetStringBundle("Err_SellGoods_NoBuy"));

                    tbSetMoney.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_PutMoney").Replace("{N}", PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.TotalPayMoney.ToString()));
                }));
                ////}
            }
        }

        /// <summary>
        /// 检测能否进行退币操作
        /// </summary>
        private void CheckIsReturnCoin()
        {
            bool blnIsReturnMoney = false;

            try
            {
                if ((PubHelper.p_BusinOper.TotalPayMoney == 0) ||
                    (!m_BlnQueryMoney))
                {
                    blnIsReturnMoney = false;
                    return;
                }

                if (PubHelper.p_BusinOper.ConfigInfo.ChangeModel == "0")
                {
                    // 如果兑零模式为关闭兑零（强制购买），顾客需购买一次商品后方可进行退币操作
                    if (PubHelper.p_BusinOper.SellGoodsNum == 0)
                    {
                        // 顾客投币后，还没有进行购买
                        blnIsReturnMoney = false;
                        return;
                    }
                }

                blnIsReturnMoney = true;
                PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.ChangeCoin;
            }
            catch
            {
                blnIsReturnMoney = false;
            }
            finally
            {
                m_BlnIsReturnMoney = blnIsReturnMoney;
                if ((m_BlnIsReturnMoney) && (m_QueryCashSource == "1"))
                {
                    // 如果是在主界面允许退币，禁止点击商品
                    m_IsClickGoods = false;
                }
                else
                {
                    m_IsClickGoods = true;
                }
            }
        }

        /// <summary>
        /// 执行退币
        /// </summary>
        private void ReturnCoin()
        {
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.ChangeCoin;

            string strTip = string.Empty;

            if (m_QueryCashSource == "0")
            {
                StopMonOutTime();

                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                {
                    SetProgressingControl(PubHelper.p_LangOper.GetStringBundle("SellGoods_ChangeCoin_Init"));// "正在退币...";
                }));
            }
            else
            {
                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                {
                    if (imgReturnCoin_Bottom.Visibility == System.Windows.Visibility.Visible)
                    {
                        imgReturnCoin_Bottom.Visibility = System.Windows.Visibility.Hidden;
                    }
                    tbMainInfo.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_ChangeCoin_Init");// "正在退币...";
                }));
            }

            int intErrCode = PubHelper.p_BusinOper.ReturnCoin(m_QueryCashSource);

            switch (intErrCode)
            {
                case 0:// 成功
                ////    if (PubHelper.p_BusinOper.TotalPayMoney == 0)
                ////    {
                ////        m_SellGoodsNum = 0;
                ////    }
                    break;

                case 1:// 要退币的金额小于控制主板目前已有金额
                    strTip = PubHelper.p_LangOper.GetStringBundle("Err_ReturnMoney_NoEnoughFee");
                    break;

                case 2:
                    // 零钱不足
                    strTip = PubHelper.p_LangOper.GetStringBundle("Err_ReturnMoney_NoCoinChange");
                    break;

                default:
                    // 硬币器故障
                    strTip = PubHelper.p_LangOper.GetStringBundle("Err_ReturnMoney_Fail");
                    break;
            }

            #region 退币结束后的处理流程

            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            {
                if (PubHelper.p_BusinOper.TotalPayMoney != 0)
                {
                    // 退币失败
                    SetOperFailControl(strTip);
                }

                SetMainInfo(true);

                if (m_QueryCashSource == "0")
                {
                    ////if (PubHelper.p_BusinOper.TotalPayMoney != 0)
                    ////{
                    ////    // 退币失败
                    ////    SetOperFailControl(strTip);
                    ////}

                    ////SetMainInfo(true);
                    BeginMainStory();
                }
            }));

            m_BlnIsReturnMoney = false;

            m_IsClickGoods = true;

            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;

            #endregion
        }

        #endregion

        /// <summary>
        /// 检测是否需要吞币
        /// </summary>
        private void CheckIsTunCoin()
        {
            // 检测是否吞币
            PubHelper.p_BusinOper.CheckIsTunCoin();
            if (PubHelper.p_BusinOper.DeviceInfo.IsClearMoney)
            {
                PubHelper.p_BusinOper.DeviceInfo.IsClearMoney = false;
                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                {
                    SetMainInfo(true);
                }));
            }
        }

        /// <summary>
        /// 检测是否需要查询金额
        /// </summary>
        private void CheckIsQueryMoney()
        {
            if (PubHelper.p_BusinOper.TotalPayMoney > 0)
            {
                m_BlnQueryMoney = true;
            }
            else
            {
                m_BlnQueryMoney = false;
            }
        }

        /// <summary>
        /// 获取最大查询金额间隔次数
        /// </summary>
        /// <returns></returns>
        private int GetMaxQueryMoneyNum(int totalMoney)
        {
            int intMaxQueryMonNum = 4;
            switch (m_QueryCashSource)
            {
                case "1":// 如果是在主界面，则查询金额的最大间隔次数为5次
                    break;
                default:// 如果是在商品详细展示页面
                    if (totalMoney == 0)
                    {
                        if ((PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_IC == BusinessEnum.PayShowStatus.Normal) ||
                            (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_NoFeeCard == BusinessEnum.PayShowStatus.Normal) ||
                            (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_AliPay_Code == BusinessEnum.PayShowStatus.Normal))
                        {
                            // 如果金额等于0，则只要一卡通、磁条会员卡、支付宝扫码支付任何一种正常
                            intMaxQueryMonNum = 2;
                        }
                    }
                    
                    break;
            }

            return intMaxQueryMonNum;
        }

        /// <summary>
        /// 检测是否允许购买当前商品
        /// </summary>
        private void CheckSaleEnvir()
        {
            m_IsCheckSaleEnvir = false;// 认为购买环境检查准备开始

            m_CheckSaleEnvirCode = PubHelper.p_BusinOper.Device_CheckSaleEnvir(m_CurrentChoiceCode, m_PaPrice, false, PubHelper.p_ChoiceGift);

            ////PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Payment;
            m_BlnQueryPaStatus = false;
            m_BlnQueryMoney = false;
            if (PubHelper.p_BusinOper.PaymentOper.PaymentList.PayShow_Cash != BusinessEnum.PayShowStatus.Pause)
            {
                m_BlnQueryMoney = true;
            }

            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.ChoiceGoodsEnd;

            //////BeginMonOutTime();

            //////if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile) ||
            //////    (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType))
            //////{
            //////    if (PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo != null)
            //////    {
            //////        tbProductName.Text = PubHelper.GetMcdName(PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.PaCode,
            //////            PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdName, true);
            //////        tbProductContent.Text = PubHelper.ConvertGoodsPropText();
            //////    }
            //////}
            //////imgExit.Visibility = System.Windows.Visibility.Visible;
            //////if (m_CheckSaleEnvirCode == BusinessEnum.ServerReason.Normal)
            //////{
            //////    // 正常，可以购买
            //////    SetPayMsgInfo();
            //////}
            //////else
            //////{
            //////    // 不正常，不能购买
            //////    tbSetMoney.Text = DictionaryHelper.Dictionary_SaleEnvirCode(m_CheckSaleEnvirCode);
            //////}
            //////DispatcherHelper.SleepControl();
            m_IsCheckSaleEnvir = true;// 认为购买环境检查已经结束

            //////this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            //////{
            //////    ////m_BlnQueryPaStatus = false;
                
            //////}));
        }

        private void ChoiceGoodsEnd()
        {
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Payment;

            BeginMonOutTime();

            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            {
                if ((PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile) ||
                (PubHelper.p_BusinOper.ConfigInfo.GoodsShowModel == BusinessEnum.GoodsShowModelType.GoodsType))
                {
                    if (PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo != null)
                    {
                        tbProductName.Text = PubHelper.GetMcdName(PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.PaCode,
                            PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdName, true);
                        tbProductContent.Text = PubHelper.ConvertGoodsPropText();
                    }
                }
                imgExit.Visibility = System.Windows.Visibility.Visible;
                if (m_CheckSaleEnvirCode == BusinessEnum.ServerReason.Normal)
                {
                    // 正常，可以购买
                    SetPayMsgInfo();
                }
                else
                {
                    // 不正常，不能购买
                    tbSetMoney.Text = DictionaryHelper.Dictionary_SaleEnvirCode(m_CheckSaleEnvirCode);
                }
                DispatcherHelper.SleepControl();

            }));
            
        }

        /// <summary>
        /// 执行储值卡刷卡
        /// </summary>
        private void PostCard()
        {
            /*****************流程说明****************
             * 1、发送查询卡信息指令
             * 2、如果有卡且卡余额充足，则发送卡扣款指令；否则显示余额不足
            ******************流程说明****************/
            string strErrInfo = string.Empty;
            int intErrCode = 0;
            string strCardNum_Title = string.Empty;
            string strBanFee_Title = string.Empty;
            string strSellPrice_Title = string.Empty;
            string strFactMoney_Title = string.Empty;

            if (PubHelper.p_BusinOper.ConfigInfo.IcBusiModel == "0")
            {
                // 如果是先查询后扣款模式
                #region 发送查询卡信息指令

                intErrCode = PubHelper.p_BusinOper.QueryCardInfo();
                if (intErrCode == 0)
                {
                    if (PubHelper.p_BusinOper.ConfigInfo.IcPayShowSwitch == BusinessEnum.ControlSwitch.Run)
                    {
                        strCardNum_Title = PubHelper.p_LangOper.GetStringBundle("PosCard_CardNum");
                        strBanFee_Title = PubHelper.p_LangOper.GetStringBundle("PosCard_BanFee");
                        strSellPrice_Title = PubHelper.p_LangOper.GetStringBundle("Pub_Money_Pay");// 扣款金额
                        strFactMoney_Title = PubHelper.p_LangOper.GetStringBundle("Pub_Money_Fact");// 实扣金额
                    }
                    // 查询成功，如果卡余额充足，则发送卡扣款指令；否则显示余额不足
                    if (m_PaPrice > Convert.ToInt32(PubHelper.p_BusinOper.UserCardInfo.BanFee))
                    {
                        // 余额不足
                        intErrCode = 12;
                    }
                    if (intErrCode == 0)
                    {
                        #region 显示会员相关信息数据

                        this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                        {
                            if (PubHelper.p_BusinOper.ConfigInfo.IcPayShowSwitch == BusinessEnum.ControlSwitch.Run)
                            {
                                cardUserArea.Visibility = System.Windows.Visibility.Visible;
                                tbCardNum.Text = strCardNum_Title + PubHelper.ConvertCardNum_IC(PubHelper.p_BusinOper.UserCardInfo.CardNum_Show);
                                tbBanfee.Text = strBanFee_Title + PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.UserCardInfo.BanFee);

                                tbSellPrice_Card.Text = strSellPrice_Title + PubHelper.p_BusinOper.MoneyIntToString(m_PaPrice.ToString());
                                tbFactFee_Card.Text = strFactMoney_Title;
                            }
                            else
                            {
                                cardUserArea.Visibility = System.Windows.Visibility.Hidden;
                            }

                            SetProgressingControl(PubHelper.p_LangOper.GetStringBundle("Pub_Posting"));
                        }));

                        #endregion
                    }
                }
                #endregion
            }
            else
            {
                intErrCode = 0;
            }

            if (intErrCode == 0)
            {
                intErrCode = PubHelper.p_BusinOper.PostCardPay(m_PaPrice);
                if (intErrCode == 0)
                {
                    #region 扣款成功
                    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                    {
                        if (PubHelper.p_BusinOper.ConfigInfo.IcPayShowSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            if (PubHelper.p_BusinOper.ConfigInfo.IcBusiModel == "1")
                            {
                                // 如果是直接扣款模式
                                strCardNum_Title = PubHelper.p_LangOper.GetStringBundle("PosCard_CardNum");
                                strBanFee_Title = PubHelper.p_LangOper.GetStringBundle("PosCard_BanFee");
                                strSellPrice_Title = PubHelper.p_LangOper.GetStringBundle("Pub_Money_Pay");// 扣款金额
                                strFactMoney_Title = PubHelper.p_LangOper.GetStringBundle("Pub_Money_Fact");// 实扣金额

                                cardUserArea.Visibility = System.Windows.Visibility.Visible;
                                tbSellPrice_Card.Text = strSellPrice_Title + PubHelper.p_BusinOper.MoneyIntToString(m_PaPrice.ToString());
                            }

                            tbCardNum.Text = strCardNum_Title + PubHelper.ConvertCardNum_IC(PubHelper.p_BusinOper.UserCardInfo.CardNum_Show);
                            tbBanfee.Text = strBanFee_Title + PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.UserCardInfo.BanFee);
                            tbFactFee_Card.Text = strFactMoney_Title + PubHelper.p_BusinOper.MoneyIntToString(m_PaPrice.ToString());
                        }
                        else
                        {
                            cardUserArea.Visibility = System.Windows.Visibility.Hidden;
                        }
                        DropGoods(true);
                    }));

                    #endregion
                }
            }

            if ((intErrCode != 0) && (intErrCode != 10) && (intErrCode != 98))
            {
                strErrInfo = DictionaryHelper.Dictionary_PostCardCode(intErrCode);
                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                {
                    SetOperFailControl(strErrInfo);

                    SetPayMsgInfo();
                    ////SetSellTip(true);
                }));
            }
        }

        /// <summary>
        /// 查询卡—弹出查询窗口
        /// </summary>
        private void QueryCard()
        {
            string strErrInfo = string.Empty;
            int intErrCode = 0;

            intErrCode = PubHelper.p_BusinOper.QueryCardInfo();
            switch (intErrCode)
            {
                case 0:// 成功
                    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                    {
                        PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.ShowCardInfo;
                        FrmShowCardInfo frmShowCardInfo = new FrmShowCardInfo();
                        frmShowCardInfo.ShowDialog();
                        PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
                    }));
                    break;

                case 10:// 无卡
                case 98:// 一卡通正在发数据
                    break;

                default:// 其它代码
                    break;
            }
        }

        /// <summary>
        /// 执行非储值会员支付
        /// </summary>
        private void PostNoFeeCard()
        {
            string strErrInfo = string.Empty;
            string strCardData = string.Empty;
            string strCardNum_Title = string.Empty;
            string strBanFee_Title = string.Empty;
            string strSellPrice_Title = string.Empty;
            string strFactMoney_Title = string.Empty;
            int intFactMoney = 0;

            int intErrCode = PubHelper.p_BusinOper.QueryNoFeeCardData(out strCardData);

            if ((intErrCode == 0) && (!string.IsNullOrEmpty(strCardData)))
            {
                // 界面更改
                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                {
                    ControlNoCashPayInfo(false, PubHelper.p_LangOper.GetStringBundle("Pub_Querying"));
                }));

                // 读取到卡号，查询会员信息
                intErrCode = PubHelper.p_BusinOper.QueryNoFeeCardInfo(strCardData, m_PaPrice);
                if (intErrCode == 0)
                {
                    if (PubHelper.p_BusinOper.ConfigInfo.NoFeeCardPayShow == BusinessEnum.ControlSwitch.Run)
                    {
                        strCardNum_Title = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_CardNum");
                        strBanFee_Title = PubHelper.p_LangOper.GetStringBundle("NoFeeCard_BanFee");
                        strSellPrice_Title = PubHelper.p_LangOper.GetStringBundle("Pub_Money_Pay");// 扣款金额
                        strFactMoney_Title = PubHelper.p_LangOper.GetStringBundle("Pub_Money_Fact");// 实扣金额
                    }

                    #region 显示会员相关信息数据

                    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                    {
                        if (PubHelper.p_BusinOper.ConfigInfo.NoFeeCardPayShow == BusinessEnum.ControlSwitch.Run)
                        {
                            cardUserArea.Visibility = System.Windows.Visibility.Visible;
                            tbCardNum.Text = strCardNum_Title + PubHelper.ConvertCardNum_IC(PubHelper.p_BusinOper.MemberUserInfo.CardNum_Out);
                            tbBanfee.Text = strBanFee_Title + PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.MemberUserInfo.BanFee);
                            tbSellPrice_Card.Text = strSellPrice_Title + PubHelper.p_BusinOper.MoneyIntToString(m_PaPrice.ToString());
                            tbFactFee_Card.Text = strFactMoney_Title;
                        }
                        else
                        {
                            cardUserArea.Visibility = System.Windows.Visibility.Hidden;
                        }
                        SetProgressingControl(PubHelper.p_LangOper.GetStringBundle("Pub_Posting"));
                    }));

                    #endregion

                    #region 会员信息查询正常，提交扣费请求

                    intErrCode = PubHelper.p_BusinOper.PostNoFeeCardPay(strCardData, m_PaPrice, out intFactMoney);

                    #endregion
                }

                switch (intErrCode)
                {
                    case 0:// 成功，出货
                        this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                        {
                            if (PubHelper.p_BusinOper.ConfigInfo.NoFeeCardPayShow == BusinessEnum.ControlSwitch.Run)
                            {
                                #region 显示会员相关数据信息

                                tbBanfee.Text = strBanFee_Title + PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.MemberUserInfo.BanFee);
                                tbFactFee_Card.Text = strFactMoney_Title + PubHelper.p_BusinOper.MoneyIntToString(intFactMoney.ToString());

                                #endregion
                            }

                            DropGoods(true);
                        }));
                        break;
                    default:
                        strErrInfo = DictionaryHelper.Dictionary_NoFeeCardCode(intErrCode);
                        break;
                }

                if (intErrCode != 0)
                {
                    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                    {
                        SetOperFailControl(strErrInfo);

                        SetPayMsgInfo();

                        ControlNoCashPayInfo(true,"");
                    }));
                }
            }
        }

        /// <summary>
        /// 执行条形码类支付（支付宝付款码）
        /// </summary>
        private void PostBarCode()
        {
            string strErrInfo = string.Empty;
            string strBarCodeData = string.Empty;
            string strErrCode = string.Empty;
            int intErrCode = PubHelper.p_BusinOper.BarCodeOper.QueryBarCodeNum(out strBarCodeData, out strErrCode);

            if ((intErrCode == 0) && (!string.IsNullOrEmpty(strBarCodeData)))
            {
                // 检测该条形码内容属于什么支付方式
                BusinessEnum.PayMent payMent = PubHelper.p_BusinOper.PaymentOper.ConvertCodeToPay(strBarCodeData);

                #region 属于支付宝付款码或者微信刷卡支付

                if ((payMent == BusinessEnum.PayMent.AliPay_Code) ||
                    (payMent == BusinessEnum.PayMent.WeChatCode) ||
                    (payMent == BusinessEnum.PayMent.BestPay_Code) ||
                    (payMent == BusinessEnum.PayMent.Volunteer))
                {
                    // 界面更改
                    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                    {
                        ControlNoCashPayInfo(false, PubHelper.p_LangOper.GetStringBundle("Pub_BarCode_Net_Pay"));
                    }));

                    string strPayType = "0";
                    if ((payMent == BusinessEnum.PayMent.Volunteer))
                    {
                        strPayType = "1";
                    }
                    
                    // 提交支付请求
                    int _resultCode = 0;
                    bool blnPayResult = false;// 支付结果 False：失败 True：成功
                    bool blnIsQuery = false;// 是否发送查询请求 False：否 True：是

                    intErrCode = PubHelper.p_BusinOper.BarCode_Net_Pay(strBarCodeData, PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.PaCode,
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdCode,
                        m_PaPrice,strPayType, out _resultCode);
                    if ((intErrCode == 0) && (payMent == BusinessEnum.PayMent.Volunteer))
                    {
                        _resultCode = 1;
                    }

                    if (intErrCode == 0)
                    {
                        if (_resultCode == 1)
                        {
                            // 支付成功
                            blnPayResult = true;
                        }
                        if (_resultCode == 3)
                        {
                            // 需要等待支付结果
                            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                            {
                                SetProgressingControl(PubHelper.p_LangOper.GetStringBundle("Pub_BarCode_Net_Query"));
                            }));

                            blnIsQuery = true;
                        }
                    }
                    else if (intErrCode == 5)
                    {
                        // 网络数据发送失败（超时等）
                        if (payMent != BusinessEnum.PayMent.Volunteer)
                        {
                            blnIsQuery = true;
                        }
                    }

                    if (blnIsQuery)
                    {
                        #region 提交查询请求

                        intErrCode = PubHelper.p_BusinOper.BarCode_Net_Query(strBarCodeData, PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.PaCode, m_PaPrice, out _resultCode);
                        if ((intErrCode == 0) && (_resultCode == 1))
                        {
                            // 支付成功
                            blnPayResult = true;
                        }

                        #endregion
                    }

                    switch (blnPayResult)
                    {
                        case true:// 成功
                            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                            {
                                DropGoods(true);
                            }));
                            break;
                        default:// 失败
                            if (payMent != BusinessEnum.PayMent.Volunteer)
                            {
                                strErrInfo = DictionaryHelper.Dictionary_AliPayCode(_resultCode);
                            }
                            else
                            {
                                strErrInfo = DictionaryHelper.Dictionary_Volunteer(intErrCode);
                            }
                            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                            {
                                SetOperFailControl(strErrInfo);

                                SetPayMsgInfo();

                                ControlNoCashPayInfo(true, "");
                            }));
                            break;
                    }
                }

                #endregion
            }
        }

        /// <summary>
        /// 执行虚拟会员卡（二维码）支付
        /// </summary>
        private void PostQrCodeCard()
        {
            //////string strCardData = string.Empty;
            //////int intErrCode = PubHelper.p_BusinOper.QueryQrCodeData(out strCardData);

            //////if ((intErrCode == 0) && (!string.IsNullOrEmpty(strCardData)))
            //////{
            //////    // 读取到卡号，提交支付
            //////    ////PayNoFeeCard(strCardData);
            //////}
        }

        /// <summary>
        /// 执行二代身份证业务（2015-06-10）
        /// </summary>
        private void PostIDCard()
        {
            ////string strErrInfo = string.Empty;
            ////string strCardData = string.Empty;
            ////string strCardNum_Title = string.Empty;
            ////string strBanFee_Title = string.Empty;
            ////string strSellPrice_Title = string.Empty;
            ////string strFactMoney_Title = string.Empty;
            ////int intFactMoney = 0;

            ////int intErrCode = PubHelper.p_BusinOper.IDCardOper.SelectIDCard();

            ////if (intErrCode == 0)
            ////{
            ////    // 寻到身份证卡，界面更改
            ////    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            ////    {
            ////        ControlNoCashPayInfo(false, PubHelper.p_LangOper.GetStringBundle("FreeSell_IDCard_Pay_Reading"));
            ////    }));

            ////    // 读取身份证信息
            ////    string strIDCardNum = string.Empty;
            ////    intErrCode = PubHelper.p_BusinOper.IDCardOper.ReadIDCardInfo(out strIDCardNum);
            ////    if (intErrCode == 0)
            ////    {
            ////        this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            ////        {
            ////            SetProgressingControl(PubHelper.p_LangOper.GetStringBundle("Pub_Querying"));
            ////        }));

            ////        #region 读取二代身份证，提交请求

            ////        intErrCode = PubHelper.p_BusinOper.PostIDCard();
            ////        intErrCode = 0;// 模拟测试

            ////        #endregion
            ////    }

            ////    switch (intErrCode)
            ////    {
            ////        case 0:// 成功，出货
            ////            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            ////            {
            ////                DropGoods(true);
            ////            }));
            ////            break;
            ////        default:
            ////            strErrInfo = DictionaryHelper.Dictionary_IDCardCode(intErrCode);
            ////            break;
            ////    }

            ////    if (intErrCode != 0)
            ////    {
            ////        this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            ////        {
            ////            SetOperFailControl(strErrInfo);

            ////            SetPayMsgInfo();

            ////            ControlNoCashPayInfo(true, "");
            ////        }));
            ////    }
            ////}
        }

        #region 执行处货（旧）

        /////// <summary>
        /////// 执行出货
        /////// </summary>
        ////private void SellGoods()
        ////{
        ////    int intErrCode = 0;
        ////    string strOperTip = string.Empty;

        ////    PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.SellGoods;

        ////    StopMonOutTime();

        ////    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
        ////    {
        ////        SetProgressingControl(PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Progress"));
        ////    }));

        ////    intErrCode = PubHelper.p_BusinOper.SellGoods(m_PaCode);
        ////    // 0：成功 1：无该货道 2：控制主板可用金额不足 3：无货 9：失败
        ////    switch (intErrCode)
        ////    {
        ////        case 0:// 成功
        ////            strOperTip = PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Suc");// "出货成功";

        ////            ////if (PubHelper.p_BusinOper.TotalPayMoney == 0)
        ////            ////{
        ////            ////    m_SellGoodsNum = 0;
        ////            ////}
        ////            ////else
        ////            ////{
        ////            ////    m_SellGoodsNum++;
        ////            ////}
        ////            break;

        ////        case 2:// 控制主板可用金额不足
        ////            strOperTip = PubHelper.p_LangOper.GetStringBundle("Err_SellGoods_NoEnoughFee");// "余额不足，不能出货";
        ////            break;

        ////        case 3:// 无货
        ////            strOperTip = PubHelper.p_LangOper.GetStringBundle("Err_SellGoods_NoGoods");// "已售完";
        ////            break;

        ////        default:
        ////            strOperTip = PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Fail");// "出货失败";
        ////            break;
        ////    }

        ////    bool blnIsReturn = true;
        ////    this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
        ////    {
        ////        if (cardUserArea.Visibility == System.Windows.Visibility.Visible)
        ////        {
        ////            cardUserArea.Visibility = System.Windows.Visibility.Hidden;
        ////        }

        ////        SetMainInfo(true);
        ////        RefreshAsileInfo(false, m_PaCode);

        ////        if (intErrCode == 0)
        ////        {
        ////            if ((PubHelper.p_BusinOper.ConfigInfo.SaleModel == "1") && (PubHelper.p_BusinOper.TotalPayMoney > 0))
        ////            {
        ////                // 如果购买方式为单次购买且当前有剩余金额，则进行自动退币
        ////                blnIsReturn = false;
        ////                m_BlnIsReturnMoney = true;
        ////            }
        ////            else
        ////            {
        ////                // 如果购买方式为连续购买或者当前金额为0，则返回主界面
        ////                (this.Resources["buyProductStory"] as Storyboard).Begin();
        ////                music.PlayMusic(Buymp3);
        ////                DispatcherHelper.SleepControl();
        ////            }
        ////        }
        ////        else
        ////        {
        ////            SetOperFailControl(strOperTip);

        ////            BeginMainStory();
        ////        }
        ////    }));

        ////    m_BlnIsSellGoods = false;

        ////    if (blnIsReturn)
        ////    {
        ////        CheckIsQueryMoney();
        ////        PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
        ////    }
        ////    else
        ////    {
        ////        m_BlnQueryMoney = false;
        ////        PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.ChangeCoin;
        ////    }
        ////}

        #endregion

        #region 执行处货（2014-11-18 新）

        /// <summary>
        /// 执行出货
        /// </summary>
        private void SellGoods()
        {
            int intErrCode = 0;
            string strOperTip = string.Empty;

            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.SellGoods;
            string strPaCode = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.PaCode;

            //MessageBox.Show("strPaCode: " + strPaCode);

            StopMonOutTime();

            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            {
                SetProgressingControl(PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Progress"));
            }));

            int intPaIndex = 0;
            // 开始执行处货
            intErrCode = PubHelper.p_BusinOper.SellGoods_Begin(strPaCode, out intPaIndex);

            if (intErrCode == 0)
            {
                #region 开始执行处货成功，界面动画改变

                this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
                {
                    (this.Resources["buyProductStory_Begin"] as Storyboard).Begin();
                }));

                #endregion

                // 查询出货结果
                intErrCode = PubHelper.p_BusinOper.SellGoods_Query(intPaIndex);
            }

            // 0：成功 1：无该货道 2：控制主板可用金额不足 3：无货 9：失败
            switch (intErrCode)
            {
                case 0:// 成功
                    strOperTip = PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Suc");// "出货成功";
                    strOperTip = strOperTip.Replace("{N}", strPaCode);

                    ////if (PubHelper.p_BusinOper.TotalPayMoney == 0)
                    ////{
                    ////    m_SellGoodsNum = 0;
                    ////}
                    ////else
                    ////{
                    ////    m_SellGoodsNum++;
                    ////}
                    break;

                case 2:// 控制主板可用金额不足
                    strOperTip = PubHelper.p_LangOper.GetStringBundle("Err_SellGoods_NoEnoughFee");// "余额不足，不能出货";
                    break;

                case 3:// 无货
                    strOperTip = PubHelper.p_LangOper.GetStringBundle("Err_SellGoods_NoGoods");// "已售完";
                    break;

                default:
                    strOperTip = PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Fail");// "出货失败";
                    break;
            }

            bool blnIsReturn = true;
            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            {
                if (cardUserArea.Visibility == System.Windows.Visibility.Visible)
                {
                    cardUserArea.Visibility = System.Windows.Visibility.Hidden;
                }

                ////SetMainInfo(true);
                ////RefreshGoodsShowList(false);

                if (intErrCode == 0)
                {
                    #region 出货成功

                    if ((PubHelper.p_BusinOper.ConfigInfo.SaleModel == "1") && (PubHelper.p_BusinOper.TotalPayMoney > 0))
                    {
                        // 如果购买方式为单次购买且当前有剩余金额，则进行自动退币
                        blnIsReturn = false;
                        m_BlnIsReturnMoney = true;
                    }
                    else
                    {
                        // 如果购买方式为连续购买或者当前金额为0，则返回主界面
                        tbSetMoney.Text = strOperTip;
                        (this.Resources["buyProductStory_Success"] as Storyboard).Begin();
                        music.PlayMusic(Buymp3);
                        DispatcherHelper.SleepControl();
                        Thread.Sleep(500);
                    }

                    #endregion
                }
                else
                {
                    SetOperFailControl(strOperTip);

                    BeginMainStory();
                }
                
                DispatcherHelper.SleepControl();
                SetMainInfo(true);
                RefreshGoodsShowList(false);
            }));

            m_BlnIsSellGoods = false;

            PubHelper.p_ChoiceGift = false;

            if (blnIsReturn)
            {
                CheckIsQueryMoney();

                #region 2016-08-29添加 检测是否需要弹出赠送礼品提示窗口

                //if ((PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.SellModel == BusinessEnum.AsileSellModel.Normal))
                if ((PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.SellModel == BusinessEnum.AsileSellModel.Normal) && (intErrCode == 0))
                {
                    // 本次购买为正常购买，且出货成功，则可能需要弹出需要赠送礼品提示窗口
                    // 检测赠送礼品的货道库存是否还有
                    int intGiftNum = 0;
                    for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.AsileList.Count; i++)
                    {
                        if ((PubHelper.p_BusinOper.AsileOper.AsileList[i].SellModel == BusinessEnum.AsileSellModel.Gift) &&
                            (PubHelper.p_BusinOper.AsileOper.AsileList[i].SurNum > 0))
                        {
                            intGiftNum ++;
                            break;
                        }
                    }
                    if (intGiftNum > 0)
                    {
                        PubHelper.p_QueryGiftNum = 0;
                        PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Gift;
                    }
                    else
                    {
                        PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
                    }
                }
                else
                {
                    // 本次购买为赠送礼品或出货失败，则直接按照正常流程
                    PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
                }

                #endregion
            }
            else
            {
                m_BlnQueryMoney = false;
                PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.ChangeCoin;
            }
        }

        #endregion

        #region 领取赠品（2016-08-31）上海志愿者

        private void ChoiceGift()
        {
            this.tbSetMoney.Dispatcher.Invoke(new Action(() =>
            {
                PubHelper.p_IsOperManager = true;// 退出管理设置
                this.Opacity = PubHelper.OPACITY_GRAY;
                PubHelper.p_ChoiceGift = false;
                FrmShZyZ_Gift FrmShZyZ_Gift = new FrmShZyZ_Gift();
                FrmShZyZ_Gift.ShowDialog();
                this.Opacity = PubHelper.OPACITY_NORMAL;
                PubHelper.p_IsOperManager = false;// 退出管理设置
                if (PubHelper.p_ChoiceGift)
                {
                    ViewProductDetail("", false, true);
                }
                else
                {
                    PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
                }
            }));
        }

        #endregion

        #endregion

        #region 终端配置及相关更新检测监控

        /// <summary>
        /// 终端配置监控线程
        /// </summary>
        private void VmConfigMonTrd()
        {
            string strAdvertCheckHour = string.Empty;
            string strAdvertNowHour = string.Empty;

            string strUploadDate = string.Empty;
            string strNowDate = string.Empty;

            int intMaxQueryAdvUploadNum = 10 * 60;// 一分钟
            int intQueryAdvNum = 0;

            string _errCode = string.Empty;

            try
            {
                while (!m_CloseForm)
                {
                    Thread.Sleep(100);

                    #region 定时更新终端配置相关参数【一天更新一次】

                    strNowDate = DateTime.Now.ToString("yyyyMMdd");
                    if (strUploadDate != strNowDate)
                    {
                        strUploadDate = strNowDate;
                        PubHelper.p_BusinOper.UploadSysCfg_Net(out _errCode);
                    }

                    #endregion

                    if (PubHelper.p_BusinOper.ConfigInfo.AdvertPlaySwitch == BusinessEnum.ControlSwitch.Run)
                    {
                        #region 定时检查广告是否需要更新【一小时更新一次】

                        strAdvertNowHour = DateTime.Now.ToString("HH");
                        if (strAdvertCheckHour != strAdvertNowHour)
                        {
                            strAdvertCheckHour = strAdvertNowHour;
                            PubHelper.p_BusinOper.UploadAdvertOper();
                        }

                        #endregion

                        #region 定时检查是否有需要下载的广告文件及新更新的广告节目单是否需要正式播放

                        intQueryAdvNum++;
                        if (intQueryAdvNum >= intMaxQueryAdvUploadNum)
                        {
                            intQueryAdvNum = 0;
                            PubHelper.p_BusinOper.QueryAdvUpload();

                            PubHelper.p_BusinOper.QueryMcdPicDown();
                        }

                        #endregion
                    }

                    #region 定时检查商品

                    #endregion
                }
            }
            catch
            {

            }
        }

        #endregion

        #region 语音识别监控

        /// <summary>
        /// 语音识别监控工作线程
        /// </summary>
        private void VoiceRecognitionTrd()
        {
            m_VoiceRecognitionTrd = false;

            m_VoiceRecognitionTrdSus = false;

            try
            {
                while (!m_CloseForm)
                {
                    Thread.Sleep(500);
                    //如果已经暂停，则暂时挂起进程，不继续识别语音
                    if (m_VoiceRecognitionTrdSus) continue;

                    if (!vrt_IsConnected)
                    {
                        if (initiateVoiceRecognitionPort())
                            vrt_IsConnected = true;
                    }


                }
                //关闭串口
                if (vrt_IsConnected)
                {
                    if (closeVoiceRecognitionPort())
                        vrt_IsConnected = false;
                }

            }
            catch (Exception ex)
            {
                PubHelper.WriteSystemLog("Voice Recognition Thread,Error " + ex.ToString());

                // 重新启动监控线程
                Thread TrdVoiceRecognition = new Thread(new ThreadStart(VoiceRecognitionTrd));
                TrdVoiceRecognition.IsBackground = true;
                TrdVoiceRecognition.Start();
                m_VoiceRecognitionTrdSus = false;
            }

            m_VoiceRecognitionTrd = true;
        }
        /// <summary>
        /// 打开语音识别监控串口
        /// </summary>
        bool initiateVoiceRecognitionPort()
        {
            if (vrt_IsConnected) return false;
            //MessageBox.Show("InitiateVoicePort " + vrt_IsConnected);
            string[] comNumber = SerialPort.GetPortNames();
            foreach (String s in comNumber)
            {
                vrt_Port = new SerialPort(s, vrt_BaudRate);
                vrt_Port.Encoding = Encoding.ASCII;
                try
                {
                    //MessageBox.Show("Port name: " + s);
                    vrt_Port.Open();
                    Thread.Sleep(100);
                    vrt_Port.Write(vrt_TestCommandSend);
                    Thread.Sleep(100);
                    if (vrt_Port.BytesToRead != 0)    //是否有收到数据
                    {
                        byte[] buf = new byte[1];
                        vrt_Port.Read(buf, 0, 1);
                        vrt_Builder.Clear();
                        foreach (byte b in buf)
                        {
                            vrt_Builder.Append(b.ToString("X2"));
                        }
                        //MessageBox.Show(vrt_Builder.ToString());
                        if (vrt_Builder.ToString().Equals(vrt_TestCommandRecv))
                        {
                            //MessageBox.Show("Succ port: " + s);
                            vrt_Port.DiscardInBuffer();
                            vrt_Port.DataReceived += port_DataReceived;
                            vrt_IsConnected = true;
                            return true;
                        }
                    }
                    vrt_Port.Close();
                    vrt_Port.Dispose();
                    vrt_Port = null;
                }
                catch (Exception ex)
                {
                    //捕获到异常信息，创建一个新的comm对象，之前的不能用了
                    vrt_Port = new SerialPort();
                    //现实异常信息给客户。  
                    //MessageBox.Show(ex.Message);
                }
            }
            return false;
        }
        //bool initiateVoiceRecognitionPort()
        //{
        //    if ("" == vrt_PortName || vrt_BaudRate < 1 || vrt_IsConnected)
        //        return false;

        //    vrt_Port = new SerialPort(vrt_PortName, vrt_BaudRate);
        //    vrt_Port.Encoding = Encoding.ASCII;
        //    vrt_Port.DataReceived += port_DataReceived;
        //    try
        //    {
        //        vrt_Port.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //        //捕获到异常信息，创建一个新的comm对象，之前的不能用了
        //        vrt_Port = new SerialPort();
        //        //现实异常信息给客户。  
        //        MessageBox.Show(ex.Message);
        //        return false;
        //    }
        //    vrt_IsConnected = true;
        //    return true;
        //}
        /// <summary>
        /// 关闭语音识别监控串口
        /// </summary>
        bool closeVoiceRecognitionPort()
        {
            if (!vrt_IsConnected)
                return false;

            try
            {
                vrt_Port.Close();
                vrt_Port.Dispose();
                vrt_Port = null;
            }
            catch (Exception ex)
            {
                //捕获到异常信息，创建一个新的comm对象，之前的不能用了
                vrt_Port = new SerialPort();
                //现实异常信息给客户。  
                MessageBox.Show(ex.Message);
                return false;
            }
            vrt_IsConnected = false;
            return true;
        }
        /// <summary>
        /// 处理收到的语音识别结果
        /// </summary>
        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int n = vrt_Port.BytesToRead;//先记录下来，避免某种原因，人为的原因，操作几次之间时间长，缓存不一致  
            byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据  
            vrt_Received_Count += n;//增加接收计数
            vrt_Port.Read(buf, 0, n);//读取缓冲数据  
            vrt_Builder.Clear();//清除字符串构造器的内容
            //依次的拼接出16进制字符串  
            foreach (byte b in buf)
            {
                vrt_Builder.Append(b.ToString("D3"));
            }
            //MessageBox.Show("Voice: " + vrt_Builder.ToString());
            //查询当前的货架上是否存在改编号的商品
            if (!m_VoiceRecognitionTrdSus && SelectGoodsByCode(vrt_Builder.ToString()))
            {
                //下面两种方法都会导致页面在更新的时候卡死
                //ViewProductDetail("", false, false);
                //this.Dispatcher.Invoke(new Action(() =>
                //{
                //    ViewProductDetail("", false, false);
                //}));

                updateProductDetail = new UpdateProductDetail(ViewProductDetailByVoice);
                this.Dispatcher.BeginInvoke(updateProductDetail, "", false, false);
            }
                //ViewProductDetail("", false, false);
            //this.Invoke((EventHandler)(delegate
            //{
            //    //判断是否是显示为16禁止  
            //    if (check_HexView.Checked)
            //    {
            //        //依次的拼接出16进制字符串  
            //        foreach (byte b in buf)
            //        {
            //            builder.Append(b.ToString("X2") + " ");
            //        }
            //    }
            //    else
            //    {
            //        //直接按ASCII规则转换成字符串  
            //        builder.Append(Encoding.ASCII.GetString(buf));
            //    }
            //    //追加的形式添加到文本框末端，并滚动到最后。  
            //    this.txtReceived.AppendText(builder.ToString());
            //    //修改接收计数  
            //    //labelGetCount.Text = "Get:" + received_count.ToString();
            //}));
        }
        /// <summary>
        /// 根据从语音识别接收到的商品编号查询货道
        /// <param name="McdCode"></param>
        /// </summary>
        bool SelectGoodsByCode(String v_McdCode)
        {
            if (v_McdCode.Equals("")) return false;

            for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.AsileList.Count; i++)
            {
                if (PubHelper.p_BusinOper.AsileOper.AsileList[i].McdCode.Equals(v_McdCode))
                {
                    if (PubHelper.p_BusinOper.GoodsShowModelType == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
                    {
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo = new AsileModel();
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.PaCode = PubHelper.p_BusinOper.AsileOper.AsileList[i].PaCode;
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.VendBoxCode = PubHelper.p_BusinOper.AsileOper.AsileList[i].VendBoxCode;
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.SellPrice = PubHelper.p_BusinOper.AsileOper.AsileList[i].SellPrice;
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdName = PubHelper.p_BusinOper.AsileOper.AsileList[i].McdName;
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.DetailInfo = PubHelper.p_BusinOper.AsileOper.AsileList[i].DetailInfo;
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.Unit = PubHelper.p_BusinOper.AsileOper.AsileList[i].Unit;
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.McdPicName = PubHelper.p_BusinOper.AsileOper.AsileList[i].McdPicName;
                        PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.DrugType = PubHelper.p_BusinOper.AsileOper.AsileList[i].DrugType;
                        return true;
                    }
                    else
                    {
                        PubHelper.p_BusinOper.GoodsOper.CurrentGoods = new GoodsModel();
                        PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdCode = PubHelper.p_BusinOper.AsileOper.AsileList[i].McdCode;
                        PubHelper.p_BusinOper.GoodsOper.CurrentGoods.Price = int.Parse(PubHelper.p_BusinOper.AsileOper.AsileList[i].SellPrice);
                        PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdName = PubHelper.p_BusinOper.AsileOper.AsileList[i].McdName;
                        PubHelper.p_BusinOper.GoodsOper.CurrentGoods.DetailInfo = PubHelper.p_BusinOper.AsileOper.AsileList[i].DetailInfo;
                        PubHelper.p_BusinOper.GoodsOper.CurrentGoods.Unit = PubHelper.p_BusinOper.AsileOper.AsileList[i].Unit;
                        PubHelper.p_BusinOper.GoodsOper.CurrentGoods.PicName = PubHelper.p_BusinOper.AsileOper.AsileList[i].McdPicName;
                        PubHelper.p_BusinOper.GoodsOper.CurrentGoods.DrugType = PubHelper.p_BusinOper.AsileOper.AsileList[i].DrugType;
                        return true;
                    }
                }
            }

            //m_CurrentChoiceCode = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdCode;
            //MessageBox.Show("McdCode: " + PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdCode);
            //strCurrCode = strMcdCode;
            //m_PaPrice = Convert.ToInt32(PubHelper.p_BusinOper.GoodsOper.CurrentGoods.Price);
            //strMcdName = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdName;
            //////strMcdContent = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.McdContent;
            //strMcdContent_Detail = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.DetailInfo;
            //strUnit = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.Unit;
            //strMcdPic = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.PicName;
            //strDrupType = PubHelper.p_BusinOper.GoodsOper.CurrentGoods.DrugType;

            return false;
        }

        #endregion

        #endregion
    }
}
