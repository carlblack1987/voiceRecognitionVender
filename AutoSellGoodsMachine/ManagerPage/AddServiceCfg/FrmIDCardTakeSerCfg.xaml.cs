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
using System.IO;
using System.Threading;
using AutoSellGoodsMachine.Model;
using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmIDCardTakeSerCfg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmIDCardTakeSerCfg : Window
    {
        #region 变量声明

        private ChoiceFileInfo m_TopFile = new ChoiceFileInfo();
        private ChoiceFileInfo m_BottomFile = new ChoiceFileInfo();

        private string m_OperType = "0";

        #endregion

        public FrmIDCardTakeSerCfg()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            #region 初始化界面

            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Title_IDCardTake");
            tbCfgTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Cfg");
            tbSerName.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_SerName");
            tbTipInfo_SerName.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_SerName_Tip");
            tbWebUrl.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_WebUrl");
            tbUserKey.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_UserKey");

            btnSave.Content = btnSaveTop.Content = btnSaveBottom.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Save");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnBrowseTop.Content = btnBrowseBottom.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Browse");

            tbTopCfg.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Skin_Top");
            tbTop_Tip.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Tip_File_Top");

            tbBottomCfg.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Skin_Bottom");
            tbBottom_Tip.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AddService_Tip_File_Bottom");

            #endregion

            #region 加载参数

            tbSerName_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IDCardFreeTake_Name");
            ////tbWebUrl_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_ServerUrl");
            ////tbUserKey_Value.Text = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WxTake_UserKey");

            // 头部文件
            string strTopFile = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IDCardFreeTake_TopMv");
            BusinessEnum.AdvertType strTopFileType = PubHelper.p_BusinOper.AdvertOper.GetFileAdvertType(strTopFile);
            bool result = false;
            string strPicPath = string.Empty;
            if (strTopFileType == BusinessEnum.AdvertType.Image)
            {
                // 图片文件
                result = PubHelper.GetFormPubPic(strTopFile, out strPicPath);
                if (result)
                {
                    imgTop.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                }
                mediaElement1.Visibility = System.Windows.Visibility.Hidden;
                imgTop.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                // 视频文件
                imgTop.Visibility = System.Windows.Visibility.Hidden;
                mediaElement1.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Images\\FormPic\\pub\\" + strTopFile);
                // 临时测试
                mediaElement1.Position = TimeSpan.FromSeconds(5);
                mediaElement1.Play();
                mediaElement1.Visibility = System.Windows.Visibility.Visible;
            }

            result = PubHelper.GetFormPubPic(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IDCardFreeTake_BottomMv"), out strPicPath);
            if (result)
            {
                imgBottom.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }

            #endregion

        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmIDCardTakeSerCfg_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mediaElement1.Stop();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string strSerName = tbSerName_Value.Text;
            ////string strWebUrl = tbWebUrl_Value.Text;
            ////string strUserKey = tbUserKey_Value.Text;

            ControlForm(false);
            if (strSerName != PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("IDCardFreeTake_Name"))
            {
                PubHelper.p_IsRefreshSerBtnName = true;
            }

            bool result = PubHelper.p_BusinOper.UpdateSysCfgValue("IDCardFreeTake_Name", strSerName);
            ////result = PubHelper.p_BusinOper.UpdateSysCfgValue("WxTake_ServerUrl", strWebUrl);
            ////result = PubHelper.p_BusinOper.UpdateSysCfgValue("WxTake_UserKey", strUserKey);

            PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            ControlForm(true);
        }

        private void mediaElement1_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement1.Position = TimeSpan.Zero;
            mediaElement1.Play();
        }

        private void btnBrowseBottom_Click(object sender, RoutedEventArgs e)
        {
            ChoiceFile("1");
        }

        private void btnBrowseTop_Click(object sender, RoutedEventArgs e)
        {
            ChoiceFile("0");
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="operType">0：选择头部文件 1：选择底部文件</param>
        private void ChoiceFile(string operType)
        {
            string strFileFilter = string.Empty;
            string strChoiceFilePath = string.Empty;
            string strChoiceFileName = string.Empty;
            // 在WPF中， OpenFileDialog位于Microsoft.Win32名称空间
            string strFilter = string.Empty;
            switch (operType)
            {
                case "0":// 选择头部文件
                    strFileFilter = "图片及视频文件类型(*.png,*.wmv,*.mp4)|*.png;*.wmv;*.mp4";
                    break;
                default:// 选择底部文件
                    strFileFilter = "图片文件类型(*.png)|*.png";
                    break;
            }

            try
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Title = "请选择文件";
                dialog.Filter = strFileFilter;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = false;
                bool result = false;
                if (dialog.ShowDialog() == true)
                {
                    strChoiceFilePath = dialog.FileName;
                    strChoiceFileName = dialog.SafeFileName;
                    // 获取格式
                    BusinessEnum.AdvertType strTopFileType = PubHelper.p_BusinOper.AdvertOper.GetFileAdvertType(strChoiceFileName);

                    if (strTopFileType == BusinessEnum.AdvertType.Image)
                    {
                        // 图片文件
                        if (operType == "0")
                        {
                            mediaElement1.Visibility = System.Windows.Visibility.Hidden;
                            imgTop.Source = new BitmapImage(new Uri(strChoiceFilePath, UriKind.Absolute));
                            imgTop.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            imgBottom.Source = new BitmapImage(new Uri(strChoiceFilePath, UriKind.Absolute));
                            imgBottom.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                    else
                    {
                        // 视频文件
                        imgTop.Visibility = System.Windows.Visibility.Hidden;
                        mediaElement1.Source = new Uri(strChoiceFilePath);
                        // 临时测试
                        mediaElement1.Position = TimeSpan.FromSeconds(5);
                        mediaElement1.Play();
                        mediaElement1.Visibility = System.Windows.Visibility.Visible;
                    }

                    FileInfo fInfo = new FileInfo(strChoiceFilePath);
                    if (operType == "0")
                    {
                        m_TopFile.FilePath = strChoiceFilePath;
                        m_TopFile.FileName = strChoiceFileName;
                        m_TopFile.FileSize = fInfo.Length;
                    }
                    else
                    {
                        m_BottomFile.FilePath = strChoiceFilePath;
                        m_BottomFile.FileName = strChoiceFileName;
                        m_BottomFile.FileSize = fInfo.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                PubHelper.ShowMsgInfo("选择文件发生错误，原因：" + ex.Message, PubHelper.MsgType.Ok);
            }
        }

        private void btnSaveTop_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_TopFile.FilePath))
            {
                PubHelper.ShowMsgInfo("请先选择文件", PubHelper.MsgType.Ok);
                return;
            }

            m_OperType = "0";
            Thread TrdSaveFile = new Thread(new ThreadStart(SaveFileTrd));
            TrdSaveFile.IsBackground = true;
            TrdSaveFile.Start();
        }

        private void btnSaveBottom_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_BottomFile.FilePath))
            {
                PubHelper.ShowMsgInfo("请先选择文件", PubHelper.MsgType.Ok);
                return;
            }

            m_OperType = "1";
            Thread TrdSaveFile = new Thread(new ThreadStart(SaveFileTrd));
            TrdSaveFile.IsBackground = true;
            TrdSaveFile.Start();
        }

        private void ControlForm(bool enable)
        {
            tbSerName_Value.IsEnabled = tbWebUrl_Value.IsEnabled = tbUserKey_Value.IsEnabled = enable;
            btnSave.IsEnabled = btnSaveBottom.IsEnabled = btnSaveTop.IsEnabled = btnBrowseBottom.IsEnabled = btnBrowseTop.IsEnabled = enable;
            btnCancel.IsEnabled = enable;
        }

        /// <summary>
        /// 保存文件线程
        /// </summary>
        private void SaveFileTrd()
        {
            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(false);
            }));

            try
            {
                string strSourceFilePath = string.Empty;
                string strSourceFileName = string.Empty;
                string strCfgName = string.Empty;
                long lnFileSize = 0;

                switch (m_OperType)
                {
                    case "0":
                        strSourceFilePath = m_TopFile.FilePath;
                        strSourceFileName = m_TopFile.FileName;
                        lnFileSize = m_TopFile.FileSize;
                        strCfgName = "IDCardFreeTake_TopMv";
                        break;
                    default:
                        strSourceFilePath = m_BottomFile.FilePath;
                        strSourceFileName = m_BottomFile.FileName;
                        lnFileSize = m_BottomFile.FileSize;
                        strCfgName = "IDCardFreeTake_BottomMv";
                        break;
                }

                #region 检测磁盘空间是否充足

                if (!PubHelper.p_BusinOper.CheckDataOper.CheckDiskIsSpace(lnFileSize * 2))
                {
                    this.tbTitle.Dispatcher.Invoke(new Action(() =>
                    {
                        PubHelper.ShowMsgInfo("磁盘空间不足，请先清理磁盘空间", PubHelper.MsgType.Ok);
                        ControlForm(true);
                    }));
                    return;
                }

                #endregion

                // 拷贝文件
                File.Copy(strSourceFilePath, AppDomain.CurrentDomain.BaseDirectory.ToString() + "Images\\FormPic\\pub\\" + strSourceFileName, true);

                // 保存参数值
                bool result = PubHelper.p_BusinOper.UpdateSysCfgValue(strCfgName, strSourceFileName);
                if (result)
                {
                    if (m_OperType == "0")
                    {
                        m_TopFile.FilePath = m_TopFile.FileName = string.Empty;
                    }
                    else
                    {
                        m_BottomFile.FilePath = m_BottomFile.FileName = string.Empty;
                    }

                    this.tbTitle.Dispatcher.Invoke(new Action(() =>
                    {
                        PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
                    }));
                }
                else
                {
                    this.tbTitle.Dispatcher.Invoke(new Action(() =>
                    {
                        PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                    }));
                }
            }
            catch
            {
                this.tbTitle.Dispatcher.Invoke(new Action(() =>
                {
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
                }));
            }

            this.tbTitle.Dispatcher.Invoke(new Action(() =>
            {
                ControlForm(true);
            }));
        }
    }
}
