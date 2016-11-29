#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：管理员登陆
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

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
    /// FrmLogin.xaml 的交互逻辑
    /// </summary>
    public partial class FrmLogin : Window
    {
        #region 变量声明

        /// <summary>
        /// 输入内容
        /// </summary>
        private string m_Input = string.Empty;

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

        public FrmLogin()
        {
            InitializeComponent();
            InitForm();
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
            m_MonOutTime = Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("InputPwdOutTime"));

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

        #region

        private void InitForm()
        {
            tbInput.Text = string.Empty;
            m_Input = string.Empty;
            tbLoginTitle.Text = PubHelper.p_LangOper.GetStringBundle("Login_Title");
            tbPassWord.Text = PubHelper.p_LangOper.GetStringBundle("Login_Password");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnOk.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Ok");
        }

        #endregion

        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            AfreshMonOutTime();
            m_IsMonTime = true;

            if (m_Input.Length >= 6)
            {
                return;
            }
            string strInput = DictionaryHelper.Dictionary_Input(sender.ToString());
            m_Input = m_Input + strInput;
            tbInput.Text = tbInput.Text + "*";
        }

        /// <summary>
        /// 按钮—返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 按钮—确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            AfreshMonOutTime();
            m_IsMonTime = true;

            if (string.IsNullOrEmpty(m_Input))
            {
                return;
            }

            // 验证密码
            bool result = PubHelper.p_BusinOper.CheckUserLogin(PubHelper.p_BusinOper.CheckDataOper.Md532(m_Input, "1"));
            if (!result)
            {
                PubHelper.p_CheckLoginResult = false;
                StopMonOutTime();
                // 验证失败
                this.Opacity = PubHelper.OPACITY_GRAY;
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_InvalidPwd"),PubHelper.MsgType.Ok);
                this.Opacity = PubHelper.OPACITY_NORMAL;
                m_Input = tbInput.Text = string.Empty;
                AfreshMonOutTime();
                m_IsMonTime = true;
            }
            else
            {
                m_CloseForm = true;
                PubHelper.p_CheckLoginResult = true;
                if ((m_Input == "000000") || (m_Input == "999999"))
                {
                    // 如果密码为初始密码，则进行提示修改密码
                    PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Login_Tip_SimplePwd"), PubHelper.MsgType.Ok);
                }

                if (PubHelper.p_LoginType == "0")
                {
                    // 如果是正常途径登陆，则显示管理菜单
                    this.Hide();
                    FrmMenu frmMenu = new FrmMenu();
                    frmMenu.ShowDialog();
                }
                this.Close();
            }
        }

        /// <summary>
        /// 按钮—清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearInput_Click(object sender, RoutedEventArgs e)
        {
            AfreshMonOutTime();
            m_IsMonTime = true;
            tbInput.Text = string.Empty;
            m_Input = string.Empty;
        }
    }
}
