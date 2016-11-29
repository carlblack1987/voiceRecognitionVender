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
    /// FrmShZyZ_Donate.xaml 的交互逻辑
    /// </summary>
    public partial class FrmShZyZ_Donate : Window
    {
        #region 变量声明

        /// <summary>
        /// 当前步骤 0：等待选择支付方式 1：选择支付方式 2：提交捐赠
        /// </summary>
        private string m_NowStep = "0";

        /// <summary>
        /// 扫描订单号规定长度，为0表示不检测长度
        /// </summary>
        private int m_CodeLen = 21;

        /// <summary>
        /// 是否关闭窗体
        /// </summary>
        private bool m_CloseForm = false;

        /// <summary>
        /// 条形码扫描内容
        /// </summary>
        private string m_BarCode = string.Empty;

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
        /// 捐赠金额
        /// </summary>
        private int m_TotalMoney = 0;

        /// <summary>
        /// 捐赠支付方式 1：现金 2：支付宝
        /// </summary>
        private string m_DonType = "1";

        /// <summary>
        /// 捐赠者手机号码
        /// </summary>
        private string m_MobilePhone = string.Empty;

        #endregion

        #region 窗体

        public FrmShZyZ_Donate()
        {
            InitializeComponent();
            InitForm();
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmShZyZ_Donate_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_CloseForm = true;
        }

        private void InitForm()
        {
            #region 计算各区域尺寸

            double scrHeight = SystemParameters.PrimaryScreenHeight;
            panelTop.Height = scrHeight * 0.33;
            panelBottom.Height = scrHeight * 0.1;
            panelMiddele.Height = scrHeight * 0.57;
            panelMiddele.Margin = new Thickness(0, panelTop.Height + 1, 0, 0);
            tbProgressInfo.Width = panelMiddele.Width;

            #endregion

            #region 加载各区域图片

            bool result = false;
            string strPicPath = string.Empty;

            string strTopFile = "ShZyZ_Top_Donate.png";
            m_TopFileType = PubHelper.p_BusinOper.AdvertOper.GetFileAdvertType(strTopFile);

            if (m_TopFileType == BusinessEnum.AdvertType.Image)
            {
                // 图片文件
                result = PubHelper.GetFormPubPic(strTopFile, out strPicPath);
                if (result)
                {
                    imgTop.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                }

                imgTop.Visibility = System.Windows.Visibility.Visible;
            }

            string strTipBg = "ShZyZ_Donate_TipBg.png";
            if (PubHelper.p_ShZyZ_Donate_PayType == BusinessEnum.PayMent.AliPay_Code)
            {
                strTipBg = "ShZyZ_Donate_TipBg_AliPay.png";
            }
            result = PubHelper.GetFormPubPic(strTipBg, out strPicPath);
            if (result)
            {
                imgMiddele.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }

            #endregion

            #region 加载字符资源

            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("SellGoods_Button_Cancel");
            btnCancel.Visibility = System.Windows.Visibility.Hidden;

            #endregion

            btnOK.IsEnabled = false;

            #region 设置相关参数

            PubHelper.p_BusinOper.O2OServerOper.VmID = PubHelper.p_BusinOper.ConfigInfo.VmId;
            PubHelper.p_BusinOper.O2OServerOper.ServerURL = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("O2OServerUrl");
            PubHelper.p_BusinOper.O2OServerOper.UserKey = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("O2OServer_UserKey");
            m_NowStep = "0";

            #endregion

            #region 启动工作线程

            switch (PubHelper.p_ShZyZ_Donate_PayType)
            {
                case BusinessEnum.PayMent.Cash:// 现金支付方式
                    m_DonType = "1";
                    btnOK.Visibility = System.Windows.Visibility.Visible;
                    sPanelMoneyNum.Visibility = System.Windows.Visibility.Hidden;
                    imgShZy_AliPay_QrCode.Visibility = System.Windows.Visibility.Hidden;
                    Thread TrdMainWork_Cash = new Thread(new ThreadStart(MainWorkDonTrd_Cash));
                    TrdMainWork_Cash.IsBackground = true;
                    TrdMainWork_Cash.Start();
                    break;
                case BusinessEnum.PayMent.AliPay_Code:// 支付宝支付方式
                    m_DonType = "2";
                    btnOK.Visibility = System.Windows.Visibility.Hidden;
                    btnCancel.Visibility = System.Windows.Visibility.Visible;
                    imgShZy_AliPay_QrCode.Visibility = System.Windows.Visibility.Visible;
                    sPanelMoneyNum.Visibility = System.Windows.Visibility.Hidden;
                    tbPhone_Value.Visibility = System.Windows.Visibility.Hidden;
                    tbPhone_Title.Visibility = System.Windows.Visibility.Hidden;
                    tbTipInfo_SerName.Visibility = System.Windows.Visibility.Hidden;
                    tbMoney_Value.Visibility = System.Windows.Visibility.Hidden;
                    tbMoney_Title.Visibility = System.Windows.Visibility.Hidden;
                    tbProgressInfo.Foreground = Brushes.Orange;
                    tbProgressInfo.Text = "请用支付宝“扫一扫”扫描二维码进行捐赠";
                    ////Thread TrdMainWork_AliPay = new Thread(new ThreadStart(MainWorkDonTrd_AliPay));
                    ////TrdMainWork_AliPay.IsBackground = true;
                    ////TrdMainWork_AliPay.Start();
                    // 开始超时监控
                    BeginMonOutTime();
                    break;
            }

            Thread TrdMonOutTime = new Thread(new ThreadStart(MonOutTimeTrd));
            TrdMonOutTime.IsBackground = true;
            TrdMonOutTime.Start();

            #endregion
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

        #region 现金捐赠方式业务

        private void MainWorkDonTrd_Cash()
        {
            int intErrCode = 0;
            bool blnCheckIDCardDevice = false;

            #region 检测纸币器、硬币器状态

            intErrCode = PubHelper.p_BusinOper.BarCodeOper.StopScan();
            intErrCode = PubHelper.p_BusinOper.ControlCash("1", true);
            intErrCode = PubHelper.p_BusinOper.ControlCoin("1", true);

            if (intErrCode != 0)
            {
                // 设备故障，退出线程
                m_ErrInfo = "现金接收设备故障，暂停服务";
            }
            if (intErrCode == 0)
            {
                blnCheckIDCardDevice = true;
                m_ErrInfo = "请投纸币或硬币";
            }

            this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
            {
                tbProgressInfo.Text = m_ErrInfo;
                if (intErrCode == 0)
                {
                    ControlPhone(true);
                }
                else
                {
                    ControlPhone(false);
                }

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

            while (!m_CloseForm)
            {
                Thread.Sleep(200);

                if (PubHelper.p_ShZyZ_Donate_PayType == BusinessEnum.PayMent.Cash)
                {
                    QueryMoney();
                }
                if ((m_NowStep == "2") && (m_TotalMoney > 0))
                {
                    StopMonOutTime();
                    this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                    {
                        tbProgressInfo.Text = "正在提交捐赠信息...";
                        btnCancel.IsEnabled = btnOK.IsEnabled = tbPhone_Value.IsEnabled = false;
                    }));

                    // 向志愿者激励平台提交捐赠信息
                    intErrCode = PubHelper.p_BusinOper.O2OServerOper.Report_DonData(m_TotalMoney.ToString(), m_DonType, m_MobilePhone, "", "");
                    // 向售货机运营平台提交现金捐赠信息

                    // 清除当前控制主板显示金额
                    intErrCode = PubHelper.p_BusinOper.ClearUsableMoney(true);

                    intErrCode = PubHelper.p_BusinOper.ControlCash("0", true);
                    intErrCode = PubHelper.p_BusinOper.ControlCoin("0", true);

                    this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                    {
                        tbProgressInfo.Text = "您捐赠【" + (m_TotalMoney / 100).ToString() + "】元，感谢您助力志愿";
                    }));

                    Thread.Sleep(5000);

                    this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Close();
                    }));
                }
            }
        }

        private void QueryMoney()
        {
            int money = 0;
            try
            {
                string strValue = string.Empty;
                int intErrCode = PubHelper.p_BusinOper.QueryMoney_AddBill(out strValue);
                if (intErrCode == 0)
                {
                    #region 如果查询成功，解析数据
                    // 纸币器状态|硬币器状态|可接收货币标识|货币类型|金额
                    string[] hexValue = strValue.Split('|');
                    if (hexValue.Length > 4)
                    {
                        // 检测是否存在找零纸币投入
                        if (hexValue[1] == "03")
                        {
                            #region 硬件退币按钮被触发

                            #endregion
                        }
                        else
                        {
                            #region 存在金额投入

                            if (hexValue[3] != "FF")
                            {
                                money = Convert.ToInt32(hexValue[4]);
                                if (money > 0)
                                {
                                    StopMonOutTime();
                                    m_TotalMoney += money;
                                    this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                                    {
                                        tbMoney_Value.Text = (m_TotalMoney / 100).ToString() + "元";
                                        if (m_TotalMoney > 0)
                                        {
                                            btnOK.IsEnabled = true;
                                        }
                                        else
                                        {
                                            btnOK.IsEnabled = false;
                                        }
                                    }));
                                }
                            }

                            #endregion
                        }
                    }

                    #endregion
                }
            }
            catch
            {

            }
        }

        #endregion

        #region 支付宝捐赠方式业务

        private void MainWorkDonTrd_AliPay()
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
                    m_ErrInfo = "选择金额后，请扫描您的支付宝付款码";
                }
            }
            if (intErrCode != 0)
            {
                // 条形码扫描设备故障，退出线程
                m_ErrInfo = "扫描设备故障，暂停服务";
            }

            this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
            {
                tbProgressInfo.Text = m_ErrInfo;
                if (intErrCode == 0)
                {
                    ControlPhone(true);
                    ControlBtnNum(true);
                }
                else
                {
                    ControlPhone(false);
                    ControlBtnNum(false);
                }
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

            while (!m_CloseForm)
            {
                Thread.Sleep(200);

                if (PubHelper.p_ShZyZ_Donate_PayType == BusinessEnum.PayMent.AliPay_Code)
                {
                    PostBarCode();
                }
            }

        }

        private void PostBarCode()
        {
            string strBarCodeData = string.Empty;
            string strErrCode = string.Empty;
            bool blnIsPost = false;// 是否提交支付宝支付信息

            try
            {
                int intErrCode = PubHelper.p_BusinOper.BarCodeOper.QueryBarCodeNum(out strBarCodeData, out strErrCode);

                if ((intErrCode == 0) && (!string.IsNullOrEmpty(strBarCodeData)))
                {
                    if ((strBarCodeData.Length == 18) && (strBarCodeData.Substring(0, 2) == "28"))
                    {
                        if (m_TotalMoney > 0)
                        {
                            blnIsPost = true;
                        }
                    }
                }

                if (blnIsPost)
                {
                    #region 提交支付信息

                    PubHelper.p_BusinOper.BarCodeOper.StopScan();
                    // 停止超时监控
                    StopMonOutTime();

                    m_BarCode = strBarCodeData;
                    this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                    {
                        // 正在验证信息...
                        tbProgressInfo.Text = "正在提交支付信息...";
                        btnCancel.IsEnabled = false;
                        ControlBtnNum(false);
                        tbMoney_Value.IsEnabled = tbPhone_Value.IsEnabled = false;
                    }));

                    #region 提交请求

                    bool blnPayResult = false;// 支付结果 False：失败 True：成功
                    bool blnIsQuery = false;// 是否发送查询请求 False：否 True：是

                    int _resultCode = 0;
                    string _orderNo = string.Empty;
                    intErrCode = PubHelper.p_BusinOper.ShZyZ_BarCode_Don_Pay(m_BarCode, "A1", m_TotalMoney, out _resultCode, out _orderNo);

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
                            this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                            {
                                tbProgressInfo.Text = "正在查询支付结果...";
                            }));

                            blnIsQuery = true;
                        }
                    }

                    if (blnIsQuery)
                    {
                        #region 提交查询请求

                        intErrCode = PubHelper.p_BusinOper.ShZyZ_BarCode_Don_Query(_orderNo, out _resultCode);
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
                            // 向志愿者激励平台提交捐赠信息
                            intErrCode = PubHelper.p_BusinOper.O2OServerOper.Report_DonData(m_TotalMoney.ToString(), m_DonType, m_MobilePhone, "", m_BarCode);

                            this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                            {
                                tbProgressInfo.Text = "您捐赠【" + (m_TotalMoney / 100).ToString() + "】元，感谢您的爱心";
                            }));
                            break;
                        default:// 失败
                            this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                            {
                                tbProgressInfo.Text = "捐赠失败，请稍候重试";
                            }));
                            break;
                    }

                    Thread.Sleep(5000);
                    PubHelper.p_BusinOper.BarCodeOper.ClearBarCodeNum();
                    if (blnPayResult)
                    {
                        this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                        {
                            this.Close();
                        }));
                    }
                    else
                    {
                        this.tbProgressInfo.Dispatcher.Invoke(new Action(() =>
                        {
                            tbProgressInfo.Text = "选择金额后，请扫描您的支付宝付款码";
                            ControlBtnNum(true);
                            btnCancel.IsEnabled = true;

                            tbMoney_Value.IsEnabled = tbPhone_Value.IsEnabled = true;
                        }));
                        
                        PubHelper.p_BusinOper.BarCodeOper.BeginScan();
                        BeginMonOutTime();
                    }

                    #endregion

                    #endregion
                }
            }
            catch
            {
            }
        }

        #endregion

        #endregion

        #endregion

        #region 控件操作

        private void ControlPhone(bool enable)
        {
            if (enable)
            {
                tbPhone_Value.Background = Brushes.White;
            }
            else
            {
                tbPhone_Value.Background = Brushes.Gray;
            }
            tbPhone_Value.IsEnabled = enable;
        }

        private void ControlBtnNum(bool enable)
        {
            btn1.IsEnabled = btn2.IsEnabled = btn3.IsEnabled = btn4.IsEnabled = btn5.IsEnabled =
                btn6.IsEnabled = btn7.IsEnabled = btn8.IsEnabled = btn9.IsEnabled =
                enable;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            string strInput = DictionaryHelper.Dictionary_Input(sender.ToString());
            tbMoney_Value.Text = strInput;
            try
            {
                strInput = strInput.Replace("元", "");
                if (!string.IsNullOrEmpty(strInput))
                {
                    m_TotalMoney = Convert.ToInt32(strInput) * 100;
                }
            }
            catch
            {
                m_TotalMoney = 0;
            }
        }

        private void btnOtherMoney_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            PubHelper.p_ShZyZ_KeyBoard_Type = "1";
            PubHelper.p_Keyboard_Input = string.Empty;
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmKeyBoard_Num frmKey = new FrmKeyBoard_Num();
            frmKey.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            if (!string.IsNullOrEmpty(PubHelper.p_Keyboard_Input))
            {
                tbMoney_Value.Text = Convert.ToInt32(PubHelper.p_Keyboard_Input).ToString() + "元";
                try
                {
                    m_TotalMoney = Convert.ToInt32(PubHelper.p_Keyboard_Input) * 100;
                }
                catch
                {
                    m_TotalMoney = 0;
                }
            }
            BeginMonOutTime();
        }

        private void tbPhone_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StopMonOutTime();
            PubHelper.p_ShZyZ_KeyBoard_Type = "0";
            PubHelper.p_Keyboard_Input = tbPhone_Value.Text;
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmKeyBoard_Num frmKey = new FrmKeyBoard_Num();
            frmKey.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            tbPhone_Value.Text = PubHelper.p_Keyboard_Input;
            m_MobilePhone = PubHelper.p_Keyboard_Input;
            
            BeginMonOutTime();
        }

        #endregion

        /// <summary>
        /// 现金中提交捐赠信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckMobilePhone())
            {
                PubHelper.ShowMsgInfo("请输入有效的手机号码", PubHelper.MsgType.Ok);
                return;
            }

            StopMonOutTime();
            m_NowStep = "2";
        }

        /// <summary>
        /// 检测手机号码是否有效
        /// </summary>
        /// <returns></returns>
        private bool CheckMobilePhone()
        {
            if (string.IsNullOrEmpty(m_MobilePhone))
            {
                return true;
            }
            return PubHelper.p_BusinOper.CheckDataOper.CheckPhoneNum(m_MobilePhone);
        }
    }
}
