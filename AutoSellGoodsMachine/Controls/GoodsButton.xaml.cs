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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

using Business;
using Business.Model;

namespace AutoSellGoodsMachine.Controls
{
    /// <summary>
    /// GoodsButton.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsButton : UserControl
    {
        private bool isCheck = false;

        private GoodsModel currentGoods;

        public GoodsModel CurrentGoods
        {
            get { return currentGoods; }
            private set { currentGoods = value; }
        }

        public GoodsButton()
        {
            InitializeComponent();

            Loaded += (GoodsButton_Loaded);
        }

        void GoodsButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (currentGoods != null)
            {
                SetGoodsPic();
            }

            if (isCheck)
            {
                ToCheck();
            }
            else
            {
                ToNormal();
            }
        }

        /// <summary>
        /// 设置当前的格子中容纳的商品
        /// </summary>
        /// <param name="mcdInfo"></param>
        internal void SetCurrentGoods(GoodsModel goodsInfo)
        {
            currentGoods = goodsInfo;
        }

        public void SetGoodsPic()
        {
            string strMcdPic = SkinHelper.p_SkinName + "ProductPage/nopapic.png";// 商品图片不存在，采用默认图片

            string strPaPicFile = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\GoodsPic\\" + currentGoods.PicName;

            if (File.Exists(strPaPicFile))
            {
                // 商品图片存在
                strMcdPic = "pack://siteoforigin:,,,/Images/GoodsPic/" + currentGoods.PicName;
            }
            imageProduct.Source = new BitmapImage(new Uri(strMcdPic));
        }

        /// <summary>
        /// 由于在触发load后才能设置是否为选中状态.所有加该方法
        /// </summary>
        /// <param name="p"></param>
        internal void IsDefaultCheck(bool p)
        {
            isCheck = p;
        }

        public void SetNoGoods(string noGoodsTitle)
        {
            tbNull.Text = noGoodsTitle;
        }

        /// <summary>
        /// 默认
        /// </summary>
        public void ToNormal()
        {
            if (currentGoods == null)
            {
                VisualStateManager.GoToState(this, "Null", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Product", true);
            }
        }

        /// <summary>
        /// 选中
        /// </summary>
        public void ToCheck()
        {
            if (currentGoods == null)
            {
                VisualStateManager.GoToState(this, "ClickNull", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Click", true);
            }
        }

        /// <summary>
        /// 设置背景颜色
        /// </summary>
        /// <param name="brush"></param>
        public void SetBackBackGroud(Brush brush)
        {
            panel.Background = brush;
        }

        /// <summary>
        /// 设置选中背景颜色
        /// </summary>
        /// <param name="brush"></param>
        public void SetCheckBackGround(Brush brush)
        {
            B_Pressed.Background = brush;
        }

        #region 库存设置

        /// <summary>
        /// 设置第一行文字行内容
        /// </summary>
        public void SetOneText(string goodsWay)
        {
            if (CurrentGoods != null)
            {
                tbOneRow.Text = goodsWay;
            }
        }

        /// <summary>
        /// 设置第二文字行内容
        /// </summary>
        /// <param name="count"></param>
        public void SetSecondText(string textValue)
        {
            if (CurrentGoods != null)
            {
                int intFont = 20;
                if (textValue.Length > 6)
                {
                    intFont = 15;
                }
                tbSecondRow.FontSize = intFont;
                tbSecondRow.Text = textValue;
            }
        }

        #endregion
    }
}
