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
    /// FrmAdvert.xaml 的交互逻辑
    /// </summary>
    public partial class FrmAdvert : Window
    {
        private bool m_IsInit = true;

        public FrmAdvert()
        {
            InitializeComponent();
            m_IsInit = true;
            InitForm();
        }

        private void InitForm()
        {
            #region 加载标签

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnSave.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnImport.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_Import");
            btnPriew.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_Priview");

            tbAdvUploadType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_UploadType");
            tbNowAdvListID.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_NowAdvListID");
            tbUploadAdvListID.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_UploadAdvListID");

            tbAdvertSwitch.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_Switch");
            ////tbImgShowTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_ImgShowTime");
            tbPlayOutTime.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_PlayOutTime");

            string strRun = PubHelper.p_LangOper.GetStringBundle("Pub_Run");
            string strStop = PubHelper.p_LangOper.GetStringBundle("Pub_Stop");
            rdbAdvertSwitch_Run.Content = strRun;
            rdbAdvertSwitch_Stop.Content = strStop;

            tbTipInfo_ImgType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_FormatInfo_Img");
            tbTipInfo_VideoType.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_FormatInfo_Video");

            tbVideoSound.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert_Volume");

            string strMin = PubHelper.p_LangOper.GetStringBundle("Pub_Minute");// 分钟
            string strSecond = PubHelper.p_LangOper.GetStringBundle("Pub_Second");// 秒
            ////for (int i = 5; i < 121; i++)
            ////{
            ////    cmbImgShowTime.Items.Add(i.ToString() + strSecond);
            ////}
            for (int i = 1; i < 31; i++)
            {
                cmbPlayOutTime.Items.Add(i.ToString() + strMin);
            }

            ControlPriew();

            #endregion

            #region 加载数据

            if (PubHelper.p_BusinOper.ConfigInfo.AdvertPlaySwitch == Business.Enum.BusinessEnum.ControlSwitch.Run)
            {
                // 开启
                rdbAdvertSwitch_Run.IsChecked = true;
                ControlSwitch(true);
            }
            else
            {
                rdbAdvertSwitch_Stop.IsChecked = true;
                ControlSwitch(false);
            }

            rdbAdvertSwitch_Run.IsEnabled = rdbAdvertSwitch_Stop.IsEnabled = false;

            tbNowAdvListID_Value.Text = PubHelper.p_BusinOper.ConfigInfo.NowAdvertPlayID;
            if (PubHelper.p_BusinOper.ConfigInfo.UpdateAdvertListID == PubHelper.p_BusinOper.ConfigInfo.NowAdvertPlayID)
            {
                tbUploadAdvListID_Value.Text = string.Empty;
            }
            else
            {
                tbUploadAdvListID_Value.Text = PubHelper.p_BusinOper.ConfigInfo.UpdateAdvertListID;
            }
            
            tbAdvUploadType_Value.Text = DictionaryHelper.Dictionary_AdvUploadType(PubHelper.p_BusinOper.ConfigInfo.AdvertUploadType);

            ////cmbImgShowTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdvertImgShowTime") + strSecond;
            cmbPlayOutTime.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdvertPlayOutTime") + strMin;

            string strAdvertVolume = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdvertVolume");
            sidSound.Value = Convert.ToDouble(strAdvertVolume);
            ////tbSoundNum.Text = strAdvertVolume;

            #endregion

            if (PubHelper.p_BusinOper.ConfigInfo.AdvertUploadType != "1")
            {
                // 允许本地导入
                btnImport.IsEnabled = true;
            }
            else
            {
                // 不允许本地导入
                btnImport.IsEnabled = false;
            }

            m_IsInit = false;
        }

        private void ControlButton(bool enable)
        {
            btnSave.IsEnabled = enable;
            btnCancel.IsEnabled = enable;
            ////rdbAdvertSwitch_Run.IsEnabled = rdbAdvertSwitch_Stop.IsEnabled = enable;
            sidSound.IsEnabled = enable;
            ControlSwitch(enable);
            DispatcherHelper.SleepControl();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ControlButton(false);
            DispatcherHelper.DoEvents();

            string strPlaySwitch = "0";
            if (rdbAdvertSwitch_Run.IsChecked == true)
            {
                strPlaySwitch = "1";
            }

            string strMin = PubHelper.p_LangOper.GetStringBundle("Pub_Minute");// 分钟
            string strSecond = PubHelper.p_LangOper.GetStringBundle("Pub_Second");// 秒

            PubHelper.p_BusinOper.UpdateSysCfgValue("AdvertPlaySwitch", strPlaySwitch);
            ////PubHelper.p_BusinOper.UpdateSysCfgValue("AdvertImgShowTime", cmbImgShowTime.Text.Replace(strSecond, ""));
            PubHelper.p_BusinOper.UpdateSysCfgValue("AdvertPlayOutTime", cmbPlayOutTime.Text.Replace(strMin, ""));
            PubHelper.p_BusinOper.UpdateSysCfgValue("AdvertVolume", tbSoundNum.Text);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);

            if (strPlaySwitch == "1")
            {
                ControlButton(true);
            }
            else
            {
                rdbAdvertSwitch_Run.IsEnabled = rdbAdvertSwitch_Stop.IsEnabled = true;
                btnSave.IsEnabled = btnCancel.IsEnabled = true;
                ControlSwitch(false);
            }
        }

        private void ControlSwitch(bool enable)
        {
            cmbPlayOutTime.IsEnabled = btnImport.IsEnabled = sidSound.IsEnabled = enable;
            if (enable)
            {
                ControlPriew();
            }
            else
            {
                btnPriew.IsEnabled = false;
            }
        }

        private void ControlPriew()
        {
            if (PubHelper.p_BusinOper.AdvertOper.AdvertList.Count <= 0)
            {
                btnPriew.IsEnabled = false;
            }
            else
            {
                btnPriew.IsEnabled = true;
            }
        }

        private void rdbAdvertSwitch_Run_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                ControlSwitch(true);
            }
        }

        private void rdbAdvertSwitch_Stop_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_IsInit)
            {
                ControlSwitch(false);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            ControlButton(false);
            DispatcherHelper.DoEvents();

            FrmAdvertImport frmAdvertImport = new AutoSellGoodsMachine.FrmAdvertImport();
            frmAdvertImport.ShowDialog();

            ControlButton(true);
            DispatcherHelper.DoEvents();
        }

        private void btnPriew_Click(object sender, RoutedEventArgs e)
        {
            ControlButton(false);
            DispatcherHelper.DoEvents();

            FrmPlayAdvert26 frmPlayAdvert26 = new FrmPlayAdvert26();
            frmPlayAdvert26.ShowDialog();

            ControlButton(true);
            DispatcherHelper.DoEvents();
        }
    }
}
