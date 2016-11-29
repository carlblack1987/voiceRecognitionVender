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
    /// FrmDeviceCfg_CoinStock_Normal.xaml 的交互逻辑
    /// </summary>
    public partial class FrmDeviceCfg_CoinStock_Normal : Window
    {
        private CashControl m_CurrentItem = null;

        private int m_ItemCount = 0;

        private int m_EachRowNum = 3;// 每行显示的最大数量

        private int m_MaxRowNum = 2;// 最大行数

        List<CashInfoModel> m_CurrentItemList = new List<CashInfoModel>();

        private string m_Unit_Coin = string.Empty;

        private bool m_StopRefill = false;// 是否停止补币，False：不停止 True：停止

        public FrmDeviceCfg_CoinStock_Normal()
        {
            InitializeComponent();

            InitForm();

            Loaded += (MainWindow_Loaded);
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_Title");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            tbProgress.Visibility = System.Windows.Visibility.Hidden;
            tbProgress.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_RefillProgress");

            btnClearBoxStock.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_ClearBoxStock");
            btnClearChaneStock.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_ClearChangeStock");
            btnBuBi_Begin.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_BuBiBegin");
            btnBuBi_End.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_BuBiEnd");

            m_StopRefill = true;

            ControlButton(true);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPage();
        }

        private void LoadPage()
        {
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

                        productControl.SetCurrentItem(m_CurrentItemList[index]);
                        productControl.Tag = m_CurrentItemList[index].CashValue.ToString();
                        productControl.SetOneText(PubHelper.p_BusinOper.MoneyIntToString(m_CurrentItemList[index].CashValue.ToString()));
                        strStatusText = m_CurrentItemList[index].StockNum.ToString() + "【" + m_CurrentItemList[index].BoxStockNum.ToString() + "】";

                        productControl.SetSecondText(strStatusText);
                    }
                    index++;
                }
            }
        }

        #endregion

        #region 窗口控件操作

        private void ControlButton(bool enable)
        {
            btnBuBi_Begin.IsEnabled = btnClearBoxStock.IsEnabled = btnClearChaneStock.IsEnabled = enable;
            btnBuBi_End.IsEnabled = !enable;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_BusinOper.GoodsOper.CurrentGoods = null;
            this.Close();
        }

        #endregion

        private void btnBuBi_Begin_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Oper_Ask_Begin");

            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                // 开始补币
                m_StopRefill = false;
                Thread TrdOperMain = new Thread(new ThreadStart(OperMainTrd));
                TrdOperMain.IsBackground = true;
                TrdOperMain.Start();
            }
        }

        private void btnClearBoxStock_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_ClearBoxStock_Ask");
            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                ControlButton(false);
                bool result = PubHelper.p_BusinOper.CashInfoOper.ClearCashStockNum_All("0", "2");
                if (result)
                {
                    CreateProduct();
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }
                ControlButton(true);
            }
        }

        private void btnBuBi_End_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Oper_Ask_Stop");
            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                m_StopRefill = true;
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
            string _value = string.Empty;
            string strValue = string.Empty;
            int money = 0;
            string strStatusText = string.Empty;

            // 使能硬币器
            intErrCode = PubHelper.p_BusinOper.ControlCoin("1", true);
            // 禁能纸币器
            intErrCode = PubHelper.p_BusinOper.ControlCash("0", true);

            while (!m_StopRefill)
            {
                Thread.Sleep(200);

                #region 查询当前投入金额，如果有可以找零面值的纸币投入到找零箱，则记录

                intErrCode = PubHelper.p_BusinOper.QueryMoney_AddBill(out strValue);
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
                                    // 投币类型 
                                    // 00：硬币（进硬币币筒或进Hopper找零箱）
                                    // 01：纸币（进纸币钞箱）
                                    // 02：纸币（进纸币找零箱）
                                    // 03：硬币（进溢币盒，只针对普通硬币器）
                                    string strMoneyTypeCode = hexValue[3];

                                    if ((strMoneyTypeCode == "00") || (strMoneyTypeCode == "03"))
                                    {
                                        if (strMoneyTypeCode == "00")
                                        {
                                            strMoneyTypeCode = "1";
                                        }
                                        else
                                        {
                                            strMoneyTypeCode = "2"; 
                                        }

                                        PubHelper.p_BusinOper.CashInfoOper.UpdateCashStockNum(money, 1, "0", strMoneyTypeCode, "0");

                                        this.tbTitle.Dispatcher.Invoke(new Action(() =>
                                        {
                                            // 实时刷新界面上相应硬币库存量
                                            int intCount = panelItem.Children.Count;
                                            for (int index = 0; index < intCount; index++)
                                            {
                                                CashControl productControl = (CashControl)panelItem.Children[index];
                                                if ((productControl != null) && (productControl.Tag.ToString() == money.ToString()))
                                                {
                                                    strStatusText = PubHelper.p_BusinOper.CashInfoOper.GetCashStockNum(money,"0","0").ToString() +
                                                        "【" + PubHelper.p_BusinOper.CashInfoOper.GetCashStockNum(money, "0", "1").ToString() + "】";

                                                    productControl.SetSecondText(strStatusText);
                                                }
                                            }
                                        }));
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    #endregion
                }
                #endregion
            }

            // 清除当前控制主板显示金额
            intErrCode = PubHelper.p_BusinOper.ClearUsableMoney(true);

            // 禁能硬币器
            intErrCode = PubHelper.p_BusinOper.ControlCoin("0", true);

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                tbProgress.Visibility = System.Windows.Visibility.Hidden;
                btnCancel.IsEnabled = true;
                ControlButton(true);
            }));
        }

        private void btnClearChaneStock_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_ClearChangeStock_Ask");
            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                ControlButton(false);
                bool result = PubHelper.p_BusinOper.CashInfoOper.ClearCashStockNum_All("0", "1");
                if (result)
                {
                    CreateProduct();
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }
                ControlButton(true);
            }
        }
    }
}
