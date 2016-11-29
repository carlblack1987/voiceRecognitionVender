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
using System.IO;

using AutoSellGoodsMachine.Controls;
using Business.Model;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmDeviceCfg_CoinStock.xaml 的交互逻辑
    /// </summary>
    public partial class FrmDeviceCfg_CoinStock_Hopper : Window
    {
        private CashControl m_CurrentItem = null;

        private int m_ItemCount = 0;

        private int m_EachRowNum = 3;// 每行显示的最大数量

        private int m_MaxRowNum = 3;// 最大行数

        List<CashInfoModel> m_CurrentItemList = new List<CashInfoModel>();

        private string m_Text_Run = string.Empty;
        private string m_Text_Stop = string.Empty;
        private string m_Unit_Coin = string.Empty;

        /// <summary>
        /// 操作类型 0：开启纸币接收 1：关闭纸币接收 2：刷新纸币接收
        /// </summary>
        private string m_OperType = string.Empty;

        public FrmDeviceCfg_CoinStock_Hopper()
        {
            InitializeComponent();

            InitForm();

            Loaded += (MainWindow_Loaded);
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_Title");
            btnRun.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnStop.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            btnRefresh.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Refresh");
            tbProgress.Visibility = System.Windows.Visibility.Hidden;
            tbProgress.Text = PubHelper.p_LangOper.GetStringBundle("Pub_OperProgress");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPage();
        }

        private void LoadPage()
        {
            m_Text_Run = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            m_Text_Stop = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            m_Unit_Coin = PubHelper.p_LangOper.GetStringBundle("Pub_Unit_Coin");

            m_ItemCount = 0;
            m_CurrentItemList = new List<CashInfoModel>();
            m_CurrentItemList.Clear();
            for (int i = 0; i < PubHelper.p_BusinOper.CashInfoOper.CashInfoList.Count; i++)
            {
                if (PubHelper.p_BusinOper.CashInfoOper.CashInfoList[i].CashType == "0")
                {
                    // 硬币
                    m_ItemCount++;
                    m_CurrentItemList.Add(PubHelper.p_BusinOper.CashInfoOper.CashInfoList[i]);
                }
            }
            btnCancel.IsEnabled = true;

            CreateProduct();
        }

        #region 创建内容控件
        /// <summary>
        /// 创建内容控件
        /// </summary>
        private void CreateProduct()
        {
            panelItem.Children.Clear();

            int index = 0;

            string strStatusText = string.Empty;
            for (int i = 0; i < m_MaxRowNum; i++)
            {
                for (int j = 0; j < m_EachRowNum; j++)
                {
                    if (index < m_CurrentItemList.Count)
                    {
                        CashControl productControl = new CashControl()
                        {
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                            Margin = new Thickness(5)
                        };

                        Grid.SetRow(productControl, i);
                        Grid.SetColumn(productControl, j);
                        panelItem.Children.Add(productControl);

                        productControl.MouseLeftButtonUp += (ItemWayChecked);

                        productControl.SetCurrentItem(m_CurrentItemList[index]);

                        productControl.SetOneText(PubHelper.p_BusinOper.MoneyIntToString(m_CurrentItemList[index].CashValue.ToString()));
                        strStatusText = ConvertSecText(m_CurrentItemList[index].Status, m_CurrentItemList[index].StockNum.ToString());

                        productControl.SetSecondText(strStatusText);
                    }
                    index++;
                }
            }

            ControlButton(false);
        }

        /// <summary>
        /// 产品选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemWayChecked(object sender, MouseButtonEventArgs e)
        {
            var itemWay = (sender as CashControl);
            if (itemWay == m_CurrentItem)
                return;
            if (itemWay != null)
            {
                if (m_CurrentItem != null)
                {
                    m_CurrentItem.ToNormal();
                }
                itemWay.ToCheck();
                m_CurrentItem = itemWay;

                ControlButton(true);
            }
            else
            {
                ControlButton(false);
            }
        }

        #endregion

        #region 窗口控件操作

        private void ControlButton(bool enable)
        {
            if (!enable)
            {
                btnRefresh.IsEnabled = btnRun.IsEnabled = btnStop.IsEnabled = btnQindDian.IsEnabled = enable;
            }
            else
            {
                btnRefresh.IsEnabled = enable;
                if (m_CurrentItem != null)
                {
                    if (m_CurrentItem.CurrentItem.Status == "0")
                    {
                        // 处于关闭接收状态
                        btnRun.IsEnabled = true;
                        btnStop.IsEnabled = false;
                        btnRefresh.IsEnabled = btnQindDian.IsEnabled = false;
                    }
                    else
                    {
                        // 处于开启接收状态
                        btnRun.IsEnabled = false;
                        btnStop.IsEnabled = true;
                        btnRefresh.IsEnabled = btnQindDian.IsEnabled = true;
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_BusinOper.GoodsOper.CurrentGoods = null;
            this.Close();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            m_OperType = "0";// 开启硬币接收
            OperBillStatus();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            m_OperType = "1";// 关闭硬币接收
            OperBillStatus();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            m_OperType = "2";// 刷新硬币库存
            OperBillStatus();
        }

        /// <summary>
        /// 清点硬币库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQindDian_Click(object sender, RoutedEventArgs e)
        {
            m_OperType = "3";// 清点硬币库存
            OperBillStatus();
        }

        private void OperBillStatus()
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Oper_Ask");

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                // 更改接收状态
                Thread TrdOperMain = new Thread(new ThreadStart(OperMainTrd));
                TrdOperMain.IsBackground = true;
                TrdOperMain.Start();
            }
        }

        private void OperMainTrd()
        {
            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                tbProgress.Visibility = System.Windows.Visibility.Visible;
                btnCancel.IsEnabled = false;
                ControlButton(false);
            }));

            int intErrCode = 0;
            string strNewStatus = string.Empty;
            bool result = false;
            string strStock = string.Empty;
            int intStockNum = 0;
            switch (m_OperType)
            {
                case "0":// 开启接收
                case "1":// 关闭纸币接收
                    if (m_OperType == "0")
                    {
                        strNewStatus = "1";
                    }
                    else
                    {
                        strNewStatus = "0";
                    }
                    strStock = m_CurrentItem.CurrentItem.StockNum.ToString();
                    result = PubHelper.p_BusinOper.CashInfoOper.UpdateCashStatus_All(m_CurrentItem.CurrentItem.CashValue, "0", strNewStatus);
                    if (!result)
                    {
                        intErrCode = 9;
                    }
                    break;
                case "2":// 刷新库存
                    strNewStatus = m_CurrentItem.CurrentItem.Status;

                    intErrCode = PubHelper.p_BusinOper.QueryDenomChangeNum(m_CurrentItem.CurrentItem.CashType, m_CurrentItem.CurrentItem.CashValue,"00",false, out intStockNum);
                    if (intErrCode == 0)
                    {
                        strStock = intStockNum.ToString();
                    }
                    break;
                case "3":// 清点货币
                    strNewStatus = m_CurrentItem.CurrentItem.Status;
                    intErrCode = PubHelper.p_BusinOper.QingDianCashStock(m_CurrentItem.CurrentItem.CashType, m_CurrentItem.CurrentItem.CashValue, out intStockNum);
                    if (intErrCode == 0)
                    {
                        strStock = intStockNum.ToString();
                    }
                    break;
            }

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                tbProgress.Visibility = System.Windows.Visibility.Hidden;
                if (intErrCode == 0)
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                    // 刷新显示区域
                    m_CurrentItem.CurrentItem.Status = strNewStatus;
                    m_CurrentItem.CurrentItem.StockNum = Convert.ToInt32(strStock);
                    string strStatusText = ConvertSecText(strNewStatus, strStock);
                    m_CurrentItem.SetSecondText(strStatusText);
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail_Code").Replace("{N}", intErrCode.ToString()), PubHelper.MsgType.Ok);
                }
                btnCancel.IsEnabled = true;
                ControlButton(true);
            }));
        }

        private string ConvertSecText(string status,string stock)
        {
            string strStatusText = string.Empty;
            switch (status)
            {
                case "0":// 关闭接收
                    strStatusText = m_Text_Stop;
                    break;
                case "1":// 开启接收
                    strStatusText = m_Text_Run;
                    break;
            }
            strStatusText += "【" + stock + m_Unit_Coin + "】";
            return strStatusText;
        }

        #endregion
    }
}
