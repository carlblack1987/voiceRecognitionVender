#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：信息提示
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

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmMsg.xaml 的交互逻辑
    /// </summary>
    public partial class FrmMsg : Window
    {
        public string MsgInfo = string.Empty;
        public PubHelper.MsgType MsgType = PubHelper.MsgType.Ok;

        public FrmMsg()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("Pub_MsgTitle");
            if (PubHelper.p_MsgModel == "1")
            {
                btnNo.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Oper_Cancel");
                btnOk.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Ok");
                btnYes.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Oper_Continue");
            }
            else
            {
                btnNo.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_No");
                btnOk.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Ok");
                btnYes.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Yes");
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_MsgResult = false;
            this.Close();
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_MsgResult = true;
            this.Close();
        }

        private void Form_Msg_Loaded(object sender, RoutedEventArgs e)
        {
            tbInfo.Text = MsgInfo;
            switch (MsgType)
            {
                case PubHelper.MsgType.Ok:
                    btnYes.Visibility = btnNo.Visibility = System.Windows.Visibility.Hidden;
                    btnOk.Visibility = System.Windows.Visibility.Visible;
                    break;

                case PubHelper.MsgType.YesNo:
                    btnYes.Visibility = btnNo.Visibility = System.Windows.Visibility.Visible;
                    btnOk.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }
        }
    }
}
