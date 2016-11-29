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
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmPlayAdvert26.xaml 的交互逻辑
    /// </summary>
    public partial class FrmPlayAdvert26 : Window
    {
        #region 私有变量

        /// <summary>
        /// 是否关闭窗体 False：不关闭 True：关闭
        /// </summary>
        private bool m_CloseForm = false;

        /// <summary>
        /// 是否开始播放下一个广告 False：不播放 True：播放
        /// </summary>
        private bool m_IsPlayNext = false;

        /// <summary>
        /// 下一个要播放的广告序号
        /// </summary>
        private int m_NextPlayNo = 0;

        /// <summary>
        /// 广告总数量
        /// </summary>
        private int m_AdvertCount = 0;

        private string m_NowAdvertName = string.Empty;

        private string m_NowAdvertType = string.Empty;

        private string m_NowAdvertFilePath = string.Empty;

        private BusinessEnum.AdvertType m_NowAdvertFormat = BusinessEnum.AdvertType.Image;

        private DateTime m_NextAdvertPlayBeginTime = DateTime.Now;

        private string m_NowAdvListID = string.Empty;
        private int m_AdvertImgShowTime = 5;

        #endregion

        #region 音量控制

        ////[DllImport("user32.dll")]
        ////public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        ////public void SetVol()
        ////{
        ////    p = Process.GetCurrentProcess();
        ////    int fd = 61 / 2;
        ////    for (int i = 0; i < fd; i++)
        ////    {
        ////        SendMessageW(p.Handle, WM_APPCOMMAND, p.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
        ////    }
        ////}

        ////private Process p;
        ////private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        ////private const int APPCOMMAND_VOLUME_UP = 0x0A0000;
        ////private const int APPCOMMAND_VOLUME_DOWN = 0x090000;
        ////private const int WM_APPCOMMAND = 0x319;

        #endregion

        #region 窗体控制

        /// <summary>
        /// 初始化窗体
        /// </summary>
        public FrmPlayAdvert26()
        {
            InitializeComponent();
            InitForm();

            #region 启动广告播放线程

            Thread TrdPlayAdvert = new Thread(new ThreadStart(PlayAdvertTrd));
            TrdPlayAdvert.IsBackground = true;
            TrdPlayAdvert.Start();

            #endregion
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_CloseForm = true;

            videoScreenMediaElement.Stop();

            Thread.Sleep(100);
        }

        #endregion

        #region 控件操作

        private void InitForm()
        {
            panelAdvert_Img.Width = videoScreenMediaElement.Width = SystemParameters.PrimaryScreenWidth;
            panelAdvert_Img.Height = videoScreenMediaElement.Height = SystemParameters.PrimaryScreenHeight - 200;

            #region 加载底部控件资源

            int intFontSize = 45;
            if (PubHelper.p_BusinOper.ConfigInfo.Language == BusinessEnum.Language.Zh_CN)
            {
                if (PubHelper.p_BusinOper.ConfigInfo.ScreenType == BusinessEnum.ScreenType.ScreenType26)
                {
                    intFontSize = 56;
                }
                else
                {
                    intFontSize = 80;
                }
            }
            
            tbInfo_Bottom.FontSize = intFontSize;

            tbInfo_Bottom.Text = PubHelper.p_LangOper.GetStringBundle("Main_TouchTip_Sell");

            #endregion

            RefreshAdvertInfo();

            SetPlayVolume();
        }

        private void RefreshAdvertInfo()
        {
            m_AdvertCount = PubHelper.p_BusinOper.AdvertOper.AdvertList.Count;

            m_NowAdvListID = PubHelper.p_BusinOper.ConfigInfo.NowAdvertPlayID;

            m_NextPlayNo = 0;
            GetAdvertInfo();

            m_IsPlayNext = true;
        }

        private void SetPlayVolume()
        {
            string strVolume = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdvertVolume");
            double dblVolume = 0;
            if (!string.IsNullOrEmpty(strVolume))
            {
                dblVolume = Convert.ToDouble(strVolume) / 100;
            }
            videoScreenMediaElement.Volume = dblVolume;
        }

        private void panelInit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void panelBottom_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        //当完成媒体加载时发生
        private void videoScreenMediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {

        }

        private void videoScreenMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            // 获取下一个要播放的广告
            GetNextAdvert();
        }

        private void videoScreenMediaElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 流程处理

        /// <summary>
        /// 广告播放主线程
        /// </summary>
        private void PlayAdvertTrd()
        {
            TimeSpan tsPlayAdvert;

            ////int intImgShowTime = PubHelper.p_BusinOper.ConfigInfo.AdvertImgShowTime * 10;// 图片显示时间，默认5秒
            int intDelayNum = 0;

            bool blnIsFileExist = false;// False：广告文件不存在 True：广告文件存在
            while (!m_CloseForm)
            {
                Thread.Sleep(100);

                if (PubHelper.p_BusinOper.AdvertOper.IsRefreshAdvert)
                {
                    // 广告需要刷新
                    RefreshAdvertInfo();
                    PubHelper.p_BusinOper.AdvertOper.IsRefreshAdvert = false;
                }

                if (m_IsPlayNext)
                {
                    // 检测广告文件是否存在
                    blnIsFileExist = CheckAdvertIsExist();

                    if (blnIsFileExist)
                    {
                        // 广告文件存在
                        #region 播放广告

                        if (m_NowAdvertFormat == BusinessEnum.AdvertType.Image)
                        {
                            #region 播放图片
                            this.tbInfo_Bottom.Dispatcher.Invoke(new Action(() =>
                            {
                                m_IsPlayNext = false;
                                if (advertImg.Visibility == System.Windows.Visibility.Hidden)
                                {
                                    advertImg.Visibility = System.Windows.Visibility.Visible;
                                }
                                if (videoScreenMediaElement.Visibility == System.Windows.Visibility.Visible)
                                {
                                    videoScreenMediaElement.Visibility = System.Windows.Visibility.Hidden;
                                    videoScreenMediaElement.Stop();
                                }
                                LoadImage(m_NowAdvertFilePath);
                            }));
                            #endregion
                        }
                        if (m_NowAdvertFormat == BusinessEnum.AdvertType.Video)
                        {
                            #region 播放视频
                            this.tbInfo_Bottom.Dispatcher.Invoke(new Action(() =>
                            {
                                m_IsPlayNext = false;
                                if (advertImg.Visibility == System.Windows.Visibility.Visible)
                                {
                                    advertImg.Visibility = System.Windows.Visibility.Hidden;
                                }
                                if (videoScreenMediaElement.Visibility == System.Windows.Visibility.Hidden)
                                {
                                    videoScreenMediaElement.Visibility = System.Windows.Visibility.Visible;
                                }
                                videoScreenMediaElement.Source = new Uri(m_NowAdvertFilePath);
                                videoScreenMediaElement.Play();
                                
                            }));
                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        intDelayNum = 0;
                        GetNextAdvert();// 广告文件不存在，播放下一个
                    }
                }

                if (blnIsFileExist)
                {
                    if (m_NowAdvertFormat == BusinessEnum.AdvertType.Image)
                    {
                        intDelayNum++;
                        if (intDelayNum >= m_AdvertImgShowTime * 10)
                        {
                            GetNextAdvert();
                            intDelayNum = 0;
                        }
                    }
                    else
                    {
                        intDelayNum = 0;
                    }
                }

                #region 检测广告是否正常播放，主要针对视频广告

                tsPlayAdvert = DateTime.Now - m_NextAdvertPlayBeginTime;
                if (tsPlayAdvert.TotalSeconds > 600)
                {
                    // 如果10分钟之内广告还没有进行切换，则强制进行切换
                    GetNextAdvert();
                }

                #endregion
            }
        }

        /// <summary>
        /// 播放图片
        /// </summary>
        /// <param name="imgFilePath"></param>
        private void LoadImage(string imgFilePath)
        {
            try
            {
                // 读取图片源文件到
                BinaryReader binReader = new BinaryReader(File.Open(imgFilePath, FileMode.Open));

                FileInfo fileInfo = new FileInfo(imgFilePath);

                byte[] bytes = binReader.ReadBytes((int)fileInfo.Length);

                binReader.Close();

                // 将图片字节赋值给BitmapImage 
                BitmapImage bitmap = new BitmapImage();

                bitmap.BeginInit(); 
                bitmap.StreamSource = new MemoryStream(bytes);

                bitmap.EndInit();

                // 最后给Image控件赋值
                advertImg.Source = bitmap;
            }
            catch
            {
                GetNextAdvert();
            }
        }

        /// <summary>
        /// 获取下一个要播放的广告序号
        /// </summary>
        private void GetNextAdvert()
        {
            m_NextPlayNo++;
            if (m_NextPlayNo >= m_AdvertCount)
            {
                m_NextPlayNo = 0;
            }

            GetAdvertInfo();

            // 记录本次广告轮寻到的时间
            m_NextAdvertPlayBeginTime = DateTime.Now;
            m_IsPlayNext = true;
        }

        private void GetAdvertInfo()
        {
            m_NowAdvertName = PubHelper.p_BusinOper.AdvertOper.AdvertList[m_NextPlayNo].FileName;
            m_NowAdvertType = PubHelper.p_BusinOper.AdvertOper.AdvertList[m_NextPlayNo].FileType;
            m_NowAdvertFormat = PubHelper.p_BusinOper.AdvertOper.AdvertList[m_NextPlayNo].AdvertType;

            if (m_NowAdvertFormat == BusinessEnum.AdvertType.Video)
            {
                string strAdvNo = PubHelper.p_BusinOper.AdvertOper.AdvertPath;
                m_NowAdvertFilePath = "pack://siteoforigin:,,,/advert/" + m_NowAdvListID + "/" + m_NowAdvertName + "." + m_NowAdvertType;
            }
            else
            {
                m_AdvertImgShowTime = PubHelper.p_BusinOper.AdvertOper.AdvertList[m_NextPlayNo].DelayTime;
                m_NowAdvertFilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "advert\\" + m_NowAdvListID + "\\" + m_NowAdvertName + "." + m_NowAdvertType;
            }
        }

        private bool CheckAdvertIsExist()
        {
            string strPicFile = AppDomain.CurrentDomain.BaseDirectory + "\\advert\\" + m_NowAdvListID + "\\" + m_NowAdvertName + "." + m_NowAdvertType;

            return File.Exists(strPicFile);
        }

        #endregion
    }
}
