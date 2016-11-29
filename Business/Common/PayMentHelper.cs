#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：支付方式处理类
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using KdbPlug;
using Business.Model;
using Business.Enum;

namespace Business.Common
{
    public class PayMentHelper
    {
        public PaymentControlModel PaymentList = new PaymentControlModel();

        public string AllowPaymentList = string.Empty;

        /// <summary>
        /// 获取当前开通的支付方式数量
        /// </summary>
        /// <returns>可用支付方式数量</returns>
        public void GetEnablePayNum()
        {
            int intPayNum = 0;

            if ((PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.Cash)))
            {
                intPayNum++;
            }
            if ((PaymentList.IC.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.IcCard)))
            {
                intPayNum++;
            }
            if ((PaymentList.WeChatCode.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.WeChatCode)))
            {
                intPayNum++;
            }
            if ((PaymentList.AliPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.AliPay_Code)))
            {
                intPayNum++;
            }
            if ((PaymentList.BestPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.BestPay_Code)))
            {
                intPayNum++;
            }
            if ((PaymentList.NoFeeCard.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.NoFeeCard)))
            {
                intPayNum++;
            }
            if ((PaymentList.UnionPay.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.QuickPass)))
            {
                intPayNum++;
            }
            if ((PaymentList.QRCodeCard.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.QRCodeCard)))
            {
                intPayNum++;
            }

            if ((PaymentList.Volunteer_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.Volunteer)))
            {
                intPayNum++;
            }

            PaymentList.EnablePayNum = intPayNum;
        }

        /// <summary>
        /// 获取当前可以使用的支付方式数量
        /// </summary>
        /// <returns></returns>
        public int GetOpenPayNum()
        {
            int intPayNum = 0;

            if (CheckPaymentIsExist(BusinessEnum.PayMent.Cash))
            {
                intPayNum++;
            }
            if (CheckPaymentIsExist(BusinessEnum.PayMent.IcCard))
            {
                intPayNum++;
            }
            if (CheckPaymentIsExist(BusinessEnum.PayMent.WeChatCode))
            {
                intPayNum++;
            }
            if (CheckPaymentIsExist(BusinessEnum.PayMent.AliPay_Code))
            {
                intPayNum++;
            }
            if (CheckPaymentIsExist(BusinessEnum.PayMent.NoFeeCard))
            {
                intPayNum++;
            }
            if (CheckPaymentIsExist(BusinessEnum.PayMent.QuickPass))
            {
                intPayNum++;
            }
            if (CheckPaymentIsExist(BusinessEnum.PayMent.QRCodeCard))
            {
                intPayNum++;
            }
            if (CheckPaymentIsExist(BusinessEnum.PayMent.Volunteer))
            {
                intPayNum++;
            }

            return intPayNum;
        }

        /// <summary>
        /// 只有一种支付方式情况下获取该支付方式
        /// </summary>
        /// <returns>支付方式</returns>
        public BusinessEnum.PayMent GetOnlyOnePayment()
        {
            if ((PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.Cash)))
            {
                PaymentList.Now_PayMent = BusinessEnum.PayMent.Cash;
                return PaymentList.Now_PayMent;
            }
            if ((PaymentList.IC.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.IcCard)))
            {
                PaymentList.Now_PayMent = BusinessEnum.PayMent.IcCard;
                return PaymentList.Now_PayMent;
            }
            if ((PaymentList.WeChatCode.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.WeChatCode)))
            {
                PaymentList.Now_PayMent = BusinessEnum.PayMent.WeChatCode;
                return PaymentList.Now_PayMent;
            }
            if ((PaymentList.AliPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.AliPay_Code)))
            {
                PaymentList.Now_PayMent = BusinessEnum.PayMent.AliPay_Code;
                return PaymentList.Now_PayMent;
            }
            if ((PaymentList.NoFeeCard.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.NoFeeCard)))
            {
                PaymentList.Now_PayMent = BusinessEnum.PayMent.NoFeeCard;
                return PaymentList.Now_PayMent;
            }
            if ((PaymentList.UnionPay.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.QuickPass)))
            {
                PaymentList.Now_PayMent = BusinessEnum.PayMent.QuickPass;
                return PaymentList.Now_PayMent;
            }
            if ((PaymentList.QRCodeCard.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.QRCodeCard)))
            {
                PaymentList.Now_PayMent = BusinessEnum.PayMent.QRCodeCard;
                return PaymentList.Now_PayMent;
            }
            if ((PaymentList.Volunteer_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run) &&
                (CheckPaymentIsExist(BusinessEnum.PayMent.Volunteer)))
            {
                PaymentList.Now_PayMent = BusinessEnum.PayMent.Volunteer;
                return PaymentList.Now_PayMent;
            }

            return BusinessEnum.PayMent.All;
        }

        /// <summary>
        /// 检测某个支付方式是否被允许
        /// </summary>
        /// <param name="payMent">支付方式</param>
        /// <returns>结果 False：不允许 True：允许</returns>
        public bool CheckPaymentIsExist(BusinessEnum.PayMent payMent)
        {
            bool result = false;

            try
            {
                string strPayMent = string.Empty;
                switch (payMent)
                {
                    case BusinessEnum.PayMent.Cash:// 现金
                        strPayMent = "cash,";
                        break;
                    case BusinessEnum.PayMent.WeChatCode:// 微信扫码
                        strPayMent = "wechatcode,";
                        break;
                    case BusinessEnum.PayMent.AliPay_Code:// 支付宝付款码
                        strPayMent = "alipaycode,";
                        break;
                    case BusinessEnum.PayMent.BestPay_Code:// 翼支付付款码
                        strPayMent = "bestpaycode,";
                        break;
                    case BusinessEnum.PayMent.JDPay_Code:// 京东钱包付款码
                        strPayMent = "jdpay,";
                        break;
                    case BusinessEnum.PayMent.IcCard:// 储值卡
                        strPayMent = "pos,";
                        break;
                    case BusinessEnum.PayMent.NoFeeCard:// 非储值卡（磁条卡）
                        strPayMent = "id,";
                        break;
                    case BusinessEnum.PayMent.QRCodeCard:
                        strPayMent = "qr,";
                        break;
                    case BusinessEnum.PayMent.QuickPass:// 银联闪付
                        strPayMent = "quickpass,";
                        break;
                    case BusinessEnum.PayMent.Volunteer:// 志愿者兑换
                        strPayMent = "volunteer,";
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(strPayMent))
                {
                    if (AllowPaymentList.IndexOf(strPayMent) > -1)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 检测某个支付方式是否可用
        /// </summary>
        /// <param name="payMent"></param>
        /// <returns></returns>
        public bool CheckPaymentControl(BusinessEnum.PayMent payMent)
        {
            bool result = false;

            try
            {
                string strPayMent = string.Empty;
                switch (payMent)
                {
                    case BusinessEnum.PayMent.Cash:// 现金
                        if (PaymentList.Cash.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    case BusinessEnum.PayMent.WeChatCode:// 微信扫码
                        if (PaymentList.WeChatCode.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    case BusinessEnum.PayMent.AliPay_Code:// 支付宝付款码
                        if (PaymentList.AliPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    case BusinessEnum.PayMent.BestPay_Code:// 翼支付付款码
                        if (PaymentList.BestPay_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    case BusinessEnum.PayMent.IcCard:// 储值卡
                        if (PaymentList.IC.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    case BusinessEnum.PayMent.NoFeeCard:// 非储值卡（磁条卡）
                        if (PaymentList.NoFeeCard.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    case BusinessEnum.PayMent.QRCodeCard:// 二维码支付
                        if (PaymentList.QRCodeCard.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    case BusinessEnum.PayMent.QuickPass:// 银联闪付
                        if (PaymentList.UnionPay.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    case BusinessEnum.PayMent.Volunteer:// 志愿者兑换
                        if (PaymentList.Volunteer_Code.ControlSwitch == BusinessEnum.ControlSwitch.Run)
                        {
                            result = true;
                        }
                        break;
                    default:
                        break;
                }
                if (result)
                {
                    result = CheckPaymentIsExist(payMent);
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 获取某个支付方式的支付开关
        /// </summary>
        /// <param name="payMent"></param>
        /// <param name="controlSwitch"></param>
        /// <returns></returns>
        public BusinessEnum.ControlSwitch ChangePaymentControl(BusinessEnum.PayMent payMent, string controlSwitch)
        {
            if (controlSwitch == "1")
            {
                // 1 启用
                // 检查该支付方式是否在允许支付方式列表中存在
                bool result = CheckPaymentIsExist(payMent);
                if (result)
                {
                    return BusinessEnum.ControlSwitch.Run;
                }
                else
                {
                    return BusinessEnum.ControlSwitch.Stop;
                }
            }
            else
            {
                // 0 停止
                return BusinessEnum.ControlSwitch.Stop;
            }
        }

        /// <summary>
        /// 检测条形码或二维码数据内容属于何种支付方式
        /// </summary>
        /// <param name="codeNum">条形码或二维码数据内容</param>
        /// <returns>支付方式</returns>
        public BusinessEnum.PayMent ConvertCodeToPay(string codeNum)
        {
            BusinessEnum.PayMent payMent = BusinessEnum.PayMent.No;

            if (string.IsNullOrEmpty(codeNum))
            {
                return payMent;
            }

            int intLen = codeNum.Length;
            switch (intLen)
            {
                case 15:// 志愿者兑换
                    payMent = BusinessEnum.PayMent.Volunteer;
                    break;
                case 18:// 支付宝付款码或微信付款码
                    if (codeNum.Substring(0, 4) == "3101")
                    {
                        payMent = BusinessEnum.PayMent.Volunteer;
                    }
                    else
                    {
                        payMent = BusinessEnum.PayMent.AliPay_Code;
                    }
                    ////string strBeginNum = codeNum.Substring(0, 2);
                    ////if (strBeginNum == "28")
                    ////{
                    ////    // 支付宝付款码开头两位是28
                    ////    if (CheckPaymentControl(BusinessEnum.PayMent.AliPay_Code))
                    ////    {
                    ////        // 如果支付宝付款码支付方式开通
                    ////        payMent = BusinessEnum.PayMent.AliPay_Code;
                    ////    }
                    ////}
                    ////if (strBeginNum == "13")
                    ////{
                    ////    // 微信付款码开头两位是13
                    ////    if (CheckPaymentControl(BusinessEnum.PayMent.WeChatCode))
                    ////    {
                    ////        // 如果微信付款码支付方式开通
                    ////        payMent = BusinessEnum.PayMent.WeChatCode;
                    ////    }
                    ////}
                    break;
            }

            return payMent;
        }

        #region 支付方式名称处理

        private string _m_DbFileName = "TermInfo.db";

        public void LoadPaymentName()
        {
            try
            {
                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;

                string strSql = @"select PayMentCode,Name_CN,Name_EN,Name_RUS from T_SYS_PAYMENT ";
                DataSet dataSet = dbOper.dataSet(strSql);
                if (dataSet.Tables.Count > 0)
                {
                    int intCount = dataSet.Tables[0].Rows.Count;
                    string strPayCode = string.Empty;
                    if (intCount > 0)
                    {
                        for (int i = 0; i < intCount; i++)
                        {
                            strPayCode = dataSet.Tables[0].Rows[i]["PayMentCode"].ToString();
                            switch (strPayCode)
                            {
                                case "IcCard":// 一卡通
                                    PaymentList.IC.Name_CN = dataSet.Tables[0].Rows[i]["Name_CN"].ToString();
                                    PaymentList.IC.Name_EN = dataSet.Tables[0].Rows[i]["Name_EN"].ToString();
                                    PaymentList.IC.Name_RUS = dataSet.Tables[0].Rows[i]["Name_RUS"].ToString();
                                    break;
                                case "NoFeeCard":// 会员卡
                                    PaymentList.NoFeeCard.Name_CN = dataSet.Tables[0].Rows[i]["Name_CN"].ToString();
                                    PaymentList.NoFeeCard.Name_EN = dataSet.Tables[0].Rows[i]["Name_EN"].ToString();
                                    PaymentList.NoFeeCard.Name_RUS = dataSet.Tables[0].Rows[i]["Name_RUS"].ToString();
                                    break;
                            }
                        }
                    }
                }

                dbOper.closeConnection();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 更新系统配置参数值
        /// </summary>
        /// <param name="cfgName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdatePaymentName(string payCode, string nameCN,string nameEN,string nameRUS)
        {
            bool result = false;
            try
            {
                string strSql = "update T_SYS_PAYMENT set [Name_CN] = '" + nameCN +
                    "',[Name_EN] = '" + nameEN + "',[Name_RUS] = '" + nameRUS + "' where [PayMentCode]='" + payCode + "'";

                DbOper dbOper = new DbOper();
                dbOper.DbFileName = _m_DbFileName;
                dbOper.ConnType = ConnectType.CloseConn;
                result = dbOper.excuteSql(strSql);
                dbOper.closeConnection();
            }
            catch
            {
                result = false;
            }

            return result;
        }      

        #endregion
    }
}
