using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Business.Enum;

namespace Business.Model
{
    public class PayMentInfoModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public PayMentInfoModel()
        {
            ControlSwitch = BusinessEnum.ControlSwitch.Stop;
            Name = string.Empty;
            Name_CN = string.Empty;
            Name_EN = string.Empty;
            Name_RUS = string.Empty;
            Pic = string.Empty;
        }

        /// <summary>
        /// 支付方式开关 0：关闭 1：开启
        /// </summary>
        public BusinessEnum.ControlSwitch ControlSwitch { get; set; }

        /// <summary>
        /// 支付方式当前显示名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 支付方式中文名称
        /// </summary>
        public string Name_CN { get; set; }

        /// <summary>
        /// 支付方式英文名称
        /// </summary>
        public string Name_EN { get; set; }

        /// <summary>
        /// 支付方式俄文名称
        /// </summary>
        public string Name_RUS { get; set; }

        /// <summary>
        /// 支付方式图片
        /// </summary>
        public string Pic{get;set;}
    }
}
