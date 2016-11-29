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
    /// FrmAdvanCfg_Log.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_Log : Window
    {
        #region 变量声明

        private string m_Record = string.Empty;

        private string m_NoRecord = string.Empty;

        #endregion

        public FrmAdvanCfg_Log()
        {
            InitializeComponent();

            InitForm();
        }

        /// <summary>
        /// 初始化窗体
        /// </summary>
        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_Title");
            tbBusiLog.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_Busi");
            tbGateWayLog.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_GateWay");
            tbKmbLog.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_Kmb");
            tbPosLog.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_Card");
            tbNoFeeIdLog.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_ID");
            tbKeepDays.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_SaveDays");

            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            m_Record = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_Write_Yes");
            m_NoRecord = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Log_Write_No");

            cmbBusiLog.Items.Add(m_Record);
            cmbBusiLog.Items.Add(m_NoRecord);

            cmbGateWayLog.Items.Add(m_Record);
            cmbGateWayLog.Items.Add(m_NoRecord);

            cmbKmbLog.Items.Add(m_Record);
            cmbKmbLog.Items.Add(m_NoRecord);

            cmbPosLog.Items.Add(m_Record);
            cmbPosLog.Items.Add(m_NoRecord);

            cmbNoFeeIdLog.Items.Add(m_Record);
            cmbNoFeeIdLog.Items.Add(m_NoRecord);

            string strDay = PubHelper.p_LangOper.GetStringBundle("Pub_Day");
            for (int i = 1; i < 16; i++)
            {
                cmbKeepDays.Items.Add(i.ToString() + strDay);
            }

            #endregion

            #region 加载参数数据

            cmbBusiLog.Text = ConvertCodeToStr("IsWriteLog_Busi");
            cmbGateWayLog.Text = ConvertCodeToStr("IsWriteLog_GateWay");
            cmbKmbLog.Text = ConvertCodeToStr("IsWriteLog_Kmb");
            cmbPosLog.Text = ConvertCodeToStr("IsWriteLog_Card");
            cmbNoFeeIdLog.Text = ConvertCodeToStr("IsWriteLog_ID");

            cmbKeepDays.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("ClearLogIntervalDay") + strDay;

            #endregion
        }

        private string ConvertCodeToStr(string sysCfgCode)
        {
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue(sysCfgCode) == "0")
            {
                // 不记录日志
                return m_NoRecord;
            }

            return m_Record;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            ConvertStrToCode(cmbBusiLog.Text, "IsWriteLog_Busi");
            ConvertStrToCode(cmbGateWayLog.Text, "IsWriteLog_GateWay");
            ConvertStrToCode(cmbKmbLog.Text, "IsWriteLog_Kmb");
            ConvertStrToCode(cmbPosLog.Text, "IsWriteLog_Card");
            ConvertStrToCode(cmbNoFeeIdLog.Text, "IsWriteLog_ID");

            string strDay = PubHelper.p_LangOper.GetStringBundle("Pub_Day");
            string strKeepDay = cmbKeepDays.Text.Replace(strDay,"");
            PubHelper.p_BusinOper.UpdateSysCfgValue("ClearLogIntervalDay", strKeepDay);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc_Restart"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void ConvertStrToCode(string strText,string sysCfgCode)
        {
            string strLogRecord = "0";
            if (strText == m_Record)
            {
                // 记录日志
                strLogRecord = "1";
            }
            PubHelper.p_BusinOper.UpdateSysCfgValue(sysCfgCode, strLogRecord);
        }
    }
}
