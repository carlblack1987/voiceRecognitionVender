#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：商品类型数据模板
// 创建标识：2015-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class GoodsTypeModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public GoodsTypeModel()
        {
            TypeCode = string.Empty;
            TypeName = string.Empty;
            GoodsCount = 0;
        }

        /// <summary>
        /// 商品类别编码
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 商品类别名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 该类别下商品数量
        /// </summary>
        public int GoodsCount { get; set; }
    }
}
