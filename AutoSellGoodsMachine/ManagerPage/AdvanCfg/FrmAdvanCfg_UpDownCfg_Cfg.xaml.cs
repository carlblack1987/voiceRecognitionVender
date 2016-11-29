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
    /// FrmAdvanCfg_UpDownCfg_Cfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_UpDownCfg_Cfg : Window
    {
        public FrmAdvanCfg_UpDownCfg_Cfg()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Title");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            tbSellGoodsType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_SellType");
            tbVendBoxCode.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_VendBox");
            tbShippPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_ShippPort");

            tbUpDownSellModel.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_SellModel");// 出货指令
            rdbUpDownSellModel_ZhiJie.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_SellModel_ZJ");
            rdbUpDownSellModel_Pra.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_SellModel_Pra");

            tbUpDownIsQueryElectStatus.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_GuangDian");
            rdbUpDownIsQueryElectStatus_Yes.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Run");
            rdbUpDownIsQueryElectStatus_No.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Close");

            tbLeftSpace_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_LeftSpace");
            tbMiddleSpace_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_MiddleSpace");
            tbRightSpace_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_UpDown_Cfg_RightSpace");

            for (int i = 1; i < 11; i++)
            {
                cmbShippPort.Items.Add("COM" + i.ToString());
            }

            #endregion

            #region 加载数据

            int intVendBoxIndex = PubHelper.p_BusinOper.AsileOper.GetVendBoxIndex(PubHelper.p_VendBoxCode);

            // 柜号
            tbVendCode_Value.Text = DictionaryHelper.Dictionary_VendBoxName(PubHelper.p_VendBoxCode);

            // 出货方式
            tbSellGoodsType_Value.Text = DictionaryHelper.Dictionary_SellGoodsType(PubHelper.p_VendBoxCode);

            // 驱动板串口
            cmbShippPort.Text = "COM" + PubHelper.p_BusinOper.AsileOper.VendBoxList[intVendBoxIndex].ShippPort;

            // 出货指令
            if (PubHelper.p_BusinOper.AsileOper.VendBoxList[intVendBoxIndex].UpDownSellModel == "0")
            {
                // 直接升降
                rdbUpDownSellModel_ZhiJie.IsChecked = true;
            }
            else
            {
                rdbUpDownSellModel_Pra.IsChecked = true;
            }

            // 光电检测
            if (PubHelper.p_BusinOper.AsileOper.VendBoxList[intVendBoxIndex].UpDownIsQueryElectStatus == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                // 开启光电检测
                rdbUpDownIsQueryElectStatus_Yes.IsChecked = true;
            }
            else
            {
                rdbUpDownIsQueryElectStatus_No.IsChecked = true;
            }

            tbLeftSpace_Value.Text = PubHelper.p_BusinOper.ConfigInfo.UpDownLeftRightNum_Left.ToString();
            tbMiddleSpace_Value.Text = PubHelper.p_BusinOper.ConfigInfo.UpDownLeftRightNum_Center.ToString();
            tbRightSpace_Value.Text = PubHelper.p_BusinOper.ConfigInfo.UpDownLeftRightNum_Right.ToString();

            #endregion
        }

        private void tbLeftSpace_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbLeftSpace_Value.Text);
            tbLeftSpace_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbMiddleSpace_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbMiddleSpace_Value.Text);
            tbMiddleSpace_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void tbRightSpace_Value_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PubHelper.ShowKeyBoard(tbRightSpace_Value.Text);
            tbRightSpace_Value.Text = PubHelper.p_Keyboard_Input;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            string strShipPort = cmbShippPort.Text.Replace("COM", "");

            string strUpDownSellModel = "0";
            if (rdbUpDownSellModel_Pra.IsChecked == true)
            {
                strUpDownSellModel = "1";
            }

            string strUpDownIsQueryElectStatus = "0";
            if (rdbUpDownIsQueryElectStatus_Yes.IsChecked == true)
            {
                strUpDownIsQueryElectStatus = "1";
            }

            string strFailMsgInfo = PubHelper.p_LangOper.GetStringBundle("Pub_OperFail");
            // 保存小车左、中、右移动位置格数
            bool result = true;
            if (!PubHelper.p_BusinOper.CheckDataOper.CheckIsNum(tbLeftSpace_Value.Text))
            {
                result = false;
            }
            else if (!PubHelper.p_BusinOper.CheckDataOper.CheckIsNum(tbMiddleSpace_Value.Text))
            {
                result = false;
            }
            else if (!PubHelper.p_BusinOper.CheckDataOper.CheckIsNum(tbRightSpace_Value.Text))
            {
                result = false;
            }
            else
            {
                result = true;
            }
            if (!result)
            {
                PubHelper.ShowMsgInfo(strFailMsgInfo, PubHelper.MsgType.Ok);
                btnSave.IsEnabled = btnCancel.IsEnabled = true;
                return;
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("UpDownLeftRightNum_Left", tbLeftSpace_Value.Text);
            PubHelper.p_BusinOper.UpdateSysCfgValue("UpDownLeftRightNum_Center", tbMiddleSpace_Value.Text);
            PubHelper.p_BusinOper.UpdateSysCfgValue("UpDownLeftRightNum_Right", tbRightSpace_Value.Text);

            result = PubHelper.p_BusinOper.AsileOper.UpDown_Update_Port(PubHelper.p_VendBoxCode, strShipPort, strUpDownSellModel, strUpDownIsQueryElectStatus);
            if (result)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                this.Close();
            }
            else
            {
                PubHelper.ShowMsgInfo(strFailMsgInfo, PubHelper.MsgType.Ok);
                btnSave.IsEnabled = btnCancel.IsEnabled = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
