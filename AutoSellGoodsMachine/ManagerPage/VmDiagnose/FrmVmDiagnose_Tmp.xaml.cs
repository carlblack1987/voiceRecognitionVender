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

using AutoSellGoodsMachine.Controls;
using Business.Model;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmVmDiagnose_Tmp.xaml 的交互逻辑
    /// </summary>
    public partial class FrmVmDiagnose_Tmp : Window
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

        private VendBoxControl m_CurrentItem = null;

        private int m_ItemCount = 0;

        private int m_EachRowNum = 3;// 每行显示的最大数量

        private int m_MaxRowNum = 2;// 最大行数

        List<VendBoxCodeModel> m_CurrentItemList = new List<VendBoxCodeModel>();

        public FrmVmDiagnose_Tmp()
        {
            InitializeComponent();

            InitForm();

            Loaded += (MainWindow_Loaded);
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose_DoorTmp");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPage();

            #region 启动超时控制工作线程

            Thread TrdMonOutTime = new Thread(new ThreadStart(MonOutTimeTrd));
            TrdMonOutTime.IsBackground = true;
            TrdMonOutTime.Start();

            m_IsMonTime = true;

            #endregion
        }

        private void LoadPage()
        {
            m_ItemCount = 0;
            m_CurrentItemList = PubHelper.p_BusinOper.AsileOper.VendBoxList;
            m_ItemCount = m_CurrentItemList.Count;

            btnCancel.IsEnabled = true;

            CreateProduct();
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

        #region 创建内容控件
        /// <summary>
        /// 创建内容控件
        /// </summary>
        private void CreateProduct()
        {
            panelItem.Children.Clear();

            int index = 0;

            string strStatusText = string.Empty;
            string strTmpModel = string.Empty;
            string strTmpNowValue = string.Empty;// 当前温度
            string strTmpTarValue = string.Empty;// 目标温度

            string strTmpNowTitle = PubHelper.p_LangOper.GetStringBundle("Pub_Device_Tmp");
            string strTmpTarTitle = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_TargetTmp");
            for (int i = 0; i < m_MaxRowNum; i++)
            {
                for (int j = 0; j < m_EachRowNum; j++)
                {
                    if (index < m_CurrentItemList.Count)
                    {
                        VendBoxControl productControl = new VendBoxControl()
                        {
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                            Margin = new Thickness(5)
                        };

                        Grid.SetRow(productControl, i);
                        Grid.SetColumn(productControl, j);
                        panelItem.Children.Add(productControl);

                        ////productControl.MouseLeftButtonUp += (ItemWayChecked);

                        productControl.SetCurrentItem(m_CurrentItemList[index]);

                        strTmpModel = DictionaryHelper.Dictionary_TmpType(index);
                        strTmpNowValue = DictionaryHelper.Dictionary_NowTmp(index, true);
                        productControl.SetOneText(DictionaryHelper.Dictionary_VendBoxName(m_CurrentItemList[index].VendBoxCode) +
                            "【" + strTmpModel + "】" +
                            "\r\n" +
                            strTmpNowTitle +  "【" + strTmpNowValue + "】" + "\r\n" +
                            strTmpTarTitle + "【" + m_CurrentItemList[index].TargetTmp + PubHelper.TMP_UNIT + "】");

                        ////productControl.SetSecondText(strStatusText);
                    }
                    index++;
                }
            }
        }

        #endregion

        #region 窗口控件操作

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_BusinOper.GoodsOper.CurrentGoods = null;
            this.Close();
        }

        #endregion

        #region 超时监控业务控制

        /// <summary>
        /// 超时监控主业务流程
        /// </summary>
        private void MonOutTimeTrd()
        {
            // 获取超时时间
            m_MonOutTime = Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("TermStatusShowTime"));

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
                            this.tbTitle.Dispatcher.Invoke(new Action(() =>
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
                                        ////if (tbOutTime.Visibility == System.Windows.Visibility.Hidden)
                                        ////{
                                        ////    tbOutTime.Visibility = System.Windows.Visibility.Visible;
                                        ////}
                                        ////// 显示剩余时间提示
                                        ////tbOutTime.Text = (m_MonOutTime - m_OperNum + 1).ToString();

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
        }

        #endregion
    }
}
