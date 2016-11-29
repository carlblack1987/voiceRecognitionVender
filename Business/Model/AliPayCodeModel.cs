#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：条形码扫描在线支付信息模板
// 创建标识：2015-01-16		谷霖
//-------------------------------------------------------------------------------------
#endregion

namespace Business.Model
{
    public class BarCodeNetPayModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public BarCodeNetPayModel()
        {
            PayCode = string.Empty;
            PayNum = string.Empty;
            PayType = string.Empty;
            Money = 0;
        }

        /// <summary>
        /// 条形码
        /// </summary>
        public string PayCode { get; set; }

        /// <summary>
        /// 支付账号
        /// </summary>
        public string PayNum { get; set; }

        /// <summary>
        /// 支付类型
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 扣款金额
        /// </summary>
        public int Money { get; set; }
    }
}
