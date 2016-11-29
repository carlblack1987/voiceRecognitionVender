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
using System.Windows.Media.Animation;
using System.IO;

using Business;
using Business.Model;
using Business.Enum;

namespace AutoSellGoodsMachine.Controls
{
    /// <summary>
    /// GoodsItemControl.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsItemControl : UserControl
    {
        GoodsModel currentGoodsItem = null;

        public GoodsModel CurrentGoodsItem
        {
            get
            {
                return currentGoodsItem;
            }
        }

        Grid parentPanel = null;

        public GoodsItemControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(GoodsItemControl_Loaded);
        }

        void GoodsItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            this.Width = this.ActualWidth;

            // ！！！！！注意：此处的ActualHeight的值可能为0，会导致错误
            this.Height = this.ActualHeight;

            if (this.Height < 60)
            {
                this.Height = 200;
            }
            if (this.Width < 20)
            {
                this.Width = 200;
            }
            imgProduct.Height = this.Height - 60;// 200;
            imgProduct.Width = this.Width;

            ////imgProduct.Height = this.Height;
            imagePrice.Width = imgProduct.Width - 10;// 200;// this.ActualWidth / 3;//2014-8-5
            imagePrice.Height = 50;
            //imagePrice.Height = 30;
            //imagePrice.Height = 40;

            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            this.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            this.SetValue(Grid.ColumnSpanProperty, PubHelper.p_BusinOper.ConfigInfo.EachRowMaxColuNum);

            InitMargin();
        }

        public void InitMargin()
        {
            Margin = new Thickness(this.Width * ((PubHelper.p_BusinOper.ConfigInfo.EachRowMaxColuNum + 1) - currentGoodsItem.ColumnIndex), 0, 0, 0);
        }
        public void ClearMargin()
        {
            Margin = new Thickness(0, 0, 0, 0);
        }

        public void SetSource(GoodsModel productItem,int rowIndex,int columnIndex)
        {
            this.Name = "productItem" + productItem.McdCode;

            ////this.SetValue(Grid.RowProperty, productItem.TrayIndex);

            int intRowIndex = rowIndex;
            if (intRowIndex < 0)
            {
                intRowIndex = 0;
            }
            int intColumnIndex = columnIndex;
            if (intColumnIndex < 0)
            {
                intColumnIndex = 0;
            }

            this.SetValue(Grid.RowProperty, intRowIndex);
            this.SetValue(Grid.ColumnProperty, intColumnIndex);

            //this.SetValue(Grid.ColumnSpanProperty, productItem.ColumnSpan + 1);

            imagePrice.Source = new BitmapImage(new Uri(SkinHelper.p_SkinName + "ProductPage/ProductList/main_price_bottom.png"));
            imgProduct.Source = new BitmapImage(new Uri(PubHelper.GetMcdPic(productItem.PicName)));

            double fds = imgProduct.Width;
            ////imagePrice.Width = imgProduct.Width;// 200;// this.ActualWidth / 3;//2014-8-5

            SetPriceAreaText(productItem);

            this.currentGoodsItem = productItem;

            ////if (this.currentGoodsItem.IsNew)
            ////{
            ////    panelNew.Visibility = System.Windows.Visibility.Visible;
            ////}
            //////panelNew.Visibility = System.Windows.Visibility.Visible;

            SetStock(productItem);
        }

        public void RefreshGoodsInfo(GoodsModel productItem)
        {
            SetPriceAreaText(productItem);
            imgProduct.Source = new BitmapImage(new Uri(PubHelper.GetMcdPic(productItem.PicName)));
            SetStock(productItem);
        }

        public void SetPosition(Grid grid)
        {
            parentPanel = grid;
        }

        /// <summary>
        /// 设置库存
        /// </summary>
        /// <param name="productItem"></param>
        private void SetStock(GoodsModel productItem)
        {
            bool blnIsEnoughStock = false;// 是否库存充足 False：无库存 True：有库存
            if (PubHelper.p_BusinOper.ConfigInfo.IsRunStock == BusinessEnum.ControlSwitch.Stop)
            {
                // 不启用库存
                blnIsEnoughStock = true;// 认为库存充足
            }
            else
            {
                // 启用库存
                if (productItem.SurNum <= 0)
                {
                    // 库存不足
                    blnIsEnoughStock = false;// 库存不足
                }
                else
                {
                    blnIsEnoughStock = true;// 库存充足
                }
            }
            gdPriceArea.Opacity = 1;
            if (blnIsEnoughStock)
            {
                // 库存充足
                imgProduct.Opacity = 1;
                //imgProduct.IsEnabled = true;
                //gdPriceArea.IsEnabled = true;
                panelSellEnd.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                // 库存不足
                double dblGoodsOpacity = Convert.ToDouble(PubHelper.p_BusinOper.ConfigInfo.GoodsOpacity) / 10;
                imgProduct.Opacity = dblGoodsOpacity;
                //gdPriceArea.Opacity = 1;
                tbSellOut.Text = PubHelper.p_LangOper.GetStringBundle("Pub_SellOut");
                tbAsilePrice.Text = tbAsileCode.Text = string.Empty;
                //imgProduct.IsEnabled = false;
                //gdPriceArea.IsEnabled = false;
                panelSellEnd.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// 设置商品价格标签
        /// </summary>
        /// <param name="productItem"></param>
        private void SetPriceAreaText(GoodsModel productItem)
        {
            string strContent = string.Empty;
            bool blnIsShowContent = true;
            string strAsileCode = productItem.McdCode;
            string strAsilePrice = PubHelper.p_BusinOper.MoneyIntToString(productItem.Price.ToString());
            string strAsileName = productItem.McdName;

            int intGoodsNameFont = 22;

            switch (PubHelper.p_BusinOper.ConfigInfo.GoodsShowContent)
            {
                case "0":// 不显示任何内容
                    blnIsShowContent = false;
                    break;
                case "1":// 只显示商品价格
                    strAsileCode = string.Empty;
                    break;
                ////case "2":// 只显示商品所在货道编号
                ////    strAsilePrice = string.Empty;
                ////    break;
                ////case "3":// 显示商品价格及所在货道编号
                ////    strAsileCode = strAsileCode + "  ";
                ////    break;
                case "4":// 只显示商品名称
                    strAsileCode = string.Empty;
                    strAsilePrice = strAsileName;
                    break;
                case "5":// 显示商品名称和价格
                    strAsileCode = strAsileName + "  ";
                    if (strAsileName.Length > 8)
                    {
                        intGoodsNameFont = 17;
                    }
                    break;
                default:// 只显示商品名称
                    strAsileCode = string.Empty;
                    strAsilePrice = strAsileName;
                    break;
            }
            if (!blnIsShowContent)
            {
                gdPriceArea.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                tbAsileCode.FontSize = intGoodsNameFont;
                tbSellOut.Text = string.Empty;
                gdPriceArea.Visibility = System.Windows.Visibility.Visible;
                tbAsileCode.Text = strAsileCode;
                tbAsilePrice.Text = strAsilePrice;
            }
        }
    }
}
