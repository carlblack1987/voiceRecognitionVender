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

using Business.Model;
using Business.Enum;

namespace AutoSellGoodsMachine.Controls
{
    /// <summary>
    /// GoodsWayProduct.xaml 的交互逻辑
    /// </summary>
    public partial class GoodsTypeControl : UserControl
    {
        GoodsTypeModel currentGoodsItem = null;

        public GoodsTypeModel CurrentGoodsItem
        {
            get
            {
                return currentGoodsItem;
            }
        }

        public GoodsTypeControl()
        {
            InitializeComponent();
            Loaded += (GoodsWayProduct_Loaded);
        }

        void GoodsWayProduct_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void SetSource(GoodsTypeModel goodsType)
        {
            this.Name = "productItem" + goodsType.TypeCode;
            this.currentGoodsItem = goodsType;
        }

        /// <summary>
        /// 设置使能背景图
        /// </summary>
        public void SetBgPic_Normal()
        {
            string strPicName = currentGoodsItem.TypeCode + "_normal.png";
            string strDefaultName = "default_normal.png";
            string strDefaultFile = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\FormPic\\goodstype\\" + strDefaultName;
            string strPicFile = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\FormPic\\goodstype\\" + strPicName;

            bool blnIsExist = true;
            if (File.Exists(strPicFile))
            {
                // 商品图片存在
                strPicFile = "pack://siteoforigin:,,,/Images/FormPic/goodstype/" + strPicName;
            }
            else
            {
                blnIsExist = File.Exists(strDefaultFile);
                if (blnIsExist)
                {
                    strPicFile = "pack://siteoforigin:,,,/Images/FormPic/goodstype/" + strDefaultName;
                }
            }
            if (blnIsExist)
            {
                imgGoodsType.Source = new BitmapImage(new Uri(strPicFile, UriKind.RelativeOrAbsolute));
            }
            else
            {
                imgGoodsType.Source = null;
            }
            tbTypeNum.Margin = new Thickness(0, 0, 0, this.ActualHeight / 2 + 15);
        }

        /// <summary>
        /// 设置禁能背景图
        /// </summary>
        public void SetBgPic_Disable()
        {
            string strPicName = currentGoodsItem.TypeCode + "_disable.png";
            string strDefaultName = "default_disable.png";
            string strDefaultFile = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\FormPic\\goodstype\\" + strDefaultName;
            string strPicFile = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\FormPic\\goodstype\\" + strPicName;

            bool blnIsExist = true;
            if (File.Exists(strPicFile))
            {
                // 商品图片存在
                strPicFile = "pack://siteoforigin:,,,/Images/FormPic/goodstype/" + strPicName;
            }
            else
            {
                blnIsExist = File.Exists(strDefaultFile);
                if (blnIsExist)
                {
                    strPicFile = "pack://siteoforigin:,,,/Images/FormPic/goodstype/" + strDefaultName;
                }
            }
            if (blnIsExist)
            {
                imgGoodsType.Source = new BitmapImage(new Uri(strPicFile, UriKind.RelativeOrAbsolute));
            }
            else
            {
                imgGoodsType.Source = null;
            }
            tbTypeNum.Margin = new Thickness(0, 0, 0, this.ActualHeight / 2 + 15);
        }

        /// <summary>
        /// 设置第一行文字行内容
        /// </summary>
        public void SetContent_One(string content,BusinessEnum.ScreenType screenType)
        {
            string strContent = content;
            int intFontSize = 22;
            try
            {
                if (strContent.Length > 8)
                {
                    strContent = strContent.Substring(0, 8) + "\r\n" + strContent.Substring(8);
                }
                switch (screenType)
                {
                    case BusinessEnum.ScreenType.ScreenType50:// 如果是50寸屏幕
                        intFontSize = 32;

                        break;
                }
            }
            catch
            {
                strContent = content;
            }
            finally
            {
                tbTypeName.FontSize = intFontSize;
                tbTypeName.Text = strContent;
            }
        }

        /// <summary>
        /// 设置第一行字体颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor_One(Color color)
        {
            tbTypeName.Foreground = new SolidColorBrush(color);
        }

        /// <summary>
        /// 设置第二行文字行内容
        /// </summary>
        public void SetContent_Second(string content, BusinessEnum.ScreenType screenType)
        {
            string strContent = content;
            int intFontSize = 22;
            try
            {
                if (strContent.Length > 8)
                {
                    strContent = strContent.Substring(0, 8) + "\r\n" + strContent.Substring(8);
                }
                switch (screenType)
                {
                    case BusinessEnum.ScreenType.ScreenType50:// 如果是50寸屏幕
                        intFontSize = 28;

                        break;
                }
            }
            catch
            {
                strContent = content;
            }
            finally
            {
                tbTypeNum.FontSize = intFontSize;
                tbTypeNum.Text = strContent;
            }
        }

        /// <summary>
        /// 设置第二行字体颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor_Second(Color color)
        {
            tbTypeNum.Foreground = new SolidColorBrush(color);
        }
    }
}
