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
    /// FrmAdvanCfg_BestPayCode.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_BestPayCode : Window
    {
        public FrmAdvanCfg_BestPayCode()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Payment_BestPayCode");
            tbSwitch.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Control");

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            rdbSwitch_Run.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            rdbSwitch_Stop.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");

            #endregion

            #region 加载数据

            string strSwitch = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("BestPayCodeSwitch");
            if (strSwitch == "0")
            {
                rdbSwitch_Stop.IsChecked = true;
            }
            else
            {
                rdbSwitch_Run.IsChecked = true;
            }

            #endregion
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            string strSwitch = "0";
            if (rdbSwitch_Run.IsChecked == true)
            {
                strSwitch = "1";
            }

            // 保存参数
            PubHelper.p_BusinOper.UpdateSysCfgValue("BestPayCodeSwitch", strSwitch);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
