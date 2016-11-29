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
using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmDeviceCfg_Print.xaml 的交互逻辑
    /// </summary>
    public partial class FrmDeviceCfg_Print : Window
    {
        private bool m_Init = true;

        public FrmDeviceCfg_Print()
        {
            InitializeComponent();

            InitForm();

            Loaded += (FrmDeviceCfg_Print_Loaded);
        }

        private void FrmDeviceCfg_Print_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void InitForm()
        {
            m_Init = true;

            #region 初始化界面

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Title");
            tbCfgTitle.Text = PubHelper.p_LangOper.GetStringBundle("Pub_DeviceCfg");
            tbControlSwitch.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_ControlSwitch");
            rdbControlSwitch_Run.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            rdbControlSwitch_Stop.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            tbPrintTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Temp_Title");
            tbPrintContent.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Temp_Content");

            tbSerPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_SerPort");
            for (int i = 1; i < 11; i++)
            {
                cmbPort.Items.Add("COM" + i.ToString());
            }
            cmbPort.Text = "COM" + PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("PrintPort");

            btnSave.Content = btnSavePrintContent.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            tbPrintTempTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Temp");
            btnUpDateTemp.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Upload");

            tbDeviceTest.Text = PubHelper.p_LangOper.GetStringBundle("Pub_DeviceTest");

            btnQuery.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Query");
            btnPrint.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Test");
            btnCut.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg_Print_Cut");

            tbErrCodeTitle.Text = PubHelper.p_LangOper.GetStringBundle("Pub_ResultCode");
            tbErrCodeContent.Text = DictionaryHelper.Dictionary_Lang_PrinterErrCode();


            #endregion

            #region 加载参数值

            if (PubHelper.p_BusinOper.ConfigInfo.IsPrintConsumeBill == BusinessEnum.ControlSwitch.Stop)
            {
                // 打印功能关闭
                rdbControlSwitch_Stop.IsChecked = true;
                ControlForm(false);
            }
            else
            {
                // 打印功能开启
                rdbControlSwitch_Run.IsChecked = true;
                ControlForm(true);
            }

            tbPrintTemp_Value.Text = PubHelper.p_BusinOper.ConfigInfo.PrintTmepContent;
            tbPrint_Title.Text = PubHelper.p_BusinOper.ConfigInfo.PrintTmepTitle;

            #endregion

            if (PubHelper.p_BusinOper.UserType != Business.Enum.BusinessEnum.UserType.SystemUser)
            {
                // 不是厂商管理员
                rdbControlSwitch_Run.IsEnabled = rdbControlSwitch_Stop.IsEnabled = btnSave.IsEnabled =
                    cmbPort.IsEnabled = 
                    btnUpDateTemp.IsEnabled = btnSavePrintContent.IsEnabled = false;
                tbPrintTemp_Value.IsReadOnly = true;
                tbPrint_Title.IsReadOnly = true;
                btnQuery.IsEnabled = btnPrint.IsEnabled = btnCut.IsEnabled = true;
            }

            m_Init = false;
        }

        /// <summary>
        /// 控制窗体控件显示
        /// </summary>
        /// <param name="enable"></param>
        private void ControlForm(bool enable)
        {
            cmbPort.IsEnabled = btnUpDateTemp.IsEnabled = enable;
            tbPrintTemp_Value.IsReadOnly = !enable;
            tbPrint_Title.IsReadOnly = !enable;
            btnQuery.IsEnabled = btnPrint.IsEnabled = btnCut.IsEnabled = btnSavePrintContent.IsEnabled = enable;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = false;
            ControlForm(false);

            string strControlSwitch = "0";
            if (rdbControlSwitch_Run.IsChecked == true)
            {
                strControlSwitch = "1";
            }

            string strPort = cmbPort.Text.Replace("COM","");

            bool result = false;

            bool blnIsEdit = false;

            result = PubHelper.p_BusinOper.UpdateSysCfgValue("IsPrintConsumeBill", strControlSwitch);

            if (PubHelper.p_BusinOper.ConfigInfo.PrintPort != strPort)
            {
                blnIsEdit = true;
                result = PubHelper.p_BusinOper.UpdateSysCfgValue("PrintPort", strPort);
            }

            if (result)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }

            btnSave.IsEnabled = true;
            if (strControlSwitch == "1")
            {
                ControlForm(true);
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            int intErrCode = PubHelper.p_BusinOper.QueryPrintStatus();
            tbErrCodeValue.Text = intErrCode.ToString();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            int intErrCode = PubHelper.p_BusinOper.PrintConsumeInfo();
            tbErrCodeValue.Text = intErrCode.ToString();
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            int intErrCode = PubHelper.p_BusinOper.CutPrintPaper();
            tbErrCodeValue.Text = intErrCode.ToString();
        }

        private void rdbControlSwitch_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_Init)
            {
                ControlForm(true);
            }
        }

        private void rdbControlSwitch_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_Init)
            {
                ControlForm(false);
            }
        }

        private void btnSavePrintContent_Click(object sender, RoutedEventArgs e)
        {
            btnSavePrintContent.IsEnabled = false;
            bool result = PubHelper.p_BusinOper.UpdateSysCfgValue("PrintTmepContent", tbPrintTemp_Value.Text);
            result = PubHelper.p_BusinOper.UpdateSysCfgValue("PrintTmepTitle", tbPrint_Title.Text);
            if (result)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }
            btnSavePrintContent.IsEnabled = true;
        }

        private void btnUpDateTemp_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_OperResult = false;
            FrmDeviceCfg_Print_Upload frmDeviceCfg_Print_Upload = new FrmDeviceCfg_Print_Upload();
            frmDeviceCfg_Print_Upload.ShowDialog();
            if (PubHelper.p_OperResult)
            {
                tbPrintTemp_Value.Text = PubHelper.p_BusinOper.ConfigInfo.PrintTmepContent;
                tbPrint_Title.Text = PubHelper.p_BusinOper.ConfigInfo.PrintTmepTitle;
            }
        }
    }
}
