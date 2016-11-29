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
using System.Windows.Threading;

using AutoSellGoodsMachine.Controls;
using Business.Model;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmStat.xaml 的交互逻辑
    /// </summary>
    public partial class FrmStat : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// 当前的货柜编号
        /// </summary>
        private string m_VendBoxCode = "1";

        private string m_Title_SaleNum = string.Empty;
        private string m_Title_SaleMoney = string.Empty;
        private string m_NoGoodsTitle = string.Empty;

        private GoodsWayProduct currentGoodsWay = null;
        private int currentTrayIndex = 0;
        private int m_AsileCount = 0;
        private int m_TrayCount = 0;

        public FrmStat()
        {
            InitializeComponent();

            InitForm();

            Loaded += (FrmStat_Loaded);
        }

        private void FrmStat_Loaded(object sender, RoutedEventArgs e)
        {
            //Tick是执行的事件           
            timer.Tick += new EventHandler(Timer1_Tick);
            //Internal是间隔时间
            timer.Interval = TimeSpan.FromSeconds(0.01);

            timer.Start();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            m_VendBoxCode = "1";// 默认第一个柜子
            CreateBox();//
            ////CreateTray();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnCashStat.Content = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_CashStat_Title");

            tbAsileTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_AsileTitle");
            tbTotalSale_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_TotalTitle");
            tbTotalSaleNum_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_TotalSaleNum");
            tbTotalSaleMoney_Title.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_TotalSaleMoney");

            m_Title_SaleNum = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_SaleNum");
            m_Title_SaleMoney = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stat_SaleMoney");
            m_NoGoodsTitle = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_Stock_NoGoods");

            // 显示总销售统计数据
            string _totalSaleNum = "0";
            string _totalSaleMoney = "0";
            bool result = PubHelper.p_BusinOper.QueryStatTotalData(out _totalSaleNum, out _totalSaleMoney);

            tbTotalSaleNum_Value.Text = _totalSaleNum;
            tbTotalSaleMoney_Value.Text = PubHelper.p_BusinOper.MoneyIntToString(_totalSaleMoney);
        }

        /// <summary>
        /// 创建货柜
        /// </summary>
        private void CreateBox()
        {
            var map = new Dictionary<int, string>();
            for (int i = 1; i < 10; i++)
            {
                map.Add(i, DictionaryHelper.Dictionary_VendBoxName(i.ToString()));
            }

            int intBoxCount = PubHelper.p_BusinOper.AsileOper.VendBoxList.Count;
            if (intBoxCount > 1)
            {
                #region 有多个柜子
                for (int i = 1; i <= intBoxCount; i++)
                {
                    panelBox.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    var palletButton = new RadioButton()
                    {
                        Focusable = false,

                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,

                        VerticalAlignment = System.Windows.VerticalAlignment.Center,

                        Style = App.Current.Resources["VendBoxButtonStyle"] as Style,

                        Foreground = new SolidColorBrush(Colors.White),

                        Padding = new Thickness(20, 4, 20, 8),

                        FontSize = 22,

                        Tag = i
                    };

                    Grid.SetColumn(palletButton, i - 1);

                    palletButton.Checked += (VendBoxButtonChecked);

                    palletButton.Content = map[i];

                    panelBox.Children.Add(palletButton);

                    if (i == 1)
                    {
                        palletButton.IsChecked = true;
                    }
                }
                #endregion
            }
            else
            {
                // 只有一个柜子
                CreateTray();
            }
        }

        /// <summary>
        /// 创建托盘
        /// </summary>
        private void CreateTray()
        {
            var map = new Dictionary<int, string>();
            for (int i = 1; i < 11; i++)
            {
                map.Add(i, PubHelper.p_BusinOper.ChangeTray(i.ToString()));
            }

            int intVendTrayMaxNum = 0;
            if (PubHelper.p_BusinOper.AsileOper.VendBoxList.Count == 1)
            {
                // 只有一个柜子
                intVendTrayMaxNum = PubHelper.p_BusinOper.AsileOper.TrayMaxNum_Total;
            }
            else
            {
                // 多个柜子
                for (int i = 0; i < PubHelper.p_BusinOper.AsileOper.AsileList.Count; i++)
                {
                    if (PubHelper.p_BusinOper.AsileOper.AsileList[i].VendBoxCode == m_VendBoxCode)
                    {
                        if (PubHelper.p_BusinOper.AsileOper.AsileList[i].TrayIndex >= intVendTrayMaxNum)
                        {
                            intVendTrayMaxNum = PubHelper.p_BusinOper.AsileOper.AsileList[i].TrayIndex;
                        }
                    }
                }
            }

            panelTray.ColumnDefinitions.Clear();
            panelTray.Children.Clear();

            for (int i = 1; i <= intVendTrayMaxNum + 1; i++)
            {
                panelTray.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                var palletButton = new RadioButton()
                {
                    Focusable = false,

                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,

                    VerticalAlignment = System.Windows.VerticalAlignment.Center,

                    Style = App.Current.Resources["PalletButtonStyle"] as Style,

                    Foreground = new SolidColorBrush(Colors.White),

                    Padding = new Thickness(20, 8, 20, 8),

                    FontSize = 32,

                    Tag = i - 1
                };

                Grid.SetColumn(palletButton, i - 1);

                palletButton.Checked += (PalletButtonChecked);

                palletButton.Content = map[i];

                panelTray.Children.Add(palletButton);

                if (i == 1)
                {
                    palletButton.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 创建货道
        /// </summary>
        private void CreateAsile(List<AsileModel> products)
        {
            panelAsile.Children.Clear();

            int index = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    GoodsWayProduct productControl = new GoodsWayProduct()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                        Margin = new Thickness(5)
                    };
                    ////productControl.SetBackBackGroud(App.Current.Resources["AsileNormal"] as Brush);
                    ////productControl.SetCheckBackGround(App.Current.Resources["AsileTest"] as Brush);
                    Grid.SetRow(productControl, i);
                    Grid.SetColumn(productControl, j);
                    panelAsile.Children.Add(productControl);

                    ////////productControl.MouseLeftButtonUp += (GoodsWayChecked);

                    if (index < products.Count)
                    {
                        productControl.SetCurrentGoodsWayProduct(products[index]);
                        productControl.SetOneText(products[index].PaCode +
                            "【" + m_Title_SaleNum + "  " + products[index].SaleNum + "】");

                        productControl.SetSecondText(m_Title_SaleMoney + "  " +
                            PubHelper.p_BusinOper.MoneyIntToString(products[index].SaleMoney.ToString()));
                    }
                    else
                    {
                        productControl.SetNoGoods(m_NoGoodsTitle);
                    }
                    index++;
                }
            }

            m_AsileCount = panelAsile.Children.Count;
        }

        /// <summary>
        /// 货柜选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VendBoxButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                currentTrayIndex = Convert.ToInt32((sender as RadioButton).Tag);
                m_VendBoxCode = currentTrayIndex.ToString();
                CreateTray();
            }
        }

        /// <summary>
        /// 托盘选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PalletButtonChecked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                currentTrayIndex = Convert.ToInt32((sender as RadioButton).Tag);
                var currentRowProducts = PubHelper.p_BusinOper.AsileOper.AsileList.Where(item => item.TrayIndex == currentTrayIndex).ToList();
                currentRowProducts = currentRowProducts.Where(item => item.VendBoxCode == m_VendBoxCode).ToList();
                CreateAsile(currentRowProducts);
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCashStatClick(object sender, RoutedEventArgs e)
        {
            this.Opacity = PubHelper.OPACITY_GRAY;
            FrmStat_Cash frmStat_Cash = new FrmStat_Cash();
            frmStat_Cash.ShowDialog();
            this.Opacity = PubHelper.OPACITY_NORMAL;
        }
    }
}
