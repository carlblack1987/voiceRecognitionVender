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
    /// FrmAdvanCfg_Reset.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_Reset : Window
    {
        public FrmAdvanCfg_Reset()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            string strInfo = string.Empty;
            if (PubHelper.p_ResetType == "0")
            {
                // 恢复出厂数据
                tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_ResetData_Title");
                strInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_ResetData_AskMsg");
            }
            else
            {
                // 恢复出厂参数
                tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_ResetCfg_Title");
                strInfo = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_ResetCfg_AskMsg");
            }
            strInfo = strInfo.Replace("{N}", "\r\n");
            tbInfo.Text = strInfo;

            tbProgress.Text = PubHelper.p_LangOper.GetStringBundle("Pub_OperProgress");
            tbProgress.Visibility = System.Windows.Visibility.Hidden;

            btnNo.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_No");
            btnYes.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Yes");
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            tbInfo.Visibility = System.Windows.Visibility.Hidden;
            tbProgress.Visibility = System.Windows.Visibility.Visible;
            btnNo.IsEnabled = btnYes.IsEnabled = false;
            btnNo.Visibility = btnYes.Visibility = System.Windows.Visibility.Hidden;
            DispatcherHelper.SleepControl(50);

            bool result = false;
            
            switch (PubHelper.p_ResetType)
            {
                case "0":// 恢复出厂数据
                    result = PubHelper.p_BusinOper.InitFactoryData();
                    break;

                default:// 恢复出厂参数
                    result = PubHelper.p_BusinOper.InitFactoryCfg();
                    if (result)
                    {
                        if (!PubHelper.p_IsRefreshAsile)
                        {
                            PubHelper.p_IsRefreshAsile = true;
                        }
                    }
                    break;
            }
            if (result)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc_Restart"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
