using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Business.Enum;

namespace Business.Model
{
    public class PaymentControlModel : PayMentInfoModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public PaymentControlModel()
        {
            Now_PayMent = BusinessEnum.PayMent.All;
            History_PayMent = BusinessEnum.PayMent.All;
            EnablePayNum = 0;
            Cash = new PayMentInfoModel();
            IC = new PayMentInfoModel();
            WeChatCode = new PayMentInfoModel();
            AliPay_Code = new PayMentInfoModel();
            BestPay_Code = new PayMentInfoModel();
            JDPay_Code = new PayMentInfoModel();
            BaiDuPay_Code = new PayMentInfoModel();
            UnionPay = new PayMentInfoModel();
            NoFeeCard = new PayMentInfoModel();
            QRCodeCard = new PayMentInfoModel();
            Volunteer_Code = new PayMentInfoModel();
            Cash.Pic = "cash.png";
            IC.Pic = "ic.png";
            WeChatCode.Pic = "alipaywave.png";
            AliPay_Code.Pic = "alipaycode.png";
            BestPay_Code.Pic = "bestpay.png";
            JDPay_Code.Pic = "jdpay.png";
            BaiDuPay_Code.Pic = "baidupay.png";
            UnionPay.Pic = "unionpay.png";
            NoFeeCard.Pic = "nofeecard.png";
            QRCodeCard.Pic = "qrcodecard.png";
        }

        /// <summary>
        /// 当前选择的支付方式
        /// </summary>
        public BusinessEnum.PayMent Now_PayMent { get; set; }

        /// <summary>
        /// 历史选择的支付方式
        /// </summary>
        public BusinessEnum.PayMent History_PayMent { get; set; }

        /// <summary>
        /// 当前支持的支付方式数量
        /// </summary>
        public int EnablePayNum { get; set; }

        /// <summary>
        /// 现金支付方式
        /// </summary>
        public PayMentInfoModel Cash { get; set; }

        /// <summary>
        /// 现金支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_Cash = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 储值卡支付方式
        /// </summary>
        public PayMentInfoModel IC { get; set; }

        /// <summary>
        /// 储值卡支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_IC = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 微信扫码支付方式
        /// </summary>
        public PayMentInfoModel WeChatCode { get; set; }

        /// <summary>
        /// 微信扫码支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_WeChatCode = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 支付宝付款码支付方式
        /// </summary>
        public PayMentInfoModel AliPay_Code { get; set; }

        /// <summary>
        /// 支付宝付款码支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_AliPay_Code = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 翼支付付款码支付方式
        /// </summary>
        public PayMentInfoModel BestPay_Code { get; set; }

        /// <summary>
        /// 翼支付付款码支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_BestPay_Code = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 京东钱包支付方式
        /// </summary>
        public PayMentInfoModel JDPay_Code { get; set; }

        /// <summary>
        /// 京东钱包支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_JDPay_Code = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 百度钱包支付方式
        /// </summary>
        public PayMentInfoModel BaiDuPay_Code { get; set; }

        /// <summary>
        /// 百度钱包支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_BaiDuPay_Code = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 志愿者兑换支付方式
        /// </summary>
        public PayMentInfoModel Volunteer_Code { get; set; }

        /// <summary>
        /// 志愿者兑换支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_Volunteer_Code = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 银联闪付支付方式
        /// </summary>
        public PayMentInfoModel UnionPay { get; set; }

        /// <summary>
        /// 银联闪付支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_UnionPay = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 磁条卡支付方式
        /// </summary>
        public PayMentInfoModel NoFeeCard { get; set; }

        /// <summary>
        /// 磁条卡支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_NoFeeCard = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 虚拟会员卡支付方式（二维码）
        /// </summary>
        public PayMentInfoModel QRCodeCard { get; set; }

        /// <summary>
        /// 虚拟会员卡支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_QRCode = BusinessEnum.PayShowStatus.Pause;

        /// <summary>
        /// 身份证支付方式的类型
        /// </summary>
        public BusinessEnum.PayShowStatus PayShow_IDCode = BusinessEnum.PayShowStatus.Pause;
    }
}
