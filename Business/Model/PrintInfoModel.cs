#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：购物单据打印信息模板
// 创建标识：2015-06-08		谷霖
//-------------------------------------------------------------------------------------
#endregion

namespace Business.Model
{
    public class PrintInfoModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public PrintInfoModel()
        {
            SerNo = string.Empty;
            GoodsName = string.Empty;
            GoodsCode = string.Empty;
            GoodsPiCi = string.Empty;
            GoodsPrice = string.Empty;
            Money = string.Empty;
            BuyTime = string.Empty;
            TermCode = string.Empty;
            PayMent = string.Empty;
            CardNum = string.Empty;
            BuyNum = 0;
            Manufacturer = string.Empty;
            GoodsSpec = string.Empty;
            ProductDate = string.Empty;
            MaxValidDate = string.Empty;
        }

        /// <summary>
        /// 交易序号
        /// </summary>
        public string SerNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 商品批次
        /// </summary>
        public string GoodsPiCi { get; set; }

        /// <summary>
        /// 商品价格
        /// </summary>
        public string GoodsPrice { get; set; }

        /// <summary>
        /// 消费金额
        /// </summary>
        public string Money { get; set; }

        /// <summary>
        /// 购买时间
        /// </summary>
        public string BuyTime { get; set; }

        /// <summary>
        /// 购买地点
        /// </summary>
        public string TermCode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMent { get; set; }

        /// <summary>
        /// 消费卡号
        /// </summary>
        public string CardNum { get; set; }

        /// <summary>
        /// 购买数量（成功）
        /// </summary>
        public int BuyNum { get; set; }

        /// <summary>
        /// 生产厂商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 商品规格
        /// </summary>
        public string GoodsSpec { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public string ProductDate { get; set; }

        /// <summary>
        /// 最大有效期
        /// </summary>
        public string MaxValidDate { get; set; }
    }
}
