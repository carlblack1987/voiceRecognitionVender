using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Model
{
    public class DeviceStatusModel
    {
        /// <summary>
        /// 析构
        /// </summary>
        public DeviceStatusModel()
        {
            Status = "01";
            DelayTime = DateTime.Now;
        }

        /// <summary>
        /// 设备当前状态
        /// </summary>
        public string Status { get; set; }

        public DateTime DelayTime { get; set; }
    }
}
