using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoSellGoodsMachineBusiness.Entity
{
    /// <summary>
    /// 商品配置类
    /// </summary>
    public class McdInfo
    {
        /// <summary>
        /// 唯一编号（如A1等）
        /// </summary>
        public string PaCode { get; set; }

        /// <summary>
        /// 所在列
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 所在行
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 商品所跨列
        /// </summary>
        public int ColumnSpan { get; set; }

        /// <summary>
        /// 图片文件(仅有文件名，不包含路径，如A1.png)
        /// </summary>
        public string McdPicName { get; set; }

        /// <summary>
        /// 商品销售价格
        /// </summary>
        public float SellPrice { get; set; }

        /// <summary>
        /// 单位（例如：元/瓶）
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 是否为新品
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 当前数量
        /// </summary>
        public int McdCount { get; set; }
    }
}
