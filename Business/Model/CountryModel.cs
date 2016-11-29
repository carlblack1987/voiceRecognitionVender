#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：国家信息模板
// 创建标识：2014-12-12		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class CountryModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public CountryModel()
        {
            CountryCode = string.Empty;
            CountryName_ZN = string.Empty;
            CountryName_EN = string.Empty;
            DecimalNum = "100";
            MinMoneyValue = "50";
            MoneySymbol = "¥";// 默认为人民币符号
        }

        /// <summary>
        /// 国家编号
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 国家中文名称
        /// </summary>
        public string CountryName_ZN { get; set; }

        /// <summary>
        /// 国家英文名称
        /// </summary>
        public string CountryName_EN { get; set; }

        /// <summary>
        /// 金额小数点位数
        /// </summary>
        public string DecimalNum { get; set; }

        /// <summary>
        /// 最小货币面值（指售货机上能接收的最小货币）
        /// </summary>
        public string MinMoneyValue { get; set; }

        /// <summary>
        /// 国家货币符号
        /// </summary>
        public string MoneySymbol { get; set; }
    }
}
