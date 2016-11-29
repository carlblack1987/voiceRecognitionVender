#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：微信取货码业务处理类
// 创建标识：2015-08-31		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KimmaTakeCode;

namespace Business.Common
{
    public class WxTakeCodeHelper
    {
        private TakeCodeServer m_TakeCodeServer = new TakeCodeServer();

        #region 属性

        /// <summary>
        /// Web服务URL地址
        /// </summary>
        public string ServerURL
        {
            get { return m_TakeCodeServer.ServerURL; }
            set { m_TakeCodeServer.ServerURL = value; }
        }

        /// <summary>
        /// 机器编号
        /// </summary>
        public string VmID
        {
            get { return m_TakeCodeServer.VmID; }
            set { m_TakeCodeServer.VmID = value; }
        }

        public string UserKey
        {
            get { return m_TakeCodeServer.UserKey; }
            set { m_TakeCodeServer.UserKey = value; }
        }

        #endregion

        #region 业务函数接口

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public int Init()
        {
            return m_TakeCodeServer.Init();
        }

        /// <summary>
        /// 验证取货码信息
        /// </summary>
        /// <param name="codeNum">取货码号</param>
        /// <param name="o_Info">取货码信息，取货码|取货码类型|条码来源|价值金额|取货方式|取货货道号</param>
        /// <returns>结果</returns>
        public int CheckTakeCodeInfo(string codeNum, out string o_Info)
        {
            o_Info = string.Empty;
            return m_TakeCodeServer.CheckTakeCodeInfo(codeNum, out o_Info);
        }

        /// <summary>
        /// 出货请求确认
        /// </summary>
        /// <param name="orderID"订单编号></param>
        /// <param name="sellStatus">出货状态 0：可吐货 1：不能吐货</param>
        /// <param name="noSellReason">不能吐货原因代码 0：商品不存在 1：商品库存不足 2：商品所在货道故障 3：其它</param>
        /// <returns>结果</returns>
        public int ConfirmSellInfo(string codeNum, string sellStatus, string noSellReason)
        {
            return m_TakeCodeServer.ConfirmSellInfo(codeNum, sellStatus, noSellReason);
        }

        #endregion
    }
}
