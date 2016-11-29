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

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmStat_Cash.xaml 的交互逻辑
    /// </summary>
    public partial class FrmStat_Cash : Window
    {
        public FrmStat_Cash()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_CashStat_Title");
            tbBillBoxStock_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_CashStat_BillBoxStock");
            tbCoinChangeStock_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_CashStat_CoinChangeStock");
            tbCoinBoxStock_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_CashStat_CoinBoxStock");

            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnClearBillStock.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_CashStat_ClearBillBoxStock");
            btnClearCoinStock.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_CashStat_ClearCoinBoxStock");

            LoadStockData();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClearCoinStock_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Oper_Ask");
            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                ControlButton(false);
                bool result = PubHelper.p_BusinOper.CashInfoOper.ClearCashStockNum_All("0", "2");

                LoadStockData(); 
                if (result)
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }
                ControlButton(true);
            }
        }

        private void ControlButton(bool enable)
        {
            btnClearBillStock.IsEnabled = btnClearCoinStock.IsEnabled = btnCancel.IsEnabled = enable;
        }

        private void btnClearBillStock_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("Pub_Oper_Ask");
            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                ControlButton(false);
                bool result = PubHelper.p_BusinOper.CashInfoOper.ClearCashStockNum_All("1", "1");
                LoadStockData();
                if (result)
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }
                ControlButton(true);
            }
        }

        private void LoadStockData()
        {
            tbBillBoxStock_Value.Text = PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.CashInfoOper.GetCashStockMoney_Type("1","0").ToString());
            tbCoinChangeStock_Value.Text = PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.CashInfoOper.GetCashStockMoney_Type("0","0").ToString());
            tbCoinBoxStock_Value.Text = PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.CashInfoOper.GetCashStockMoney_Type("0","1").ToString());
        }
    }
}
