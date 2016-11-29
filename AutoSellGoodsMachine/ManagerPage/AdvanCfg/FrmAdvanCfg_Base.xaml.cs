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
    /// FrmAdvanCfg_Base.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_Base : Window
    {
        public FrmAdvanCfg_Base()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_Title");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            rdbIsTestVer_Normal.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_Soft_Normal");
            rdbIsTestVer_Demo.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_Soft_Demo");

            tbVmId.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_VmId");
            tbLgsId.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_LgsId");
            tbIsTestVer.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_Soft");
            tbCountry.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Base_Country");

            string strNumberTip = PubHelper.p_LangOper.GetStringBundle("Tip_Number_FixedLen");
            tbTipInfo_VmId.Text = strNumberTip.Replace("{N}", "10");
            tbTipInfo_LgsId.Text = strNumberTip.Replace("{N}", "3");

            #endregion

            #region 加载参数数据

            tbVmId_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("VmId");
            tbLgsId_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("LgsId");
            string strSoftType = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IsTestVer");
            if (strSoftType == "0")
            {
                // 不是测试版本
                rdbIsTestVer_Normal.IsChecked = true;
            }
            else
            {
                // 测试版本
                rdbIsTestVer_Demo.IsChecked = true;
            }

            string strCountryName = string.Empty;
            string strCountryCode = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("CountryCode");
            int intCountryCount = PubHelper.p_BusinOper.SysCfgOper.CountryList.Count;
            if (intCountryCount > 0)
            {
                for (int i = 0; i < intCountryCount; i++)
                {
                    if (PubHelper.p_BusinOper.ConfigInfo.Language == Business.Enum.BusinessEnum.Language.Zh_CN)
                    {
                        // 中文
                        strCountryName = PubHelper.p_BusinOper.SysCfgOper.CountryList[i].CountryName_ZN;
                    }
                    else
                    {
                        // 英文
                        strCountryName = PubHelper.p_BusinOper.SysCfgOper.CountryList[i].CountryName_EN;
                    }
                    cmbCountry.Items.Add(strCountryName);
                    if (strCountryCode == PubHelper.p_BusinOper.SysCfgOper.CountryList[i].CountryCode)
                    {
                        cmbCountry.Text = strCountryName;
                    }
                }
            }

            #endregion
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strVmId = tbVmId_Value.Text;

            string strLgsId = tbLgsId_Value.Text;

            string strSoftType = "0";
            if (rdbIsTestVer_Demo.IsChecked == true)
            {
                strSoftType = "1";
            }

            // 检测数据有效性
            if (!(PubHelper.p_BusinOper.CheckDataOper.CheckIsPosInt(strVmId)) ||
                (strVmId.Length != 10))
            {
                // 机器出厂号无效
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_Input_InvalidVmId"), PubHelper.MsgType.Ok);
                return;
            }

            bool blnLgsIdIsNormal = true;
            if (strLgsId.Length != 3)
            {
                blnLgsIdIsNormal = false;
            }
            else
            {
                if ((strLgsId != "000") && (!(PubHelper.p_BusinOper.CheckDataOper.CheckIsPosInt(strLgsId))))
                {
                    blnLgsIdIsNormal = false;
                }
            }

            if (!blnLgsIdIsNormal)
            {
                // 客户编码无效
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_Input_InvalidLgsId"), PubHelper.MsgType.Ok);
                return;
            }

            #region 获取国家代码信息

            string strCmbCountryName = cmbCountry.Text;
            int intCountryCount = PubHelper.p_BusinOper.SysCfgOper.CountryList.Count;
            string strCountryName = string.Empty;
            string strCountryCode = string.Empty;
            if (intCountryCount > 0)
            {
                for (int i = 0; i < intCountryCount; i++)
                {
                    if (PubHelper.p_BusinOper.ConfigInfo.Language == Business.Enum.BusinessEnum.Language.English)
                    {
                        // 英文
                        strCountryName = PubHelper.p_BusinOper.SysCfgOper.CountryList[i].CountryName_EN;
                    }
                    else
                    {
                        // 中文
                        strCountryName = PubHelper.p_BusinOper.SysCfgOper.CountryList[i].CountryName_ZN;
                    }
                    if (strCmbCountryName == strCountryName)
                    {
                        strCountryCode = PubHelper.p_BusinOper.SysCfgOper.CountryList[i].CountryCode;
                        break;
                    }
                }
            }

            #endregion

            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            // 保存数据
            PubHelper.p_BusinOper.UpdateSysCfgValue("VmId", strVmId);
            PubHelper.p_BusinOper.UpdateSysCfgValue("LgsId", strLgsId);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IsTestVer", strSoftType);
            PubHelper.p_BusinOper.UpdateSysCfgValue("CountryCode", strCountryCode);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc_Restart"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void btnClearVmId_Click(object sender, RoutedEventArgs e)
        {
            tbVmId_Value.Text = string.Empty;
        }

        private void btnClearLgsId_Click(object sender, RoutedEventArgs e)
        {
            tbLgsId_Value.Text = string.Empty;
        }

        private void tbVmId_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbVmId_Value.Text);
            tbVmId_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbLgsId_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbLgsId_Value.Text);
            tbLgsId_Value.Text = PubHelper.p_Keyboard_Input;
        }
    }
}
