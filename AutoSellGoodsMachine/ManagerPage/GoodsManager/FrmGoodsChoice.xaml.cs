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

using AutoSellGoodsMachine.Controls;
using Business.Model;

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmGoodsChoice.xaml 的交互逻辑
    /// </summary>
    public partial class FrmGoodsChoice : Window
    {
        private GoodsButton currentGoods = null;

        /// <summary>
        /// 选择商品事件
        /// </summary>
        ////public event EventHandler<SelectProductEventArgs> SelectProductEvent;

        private int m_GoodsCount = 0;

        private int m_PageCount = 0;

        private int m_CurrentPage = 1;

        public FrmGoodsChoice()
        {
            InitializeComponent();

            InitForm();

            Loaded += (MainWindow_Loaded);
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_ChoiceGoods");
            btnPrevious.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Page_Preview");
            btnDown.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Page_Next");
            btnOk.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Ok");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");

            m_GoodsCount = PubHelper.p_BusinOper.GoodsOper.GoodsList_Total.Count;

            if (m_GoodsCount == 0)
            {
                btnPrevious.IsEnabled = btnDown.IsEnabled = false;
            }

            btnOk.IsEnabled = false;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetSize();

            InitProductPage();

            InitCurrentPageProduct(m_CurrentPage);

            ButtonEnable();
        }

        /// <summary>
        /// 根据分辨率设置大小(该代码在这里为了在电脑上显示效果美化 实际终端请去掉)
        /// </summary>
        private void SetSize()
        {
            mainPanel.Width = 700;// this.ActualWidth * 0.9;
        }

        #region 初始化页数
        /// <summary>
        /// 初始化页数
        /// </summary>
        private void InitProductPage()
        {
            m_PageCount = ((m_GoodsCount / 15) +
                ((m_GoodsCount % 15) > 0 ? 1 : 0));

            tbCountPage.Text = m_PageCount.ToString();
        }
        #endregion

        #region 创建商品控件
        /// <summary>
        /// 创建商品控件
        /// </summary>
        private void CreateProduct(List<GoodsModel> products)
        {
            panelProduct.Children.Clear();

            int index = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (index < products.Count)
                    {
                        GoodsButton productControl = new GoodsButton()
                        {
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                            Margin = new Thickness(5)
                        };

                        Grid.SetRow(productControl, i);
                        Grid.SetColumn(productControl, j);
                        panelProduct.Children.Add(productControl);

                        productControl.MouseLeftButtonUp += (GoodsWayChecked);

                        productControl.SetCurrentGoods(products[index]);

                        productControl.SetOneText(products[index].McdCode);
                        productControl.SetSecondText(products[index].McdName);
                    }
                    index++;
                }
            }
        }

        /// <summary>
        /// 产品选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoodsWayChecked(object sender, MouseButtonEventArgs e)
        {
            var goodsWay = (sender as GoodsButton);
            if (goodsWay == currentGoods)
                return;
            if (goodsWay != null)
            {
                if (currentGoods != null)
                {
                    currentGoods.ToNormal();
                }
                goodsWay.ToCheck();
                currentGoods = goodsWay;

                btnOk.IsEnabled = true;
            }
        }

        #endregion

        #region 上一页 下一页

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            InitCurrentPageProduct(m_CurrentPage + 1);
            ButtonEnable();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            InitCurrentPageProduct(m_CurrentPage - 1);
            ButtonEnable();
        }

        /// <summary>
        /// 按钮状态控制
        /// </summary>
        private void ButtonEnable()
        {
            ////btnPrevious.IsEnabled = (!(m_CurrentPage == 1)) || (m_PageCount == 1);
            ////btnDown.IsEnabled = !(m_CurrentPage == m_PageCount);

            if (m_PageCount == 1)
            {
                btnPrevious.IsEnabled = btnDown.IsEnabled = false;
            }
            else
            {
                if (m_CurrentPage == 1)
                {
                    btnPrevious.IsEnabled = false;
                    btnDown.IsEnabled = true;
                }
                else if (m_CurrentPage == m_PageCount)
                {
                    btnPrevious.IsEnabled = true;
                    btnDown.IsEnabled = false;
                }
                else
                {
                    btnPrevious.IsEnabled = true;
                    btnDown.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 初始化当前页的商品
        /// </summary>
        /// <param name="i"></param>
        private void InitCurrentPageProduct(int i)
        {
            m_CurrentPage = i;

            tbCurrentPage.Text = i.ToString();

            int startIndex = (i - 1) * 15;

            int endIndex = i * 15;

            List<GoodsModel> currentSource = new List<GoodsModel>();

            for (int index = 0; index < m_GoodsCount; index++)
            {
                if (index >= startIndex && index < endIndex)
                {
                    currentSource.Add(PubHelper.p_BusinOper.GoodsOper.GoodsList_Total[index]);
                }
            }

            CreateProduct(currentSource);
        }
        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_BusinOper.GoodsOper.CurrentGoods = null;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (currentGoods.CurrentGoods != null)
            {
                PubHelper.p_BusinOper.GoodsOper.CurrentGoods = currentGoods.CurrentGoods;
                this.Close();
            }
        }
    }

}
