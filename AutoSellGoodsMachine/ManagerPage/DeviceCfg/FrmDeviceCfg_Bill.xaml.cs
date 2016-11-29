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
    /// FrmDeviceCfg_Bill.xaml 的交互逻辑
    /// </summary>
    public partial class FrmDeviceCfg_Bill : Window
    {
        /// <summary>
        /// 是否开始补纸币 False：否 True：开始
        /// </summary>
        private bool m_BlnAddBill = false;

        /// <summary>
        /// 是否关闭窗体 False：否 True：关闭
        /// </summary>
        private bool m_CloseForm = false;

        public FrmDeviceCfg_Bill()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Title");
            tbNowStockTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_NowStockNum");

            btnBegin.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Begin");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnStop.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Stop");
            btnStop.IsEnabled = false;
            btnReturnBill.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Return");
            btnClear.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Clear");

            tbOneBill_Stock_Title.Text = (PubHelper.p_BusinOper.ConfigInfo.ReturnBillMoney / 100).ToString() +
                PubHelper.p_LangOper.GetStringBundle("Pub_Unit_Money") + PubHelper.p_LangOper.GetStringBundle("Pub_Cash_Bill");

            string strUnit = PubHelper.p_LangOper.GetStringBundle("Pub_Unit_Bill");
            tbOneBill_Stock_Unit.Text = strUnit;

            tbTipInfo.Text = string.Empty;
            tbTipInfo.Visibility = System.Windows.Visibility.Hidden;

            #region 获取10元纸币库存及相关信息

            ShowReturnBillNum();

            #endregion

            m_BlnAddBill = false;
            
        }

        private void ShowReturnBillNum()
        {
            tbOneBill_Stock_Value.Text = PubHelper.p_BusinOper.CashInfoOper.GetCashStockNum(PubHelper.p_BusinOper.ConfigInfo.ReturnBillMoney,
                "1","1").ToString();
        }

        private void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            m_BlnAddBill = true;
            // 进入补纸币工作线程
            Thread TrdMainWork = new Thread(new ThreadStart(AddBillTrd));
            TrdMainWork.IsBackground = true;
            TrdMainWork.Start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            m_BlnAddBill = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 按钮—清除纸币找零库存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            string strAskInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_ClearAsk");
            PubHelper.ShowMsgInfo(strAskInfo, PubHelper.MsgType.YesNo);
            if (PubHelper.p_MsgResult)
            {
                if (tbTipInfo.Visibility == System.Windows.Visibility.Visible)
                {
                    tbTipInfo.Visibility = System.Windows.Visibility.Hidden;
                }
                ControlForm(false,false);
                bool result = PubHelper.p_BusinOper.CashInfoOper.ClearCashStockNum(PubHelper.p_BusinOper.ConfigInfo.ReturnBillMoney, "1");
                if (result)
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                    ShowReturnBillNum();
                }
                else
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }
                ControlForm(true,true);
            }
        }

        /// <summary>
        /// 按钮—退纸币
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturnBill_Click(object sender, RoutedEventArgs e)
        {
            m_BlnAddBill = true;
            // 进入退纸币工作线程
            Thread TrdMainWork = new Thread(new ThreadStart(ReturnBillTrd));
            TrdMainWork.IsBackground = true;
            TrdMainWork.Start();
        }

        private void btnTongBu_Click(object sender, RoutedEventArgs e)
        {
            // 进入同步纸币找零工作线程
            Thread TrdTongBuBill = new Thread(new ThreadStart(TongBuBillTrd));
            TrdTongBuBill.IsBackground = true;
            TrdTongBuBill.Start();
        }

        private void ControlForm(bool enable,bool isStop)
        {
            btnBegin.IsEnabled = btnCancel.IsEnabled = btnReturnBill.IsEnabled =
                btnClear.IsEnabled = btnTongBu.IsEnabled = enable;
            if ((!enable) && (!isStop))
            {
                btnStop.IsEnabled = false;
            }
            else
            {
                btnStop.IsEnabled = !enable;
            }
        }

        private void AddBillTrd()
        {
            int intErrCode = 0;
            string _value = string.Empty;
            int ReturnBillMoney = PubHelper.p_BusinOper.ConfigInfo.ReturnBillMoney;// 可找零纸币面值
            string strValue = string.Empty;
            int money = 0;

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(false,true);
                tbTipInfo.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_OnlyBill").Replace("{N}", (ReturnBillMoney / 100).ToString());
                if (tbTipInfo.Visibility == System.Windows.Visibility.Hidden)
                {
                    tbTipInfo.Visibility = System.Windows.Visibility.Visible;
                }
            }));

            // 使能纸币器
            intErrCode = PubHelper.p_BusinOper.ControlCash("1", true);

            while (m_BlnAddBill)
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
                                    string strMoneyTypeCode = hexValue[3];// 投币类型 00：硬币 01：纸币（进钞箱） 02：纸币（进纸币找零箱）

                                    if (strMoneyTypeCode == "02")
                                    {
                                        // 如果是纸币进入了纸币找零箱，则更新该纸笔库存数据 2015-07-27增加
                                        if (money == ReturnBillMoney)
                                        {
                                            PubHelper.p_BusinOper.CashInfoOper.UpdateCashStockNum(money, 1, "1","2", "0");

                                            this.tbTitle.Dispatcher.Invoke(new Action(() =>
                                            {
                                                ShowReturnBillNum();
                                            }));
                                        }
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

            // 禁能纸币器
            intErrCode = PubHelper.p_BusinOper.ControlCash("0", true);

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(true,true);
                if (tbTipInfo.Visibility == System.Windows.Visibility.Visible)
                {
                    tbTipInfo.Visibility = System.Windows.Visibility.Hidden;
                }
            }));
        }

        private void ReturnBillTrd()
        {
            int intErrCode = 0;
            string _value = string.Empty;
            int ReturnBillMoney = PubHelper.p_BusinOper.ConfigInfo.ReturnBillMoney;// 可找零纸币面值
            string strValue = string.Empty;
            int money = 0;

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(false,false);
                tbTipInfo.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_ChangeCoin");
                if (tbTipInfo.Visibility == System.Windows.Visibility.Hidden)
                {
                    tbTipInfo.Visibility = System.Windows.Visibility.Visible;
                }
            }));

            // 使能纸币器
            intErrCode = PubHelper.p_BusinOper.ControlCash("1", true);

            Thread.Sleep(200);

            #region 退找零纸币，一次退一张

            intErrCode = PubHelper.p_BusinOper.ReturnCoin_CashStock(ReturnBillMoney, "1", out strValue);
            if (intErrCode == 0)
            {
                #region 如果退币成功，解析数据

                PubHelper.p_BusinOper.CashInfoOper.UpdateCashStockNum(ReturnBillMoney, 1, "1", "2","1");

                this.tbTitle.Dispatcher.Invoke(new Action(() =>
                {
                    ShowReturnBillNum();
                }));

                #endregion
            }
            if (intErrCode == 20)
            {
                // 没有纸币可找
                PubHelper.p_BusinOper.CashInfoOper.UpdateCashStockNum(ReturnBillMoney, 0, "1", "2","2");
                this.tbTitle.Dispatcher.Invoke(new Action(() =>
                {
                    ShowReturnBillNum();
                }));
            }
            string strTipInfo = string.Empty;
            switch (intErrCode)
            {
                case 0:// 成功
                    strTipInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Suc");
                    break;
                case 20:// 纸币器无币可找
                    strTipInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_NoBill").Replace("{N}", (ReturnBillMoney / 100).ToString());
                    break;
                default:
                    strTipInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Bill_Fail").Replace("{N}", intErrCode.ToString());// "退币失败，代码：" + intErrCode;
                    break;
            }
            #endregion

            // 禁能纸币器
            intErrCode = PubHelper.p_BusinOper.ControlCash("0", true);

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(true,true);
                tbTipInfo.Text = strTipInfo;
                if (tbTipInfo.Visibility == System.Windows.Visibility.Hidden)
                {
                    tbTipInfo.Visibility = System.Windows.Visibility.Visible;
                }
            }));
        }

        private void TongBuBillTrd()
        {
            int intErrCode = 0;
            string _value = string.Empty;
            int ReturnBillMoney = PubHelper.p_BusinOper.ConfigInfo.ReturnBillMoney;// 可找零纸币面值
            string strValue = string.Empty;

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(false,false);
                if (tbTipInfo.Visibility == System.Windows.Visibility.Visible)
                {
                    tbTipInfo.Visibility = System.Windows.Visibility.Hidden;
                }
            }));

            // 使能纸币器
            intErrCode = PubHelper.p_BusinOper.ControlCash("1", true);

            #region 同步纸币找零库存量

            int intStockNum = 0;
            intErrCode = PubHelper.p_BusinOper.QueryDenomChangeNum("1", ReturnBillMoney,"02",false, out intStockNum);
            if (intErrCode == 0)
            {
                this.tbTitle.Dispatcher.Invoke(new Action(() =>
                {
                    ShowReturnBillNum();
                }));
            }
            #endregion

            // 禁能纸币器
            intErrCode = PubHelper.p_BusinOper.ControlCash("0", true);

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(true,true);
            }));
        }
    }
}
