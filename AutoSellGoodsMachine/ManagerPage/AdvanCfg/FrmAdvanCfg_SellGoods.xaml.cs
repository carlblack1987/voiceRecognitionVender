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
    /// FrmAdvanCfg_SellGoods.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_SellGoods : Window
    {
        public FrmAdvanCfg_SellGoods()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_Title");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            tbSellGoodsType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_SellType");
            tbKmbControlSwitch.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_KmbControlSwitch");
            tbKmbPort.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_KmbPort");

            tbSellFailTryNum.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_SellFailTryNum");
            tbColumnType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_ColumnType");
            rdbColumnType_Symbol.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_ColumnType_Str");
            rdbColumnType_Num.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_ColumnType_Num");

            string strExist = PubHelper.p_LangOper.GetStringBundle("Pub_Exist");
            string strNoExist = PubHelper.p_LangOper.GetStringBundle("Pub_NoExist");
            tbIsControlCircle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_IsControlCircle");

            tbIsShowPoint.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_IsShowPoint");
            rdbIsShowPoint_Yes.Content = strExist;
            rdbIsShowPoint_No.Content = strNoExist;

            for (int i = 1; i < 11; i++)
            {
                cmbKmbPort.Items.Add("COM" + i.ToString());
            }
            for (int i = 0; i < 6; i++)
            {
                cmbSellFailTryNum.Items.Add(i.ToString());
            }

            #endregion

            #region 加载数据

            // 出货方式
            //////if (PubHelper.p_BusinOper.DeviceInfo.SellGoodsType == Business.Enum.BusinessEnum.SellGoodsType.Spring)
            //////{
            //////    // 弹簧方式出货
            //////    tbSellGoodsType_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_SellType_Spring");
            //////}
            //////else
            //////{
            //////    // 升降机方式
            //////    tbSellGoodsType_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_SellType_Lifter");
            //////}
            tbSellGoodsType_Value.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Sell_SellType_Spring");

            // 控制主板是否启用
            if (PubHelper.p_BusinOper.DeviceInfo.KmbControlSwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                tbKmbControlSwitch_Value.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
                cmbKmbPort.IsEnabled = true;
            }
            else
            {
                tbKmbControlSwitch_Value.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
                cmbKmbPort.IsEnabled = false;
            }

            // 控制主板串口
            cmbKmbPort.Text = "COM" + PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("KmbPort");

            // 出货失败重试次数
            cmbSellFailTryNum.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("SellFailTryNum");

            // 货道标签类型
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("ColumnType") == "0")
            {
                rdbColumnType_Symbol.IsChecked = true;
            }
            else
            {
                rdbColumnType_Num.IsChecked = true;
            }

            // 8路控制板是否存在
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IsControlCircle") == "0")
            {
                tbIsControlCircle_Value.Text = strNoExist;
            }
            else
            {
                tbIsControlCircle_Value.Text = strExist;
            }

            // 是否显示金额小数点
            if (PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IsShowPoint") == "0")
            {
                // 不显示
                rdbIsShowPoint_No.IsChecked = true;
            }
            else
            {
                rdbIsShowPoint_Yes.IsChecked = true;
            }

            #endregion
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            string strKmbPort = cmbKmbPort.Text.Replace("COM", "");
            string strSellFailTryNum = cmbSellFailTryNum.Text;

            string strColumnType = "0";
            if (rdbColumnType_Num.IsChecked == true)
            {
                strColumnType = "1";
            }

            string strIsShowPoint = "0";
            if (rdbIsShowPoint_Yes.IsChecked == true)
            {
                strIsShowPoint = "1";
            }

            PubHelper.p_BusinOper.UpdateSysCfgValue("KmbPort", strKmbPort);
            PubHelper.p_BusinOper.UpdateSysCfgValue("SellFailTryNum", strSellFailTryNum);
            PubHelper.p_BusinOper.UpdateSysCfgValue("ColumnType", strColumnType);
            PubHelper.p_BusinOper.UpdateSysCfgValue("IsShowPoint", strIsShowPoint);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc_Restart"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
