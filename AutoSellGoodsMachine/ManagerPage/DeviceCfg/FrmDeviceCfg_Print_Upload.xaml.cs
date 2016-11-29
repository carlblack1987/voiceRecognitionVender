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
    /// FrmDeviceCfg_Print_Upload.xaml 的交互逻辑
    /// </summary>
    public partial class FrmDeviceCfg_Print_Upload : Window
    {
        private bool m_IsTrd = false;
        private bool m_UploadResult = false;

        public FrmDeviceCfg_Print_Upload()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("Pub_MsgTitle");
            tbInfo.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Temp_Ask");
            btnNo.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_No");
            btnYes.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Yes");

            PubHelper.p_OperResult = false;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            btnYes.Visibility = btnNo.Visibility = System.Windows.Visibility.Hidden;
            tbInfo.Text = PubHelper.p_LangOper.GetStringBundle("Pub_OperProgress");

            m_IsTrd = m_UploadResult = false;

            // 启动一个工作线程，获取参数
            Thread TrdUploadCfg = new Thread(new ThreadStart(UploadCfgTrd));
            TrdUploadCfg.IsBackground = true;
            TrdUploadCfg.Start();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UploadCfgTrd()
        {
            m_IsTrd = false;

            string strErrCode = string.Empty;
            m_UploadResult = PubHelper.p_BusinOper.UploadSysCfg_Print(out strErrCode);

            m_IsTrd = true;

            string strOperInfo = string.Empty;
            PubHelper.p_OperResult = m_UploadResult;
            if (m_UploadResult)
            {
                strOperInfo = PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc");
            }
            else
            {
                strOperInfo = PubHelper.p_LangOper.GetStringBundle("Pub_OperFail_Code").Replace("{N}", strErrCode);
            }

            this.tbInfo.Dispatcher.Invoke(new Action(() =>
            {
                PubHelper.ShowMsgInfo(strOperInfo, PubHelper.MsgType.Ok);
                this.Close();
            }));

        }
    }
}
