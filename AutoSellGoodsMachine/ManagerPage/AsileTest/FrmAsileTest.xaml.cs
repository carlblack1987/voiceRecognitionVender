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
using System.Windows.Threading;

using AutoSellGoodsMachine.Controls;
using Business.Model;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmAsileTest.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAsileTest : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// 当前的货柜编号
        /// </summary>
        private string m_VendBoxCode = "1";

        private GoodsWayProduct currentGoodsWay = null;
        private int currentTrayIndex = 0;
        private int m_TestTotalNum = 0;
        private int m_AsileCount = 0;
        private int m_TrayCount = 0;
        private string m_NoGoodsTitle = string.Empty;
        private string m_Testing = string.Empty;
        private string m_Title_AsileNormal = string.Empty;
        private string m_Title_AsileFail = string.Empty;
        private string m_Title_AsileFail_Code = string.Empty;

        private string m_Title_SingleTest = string.Empty;
        private string m_Title_TrayTest = string.Empty;
        private string m_Title_WholeTest = string.Empty;

        /// <summary>
        /// 测试方式 0：单货道测试  1：托盘测试 2：整机测试
        /// </summary>
        private string m_TestType = "0";

        /// <summary>
        /// 当前测试货道编号、托盘
        /// </summary>
        private string m_TestCode = string.Empty;

        /// <summary>
        /// 线程操作是否正在进行中 False：没有 True：正在进行
        /// </summary>
        private bool m_IsTrdOper = false;

        /// <summary>
        /// 是否停止测试 False：不停止 True：停止
        /// </summary>
        public bool m_IsStopTest = false;

        /// <summary>
        /// 是否是循环测试 False：否 True：是
        /// </summary>
        public bool m_IsLoopTest = false;

        public FrmAsileTest()
        {
            InitializeComponent();
            InitForm();
            
            Loaded += (FrmAsileTest_Loaded);
        }

        private void FrmAsileTest_Loaded(object sender, RoutedEventArgs e)
        {
            //Tick是执行的事件           
            timer.Tick += new EventHandler(Timer1_Tick);
            //Internal是间隔时间
            timer.Interval = TimeSpan.FromSeconds(0.01);

            timer.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            m_VendBoxCode = "1";// 默认第一个柜子
            CreateBox();//
            ////CreateTray();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnAsileTest_Begin.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_TrayTest");
            btnWholeTest_Begin.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_WholeTest");

            btnWholeTest_Stop.Content = btnAsileTest_Stop.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_StopTest");

            btnViewResult.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_ViewResult");

            tbAsileTest.Text = m_Title_SingleTest = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_SingleTest");
            m_Title_TrayTest = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_TrayTest");
            m_Title_WholeTest = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_WholeTest");

            tbAsile_Normal.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_Normal");
            tbAsile_Testing.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_Testing");

            //tbWholeTest.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_WholeTest");
            tbNowAsile.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_NowAsile");
            tbTestNum.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_TestNum");

            m_NoGoodsTitle = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_NoGoods");
            m_Testing = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_Testing");
            m_Title_AsileNormal = PubHelper.p_LangOper.GetStringBundle("Pub_Normal");
            m_Title_AsileFail = PubHelper.p_LangOper.GetStringBundle("Pub_Error");
            m_Title_AsileFail_Code = PubHelper.p_LangOper.GetStringBundle("Pub_Error_Code");

            btnWholeTest_Stop.IsEnabled = btnAsileTest_Stop.IsEnabled = false;

            m_TestTotalNum = 0;
            tbNowAsileCode.Text = string.Empty;
            tbTestTotalNum.Text = m_TestTotalNum.ToString();
        }

        /////// <summary>
        /////// 根据分辨率设置大小(该代码在这里为了在电脑上显示效果美化 实际终端请去掉)
        /////// </summary>
        ////private void SetSize()
        ////{
        ////    mainPanel.Width = 700;// this.ActualWidth * 0.9;
        ////}

        /// <summary>
        /// 创建货柜
        /// </summary>
        private void CreateBox()
        {
            ////string strVendBoxTitle = PubHelper.p_LangOper.GetStringBundle("Pub_VendBox");
            var map = new Dictionary<int, string>();
            for (int i = 1; i < 10; i++)
            {
                map.Add(i, DictionaryHelper.Dictionary_VendBoxName(i.ToString()));
            }

            int intBoxCount = PubHelper.p_BusinOper.AsileOper.VendBoxList.Count;
            if (intBoxCount > 1)
            {
                #region 有多个柜子
                for (int i = 1; i <= intBoxCount; i++)
                {
                    panelBox.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    var palletButton = new RadioButton()
                    {
                        Focusable = false,

                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,

                        VerticalAlignment = System.Windows.VerticalAlignment.Center,

                        Style = App.Current.Resources["VendBoxButtonStyle"] as Style,

                        Foreground = new SolidColorBrush(Colors.White),

                        Padding = new Thickness(20, 4, 20, 8),

                        FontSize = 22,

                        Tag = i
                    };

                    Grid.SetColumn(palletButton, i - 1);

                    palletButton.Checked += (VendBoxButtonChecked);

                    palletButton.Content = map[i];

                    panelBox.Children.Add(palletButton);

                    if (i == 1)
                    {
                        palletButton.IsChecked = true;
                    }
                }
                #endregion
            }
            else
            {
                // 只有一个柜子
                CreateTray();
            }
        }

        /// <summary>
        /// 创建托盘
        /// </summary>
        private void CreateTray()
        {
            var map = new Dictionary<int, string>();
            for (int i = 1; i < 11; i++)
            {
                map.Add(i, PubHelper.p_BusinOper.ChangeTray(i.ToString()));
            }

            int intVendTrayMaxNum = 0;
            if (PubHelper.p_BusinOper.AsileOper.VendBoxList.Count == 1)
            {
                // 只有一个柜子
                intVendTrayMaxNum = PubHelper.p_BusinOper.AsileOper.TrayMaxNum_Total;
            }
            else
            {
                // 多个柜子
                for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.AsileList.Count; i++)
                {
                    if (PubHelper.p_BusinOper.AsileOper.AsileList[i].VendBoxCode == m_VendBoxCode)
                    {
                        if (PubHelper.p_BusinOper.AsileOper.AsileList[i].TrayIndex >= intVendTrayMaxNum)
                        {
                            intVendTrayMaxNum = PubHelper.p_BusinOper.AsileOper.AsileList[i].TrayIndex;
                        }
                    }
                }
            }

            panelTray.ColumnDefinitions.Clear();
            panelTray.Children.Clear();

            for (int i = 1; i <= intVendTrayMaxNum + 1; i++)
            {
                panelTray.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                var palletButton = new RadioButton()
                {
                    Focusable = false,

                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,

                    VerticalAlignment = System.Windows.VerticalAlignment.Center,

                    Style = App.Current.Resources["PalletButtonStyle"] as Style,

                    Foreground = new SolidColorBrush(Colors.White),

                    Padding = new Thickness(20, 8, 20, 8),

                    FontSize = 32,

                    Tag = i - 1
                };

                Grid.SetColumn(palletButton, i - 1);

                palletButton.Checked += (PalletButtonChecked);

                palletButton.Content = map[i];

                panelTray.Children.Add(palletButton);

                if (i == 1)
                {
                    palletButton.IsChecked = true;
                }
            }
            
            // 如果是复杂型升降机方式测试，则不能进行托盘和整机测试
            if (PubHelper.p_BusinOper.AsileOper.VendBoxList[PubHelper.p_BusinOper.AsileOper.GetVendBoxIndex(m_VendBoxCode)].SellGoodsType == Business.Enum.BusinessEnum.SellGoodsType.Lifter_Comp)
            {
                btnAsileTest_Begin.Visibility = btnWholeTest_Begin.Visibility =
                    btnAsileTest_Stop.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                btnAsileTest_Begin.Visibility = btnWholeTest_Begin.Visibility =
                    btnAsileTest_Stop.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// 创建货道
        /// </summary>
        private void CreateAsile(List<AsileModel> products)
        {
            panelAsile.Children.Clear();
            
            int index = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GoodsWayProduct productControl = new GoodsWayProduct()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                        Margin = new Thickness(5)
                    };
                    productControl.SetBackBackGroud(App.Current.Resources["AsileNormal"] as Brush);
                    productControl.SetCheckBackGround(App.Current.Resources["AsileTest"] as Brush);
                    Grid.SetRow(productControl, i);
                    Grid.SetColumn(productControl, j);
                    panelAsile.Children.Add(productControl);

                    productControl.MouseLeftButtonUp += (GoodsWayChecked);

                    if (index < products.Count)
                    {
                        productControl.SetCurrentGoodsWayProduct(products[index]);
                        productControl.SetOneText(products[index].PaCode);

                        productControl.SetSecondText(GetAsileStatus(products[index].PaStatus));
                    }
                    else
                    {
                        productControl.SetNoGoods(m_NoGoodsTitle);
                    }
                    index++;
                }
            }

            m_AsileCount = panelAsile.Children.Count;

            if (!m_IsTrdOper)
            {
                if (products.Count == 0)
                {
                    btnAsileTest_Begin.IsEnabled = btnAsileTest_Stop.IsEnabled = false;
                }
                else
                {
                    btnAsileTest_Begin.IsEnabled = true;
                    btnAsileTest_Stop.IsEnabled = false;
                }

            }
        }

        private string GetAsileStatus(string status)
        {
            string strStatus = string.Empty;
            switch (status)
            {
                case "02":// 正常
                    strStatus = m_Title_AsileNormal;
                    break;
                default:// 故障
                    strStatus = m_Title_AsileFail;
                    break;
            }
            return strStatus;
        }

        /// <summary>
        /// 货柜选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VendBoxButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                currentTrayIndex = Convert.ToInt32((sender as RadioButton).Tag);
                m_VendBoxCode = currentTrayIndex.ToString();
                CreateTray();
            }
        }

        /// <summary>
        /// 托盘选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PalletButtonChecked(object sender, RoutedEventArgs e)
        {
            if (m_TestType != "2")
            {
                if (CheckIsTesting())
                {
                    // 如果正在测试
                    return;
                }
            }
            if (sender is RadioButton)
            {
                currentTrayIndex = Convert.ToInt32((sender as RadioButton).Tag);
                var currentRowProducts = PubHelper.p_BusinOper.AsileOper.AsileList.Where(item => item.TrayIndex == currentTrayIndex).ToList();
                currentRowProducts = currentRowProducts.Where(item => item.VendBoxCode == m_VendBoxCode).ToList();
                CreateAsile(currentRowProducts);
            }
        }

        /// <summary>
        /// 货道中的产品选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoodsWayChecked(object sender, MouseButtonEventArgs e)
        {
            if (CheckIsTesting())
            {
                return;
            }

            var goodsWay = (sender as GoodsWayProduct);
            if (goodsWay == currentGoodsWay)
                return;
            if (goodsWay != null)
            {
                if (currentGoodsWay != null)
                {
                    currentGoodsWay.ToNormal();
                }
                goodsWay.ToCheck();
                currentGoodsWay = goodsWay;
            }
            if (currentGoodsWay.CurrentGoodsWayProduct != null)
            {
                ////ResetProductInventory(currentGoodsWay.CurrentGoodsWayProduct.SurNum);
                ////btnSave.IsEnabled = true;
                // 开始单货道测试
                m_TestType = "0";
                tbAsileTest.Text = m_Title_SingleTest;
                m_TestCode = currentGoodsWay.CurrentGoodsWayProduct.PaCode;
                tbNowAsileCode.Text = m_TestCode;
                Thread TrdAsileTest = new Thread(new ThreadStart(AsileTestTrd));
                TrdAsileTest.IsBackground = true;
                TrdAsileTest.Start();
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 托盘开始测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAsileTest_Begin_Click(object sender, RoutedEventArgs e)
        {
            m_TestType = "1";
            ////m_TestCode = currentGoodsWay.CurrentGoodsWayProduct.PaCode;
            tbAsileTest.Text = m_Title_TrayTest;

            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_CircleAsk");

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);

            m_IsLoopTest = PubHelper.p_MsgResult;
            
            m_TrayCount = 1;
            m_AsileCount = panelAsile.Children.Count;

            m_IsStopTest = false;

            Thread TrdAsileTest = new Thread(new ThreadStart(AsileTestTrd));
            TrdAsileTest.IsBackground = true;
            TrdAsileTest.Start();
        }

        /// <summary>
        /// 托盘停止测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAsileTest_Stop_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_StopAsk");

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);

            if (PubHelper.p_MsgResult)
            {
                btnAsileTest_Stop.IsEnabled = false;
                m_IsStopTest = true;
            }
            ////btnAsileTest_Begin.IsEnabled = btnWholeTest_Begin.IsEnabled = true;
            ////btnAsileTest_Stop.IsEnabled = btnWholeTest_Stop.IsEnabled = false;
        }

        /// <summary>
        /// 整机开始测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWholeTest_Begin_Click(object sender, RoutedEventArgs e)
        {
            m_TestType = "2";
            //m_TestCode = currentGoodsWay.CurrentGoodsWayProduct.PaCode;
            tbAsileTest.Text = m_Title_WholeTest;

            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_CircleAsk");

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);

            m_IsLoopTest = PubHelper.p_MsgResult;
            
            m_TrayCount = panelTray.Children.Count;

            m_IsStopTest = false;

            Thread TrdAsileTest = new Thread(new ThreadStart(AsileTestTrd));
            TrdAsileTest.IsBackground = true;
            TrdAsileTest.Start();
        }

        /// <summary>
        /// 整机停止测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWholeTest_Stop_Click(object sender, RoutedEventArgs e)
        {
            m_IsStopTest = true;
        }

        /// <summary>
        /// 是否正在测试
        /// </summary>
        /// <returns>结果 True：正在测试 False：没有测试</returns>
        private bool CheckIsTesting()
        {
            if (m_IsTrdOper)
            {
                // 正在测试
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest_Err_Test"), PubHelper.MsgType.Ok);
                return true;
            }
            return false;
        }

        private void ControlForm(bool enable)
        {
            panelAsile.IsEnabled = enable;
            panelTray.IsEnabled = enable;
            btnCancel.IsEnabled = enable;
            btnViewResult.IsEnabled = enable;

            switch (m_TestType)
            {
                case "0":// 单货道测试
                    if (!enable)
                    {
                        btnAsileTest_Begin.IsEnabled = btnAsileTest_Stop.IsEnabled =
                            btnWholeTest_Begin.IsEnabled = enable;
                        currentGoodsWay.SetSecondText(m_Testing);
                    }
                    else
                    {
                        btnAsileTest_Begin.IsEnabled = btnWholeTest_Begin.IsEnabled = true;
                        btnAsileTest_Stop.IsEnabled = false;
                    }
                    
                    break;

                //case "1":// 托盘测试
                //    btnAsileTest_Stop.IsEnabled = !enable;
                //    btnAsileTest_Begin.IsEnabled = btnWholeTest_Begin.IsEnabled = enable;
                //    break;

                default:// 托盘测试或整机测试
                    btnAsileTest_Stop.IsEnabled = !enable;
                    btnAsileTest_Begin.IsEnabled = btnWholeTest_Begin.IsEnabled = enable;
                    break;
            }
        }

        /// <summary>
        /// 查询测试结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewResult_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 货道测试工作主线程
        /// </summary>
        private void AsileTestTrd()
        {
            int intErrCode = 0;

            m_IsTrdOper = true;

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(false);
            }));

            switch (m_TestType)
            {
                case "0":// 单货道测试
                    m_TestTotalNum++;
                    intErrCode = PubHelper.p_BusinOper.TestAsile(m_TestCode);
                    this.tbTitle.Dispatcher.Invoke(new Action(() =>
                    {
                        tbTestTotalNum.Text = m_TestTotalNum.ToString();
                        if (intErrCode == 0)
                        {
                            currentGoodsWay.SetSecondText(m_Title_AsileNormal);
                        }
                        else
                        {
                            currentGoodsWay.SetSecondText(m_Title_AsileFail_Code.Replace("{N}",intErrCode.ToString()));
                        }
                    }));

                    break;

                case "1":// 托盘测试
                case "2":// 整机测试
                    if (m_TestType == "2")
                    {
                        this.tbTitle.Dispatcher.Invoke(new Action(() =>
                        {
                            m_TrayCount = panelTray.Children.Count;
                        }));
                    }
                    bool blnAsileIsNull = false;// 货道格子是否为空 False：不为空 True：为空
                    for (int trayIndex = 0; trayIndex < m_TrayCount; trayIndex++)
                    {
                        if (m_IsStopTest)
                        {
                            break;
                        }

                        if (m_TestType == "2")
                        {
                            this.tbTitle.Dispatcher.Invoke(new Action(() =>
                            {
                                var palletButton = panelTray.Children[trayIndex] as RadioButton;
                                palletButton.IsChecked = true;
                            }));

                            if (m_IsLoopTest)
                            {
                                // 整机测试做循环测试
                                if (trayIndex == m_TrayCount - 1)
                                {
                                    trayIndex = -1;
                                }
                            }
                        }

                        Thread.Sleep(200);
                        for (int asileIndex = 0; asileIndex < m_AsileCount; asileIndex++)
                        {
                            if (m_IsStopTest)
                            {
                                break;
                            }
                            blnAsileIsNull = true;
                            m_TestTotalNum++;
                            this.tbTitle.Dispatcher.Invoke(new Action(() =>
                            {
                                GoodsWayProduct productControl = (GoodsWayProduct)panelAsile.Children[asileIndex];
                                if (productControl.CurrentGoodsWayProduct != null)
                                {
                                    blnAsileIsNull = false;
                                    if (currentGoodsWay != null)
                                    {
                                        currentGoodsWay.ToNormal();
                                    }
                                    //
                                    currentGoodsWay = productControl;
                                    currentGoodsWay.ToCheck();
                                    
                                    m_TestCode = currentGoodsWay.CurrentGoodsWayProduct.PaCode;
                                    tbNowAsileCode.Text = m_TestCode;
                                    currentGoodsWay.SetSecondText(m_Testing);
                                }
                            }));
                            if (m_TestType == "1")
                            {
                                // 如果是托盘循环测试
                                if (m_IsLoopTest)
                                {
                                    if (asileIndex == m_AsileCount - 1)
                                    {
                                        asileIndex = -1;
                                    }
                                }
                            }

                            if (!blnAsileIsNull)
                            {
                                intErrCode = PubHelper.p_BusinOper.TestAsile(m_TestCode);
                                this.tbTitle.Dispatcher.Invoke(new Action(() =>
                                {
                                    tbTestTotalNum.Text = m_TestTotalNum.ToString();
                                    if (intErrCode == 0)
                                    {
                                        currentGoodsWay.SetSecondText(m_Title_AsileNormal);
                                    }
                                    else
                                    {
                                        currentGoodsWay.SetSecondText(m_Title_AsileFail_Code.Replace("{N}", intErrCode.ToString()));
                                    }
                                }));
                            }
                        }
                    }

                    break;
            }

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(true);
            }));

            m_IsTrdOper = false;
        }
    }
}
