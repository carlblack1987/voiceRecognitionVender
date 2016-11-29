#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：货道数据模板
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
    public class AsileModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public AsileModel()
        {
            VendBoxCode = "1";
            PaId = string.Empty;
            PaCode = string.Empty;
            TrayIndex = 0;
            RowIndex = 0;
            ColumnIndex = 0;
            SellPrice = "0";
            SpringNum = 0;
            PaStackNum = 0;
            SurNum = 0;
            PaKind = "0";
            PaStatus = "01";
            McdCode = string.Empty;
            McdName = string.Empty;
            McdPicName = string.Empty;
            PaAdvertFile = string.Empty;
            IsNew = false;
            Unit = string.Empty;
            TestNum = 0;
            IsQueryStatus = false;
            IsFree = "0";
            PiCi = string.Empty;
            ProductDate = string.Empty;
            Manufacturer = string.Empty;
            GoodsSpec = string.Empty;
            MaxValidDate = string.Empty;
            DrugType = "0";
            SaleNum = 0;
            SaleMoney = 0;
            DetailInfo = string.Empty;
            SellModel = BusinessEnum.AsileSellModel.Normal;
            McdSaleType = BusinessEnum.McdSaleType.Normal;
        }

        /// <summary>
        /// 所属机柜，默认机柜1
        /// </summary>
        public string VendBoxCode { get; set; }

        /// <summary>
        /// 货道内部编号
        /// </summary>
        public string PaId { get; set; }

        /// <summary>
        /// 货道外部编号
        /// </summary>
        public string PaCode { get; set; }

        /// <summary>
        /// 所在托盘编号，从上到下为0、1、2等
        /// </summary>
        public int TrayIndex { get; set; }

        /// <summary>
        /// 所在行
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 所在列
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public string SellPrice { get; set; }

        /// <summary>
        /// 弹簧圈数
        /// </summary>
        public int SpringNum { get; set; }

        /// <summary>
        /// 货道容量
        /// </summary>
        public int PaStackNum { get; set; }

        /// <summary>
        /// 货道商品余量
        /// </summary>
        public int SurNum { get; set; }

        /// <summary>
        /// 货道服务状态 0：正常服务 1：人工暂停服务（该货道正常安装） 2：不存在该货道（电机未安装） 3：出厂状态（未设置价格）
        /// </summary>
        public string PaKind { get; set; }

        /// <summary>
        /// 货道运行状态 0：正常 其它：故障
        /// </summary>
        public string PaStatus { get; set; }

        /// <summary>
        /// 是否已经查询了货道运行状态 False：否 True：是
        /// </summary>
        public bool IsQueryStatus { get; set; }

        /// <summary>
        /// 货道商品编码
        /// </summary>
        public string McdCode { get; set; }

        /// <summary>
        /// 货道商品名称
        /// </summary>
        public string McdName { get; set; }

        /// <summary>
        /// 货道商品介绍
        /// </summary>
        public string McdContent { get; set; }

        /// <summary>
        /// 货道商品图片名称
        /// </summary>
        public string McdPicName { get; set; }

        /// <summary>
        /// 货道商品广告图片名称
        /// </summary>
        public string PaAdvertFile { get; set; }

        /// <summary>
        /// 商品单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 是否为新品 False：否 True：是
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 货道测试次数
        /// </summary>
        public int TestNum { get; set; }

        /// <summary>
        /// 是否免费赠送 0：不免费 1：免费
        /// </summary>
        public string IsFree { get; set; }

        /// <summary>
        /// 生产批次
        /// </summary>
        public string PiCi { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public string ProductDate { get; set; }

        /// <summary>
        /// 生产厂商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 商品规格
        /// </summary>
        public string GoodsSpec { get; set; }

        /// <summary>
        /// 最大有效期
        /// </summary>
        public string MaxValidDate { get; set; }

        /// <summary>
        /// 商品标示类型（主要针对药品） 0：普通商品 1：红色OTC 2：绿色OTC 3：保健食品
        /// </summary>
        public string DrugType { get; set; }

        /// <summary>
        /// 销售次数
        /// </summary>
        public int SaleNum { get; set; }

        /// <summary>
        /// 销售金额
        /// </summary>
        public int SaleMoney { get; set; }

        /// <summary>
        /// 商品详细说明
        /// </summary>
        public string DetailInfo { get; set; }

        /// <summary>
        /// 货道销售类型 0：正常销售 1：赠品
        /// </summary>
        public BusinessEnum.AsileSellModel SellModel { get; set; }

        /// <summary>
        /// 货道商品销售类型
        /// </summary>
        public BusinessEnum.McdSaleType McdSaleType { get; set; }
    }
}
