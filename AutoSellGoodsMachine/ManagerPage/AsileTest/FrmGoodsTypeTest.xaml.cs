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
    public partial class FrmGoodsTypeTest : Window
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

        public FrmGoodsTypeTest()
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
            ////CreateBox();//
            ////CreateTray();

            CreateAsile();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
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
        /// 创建货道
        /// </summary>
        private void CreateAsile()
        {
            panelAsile.Width = 500;
            panelAsile.Height = 600;

            panelAsile.Children.Clear();

            int index = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    GoodsTypeControl productControl = new GoodsTypeControl()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                        Margin = new Thickness(5)
                    };
                    Grid.SetRow(productControl, i);
                    Grid.SetColumn(productControl, j);
                    panelAsile.Children.Add(productControl);

                    ////productControl.MouseLeftButtonUp += (GoodsWayChecked);

                    index++;
                }
            }

            m_AsileCount = panelAsile.Children.Count;
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
    }
}
