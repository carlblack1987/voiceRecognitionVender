#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：菜单显示
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

using Business.Enum;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmMenu.xaml 的交互逻辑
    /// </summary>
    public partial class FrmMenu : Window
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

        public FrmMenu()
        {
            InitializeComponent();

            // 初始化界面元素
            InitFormControl();
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

        private void InitFormControl()
        {
            tbMenuTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_LeftTitle");
            Menu_AsileCfg.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileCfg");// 货道设置

            #region 制冷或加热

            ////string strRefCfgTitle = string.Empty;
            ////switch (PubHelper.p_BusinOper.ConfigInfo.TmpControlModel)
            ////{
            ////    case BusinessEnum.TmpControlModel.Refrigeration:// 制冷
            ////        strRefCfgTitle = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_Ref");// 制冷设置
            ////        Menu_RefCfg.Style = (Style)this.FindResource("Menu_RefCfg_Ref");
            ////        break;
            ////    default:
            ////        strRefCfgTitle = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_Heating");// 加热设置
            ////        Menu_RefCfg.Style = (Style)this.FindResource("Menu_RefCfg_Heating");
            ////        break;
            ////}

            Menu_RefCfg.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_RefCfg_Ref");

            #endregion
            Menu_Energy.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Energy");// 节能设置
            Menu_SaleModel.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_SaleModel");// 销售模式
            Menu_Goods.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Goods");// 商品管理
            Menu_Stock.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock");// 库存设置
            Menu_Stat.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat");// 销售统计
            Menu_Diagnose.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Diagnose");// 机器诊断
            Menu_AsileTest.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AsileTest");// 货道测试
            Menu_BaseCfg.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_BaseCfg");// 基本设置
            Menu_EditPwd.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_EditPwd");// 修改密码
            Menu_AdvanCfg.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_AdvanCfg");// 高级设置
            Menu_Cancel.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Cancel");// 返回系统
            Menu_Close.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Close");// 关闭系统
            Menu_Advert.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Advert");// 广告设置
            Menu_DeviceCfg.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_DeviceCfg");// 硬件设备维护
        }

        /// <summary>
        /// 菜单—货道设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_AsileCfg_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            FrmAsileCfg frmAsileCfg = new FrmAsileCfg();
            frmAsileCfg.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—制冷设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_RefCfg_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            FrmRefriCfg frmRefriCfg = new FrmRefriCfg();
            frmRefriCfg.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—节能设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Energy_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            bool blnIsControlCircle = false;
            for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.VendBoxList.Count; i++)
            {
                if (PubHelper.p_BusinOper.AsileOper.VendBoxList[i].IsControlCircle == "1")
                {
                    blnIsControlCircle = true;
                    break;
                }
            }

            if (!blnIsControlCircle)
            {
                // 如果没有回路控制回路板，则不能使用节能设置
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoControlCircle"), PubHelper.MsgType.Ok);
                m_IsMonTime = true;
                return;
            }

            FrmEnergyCfg frmEnergyCfg = new FrmEnergyCfg();
            frmEnergyCfg.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—销售模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_SaleModel_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            FrmSaleModel frmSaleModel = new FrmSaleModel();
            frmSaleModel.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—商品管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Goods_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            FrmGoodsManager frmGoodsManager = new FrmGoodsManager();
            frmGoodsManager.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—库存设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Stock_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            if (PubHelper.p_BusinOper.ConfigInfo.IsRunStock == BusinessEnum.ControlSwitch.Stop)
            {
                // 不启用库存
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_StockStop"), PubHelper.MsgType.Ok);
                m_IsMonTime = true;
                return;
            }
            FrmStock frmStock = new FrmStock();
            frmStock.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—销售统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Stat_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            FrmStat frmStat = new FrmStat();
            frmStat.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—机器诊断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Diagnose_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            FrmVmDiagnose frmVmDiagnose = new FrmVmDiagnose();
            frmVmDiagnose.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—货道测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_AsileTest_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.AsileTest;
            StopMonOutTime();
            ////FrmGoodsTypeTest frmGoodsTypeTest = new FrmGoodsTypeTest();
            ////frmGoodsTypeTest.ShowDialog();
            FrmAsileTest frmAsileTest = new FrmAsileTest();
            frmAsileTest.ShowDialog();
            m_IsMonTime = true;
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
        }

        /// <summary>
        /// 菜单—硬件设备维护
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_DeviceCfg_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.DeviceCfg;
            StopMonOutTime();
            FrmDeviceCfg frmDeviceCfg = new FrmDeviceCfg();
            frmDeviceCfg.ShowDialog();
            m_IsMonTime = true;
            PubHelper.p_BusinOper.OperStep = BusinessEnum.OperStep.Main;
        }

        /// <summary>
        /// 菜单—广告设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Advert_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            FrmAdvert frmAdvert = new FrmAdvert();
            frmAdvert.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_EditPwd_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmEditPwd frmEditPwd = new FrmEditPwd();
            frmEditPwd.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—高级设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_AdvanCfg_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            if (PubHelper.p_BusinOper.UserType != Business.Enum.BusinessEnum.UserType.SystemUser)
            {
                // 不是厂商管理员
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_NoLimit"), PubHelper.MsgType.Ok);
                m_IsMonTime = true;
                return;
            }

            FrmAdvanCfg frmAdvanCfg = new FrmAdvanCfg();
            frmAdvanCfg.ShowDialog();
            m_IsMonTime = true;
        }

        /// <summary>
        /// 菜单—返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 菜单—关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Menu_Close_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_IsShutDown = true;
            this.Close();
        }

        private void Menu_BaseCfg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrmBaseCfg frmBaseCfg = new FrmBaseCfg();
            frmBaseCfg.ShowDialog();
            // 检测语言是否更改
            if (PubHelper.p_IsRefreshLanguage)
            {
                InitFormControl();
            }
        }

        private void Menu_BaseCfg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrmBaseCfg frmBaseCfg = new FrmBaseCfg();
            frmBaseCfg.ShowDialog();
            // 检测语言是否更改
            if (PubHelper.p_IsRefreshLanguage)
            {
                InitFormControl();
            }
        }

        private void Menu_BaseCfg_Click(object sender, RoutedEventArgs e)
        {
            StopMonOutTime();
            FrmBaseCfg frmBaseCfg = new FrmBaseCfg();
            frmBaseCfg.ShowDialog();
            // 检测语言是否更改
            if (PubHelper.p_IsRefreshLanguage)
            {
                InitFormControl();
            }
            m_IsMonTime = true;
        }

        #region 超时监控业务控制

        /// <summary>
        /// 超时监控主业务流程
        /// </summary>
        private void MonOutTimeTrd()
        {
            // 获取超时时间，2分钟超时
            m_MonOutTime = 120;// Convert.ToInt32(PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("InputPwdOutTime"));

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
                            this.tbMenuTitle.Dispatcher.Invoke(new Action(() =>
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
