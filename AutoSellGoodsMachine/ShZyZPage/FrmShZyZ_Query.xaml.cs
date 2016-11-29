/*
 * 上海志愿者协会—查询
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
using Business.Enum;


namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmShZyZ_Query.xaml 的交互逻辑
    /// </summary>
    public partial class FrmShZyZ_Query : Window
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

        #endregion

        public FrmShZyZ_Query()
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

            string strPicPath = string.Empty;
            bool result = PubHelper.GetFormPubPic("ShZyZ_Query_Bg.png", out strPicPath);
            if (result)
            {
                img_Bg.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }

            result = PubHelper.GetFormPubPic("ShZyZ_Query_ZhiYuanZhe.png", out strPicPath);
            if (result)
            {
                imgShZy_Query_ZhiYuanZhe.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }

            result = PubHelper.GetFormPubPic("ShZyZ_Query_JiJinHui.png", out strPicPath);
            if (result)
            {
                imgShZy_Query_JiJinHui.Source = new BitmapImage(new Uri(strPicPath, UriKind.RelativeOrAbsolute));
            }
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

        private void imgShZy_Query_ZhiYuanZhe_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StopMonOutTime();
            PubHelper.p_Now_WebUrl = BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_ItemList;
            FrmWebBrowser frmWebBrowser = new FrmWebBrowser();
            frmWebBrowser.ShowDialog();
            
            AfreshMonOutTime();
            m_IsMonTime = true;
        }

        private void imgShZy_Query_JiJinHui_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StopMonOutTime();
            PubHelper.p_Now_WebUrl = BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_JiJinHui_ItemList;
            FrmWebBrowser frmWebBrowser = new FrmWebBrowser();
            frmWebBrowser.ShowDialog();
            AfreshMonOutTime();
            m_IsMonTime = true;
        }
    }
}
