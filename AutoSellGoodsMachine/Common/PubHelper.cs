#region [ KIMMA Co.,Ltd. Copyright (C) 2013 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件公共处理类
// 业务功能：iVend终端软件公共处理类
// 创建标识：2013-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

using UIPlug;
using LangPlug;
using Business;
using Business.Enum;

namespace AutoSellGoodsMachine
{
    public class PubHelper
    {
        #region 常量声明

        public const string TMP_UNIT = "℃";// 温度单位

        public const double OPACITY_GRAY = 0.4;// 窗体透明度灰度值
        public const double OPACITY_NORMAL = 1;// 窗体透明度正常值

        #endregion

        #region 公共变量声明

        /// <summary>
        /// UI界面类对象
        /// </summary>
        public static UIOper p_UIOper = new UIOper();

        /// <summary>
        /// 语言资源类对象
        /// </summary>
        public static LangOper p_LangOper = new LangOper();

        /// <summary>
        /// 业务处理类对象
        /// </summary>
        public static BusinessOper p_BusinOper = new BusinessOper();

        /// <summary>
        /// 是否正在进行线程操作 False：正在进行中 True：已经进行完毕
        /// </summary>
        public static bool p_IsTrdOper = false;

        /// <summary>
        /// 是否中止当前操作 false：不中止 true：中止
        /// </summary>
        public static bool p_IsStopOper = false;

        /// <summary>
        /// 是否退出当前软件 False：否 True：是
        /// </summary>
        public static bool p_IsShutDown = false;

        /// <summary>
        /// 提示窗体的结果
        /// </summary>
        public static bool p_MsgResult = false;

        // 提示窗体的模式 0：管理提示窗体 1：消费者业务流程提示窗体
        public static string p_MsgModel = "0";

        /// <summary>
        /// 是否自检了设备 False：没有自检 True：已经自检
        /// </summary>
        public static bool p_IsCheckDevice = false;

        /// <summary>
        /// 是否刷新语言 False：不刷新 True：刷新
        /// </summary>
        public static bool p_IsRefreshLanguage = false;

        /// <summary>
        /// 键盘输入内容
        /// </summary>
        public static string p_Keyboard_Input = string.Empty;

        /// <summary>
        /// 是否刷新主界面上的货道信息
        /// </summary>
        public static bool p_IsRefreshAsile = false;

        /// <summary>
        /// 是否刷新主界面上的商品类别信息
        /// </summary>
        public static bool p_IsRefreshGoodsType = false;

        /// <summary>
        /// 是否更新界面皮肤
        /// </summary>
        public static bool p_IsRefreshSkin = false;

        /// <summary>
        /// 是否刷新增值服务按钮名称
        /// </summary>
        public static bool p_IsRefreshSerBtnName = false;

        /// <summary>
        /// 恢复出厂的操作类型 0：恢复出厂数据 1：恢复出厂参数
        /// </summary>
        public static string p_ResetType = "0";

        /// <summary>
        /// 登陆途径方式 0：正常途径 1：异常途径（设备故障 无法退出时）
        /// </summary>
        public static string p_LoginType = "0";

        /// <summary>
        /// 登陆验证结果 True：成功 False：失败
        /// </summary>
        public static bool p_CheckLoginResult = false;

        /// <summary>
        /// 软件设备工作状态 False：异常 True：正常
        /// </summary>
        public static bool p_SoftWorkStatus = false;

        /// <summary>
        /// 广告播放时间
        /// </summary>
        public static DateTime p_AdvertPlayTime = DateTime.Now;

        /// <summary>
        /// 是否正在播放广告 True：是 False：否
        /// </summary>
        public static bool p_IsPlayingAdvert = false;

        /// <summary>
        /// 是否在进行管理设置 True：是 False：否
        /// </summary>
        public static bool p_IsOperManager = false;

        /// <summary>
        /// 选货键盘输入内容 2015-05-04
        /// </summary>
        public static string p_Keyboard_Input_AsileCode = string.Empty;

        /// <summary>
        /// 货道编码验证结果 True：成功 False：失败
        /// </summary>
        public static bool p_CheckKeyAsileCodeResult = false;

        /// <summary>
        /// 当前页要显示的商品数量，只针对商品显示模式和商品类型显示模式
        /// </summary>
        public static int p_ShowGoodsCount = 0;

        /// <summary>
        /// 是否点击了商品类型 False：没有 True：点击
        /// </summary>
        public static bool p_IsClickGoodsType = false;

        /// <summary>
        /// 是否曾经点击过商品类型 False：否 True：有
        /// </summary>
        public static bool p_ClickGoodsType = false;

        /// <summary>
        /// 某操作处理结果 True：成功 False：失败
        /// </summary>
        public static bool p_OperResult = false;

        /// <summary>
        /// 当前点击选择的货柜编号
        /// </summary>
        public static string p_VendBoxCode = string.Empty;

        /// <summary>
        /// 当前点击的网址
        /// </summary>
        public static BusinessEnum.WebUrl p_Now_WebUrl = BusinessEnum.WebUrl.ShangHai_ZhiYuanZhe_XieHui;

        /// <summary>
        /// 当前进行配置的增值服务
        /// </summary>
        public static BusinessEnum.AddServiceType p_Now_Cfg_AddService = BusinessEnum.AddServiceType.BarCode_Take;

        /// <summary>
        /// 上海志愿者的输入键盘类型 0：输入手机号码 1：输入金额
        /// </summary>
        public static string p_ShZyZ_KeyBoard_Type;

        /// <summary>
        /// 上海志愿者爱心捐赠所选择的支付方式
        /// </summary>
        public static BusinessEnum.PayMent p_ShZyZ_Donate_PayType = BusinessEnum.PayMent.No;

        /// <summary>
        /// 是否选择领取赠品 False：不选择 True：选择
        /// </summary>
        public static bool p_ChoiceGift = false;

        public static long p_QueryGiftNum = 0;

        #endregion

        #region 私有变量声明

        /// <summary>
        /// 上次清除日志的日期
        /// </summary>
        private static string m_LogClearDate = string.Empty;

        #endregion

        #region 公共函数

        /// <summary>
        /// 刷新界面语言版本
        /// </summary>
        /// <returns>结果 False：失败 True：成功</returns>
        public static bool RefreshLanguage()
        {
            bool result = false;

            string strLangXmlName = "1";
            switch (p_BusinOper.ConfigInfo.Language)
            {
                case BusinessEnum.Language.English:// 英文
                    strLangXmlName = "0";
                    break;

                case BusinessEnum.Language.Zh_CN:// 中文简体
                    strLangXmlName = "1";
                    break;

                case BusinessEnum.Language.Russian:// 俄文
                    strLangXmlName = "2";
                    break;

                case BusinessEnum.Language.French:// 法文
                    strLangXmlName = "3";
                    break;
            }

            int intErrCode = p_LangOper.Initialize(strLangXmlName);

            if (intErrCode == 0)
            {
                result = true;
            }

            return result;
        }

        public static string ChangeTime(string time)
        {
            if (time.Length != 4)
            {
                return "";
            }

            return time.Substring(0, 2) + ":" + time.Substring(2);
        }

        private static string GetLangXmlName()
        {
            string strLangXmlName = string.Empty;

            switch (p_BusinOper.ConfigInfo.Language)
            {
                case BusinessEnum.Language.English:// 英文
                    strLangXmlName = "en";
                    break;

                case BusinessEnum.Language.Zh_CN:// 中文简体
                    strLangXmlName = "zh-cn";
                    break;

                case BusinessEnum.Language.Russian:// 俄文
                    strLangXmlName = "russian";
                    break;

                case BusinessEnum.Language.French:// 法文
                    strLangXmlName = "french";
                    break;
            }

            return strLangXmlName;
        }

        #region 支付方式文本处理

        /// <summary>
        /// 根据当前可以支持的支付方式获取需要显示的支付方式提示文本
        /// </summary>
        /// <returns></returns>
        ////public static string GetPayTitle()
        ////{
        ////    string strPayTitle = string.Empty;

        ////    p_BusinOper.GetPayType();
            
        ////    switch (p_BusinOper.PayShow)
        ////    {
        ////        case BusinessEnum.PayShow.Pause:// 暂停服务
        ////            strPayTitle = "Err_PauseServer_Sell";// 购物暂停服务
        ////            break;

        ////        case BusinessEnum.PayShow.Cash_Coin_Ic:// 支持纸币、硬币、刷卡
        ////            strPayTitle = "SellGoods_Cash_Coin_Card";// 请投币或刷卡
        ////            break;

        ////        case BusinessEnum.PayShow.Cash_Coin:// 只支持纸币、硬币
        ////            strPayTitle = "SellGoods_Cash_Coin";// 请现金购物
        ////            break;

        ////        case BusinessEnum.PayShow.Cash_Ic:// 只支持纸币、刷卡
        ////            strPayTitle = "SellGoods_Cash_Card";// 请投纸币或刷卡购物
        ////            break;

        ////        case BusinessEnum.PayShow.Coin_Ic:// 只支持硬币、刷卡
        ////            strPayTitle = "SellGoods_Coin_Card";// 请投硬币或刷卡购物
        ////            break;

        ////        case BusinessEnum.PayShow.Cash:// 只支持纸币
        ////            strPayTitle = "SellGoods_Cash";// 请投纸币购物
        ////            break;

        ////        case BusinessEnum.PayShow.Coin:// 只支持硬币
        ////            strPayTitle = "SellGoods_Coin";// 请投硬币购物
        ////            break;

        ////        case BusinessEnum.PayShow.Ic:// 只支持刷卡
        ////            strPayTitle = "SellGoods_Card";// 请刷卡购物
        ////            break;

        ////        default:
        ////            strPayTitle = "Err_PauseServer_Sell";// 购物暂停服务
        ////            break;
        ////    }

        ////    strPayTitle = p_LangOper.GetStringBundle(strPayTitle);
        ////    return strPayTitle;
        ////}

        /// <summary>
        /// 根据当前的支付方式获取需要显示的该支付方式提示文本信息
        /// </summary>
        /// <returns></returns>
        public static string GetPayMentMsgTitle()
        {
            string strPayTitle = string.Empty;

            if (p_BusinOper.PaymentOper.PaymentList.PayShow_IDCode == BusinessEnum.PayShowStatus.Pause)
            {
                #region 2014-11-08修改

                int intEnablePayNum = p_BusinOper.PaymentOper.PaymentList.EnablePayNum;

                if (intEnablePayNum == 1)
                {
                    #region 只有一种支付方式可用

                    // 现金
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_Cash != BusinessEnum.PayShowStatus.Pause)
                    {
                        strPayTitle = GetPayMentTitle_Cash();
                        return strPayTitle;
                    }
                    // 储值卡
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_IC == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = GetPayMentTitle_ICCard();
                        return strPayTitle;
                    }
                    // 非储值卡
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_NoFeeCard == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = GetPayMentTitle_NoFeeCard();
                        return strPayTitle;
                    }
                    // 支付宝付款码
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_AliPay_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = GetPayMentTitle_AliPayCode();
                        return strPayTitle;
                    }
                    // 翼支付付款码
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_BestPay_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = p_LangOper.GetStringBundle("Payment_BestPay");
                        return strPayTitle;
                    }
                    // 京东钱包付款码
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_JDPay_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = p_LangOper.GetStringBundle("Payment_JDPay");
                        return strPayTitle;
                    }
                    // 百度钱包付款码
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_BaiDuPay_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = p_LangOper.GetStringBundle("Payment_BaiDuPay");
                        return strPayTitle;
                    }
                    // 二维码
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_QRCode == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = GetPayMentTitle_QRCode();
                        return strPayTitle;
                    }
                    // 微信支付
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_WeChatCode == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = p_LangOper.GetStringBundle("Payment_WeChatCode");
                        return strPayTitle;
                    }
                    // 银联
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_UnionPay == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = p_LangOper.GetStringBundle("Payment_UnionPay");
                        return strPayTitle;
                    }
                    // 志愿者兑换 
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_Volunteer_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        strPayTitle = p_LangOper.GetStringBundle("Payment_Volunteer");
                        return strPayTitle;
                    }
                    #endregion
                }
                else
                {
                    #region 多种支付方式可用

                    string strDunHao = p_LangOper.GetStringBundle("Pub_Btn_DunHao");
                    string strPayList = string.Empty;

                    #region 现金

                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_Cash == BusinessEnum.PayShowStatus.Cash_Coin)
                    {
                        // 纸币、硬币
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Cash") + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_Cash == BusinessEnum.PayShowStatus.Cash)
                    {
                        // 纸币
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Cash_Bill") + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_Cash == BusinessEnum.PayShowStatus.Coin)
                    {
                        // 硬币
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Cash_Coin") + strDunHao;
                    }

                    #endregion

                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_IC == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 储值卡可用
                        strPayList += ConvertIcCardPayName() + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_AliPay_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 支付宝付款码可用
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_AliPayCode") + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_BestPay_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 翼支付付款码可用
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_BestPay") + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_JDPay_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 京东钱包付款码可用
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_JDPay") + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_BaiDuPay_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 百度钱包付款码可用
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_BaiDuPay") + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_WeChatCode == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 微信扫码可用
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_WeChatCode") + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_NoFeeCard == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 非储值卡可用
                        strPayList += ConvertNoFeeCardPayName() + strDunHao;
                    }
                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_UnionPay == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 银联可用
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_UnionPay") + strDunHao;
                    }

                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_QRCode == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 二维码可用
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_QRCodeCard") + strDunHao;
                    }

                    if (p_BusinOper.PaymentOper.PaymentList.PayShow_Volunteer_Code == BusinessEnum.PayShowStatus.Normal)
                    {
                        // 志愿者兑换码可用
                        strPayList += PubHelper.p_LangOper.GetStringBundle("Pub_Payment_Volunteer") + strDunHao;
                    }

                    if (!string.IsNullOrEmpty(strPayList))
                    {
                        strPayList = strPayList.Substring(0, strPayList.Length - 1);
                    }
                    strPayTitle = p_LangOper.GetStringBundle("Pub_Payment_MsgInfo").Replace("{N}", strPayList);

                    #endregion
                }

                #endregion
            }
            else
            {
                strPayTitle = p_LangOper.GetStringBundle("FreeSell_IDCard_Pay_Progress");
                return strPayTitle;
            }

            ////switch (p_BusinOper.PaymentOper.PaymentList.Now_PayMent)
            ////{
            ////    case BusinessEnum.PayMent.Cash:// 现金
            ////        strPayTitle = GetPayMentTitle_Cash();
            ////        break;
            ////    case BusinessEnum.PayMent.IcCard:// 储值卡
            ////        strPayTitle = GetPayMentTitle_ICCard();
            ////        break;
            ////    case BusinessEnum.PayMent.NoFeeCard:// 磁条卡
            ////        strPayTitle = GetPayMentTitle_NoFeeCard();
            ////        break;
            ////    case BusinessEnum.PayMent.QRCodeCard:// 虚拟会员卡
            ////        strPayTitle = GetPayMentTitle_QRCode();
            ////        break;
            ////    case BusinessEnum.PayMent.AliPay:// 支付宝
            ////        strPayTitle = p_LangOper.GetStringBundle("Payment_AliPay");
            ////        break;
            ////    case BusinessEnum.PayMent.QuickPass:// 银联闪付
            ////        strPayTitle = p_LangOper.GetStringBundle("Payment_UnionPay");
            ////        break;
            ////    default:
            ////        break;
            ////}

            return strPayTitle;
        }

        /// <summary>
        /// 根据当前可以支持的支付方式获取需要显示的支付方式提示文本—现金支付
        /// </summary>
        /// <returns></returns>
        private static string GetPayMentTitle_Cash()
        {
            string strPayTitle = string.Empty;

            switch (p_BusinOper.PaymentOper.PaymentList.PayShow_Cash)
            {
                case BusinessEnum.PayShowStatus.Pause:// 不能现金支付
                    strPayTitle = "Err_PauseServer_Reason_Cash";// 不能现金支付
                    break;

                case BusinessEnum.PayShowStatus.Cash_Coin:// 只支持纸币、硬币
                    strPayTitle = "Payment_Cash_CashCoin";// 请现金购物
                    break;

                case BusinessEnum.PayShowStatus.Cash:// 只支持纸币
                    strPayTitle = "Payment_Cash_Cash";// 请投纸币购物
                    break;

                case BusinessEnum.PayShowStatus.Coin:// 只支持硬币
                    strPayTitle = "Payment_Cash_Coin";// 请投硬币购物
                    break;

                default:
                    strPayTitle = "Err_PauseServer_Reason_Cash";// 不能现金支付
                    break;
            }

            strPayTitle = p_LangOper.GetStringBundle(strPayTitle);
            return strPayTitle;
        }

        /// <summary>
        /// 根据当前可以支持的支付方式获取需要显示的支付方式提示文本—储值卡支付
        /// </summary>
        /// <returns></returns>
        private static string GetPayMentTitle_ICCard()
        {
            string strPayTitle = string.Empty;

            string strPayName = ConvertIcCardPayName();
            switch (p_BusinOper.PaymentOper.PaymentList.PayShow_IC)
            {
                case BusinessEnum.PayShowStatus.Pause:// 不能储值卡支付
                    strPayTitle = "Err_PauseServer_Reason_IC";// 不能储值卡支付
                    break;

                case BusinessEnum.PayShowStatus.Normal:// 可以刷卡
                    strPayTitle = "Payment_ICCard";// 请刷卡购物
                    break;

                default:
                    strPayTitle = "Err_PauseServer_Reason_IC";// 不能储值卡支付
                    break;
            }

            strPayTitle = p_LangOper.GetStringBundle(strPayTitle);
            strPayTitle = strPayTitle.Replace("{N}", strPayName);
            return strPayTitle;
        }

        public static string ConvertIcCardPayName()
        {
            string strPayName = string.Empty;

            switch (p_BusinOper.ConfigInfo.Language)
            {
                case BusinessEnum.Language.English:// 英文
                    strPayName = PubHelper.p_BusinOper.PaymentOper.PaymentList.IC.Name_EN;
                    break;

                case BusinessEnum.Language.Zh_CN:// 中文简体
                    strPayName = PubHelper.p_BusinOper.PaymentOper.PaymentList.IC.Name_CN;
                    break;

                case BusinessEnum.Language.Russian:// 俄文
                    strPayName = PubHelper.p_BusinOper.PaymentOper.PaymentList.IC.Name_RUS;
                    break;
            }
            if (string.IsNullOrEmpty(strPayName))
            {
                strPayName = p_LangOper.GetStringBundle("Pub_Payment_IC");
            }

            return strPayName;
        }

        /// <summary>
        /// 根据当前可以支持的支付方式获取需要显示的支付方式提示文本—非储值卡支付
        /// </summary>
        /// <returns></returns>
        private static string GetPayMentTitle_NoFeeCard()
        {
            string strPayTitle = string.Empty;
            string strPayName = ConvertNoFeeCardPayName();
            switch (p_BusinOper.PaymentOper.PaymentList.PayShow_NoFeeCard)
            {
                case BusinessEnum.PayShowStatus.Pause:// 不能非储值卡支付
                    strPayTitle = "Err_PauseServer_Reason_NoFeeCard";// 不能储值卡支付
                    break;

                case BusinessEnum.PayShowStatus.Normal:// 可以刷卡
                    strPayTitle = "Payment_NoFeeCard";// 请刷卡购物
                    break;

                default:
                    strPayTitle = "Err_PauseServer_Reason_NoFeeCard";// 不能储值卡支付
                    break;
            }

            strPayTitle = p_LangOper.GetStringBundle(strPayTitle);
            strPayTitle = strPayTitle.Replace("{N}", strPayName);
            return strPayTitle;
        }

        public static string ConvertNoFeeCardPayName()
        {
            string strPayName = string.Empty;

            switch (p_BusinOper.ConfigInfo.Language)
            {
                case BusinessEnum.Language.English:// 英文
                    strPayName = PubHelper.p_BusinOper.PaymentOper.PaymentList.NoFeeCard.Name_EN;
                    break;

                case BusinessEnum.Language.Zh_CN:// 中文简体
                    strPayName = PubHelper.p_BusinOper.PaymentOper.PaymentList.NoFeeCard.Name_CN;
                    break;

                case BusinessEnum.Language.Russian:// 俄文
                    strPayName = PubHelper.p_BusinOper.PaymentOper.PaymentList.NoFeeCard.Name_RUS;
                    break;
            }
            if (string.IsNullOrEmpty(strPayName))
            {
                strPayName = p_LangOper.GetStringBundle("Pub_Payment_NoFeeCard");
            }

            return strPayName;
        }

        /// <summary>
        /// 根据当前可以支持的支付方式获取需要显示的支付方式提示文本—二维码支付
        /// </summary>
        /// <returns></returns>
        private static string GetPayMentTitle_QRCode()
        {
            string strPayTitle = string.Empty;

            switch (p_BusinOper.PaymentOper.PaymentList.PayShow_QRCode)
            {
                case BusinessEnum.PayShowStatus.Pause:// 不能支付
                    strPayTitle = "Err_PauseServer_Reason_QrCode";// 不能支付
                    break;

                case BusinessEnum.PayShowStatus.Normal:// 可以
                    strPayTitle = "Payment_QrCodeCard";// 请刷卡购物
                    break;

                default:
                    strPayTitle = "Err_PauseServer_Reason_QrCode";// 不能支付
                    break;
            }

            strPayTitle = p_LangOper.GetStringBundle(strPayTitle);
            return strPayTitle;
        }

        /// <summary>
        /// 根据当前可以支持的支付方式获取需要显示的支付方式提示文本—支付宝付款码
        /// </summary>
        /// <returns></returns>
        private static string GetPayMentTitle_AliPayCode()
        {
            string strPayTitle = string.Empty;

            switch (p_BusinOper.PaymentOper.PaymentList.PayShow_AliPay_Code)
            {
                case BusinessEnum.PayShowStatus.Pause:// 不能支付
                    strPayTitle = "Err_PauseServer_Reason_AliPayCode";// 不能支付
                    break;

                case BusinessEnum.PayShowStatus.Normal:// 可以
                    strPayTitle = "Payment_AliPayCode";// 
                    break;

                default:
                    strPayTitle = "Err_PauseServer_Reason_AliPayCode";// 不能支付
                    break;
            }

            strPayTitle = p_LangOper.GetStringBundle(strPayTitle);
            return strPayTitle;
        }

        #endregion

        #endregion

        public static bool CheckModelTime(string beginTime1,string endTime1,
            string beginTime2,string endTime2,out string errInfo)
        {
            errInfo = string.Empty;
            errInfo = PubHelper.p_LangOper.GetStringBundle("Err_Time_Invalid");

            bool blnIsCheckAll1 = false;
            bool blnIsCheckAll2 = false;

            // 检测所选时间段的有效性
            if ((beginTime1.Length > 0) || (endTime1.Length > 0))
            {
                if (beginTime1.Length != 4)
                {
                    errInfo = errInfo.Replace("{N}", "1");
                    return false;
                }
                if (endTime1.Length != 4)
                {
                    errInfo = errInfo.Replace("{N}", "1");
                    return false;
                }
                if (Convert.ToInt32(beginTime1) >= Convert.ToInt32(endTime1))
                {
                    errInfo = errInfo.Replace("{N}", "1");
                    return false;
                }

                if ((beginTime1.Length == 4) && (endTime1.Length == 4))
                {
                    blnIsCheckAll1 = true;
                }
            }
            else
            {
                beginTime1 = string.Empty;
                endTime1 = string.Empty;
            }

            if ((beginTime2.Length > 0) || (endTime2.Length > 0))
            {
                if (beginTime2.Length != 4)
                {
                    errInfo = errInfo.Replace("{N}", "2");
                    return false;
                }
                if (endTime2.Length != 4)
                {
                    errInfo = errInfo.Replace("{N}", "2");
                    return false;
                }
                if (Convert.ToInt32(beginTime2) >= Convert.ToInt32(endTime2))
                {
                    errInfo = errInfo.Replace("{N}", "2");
                    return false;
                }

                if ((beginTime2.Length == 4) && (endTime2.Length == 4))
                {
                    blnIsCheckAll2 = true;
                }
            }
            else
            {
                beginTime2 = string.Empty;
                endTime2 = string.Empty;
            }

            if ((string.IsNullOrEmpty(beginTime1)) && (string.IsNullOrEmpty(endTime1)) &&
                (string.IsNullOrEmpty(beginTime2)) && (string.IsNullOrEmpty(endTime2)))
            {
                errInfo = errInfo.Replace("{N}", "");
                return false;
            }

            if ((blnIsCheckAll1) && (blnIsCheckAll2))
            {
                // 如果选择了两个定时段，则检查是否存在交叉
                bool blnIsErr = false;
                if (Convert.ToInt32(beginTime2) > Convert.ToInt32(endTime1))
                {
                    blnIsErr = true;
                }
                if (Convert.ToInt32(beginTime1) > Convert.ToInt32(endTime2))
                {
                    blnIsErr = true;
                }
                if (!blnIsErr)
                {
                    errInfo = PubHelper.p_LangOper.GetStringBundle("Err_Time_Repeat");
                    return false;
                }
            }

            return true;
        }

        public static string GetMcdPic(string mcdPicName)
        {
            string strMcdPic = SkinHelper.p_SkinName + "ProductPage/nopapic.png";// 商品图片不存在，采用默认图片

            string strPaPicFile = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\GoodsPic\\" + mcdPicName;

            if (File.Exists(strPaPicFile))
            {
                // 商品图片存在
                strMcdPic = "pack://siteoforigin:,,,/Images/GoodsPic/" + mcdPicName;
            }

            return strMcdPic;
        }

        public static string GetMcdName(string paCode,string mcdName,bool isRefresh)
        {
            // 0：不显示任何内容 1：只显示商品名称 2：只显示货道编号 3：显示商品名称及货道编号

            // 注意：除了0之外，不管其他类型，如果商品名称为空，则都自动显示货道编号
            string strMcdName = string.Empty;

            #region 如果是商品和货道一一对应

            if (p_BusinOper.GoodsShowModelType == BusinessEnum.GoodsShowModelType.GoodsToOnlyAsile)
            {
                if (string.IsNullOrEmpty(mcdName))
                {
                    strMcdName = paCode;
                }
                else
                {
                    switch (p_BusinOper.ConfigInfo.GoodsNameShowType)
                    {
                        case "0":// 不显示任何内容
                            strMcdName = string.Empty;
                            break;

                        case "1":// 只显示商品名称
                            strMcdName = mcdName;
                            break;

                        case "2":// 只显示货道编号
                            strMcdName = paCode;
                            break;

                        case "3":// 显示商品名称及货道编号
                            strMcdName = mcdName + "  【" + paCode + "】";
                            break;

                        default:// 其它，不显示任何内容
                            strMcdName = string.Empty;
                            break;
                    }
                }
            }

            #endregion

            #region 如果是商品图片显示，不和货道一一对应

            if (p_BusinOper.GoodsShowModelType == BusinessEnum.GoodsShowModelType.GoodsToMultiAsile)
            {
                if (string.IsNullOrEmpty(mcdName))
                {
                    strMcdName = mcdName;
                }
                else
                {
                    switch (p_BusinOper.ConfigInfo.GoodsNameShowType)
                    {
                        case "0":// 不显示任何内容
                            strMcdName = string.Empty;
                            break;

                        case "1":// 只显示商品名称
                            strMcdName = mcdName;
                            break;

                        case "2":// 只显示货道编号
                            if (!isRefresh)
                            {
                                strMcdName = string.Empty;
                            }
                            else
                            {
                                strMcdName = paCode;
                            }
                            break;

                        case "3":// 显示商品名称及货道编号
                            if (!isRefresh)
                            {
                                strMcdName = mcdName ;
                            }
                            else
                            {
                                strMcdName = mcdName + "  【" + paCode + "】";
                            }
                            break;

                        default:// 其它，不显示任何内容
                            strMcdName = string.Empty;
                            break;
                    }
                }
            }

            #endregion

            return strMcdName;
        }

        /// <summary>
        /// 转换商品属性显示区域内容
        /// </summary>
        /// <returns></returns>
        public static string ConvertGoodsPropText()
        {
            string strGoodsPropText = string.Empty;

            string strGoodsSpec = string.Empty;
            string strGoodsManuf = string.Empty;
            string strPiCi = string.Empty;
            string strMaxValidDate = string.Empty;

            try
            {
                if (PubHelper.p_BusinOper.AsileOper.CurrentMcdInfo == null)
                {
                    return "";
                }
                strGoodsSpec = p_BusinOper.AsileOper.CurrentMcdInfo.GoodsSpec;
                strGoodsManuf = p_BusinOper.AsileOper.CurrentMcdInfo.Manufacturer;
                strPiCi = p_BusinOper.AsileOper.CurrentMcdInfo.PiCi;
                strMaxValidDate = p_BusinOper.AsileOper.CurrentMcdInfo.MaxValidDate;

                string strGoodsPropShowType = p_BusinOper.ConfigInfo.GoodsPropShowType;
                switch (strGoodsPropShowType)
                {
                    case "0":// 不显示任何内容
                        strGoodsPropText = "";
                        break;
                    case "1":// 只显示商品简介
                        strGoodsPropText = p_BusinOper.AsileOper.CurrentMcdInfo.McdContent;
                        break;
                    case "2":// 只显示商品规格
                        strGoodsPropText = strGoodsSpec;
                        break;
                    case "3":// 只显示生产厂家
                        strGoodsPropText = strGoodsManuf;
                        break;
                    case "4":// 显示商品规格和生产厂家
                    case "5":// 显示生产批次和有效期
                    case "6":// 医药行业显示方式（显示生产厂家、商品规格、生产批次、有效期）
                        string strGuiInfo = string.Empty;
                        string strPiCiInfo = string.Empty;
                        if ((strGoodsPropShowType == "4") || (strGoodsPropShowType == "6"))
                        {
                            if (string.IsNullOrEmpty(strGoodsSpec))
                            {
                                strGuiInfo = strGoodsManuf;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(strGoodsManuf))
                                {
                                    strGuiInfo = strGoodsSpec;
                                }
                                else
                                {
                                    strGuiInfo = strGoodsManuf + "【" + strGoodsSpec + "】";
                                }
                            }
                        }
                        if ((strGoodsPropShowType == "5") || (strGoodsPropShowType == "6"))
                        {
                            strPiCiInfo = p_LangOper.GetStringBundle("Pub_PiCi") + strPiCi +
                                "  " + PubHelper.p_LangOper.GetStringBundle("Pub_MaxValidDate") + strMaxValidDate;
                        }

                        if (strGoodsPropShowType == "4")
                        {
                            strGoodsPropText = strGuiInfo;
                        }
                        if (strGoodsPropShowType == "4")
                        {
                            strGoodsPropText = strPiCiInfo;
                        }
                        if (strGoodsPropShowType == "6")
                        {
                            strGoodsPropText = strGuiInfo + "\r\n" + strPiCiInfo;
                        }

                        break;
                }
            }
            catch
            {

            }

            return strGoodsPropText;
        }

        /// <summary>
        /// 根据商品标示转换商品标示图片
        /// </summary>
        /// <param name="drupType"></param>
        /// <param name="drupPic"></param>
        /// <returns>False：不显示商品标示图片</returns>
        public static bool ConvertDrupType(string drupType, out string drupPic)
        {
            bool result = true;
            drupPic = string.Empty;
            string strPicName = string.Empty;

            switch (drupType)
            {
                case "1":// 1：红色OTC
                    strPicName = "otc_red.png";
                    break;
                case "2":// 2：绿色OTC
                    strPicName = "otc_green.png";
                    break;
                case "3":// 3：保健食品
                    strPicName = "baojian.png";
                    break;
                default:
                    result = false;
                    break;
            }
            if (result)
            {
                string strPicPath = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\FormPic\\pub\\" + strPicName;
                if (File.Exists(strPicPath))
                {
                    drupPic = "pack://siteoforigin:,,,/Images/FormPic/pub/" + strPicName;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool CheckInputPrice(string price,out string _errInfo)
        {
            bool result = false;
            _errInfo = string.Empty;

            if (!p_BusinOper.CheckDataOper.CheckIsPosDouble(price))
            {
                // 金额格式错误
                _errInfo = PubHelper.p_LangOper.GetStringBundle("Err_Input_NoMoney");
                return false ;
            }

            // 如果存在现金支付方式，且现金支付方式处于开启状态
            if (!p_BusinOper.CheckPriceIsMultiple(price))
            {
                // 不能被最小货币面值整除
                _errInfo = PubHelper.p_LangOper.GetStringBundle("Err_Input_NoMoney_Cash");
                string strPoint = (Convert.ToDouble(p_BusinOper.ConfigInfo.MinMoneyValue) / p_BusinOper.ConfigInfo.DecimalNum).ToString();
                _errInfo = _errInfo.Replace("{N}", strPoint);
                return false;
            }
            
            result = true;

            return result;
        }

        public static bool GetFormPubPic(string pubPicName,out string picPath)
        {
            picPath = string.Empty;
            bool result = false;
            string strPicPath = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\FormPic\\pub\\" + pubPicName;
            if (File.Exists(strPicPath))
            {
                picPath = "pack://siteoforigin:,,,/Images/FormPic/pub/" + pubPicName;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 弹出提示窗口
        /// </summary>
        /// <param name="msgInfo"></param>
        /// <param name="msgType"></param>
        public static void ShowMsgInfo(string msgInfo,MsgType msgType)
        {
            PubHelper.p_MsgResult = false;
            FrmMsg frmMsg = new FrmMsg();
            frmMsg.MsgInfo = msgInfo;
            frmMsg.MsgType = msgType;
            frmMsg.ShowDialog();
        }

        /// <summary>
        /// 弹出键盘输入窗口
        /// </summary>
        public static void ShowKeyBoard(string defautInput)
        {
            p_Keyboard_Input = defautInput;
            if (p_BusinOper.ConfigInfo.KeyBoardType == "0")
            {
                // 小键盘
                FrmKeyboard frmKeyBoard = new FrmKeyboard();
                frmKeyBoard.ShowDialog();
            }
            else
            {
                FrmKeyboard_Big frmKeyBoard = new FrmKeyboard_Big();
                frmKeyBoard.ShowDialog();
            }
        }

        public static string ConvertCardNum_IC(string cardNum)
        {
            return ConvertCardNum(cardNum, "0");
        }

        public static string ConvertCardNum_NoFee(string cardNum)
        {
            return ConvertCardNum(cardNum, "1");
        }

        private static string ConvertCardNum(string cardNum,string type)
        {
            string strCardNum = string.Empty;

            try
            {
                if ((string.IsNullOrEmpty(cardNum)) || (cardNum == "0"))
                {
                    return "";
                }
                bool blnIsHide = false;
                switch (type)
                {
                    case "0":// 储值卡号
                        if (p_BusinOper.ConfigInfo.IcCardNumHide == BusinessEnum.ControlSwitch.Run)
                        {
                            blnIsHide = true;
                        }
                        break;
                    case "1":// 会员卡号
                        if (p_BusinOper.ConfigInfo.NoFeeCardNumHide == BusinessEnum.ControlSwitch.Run)
                        {
                            blnIsHide = true;
                        }
                        break;
                }

                if (!blnIsHide)
                {
                    // 卡号信息不用*字显示
                    strCardNum = cardNum;
                }
                else
                {
                    // 卡号信息后3位用*字显示
                    if (cardNum.Length <= 1)
                    {
                        // 卡号长度只有1位，不显示
                        strCardNum = cardNum;
                    }
                    else if (cardNum.Length <= 3)
                    {
                        // 卡号长度小于3位，则只最后一位用*字代替
                        strCardNum = cardNum.Substring(0, cardNum.Length - 1) + "*";
                    }
                    else
                    {
                        // 卡号长度大于3位，则最后3位用*字代替
                        strCardNum = cardNum.Substring(0, cardNum.Length - 3) + "***";
                    }
                }
            }
            catch
            {
                strCardNum = "";
            }

            return strCardNum;
        }

        #region

        #region 获取软件所在盘信息

        public static bool CreatDire(string dire)
        {
            bool result = false;
            try
            {
                if (!Directory.Exists(dire))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(dire);
                }
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        #endregion

        #endregion

        #region 日志记录

        /// <summary>
        /// 记录系统日志信息
        /// </summary>
        /// <param name="logInfo"></param>
        public static void WriteSystemLog(string logInfo)
        {
            try
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Log\\SystemLog.log", DateTime.Now + ": " + logInfo + "\r\n",
                        Encoding.Default);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 删除过期的某个日志文件夹
        /// </summary>
        /// <param name="logDirectory"></param>
        public static void DeleteLogDirectory()
        {
            try
            {
                string strNowDate = DateTime.Now.ToString("yyyyMMdd");
                if (strNowDate == m_LogClearDate)
                {
                    return;
                }

                string intervalDay = PubHelper.p_BusinOper.ConfigInfo.ClearLogIntervalDay;
                int intClearLogIntervalDay = Convert.ToInt32(intervalDay);
                if (intClearLogIntervalDay > 0)
                {
                    string strClearDay = DateTime.Now.AddDays(-intClearLogIntervalDay).ToString("yyyyMMdd");

                    string strLog = string.Empty;
                    string strLogList = @"Log_BarCodeScan,Log_Business,Log_GateWay,Log_Kmb,Log_PostCard,Log_PostCard_Web,
                            Log_RTGateServer,Log_UpDownEqu,Log_NoFeeCard,Log_UnionPay,Log_ThirdCenter,Log_O2OServer,
                            Log_WxTakeCode,Log_OnlinePay,Log_SoftTermUpdate,";
                    string[] hexLogList = strLogList.Split(',');
                    int hexLen = hexLogList.Length;
                    string strAppDire = AppDomain.CurrentDomain.BaseDirectory.ToString();
                    string strLogTempFile = string.Empty;
                    for (int i = 0; i < hexLen; i++)
                    {
                        strLogTempFile = hexLogList[i];
                        if (!string.IsNullOrEmpty(strLogTempFile))
                        {
                            strLog = strAppDire + "Log\\" + strLogTempFile + "\\" + strLogTempFile + "_" + strClearDay;
                            if (Directory.Exists(strLog))
                            {
                                // 存在，删除该文件夹下的所有文件
                                Directory.Delete(strLog, true);
                            }
                        }
                    }
                }

                m_LogClearDate = strNowDate;
            }
            catch
            {
            }
        }

        #endregion

        #region 枚举

        /// <summary>
        /// 弹出窗口类型
        /// </summary>
        public enum MsgType
        {
            Ok,// 确认窗体
            YesNo,// 询问窗体
        }

        #endregion
    }
}
