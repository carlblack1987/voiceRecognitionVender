#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：O2O业务处理类
// 创建标识：2015-08-06		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using O2OServer;

namespace Business.Common
{
    public class O2OHelper
    {
        private O2OOper m_O2OOper = new O2OOper();

        #region 属性

        /// <summary>
        /// O2O服务URL地址
        /// </summary>
        public string ServerURL
        {
            get { return m_O2OOper.ServerURL; }
            set { m_O2OOper.ServerURL = value; }
        }

        /// <summary>
        /// 接入UserKey
        /// </summary>
        public string UserKey
        {
            get { return m_O2OOper.UserKey; }
            set { m_O2OOper.UserKey = value; }
        }

        /// <summary>
        /// 机器编号
        /// </summary>
        public string VmID
        {
            get { return m_O2OOper.VmID; }
            set { m_O2OOper.VmID = value; }
        }

        private bool m_IsSendWaitData = false;
        /// <summary>
        /// 是否发送待发数据
        /// </summary>
        public bool IsSendWaitData
        {
            get { return m_O2OOper.IsSendWaitData; }
            set { m_O2OOper.IsSendWaitData = value; }
        }

        #endregion

        #region 业务函数接口

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            return m_O2OOper.Init();
        }

        /// <summary>
        /// 关闭服务器通信
        /// </summary>
        public void Displose()
        {
            m_O2OOper.Displose();
        }

        /// <summary>
        /// 查询订单信息
        /// </summary>
        /// <param name="orderID">订单编号</param>
        /// <param name="o_orderInfo">订单信息，订单编号|商品编号|商品数量</param>
        /// <returns>结果</returns>
        public int QueryOrderInfo(string orderID, out string o_orderInfo)
        {
            o_orderInfo = string.Empty;
            return m_O2OOper.QueryOrderInfo(orderID, out o_orderInfo);
        }

        public int PostVolunteerPay(string exCode, string asileCode, string goodsCode, string money)
        {
            return m_O2OOper.PostVolunteerPay(exCode, asileCode, goodsCode, money);
        }

        /// <summary>
        /// 出货请求确认
        /// </summary>
        /// <param name="orderID"订单编号></param>
        /// <param name="sellStatus">出货状态 0：可吐货 1：不能吐货</param>
        /// <param name="noSellReason">不能吐货原因代码 0：商品不存在 1：商品库存不足 2：商品所在货道故障 3：其它</param>
        /// <returns>结果</returns>
        public int ConfirmSellInfo(string orderID, string sellStatus, string noSellReason)
        {
            return m_O2OOper.ConfirmSellInfo(orderID, sellStatus, noSellReason);
        }

        /// <summary>
        /// 出货结果确认
        /// </summary>
        /// <param name="orderID">订单编号</param>
        /// <param name="sellStatus">出货结果 0：出货失败 1：出货成功</param>
        /// <param name="noSellReason">不能吐货原因代码 0：商品不存在 1：商品库存不足 2：商品所在货道故障 3：其它</param>
        /// <returns></returns>
        public int PostSellResult(string orderID, string sellStatus, string noSellReason)
        {
            return m_O2OOper.PostSellResult(orderID, sellStatus, noSellReason);
        }

        /// <summary>
        /// 身份证订单查询
        /// </summary>
        /// <param name="idCard">身份证</param>
        /// <param name="name">姓名</param>
        /// <param name="sex">性别</param>
        /// <param name="ethnic">民族</param>
        /// <param name="birthday">生日</param>
        /// <param name="address">地址</param>
        /// <param name="issuance_organ">签发机关</param>
        /// <param name="validity_period">有效期</param>
        /// <param name="o_orderInfo">订单信息，订单编号|商品编号|商品数量</param>
        /// <returns></returns>
        public int QueryIDOrderInfo(string idCard, string name, string sex, string ethnic, string birthday,
            string address, string issuance_organ, string validity_period, out string o_orderInfo)
        {
            o_orderInfo = string.Empty;
            return m_O2OOper.QueryIDOrderInfo(idCard, name, sex, ethnic, birthday,
                address, issuance_organ, validity_period, out o_orderInfo);
        }

        /// <summary>
        /// 温度上报函数
        /// </summary>
        /// <param name="vendBox">所属柜子编号</param>
        /// <param name="tmpValue">温度值</param>
        /// <returns>结果代码</returns>
        public int Report_Tmp(string vendBox, string tmpValue)
        {
            return m_O2OOper.Report_Tmp(vendBox, tmpValue);
        }

        /// <summary>
        /// 商品补货数据上报
        /// </summary>
        /// <param name="mcdCode">商品编号</param>
        /// <param name="num">补货后数量</param>
        /// <param name="paCode">补货货道</param>
        /// <param name="productDate">生产日期</param>
        /// <param name="validDate">有效期至</param>
        /// <param name="piCi">生产批次</param>
        /// <returns>结果代码</returns>
        public int Report_Stock(string mcdCode, string num, string paCode, string productDate, string validDate, string piCi)
        {
            return m_O2OOper.Report_Stock(mcdCode, num, paCode, productDate, validDate, piCi);
        }

        /// <summary>
        /// 出货结果上报
        /// </summary>
        /// <param name="serNo">业务交易流水号</param>
        /// <param name="mcdCode">商品编号</param>
        /// <param name="num">出货数量</param>
        /// <param name="price">商品售价</param>
        /// <param name="sellTime">出货时间</param>
        /// <param name="paCode">出货货道编号</param>
        /// <param name="payType">支付类型</param>
        /// <param name="payAccount">支付账号</param>
        /// <param name="payAmount">交易金额</param>
        /// <returns>结果代码</returns>
        public int Report_SellResult(string serNo, string mcdCode, string num, string price, string sellTime, string paCode, string payType,
            string payAccount, string payAmount)
        {
            return m_O2OOper.Report_SellResult(serNo, mcdCode, num, price, sellTime, paCode, payType,
            payAccount, payAmount);
        }

        /// <summary>
        /// 捐赠数据上报
        /// </summary>
        /// <param name="donMoney">捐赠金额</param>
        /// <param name="donType">捐赠方式 1：现金 2：支付宝 3：微信</param>
        /// <param name="mobilePhone">捐赠者手机号码</param>
        /// <param name="donAccount"></param>
        /// <param name="thirdSerno"></param>
        /// <returns></returns>
        public int Report_DonData(string donMoney, string donType,
            string mobilePhone, string donAccount, string thirdSerno)
        {
            return m_O2OOper.Report_DonData(donMoney, donType, mobilePhone, donAccount, thirdSerno);
        }

        /// <summary>
        /// 获取待发数据总数量
        /// </summary>
        /// <returns>总数量</returns>
        public int GetNetDataCount()
        {
            return m_O2OOper.GetNetDataCount();
        }

        #endregion
    }
}
