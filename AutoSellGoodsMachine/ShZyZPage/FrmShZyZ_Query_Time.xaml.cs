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
    /// FrmShZyZ_Query_Time.xaml 的交互逻辑
    /// </summary>
    public partial class FrmShZyZ_Query_Time : Window
    {
        #region 变量声明

        /// <summary>
        /// 输入内容
        /// </summary>
        private string m_Input = string.Empty;

        #endregion

        public FrmShZyZ_Query_Time()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            tbInput.Text = string.Empty;
            m_Input = string.Empty;
            tbTitle.Text = "志愿者服务时间查询";// PubHelper.p_LangOper.GetStringBundle("Keyboard_Title");
            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnOk.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Ok");
            btnSpace.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Space");
            btnDel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Del");

            tbInput.Text = m_Input = PubHelper.p_Keyboard_Input;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            tbInput.Text = m_Input = string.Empty;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            PubHelper.p_Keyboard_Input = m_Input;
            this.Close();
        }

        private void btnSpace_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(m_Input))
            {
                return;
            }
            m_Input = m_Input.Substring(0, m_Input.Length - 1);
            tbInput.Text = m_Input;
        }

        private void btnClearInput_Click(object sender, RoutedEventArgs e)
        {
            tbInput.Text = m_Input = string.Empty;
        }

        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            string strInput = DictionaryHelper.Dictionary_Input(sender.ToString());
            m_Input = m_Input + strInput;
            tbInput.Text = m_Input;
        }
    }
}
