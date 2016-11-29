#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件管理设置
// 业务功能：密码修改
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

namespace AutoSellGoodsMachine
{
    /// <summary>
    /// FrmEditPwd.xaml 的交互逻辑
    /// </summary>
    public partial class FrmEditPwd : Window
    {
        #region 变量声明

        /// <summary>
        /// 输入内容
        /// </summary>
        private string m_Input = string.Empty;

        /// <summary>
        /// 旧密码
        /// </summary>
        private string m_OldPwd = string.Empty;

        private string m_Input_Old = string.Empty;
        private string m_Input_New = string.Empty;
        private string m_Input_Confirm = string.Empty;

        private string m_PwdCfg = string.Empty;

        /// <summary>
        /// 当前密码输入类型 0：旧密码 1：新密码 2：确认密码
        /// </summary>
        private string m_PwdType = "0";

        #endregion

        public FrmEditPwd()
        {
            InitializeComponent();

            InitForm();
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbInput_Old.Focus();
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        #region

        private void InitForm()
        {
            tbInput_Old.Text = tbInput_New.Text = tbInput_Confirm.Text = string.Empty;
            m_Input = string.Empty;
            tbTitle.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_EditPwd");

            btnCancel.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Cancel");
            btnOk.Content = PubHelper.p_LangOper.GetStringBundle("Pub_Btn_Ok");

            tbOldPassWord.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_EditPwd_OldPwd");
            tbNewPassWord.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_EditPwd_NewPwd");
            tbConfirmPassWord.Text = PubHelper.p_LangOper.GetStringBundle("SysCfg_Menu_EditPwd_ConfirmPwd");

            switch (PubHelper.p_BusinOper.UserType)
            {
                case Business.Enum.BusinessEnum.UserType.NormalUser:// 一级管理人员
                    m_PwdCfg = "AdminPwd1";
                    break;

                case Business.Enum.BusinessEnum.UserType.SecondUser:// 二级管理人员
                    m_PwdCfg = "AdminPwd2";
                    break;

                case Business.Enum.BusinessEnum.UserType.SystemUser:// 厂商管理人员
                    m_PwdCfg = "SuperPwd";
                    break;
            }
            m_OldPwd = PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue(m_PwdCfg);
        }

        #endregion

        private void inputOld_GotFocus(object sender, RoutedEventArgs e)
        {
            m_PwdType = "0";
            m_Input = string.Empty;
        }

        private void inputNew_GotFocus(object sender, RoutedEventArgs e)
        {
            m_PwdType = "1";
            m_Input = string.Empty;
        }

        private void inputConfirm_GotFocus(object sender, RoutedEventArgs e)
        {
            m_PwdType = "2";
            m_Input = string.Empty;
        }

        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            if (m_Input.Length >= 6)
            {
                return;
            }
            string strInput = DictionaryHelper.Dictionary_Input(sender.ToString());
            m_Input = m_Input + strInput;
            switch (m_PwdType)
            {
                case "0":// 旧密码
                    m_Input_Old = m_Input;
                    tbInput_Old.AppendText("*");
                    tbInput_Old.Select(tbInput_Old.Text.Length, 0);
                    break;
                case "1":// 新密码
                    m_Input_New = m_Input;
                    tbInput_New.AppendText("*");
                    tbInput_New.Select(tbInput_New.Text.Length, 0);
                    break;
                case "2":// 确认密码
                    m_Input_Confirm = m_Input;
                    tbInput_Confirm.AppendText("*");
                    tbInput_Confirm.Select(tbInput_Confirm.Text.Length, 0);
                    break;
            }

            ////tbInput.Text = tbInput.Text + "*";
        }

        /// <summary>
        /// 按钮—返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 按钮—确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            #region 校验密码有效性

            if (string.IsNullOrEmpty(m_Input_Old))
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_PwdEdit_NoOld"), PubHelper.MsgType.Ok);
                tbInput_Old.Focus();
                return;
            }
            if (string.IsNullOrEmpty(m_Input_New))
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_PwdEdit_NoNew"), PubHelper.MsgType.Ok);
                tbInput_New.Focus();
                return;
            }
            if (string.IsNullOrEmpty(m_Input_Confirm))
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_PwdEdit_NoConfirm"), PubHelper.MsgType.Ok);
                tbInput_Confirm.Focus();
                return;
            }
            if (m_Input_New != m_Input_Confirm)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_PwdEdit_NoSame"), PubHelper.MsgType.Ok);
                tbInput_New.Text = tbInput_Confirm.Text = m_Input_New = m_Input_Confirm = string.Empty;
                tbInput_New.Focus();
                return;
            }
            string strMd5OldPwd = PubHelper.p_BusinOper.CheckDataOper.Md532(m_Input_Old, "1");
            if (strMd5OldPwd != m_OldPwd)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_PwdEdit_ErrOldPwd"), PubHelper.MsgType.Ok);
                tbInput_Old.Text = m_Input_Old = string.Empty;
                tbInput_Old.Focus();
                return;
            }
            string strMd5NewPwd = PubHelper.p_BusinOper.CheckDataOper.Md532(m_Input_New, "1");
            if (strMd5NewPwd == m_OldPwd)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_PwdEdit_Same"), PubHelper.MsgType.Ok);
                tbInput_New.Text = tbInput_Confirm.Text = m_Input_New = m_Input_Confirm = string.Empty;
                tbInput_New.Focus();
                return;
            }

            // 检测新修改的密码是否和另外的管理员的密码一致
            bool blnIsSame = false;
            if (PubHelper.p_BusinOper.UserType ==  Business.Enum.BusinessEnum.UserType.NormalUser)
            {
                // 检查和厂商管理员的密码是否一致
                if (strMd5NewPwd == PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("SuperPwd"))
                {
                    // 一致，不能修改
                    blnIsSame = true;
                }
            }
            if (PubHelper.p_BusinOper.UserType ==  Business.Enum.BusinessEnum.UserType.SystemUser)
            {
                // 检查和一级管理员的密码是否一致
                if (strMd5NewPwd == PubHelper.p_BusinOper.SysCfgOper.GetSysCfgValue("AdminPwd1"))
                {
                    // 一致，不能修改
                    blnIsSame = true;
                }
            }
            if (blnIsSame)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Err_PwdEdit_OtherSame"), PubHelper.MsgType.Ok);
                tbInput_New.Text = tbInput_Confirm.Text = m_Input_New = m_Input_Confirm = string.Empty;
                tbInput_New.Focus();
                return;
            }

            #endregion

            // 保存
            bool result = PubHelper.p_BusinOper.UpdateSysCfgValue(m_PwdCfg, strMd5NewPwd);
            if (result)
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperSuc"), PubHelper.MsgType.Ok);
            }
            else
            {
                PubHelper.ShowMsgInfo(PubHelper.p_LangOper.GetStringBundle("Pub_OperFail"), PubHelper.MsgType.Ok);
            }

            this.Close();
        }

        /// <summary>
        /// 按钮—清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearInputOld_Click(object sender, RoutedEventArgs e)
        {
            m_Input = m_Input_Old = tbInput_Old.Text = string.Empty;
        }

        /// <summary>
        /// 按钮—清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearInputNew_Click(object sender, RoutedEventArgs e)
        {
            m_Input = m_Input_New = tbInput_New.Text = string.Empty;
        }

        /// <summary>
        /// 按钮—清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearInputConfirm_Click(object sender, RoutedEventArgs e)
        {
            m_Input = m_Input_Confirm = tbInput_Confirm.Text = string.Empty;
        }
    }
}
