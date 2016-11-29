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
    /// FrmAdvanCfg_OutTime.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvanCfg_OutTime : Window
    {
        public FrmAdvanCfg_OutTime()
        {
            InitializeComponent();

            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面资源

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_Title");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            tbTunOutTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_TunOutTime");// 吞币超时，以分钟为单位
            tbSellOperOutTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_SellOperOutTime");// 无操作退出时间，以秒为单位
            tbTermStatusShowTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_TermStatusShowTime");// 机器诊断页面显示时间，以秒为单位
            tbInputPwdOutTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_InputPwdOutTime");// 密码输入超时时间，以秒为单位
            tbRefOpenMaxTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_RefOpenMaxTime");// 压缩机最长工作时间，以分钟为单位
            tbRefCloseDelayTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_RefCloseDelayTime");// 压缩机关闭延时，以分钟为单位

            tbWebUrlOutTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_WebOutTime");// 网页浏览延时，以分钟为单位
            tbOtherBrowseOutTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg_Time_OtherBrowseOutTime");// 其它内容浏览延时，以分钟为单位

            string strMin = PubHelper.p_LangOper.GetStringBundle("Pub_Minute");// 分钟
            string strSecond = PubHelper.p_LangOper.GetStringBundle("Pub_Second");// 秒

            for (int i = 1; i < 16; i++)
            {
                cmbTunOutTime.Items.Add(i.ToString() + strMin);
            }
            for (int i = 20; i < 121; i++)
            {
                cmbSellOperOutTime.Items.Add(i.ToString() + strSecond);
            }
            for (int i = 1; i < 31; i++)
            {
                cmbTermStatusShowTime.Items.Add(i.ToString() + strSecond);
                cmbInputPwdOutTime.Items.Add(i.ToString() + strSecond);
            }
            for (int i = 10; i < 61; i++)
            {
                cmbRefOpenMaxTime.Items.Add(i.ToString() + strMin);
            }
            for (int i = 1; i < 21; i++)
            {
                cmbRefCloseDelayTime.Items.Add(i.ToString() + strMin);
            }

            for (int i = 1; i < 11; i++)
            {
                cmbWebUrlOutTime.Items.Add(i.ToString() + strMin);
                cmbOtherBrowseOutTime.Items.Add(i.ToString() + strMin);
            }

            #endregion

            #region 加载参数数据

            cmbTunOutTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("TunOutTime") + strMin;
            cmbSellOperOutTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("SellOperOutTime") + strSecond;
            cmbTermStatusShowTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("TermStatusShowTime") + strSecond;
            cmbInputPwdOutTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("InputPwdOutTime") + strSecond;
            cmbRefOpenMaxTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("RefOpenMaxTime") + strMin;
            cmbRefCloseDelayTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("TmpRunDelay") + strMin;

            cmbWebUrlOutTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WebUrlOutTime") + strMin;
            cmbOtherBrowseOutTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("OtherBrowseOutTime") + strMin;

            #endregion

            //////if (PubHelper.p_BusinOper.DeviceInfo.SellGoodsType == Business.Enum.BusinessEnum.SellGoodsType.Spring)
            //////{
            //////    // 如果是弹簧方式出货，则表示控制主板自己控制压缩机的这些参数，不能修改
            //////    cmbRefOpenMaxTime.IsEnabled = cmbRefCloseDelayTime.IsEnabled = false;
            //////}
            cmbRefOpenMaxTime.IsEnabled = cmbRefCloseDelayTime.IsEnabled = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnSave.IsEnabled = btnCancel.IsEnabled = false;
            DispatcherHelper.DoEvents();

            string strMin = PubHelper.p_LangOper.GetStringBundle("Pub_Minute");// 分钟
            string strSecond = PubHelper.p_LangOper.GetStringBundle("Pub_Second");// 秒

            PubHelper.p_BusinOper.UpdateSysCfgValue("TunOutTime", cmbTunOutTime.Text.Replace(strMin,""));
            PubHelper.p_BusinOper.UpdateSysCfgValue("SellOperOutTime", cmbSellOperOutTime.Text.Replace(strSecond,""));
            PubHelper.p_BusinOper.UpdateSysCfgValue("TermStatusShowTime", cmbTermStatusShowTime.Text.Replace(strSecond,""));
            PubHelper.p_BusinOper.UpdateSysCfgValue("InputPwdOutTime", cmbInputPwdOutTime.Text.Replace(strSecond,""));
            PubHelper.p_BusinOper.UpdateSysCfgValue("RefOpenMaxTime", cmbRefOpenMaxTime.Text.Replace(strMin,""));
            PubHelper.p_BusinOper.UpdateSysCfgValue("TmpRunDelay", cmbRefCloseDelayTime.Text.Replace(strMin, ""));

            PubHelper.p_BusinOper.UpdateSysCfgValue("WebUrlOutTime", cmbWebUrlOutTime.Text.Replace(strMin, ""));
            PubHelper.p_BusinOper.UpdateSysCfgValue("OtherBrowseOutTime", cmbOtherBrowseOutTime.Text.Replace(strMin, ""));

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
