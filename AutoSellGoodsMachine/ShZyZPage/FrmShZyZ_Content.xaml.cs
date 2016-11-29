/*
 * 上海志愿者协会—项目介绍
 * 
*/
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
    /// FrmShZyZ_Content.xaml 的交互逻辑
    /// </summary>
    public partial class FrmShZyZ_Content : Window
    {
        #region 变量声明

        /// <summary>
        /// 是否关闭窗体 False：不关闭 True：关闭
        /// </summary>
        private bool m_CloseForm = false;

        /// <summary>
        /// 是否启动操作时间超时监控
        /// </summary>
        private bool m_IsMonTime = false;

        /// <summary>
        /// 监控操作超时参数
        /// </summary>
        private int m_OutNum = 0;
        private int m_OperNum = 0;

        /// <summary>
        /// 监控操作的超时时间，以秒为单位
        /// </summary>
        private int m_MonOutTime = 0;

        private int m_PageIndex = 0;

        #endregion

        public FrmShZyZ_Content()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            #region 计算各区域尺寸

            ////double scrHeight = SystemParameters.PrimaryScreenHeight;
            ////panel_Content.Height = scrHeight - 100;

            #endregion

            m_PageIndex = 1;
            btnUp.IsEnabled = false;
            btnDown.IsEnabled = true;

            LoadBgImage();
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 启动超时控制工作线程

            Thread TrdMonOutTime = new Thread(new ThreadStart(MonOutTimeTrd));
            TrdMonOutTime.IsBackground = true;
            TrdMonOutTime.Start();

            m_IsMonTime = true;

            #endregion
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_IsMonTime = false;

            m_CloseForm = true;
        }

        private void LoadBgImage()
        {
            string strPicPath = string.Empty;
            string strPicName = string.Empty;
            switch (m_PageIndex)
            {
                case 1:// 第一页
                    strPicName = "ShZyZ_Content_Bg.png";
                    break;
                case 2:// 第二页
                    strPicName = "ShZyZ_Content_Bg_2.png";
                    break;
                case 3:// 第三页
                    strPicName = "ShZyZ_Content_Bg_3.png";
                    break;
                default:
                    strPicName = "ShZyZ_Content_Bg.png";
                    break;
            }

            try
            {
                bool result = PubHelper.GetFormPubPic(strPicName, out strPicPath);
                if (result)
                {
                    img_Bg.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
                }
            }
            catch
            {
            }
        }

        #region 超时监控业务控制

        /// <summary>
        /// 超时监控主业务流程
        /// </summary>
        private void MonOutTimeTrd()
        {
            // 获取超时时间
            m_MonOutTime = Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("OtherBrowseOutTime")) * 60;

            while (!m_CloseForm)
            {
                Thread.Sleep(20);

                if (!m_IsMonTime)
                {
                    // 重新开始超时监控
                    AfreshMonOutTime();
                }
                else
                {
                    m_OutNum++;
                    if (m_OutNum >= 50)
                    {
                        m_OutNum = 0;
                        m_OperNum++;

                        try
                        {
                            this.tbOutTime.Dispatcher.Invoke(new Action(() =>
                            {
                                if (!m_CloseForm)
                                {
                                    if (m_OperNum > m_MonOutTime)
                                    {
                                        // 超时，自动返回
                                        // 重新开始超时监控
                                        AfreshMonOutTime();
                                        m_IsMonTime = true;
                                        //OperStepKind();

                                        this.Close();
                                        //Application.DoEvents();
                                    }
                                    else
                                    {
                                        if (tbOutTime.Visibility == System.Windows.Visibility.Hidden)
                                        {
                                            tbOutTime.Visibility = System.Windows.Visibility.Visible;
                                        }
                                        // 显示剩余时间提示
                                        tbOutTime.Text = (m_MonOutTime - m_OperNum + 1).ToString();

                                        DispatcherHelper.DoEvents();
                                    }
                                }
                            }));
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 重新开始超时监控
        /// </summary>
        private void AfreshMonOutTime()
        {
            m_OutNum = 0;
            m_OperNum = 0;
        }

        /// <summary>
        /// 停止超时监控
        /// </summary>
        private void StopMonOutTime()
        {
            m_IsMonTime = false;
            if (tbOutTime.Visibility == System.Windows.Visibility.Visible)
            {
                tbOutTime.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 按钮—下翻
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            AfreshMonOutTime();
            if (m_PageIndex >= 3)
            {
                m_PageIndex = 3;
                return;
            }
            m_PageIndex++;
            LoadBgImage();
            if (m_PageIndex >= 3)
            {
                btnUp.IsEnabled = true;
                btnDown.IsEnabled = false;
            }
            else
            {
                btnUp.IsEnabled = btnDown.IsEnabled = true;
            }
        }

        /// <summary>
        /// 按钮—上翻
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            AfreshMonOutTime();
            if (m_PageIndex <= 1)
            {
                m_PageIndex = 1;
                return;
            }
            m_PageIndex--;
            LoadBgImage();
            if (m_PageIndex <= 1)
            {
                btnUp.IsEnabled = false;
                btnDown.IsEnabled = true;
            }
            else
            {
                btnUp.IsEnabled = true;
                btnDown.IsEnabled = true;
            }
        }
    }
}
