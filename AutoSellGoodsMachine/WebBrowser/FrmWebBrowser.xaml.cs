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
using System.Windows.Forms;
using System.ComponentModel;

using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmWebBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class FrmWebBrowser : Window
    {
        #region 变量声明

        System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
        System.Windows.Forms.WebBrowser web = new System.Windows.Forms.WebBrowser();

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

        #region 窗体操作

        public FrmWebBrowser()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            #region 设置浏览器控件属性

            web.AllowWebBrowserDrop = false;// 禁止拖放
            web.WebBrowserShortcutsEnabled = false;// 禁止使用快捷键
            web.IsWebBrowserContextMenuEnabled = false;// 禁止右键上下文菜单
            web.ScriptErrorsSuppressed = true;// 禁止弹出脚本错误提示框

            web.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);

            web.NewWindow += new System.ComponentModel.CancelEventHandler(webBrowser_NewWindow);

            
            host.Child = web;
            web.Location = new System.Drawing.Point(0);
            this.panelTop.Children.Add(host);

            LoadWebIndex();

            #endregion

            #region 计算各区域尺寸

            double scrHeight = SystemParameters.PrimaryScreenHeight;
            panelTop.Height = scrHeight - 100;

            web.Height = Convert.ToInt32(panelTop.Height);
            web.Width = Convert.ToInt32(SystemParameters.PrimaryScreenWidth);

            #endregion
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

            btnCancel.IsEnabled = btnGoBack.IsEnabled = btnIndex.IsEnabled = false;
            web.Dispose();
        }

        #endregion

        #region 浏览器控件

        /// <summary>
        /// 加载浏览器主页面
        /// </summary>
        private void LoadWebIndex()
        {
            try
            {
                web.Url = new Uri(ConvertWebUrl());
            }
            catch
            {
            }
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            AfreshMonOutTime();
            //将所有的链接的目标，指向本窗体    
            foreach (HtmlElement archor in this.web.Document.Links)
            {
                archor.SetAttribute("target", "_self");
            }
            //将所有的FORM的提交目标，指向本窗体 
            foreach (HtmlElement form in this.web.Document.Forms)
            {
                form.SetAttribute("target", "_self");
            }
        }

        private void webBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            AfreshMonOutTime();
            e.Cancel = true;

            web.Navigate(this.web.StatusText);
        }

        #endregion

        #region 窗口控件操作

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnIndex_Click(object sender, RoutedEventArgs e)
        {
            AfreshMonOutTime();
            LoadWebIndex();
        }

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            AfreshMonOutTime();
            web.GoBack();
        }

        #endregion

        #region 超时监控业务控制

        /// <summary>
        /// 超时监控主业务流程
        /// </summary>
        private void MonOutTimeTrd()
        {
            // 获取超时时间
            m_MonOutTime = Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("WebUrlOutTime")) * 60;

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

        #region 私有业务函数

        /// <summary>
        /// 转换WebUrl链接地址
        /// </summary>
        /// <returns></returns>
        private string ConvertWebUrl()
        {
            string strUrl = "http://www.kimma.com.cn";
            switch (PubHelper.p_Now_WebUrl)
            {
                case BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_XieHui:// 上海志愿者协会网址
                    strUrl = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Url_ZyZXieHui_Index");
                    break;
                case BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_JiJinHui:// 上海志愿者基金会网址
                    strUrl = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Url_ZyZJiJinHui_Index");
                    break;
                case BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_ItemList:// 上海志愿者活动列表
                    strUrl = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Url_ZyZXieHui_Item");
                    break;
                case BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_JiJinHui_ItemList:// 上海志愿基金会公益查询
                    strUrl = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue_Third("Url_ZyZJiJinHui_Item");
                    break;
            }
            return strUrl;
        }

        #endregion
    }
}
