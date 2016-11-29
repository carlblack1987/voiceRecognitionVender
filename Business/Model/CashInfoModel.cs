#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：货币信息模板
// 创建标识：2015-07-23		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class CashInfoModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public CashInfoModel()
        {
            CashValue = 100;
            StockNum = 0;
            CashType = "0";// 0：硬币 1：纸币
            Channel = 1;// 
            Status = "1";
            BoxStockNum = 0;// 溢币盒库存量或纸币找零箱库存量，溢币盒针对普通硬币器，纸币找零箱库存量针对找零纸币器
        }

        /// <summary>
        /// 货币面值
        /// </summary>
        public int CashValue { get; set; }

        /// <summary>
        /// 货币库存数量，如果是硬币，则指币筒或Hopper里的库存量；如果是纸币，则指钞箱里的库存量
        /// </summary>
        public int StockNum { get; set; }

        /// <summary>
        /// 货币类型 0：硬币 1：纸币
        /// </summary>
        public string CashType { get; set; }

        /// <summary>
        /// 货币通道，主要针对纸币
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// 状态 0：关闭接收 1：开启接收
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 溢币盒库存量或纸币找零箱库存量，溢币盒针对普通硬币器，纸币找零箱库存量针对找零纸币器
        /// </summary>
        public int BoxStockNum { get; set; }
    }
}
