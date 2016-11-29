#region [ KIMMA Co.,Ltd. Copyright (C) 2014 ]
//-------------------------------------------------------------------------------------
// 开发单位：湖南金码智能设备制造有限公司
// 业务模块：iVend终端软件业务处理类
// 业务功能：设备控制数据模板
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
    public class DeviceControlModel
    {
        /// <summary>
        /// 控制模式 1：全时段开启（默认） 2：全时段关闭 0：定时开启
        /// </summary>
        public string ControlModel { get; set; }

        /////// <summary>
        /////// 目标温度，该参数只针对于制冷压缩机策略使用
        /////// </summary>
        ////public string TargetTmp { get; set; }

        /// <summary>
        /// 开始时间1
        /// </summary>
        public string BeginTime1 { get; set; }

        /// <summary>
        /// 结束时间1
        /// </summary>
        public string EndTime1 { get; set; }

        /// <summary>
        /// 开始时间2
        /// </summary>
        public string BeginTime2 { get; set; }

        /// <summary>
        /// 结束时间2
        /// </summary>
        public string EndTime2 { get; set; }

        /// <summary>
        /// 开始时间3
        /// </summary>
        public string BeginTime3 { get; set; }

        /// <summary>
        /// 结束时间3
        /// </summary>
        public string EndTime3 { get; set; }

        /// <summary>
        /// 最后一次关闭时间
        /// </summary>
        public DateTime LastCloseTime { get; set; }

        /// <summary>
        /// 最后一次开启时间
        /// </summary>
        public DateTime LastOpenTime { get; set; }

        /// <summary>
        /// 打开最长时间，以分钟为单位
        /// </summary>
        public int OpenMaxTime { get; set; }

        /// <summary>
        /// 关闭后再次打开的延长时间，以分钟为单位
        /// </summary>
        public int CloseDelayTime { get; set; }

        /// <summary>
        /// 设备当前控制运行状态
        /// </summary>
        public BusinessEnum.DeviceControlStatus ControlStatus { get; set; }

        /// <summary>
        /// 是否刷新当前该设备控制策略参数
        /// </summary>
        public bool IsRefreshCfg { get; set; }

        /// <summary>
        /// 设备启动后的运行时间，以分钟为单位
        /// </summary>
        public string OpenTotalTime { get; set; }
    }
}
