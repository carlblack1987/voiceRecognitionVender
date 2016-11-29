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
    /// FrmShowCardInfo.xaml 的交互逻辑
    /// </summary>
    public partial class FrmShowCardInfo : Window
    {
        private int m_MonOutTime = 0;
        private bool m_CloseForm = false;

        public FrmShowCardInfo()
        {
            InitializeComponent();

            tbOneText.Text = PubHelper.p_LangOper.GetStringBundle("PosCard_CardNum") + 
                PubHelper.ConvertCardNum_IC(PubHelper.p_BusinOper.UserCardInfo.CardNum_Show);
            tbSecondText.Text = PubHelper.p_LangOper.GetStringBundle("PosCard_BanFee") + 
                PubHelper.p_BusinOper.MoneyIntToString(PubHelper.p_BusinOper.UserCardInfo.BanFee);
            imgShowInfoBg.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/showinfo_bg.png", UriKind.RelativeOrAbsolute));
            tbNoticeInfo.Text = PubHelper.p_LangOper.GetStringBundle("SellGoods_ShowCard_Notice");
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
        }

        /// <summary>
        /// 超时监控主业务流程
        /// </summary>
        private void MonOutTimeTrd()
        {
            // 获取超时时间
            m_MonOutTime = PubHelper.p_BusinOper.ConfigInfo.ShowCardInfoTime;

            int m_OutNum = 0;
            int m_OperNum = 0;

            while (!m_CloseForm)
            {
                Thread.Sleep(20);

                m_OutNum++;
                if (m_OutNum >= 50)
                {
                    m_OutNum = 0;
                    m_OperNum++;

                    try
                    {
                        this.tbOneText.Dispatcher.Invoke(new Action(() =>
                        {
                            if (!m_CloseForm)
                            {
                                if (m_OperNum >= m_MonOutTime)
                                {
                                    // 超时，自动返回

                                    this.Close();
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
}
