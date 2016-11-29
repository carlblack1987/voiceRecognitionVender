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
    /// FrmDeviceCfg_Coin.xaml 的交互逻辑
    /// </summary>
    public partial class FrmDeviceCfg_Coin : Window
    {
        public FrmDeviceCfg_Coin()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_Title");
            tbNowStockTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_NowStockNum");

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnReturnCoin.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_Return");
            btnReturnCoin.Visibility = System.Windows.Visibility.Hidden;

            tbAddStock_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Coin_AddNum");
            string strCoinUnit = PubHelper.p_LangOper.GetStringBundle("Pub_Unit_Coin");
            tbCoinUnit.Text = strCoinUnit;

            tbReturnProgress.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_ChangeCoin_Init");
            tbReturnProgress.Visibility = System.Windows.Visibility.Hidden;

            tbOneCoin_AddStock_Value.Text = "0";

            #region 获取1元硬币库存及相关信息

            tbOneCoin_Stock_Value.Text = PubHelper.p_BusinOper.CashInfoOper.GetCashStockNum(100, "0","0").ToString() + strCoinUnit;
            
            #endregion
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            #region 检测数据有效性

            string strAddStockNum = tbOneCoin_AddStock_Value.Text;
            int intAddStockNum = 0;
            bool result = PubHelper.p_BusinOper.CheckDataOper.CheckIsNum(strAddStockNum);
            if (result)
            {
                intAddStockNum = Convert.ToInt32(strAddStockNum);
                if ((intAddStockNum < 1) || (intAddStockNum > 600))
                {
                    result = false;
                }
            }
            if (!result)
            {
                string strTipInfo = PubHelper.p_LangOper.GetStringBundle("Err_Input_InvalidNum_Int");
                strTipInfo = strTipInfo.Replace("{N1}","1");
                strTipInfo = strTipInfo.Replace("{N2}","600");
                PubHelper.ShowMsgInfo(strTipInfo, PubHelper.MsgType.Ok);
                return;
            }

            #endregion

            ControlForm(false);

            // 保存
            ////////////result = PubHelper.p_BusinOper.CashInfoOper.UpdateCashStockNum(100, intAddStockNum, "0", "0");
            if (result)
            {
                // 更改库存显示
                tbOneCoin_Stock_Value.Text = PubHelper.p_BusinOper.CashInfoOper.GetCashStockNum(100, "0","0").ToString() +
                    PubHelper.p_LangOper.GetStringBundle("Pub_Unit_Coin");
                tbOneCoin_AddStock_Value.Text = "0";

                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }

            ControlForm(true);
        }

        private void btnReturnCoin_Click(object sender, RoutedEventArgs e)
        {
            // 进入退币工作线程
            Thread TrdReturnCoin = new Thread(new ThreadStart(ReturnCoinTrd));
            TrdReturnCoin.IsBackground = true;
            TrdReturnCoin.Start();
        }

        private void ControlForm(bool enable)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = btnReturnCoin.IsEnabled = tbOneCoin_AddStock_Value.IsEnabled = enable; ;
        }

        private void tbOneCoin_AddStock_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbOneCoin_AddStock_Value.Text);
            tbOneCoin_AddStock_Value.Text = PubHelper.p_Keyboard_Input;
        }

        /// <summary>
        /// 退币工作线程
        /// </summary>
        private void ReturnCoinTrd()
        {
            int intErrCode = 0;

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(false);

                tbReturnProgress.Visibility = System.Windows.Visibility.Visible;
            }));

            // 使能硬币器
            intErrCode = PubHelper.p_BusinOper.ControlCoin("1", true);
            if (intErrCode == 0)
            {
                // 开始退币，每次退5个
            }

            
        }
    }
}
