#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：商品数据模板
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business.Enum;

namespace Business.Model
{
    public class GoodsModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public GoodsModel()
        {
            McdCode = string.Empty;
            McdName = string.Empty;
            McdContent = string.Empty;
            Price = 0;
            PicName = string.Empty;
            SurNum = 0;
            RowIndex = 0;
            ColumnIndex = 0;
            PageNo = "1";
            Unit = string.Empty;
            IsFree = "0";
            Manufacturer = string.Empty;
            GoodsSpec = string.Empty;
            TypeCode = string.Empty;
            DrugType = "0";
            DetailInfo = string.Empty;
            McdSaleType = BusinessEnum.McdSaleType.Normal;
        }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string McdCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string McdName { get; set; }

        /// <summary>
        /// 商品介绍
        /// </summary>
        public string McdContent { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string PicName { get; set; }

        /// <summary>
        /// 商品单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 所在行
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 所在列
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 商品余量
        /// </summary>
        public int SurNum { get; set; }

        /// <summary>
        /// 商品所在页序号
        /// </summary>
        public string PageNo { get; set; }

        /// <summary>
        /// 是否免费赠送 0：不免费 1：免费
        /// </summary>
        public string IsFree { get; set; }

        /// <summary>
        /// 生产厂商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 商品规格
        /// </summary>
        public string GoodsSpec { get; set; }

        /// <summary>
        /// 商品类别
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 商品标示类型（主要针对药品） 0：普通商品 1：红色OTC 2：绿色OTC 3：保健食品
        /// </summary>
        public string DrugType { get; set; }

        /// <summary>
        /// 商品详细说明
        /// </summary>
        public string DetailInfo { get; set; }

        /// <summary>
        /// 商品销售类型 0：正常销售 1：热点商品 2：新品 3：打折商品 4：免费商品
        /// </summary>
        public BusinessEnum.McdSaleType McdSaleType { get; set; }
    }
}
