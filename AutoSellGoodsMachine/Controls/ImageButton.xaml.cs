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

namespace AutoSellGoodsMachine.Controls
{
    /// <summary>
    /// ImageButton.xaml 的交互逻辑
    /// </summary>
    public partial class ImageButton : UserControl
    {
        public string Path
        {
            get
            {
                return "";
            }
            set
            {
                btnConfig.Source = new BitmapImage(new Uri(value));
            }
        }

        public ImageButton()
        {
            InitializeComponent();
        }
    }
}
