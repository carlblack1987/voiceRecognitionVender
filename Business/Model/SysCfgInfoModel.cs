#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：配置参数模板
// 创建标识：2014-06-10		谷霖
//-------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class SysCfgInfoModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public SysCfgInfoModel()
        {
            CfgId = string.Empty;
            CfgName = string.Empty;
            CfgFactValue = string.Empty;
            CfgFactoryValue = string.Empty;
            CfgType = string.Empty;
            CfgLevel = string.Empty;
            IsReset = "1";
        }

        /// <summary>
        /// 参数编号
        /// </summary>
        public string CfgId { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string CfgName { get; set; }

        /// <summary>
        /// 参数实际值
        /// </summary>
        public string CfgFactValue { get; set; }

        /// <summary>
        /// 参数出厂值
        /// </summary>
        public string CfgFactoryValue { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public string CfgType { get; set; }

        /// <summary>
        /// 参数级别
        /// </summary>
        public string CfgLevel { get; set; }

        /// <summary>
        /// 能否出厂初始化
        /// </summary>
        public string IsReset { get; set; }
    }
}
