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
    /// VendBoxControl.xaml 的交互逻辑
    /// </summary>
    public partial class VendBoxControl : UserControl
    {
        private bool isCheck = false;

        private VendBoxCodeModel currentItem;

        public VendBoxCodeModel CurrentItem
        {
            get { return currentItem; }
            private set { currentItem = value; }
        }

        public VendBoxControl()
        {
            InitializeComponent();

            Loaded += (Control_Loaded);
        }

        void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (currentItem != null)
            {
                SetControlPic();
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
        /// 设置当前的格子中容纳的信息
        /// </summary>
        /// <param name="mcdInfo"></param>
        internal void SetCurrentItem(VendBoxCodeModel itemInfo)
        {
            currentItem = itemInfo;
        }

        public void SetControlPic()
        {
            string strPicName = "vendbox.png";
            string strPicFile = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\ManagerPic\\Pic_Pub\\" + strPicName;

            if (File.Exists(strPicFile))
            {
                // 图片存在
                strPicFile = "pack://siteoforigin:,,,/Images/ManagerPic/Pic_Pub/" + strPicName;
                imageButton.Source = new BitmapImage(new Uri(strPicFile));
            }
            else
            {
                imageButton.Source = null;
            }
        }

        /// <summary>
        /// 由于在触发load后才能设置是否为选中状态.所有加该方法
        /// </summary>
        /// <param name="p"></param>
        internal void IsDefaultCheck(bool p)
        {
            isCheck = p;
        }

        public void SetNoInfo(string noTitle)
        {
            tbNull.Text = noTitle;
        }

        /// <summary
        /// 默认
        /// </summary>
        public void ToNormal()
        {
            if (currentItem == null)
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
            if (currentItem == null)
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

        #region 设置

        /// <summary>
        /// 设置第一行文字行内容
        /// </summary>
        public void SetOneText(string textValue)
        {
            if (CurrentItem != null)
            {
                tbOneRow.Text = textValue;
            }
        }

        /// <summary>
        /// 设置第二文字行内容
        /// </summary>
        /// <param name="count"></param>
        public void SetSecondText(string textValue)
        {
            ////if (CurrentItem != null)
            ////{
            ////    int intFont = 20;
            ////    if (textValue.Length > 6)
            ////    {
            ////        intFont = 20;
            ////    }
            ////    tbSecondRow.FontSize = intFont;
            ////    tbSecondRow.Text = textValue;
            ////}
        }

        #endregion
    }
}
