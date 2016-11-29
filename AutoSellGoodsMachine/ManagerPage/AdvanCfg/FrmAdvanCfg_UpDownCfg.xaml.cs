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
using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmAdvanCfg_UpDownCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_UpDownCfg : Window
    {
        #region 变量声明

        private DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// 当前的货柜编号
        /// </summary>
        private string m_VendBoxCode = "1";

        /// <summary>
        /// 当前选中的托盘对象
        /// </summary>
        private RadioButton m_CurrentTray = null;

        /// <summary>
        /// 当前的托盘编号
        /// </summary>
        private int m_TrayNum = 1;

        /// <summary>
        /// 当前选中的货道编号
        /// </summary>
        private string m_AsileCode = string.Empty;

        /// <summary>
        /// 当前选中的货柜出货方式类型
        /// </summary>
        private BusinessEnum.SellGoodsType m_VendBox_SellGoodsType = BusinessEnum.SellGoodsType.Lifter_Comp;

        private GoodsWayProduct currentGoodsWay = null;
        private int currentTrayIndex = 0;
        private string m_NoGoodsTitle = string.Empty;

        /// <summary>
        /// 线程操作是否正在进行中 False：没有 True：正在进行
        /// </summary>
        private bool m_IsTrdOper = false;

        /// <summary>
        /// 上下移动码盘最大数值
        /// </summary>
        private int m_UpDownNums_Max = 400;
        /// <summary>
        /// 上下移动码盘最小数值
        /// </summary>
        private int m_UpDownNums_Min = 0;

        /// <summary>
        /// 出货延时最大时长
        /// </summary>
        private int m_DelayTimesNum_Max = 500;
        private int m_DelayTimesNum_Min = 0;

        /// <summary>
        /// 小车送货后最大延时时长
        /// </summary>
        private int m_SendGoodsTimes_Max = 255;
        private int m_SendGoodsTimes_Min = 0;

        private string m_DelayTimeNums_Text_Comp;
        private string m_SendGoodsTimes_Text_Comp;
        private string m_DelayTimeNums_Text_Singal;
        private string m_SendGoodsTimes_Text_Singal;

        private string m_LeftRightNums_Text_Comp;
        private string m_LeftRightNums_Text_Singal;

        #endregion

        public FrmAdvanCfg_UpDownCfg()
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
            m_DelayTimeNums_Text_Comp = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_DelayTimeNums_Text_Comp");
            m_SendGoodsTimes_Text_Comp = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_SendGoodsTimes_Text_Comp");

            m_DelayTimeNums_Text_Singal = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_DelayTimeNums_Text_Singal");
            m_SendGoodsTimes_Text_Singal = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_SendGoodsTimes_Text_Singal");

            m_LeftRightNums_Text_Comp = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_AsileTitle_Comp");
            m_LeftRightNums_Text_Singal = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_AsileTitle_Singal");

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Title");
            tbTrayTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_TrayTitle");
            ////tbAsileTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_AsileTitle");
            tbNowTrayCode.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_NowTray");
            tbUpDownNums.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_UpDownNums");
            tbDelayTimesTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_DelayTimeNums_Title");
            //////tbDelayTimeNums.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_DelayTimeNums_Text");
            //////tbSendGoodsTimes.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_SendGoodsTimes_Text");

            string strTipInfo_Range = PubHelper.p_LangOper.GetStringBundle("Pub_Tip_InterRange");
            string strTipInfo_UpDownNums = strTipInfo_Range;
            strTipInfo_UpDownNums = strTipInfo_UpDownNums.Replace("{N1}", m_UpDownNums_Min.ToString());
            strTipInfo_UpDownNums = strTipInfo_UpDownNums.Replace("{N2}", m_UpDownNums_Max.ToString());
            tbTipInfo_UpDownNums.Text = strTipInfo_UpDownNums;

            string strTipInfo_DelayTimesNums = strTipInfo_Range;
            strTipInfo_DelayTimesNums = strTipInfo_DelayTimesNums.Replace("{N1}", m_DelayTimesNum_Min.ToString());
            strTipInfo_DelayTimesNums = strTipInfo_DelayTimesNums.Replace("{N2}", m_DelayTimesNum_Max.ToString());
            tbTipInfo_DelayTimeNums.Text = strTipInfo_DelayTimesNums;

            string strTipInfo_SendGoodsTimes = strTipInfo_Range;
            strTipInfo_SendGoodsTimes = strTipInfo_SendGoodsTimes.Replace("{N1}", m_SendGoodsTimes_Min.ToString());
            strTipInfo_SendGoodsTimes = strTipInfo_SendGoodsTimes.Replace("{N2}", m_SendGoodsTimes_Max.ToString());
            tbTipInfo_SendGoodsTimes.Text = strTipInfo_SendGoodsTimes;

            btnSave_UpDownNums.Content = btnSave_DelayTimeNums.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnTest.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Test_Sell");

            btnConfig.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Config");

            m_NoGoodsTitle = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_NoGoods");
        }

        /// <summary>
        /// 创建货柜
        /// </summary>
        private void CreateBox()
        {
            int intBoxCount = PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter.Count;
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

                    Tag = PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter[i - 1].VendBoxCode
                };

                Grid.SetColumn(palletButton, i - 1);

                palletButton.Checked += (VendBoxButtonChecked);

                palletButton.Content = DictionaryHelper.Dictionary_VendBoxName(PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter[i - 1].VendBoxCode);

                panelBox.Children.Add(palletButton);

                if (i == 1)
                {
                    palletButton.IsChecked = true;
                }
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

                    FontSize = 22,

                    Tag = i - 1
                };

                Grid.SetColumn(palletButton, i - 1);

                palletButton.Checked += (PalletButtonChecked);

                palletButton.Content = map[i] + "\r\n" +
                    PubHelper.p_BusinOper.AsileOper.UpDown_GetUpDownCodeNum(m_VendBoxCode, i).ToString();

                panelTray.Children.Add(palletButton);

                if (i == 1)
                {
                    palletButton.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 创建货道
        /// </summary>
        private void CreateAsile(List<AsileModel> products)
        {
            panelAsile.Children.Clear();

            int index = 0;

            string strLeftRightCodeNum = string.Empty;
            ////string strLeftRightNumsTitle = string.Empty;
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

                        string strVendBoxCode = products[index].VendBoxCode;
                        int intVendBoxIndex = PubHelper.p_BusinOper.AsileOper.GetVendBoxIndex(strVendBoxCode);
                        strLeftRightCodeNum = PubHelper.p_BusinOper.AsileOper.UpDown_GetLeftRightCodeNum(strVendBoxCode,
                            products[index].PaId.Substring(1,1),PubHelper.p_BusinOper.AsileOper.VendBoxList[intVendBoxIndex].SellGoodsType,
                            PubHelper.p_BusinOper.ConfigInfo.UpDownLeftRightNum_Left,
                            PubHelper.p_BusinOper.ConfigInfo.UpDownLeftRightNum_Center, 
                            PubHelper.p_BusinOper.ConfigInfo.UpDownLeftRightNum_Right).ToString();
                        productControl.SetSecondText(strLeftRightCodeNum);
                    }
                    else
                    {
                        productControl.SetNoGoods(m_NoGoodsTitle);
                    }
                    index++;
                }
            }
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

                // 获取该货柜的升降出货延时等参数
                for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter.Count; i++)
                {
                    if (PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter[i].VendBoxCode == m_VendBoxCode)
                    {
                        m_VendBox_SellGoodsType = PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter[i].SellGoodsType;
                        switch (m_VendBox_SellGoodsType)
                        {
                            case BusinessEnum.SellGoodsType.Lifter_Comp://
                                tbDelayTimeNums.Text = m_DelayTimeNums_Text_Comp;
                                tbSendGoodsTimes.Text = m_SendGoodsTimes_Text_Comp;
                                tbAsileTitle.Text = m_LeftRightNums_Text_Comp;
                                break;
                            case BusinessEnum.SellGoodsType.Lifter_Simple://
                                tbDelayTimeNums.Text = m_DelayTimeNums_Text_Singal;
                                tbSendGoodsTimes.Text = m_SendGoodsTimes_Text_Singal;
                                tbAsileTitle.Text = m_LeftRightNums_Text_Singal;
                                break;
                        }
                        tbDelayTimeNums_Value.Text = PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter[i].UpDownDelayTimeNums;
                        tbSendGoodsTimes_Value.Text = PubHelper.p_BusinOper.AsileOper.VendBoxList_Lifter[i].UpDownSendGoodsTimes;
                    }
                }

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
            if (CheckIsTesting())
            {
                // 如果正在测试
                return;
            }
            if (sender is RadioButton)
            {
                m_CurrentTray = (sender as RadioButton);
                if (m_CurrentTray != null)
                {
                    currentTrayIndex = Convert.ToInt32((sender as RadioButton).Tag);
                    m_TrayNum = currentTrayIndex + 1;
                    var currentRowProducts = PubHelper.p_BusinOper.AsileOper.AsileList.Where(item => item.TrayIndex == currentTrayIndex).ToList();
                    tbNowTrayCode_Value.Text = PubHelper.p_BusinOper.ChangeTray(m_TrayNum.ToString());
                    tbUpDownNums_Value.Text = PubHelper.p_BusinOper.AsileOper.UpDown_GetUpDownCodeNum(m_VendBoxCode, m_TrayNum).ToString();
                    currentRowProducts = currentRowProducts.Where(item => item.VendBoxCode == m_VendBoxCode).ToList();
                    m_AsileCode = string.Empty;
                    CreateAsile(currentRowProducts);
                }
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
                btnTest.IsEnabled = true;
                m_AsileCode = currentGoodsWay.CurrentGoodsWayProduct.PaCode;
            }
            else
            {
                btnTest.IsEnabled = false;
                m_AsileCode = string.Empty;
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            btnSave_DelayTimeNums.IsEnabled = btnSave_UpDownNums.IsEnabled = btnTest.IsEnabled = btnCancel.IsEnabled = enable;
            btnConfig.IsEnabled = enable;
            tbDelayTimeNums_Value.IsEnabled = tbUpDownNums_Value.IsEnabled = tbSendGoodsTimes_Value.IsEnabled = enable;
        }

        /// <summary>
        /// 按钮—上下移动码盘保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_UpDownNums_Click(object sender, RoutedEventArgs e)
        {
            #region 检测上下移动码盘数据有效性
            // 0-300
            string strUpDownNums = tbUpDownNums_Value.Text;
            int intUpDownNums = 0;
            bool result = true;
            if (!PubHelper.p_BusinOper.CheckDataOper.CheckIsNum(strUpDownNums))
            {
                result = false;
            }
            else
            {
                intUpDownNums = Convert.ToInt32(strUpDownNums);
                if ((intUpDownNums < m_UpDownNums_Min) || (intUpDownNums > m_UpDownNums_Max))
                {
                    result = false;
                }
            }
            if (!result)
            {
                string strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Input_InvalidNum_Int");
                strMsgInfo = strMsgInfo.Replace("{N1}", m_UpDownNums_Min.ToString());
                strMsgInfo = strMsgInfo.Replace("{N2}", m_UpDownNums_Max.ToString());
                PubHelper.ShowMsgInfo(strMsgInfo, PubHelper.MsgType.Ok);
                return;
            }
            #endregion

            ControlForm(false);

            result = PubHelper.p_BusinOper.AsileOper.UpDown_UpdateUpDownCodeNum(m_VendBoxCode, m_TrayNum, intUpDownNums);
            if (result)
            {
                // 更新码盘值
                if (m_CurrentTray != null)
                {
                    m_CurrentTray.Content = PubHelper.p_BusinOper.ChangeTray(m_TrayNum.ToString()) 
                        + "\r\n" +
                        PubHelper.p_BusinOper.AsileOper.UpDown_GetUpDownCodeNum(m_VendBoxCode, m_TrayNum).ToString();
                }
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }

            ControlForm(true);
        }

        /// <summary>
        /// 按钮—出货延时时长等保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_DelayTimeNums_Click(object sender, RoutedEventArgs e)
        {
            bool result = CheckGoodsValid();
            if (!result)
            {
                return;
            }

            ControlForm(false);
            result = PubHelper.p_BusinOper.AsileOper.UpDown_Update_Times(m_VendBoxCode,
                tbDelayTimeNums_Value.Text, tbSendGoodsTimes_Value.Text);
            if (result)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }
            ControlForm(true);
        }

        /// <summary>
        /// 按钮—测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_AsileCode))
            {
                // 没有选择货道
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_SelectAsile"), PubHelper.MsgType.Ok);
                return;
            }

            bool result = CheckGoodsValid();
            if (!result)
            {
                return;
            }

            Thread TrdAsileTest = new Thread(new ThreadStart(AsileTestTrd));
            TrdAsileTest.IsBackground = true;
            TrdAsileTest.Start();
        }

        /// <summary>
        /// 检测出货延时等参数是否有效
        /// </summary>
        /// <returns></returns>
        private bool CheckGoodsValid()
        {
            #region 检测上下移动码盘数据有效性
            // 0-300
            string strDelayTimesNums = tbDelayTimeNums_Value.Text;
            int intDelayTimesNums = 0;
            string strSendGoodsTimes = tbSendGoodsTimes_Value.Text;
            int intSendGoodsTimes = 0;

            bool result = true;

            string strMsgInfo = PubHelper.p_LangOper.GetStringBundle("Err_Input_InvalidNum_Int");

            #region 检测出货延时时长是否正确

            if (!PubHelper.p_BusinOper.CheckDataOper.CheckIsNum(strDelayTimesNums))
            {
                result = false;
            }
            else
            {
                intDelayTimesNums = Convert.ToInt32(strDelayTimesNums);
                if ((intDelayTimesNums < m_DelayTimesNum_Min) || (intDelayTimesNums > m_DelayTimesNum_Max))
                {
                    result = false;
                }
            }

            if (!result)
            {
                string strTipInfo_DelayTimesNum = strMsgInfo;
                strTipInfo_DelayTimesNum = strTipInfo_DelayTimesNum.Replace("{N1}", m_DelayTimesNum_Min.ToString());
                strTipInfo_DelayTimesNum = strTipInfo_DelayTimesNum.Replace("{N2}", m_DelayTimesNum_Max.ToString());
                PubHelper.ShowMsgInfo(strTipInfo_DelayTimesNum, PubHelper.MsgType.Ok);
                return false;
            }

            #endregion

            #region 检测小车送货延时时长是否正确

            if (!PubHelper.p_BusinOper.CheckDataOper.CheckIsNum(strSendGoodsTimes))
            {
                result = false;
            }
            else
            {
                intSendGoodsTimes = Convert.ToInt32(strSendGoodsTimes);
                if ((intSendGoodsTimes < m_SendGoodsTimes_Min) || (intSendGoodsTimes > m_SendGoodsTimes_Max))
                {
                    result = false;
                }
            }

            if (!result)
            {
                string strTipInfo_SendGoodsTimes = strMsgInfo;
                strTipInfo_SendGoodsTimes = strTipInfo_SendGoodsTimes.Replace("{N1}", m_SendGoodsTimes_Min.ToString());
                strTipInfo_SendGoodsTimes = strTipInfo_SendGoodsTimes.Replace("{N2}", m_SendGoodsTimes_Max.ToString());
                PubHelper.ShowMsgInfo(strTipInfo_SendGoodsTimes, PubHelper.MsgType.Ok);
                return false;
            }

            #endregion

            #endregion

            return true;
        }

        /// <summary>
        /// 货道测试工作主线程
        /// </summary>
        private void AsileTestTrd()
        {
            int intErrCode = 0;

            m_IsTrdOper = true;
            int intUpDownNums = 0;
            int intDelayTimeNums = 0;

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                intUpDownNums = Convert.ToInt32(tbUpDownNums_Value.Text);
                intDelayTimeNums = Convert.ToInt32(tbDelayTimeNums_Value.Text);
                ControlForm(false);
            }));

            intErrCode = PubHelper.p_BusinOper.TestUpDown(m_VendBox_SellGoodsType, m_AsileCode, intUpDownNums, intDelayTimeNums);

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(true);
            }));

            m_IsTrdOper = false;
        }

        private void tbUpDownNums_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbUpDownNums_Value.Text);
            tbUpDownNums_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbDelayTimeNums_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbDelayTimeNums_Value.Text);
            tbDelayTimeNums_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbSendGoodsTimes_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbSendGoodsTimes_Value.Text);
            tbSendGoodsTimes_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            PubHelper.p_VendBoxCode = m_VendBoxCode;
            FrmAdvanCfg_UpDownCfg_Cfg FrmAdvanCfg_UpDownCfg_Cfg = new FrmAdvanCfg_UpDownCfg_Cfg();
            FrmAdvanCfg_UpDownCfg_Cfg.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }
    }
}
