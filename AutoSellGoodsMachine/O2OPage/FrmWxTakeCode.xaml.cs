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

using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmWxTakeCode.xaml 的交互逻辑
    /// </summary>
    public partial class FrmWxTakeCode : Window
    {
        #region 变量声明

        /// <summary>
        /// 微信取货码规定长度，为0表示不检测长度
        /// </summary>
        private int m_CodeLen = 6;

        /// <summary>
        /// 是否关闭窗体
        /// </summary>
        private bool m_CloseForm = false;

        /// <summary>
        /// 读取到的取货码
        /// </summary>
        private string m_TakeCodeNum = string.Empty;

        /// <summary>
        /// 取货方式
        /// </summary>
        private string m_TakeType = string.Empty;

        /// <summary>
        /// 取货货道编号
        /// </summary>
        private string m_TakePaCode = string.Empty;

        private BusinessEnum.ServerReason m_CheckSaleEnvirCode = BusinessEnum.ServerReason.Normal;

        private string m_ErrInfo = string.Empty;

        /// <summary>
        /// 头部文件格式
        /// </summary>
        private BusinessEnum.AdvertType m_TopFileType = BusinessEnum.AdvertType.Image;

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
        /// 无操作超时时间，以秒为单位
        /// </summary>
        private int m_SellOperOutTime = 0;

        /// <summary>
        /// 微信取货码的码名称
        /// </summary>
        private string m_CodeNum_Name = string.Empty;

        #endregion

        #region 窗体

        /// <summary>
        /// 初始化
        /// </summary>
        public FrmWxTakeCode()
        {
            InitializeComponent();
            InitForm();
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmWxTakeCode_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_TopFileType == BusinessEnum.AdvertType.Video)
            {
                videoScreenMediaElement.Stop();
            }

            m_CloseForm = true;
        }

        private void InitForm()
        {
            #region 计算各区域尺寸

            double scrHeight = SystemParameters.PrimaryScreenHeight;
            panelTop.Height = scrHeight * 0.23;
            panelBottom.Height = scrHeight * 0.36;
            panelMiddele.Height = scrHeight * 0.41;
            panelMiddele.Margin = new Thickness(0, panelTop.Height + 1, 0, 0);
            tbProgressInfo.Width = panelMiddele.Width;

            videoScreenMediaElement.Height = panelTop.Height;
            videoScreenMediaElement.Width = panelTop.Width;

            #endregion

            #region 加载各区域图片

            bool result = false;
            string strPicPath = string.Empty;

            string strTopFile = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_TopMv");
            m_TopFileType = PubHelper.p_BusinOper.AdvertOper.GetFileAdvertType(strTopFile);

            if (m_TopFileType == BusinessEnum.AdvertType.Image)
            {
                // 图片文件
                result = PubHelper.GetFormPubPic(strTopFile, out strPicPath);
                if (result)
                {
                    imgTop.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                }
                videoScreenMediaElement.Visibility = System.Windows.Visibility.Hidden;
                imgTop.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                // 视频文件
                imgTop.Visibility = System.Windows.Visibility.Hidden;
                videoScreenMediaElement.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Images\\FormPic\\pub\\" + strTopFile);
                // 临时测试
                videoScreenMediaElement.Position = TimeSpan.FromSeconds(5);
                videoScreenMediaElement.Play();
                videoScreenMediaElement.Visibility = System.Windows.Visibility.Visible;
            }

            result = PubHelper.GetFormPubPic("WxTake_TipBg.png", out strPicPath);
            if (result)
            {
                imgMiddele.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }
            result = PubHelper.GetFormPubPic("WxTake_Swip.png", out strPicPath);
            if (result)
            {
                imgSwipTip.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }
            result = PubHelper.GetFormPubPic(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_BottomMv"), out strPicPath);
            if (result)
            {
                imgBottom.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }

            #endregion

            #region 加载字符资源

            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("SellGoods_Button_Cancel");
            btnCancel.Visibility = System.Windows.Visibility.Hidden;

            m_CodeNum_Name = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_Name_CodeNum");

            #endregion

            #region 设置相关参数

            PubHelper.p_BusinOper.WxTakeCodeOper.VmID = PubHelper.p_BusinOper.ConfigInfo.VmId;
            PubHelper.p_BusinOper.WxTakeCodeOper.ServerURL = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_ServerUrl");
            PubHelper.p_BusinOper.WxTakeCodeOper.UserKey = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_UserKey");

            GetCodeLen();

            #endregion

            #region 启动工作线程

            Thread TrdMainWork = new Thread(new ThreadStart(MainWorkTrd));
            TrdMainWork.IsBackground = true;
            TrdMainWork.Start();

            Thread TrdMonOutTime = new Thread(new ThreadStart(MonOutTimeTrd));
            TrdMonOutTime.IsBackground = true;
            TrdMonOutTime.Start();

            #endregion
        }

        private void GetCodeLen()
        {
            try
            {
                m_CodeLen = Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_CodeLen"));
            }
            catch
            {
                m_CodeLen = 6;
            }
        }

        #endregion

        #region 业务流程

        #region 超时监控

        /// <summary>
        /// 监控超时线程
        /// </summary>
        private void MonOutTimeTrd()
        {
            // 获取允许操作超时时间
            m_SellOperOutTime = PubHelper.p_BusinOper.ConfigInfo.SellOperOutTime;

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
                                    if (m_OperNum > m_SellOperOutTime)
                                    {
                                        // 超时，自动返回
                                        this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                                        {
                                            this.Close();
                                        }));
                                    }
                                    else
                                    {
                                        this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                                        {
                                            tbOutTime.Text = (m_SellOperOutTime - m_OperNum + 1).ToString();
                                            if (tbOutTime.Visibility == System.Windows.Visibility.Hidden)
                                            {
                                                tbOutTime.Visibility = System.Windows.Visibility.Visible;
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
                }
            }
            catch
            {

            }
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

        #endregion

        #region 业务处理线程

        /// <summary>
        /// 业务主线程
        /// </summary>
        private void MainWorkTrd()
        {
            bool result = false;
            bool blnCheckIDCardDevice = false;
            int intErrCode = 0;

            #region 检测条形码扫描设备

            // 初始条形码扫描数据
            PubHelper.p_BusinOper.BarCodeOper.ClearBarCodeNum();

            // 检测条形码设备状态
            intErrCode = PubHelper.p_BusinOper.BarCodeOper.CheckDeviceStatus();
            if (intErrCode == 0)
            {
                // 设备正常，开启条形码扫描
                intErrCode = PubHelper.p_BusinOper.BarCodeOper.BeginScan();
                if (intErrCode == 0)
                {
                    blnCheckIDCardDevice = true;
                    m_ErrInfo = PubHelper.p_LangOper.GetStringBundle("WxTakeCode_Pay_Progress").Replace("{N}", m_CodeNum_Name);
                }
            }
            if (intErrCode != 0)
            {
                // 条形码扫描设备故障，退出线程
                m_ErrInfo = PubHelper.p_LangOper.GetStringBundle("WxTakeCode_Err_Device");
            }

            this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
            {
                tbProgressInfo.Text = m_ErrInfo;
                btnCancel.Visibility = System.Windows.Visibility.Visible;
            }));

            #endregion

            // 开始超时监控
            BeginMonOutTime();

            if (!blnCheckIDCardDevice)
            {
                // 设备故障，退出线程
                return;
            }

            bool blnIsFindBarCode = false;

            while (!m_CloseForm)
            {
                Thread.Sleep(200);

                #region 读取条形码

                blnIsFindBarCode = false;

                result = PostBarCode(out blnIsFindBarCode);

                #endregion

                if (result)
                {
                    #region 检测需要出货的商品是否能够出货

                    /*
                    * 1、检测所出货商品库存是否够
                    * 2、检测出货商品所在货道是否正常
                    */
                    result = CheckSaleEnvr();

                    #endregion

                    if (result)
                    {
                        #region 出货

                        result = SellGoods();

                        #endregion
                    }
                }

                if (blnIsFindBarCode)
                {
                    this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                    {
                        tbProgressInfo.Text = m_ErrInfo;
                        DispatcherHelper.SleepControl();
                    }));
                    Thread.Sleep(2000);
                    this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                    {
                        if (!result)
                        {
                            btnCancel.Visibility = System.Windows.Visibility.Visible;
                            tbProgressInfo.Text = PubHelper.p_LangOper.GetStringBundle("WxTakeCode_Pay_Progress").Replace("{N}", m_CodeNum_Name);
                            DispatcherHelper.SleepControl();
                            BeginMonOutTime();// 重新开始超时监控
                        }
                        else
                        {
                            this.Close();
                        }
                    }));
                }
            }

            intErrCode = PubHelper.p_BusinOper.BarCodeOper.StopScan();
        }

        /// <summary>
        /// 执行条形码扫描业务（2015-08-10）
        /// </summary>
        private bool PostBarCode(out bool isFindBarCode)
        {
            bool result = false;
            isFindBarCode = false;
            string strBarCodeData = string.Empty;
            string strErrCode = string.Empty;

            try
            {
                int intErrCode = PubHelper.p_BusinOper.BarCodeOper.QueryBarCodeNum(out strBarCodeData, out strErrCode);

                if ((intErrCode == 0) && (!string.IsNullOrEmpty(strBarCodeData)))
                {
                    if ((m_CodeLen == 0) || ((m_CodeLen > 0) && (strBarCodeData.Length == m_CodeLen)))
                    {
                        isFindBarCode = true;

                        #region 寻到取货码

                        // 停止超时监控
                        StopMonOutTime();

                        m_TakeCodeNum = strBarCodeData;
                        this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                        {
                            // 正在验证信息...
                            tbProgressInfo.Text = PubHelper.p_LangOper.GetStringBundle("WxTakeCode_Pay_Checking").Replace("{N}", m_CodeNum_Name);
                        }));

                        #region 提交请求

                        string _orderInfo = string.Empty;// 取货码|取货码类型|条码来源|价值金额|取货方式|取货货道号
                        intErrCode = PubHelper.p_BusinOper.WxTakeCodeOper.CheckTakeCodeInfo(strBarCodeData, out _orderInfo);

                        switch (intErrCode)
                        {
                            case 0:// 提交成功
                                #region 解析相关数据

                                string[] hexOrderData = _orderInfo.Split('|');
                                if (hexOrderData.Length > 4)
                                {
                                    result = true;
                                    m_TakeType = hexOrderData[4];// 取货方式 0：用户自主选货 1：指定货道出货 2：随机出货
                                    m_TakePaCode = hexOrderData[5];// 取货货道号
                                    if ((m_TakeType == "1") || (m_TakeType == "3"))
                                    {
                                        // 如果是指定货道出货或者指定商品出货
                                        if (string.IsNullOrEmpty(m_TakePaCode))
                                        {
                                            intErrCode = 99;
                                        }
                                    }
                                }
                                else
                                {
                                    intErrCode = 99;
                                }

                                #endregion
                                break;
                        }

                        #endregion

                        if (intErrCode != 0)
                        {
                            m_ErrInfo = DictionaryHelper.Dictionary_WxTakeCode(intErrCode, m_CodeNum_Name);
                        }

                        #endregion
                    }
                }
            }
            catch
            {
                isFindBarCode = false;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 检测是否允许出货
        /// </summary>
        private bool CheckSaleEnvr()
        {
            int intErrCode = 0;

            bool result = false;

            try
            {
                this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                {
                    // 正在检测设备状态...
                    tbProgressInfo.Text = PubHelper.p_LangOper.GetStringBundle("Check_Device");
                }));

                #region 检测是否允许出货

                m_CheckSaleEnvirCode = BusinessEnum.ServerReason.Normal;

                m_CheckSaleEnvirCode = PubHelper.p_BusinOper.Device_CheckSaleEnvir_WxTakeCode(m_TakeCodeNum, m_TakeType,m_TakePaCode);

                #endregion

                #region 上报准备出货

                string strSellStatus = string.Empty;
                string strNoSellReason = string.Empty;
                if (m_CheckSaleEnvirCode == BusinessEnum.ServerReason.Normal)
                {
                    strSellStatus = "0";// 可吐货
                }
                else
                {
                    strSellStatus = "1";// 不能吐货
                    switch (m_CheckSaleEnvirCode)
                    {
                        case BusinessEnum.ServerReason.Err_NoStock:// 库存不足
                            strNoSellReason = "1";
                            m_ErrInfo = "";
                            break;
                        case BusinessEnum.ServerReason.Err_GoodsExist:// 商品不存在
                            strNoSellReason = "0";
                            m_ErrInfo = "";
                            break;
                        case BusinessEnum.ServerReason.Err_AsileStatus:// 货到故障
                            strNoSellReason = "2";
                            m_ErrInfo = "";
                            break;
                        default:
                            strNoSellReason = "3";
                            m_ErrInfo = "";
                            break;
                    }
                }

                intErrCode = PubHelper.p_BusinOper.WxTakeCodeOper.ConfirmSellInfo(m_TakeCodeNum, strSellStatus, strNoSellReason);

                bool blnIsSell = false;
                if (m_CheckSaleEnvirCode == BusinessEnum.ServerReason.Normal)
                {
                    // 如果检测可以购买，但是提交吐货请求时出现故障，仍然暂停服务
                    if (intErrCode != 0)
                    {
                        m_ErrInfo = DictionaryHelper.Dictionary_WxTakeCode(intErrCode, m_CodeNum_Name);
                    }
                    else
                    {
                        blnIsSell = true;
                    }
                }

                result = blnIsSell;

                #endregion
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 执行出货
        /// </summary>
        private bool SellGoods()
        {
            int intErrCode = 0;
            bool result = false;

            string strPaCode = PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo.PaCode;

            this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
            {
                // 正在出货...
                tbProgressInfo.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Progress");
            }));

            int intPaIndex = 0;

            // 开始执行处货
            intErrCode = PubHelper.p_BusinOper.SellGoods_Begin(strPaCode, out intPaIndex);

            if (intErrCode == 0)
            {
                // 查询出货结果
                intErrCode = PubHelper.p_BusinOper.SellGoods_Query(intPaIndex);
            }

            // 0：成功 1：无该货道 2：控制主板可用金额不足 3：无货 9：失败
            string strSellStatus = "0";
            string strNoSellReason = string.Empty;

            switch (intErrCode)
            {
                case 0:// 成功
                    m_ErrInfo = PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Suc");// "出货成功";
                    m_ErrInfo = m_ErrInfo.Replace("{N}", strPaCode);
                    strSellStatus = "1";
                    result = true;
                    break;

                case 3:// 无货
                    m_ErrInfo = PubHelper.p_LangOper.GetStringBundle("Err_SellGoods_NoGoods");// "已售完";
                    strNoSellReason = "1";
                    break;

                default:
                    m_ErrInfo = PubHelper.p_LangOper.GetStringBundle("SellGoods_Shipping_Fail");// "出货失败";
                    strNoSellReason = "3";
                    break;
            }

            return result;
        }

        #endregion

        #endregion

        #region 控件操作

        private void ControlForm()
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //当完成媒体加载时发生
        private void videoScreenMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {

        }

        private void videoScreenMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                videoScreenMediaElement.Position = TimeSpan.Zero;
                videoScreenMediaElement.Play();
            }
            catch
            {
            }
        }

        #endregion
    }
}
